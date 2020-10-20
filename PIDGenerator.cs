using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static REBGV.Functions.Helpers;
using System.IO;



namespace REBGV.Functions
{

    public static class PIDGenerator
    {

        public static IConfiguration Config { get; private set; }

        /* This method, Run(), is the entry point.

        It extracts a quantity from an incoming POST request, so long as a quantity was
        supplied in the body. For a GET request it responds back with instructions.

        It also initiates a connection to an instance of CosmosDB and retrieves the most
        current PID value within a 'PidDbItem' object. */

        [FunctionName("PIDGenerator")]
        public static IActionResult Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                "post",
                Route = null
            )]
            HttpRequest request,
            [CosmosDB(
                databaseName: "pid-database",
                collectionName: "currentPid",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "1",
                PartitionKey = "1"
            )]
            PidDbItem pidDbItem,
            ExecutionContext context,
            ILogger log
        )
        {
            log.LogInformation("PIDGenerator function began processing a request.");

            FetchConfigurationDetails(context);

            string responseMessage = VerifyRequest(request, pidDbItem);

            

            return new OkObjectResult(responseMessage);
        }



        public static void FetchConfigurationDetails(ExecutionContext context)
        {
            /* This pulls in settings regardless of whether the function is running in a
            local development environment or in production on Azure. */

            Config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables() 
                .Build(); 
        }



        public static string VerifyRequest(HttpRequest request, PidDbItem pidDbItem)
        {
            int quantity;
            int finalPid;
            string body = new StreamReader(request.Body).ReadToEnd();
            dynamic contents = JsonConvert.DeserializeObject(body);

            /* 'userInput' is expecting an attribute named 'quantity', sourced from the
            body of the POST request. */

            string userInput = contents?.quantity;

            if (int.TryParse(userInput, out quantity))
            {
                string pids = JsonConvert.SerializeObject(
                    
                    GeneratePids(pidDbItem.CurrentPid, quantity, out finalPid)
                );

                UpdateDatabaseAsync(finalPid);

                return pids;
            }
            else
            {
                return @"This HTTP triggered function generates PIDs. Please pass a
                quantity between 1 and 10 in the request body.";
            }
        }
        
        

        public static async void UpdateDatabaseAsync(int finalPid)
        {
            dynamic item = new { id = "1", currentPid = finalPid };
            
            using CosmosClient client = new CosmosClient(Config["CosmosDBConnection"]);

            Container container = client.GetContainer("pid-database", "currentPid");

            await container.ReplaceItemAsync(item, "1").Response;
        }
    }
}

