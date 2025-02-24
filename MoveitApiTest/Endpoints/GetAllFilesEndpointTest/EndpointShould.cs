using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoveitApi.Controllers;
using MoveitClient;
using MoveitClient.Models.ResponseModels;
using Newtonsoft.Json;

namespace MoveitApiTest.Endpoints.GetAllFilesEndpointTest
{
    public class EndpointShould
    {
        private readonly GetAllFilesEndpoint _getAllFilesEndpoint;
        private readonly Mock<IClient> _mockMoveitClient;

        public EndpointShould()
        {
            _getAllFilesEndpoint = new GetAllFilesEndpoint();
            _mockMoveitClient = new Mock<IClient>();
        }

        [Test]
        public async Task GetAllFilesWhenTokeIsInvalid()
        {
            var result = await _getAllFilesEndpoint.GetAllFiles(null, CancellationToken.None, null);

            Assert.That(result, Is.TypeOf<UnauthorizedHttpResult>());
        }

        [Test]
        public async Task GetAllFilesReturnsFilesSuccesfully()
        {
            var getfilesResponse = new GetFilesResponse();
            getfilesResponse.Items = new List<Item>();

            for (int i = 0; i < 10; i++)
            {
                var item = new Item();
                item.Name = $"file{i}";
                item.Size = i;
                item.Path = $"path{i}";
                getfilesResponse.Items.Add(item);
            }


            _mockMoveitClient.Setup(m => m.GetAllFilesAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(getfilesResponse);

            var result = await _getAllFilesEndpoint.GetAllFiles("token", CancellationToken.None, _mockMoveitClient.Object);

            var okResult = result as Ok<GetFilesResponse>;
            var response = okResult.Value;

            Assert.That(result, Is.Not.Null);
            Assert.That(response.Items.Count, Is.EqualTo(10));

            for (int i = 0; i < 10; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.Items[i].Name, Is.EqualTo($"file{i}"));
                    Assert.That(response.Items[i].Size, Is.EqualTo(i));
                    Assert.That(response.Items[i].Path, Is.EqualTo($"path{i}"));
                });
            }
        }
    }
}
