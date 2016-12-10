using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Gaze.API.Model.SMS;

namespace Gaze.API
{
    class SendMessage
    {
        //TODO: put somewhere proper app config maybe
        private readonly String _sendMessageURL = "https://developer.tm.com.my:8443/SMSSBV1/SMSImpl/SMSImplRS/SendMessage";

        public static void OnAPICallback(string message)
        {
            //Do something here
            Console.WriteLine(message);
        }

        public void Invoke(String message, String to)
        {
            //hook up delegate
            APICallbackDelegate handler = OnAPICallback;

            var request = new GenericRest<SMSRespond>();
            request.invoke(createClient(), createRequest(message, to), handler);
        }

        private IRestClient createClient()
        {
            return new RestClient(_sendMessageURL);
        }

        private IRestRequest createRequest(String message, String to)
        {
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            addHeader(ref request);
            request.AddBody(createBody(message, to));
            return request;
        }

        private object createBody(String message, String to)
        {
            return new SMSBody(message, to);
        }

        private void addHeader(ref RestRequest request)
        {
            request.AddHeader("APITokenId", SMSHeader.APITokenId);
            request.AddHeader("PartnerId", SMSHeader.PartnerId);
            request.AddHeader("PartnerTokenId", SMSHeader.PartnerTokenId);
        }
    }
}
