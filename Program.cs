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

//Debug模式， 文件位置 D:\vs2022\Project\SignSystem\bin\Debug\net6.0\PersonInfo.json
var listPerson = JsonMapper.ToObject<List<PersonInfo>>(File.ReadAllText("PersonInfo.json"));

/*su.StartProxyServer();
su.SetProxyPort(8888);

var task = Task.Run(() =>
{
    su.exit = false;
    su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
});
task.Wait();
Console.WriteLine("打卡完成");
//取消代理。连续3次启动的话就失败。第三次代理服务器起不来。
su.StopProxyServer();

await Task.Delay(5000);

su.StartProxyServer();
su.SetProxyPort(8888);

task = Task.Run(() =>
{
    su.exit = false;
    su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
});
task.Wait();
Console.WriteLine("打卡完成");
//取消代理。连续3次启动的话就失败。第二次代理服务器起不来。
su.StopProxyServer();*/

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
        su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
        Console.WriteLine("早上打卡结束");
    }
    else if (DateTime.Now.Hour == 16 && DateTime.Now.Minute == 30)
    {
        Console.WriteLine("晚上打卡: 当前进程是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);

       // Thread.Sleep(random.Next(min, max) * 60 * 1000);
        su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
        su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
        su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
        su.Sign(listPerson[0].name, listPerson[0].email, listPerson[0].password);
        Console.WriteLine("晚上打卡结束");
    }
    else
    {
        Console.WriteLine("else: 当前进程是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);
        Console.WriteLine("当前时间是" + DateTime.Now);
    }
}