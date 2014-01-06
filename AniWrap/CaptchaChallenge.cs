using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace AniWrap
{
    public class CaptchaChallenge : IDisposable
    {
        private MemoryStream memio;

        public CaptchaChallenge(MemoryStream data, string challenge)
        {
            this.memio = data;
            this.ChallengeField = challenge;
            this.CaptchaImage = Image.FromStream(memio);
        }

        public Image CaptchaImage
        {
            get;
            private set;
        }

        public string ChallengeField { get; private set; }

        public void Dispose()
        {
            this.CaptchaImage.Dispose();
            this.memio.Close();
            this.memio.Dispose();
        }
    }
}
