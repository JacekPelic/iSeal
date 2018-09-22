using iSeal.API;
using iSeal.API.Configuration;
using iSeal.API.DTO;
using iSeal.Dal.Entities;
using iSeal.integrationTest.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace iSeal.integrationTest
{
    [Collection("iSeal Integration collection")]
    public class AuthorizationControllerShould
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _httpClient;

        public AuthorizationControllerShould(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task SucesfullyRegisterUser()
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register");

            var user = new UserRegister
            {
                Email = "testAdmin@iseal.com",
                Password = "!se4lP4ss",
                PhoneNumber = "987654321"
            };

            postRequest.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpResponseMessage postResponse = await _httpClient.SendAsync(postRequest);

            postResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task FailToRegisterUserWithExistingEmail()
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register");

            var user = new UserRegister
            {
                Email = "admin@iseal.com",
                Password = "!se4lP4ss",
                PhoneNumber = "123456789"
            };

            postRequest.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpResponseMessage postResponse = await _httpClient.SendAsync(postRequest);

            Assert.Equal(HttpStatusCode.Conflict, postResponse.StatusCode);
        }

        [Fact]
        public async Task FailToRegisterUserWithExistingOrganization()
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register");

            var user = new UserRegister
            {
                Email = "existingOrganization@iseal.com",
                Password = "!se4lP4ss",
                PhoneNumber = "123456789",
                Organization = "iSeal"
            };

            postRequest.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpResponseMessage postResponse = await _httpClient.SendAsync(postRequest);

            Assert.Equal(HttpStatusCode.Conflict, postResponse.StatusCode);
        }

        [Fact]
        public async Task CreateAccessTokenForExistingUser()
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login");

            var user = new UserCredential
            {
                Email = "admin@iseal.com",
                Password = "!se4lP4ss"
            };

            postRequest.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpResponseMessage postResponse = await _httpClient.SendAsync(postRequest);

            postResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task FailCreatingAccessTokenForNonExistingUser()
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login");

            var user = new UserCredential
            {
                Email = "someWrongCredentials@iseal.com",
                Password = "!se4lP4ss"
            };

            postRequest.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpResponseMessage postResponse = await _httpClient.SendAsync(postRequest);

            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
            Assert.Contains("Wrong credentials", await postResponse.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task CheckSubClaimForAccessToken()
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login");

            var user = new UserCredential
            {
                Email = "admin@iseal.com",
                Password = "!se4lP4ss"
            };

            postRequest.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpResponseMessage postResponse = await _httpClient.SendAsync(postRequest);
            postResponse.EnsureSuccessStatusCode();
            var jsonContent = await postResponse.Content.ReadAsStringAsync();

            Token token = JsonConvert.DeserializeObject<Token>(jsonContent);

            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwtToken = jwtTokenHandler.ReadJwtToken(token.accessToken);

            var sub = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub").Value;
            var iss = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss").Value;
            var aud = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "aud").Value;

            Assert.Equal(user.Email, sub);
        }

        [Fact]
        public async Task CheckExpirationDate()
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login");

            var user = new UserCredential
            {
                Email = "admin@iseal.com",
                Password = "!se4lP4ss"
            };

            postRequest.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpResponseMessage postResponse = await _httpClient.SendAsync(postRequest);
            postResponse.EnsureSuccessStatusCode();
            var jsonContent = await postResponse.Content.ReadAsStringAsync();

            Token token = JsonConvert.DeserializeObject<Token>(jsonContent);

            Assert.True(token.expiration > DateTime.UtcNow);
        }
    }
}
