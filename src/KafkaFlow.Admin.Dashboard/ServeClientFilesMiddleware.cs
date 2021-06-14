namespace KafkaFlow.Admin.Dashboard
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    internal class ServeClientFilesMiddleware : IMiddleware
    {
        private const string DashboardUrlPrefix = "/kafka-flow";

        private readonly string[] homeUrls =
        {
            DashboardUrlPrefix,
            DashboardUrlPrefix + "/",
            DashboardUrlPrefix + "/index.html",
        };

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var requestUrl = context.Request.Path.Value.ToLower();

            if (!requestUrl.StartsWith(DashboardUrlPrefix))
            {
                await next(context);
                return;
            }

            var fileName = this.IsHomeUrl(requestUrl) ?
                "index.html" :
                Path.GetFileName(requestUrl);

            await this.RespondResourceFileAsync(context, fileName);
        }

        private static StringValues ResolveContentType(string resourceName)
        {
            return Path.GetExtension(resourceName) switch
            {
                ".js" => "text/javascript",
                ".css" => "text/css",
                ".html" => "text/html",
                _ => throw new NotSupportedException("MIME type not supported")
            };
        }

        private async Task RespondResourceFileAsync(HttpContext context, string fileName)
        {
            var assembly = this.GetType().Assembly;

            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(fileName));

            if (resourceName is null)
            {
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                await context.Response.WriteAsync(string.Empty);
                return;
            }

            var resourceStream = assembly.GetManifestResourceStream(resourceName);

            context.Response.Headers["Content-Type"] = ResolveContentType(resourceName);
            await resourceStream.CopyToAsync(context.Response.Body);
        }

        private bool IsHomeUrl(string url)
        {
            return this.homeUrls.Any(home => home == url);
        }
    }
}
