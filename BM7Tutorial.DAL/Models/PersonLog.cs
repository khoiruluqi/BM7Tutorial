using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BM7Tutorial.DAL.Models
{
    public class PersonLog : ModelBase
    {
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
    }
}
