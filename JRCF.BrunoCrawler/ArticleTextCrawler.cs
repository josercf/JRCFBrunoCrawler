
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

namespace JRCF.BrunoCrawler
{
    public static class ArticleTextCrawler
    {
        [FunctionName("ArticleTextCrawler")]
        public static  IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            string urlEncoded = req.Query["url"];
            string fileName = req.Query["fileName"];

            var nvc = req.Form.ToDictionary(c => c.Key, x => x.Value);

            if (string.IsNullOrEmpty(urlEncoded))
                urlEncoded = nvc["url"];

            if (string.IsNullOrEmpty(fileName))
                fileName = nvc["fileName"];

            log.Info("Dowloading site {0}", urlEncoded);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(urlEncoded);

            var sb = new StringBuilder();
            IEnumerable<HtmlNode> nodes = document.DocumentNode.Descendants().Where(n =>
                                                   n.NodeType == HtmlNodeType.Text &&
                                                   n.ParentNode.Name != "script" &&
                                                   n.ParentNode.Name != "style");
            foreach (HtmlNode node in nodes)
            {
                if (!node.HasChildNodes)
                {
                    string text = node.InnerText;
                    if (!string.IsNullOrEmpty(text))
                        sb.AppendLine(text.Trim());
                }
            }

            log.Info("C# HTTP trigger function processed a request. File returned: {0}.txt", fileName);

            var contentType = "text/text";
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var result = new FileContentResult(bytes, contentType);
            result.FileDownloadName = $"{fileName}.txt";
            return result;
        }
    }
}
