using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.API
{
    public delegate void APICallbackDelegate(String message, IRespondParameter parameter);

    public class GenericRest<T> where T : new()
    {
        public void invoke(IRestClient client, IRestRequest request, APICallbackDelegate callback)
        {
            //To bypass invalid certificate
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };

            var handle = client.ExecuteAsync<T>(request, response => {


  
                callback(response.StatusCode.ToString(), response.Data as IRespondParameter);
            });
        }
    }
}
