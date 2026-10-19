using BACKBONE.Application.Interfaces.SecurityInterface;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Infrastructure.SecurityRepo
{
    public class HrisAuthService : IHrisAuthService
    {
        public async Task<string> Login(string username, string password)
        {
            // First attempt
            try
            {
                Console.WriteLine($"Attempting HRIS authentication for user {username}");
                
                var client = new HttpClient();
                var firstRequest = new HttpRequestMessage(HttpMethod.Post, "http://hris.prangroup.com:8696/Login/LoginHris");
                firstRequest.Headers.Add("Authorization", "Basic YXV0aDoxMlByYW5AMTIzNDU2JA==");

                var firstRequestBody = new { username = username ?? string.Empty, password = password ?? string.Empty };
                var firstJson = JsonConvert.SerializeObject(firstRequestBody);
                firstRequest.Content = new StringContent(firstJson, Encoding.UTF8, "application/json");

                var firstResponse = await client.SendAsync(firstRequest);
                firstResponse.EnsureSuccessStatusCode();
                var firstResponseBody = await firstResponse.Content.ReadAsStringAsync();
                var firstData = JsonConvert.DeserializeObject<dynamic>(firstResponseBody);

                if (firstData != null && firstData["status"] != null && firstData["status"].ToString() == "Success")
                {
                    Console.WriteLine($"HRIS authentication successful for user {username}");
                    return "Y";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Primary HRIS authentication endpoint failed for user {username}: {ex.Message}");
            } // Swallow exception to try secondary endpoint

            // Fallback attempt
            try
            {
                Console.WriteLine($"Attempting fallback HRIS authentication for user {username}");
                
                var client = new HttpClient();
                var secondRequest = new HttpRequestMessage(HttpMethod.Post, "http://hrisapi.prangroup.com:8083/v1/Login/HrisLogin");
                secondRequest.Headers.Add("S_KEYL", "RxsJ4LQdkVFTv37rYfW9b6");
                secondRequest.Headers.Add("Authorization", "Basic YXV0aDoxMlByYW5AMTIzNDU2JA==");

                var secondRequestBody = new { UserName = username ?? string.Empty, Password = password ?? string.Empty };
                var secondJson = JsonConvert.SerializeObject(secondRequestBody);
                secondRequest.Content = new StringContent(secondJson, Encoding.UTF8, "application/json");

                var secondResponse = await client.SendAsync(secondRequest);
                secondResponse.EnsureSuccessStatusCode();
                var secondResponseBody = await secondResponse.Content.ReadAsStringAsync();
                var secondData = JsonConvert.DeserializeObject<dynamic>(secondResponseBody);

                var result = (secondData != null && secondData["succesS_MESSAGE"] != null && secondData["succesS_MESSAGE"].ToString() == "LOGIN_SUCCESS") ? "Y" : "Fallback failed";
                
                if (result == "Y")
                {
                    Console.WriteLine($"Fallback HRIS authentication successful for user {username}");
                }
                else
                {
                    Console.WriteLine($"Fallback HRIS authentication failed for user {username}");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HRIS authentication failed for user {username}: {ex.Message}");
                return ex.Message;
            }
        }
    }
}