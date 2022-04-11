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

namespace SignSystem
{
    internal class SignUtils
    {
        public static void Sign()
        {
            IWebDriver wd = new OpenQA.Selenium.Edge.EdgeDriver();
            wd.Quit();
        }
    }
}
