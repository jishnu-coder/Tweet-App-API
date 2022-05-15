using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.TokenHandler
{
    public  class GetAccessTokenClass
    {
         

        public async Task<TokenResponse> GetAccessToken(string loginId,string email ,  string password)
        {
            TokenResponse apiResponse;
          
            using (var httpClient = new HttpClient())//handler
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer ");
                using (var response = await httpClient.GetAsync($"https://localhost:6001/api/Auth?Username={loginId}&Password={password}&email={email}"))
                {
                    var json =  response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<TokenResponse>(json);
                }                
                
            }

            return apiResponse;
        }
    }
}
