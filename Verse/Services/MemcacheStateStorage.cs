using System;
using Microsoft.Extensions.Caching.Memory;
using Verse.Abstract;
using Verse.Models;

namespace Verse.Services
{
    public class MemcacheStateStorage : IStateStorage<SessionState>
    {
        private readonly IMemoryCache _memoryCache;

        public MemcacheStateStorage(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public SessionState GetState(string userId)
        {
            _memoryCache.TryGetValue(userId, out SessionState state);
            return state ?? new SessionState();
        }

        public void SetState(string userId, SessionState state)
        {
            _memoryCache.Set(userId, state, TimeSpan.FromMinutes(30));
        }
    }
}