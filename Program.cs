using SignSystem;
using System.Timers;


Console.Title = "SignSystem";
IntPtr ParenthWnd = new IntPtr(0);
ParenthWnd =SignUtils.User32.FindWindow(null, "SignSystem");
SignUtils.User32.ShowWindow(ParenthWnd, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化


//需要考虑法定节假日和周末
Console.WriteLine(DateTime.Now);
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
timer.Elapsed += new System.Timers.ElapsedEventHandler(Sign);

Console.ReadKey();


void Sign(object? source, ElapsedEventArgs e)
{
    
    if (DateTime.Now.Hour == 7 && DateTime.Now.Minute == 00)  //如果当前时间是20点15分
    {
        MorningSign();
    }
    else if (DateTime.Now.Hour == 19  && DateTime.Now.Minute == 48)
    {
        EveSign();
    }
    else
    {
        Console.WriteLine(DateTime.Now.ToString());

    }
}

void MorningSign()
{
    Thread.Sleep(random.Next(min, max) * 60 * 1000);
    SignUtils.Sign();
    Console.WriteLine("早上打卡结束");
}

void EveSign()
{
    Thread.Sleep(random.Next(min, max) * 60 * 1000);
    SignUtils.Sign();
    Console.WriteLine("晚上打卡结束");
}