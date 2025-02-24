using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using MoveitApi.Controllers;
using MoveitApi.SignalR;
using MoveitClient;
using System.Net;
using System.Text;

namespace MoveitApiTest.Endpoints.DeleteFileEndpointTest
{
    public class EndpointShould
    {
        private readonly DeleteFileEndpoint _deleteFileEndpoint;
        private readonly Mock<IClient> _mockMoveitClient;
        private readonly Mock<IHubContext<FileObserverHub>> _mockHubContext;

        public EndpointShould()
        {
            _deleteFileEndpoint = new DeleteFileEndpoint();
            _mockMoveitClient = new Mock<IClient>();
            _mockHubContext = new Mock<IHubContext<FileObserverHub>>();

            _mockHubContext.Setup(x => x.Clients.All.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        }

        [Test]
        public async Task DeleteFileWhenTokeIsInvalid()
        {
            var result = await _deleteFileEndpoint.DeleteFile(null, "fileId", CancellationToken.None, null, null);

            Assert.That(result, Is.TypeOf<UnauthorizedHttpResult>());
        }

        [Test]
        public async Task DeleteFileSuccesfully()
        {
            var fileId = "testFile";
            var token = "token";
            var responseMessage = new HttpResponseMessage();

            _mockMoveitClient.Setup(x => x.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseMessage);

            var result = await _deleteFileEndpoint.DeleteFile(token, fileId, CancellationToken.None, _mockMoveitClient.Object, _mockHubContext.Object);
            
            Assert.That(responseMessage.IsSuccessStatusCode, Is.EqualTo(true));
            Assert.That(result, Is.InstanceOf<Ok<string>>());
        }

        [Test]
        public async Task DeleteFileReturnsBadRequest()
        {
            var fileId = "file123";
            var token = "valid_token";
            var responseContent = "Error deleting file";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };
            _mockMoveitClient.Setup(m => m.DeleteFileAsync(fileId, token))
                .ReturnsAsync(responseMessage);

            var result = await _deleteFileEndpoint.DeleteFile(token, fileId, CancellationToken.None, _mockMoveitClient.Object, _mockHubContext.Object);

            Assert.That(result, Is.InstanceOf<BadRequest<string>>());
        }
    }
}
