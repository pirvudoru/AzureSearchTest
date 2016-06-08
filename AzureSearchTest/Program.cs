using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace AzureSearchTest
{
    class Program
    {
        private const string IndexName = "carmodelindex";

        private static SearchServiceClient _client;

        static void Main(string[] args)
        {
            string url = null;
            string adminKey = null;

            _client = new SearchServiceClient(new Uri(url), new SearchCredentials(adminKey));

//            CreateCarModelIndex();
//            PopulateIndex();

            DoSearch();
        }

        private static void CreateCarModelIndex()
        {
            _client.Indexes.Delete(IndexName);

            var index = new Index
            {
                Name = IndexName,
                Fields = new List<Field>
                {
                    new Field("id", DataType.String) { IsKey = true, IsRetrievable = true },
                    new Field("makerId", DataType.String) { IsFacetable = true, IsRetrievable  = true },
                    new Field("makerName", DataType.String) { IsSearchable = true },
                    new Field("makerCountry", DataType.String) { IsFilterable = true },
                    new Field("modelName", DataType.String) { IsSearchable = true },
                    new Field("price", DataType.Double) { IsSortable = true, IsFilterable = true },
                    new Field("features", DataType.Collection(DataType.String)) { IsFilterable = true }
                }
            };

            _client.Indexes.Create(index);
        }

        private static void DoSearch()
        {
            var client = _client.Indexes.GetClient(IndexName);

            var searchParameters = new SearchParameters
            {
                SearchMode = SearchMode.Any,
//                Filter = "makerCountry eq 'Germany' and price gt 9 and price lt 101 and features/any(f: f eq '1') and features/any(f: f eq '2')",
                Filter = "makerCountry eq 'Germany' and price gt 9 and price lt 101",
                Facets = new List<string> { "makerId" },
                OrderBy = new List<string> { "price" }
            };
            var documentSearchResult = client.Documents.Search("*", searchParameters);
            var facets = documentSearchResult.Facets;
            var result = documentSearchResult.Results.Select(r => new { id = r.Document["id"], makerId = r.Document["makerId"] }).ToList();
        }

        private static void PopulateIndex()
        {
            var client = _client.Indexes.GetClient(IndexName);

            var models = GetDenormalizedModels().Select(IndexAction.Upload).ToList();
            client.Documents.Index(new IndexBatch<DenormalizedCarModel>(models));
        }

        private static List<DenormalizedCarModel> GetDenormalizedModels()
        {
            var audi = new CarMaker { Id = 1, MakerName = "Audi", MakerCountry = "Germany" };
            var bmw = new CarMaker { Id = 2, MakerName = "BMW", MakerCountry = "Germany" };
            var renault = new CarMaker { Id = 3, MakerName = "Renault", MakerCountry = "France" };

            var brakes = new CarFeature { Id = 1, Name = "Brakes" };
            var windows = new CarFeature { Id = 2, Name = "Windows" };
            var mirrors = new CarFeature { Id = 3, Name = "Mirrors" };
            
            return new List<CarModel>
            {
                new CarModel
                {
                    Id = 1,
                    ModelName = "A1",
                    Maker = audi,
                    Price = 10,
                    Features = new List<CarFeature> { brakes, windows }
                },
                new CarModel
                {
                    Id = 2,
                    ModelName = "A6",
                    Maker = audi,
                    Features = new List<CarFeature> { brakes, mirrors },
                    Price = 100
                },
                new CarModel
                {
                    Id = 3,
                    ModelName = "A4",
                    Maker = audi,
                    Price = 100,
                    Features = new List<CarFeature> { windows }
                },


                new CarModel
                {
                    Id = 4,
                    ModelName = "320",
                    Maker = bmw,
                    Price = 20,
                    Features = new List<CarFeature> { brakes }
                },
                new CarModel
                {
                    Id = 5,
                    ModelName = "520",
                    Maker = bmw,
                    Price = 60,
                    Features = new List<CarFeature> { mirrors }
                },
                new CarModel
                {
                    Id = 6,
                    ModelName = "120",
                    Maker = bmw,
                    Price = 70,
                    Features = new List<CarFeature> { windows }
                },

                new CarModel
                {
                    Id = 7,
                    ModelName = "320",
                    Maker = renault,
                    Price = 1,
                    Features = new List<CarFeature> { brakes }
                },
                new CarModel
                {
                    Id = 8,
                    ModelName = "520",
                    Maker = renault,
                    Price = 2,
                    Features = new List<CarFeature> { mirrors }
                },
                new CarModel
                {
                    Id = 9,
                    ModelName = "120",
                    Maker = renault,
                    Price = 3,
                    Features = new List<CarFeature> { windows }
                }
            }
                .Select(m => new DenormalizedCarModel
                {
                    Id = m.Id.ToString(),
                    ModelName = m.ModelName,
                    MakerId = m.Maker.Id.ToString(),
                    MakerName = m.Maker.MakerName,
                    MakerCountry = m.Maker.MakerCountry,
                    Price = m.Price,
                    Features = m.Features.Select(f => f.Id.ToString()).ToList(),
                })
                .ToList();
        }
    }
}
