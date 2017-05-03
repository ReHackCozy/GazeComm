using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.API.Model
{
    class Authorization
    {



        //internal class AuthorizationTokenHeader
        //{
        //    public static readonly String AppKey = "fnuXeqWBp7Z18j8Y_qClin3mUFEa";
        //    public static readonly String ResponseType = "code";
        //    public static readonly String CallbackURL = "=";
        //}

        //internal class AuthorizationTokenResponse
        //{
        //    public String code { get; set; }
        //}

        //internal class AccessTokenHeader
        //{
        //    public static readonly String AppKey = "fnuXeqWBp7Z18j8Y_qClin3mUFEa";
        //    public static readonly String GrantType = "authorization_code";
        //    public static readonly String CallbackURL = "=";
        //    public static readonly String AppSecret = "ZtpheCMH4f16ikZgFKzM1K4j5xAa";
        //    //public static readonly String AuthorizationTokenCode = "";

        //}

        //internal class AccessTokenResponse
        //{
        //    public String access_token { get; set; }
        //    public String refresh_token { get; set; }
        //    public String token_type { get; set; }
        //    public String expires_in { get; set; }
        //    public String scope { get; set; }
        //}

        internal class SimpleAuthorizationHeader
        {
            //old
            public static readonly String Version = "v1.0";
            public static readonly String AppKey = "fnuXeqWBp7Z18j8Y_qClin3mUFEa";
            public static readonly String Username = "+601546010523";
            public static readonly String Type = "1";
            public static readonly String Format = "json";
            public static readonly String Authorization = "Caas12345";
            //new
            public static readonly String APITokenId = "S2fs7KKfg1HaR89RTJUXqi4uBQc=";
            public static readonly String PartnerId = "NhnvbjxtS/7eTCKl+L22OkA/Z7s=";
            public static readonly String PartnerTokenId = "yb0qO3ysMhBih9db65ma1048Rlc=";
        }

        internal class RealResponse
            {
            public String access_token { get; set; }
            public String refresh_token { get; set; }
            public String resultcode { get; set; }
            public String expires_in { get; set; }
            }

        internal class SimpleAuthorizationResponse : IRespondParameter
        {
            

            public ResponseResult responseResult { get; set; }
            public String responseCode { get; set; }

            internal class ResponseResult
                {
                public String type { get; set; }
                public String value { get; set; }
                }


            }
    }
}
