using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniWrap
{
   public class SolvedCaptcha
    {

        public string ChallengeField { get; private set; }
        public string ResponseField { get; private set; }

        public SolvedCaptcha(string challenge, string response) 
        {
            this.ChallengeField = challenge;
            this.ResponseField = response;
        }
    }
}
