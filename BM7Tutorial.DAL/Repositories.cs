using BM7Tutorial.DAL.Models;
using Microsoft.Azure.Documents.Client;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BM7Tutorial.DAL
{
    public class Repositories
    {
        private static readonly string C_EventGridEndPoint = Environment.GetEnvironmentVariable("EventGridEndPoint");
        private static readonly string C_EventGridKey = Environment.GetEnvironmentVariable("EventGridKey");

        public class PersonRepository : DocumentDBRepository<Person>
        {
            public PersonRepository(DocumentClient client) :
                base(databaseId: "Course", cosmosDBClient: client, partitionProperties: "City",
                    eventGridEndPoint: C_EventGridEndPoint, eventGridKey: C_EventGridKey)
            { }
        }

        public class PersonLogRepository : DocumentDBRepository<PersonLog>
        {
            public PersonLogRepository(DocumentClient client) :
                base(databaseId: "Course", cosmosDBClient: client, partitionProperties: "City",
                    eventGridEndPoint: "", eventGridKey: "")
            { }
        }
    }
}
