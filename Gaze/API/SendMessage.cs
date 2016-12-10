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
        private readonly String _sendMessageURL = "https://developer.tm.com.my:8443/SMSSBV1/";
        private readonly String _sendMessageAction = "SMSImpl/SMSImplRS/SendMessage";

        public void Invoke(String message, String to)
        {
            var request = new GenericRest<SMSRespond>();
            var response = request.invoke(createClient(), createRequest(message, to));
        }

        private IRestClient createClient()
        {
            var client = new RestClient(_sendMessageURL);
            return client;
        }

        private IRestRequest createRequest(String message, String to)
        {
            var request = new RestRequest(_sendMessageAction, Method.POST);

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
