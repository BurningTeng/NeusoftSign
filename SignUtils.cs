using OpenCvSharp;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace SignSystem
{
    internal class SignUtils
    {
        public static void TestOpenCv(string template)
        {
            Mat src = new Mat(template, ImreadModes.Grayscale);//小写的scale，不然报错
            Mat dst = new Mat();
            Mat dst1 = new Mat();

            Cv2.Canny(src, dst, 50, 200);
            Cv2.Add(src, dst, dst1);
            using (new Window("src image", src))
            using (new Window("dst image", dst))
            using (new Window("dst1 image", dst1))
            {
                Cv2.WaitKey();
            }
        }

        public static int Match(string template, string target)
        {
            //模板图片
            Mat temp = new Mat(template, ImreadModes.AnyColor);
            //被匹配图
            Mat wafer = new Mat(target, ImreadModes.AnyColor);

            //Canny边缘检测
            //Canny边缘检测
            Mat temp_canny_Image = new Mat();
            Cv2.Canny(temp, temp_canny_Image, 100, 200);
            Mat wafer_canny_Image = new Mat();
            Cv2.Canny(wafer, wafer_canny_Image, 100, 200);


            //匹配结果
            Mat result = new Mat();
            //模板匹配
            Cv2.MatchTemplate(wafer_canny_Image, temp_canny_Image, result, TemplateMatchModes.CCoeffNormed);//最好匹配为1,值越小匹配越差
            //数组位置下x,y
            Point minLoc = new Point(0, 0);
            Point maxLoc = new Point(0, 0);
            Point matchLoc = new Point(0, 0);
            Cv2.MinMaxLoc(result, out minLoc, out maxLoc);
            matchLoc = maxLoc;

            return matchLoc.X + 15;
        }

        public static void SendMail(string content)
        {
            MailMessage message = new MailMessage();

            //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            MailAddress fromAddr = new MailAddress("2250911301@qq.com");
            message.From = fromAddr;
            //设置收件人,可添加多个,添加方法与下面的一样
            message.To.Add("2250911301@qq.com");
            //设置抄送人
            // message.CC.Add("1592035782@qq.com");
            //设置邮件标题
            message.Subject = "打卡通知";
            //设置邮件内容
            message.Body = content;

/*            string file = CaptureScreen();
            Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(file);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
            // Add the file attachment to this email message.
            message.Attachments.Add(data);*/

            //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看
            SmtpClient client = new SmtpClient("smtp.qq.com", 25);
            //设置发送人的邮箱账号和授权码
            client.Credentials = new NetworkCredential("2250911301@qq.com", "gideydwxiwmudjji");

            //启用ssl,也就是安全发送
            client.EnableSsl = true;
            //发送邮件
            client.Send(message);

            Console.WriteLine("打卡成功" + DateTime.Now);
        }

       /* public static string CaptureScreen()
        {
            var img = CaptureWindow(User32.GetDesktopWindow());
            const string V = @"D:\books\";
            var fileName = V + Guid.NewGuid().ToString() + ".jpg";

            try
            {
                img.Save(fileName);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return fileName;
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        private static Image CaptureWindow(IntPtr handle)
        {
            // Get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY | GDI32.CAPTUREBLT);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }*/



        private class GDI32
        {
            public const int CAPTUREBLT = 1073741824;
            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        public class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
            [DllImport("user32.dll", EntryPoint = "ShowWindow")]
            public static extern bool ShowWindow(IntPtr hwnd, int type);
            [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
            public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
            [DllImport("User32.dll", EntryPoint = "FindWindow")]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        }


        public void Sign()
        {
            IWebDriver wd = new ChromeDriver();
            wd.Navigate().GoToUrl("http://kq.neusoft.com/");
            IWindow window = wd.Manage().Window;
            window.Maximize();
            Thread.Sleep(3000);
            wd.FindElement(By.ClassName("userName")).SendKeys("tengyb");
            wd.FindElement(By.ClassName("password")).SendKeys("18345093167ASdgy123");
            int distance = 0;
            while (true)
            {
                Console.WriteLine("Sign: 当前进程号是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);

                Monitor.Enter(this);
                distance = Match(AppDomain.CurrentDomain.BaseDirectory + "\\template.png", AppDomain.CurrentDomain.BaseDirectory + "\\target.png");
                Monitor.Exit(this);
                Console.WriteLine("匹配检测出的距离是:" + distance);
                //这是滑块
                var slide = wd.FindElement(By.ClassName("ui-slider-btn"));
                Actions action = new (wd);
                //点击并按住滑块元素
                action.ClickAndHold(slide);
                action.MoveByOffset(distance, 0);
                Thread.Sleep(1000);
                string alert;

                try
                {
                    alert = wd.FindElement(By.ClassName("ui-slider-text")).Text;
                    Console.WriteLine(alert);
                }
                catch (Exception e)
                {
                    Console.WriteLine("发生异常:" + e.ToString());
                    alert = "";
                }


                if (alert.Contains("验证成功"))
                {
                    Console.WriteLine("滑块验证成功!");
                    Console.WriteLine("移动的距离是:" + distance);
                    break;
                }
                else
                {
                    Console.WriteLine("滑块验证失败!");
                    
                    action.Release().Perform();
                    Thread.Sleep(1000);
                    wd.SwitchTo().DefaultContent();
                }
                Thread.Sleep(1000);

            }
            wd.FindElement(By.Id("loginButton")).Click();
            Thread.Sleep(3000);

            //Thread.Sleep(3*60*1000);
            string js_sign = "javascript:document.attendanceForm.submit();";
            ((ChromeDriver)wd).ExecuteScript(js_sign, null);
            Thread.Sleep(2000);

            //发送邮件
            SendMail("打卡成功" + DateTime.Now);
            //取消代理，防止第二次调用代理中的saveImage。退出再次调用可能导致死锁。有机会检验一下。
            StopProxyServer();
            //调用js
            string js_exit = "javascript:exitAttendance();";
            ((ChromeDriver)wd).ExecuteScript(js_exit, null);
            Thread.Sleep(3000);
            wd.Quit();
        }


        private ProxyServer proxyServer = new ProxyServer();

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

        public void SetProxyPort()
        {
            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true)
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
                    Console.WriteLine("OnRequest: 当前进程号是:" + Process.GetCurrentProcess() + ", 线程号是:"+  Thread.CurrentThread.ManagedThreadId);
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
                        Console.WriteLine("OnResponse: 当前进程号是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);
                        SaveImage(stringResponse);
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

        private void SaveImage(String stringResponse)
        {

            JObject jo = (JObject)JsonConvert.DeserializeObject(stringResponse);
            template = jo["smallImage"].ToString();
            target = jo["bigImage"].ToString();
            template = "http://kq.neusoft.com/upload/jigsawImg/" + template + ".png";
            target = "http://kq.neusoft.com/upload/jigsawImg/" + target + ".png";

            //System.IO.FileStream fs;
            /*
                            var client = new HttpClient();
                            byte[] urlContents = await client.GetByteArrayAsync(template);
                            fs = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\template.png", System.IO.FileMode.Create);
                            fs.Write(urlContents, 0, urlContents.Length);

                            urlContents = await client.GetByteArrayAsync(target);
                            fs = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\target.pmg", System.IO.FileMode.Create);
                            fs.Write(urlContents, 0, urlContents.Length);

                            fs.Close();*/
            WebClient client = new WebClient();
            //不加锁的话只能下载第一个图片，然后就去匹配去了，由于第二个图片还没有下载下来，导致匹配的时候报错。
            //为什么第二个图片下载不下来需要进一步调查。
            Monitor.Enter(this);
            /*          client.DownloadFile(target, "D:\test_png\target.png");
                      client.DownloadFile(template, "D:\test_png\template.png");*/
            client.DownloadFile(target, AppDomain.CurrentDomain.BaseDirectory + "\\target.png");
            client.DownloadFile(template, AppDomain.CurrentDomain.BaseDirectory + "\\template.png");
            Console.Write("Finish download image ");
            Monitor.Exit(this);
        }

        public static async Task DownPic()
        {
            string imgSourceURL = "https://img.infinitynewtab.com/wallpaper/";
            var client = new HttpClient();
            System.IO.FileStream fs;
            int a = 1;
            //文件名：序号+.jpg。可指定范围，以下是获取100.jpg~500.jpg.
            for (int i = 1; i <= 50; i++)
            {
                var uri = new Uri(Uri.EscapeUriString(imgSourceURL + i.ToString() + ".jpg"));
                byte[] urlContents = await client.GetByteArrayAsync(uri);
                fs = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\" + i.ToString() + ".jpg", System.IO.FileMode.CreateNew);
                fs.Write(urlContents, 0, urlContents.Length);
                Console.WriteLine(a++);
            }
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
