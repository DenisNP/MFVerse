using System;
using Verse.Models.Salute.Web;

namespace Verse.Models
{
    public class Phrase
    {
        private readonly string _sber;
        private readonly string _athena;
        private readonly string _joy;

        public Phrase(string sber, string athena, string joy)
        {
            _sber = sber;
            _athena = athena;
            _joy = joy;
        }

        public string For(SaluteRequest request)
        {
            return request.Payload.Character.Id switch
            {
                CharacterId.Sber => _sber,
                CharacterId.Athena => _athena,
                CharacterId.Joy => _joy,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}