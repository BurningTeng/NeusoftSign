using SignSystem;
using System.Timers;


/*Console.Title = "SignSystem";
IntPtr ParenthWnd = new IntPtr(0);
ParenthWnd =SignUtils.User32.FindWindow(null, "SignSystem");
SignUtils.User32.ShowWindow(ParenthWnd, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化*/


//需要考虑法定节假日和周末
Console.WriteLine("今年是：" + DateTime.Now.Year);
List<string> list = new List<string>();

//sk.Factory.StartNew(() => SignUtils.TestOpenCv("C:/Users/burning/Pictures/20201205234844361.png"));
Random random = new();
int min = 0;
int max = 120;
SignUtils su = new();


//signProxy.StopProxyServer();


//等待任务完成
/*task.GetAwaiter().OnCompleted(() =>
{
    Console.WriteLine("打卡完成");
   // signProxy.StopProxyServer();
});*/
//定时器相关代码
System.Timers.Timer timer = new System.Timers.Timer();
timer.Enabled = true;
timer.Interval = 60000;//执行间隔时间,单位为毫秒;此时时间间隔为1分钟  
timer.Start();
timer.Elapsed += new System.Timers.ElapsedEventHandler(SignFilter);
Console.ReadKey();

void SignFilter(object? source, ElapsedEventArgs e)
{
    if (DateTime.Now.Hour == 7 && DateTime.Now.Minute == 00)  //如果当前时间是20点15分
    {
        Thread.Sleep(random.Next(min, max) * 60 * 1000);
        su.StartProxyServer();
        su.SetProxyPort();
        var task = Task.Run(() =>
        {
            su.Sign();
        });
        task.Wait();
        su.StopProxyServer();
        Console.WriteLine("早上打卡结束");
    }
    else if (DateTime.Now.Hour == 19 && DateTime.Now.Minute == 00)
    {
        Thread.Sleep(random.Next(min, max) * 60 * 1000);
        su.StartProxyServer();
        su.SetProxyPort();
        var task = Task.Run(() =>
        {
            su.Sign();
        });
        task.Wait();
        su.StopProxyServer();
        Console.WriteLine("晚上打卡结束");
    }
    else
    {
        Console.WriteLine("当前时间是" + DateTime.Now);
    }

}