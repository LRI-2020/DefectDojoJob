using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

namespace DefectDojoJob.Tests.Helpers.Tests;

public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode statusCode;
        private readonly string? responseContent;

        public FakeHttpMessageHandler(HttpStatusCode statusCode, string jsonString = null)
        {
            this.statusCode = statusCode;
            this.responseContent = jsonString;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent??"", Encoding.UTF8, "application/json")
            };
            return await Task.FromResult(response);
        }
    }
