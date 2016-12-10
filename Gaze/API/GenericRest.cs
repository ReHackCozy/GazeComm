using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.API
{
    class GenericRest<T> where T : new()
    {

        public String invoke(IRestClient client, IRestRequest request)
        {
            //TODO: async
            var response = client.Execute<T>(request);
            return response.Content; 
        }
    }
}
