using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;//this gives us helper methods like PostAsJsonAsync and ReadFromJsonAsync. so we c an send/recieve JSON without writing manual serialization code.

namespace Integration.Agent
{

    
        //OtClient is a  small wrapper class around HttpClient to talk to the OT WebAPI.
        public sealed class OtClient
        {
            private readonly HttpClient _http; //the actual HTTP engine that sends requests
            private readonly string _baseUrl; //base address of the OT system (e.g. "http://localhost:5189")

            //Constructor is called when we create a new OtClient instance.
            //It receives HttpClient injected bY DI
            //base url  where the OT service is running 
            public OtClient(HttpClient http,string baseUrl,string? apiKey = null,int timeoutSeconds = 5)
            {
                _http = http;
                _baseUrl = baseUrl.TrimEnd('/'); //remove trailing slash if any
                _http.Timeout = TimeSpan.FromSeconds(timeoutSeconds); //set timeout for all requests

                //if caller supplied an API key 
                if(!string.IsNullOrWhiteSpace(apiKey) && !_http.DefaultRequestHeaders.Contains("X-Api-Key"))
                    _http.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            }

            //Local DTOS matching to OT system
            public record OrderDto(int OrderId, string CustomerName, string DishName,int Quantity);
            public record StatusDto(int OrderId,string Status);


            //SendAsync :called when integration agent finds new order  in IT DB that hasnt ebenn sent to OT yet
            public async Task<bool> SendAsync(int orderId,string customerName,string dishName,int quantity,CancellationToken ct = default)
            {
                var dto = new OrderDto(orderId, customerName, dishName, quantity);
                var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/orders", dto);
                return response.IsSuccessStatusCode;
            }

            //GetStatusAsync: called when integration agent wants to check status of an order in OT system
            public async Task<string?> GetStatusAsync(int orderId,CancellationToken ct = default)
            {
                var response = await _http.GetAsync($"{_baseUrl}/api/orders/{orderId}/status",ct);
                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadFromJsonAsync<StatusDto>(cancellationToken:ct);
                    return dto?.Status;
                }
                return null; //null means error

            }
        }
}

