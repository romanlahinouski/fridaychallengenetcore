using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Infrastructure
{
    public class HttpClient
    {

        public static System.Net.Http.HttpClient Instance { get; private set; }


        public HttpClient(Func<System.Net.Http.HttpClient> configBuilder)
        {
           if(Instance == null)
            {
                Instance = configBuilder();               
            }                 
        }
        
        public System.Net.Http.HttpClient GetInstance()
        {        
            return Instance;
        }


        public async Task<HttpResponseMessage> PostAsync(string url,string stringContent){

         StringContent content = new StringContent(stringContent,Encoding.UTF8, "application/json" );

          try{
            var response = await Instance.PostAsync(url, content);
                     
            if(!response.IsSuccessStatusCode)  {
                var responseContentString = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(responseContentString);         
            }
              return response;
          }

          catch(HttpRequestException ex){         
              throw;
          }         
        }

    }
}
