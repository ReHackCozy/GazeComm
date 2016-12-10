using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.API.Model
{
    class SMS
    {

        internal static class SMSHeader
        {
            public static readonly String APITokenId = "A83NXDEnRCcwkd5k3KLp4qknuak=";
            public static readonly String PartnerId = "x53cWqsrAJP3qaD8WAack/z/bRc=";
            public static readonly String PartnerTokenId = "gc5aD4RFwCPvl6jfQicJbGCZszE=";
        }

        internal class SMSBody
        {
            //hardcoded - TM's fault
            public String username
            {
                get { return "ceo"; }
                private set { }
            }
            public String password
            {
                get { return Utilities.Util.HashSHA1("ceotab"); }
                private set { }
            }
            public String msgtype
            {
                get { return "text"; }
                private set { }
            }
            public String message { get; set; }
            public String to { get; set; }
            public String hashkey
            {
                get { return Utilities.Util.HashSHA1("ceoceotab" + to); }
                private set { }
            }
            public String filename
            {
                get { return "null"; }
                private set { }
            }
            public String transcid
            {
                get { return Guid.NewGuid().ToString(); }
                private set { }
            }

            public SMSBody(String message, String to)
            {
                this.message = message;
                this.to = to;
            }
        }

        internal class SMSRespond
        {
            public String RespondResult { get; set; }
            public String RespondCode { get; set; }
        }
    }
}
