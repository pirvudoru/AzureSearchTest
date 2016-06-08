using System.Collections.Generic;

namespace AzureSearchTest
{
    public class CarModel
    {
        public int Id { get; set; }

        public string ModelName { get; set; }

        public double Price { get; set; }

        public List<CarFeature> Features { get; set; }

        public CarMaker Maker { get; set; }

        public CarModel()
        {
            Features = new List<CarFeature>();
        }
    }
}