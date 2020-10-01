using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Collections.Generic;

namespace TestService
{
    public class Function
    {
        private static Dictionary<Guid, User> users = new Dictionary<Guid, User>();

        [FunctionName("GetUser")]
        public IActionResult GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string idInString = req.Query["id"];

            var result = Guid.TryParse(idInString, out Guid id);

            if (!result)
            {
                return new BadRequestObjectResult("Name is empty");
            }
            else
            {
                users.TryGetValue(id, out User user);

                return new OkObjectResult(user);
            }
        }

        [FunctionName("GetUserByRoute")]
        public IActionResult GetUserByRoute(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{id}")] HttpRequest req,
            Guid id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            users.TryGetValue(id, out User user);

            return new OkObjectResult(user);
        }

        [FunctionName("AddUser")]
        public async Task<IActionResult> AddUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user")] HttpRequest req,
            ILogger log)
        {
            
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonSerializer.Deserialize<User>(requestBody);

            var id = Guid.NewGuid();
            users.Add(id, user);

            return new OkObjectResult(id);
        }
    }

    public class User
    {
        public string FirstName{ set; get; }
        public string LastName { set; get; }
    }
}
