using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async void TestMethod1()
        {
            var requestUri = "http://localhost:7071/api/Function2";
            var response = await new HttpClient().GetStringAsync(requestUri).ConfigureAwait(false);
            Assert.IsNotNull(response);
        }
    }
}
