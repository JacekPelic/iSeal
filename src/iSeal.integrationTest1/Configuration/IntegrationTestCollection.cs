using iSeal.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace iSeal.integrationTest.Configuration
{
    [CollectionDefinition("iSeal Integration collection")]
    public class IntegrationTestCollection : ICollectionFixture<TestFixture<Startup>>
    {
    }
}
