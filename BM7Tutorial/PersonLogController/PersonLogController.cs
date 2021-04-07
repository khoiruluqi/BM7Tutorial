using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BM7Tutorial.DAL.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.Base.EventHubExtensions;
using static BM7Tutorial.DAL.Repositories;

namespace BM7Tutorial.PersonLogController
{
    public static class PersonLogController
    {
        [FunctionName("PersonLogController")]
        public static async Task Run([EventHubTrigger("person.personlog", Connection = "evh-bl-tutorial")] EventData[] events,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            var eventList = events.GetData<EventGridEvent>();

            foreach (var eventData in eventList)
            {
                try
                {
                    var person = JsonConvert.DeserializeObject<Person>(eventData.Data.ToString());
                    var personLog = new PersonLog()
                    {
                        Name = person.FirstName + " " + person.LastName,
                        City = person.City,
                        Data = eventData.Data.ToString()
                    };

                    var repsPersonLog = new PersonLogRepository(documentClient);
                    await repsPersonLog.CreateAsync(personLog);
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
