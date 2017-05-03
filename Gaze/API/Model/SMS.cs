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
            public static readonly String APITokenId = "bveSj3lV1cRULaxbbn/polMUxmk=";
            public static readonly String PartnerId = "NhnvbjxtS/7eTCKl+L22OkA/Z7s=";
            public static readonly String PartnerTokenId = "yb0qO3ysMhBih9db65ma1048Rlc=";
        }

        internal class SMSBody
        {
      //hardcoded - TM's fault
      private static String IBiD = "601546011016";
      private static String IBPwd = "i5Os0vhR";
      //public String username
      //      {
      //          get { return "ceo"; }
      //          private set { }
      //      }
      //      public String password
      //      {
      //          get { return Utilities.Util.HashSHA1("ceotab"); }
      //          private set { }
      //      }
            public String msgtype
            {
                get { return "text"; }
                private set { }
            }
            public String message { get; set; }
            public String to { get; set; }
            public String hashkey
            {
                get { return Utilities.Util.HashSHA1(IBiD + IBPwd + to); }
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

        internal class SMSResponse : IRespondParameter
        {
            public String RespondResult { get; set; }
            public String RespondCode { get; set; }
        }
    }
}
