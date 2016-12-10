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
            public static readonly String APITokenId = "eI4WlvTSD7FFip1595L8suhlPcw=";
            public static readonly String PartnerId = "x53cWqsrAJP3qaD8WAack/z/bRc=";
            public static readonly String PartnerTokenId = "gc5aD4RFwCPvl6jfQicJbGCZszE=";
        }

        internal class SMSBody
        {
            //hardcoded - TM's fault
            public String Username
            {
                get { return "ceo"; }
                private set { }
            }
            public String Password
            {
                get { return Utilities.Util.HashSHA1("ceotab"); }
                private set { }
            }
            public String MessageType
            {
                get { return "text"; }
                private set { }
            }
            public String Message { get; set; }
            public String To { get; set; }
            public String HashKey
            {
                get { return Utilities.Util.HashSHA1("ceoceotab" + To); }
                private set { }
            }
            public String Filename
            {
                get { return "null"; }
                private set { }
            }
            public String TranscId
            {
                get { return Guid.NewGuid().ToString(); }
                private set { }
            }

            public SMSBody(String message, String to)
            {
                Message = message;
                To = to;
            }
        }

        internal class SMSRespond
        {
            public String RespondResult { get; set; }
            public String RespondCode { get; set; }
        }
    }
}
