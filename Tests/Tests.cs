using System;
using System.Collections.Generic;
using DataverseFunc1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Xunit;

namespace Tests
{
    public partial class Tests
    {
        private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger("Test");
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public async void MSDYNAddRangeTest()
        {
            var Start = new DateTime(2022, 02, 11);
            var End = new DateTime(2022, 02, 13);
            Guid first = Guid.Parse("de3ab269-8517-4785-8cd4-f612ff363b10");
            Guid second = Guid.Parse("78d61aa4-3c5a-4a93-9adb-4d11d65150d7");
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(GenerateJson(Start, End))
            };
            var mock = new Mock<MService>();
            var list = new List<Entity>();
            var ent = new Entity("msdyn_timeentry");
            ent.Attributes.Add("msdyn_start", Start.ToShortDateString());
            ent.Attributes.Add("msdyn_end", Start.ToShortDateString());
            list.Add(ent);
            var coll = new EntityCollection(list); 
            var exp = MSDYNAddRange.GetQuery(Start, End);
            mock.Setup(z => z.RetrieveMultipleAsync(exp)).ReturnsAsync(coll);
            //mock.Setup(z => z.CreateAsync(It.IsAny<Entity>())).ReturnsAsync(Guid.NewGuid);
            var account1 = new Entity("msdyn_timeentry");
            var test = Start.AddDays(1).ToString("MM.dd.yyyy");
            account1["msdyn_start"] = Start.AddDays(1).ToString("MM.dd.yyyy");
            account1["msdyn_end"] = Start.AddDays(1).ToString("MM.dd.yyyy");
            mock.Setup(z => z.CreateAsync(account1)).ReturnsAsync(first);
            var account2 = new Entity("msdyn_timeentry");
            account2["msdyn_start"] = Start.AddDays(2).ToString("MM.dd.yyyy");
            account2["msdyn_end"] = Start.AddDays(2).ToString("MM.dd.yyyy");
            mock.Setup(z => z.CreateAsync(account2)).ReturnsAsync(second);
            MSDYNAddRange.svc = mock.Object;
            MSDYNAddRange.exp = exp;
            var res = (OkObjectResult)await MSDYNAddRange.Run(request, logger);
            Assert.NotEqual(((List<Guid>)res.Value)[0], new Guid());
            Assert.NotEqual(((List<Guid>)res.Value)[1], new Guid());
            Assert.Equal(((List<Guid>)res.Value)[0], first);
            Assert.Equal(((List<Guid>)res.Value)[1], second);
        }
    }
}