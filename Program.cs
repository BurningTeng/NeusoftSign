using SignSystem;
using System.Diagnostics;
using System.Timers;
using ThirdParty.Json.LitJson;


/*Console.Title = "SignSystem";
IntPtr ParenthWnd = new IntPtr(0);
ParenthWnd =SignUtils.User32.FindWindow(null, "SignSystem");
SignUtils.User32.ShowWindow(ParenthWnd, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化*/


//需要考虑法定节假日和周末
Console.WriteLine("今年是：" + DateTime.Now.Year);
Console.WriteLine("当前进程:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);

//sk.Factory.StartNew(() => SignUtils.TestOpenCv("C:/Users/burning/Pictures/20201205234844361.png"));
Random random = new();
int min = 0;
int max = 120;
SignUtils su = new();

var listPerson = JsonMapper.ToObject<List<PersonInfo>>(File.ReadAllText("PersonInfo.json"));


/*var task = Task.Run(() =>
{
    su.StartProxyServer();
    su.SetProxyPort();
    su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
});*/
/*.ContinueWith((task1) =>
{
    su.StartProxyServer();
    su.SetProxyPort();
    su.Sign();
});
task.Wait();*/

Console.ReadKey();

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
    Console.WriteLine("SignFilter: 当前进程号是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);

    if (DateTime.Now.Hour == 7 && DateTime.Now.Minute == 00)  //如果当前时间是20点15分
    {
        Thread.Sleep(random.Next(min, max) * 60 * 1000);
        var task = Task.Run(() =>
        {
            su.StartProxyServer();
            su.SetProxyPort();
            su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
        });
        task.Wait();
        Console.WriteLine("早上打卡结束");
    }
    else if (DateTime.Now.Hour == 19 && DateTime.Now.Minute == 00)
    {
        Console.WriteLine("晚上打卡: 当前进程是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);

        Thread.Sleep(random.Next(min, max) * 60 * 1000);
        var task = Task.Run(() =>
        {
            su.StartProxyServer();
            su.SetProxyPort();
            su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
        });
        task.Wait();
        Console.WriteLine("晚上打卡结束");
    }
    else
    {
        Console.WriteLine("else: 当前进程是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);
        Console.WriteLine("当前时间是" + DateTime.Now);
    }

}