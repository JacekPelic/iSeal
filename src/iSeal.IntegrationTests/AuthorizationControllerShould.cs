using System;
using Xunit;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using iSeal.API.DTO;
using Newtonsoft.Json;
using System.Text;

namespace iSeal.IntegrationTests
{
    public class AuthorizationControllerShould : IClassFixture<WebApplicationFactory<API.Startup>>
    {
        private readonly WebApplicationFactory<API.Startup> _factory;

        public AuthorizationControllerShould(WebApplicationFactory<API.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SucesfullyRegisterUser()
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register");

            var user = new UserRegister
            {
                Email = "admin12@iseal.com",
                Password = "!se4lP4ss",
                PhoneNumber = "123456789"
            };

            postRequest.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpResponseMessage postResponse = await _factory.CreateClient().SendAsync(postRequest);

            postResponse.EnsureSuccessStatusCode();
        }
    }
}
