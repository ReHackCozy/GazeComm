using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Gaze.API.Model.TTS;

namespace Gaze.API
{
    class SendTTS
    {
        //TODO: put somewhere proper app config maybe
        private readonly String _sendMessageURL = "https://ompserver.tm.com.my/rest/httpsessions/tts2Note/" + TTSRequestURL.Version;

        public void OnAPICallback(string message, IRespondParameter parameters)
        {
            //Do something here
            Console.WriteLine(message);
        }

        public void Invoke(String message, String to, String accessToken)
        {
            //hook up delegate
            APICallbackDelegate handler = OnAPICallback;

            var request = new GenericRest<TTSResponse>();
            request.invoke(createClient(), createRequest(message, to, accessToken), handler);
        }

        private IRestClient createClient()
        {
            return new RestClient(_sendMessageURL);
        }

        private IRestRequest createRequest(String message, String to, String accessToken)
        {
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;

            request.AddQueryParameter("app_key", TTSRequestURL.AppKey);
            request.AddQueryParameter("access_token", accessToken);
            request.AddQueryParameter("format", TTSRequestURL.Format);
            request.AddBody(createBody(message, to));
            return request;
        }

        private object createBody(String message, String to)
        {
            return new TTSRequestBody(message, to);
        }
    }
}
