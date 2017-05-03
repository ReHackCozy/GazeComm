using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.API.Model
{
    class TTS
    {
        internal class TTSRequestURL
        {
            //old
            public static readonly String Version = "v1.0";
            public static readonly String AppKey = "fnuXeqWBp7Z18j8Y_qClin3mUFEa";
            public static readonly String Format = "json";
            //new
            public static readonly String APITokenId = "1B0VG7PaKrwZAmcNi6PoVz/b/0s=";
            public static readonly String PartnerId = "NhnvbjxtS/7eTCKl+L22OkA/Z7s=";
            public static readonly String PartnerTokenId = "yb0qO3ysMhBih9db65ma1048Rlc=";
            }

        internal class TTSRequestBody
        {
            public String displayNbr
            {
                get { return calleeNbr; }
                private set { }
            }
            public String calleeNbr { get; set; }
            public String languageType
            {
                // 0 = english, 1 = chinese
                get { return "0"; }
                private set { }
            }
            public String ttsContent { get; set; }
            public  String replayTimes
            {
                get { return "1"; }
                private set { }
            }
            public TTSRequestBody(String message, String to)
            {
                this.ttsContent = Utilities.Util.Base64Encode(message);
                this.calleeNbr = to;
            }
        }

        internal class TTSResponse : IRespondParameter
        {
            public String RespondResult { get; set; }
            public String RespondCode { get; set; }
        }
    }
}
