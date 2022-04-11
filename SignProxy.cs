using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace SignSystem
{
    internal class SignProxy
    {


        private ProxyServer  proxyServer = new ProxyServer();

        public string? target { get; set; }
        public string? template { get; set; }

        // locally trust root certificate used by this proxy 
        //proxyServer.CertificateManager.TrustRootCertificate(true);

        // optionally set the Certificate Engine
        // Under Mono only BouncyCastle will be supported
        //proxyServer.CertificateManager.CertificateEngine = Network.CertificateEngine.BouncyCastle;
        public void StartProxyServer()
        {
            proxyServer.BeforeRequest += OnRequest;
            proxyServer.BeforeResponse += OnResponse;
            proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            proxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;


        }


        public void SetProxyPort(int port)
        {
            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, port, true)
            {
                // Use self-issued generic certificate on all https requests
                // Optimizes performance by not creating a certificate for each https-enabled domain
                // Useful when certificate trust is not required by proxy clients
                //GenericCertificate = new X509Certificate2(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "genericcert.pfx"), "password")
            };

            // Fired when a CONNECT request is received
            //explicitEndPoint.BeforeTunnelConnect += OnBeforeTunnelConnect;

            // An explicit endpoint is where the client knows about the existence of a proxy
            // So client sends request in a proxy friendly manner
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();

            //proxyServer.UpStreamHttpProxy = new ExternalProxy() { HostName = "localhost", Port = 8888 };
            //proxyServer.UpStreamHttpsProxy = new ExternalProxy() { HostName = "localhost", Port = 8888 };
            foreach (var endPoint in proxyServer.ProxyEndPoints)
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
                    endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);

            // Only explicit proxies can be set as system proxy!
            proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);

        }

        public void StopProxyServer()
        {

            // Unsubscribe & Quit
            //explicitEndPoint.BeforeTunnelConnect -= OnBeforeTunnelConnect;
            proxyServer.BeforeRequest -= OnRequest;
            proxyServer.BeforeResponse -= OnResponse;
            proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
            proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;

            proxyServer.Stop();

        }

        private async Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;

            if (hostname.Contains("dropbox.com"))
            {
                // Exclude Https addresses you don't want to proxy
                // Useful for clients that use certificate pinning
                // for example dropbox.com
                e.DecryptSsl = false;
            }
        }

        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("neusoft.com"))
            {
                Console.WriteLine(e.HttpClient.Request.Url);
            }

            // read request headers
            var requestHeaders = e.HttpClient.Request.Headers;

            var method = e.HttpClient.Request.Method.ToUpper();
            if ((method == "POST" || method == "PUT" || method == "PATCH"))
            {
                if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("jigsawVerify"))
                {
                   // await SaveImage(stringResponse);
                   Console.WriteLine("try get image");

                }
                // Get/Set request body bytes
                byte[] bodyBytes = await e.GetRequestBody();
                e.SetRequestBody(bodyBytes);

                // Get/Set request body as string
                string bodyString = await e.GetRequestBodyAsString();
                e.SetRequestBodyString(bodyString);

                // store request 
                // so that you can find it from response handler 
                e.UserData = e.HttpClient.Request;
            }

            // To cancel a request with a custom HTML content
            // Filter URL
            if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("360"))
            {
                e.Ok("<!DOCTYPE html>" +
                    "<html><body><h1>" +
                    "Website Blocked" +
                    "</h1>" +
                    "<p>Blocked by titanium web proxy.</p>" +
                    "</body>" +
                    "</html>");
            }

            // Redirect example
            if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("wikipedia.org"))
            {
                e.Redirect("https://www.paypal.com");
            }
        }

        // Modify response
        private async Task OnResponse(object sender, SessionEventArgs e)
        {

            // Console.WriteLine(e.HttpClient.Response.ToString());

            // read response headers
            var responseHeaders = e.HttpClient.Response.Headers;

            //if (!e.ProxySession.Request.Host.Equals("medeczane.sgk.gov.tr")) return;
            if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
            {

                if (e.HttpClient.Response.StatusCode == 200)
                {
                    string stringResponse = await e.GetResponseBodyAsString();
                    Console.WriteLine(e.HttpClient.Request.RequestUri.AbsoluteUri);
                    if ("http://kq.neusoft.com/jigsaw".Equals(e.HttpClient.Request.Url))
                    {
                        await SaveImage(stringResponse);
                       // Console.WriteLine("finish download");

                    }
                    /* if (e.HttpClient.Response.ContentType != null && e.HttpClient.Response.ContentType.Trim().ToLower().Contains("text/html"))
                     {
                         byte[] bodyBytes = await e.GetResponseBody();
                         e.SetResponseBody(bodyBytes);

                         string body = await e.GetResponseBodyAsString();
                         e.SetResponseBodyString(body);
                     }*/
                }
            }

            if (e.UserData != null)
            {
                // access request from UserData property where we stored it in RequestHandler
                var request = (Request)e.UserData;
            }
        }

        private async Task SaveImage(String stringResponse)
        {
            //  throw new NotImplementedException();
            await Task.Run(async () =>
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(stringResponse);
                template = jo["smallImage"].ToString();
                target = jo["bigImage"].ToString();
                template = "http://kq.neusoft.com/upload/jigsawImg/" + template + ".png";
                target = "http://kq.neusoft.com/upload/jigsawImg/" + target + ".png";
                WebClient wc = new();

                await Task.Run(async () =>
                {
                   wc.DownloadFile(target, "D:\\test_png\\target.png");
                   wc.DownloadFile(template, "D:\\test_png\\template.png");
                });

            });
        }

        // Allows overriding default certificate validation logic
        private Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            // set IsValid to true/false based on Certificate Errors
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                e.IsValid = true;

            return Task.CompletedTask;
        }

        // Allows overriding default client certificate selection logic during mutual authentication
        private Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            // set e.clientCertificate to override
            return Task.CompletedTask;
        }
    }
}
