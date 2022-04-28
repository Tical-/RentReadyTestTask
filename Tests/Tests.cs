using System;
using System.Collections.Generic;
using DataverseFunc;
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
        [Fact]
        public async void MSDYNAddRangeTest()
        {
            var Start = new DateTime(2022, 02, 11);
            var End = new DateTime(2022, 02, 13);
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
            mock.Setup(z => z.CreateAsync(It.IsAny<Entity>())).ReturnsAsync(Guid.NewGuid);
            MSDYNAddRange.svc = mock.Object;
            MSDYNAddRange.exp = exp;
            var res = (OkObjectResult)await MSDYNAddRange.Run(request, logger);
            Assert.NotEqual(((List<Guid>)res.Value)[0], new Guid());
            Assert.NotEqual(((List<Guid>)res.Value)[1], new Guid());
            Assert.NotNull(((List<Guid>)res.Value)[0]);
            Assert.NotNull(((List<Guid>)res.Value)[1]);
            Assert.Equal(2, ((List<Guid>)res.Value).Count);
        }
        [Fact]
        public async void MSDYNAddRangeWrongDateTest()
        {
            var Start = new DateTime(2022, 02, 13);
            var End = new DateTime(2022, 02, 11);
            var res = (OkObjectResult)await MSDYNAddRange.Run(new DefaultHttpRequest(new DefaultHttpContext()), logger);
            Assert.Equal("Date is wrong", res.ToString());
        }
    }
}