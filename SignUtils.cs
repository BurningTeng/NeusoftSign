using OpenCvSharp;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignSystem
{
    internal class SignUtils
    {
        public static void TestOpenCv()
        {
            Mat src = new Mat("D:\\vs2022\\Project\\cvsharp_check\\lenna.jpg", ImreadModes.Grayscale);//小写的scale，不然报错
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
            Random random = new Random();
            int minoffset = 20;
            int maxoffset = 85;

            int times = 0;
            while (true)
            {

                //这是滑块
                var slide = wd.FindElement(By.ClassName("ui-slider-btn"));
                Actions action = new Actions(wd);
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
            //调用js
            string js_exit = "javascript:exitAttendance();";
            ((ChromeDriver)wd).ExecuteScript(js_exit, null);
            Thread.Sleep(3000);
            wd.Quit();
        }

    }
}
