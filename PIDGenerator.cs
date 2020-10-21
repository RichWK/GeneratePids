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
using System.Threading.Tasks;

namespace REBGV.Functions
{

    public static class PIDGenerator
    {

        public static IConfiguration Config { get; private set; }
        public static PartitionKey Key { get; private set; } = new PartitionKey("1");
        public static int Quantity { get; private set; }
        public static int CurrentPid { get; private set; }
        public static int FinalPid { get; private set; }

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
            ExecutionContext context,
            ILogger log
        )
        {
            log.LogInformation("PIDGenerator function began processing a request.");

            if (VerifyRequest(request))
            {
                FetchConfiguration(context);

                using CosmosClient client = new CosmosClient(Config["CosmosDBConnection"]);
                Container container = client.GetContainer("pid-database", "currentPid");

                ReadFromDatabase(container);

                // string pids = GeneratePids(ReadFromDatabaseAsync(container),Quantity);

                // UpdateDatabaseAsync

                // return Response(pids);

                return Response("");
            }
            else {
                
                return Response("");
            }
        }



        public static void FetchConfiguration(ExecutionContext context)
        {
            /* This pulls in settings for a local development environment AND for when
            the app is in production in Azure. */

            Config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables() 
                .Build();
        }



        public static bool VerifyRequest(HttpRequest request)
        {
            int quantity = 0;

            string requestBody = new StreamReader(request.Body).ReadToEnd();
            dynamic contents = JsonConvert.DeserializeObject(requestBody);

            /* 'userInput' holds the contents of an attribute named 'quantity', which is
            sourced from the body of the POST request. */

            string userInput = contents?.quantity;

            bool isValidInput = int.TryParse(userInput, out quantity);

            Quantity = quantity;

            return isValidInput;
        }



        // public static string VerifyRequestOld(HttpRequest request, PidDbItem pidDbItem)
        // {
        //     int quantity;
        //     int finalPid;
        //     string body = new StreamReader(request.Body).ReadToEnd();
        //     dynamic contents = JsonConvert.DeserializeObject(body);

        //     /* 'userInput' is expecting an attribute named 'quantity', sourced from the
        //     body of the POST request. */

        //     string userInput = contents?.quantity;

        //     if (int.TryParse(userInput, out quantity))
        //     {
        //         string pids = JsonConvert.SerializeObject(
                    
        //             GeneratePids(pidDbItem.CurrentPid, quantity, out finalPid)
        //         );

        //         UpdateDatabaseAsync(finalPid);

        //         return pids;
        //     }
        //     else
        //     {
        //         return @"This HTTP triggered function generates PIDs. Please pass a
        //         quantity between 1 and 10 in the request body.";
        //     }
        // }



        public static void ReadFromDatabase(Container container)
        {
            CurrentPid = int.Parse(container.ReadItemAsync<PidDbItem>("1", Key)
                .Result
                .Resource
                .CurrentPid);
        }
        
        

        public static async void UpdateDatabaseAsync()
        {
            dynamic item = new { id = "1", currentPid = FinalPid };
            
            // using CosmosClient client = new CosmosClient(Config["CosmosDBConnection"]);

            // Container container = client.GetContainer("pid-database", "currentPid");

            // await container.ReplaceItemAsync(item, "1");
        }



        public static IActionResult Response(string message)
        {
            return new OkObjectResult(message);
        }
    }
}

