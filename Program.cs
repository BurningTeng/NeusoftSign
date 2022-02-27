using SignSystem;
using System.Timers;


/*Console.Title = "SignSystem";
IntPtr ParenthWnd = new IntPtr(0);
ParenthWnd =SignUtils.User32.FindWindow(null, "SignSystem");
SignUtils.User32.ShowWindow(ParenthWnd, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化*/


//需要考虑法定节假日和周末
Console.WriteLine("今年是：" + DateTime.Now.Year + "年");
List<string> list = new List<string>();

//sk.Factory.StartNew(() => SignUtils.TestOpenCv("C:/Users/burning/Pictures/20201205234844361.png"));
Random random = new();
int min = 0;
int max = 120;

/*SignProxy signProxy = new();
Task task = new(() => { 
    signProxy.StartProxyServer();
    signProxy.SetProxyPort();
    Console.ReadKey();
    Console.WriteLine("close proxy");
    signProxy.StopProxyServer();
});
task.Start();*/

//定时器相关代码
System.Timers.Timer timer = new System.Timers.Timer();
timer.Enabled = true;
timer.Interval = 60000;//执行间隔时间,单位为毫秒;此时时间间隔为1分钟  
timer.Start();
timer.Elapsed += new System.Timers.ElapsedEventHandler(SignFilter);

Console.ReadKey();


void SignFilter(object? source, ElapsedEventArgs e)
{
    if (DateTime.Now.Month == 4)
    {
        if (DateTime.Now.Day >=  2 && DateTime.Now.Day <= 5)
        {
            Console.WriteLine("清明假期，正常打卡");
            Sign();
        }
        //周日
        else if (DateTime.Now.Day == 24)
        {
            Console.WriteLine("劳动节调休，周日正常上班");
            Sign();
        }
        //五一假期
        else if (DateTime.Now.Day == 30 )
        {
            Console.WriteLine("劳动节假期，正常打卡");
            Sign();
        }
        else
        {
            SignNormal();
        }
    }
    //劳动节
    else if (DateTime.Now.Month == 5)
    {
        Console.WriteLine(DateTime.Now);
        if (DateTime.Now.Day >= 1 && DateTime.Now.Day <= 4) {
            Sign();
        }
        else if (DateTime.Now.Day == 7)
        {
            Sign();
        } 
        else
        {
            SignNormal();
        }
    }
    //端午节
    else if (DateTime.Now.Month == 6)
    {
        if (DateTime.Now.Day >= 3 && DateTime.Now.Day <= 5)
        {
            Sign();
        } else
        {
            SignNormal();
        }

    }

    //中秋节
    else if (DateTime.Now.Month == 9)
    {
        if (DateTime.Now.Day >= 10 && DateTime.Now.Day <= 12)
        {
            Sign();
        }
        else
        {
            SignNormal();
        }

    }

    //国庆节
    else if (DateTime.Now.Month == 10)
    {
        if (DateTime.Now.Day >= 2 && DateTime.Now.Day <= 9)
        {
            Sign();
        }
        else
        {
            SignNormal();
        }
    } else
    {
        SignNormal();
    }
}

void SignNormal()
{
    if (DateTime.Now.DayOfWeek.ToString().Equals("Saturday") || (DateTime.Now.DayOfWeek.ToString().Equals("Sunday")))
    {
        Console.WriteLine("今天是" + DateTime.Now.DayOfWeek + "时间是:" + DateTime.Now);
    } else
    {
        Sign();
    }
}

void Sign()
{

    if (DateTime.Now.Hour == 7 && DateTime.Now.Minute == 00)  //如果当前时间是20点15分
    {
        Thread.Sleep(random.Next(min, max) * 60 * 1000);
        SignUtils.Sign();
        Console.WriteLine("早上打卡结束");
    }
    else if (DateTime.Now.Hour == 19 && DateTime.Now.Minute == 48)
    {
        Thread.Sleep(random.Next(min, max) * 60 * 1000);
        SignUtils.Sign();
        Console.WriteLine("晚上打卡结束");
    }
    else
    {
        Console.WriteLine(DateTime.Now);
    }

}