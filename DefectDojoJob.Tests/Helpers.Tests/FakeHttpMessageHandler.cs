using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

namespace DefectDojoJob.Tests.Helpers.Tests;

/// <summary>
/// Fake the handler to define the response of the http call;
/// Store the request body in the resquestBody property
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode statusCode;
        private readonly string? responseContent;
        public string? requestBody;
      
        public FakeHttpMessageHandler(HttpStatusCode statusCode, string? jsonString = null)
        {
            this.statusCode = statusCode;
            responseContent = jsonString;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                requestBody = await request.Content?.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception e)
            {
                requestBody = "error when retrieving requestBody";
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
