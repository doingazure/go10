using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DoingAzure.SslDays
{
    record SslDays(int sslDays)
    {
        public SslDays() : this(42)
        {
        }
    }

    public static class ssldays
    {
        [FunctionName("ssldays")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;


            string url = name;
            DateTime expirationDate = GetSslCertificateExpiration(url);
            Console.WriteLine($"The SSL certificate for {url} expires on {expirationDate}");
            Console.WriteLine($"there are {(expirationDate - DateTime.Now).TotalDays} full days left");

            // See https://aka.ms/new-console-template for more information
            // Console.WriteLine("Hello, World!");

            var ssldays = expirationDate.Subtract(DateTime.Now).Days;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                // : $"Hello, {name}. This HTTP triggered function executed successfully.";
                : $"{ssldays}";

            var jsonResponse = new SslDays(ssldays);
            return new OkObjectResult(jsonResponse);
        }

        DateTime GetSslCertificateExpiration(string url, int port = 443)
        {
            using (TcpClient client = new TcpClient(url, port))
            {
                using (SslStream sslStream = new SslStream(client.GetStream(), false,
                   new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; }), null))
                {
                    sslStream.AuthenticateAsClient(url);
                    return ((X509Certificate2)sslStream.RemoteCertificate).NotAfter;
                }
            }
        }

    }
}
