using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BM7Tutorial
{
    class PersonDTO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
    }
}
