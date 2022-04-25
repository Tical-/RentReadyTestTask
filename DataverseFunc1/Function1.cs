using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Caching;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace DataverseFunc1
{
    public static class Function1
    {
        [FunctionName("Function2")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Input>(requestBody);
            string url = "https://org2b4ad762.crm4.dynamics.com";
            string userName = "SanzharSeidakhmetov@RentReady592.onmicrosoft.com";
            string password = "NEWwave320011@@##";
            string conn = $@"Url={url};AuthType=OAuth;UserName={userName};Password={password};AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;LoginPrompt=Auto;RequireNewInstance=True";
            DateTime.TryParseExact(data.properties.StartOn.value, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var Start);
            DateTime.TryParseExact(data.properties.EndOn.value, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var End);
            if (Start >= End)
            {
                return new OkObjectResult("Dates is wrong");
            }

            using (var svc = new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(conn))
            {
                //var condition1 = new ConditionExpression();
                //condition1.AttributeName = "msdyn_start";
                //condition1.Operator = ConditionOperator.GreaterEqual;
                //condition1.Values.Add(Start);
                //var filter1 = new FilterExpression();
                //filter1.Conditions.Add(condition1);

                var condition2 = new ConditionExpression();
                condition2.AttributeName = "msdyn_start";
                condition2.Operator = ConditionOperator.LessEqual;
                condition2.Values.Add(End);
                var filter2 = new FilterExpression();
                filter2.Conditions.Add(condition2);

                var query = new QueryExpression("msdyn_timeentry");
                query.ColumnSet.AddColumns("msdyn_start", "msdyn_end");
                //query.Criteria.AddFilter(filter1);
                query.Criteria.AddFilter(filter2);

                var result = await svc.RetrieveMultipleAsync(query);
                var mas = result.Entities.Select(z => DateTime.Parse(z.Attributes["msdyn_start"].ToString()).ToShortDateString()).ToList();
                while (Start <= End)
                {
                    if (!mas.Contains(Start.ToShortDateString()))
                    {
                        var account = new Entity("msdyn_timeentry");
                        account["msdyn_start"] = Start.ToString("MM.dd.yyyy");
                        account["msdyn_end"] = Start.ToString("MM.dd.yyyy");
                        var resp = await svc.CreateAsync(account);
                    }
                    Start = Start.AddDays(1);
                }
            }


            return new OkObjectResult("");
        }
    }

    public class StartOn
    {
        public string type { get; set; }
        public string format { get; set; }
        public string value { get; set; }
    }

    public class EndOn
    {
        public string type { get; set; }
        public string format { get; set; }
        public string value { get; set; }
    }

    public class Properties
    {
        public StartOn StartOn { get; set; }
        public EndOn EndOn { get; set; }
    }

    public class Input
    {
        [JsonProperty("$schema")]
        public string Schema { get; set; }
        public string type { get; set; }
        public Properties properties { get; set; }
        public List<string> required { get; set; }
    }
}