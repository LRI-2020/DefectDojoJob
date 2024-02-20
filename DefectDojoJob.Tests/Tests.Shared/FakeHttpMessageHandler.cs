using System.Net;
using System.Text;

namespace DefectDojoJob.Tests.Tests.Shared;

/// <summary>
/// Fake the handler to define the response of the http call;
/// Store the request body in the request body property
/// Store the request Url in the request Url property
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode statusCode;
        private readonly string? responseContent;
        public string? RequestBody;
        public Uri? RequestUrl;
      
        public FakeHttpMessageHandler(HttpStatusCode statusCode, string? jsonString = null)
        {
            this.statusCode = statusCode;
            responseContent = jsonString;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            try
            {
                RequestUrl = request.RequestUri;
                RequestBody = await (request.Content ?? new StringContent("")).ReadAsStringAsync(cancellationToken)??"";
            }
            catch (Exception e)
            {
                RequestBody = "error when retrieving requestBody";
                Console.WriteLine(e.Message);
            }
            var response = new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent??"", Encoding.UTF8, "application/json")
            };
            return await Task.FromResult(response);
        }
    }
