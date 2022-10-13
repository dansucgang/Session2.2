using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using RestSharp;
using System.Net;

namespace Session2_2
{
    [TestClass]
    public class RestSharp
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostMethod()
        {
            #region CreateUser
            //Create User

            List<Tags> tags = new List<Tags>();
            tags.Add(new Tags()
            {
                Id = 8099,
                Name = "Persian"
            });


            PetModel petData = new PetModel()
            {
                Id = 8099,
                Name = "Elephant",
                PhotoUrls = new List<string> { "Photo_String" },
                Category = new Category()
                {
                    Id = 8099,
                    Name = "Elephant"
                },
                Tags = tags,
                Status = "available"
            };

            // Send Post Request
            var temp = GetURI(PetEndpoint);
            var postRestRequest = new RestRequest(GetURI(PetEndpoint)).AddJsonBody(petData);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);
            #endregion

            #region GetUser
            var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{petData.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<PetModel>(restRequest);
            #endregion

            #region Assertions
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(petData.Name, restResponse.Data.Name, "Name did not match.");
            Assert.AreEqual(petData.Status, restResponse.Data.Status, "Status did not match.");
            Assert.AreEqual(petData.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "PhotoUrls did not match.");
            Assert.AreEqual(petData.Category.Id, restResponse.Data.Category.Id, "Category ID did not match.");
            Assert.AreEqual(petData.Category.Name, restResponse.Data.Category.Name, "Category Name did not match.");
            Assert.AreEqual(petData.Tags[0].Id, restResponse.Data.Tags[0].Id, "Tags ID did not match.");
            Assert.AreEqual(petData.Tags[0].Name, restResponse.Data.Tags[0].Name, "Tags Name did not match.");
            ;
            #endregion

            #region CleanUp
            cleanUpList.Add(petData);
            #endregion
        }
    }
}