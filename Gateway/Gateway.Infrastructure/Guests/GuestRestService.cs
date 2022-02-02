using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gateway.Application.Guests;
using Gateway.Application.Guests.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Gateway.Infrastructure.Guests
{

    public class GuestRestService : IGuestService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<GuestRestService> logger;

        public GuestRestService(HttpClient httpClient, IConfiguration configuration, ILogger<GuestRestService> logger)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.logger = logger;
        }
       
       
        public async Task CreateGuestAsync(CreateGuestCommand createGuestCommand)
        {
           logger.LogInformation($"Started execution of {nameof(CreateGuestAsync)}");
           
           string serializedCreateGuestCommand = JsonConvert.SerializeObject(createGuestCommand);

           string targetUrl = String.Concat(configuration["ConnectionStrings:GuestManagementService"],"api/Guest/");

           logger.LogInformation($"Going to make request under the {targetUrl}");

           var response = await httpClient.PostAsync(targetUrl, serializedCreateGuestCommand);

           string responseContent = await response.Content.ReadAsStringAsync();

           logger.LogInformation($"Received response with the following status code {response.StatusCode} with the following message {responseContent}");
                              
        }

        public async Task RegisterForAVisitAsync(RegisterGuestCommand registerGuestCommand)
        {
           string serializedRegisterGuestCommand = JsonConvert.SerializeObject(registerGuestCommand);

           string targetUrl = String.Concat(configuration["ConnectionStrings:GuestManagementService"],"api/Guest/VisitRegistration");

           //var response = await httpClient.PostAsync(String.Concat(configuration["ConnectionStrings:GuestManagementService"],"api/Guest/VisitRegistration"),content);

            await httpClient.PostAsync(targetUrl,serializedRegisterGuestCommand);
        }


        public async Task CancelRegistrationAsync(CancelRegistrationCommand cancelRegistrationCommand)
        {
            // string serializedCancelRegistrationCommand = JsonConvert.SerializeObject(cancelRegistrationCommand);
            // StringContent content = new StringContent(serializedCancelRegistrationCommand, Encoding.UTF8, "application/json");
            
            // var response = await httpClient.PostAsync(String.Concat(configuration["ConnectionStrings:GuestManagementService"],"api/Guest/"), content);

            // if(!response.IsSuccessStatusCode){
            //     throw new HttpRequestException(response.ReasonPhrase);
            // }
        }


    }

}