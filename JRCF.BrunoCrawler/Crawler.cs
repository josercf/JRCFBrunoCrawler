using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Reflection;

namespace JRCF.BrunoCrawler
{
    public static class Crawler
    {
        [FunctionName("Crawler")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ExecutionContext context, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            var stream = new FileStream($@"{context.FunctionDirectory}\index.html", FileMode.Open);

            //var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "JRCF.BrunoCrawler.index.html";
            //Stream stream = assembly.GetManifestResourceStream(resourceName);

            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}
