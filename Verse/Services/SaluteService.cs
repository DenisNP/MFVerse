using System;
using Verse.Abstract;
using Verse.Helpers;
using Verse.Models;
using Verse.Models.Salute.Web;

namespace Verse.Services
{
    public class SaluteService
    {
        private readonly IStateStorage<UserState> _userStateStorage;
        private readonly IStateStorage<SessionState> _sessionStateStorage;

        private static readonly string[] Suggestions =
        {
            ""
        };
        private readonly Random _random = new();

        public SaluteService(
            IStateStorage<UserState> userStateStorage,
            IStateStorage<SessionState> sessionStateStorage
        )
        {
            _userStateStorage = userStateStorage;
            _sessionStateStorage = sessionStateStorage;
        }
        
        public SaluteResponse Handle(SaluteRequest request)
        {
            if (request.IsEnter)
                return Enter(request);

            if (request.Tokens.Length <= 3)
            {
                if (request.HasWords("помощь", "что уметь", "что мочь"))
                    return Help(request);

                foreach (FootType footType in Enum.GetValues<FootType>())
                {
                    if (footType == FootType.Unknown) continue;
                    var foot = new Foot(footType);

                    if (request.HasWords(foot.Name))
                        return TellAbout(foot, request);
                }

                return TooShort(request);
            }

            return TryParseFoot(request);
        }

        private SaluteResponse Enter(SaluteRequest request)
        {
            throw new NotImplementedException();
        }

        private SaluteResponse TryParseFoot(SaluteRequest request)
        {
            throw new NotImplementedException();
        }

        private SaluteResponse TellAbout(Foot foot, SaluteRequest request)
        {
            throw new NotImplementedException();
        }

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