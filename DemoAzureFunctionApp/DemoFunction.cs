using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DemoFunction
{
    public static class DemoFunction
    {
        [FunctionName("DemoFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["APICheck"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.APICheck;

            string APIResponse = string.Empty;
            string url = string.Empty;

            if (name == "GET")
            {
                url = @"https://reqres.in/api/unknown/2";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    APIResponse = reader.ReadToEnd();
                }
            }
            else if (name == "POST")
            {
                var formVars = new Dictionary<string, string>();
                formVars.Add("email", "eve.holt@reqres.in");
                formVars.Add("password", "cityslicka");
                var content = new FormUrlEncodedContent(formVars);

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = client.PostAsync("https://reqres.in/api/login", content).Result;
                APIResponse = result.ToString();
            }
            //test end 

            return (APIResponse != null && APIResponse != string.Empty) 
                    ? (ActionResult)new OkObjectResult($"APIResponse , {APIResponse} ")
                    : new BadRequestObjectResult("Please pass a APICheck=GET or APICheck=POST in the query string to test get or post requests from Azure function.");
        }



    }
}

