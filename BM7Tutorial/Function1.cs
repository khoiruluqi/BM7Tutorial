using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using static BM7Tutorial.DAL.Repositories;
using System.Collections.Generic;
using BM7Tutorial.DAL.Models;
using BM7Tutorial.BLL;

namespace BM7Tutorial
{
    public static class Function1
    {
        [FunctionName("GetAllPerson")]
        public static async Task<IActionResult> GetAllPerson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            var query = new SqlQuerySpec("SELECT * FROM c");
            var pk = new PartitionKey("Person");
            var options = new FeedOptions() { PartitionKey = pk };
            var data = documentClient.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri("Course", "Person"), query, options);
            return new OkObjectResult(data);
        }

        [FunctionName("GetPersonById")]
        public static IActionResult GetPersonById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Person/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "Template",
                collectionName: "Person",
                ConnectionStringSetting = "cosmos-bl-tutorial-serverless",
                Id = "{id}",
                PartitionKey = "Person")] Person person,
            ILogger log)
        {
            return new OkObjectResult(person);
        }

        [FunctionName("CreateData")]
        public static async Task<IActionResult> CreateData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Person/create")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<PersonDTO>(requestBody);
            var pk = new PartitionKey(data.City);
            var options = new RequestOptions() { PartitionKey = pk };

            var person = new Person
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = data.FirstName,
                LastName = data.LastName,
                City = data.City
            };

            await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("Course", "Person"), person);

            return new OkObjectResult(person);
        }

        [FunctionName("NexusCreateData")]
        public static async Task<IActionResult> NexusCreateData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Nexus/Person/create")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<PersonDTO>(requestBody);

            var repsPerson = new PersonRepository(documentClient);
            var person = new Person
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                City = data.City
            };

            await repsPerson.CreateAsync(person);

            return new OkObjectResult(person);
        }

        [FunctionName("NexusGetPerson")]
        public static async Task<IActionResult> NexusGetPerson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Nexus/Person")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            var repsPerson = new PersonRepository(documentClient);
            //var pk = new Dictionary<string, string> { { "City", "Depok" } };
            var data = await repsPerson.GetAsync();
            return new OkObjectResult(data);
        }

        [FunctionName("NexusGetPersonById")]
        public static async Task<IActionResult> NexusGetPersonById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Nexus/Person/{personId}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string personId,
            ILogger log)
        {
            var repsPerson = new PersonRepository(documentClient);
            var pk = new Dictionary<string, string> { { "City", "Depok" } };
            var person = await repsPerson.GetByIdAsync(personId, partitionKeys: pk);
            return new OkObjectResult(person);
        }

        [FunctionName("UTGetPersonById")]
        public static async Task<IActionResult> UTGetPersonById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "UT/Person/{personId}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string personId,
            ILogger log)
        {
            try
            {
                var repsPerson = new PersonRepository(documentClient);
                var pk = new Dictionary<string, string> { { "City", "Depok" } };
                var personService = new PersonService(repsPerson);
                var person = await personService.GetPersonById(personId, pk);
                return new OkObjectResult(person);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [FunctionName("NexusDeletePersonById")]
        public static async Task<IActionResult> NexusDeletePersonById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Nexus/Person/{personId}/Delete")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string personId,
            ILogger log)
        {
            var repsPerson = new PersonRepository(documentClient);
            var pk = new Dictionary<string, string> { { "City", "Depok" } };
            await repsPerson.DeleteAsync(personId);
            return new OkResult();
        }
    }
}
