using RestSharp;
using System;
using static Gaze.API.Model.Authorization;

namespace Gaze.API
{
    class AuthorizationAPI
    {
        //TODO: put somewhere proper app config maybe
        private readonly String _simpleAuthorizationURL = "https://ompserver.tm.com.my/rest/fastlogin/" + SimpleAuthorizationHeader.Version;
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
                AccessToken = (parameters as SimpleAuthorizationResponse).access_token;
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
            request.AddQueryParameter("app_key", SimpleAuthorizationHeader.AppKey);
            request.AddQueryParameter("username", SimpleAuthorizationHeader.Username);
            request.AddQueryParameter("type", SimpleAuthorizationHeader.Type);
            request.AddQueryParameter("format", SimpleAuthorizationHeader.Format);
            request.AddHeader("Authorization", SimpleAuthorizationHeader.Authorization);
            return request;
        }
    }
}
