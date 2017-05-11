using RestSharp;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using static Gaze.API.Model.Authorization;
using static Gaze.API.Model.Authorization.SimpleAuthorizationResponse;

namespace Gaze.API
{
    //fastlogin
    class AuthorizationAPI
    {
        //TODO: put somewhere proper app config maybe
        private readonly String _simpleAuthorizationURL = "https://developer.tm.com.my:8443/CaasSBV1/Impl/ImplRS/fastlogin";
        //TODO: this shouldn't be here
        public String AccessToken { get; set; }
        private Action _forwardCall;

        public AuthorizationAPI(Action forward)
        {
            _forwardCall = forward;
        }

        public void OnAPICallback(string message, IRespondParameter parameters)
        {
            if(parameters is SimpleAuthorizationResponse) {
                ResponseResult result = (parameters as SimpleAuthorizationResponse).responseResult;
                String value = result.value;
                RealResponse response = SimpleJson.DeserializeObject<RealResponse>(value);
                AccessToken = response.access_token;
                // temp fix
                //AccessToken = (parameters as SimpleAuthorizationResponse).access_token;
            }
            Console.WriteLine(message);
            _forwardCall();
        }

        public void Invoke()
        {
            //hook up delegate
            APICallbackDelegate handler = OnAPICallback;

            var request = new GenericRest<SimpleAuthorizationResponse>();
            request.invoke(createClient(), createRequest(), handler);
        }

        private IRestClient createClient()
        {
            return new RestClient(_simpleAuthorizationURL);
        }

        private IRestRequest createRequest()
        {
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("APITokenId", SimpleAuthorizationHeader.APITokenId);
            request.AddHeader("PartnerId", SimpleAuthorizationHeader.PartnerId);
            request.AddHeader("PartnerTokenId", SimpleAuthorizationHeader.PartnerTokenId);
            //request.AddQueryParameter("format", SimpleAuthorizationHeader.Format);
            //request.AddHeader("Authorization", SimpleAuthorizationHeader.Authorization);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            return request;
        }
    }
}
