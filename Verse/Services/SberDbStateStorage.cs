using System;
using Newtonsoft.Json;
using Verse.Abstract;
using Verse.Helpers;
using Verse.Models;

namespace Verse.Services
{
    public class SberDbStateStorage : IStateStorage<UserState>
    {
        private string _appHash;
        private const string SberDbAddress = "https://smartapp-code.sberdevices.ru/tools/api/data/";

        private void Init()
        {
            _appHash = Environment.GetEnvironmentVariable("SBERDB_HASH_ALGEBRA");
            
            if (string.IsNullOrEmpty(_appHash))
                throw new ArgumentNullException(nameof(_appHash), "Invalid state storage hash");
        }

        public UserState GetState(string userId)
        {
            if (string.IsNullOrEmpty(_appHash))
                Init();

            string response = Web.GetRequest(GetFullPath(userId));
            if (string.IsNullOrEmpty(response))
                return new UserState {UserId = userId};

            var state = JsonConvert.DeserializeObject<UserState>(response);
            return state == null ? new UserState {UserId = userId} : state;
        }

        public void SetState(string userId, UserState state)
        {
            if (string.IsNullOrEmpty(_appHash))
                Init();

            string data = JsonConvert.SerializeObject(state);
            Web.PostRequest(GetFullPath(userId), data);
        }

        private string GetFullPath(string userId)
        {
            return $"{SberDbAddress}{($"{_appHash}_{userId}").Replace("/", "_")}";
        }
    }
}