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

namespace DataverseFunc1
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
                throw new Exception("Dates is wrong");
            }
            var result = await svc.RetrieveMultipleAsync(exp?? GetQuery(Start, End));
            var mas = result.Entities.Select(z => DateTime.Parse(z.Attributes["msdyn_start"].ToString()).ToShortDateString()).ToList();
            while (Start <= End)
            {
                if (!mas.Contains(Start.ToShortDateString()))
                {
                    var account = new Entity("msdyn_timeentry");
                    account["msdyn_start"] = Start.ToString("MM.dd.yyyy");
                    account["msdyn_end"] = Start.ToString("MM.dd.yyyy");
                    output.Add(await svc.CreateAsync(account));
                }
                Start = Start.AddDays(1);
            }
            return new OkObjectResult(output);
        }
    }
}

//Сейчас мы добавляем рандомные даты, но мы хотим наверное проверить даты которые нужны в Entity CreateAsync
//Но хотя непонятно, думаю нужно вручную указывать даты входящих параметров для проверки алгоритма
//Без указания Алгоритма, просто указать 2 Entity какие должны быть вместо It.IsAny<Entity>()