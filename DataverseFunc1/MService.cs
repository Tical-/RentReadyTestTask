using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DataverseFunc1
{
    public interface MService : IOrganizationService,
        IOrganizationServiceAsync2,
        IOrganizationServiceAsync,
        IDisposable
    {

    }
    public class MClient : Microsoft.PowerPlatform.Dataverse.Client.ServiceClient, MService
    {
        private static string _dataverseConnectionString;
        private static string url = "https://org2b4ad762.crm4.dynamics.com";
        private static string userName = "SanzharSeidakhmetov@RentReady592.onmicrosoft.com";
        private static string password = "NEWwave320011@@##";
        public static string con = $@"Url={url};AuthType=OAuth;UserName={userName};Password={password};AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;LoginPrompt=Auto;RequireNewInstance=True";
        public MClient() : base(con)
        {

        }
    }
}