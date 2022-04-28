using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using Exception = System.Exception;

namespace DataverseFunc
{
    public static partial class MSDYNAddRange
    {
        public static MService svc;
        public static QueryExpression exp;
        [FunctionName("MSDYNAddRange")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            var output = new List<Guid>();
            svc ??= new MClient();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Input>(requestBody);
            DateTime.TryParseExact(data.properties.StartOn.value, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var Start);
            DateTime.TryParseExact(data.properties.EndOn.value, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var End);
            if (Start >= End)
            {
                return new OkObjectResult("Dates is wrong");
            }
            var result = await svc.RetrieveMultipleAsync(exp?? GetQuery(Start, End));
            var mas = result.Entities.Select(z => DateTime.Parse(z.Attributes["msdyn_start"].ToString()).ToShortDateString()).ToList();
            while (Start <= End)
            {
                if (!mas.Contains(Start.ToShortDateString()))
                {
                    var account = new Entity("msdyn_timeentry")
                    {
                        ["msdyn_start"] = Start.ToString("MM.dd.yyyy"),
                        ["msdyn_end"] = Start.ToString("MM.dd.yyyy")
                    };
                    output.Add(await svc.CreateAsync(account));
                }
                Start = Start.AddDays(1);
            }
            return new OkObjectResult(output);
        }
    }
}