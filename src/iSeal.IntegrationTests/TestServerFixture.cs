using iSeal.Dal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Net.Http;
namespace iSeal.IntegrationTests
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _testServer;
        public HttpClient Client { get; }

        public TestServerFixture()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(getContentRoot())
                .UseEnvironment("Development")
                .UseStartup<API.Startup>()
                .Build();

            _testServer = new TestServer((IWebHostBuilder)builder);

            Client = _testServer.CreateClient();
        }

        private string getContentRoot()
        {
            string testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;

            var relativePathToWebProject = @"..\..\..\..\iSeal.API";

            return Path.Combine(testProjectPath, relativePathToWebProject);
        }

        public void Dispose()
        {
            _testServer.Dispose();
            Client.Dispose();
        }
    }
}
