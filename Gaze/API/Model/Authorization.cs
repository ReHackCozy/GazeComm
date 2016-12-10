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
            public static readonly String Version = "v1.0";
            public static readonly String AppKey = "fnuXeqWBp7Z18j8Y_qClin3mUFEa";
            public static readonly String Username = "+601546010523";
            public static readonly String Type = "1";
            public static readonly String Format = "json";
            public static readonly String Authorization = "Caas12345";
        }

        internal class SimpleAuthorizationResponse : IRespondParameter
        {
            public String access_token { get; set; }
            public String refresh_token { get; set; }
            public String resultcode { get; set; }
            public String expires_in { get; set; }
        }
    }
}
