using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace REBGV.Functions
{
    public static class PIDGenerator
    {

        // Run() is the entry point: it extracts a quantity from the incoming HTTP request.

        [FunctionName("PIDGenerator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("PIDGenerator function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // 'userInput' expects an argument named 'quantity' in the request body.

            string userInput = data?.quantity;

            int quantity;

            string responseMessage = int.TryParse(userInput, out quantity)
                ? JsonConvert.SerializeObject(GeneratePIDs.Generate(quantity))
                : "This HTTP triggered function generates PIDs. Please pass a quantity between 1 and 10 in the request body.";

            // responseMessage += FetchCurrentPidFromCosmosDb();

            return new OkObjectResult(responseMessage);
        }


        private static async Task<string> FetchCurrentPidFromCosmosDb(
            [CosmosDB(
                databaseName: "pid-database",
                collectionName: "currentPid",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{Query.id}",
                PartitionKey = "{Query.partitionKey}")] ToDoItem toDoItem
            )
        {
            return "";
        }


        // private static async Task<string> FetchLatestPidFromBlobStorage()
        // {
        //     string sasToken = "secret...";
        //     string storageAccount = "rebgvstordev";
        //     string containerName = "Container Name";
        //     string blobName = "LatestPID.txt";

        //     string requestUri = $"https://{storageAccount}.blob.core.windows.net/{containerName}/{blobName}?{sasToken}";

        //     HttpWebRequest request = WebRequest.CreateHttp(requestUri);
        //     request.Method = "GET";

        //     using Stream requestStream = request.GetRequestStreamAsync().Result;
        //     string fileData = await requestStream.ReadAsync();
            
        //     using HttpWebResponse resp = (HttpWebResponse)request.GetResponseAsync().Result;

        //     if (resp.StatusCode == HttpStatusCode.OK)
        //     {
        //         return "";
        //     }
        //     else {
        //         return "";
        //     }
        // }
        

    }
}
