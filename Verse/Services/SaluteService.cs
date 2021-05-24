using System;
using System.Collections.Generic;
using System.Linq;
using Nestor;
using Nestor.Models;
using Newtonsoft.Json;
using Verse.Helpers;
using Verse.Models;
using Verse.Models.Salute.Web;

namespace Verse.Services
{
    public class SaluteService
    {
        private static readonly string[] Suggestions =
        {
            "Буря мглою небо кроет",
            "Мороз и солнце день чудесный",
            "На заре ты её не буди",
            "Тучки небесные вечные странники",
            "Есть женщины в русских селеньях"
        };
        private readonly Random _random = new();
        private NestorMorph _nestor;
        private readonly HashSet<string> _vowels = new() {"а", "о", "у", "ы", "э", "я", "ё", "ю", "и", "е"};

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

            if (request.Tokens.Length < 3)
            {
                if (request.HasWords("помощь", "что уметь", "что мочь"))
                    return Help(request);

                return TooShort(request);
            } 
            else if (request.Tokens.Length > 10)
            {
                return TooLong(request);
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
            var response = new SaluteResponse(request);
            var phrase = new Phrase(
                "<speak>Здоровья вам желаю, <break time=\"1ms\" />леди или сэр.<break time=\"200ms\" />" +
                "Читайте мне строку, я вам скажу размер.</speak>",
                "<speak>Вам, пользователь, здравствовать желаю. <break time=\"200ms\" />" +
                "Строку отправьте — я размер <break time=\"1ms\" />узна'ю.</speak>",
                "<speak>Привет-привет, <break time=\"1ms\" />любимый юзер мой. <break time=\"200ms\" />" +
                "Прочти мне стих, пойму размер любой!<speak>"
            );
            
            response
                .AppendText(request, phrase)
                .AppendSuggestions(RandomSuggestion(), "Помощь", "Выход");

            response.Payload.AutoListening = true;
            return response;
        }

        private SaluteResponse TryParseFoot(SaluteRequest request)
        {
            int numVowels = string.Join("", request.Tokens).Count(c => _vowels.Contains(c.ToString()));
            if (numVowels < 4)
                return TooShort(request);
            if (numVowels > 20)
                return TooLong(request);

            var response = new SaluteResponse(request);
            ParseResult result = GetParseResult(request.Tokens, out int bestDistance);
            
            if (bestDistance > 10 || result.FootType == FootType.Unknown)
            {
                // cannot determine foot
                response
                    .AppendText(request, new Phrase("Не понимаю", "Не понимаю", "Не понимаю"))
                    .AppendSendData("state_updated", JsonConvert.SerializeObject(ParseResult.Empty))
                    .AppendSuggestions(RandomSuggestion(), "Помощь", "Выход");
                return response;
            }
            
            // return parsed phrase and foot
            var stepsText = $"В данном случае стих {result.StepsName}.";
            var foot = new Foot(result.FootType);
            response
                .AppendText($"<speak>{foot.Name}<break time=\"400ms\" />\n{foot.Description}. <break time=\"200ms\" />{stepsText}</speak>")
                .AppendSendData("state_updated", JsonConvert.SerializeObject(result))
                .AppendSuggestions(RandomSuggestion(), "Помощь", "Выход");

            return response;
        }

        private ParseResult GetParseResult(string[] tokens, out int bestDistance)
        {
            int[] stresses = tokens.SelectMany(GetStresses).ToArray();

            // find best foot
            Foot bestFoot = null;
            bestDistance = int.MaxValue;
            foreach (FootType footType in Enum.GetValues<FootType>())
            {
                if (footType == FootType.Unknown) continue;
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
            if (syllables.Length > 0 && syllables[0].Length > 0)
                syllables[0][0].Text = syllables[0][0].Text.ToUpperFirst();

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
            
            void FlushConsonant()
            {
                if (string.IsNullOrEmpty(lastConsonant))
                    return;
                
                syllables.Add(new Syllable
                {
                    Text = lastConsonant,
                    Type = SyllableType.Consonant
                });
                lastConsonant = "";
            }
            
            foreach (string c in word.Select(t => t.ToString()))
            {
                if (!_vowels.Contains(c))
                {
                    lastConsonant += c;
                }
                else
                {
                    FlushConsonant();
                    
                    int nextStress = stresses.First();
                    stresses.RemoveAt(0);
                    syllables.Add(new Syllable
                    {
                        Text = c,
                        Type = nextStress == 1 ? SyllableType.Stressed : SyllableType.Unstressed
                    });
                }
            }
            
            FlushConsonant();
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
                if (wordStress != -1)
                {
                    if (maskStress == 0 && wordStress == 1)
                        dist += 5;
                    else if (maskStress == 1 && wordStress == 0)
                        dist += 2;
                }
            }

            if (foot.Mask.Last() != stresses.Last())
                dist += 1;

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
            if (knownAccents.Count == 1)
            {
                // strong single accent
                accents[knownAccents.First() - 1] = 1;
            }
            else
            {
                // various accents
                foreach (int knownAccent in knownAccents) 
                    accents[knownAccent - 1] = -1;
            }

            return accents;
        }

        private SaluteResponse Help(SaluteRequest request)
        {
            var response = new SaluteResponse(request);
            response
                .AppendText(
                    request,
                    new Phrase(
                        "Прочитайте мне строку из стихотворения, и я скажу вам его стихотворный размер. " +
                        "Пока что я понимаю классические двусложные и трёхсложные размеры.",
                        "Прочитайте мне строку из стихотворения, и я определю его стихотворный размер. " +
                        "На настоящий момент я понимаю классические двусложные и трёхсложные размеры.",
                        "Прочитай мне строчку из стихотворения, и я скажу тебе его стихотворный размер. " +
                        "Пока что я понимаю простые двусложные и трёхсложные размеры."
                    )
                )
                .AppendSendData("state_updated", JsonConvert.SerializeObject(ParseResult.Empty))
                .AppendSuggestions(RandomSuggestion(), "Помощь", "Выход");
            return response;
        }

        private SaluteResponse TooShort(SaluteRequest request)
        {
            var response = new SaluteResponse(request);
            response
                .AppendText(
                    request, new Phrase(
                        "Простите, текст слишком короткий, по нему не смогу определить размер",
                        "Для определения размера нужна строка подлиннее",
                        "Прости, строчка слишком короткая, давай подлиннее"
                    )
                )
                .AppendSendData("state_updated", JsonConvert.SerializeObject(ParseResult.Empty))
                .AppendSuggestions(RandomSuggestion(), "Помощь", "Выход");
            return response;
        }
        
        private SaluteResponse TooLong(SaluteRequest request)
        {
            var response = new SaluteResponse(request);
            response
                .AppendText(
                    request, new Phrase(
                        "Простите, текст слишком длинный, постарайтесь прочитать одну или две строчки",
                        "Текст слишком длинный. Для определения размера нужна одна-две строки",
                        "Прости, текст слишком длинный, давай ровно одну или две строчки"
                    )
                )
                .AppendSendData("state_updated", JsonConvert.SerializeObject(ParseResult.Empty))
                .AppendSuggestions(RandomSuggestion(), "Помощь", "Выход");
            return response;
        }

        private string RandomSuggestion()
        {
            return Suggestions[_random.Next(0, Suggestions.Length)];
        }
    }
}