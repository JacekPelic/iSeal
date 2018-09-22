using iSeal.API;
using iSeal.API.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace iSeal.integrationTest
{
    [Collection("iSeal Integration collection")]
    public class ManageOrganizationControllerShould
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _httpClient;

        public ManageOrganizationControllerShould(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Theory]
        [InlineData("iSeal")]
        public async Task GetUsersForExistingOrganization(string organizationName)
        {
            
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/organization/users/{organizationName}");            

            HttpResponseMessage getResponse = await _httpClient.SendAsync(getRequest);

            string jsonContent = await getResponse.Content.ReadAsStringAsync();
            IEnumerable<OrganizationUser> users = JsonConvert.DeserializeObject <IEnumerable<OrganizationUser>>(jsonContent);

            getResponse.EnsureSuccessStatusCode();
            Assert.NotEmpty(users);
        }
    }
}
