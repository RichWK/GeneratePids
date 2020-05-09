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

namespace REBGV.Functions
{
    public static class PIDGenerator
    {
        // This is the entry point. Run() extracts a quantity from the incoming HTTP request.

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

            return new OkObjectResult(responseMessage);
        }
    }
}
