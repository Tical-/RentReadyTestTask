using System;
using System.Collections.Generic;
using DataverseFunc1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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
            mock.Setup(z => z.RetrieveMultipleAsync(MSDYNAddRange.GetQuery(Start, End))).ReturnsAsync(coll);
            //mock.Setup(z => z.CreateAsync(exp)).ReturnsAsync(coll);
            MSDYNAddRange.svc = mock.Object;
            MSDYNAddRange.exp = exp;
            await MSDYNAddRange.Run(request, logger);
        }
    }
}