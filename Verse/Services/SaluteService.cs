using System;
using System.Collections.Generic;
using System.Linq;
using Nestor;
using Nestor.Models;
using Verse.Abstract;
using Verse.Helpers;
using Verse.Models;
using Verse.Models.Salute.Web;

namespace Verse.Services
{
    public class SaluteService
    {
        private readonly IStateStorage<UserState> _userStateStorage;

        private static readonly string[] Suggestions =
        {
            ""
        };
        private readonly Random _random = new();
        private NestorMorph _nestor;
        private readonly HashSet<string> _vowels = new() {"а", "о", "у", "ы", "э", "я", "ё", "ю", "и", "е"};

        public SaluteService(
            IStateStorage<UserState> userStateStorage
        )
        {
            _userStateStorage = userStateStorage;
        }

        public void Init()
        {
            _nestor = new NestorMorph();
        }
        
        public SaluteResponse Handle(SaluteRequest request)
        {
            if (request.IsEnter)
                return Enter(request);
            
            if (request.HasWords("выход", "выйти", "закрыть", "закрой"))
                return Exit(request);

            if (request.Tokens.Length <= 3)
            {
                if (request.HasWords("помощь", "что уметь", "что мочь"))
                    return Help(request);

                /*foreach (FootType footType in Enum.GetValues<FootType>())
                {
                    if (footType == FootType.Unknown) continue;
                    var foot = new Foot(footType);

                    if (request.HasWords(foot.Name))
                        return TellAbout(foot, request);
                }*/

                return TooShort(request);
            }

            return TryParseFoot(request);
        }

        private SaluteResponse Exit(SaluteRequest request)
        {
            var response = new SaluteResponse(request);
            response
                .AppendText(request, new Phrase("До свидания.", "Закрываю приложение.", "Пока!"))
                .AppendSendData("close", "");

            response.Payload.Finished = true;

            return response;
        }

        private SaluteResponse Enter(SaluteRequest request)
        {
            UserState state = _userStateStorage.GetState(request.UserId);
            TimeSpan timeLeft = DateTime.UtcNow - state.LastEnter;

            var response = new SaluteResponse(request);
            Phrase phrase = timeLeft.TotalDays < 30
                ? new Phrase("1", "", "")
                : new Phrase("", "", "");

            response
                .AppendText(request, phrase)
                .AppendSuggestions(RandomSuggestion(), "Выход");

            state.LastEnter = DateTime.UtcNow;
            _userStateStorage.SetState(request.UserId, state);
            
            return response;
        }

        private SaluteResponse TryParseFoot(SaluteRequest request)
        {
            throw new NotImplementedException();
        }

        private ParseResult GetParseResult(string[] tokens)
        {
            int[] stresses = tokens.SelectMany(GetStresses).ToArray();

            // find best foot
            Foot bestFoot = null;
            var bestDistance = int.MaxValue;
            foreach (FootType footType in Enum.GetValues<FootType>())
            {
                var foot = new Foot(footType);
                int dist = DistanceToFoot(foot, stresses);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestFoot = foot;
                }
            }

            if (bestFoot == null)
                throw new InvalidOperationException();
            
            // distribute syllables
            List<int> needStresses = bestFoot.GetMaskOfLength(stresses.Length).ToList();
            Syllable[][] syllables = tokens.Select(t => SplitWord(t, needStresses)).ToArray();

            return new ParseResult
            {
                FootType = bestFoot.Type,
                Syllables = syllables
            };
        }

        private Syllable[] SplitWord(string word, List<int> stresses)
        {
            var syllables = new List<Syllable>();
            var lastConsonant = "";
            
            void AddConsonant()
            {
                syllables.Add(new Syllable
                {
                    Text = lastConsonant,
                    Type = SyllableType.Consonant
                });
                lastConsonant = "";
            }
            
            foreach (string c in word.Select(t => t.ToString()))
            {
                if (_vowels.Contains(c))
                {
                    if (!string.IsNullOrEmpty(lastConsonant)) 
                        AddConsonant();
                }
                else
                {
                    int nextStress = stresses.First();
                    stresses.RemoveAt(0);
                    syllables.Add(new Syllable
                    {
                        Text = c,
                        Type = nextStress == 1 ? SyllableType.Stressed : SyllableType.Unstressed
                    });
                }
            }

            return syllables.ToArray();
        }

        private int DistanceToFoot(Foot foot, int[] stresses)
        {
            int[] mask = foot.GetMaskOfLength(stresses.Length);
            var dist = 0;
            for (var i = 0; i < mask.Length; i++)
            {
                int wordStress = stresses[i];
                int maskStress = mask[i];
                if (wordStress != -1 && wordStress != maskStress)
                    dist++;
            }

            return dist;
        }

        private int[] GetStresses(string word)
        {
            int vCount = word.Count(l => _vowels.Contains(l.ToString()));
            
            // if there are no vowels, return empty array
            if (vCount == 0)
                return Array.Empty<int>();
            
            // with one vowel return single undefined accent
            if (vCount == 1)
                return new[] {-1};

            var knownAccents = new HashSet<int>();
            
            Word[] wordInfos = _nestor.WordInfo(word);
            foreach (Word wordInfo in wordInfos)
            {
                WordForm[] exactForms = wordInfo.ExactForms(word);
                foreach (WordForm form in exactForms)
                {
                    if (form.Accent > 0)
                        knownAccents.Add(form.Accent);
                }
            }

            // there are no known accents, every vowel can be an accent
            if (knownAccents.Count == 0)
                return Enumerable.Repeat(-1, vCount).ToArray();
            
            // only certain vowels are accents, any other are not
            int[] accents = Enumerable.Repeat(0, vCount).ToArray();
            foreach (int knownAccent in knownAccents) 
                accents[knownAccent - 1] = 1;

            return accents;
        }

        /*private SaluteResponse TellAbout(Foot foot, SaluteRequest request)
        {
            throw new NotImplementedException();
        }*/

        private SaluteResponse Help(SaluteRequest request)
        {
            throw new NotImplementedException();
        }

        private SaluteResponse TooShort(SaluteRequest request)
        {
            throw new NotImplementedException();
        }

        private string RandomSuggestion()
        {
            return Suggestions[_random.Next(0, Suggestions.Length)];
        }
    }
}