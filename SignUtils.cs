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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

            string file = CaptureScreen();
            Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(file);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
            // Add the file attachment to this email message.
            message.Attachments.Add(data);

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

        public static string CaptureScreen()
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
        }



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


        public static void Sign()
        {
            Console.WriteLine("Hello World!");

            IWebDriver wd = new ChromeDriver();
            wd.Navigate().GoToUrl("http://kq.neusoft.com/");
            IWindow window = wd.Manage().Window;
            window.Maximize();
            Thread.Sleep(3000);
            wd.FindElement(By.ClassName("userName")).SendKeys("tengyb");
            wd.FindElement(By.ClassName("password")).SendKeys("18345093167ASdgy123");
            int distance = 85;
            Random random = new ();
            int minoffset = 20;
            int maxoffset = 85;

            while (true)
            {

                //这是滑块
                var slide = wd.FindElement(By.ClassName("ui-slider-btn"));
                Actions action = new (wd);
                //点击并按住滑块元素
                action.ClickAndHold(slide);
                action.MoveByOffset(distance*2, 0);
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
                    Console.WriteLine("移动的距离是:" + distance * 2);
                    break;
                }
                else
                {
                    Console.WriteLine("滑块验证失败!");
                    action.Release().Perform();
                    distance = random.Next(minoffset, maxoffset);
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
            //调用js
            string js_exit = "javascript:exitAttendance();";
            ((ChromeDriver)wd).ExecuteScript(js_exit, null);
            Thread.Sleep(3000);
            wd.Quit();
        }

    }
}
