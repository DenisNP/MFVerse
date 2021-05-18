using System;
using Verse.Abstract;
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
            throw new NotImplementedException();
        }

        private string RandomSuggestion()
        {
            return Suggestions[_random.Next(0, Suggestions.Length)];
        }
    }
}