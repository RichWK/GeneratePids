using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;


namespace REBGV.Functions
{

    public static class PIDGenerator
    {

        private static IConfiguration _config;
        private static int _quantity;
        private static int _limit { get; } = 10;

        /* This method, Run(), is the entry point. */

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

                using CosmosClient client = new CosmosClient(_config["CosmosDBConnection"]);
                Container container = client.GetContainer("pid-database", "currentPid");

                // TODO: return a different status in the event of a failure.

                return Response(GeneratePids(container));
            }
            else {
                
                return Response(@"This HTTP triggered function generates PIDs. Please
                pass a quantity between 1 and 10 in the request body.");
            }
        }



        public static bool VerifyRequest(HttpRequest request)
        {
            /* Checking if the HTTP request was a POST request that contains a 'quantity'
            attribute in its message body. */
            
            int quantity = 0;

            string requestBody = new StreamReader(request.Body).ReadToEnd();
            dynamic contents = JsonConvert.DeserializeObject(requestBody);

            string userInput = contents?.quantity;
            bool isValidInput = int.TryParse(userInput, out quantity);

            // Validation to ensure the built-in limit isn't exceeded.

            quantity = quantity <= _limit ? quantity : _limit;
            _quantity = quantity;

            return isValidInput;
        }



        public static void FetchConfiguration(ExecutionContext context)
        {
            /* This pulls in config both for local development environments AND for when
            the app is in production in Azure. */

            _config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables() 
                .Build();
        }



        public static string GeneratePids(Container container)
        {
            // The third parameter here is passed to the stored procedure.

            return container.Scripts.ExecuteStoredProcedureAsync<string>(

                "GeneratePids",
                new PartitionKey("1"),
                new dynamic[] { _quantity }

            ).Result.Resource;
        }



        public static IActionResult Response(string message)
        {
            return new OkObjectResult(message);
        }
    }
}

