using System.Collections.Generic;
using Microsoft.Azure.Search.Models;

namespace AzureSearchTest
{
    [SerializePropertyNamesAsCamelCase]
    public class DenormalizedCarModel
    {
        public string Id { get; set; }

        public string MakerId { get; set; }

        public string MakerName { get; set; }

        public string MakerCountry { get; set; }

        public string ModelName { get; set; }

        public double Price { get; set; }

        public List<string> Features { get; set; }

        public DenormalizedCarModel()
        {
            Features = new List<string>();
        }
    }
}