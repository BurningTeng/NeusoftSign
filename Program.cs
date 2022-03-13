﻿using SignSystem;
using System.Diagnostics;
using System.Timers;

//需要考虑法定节假日和周末
Console.WriteLine("今年是：" + DateTime.Now.Year);
Console.WriteLine("当前进程号是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);

Random random = new();
int min = 0;
int max = 120;
SignUtils su = new();

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
        su.StartProxyServer();
        su.SetProxyPort();
        var task = Task.Run(() =>
        {
            su.Sign();
        });
        task.Wait();
        Console.WriteLine("早上打卡结束");
    }
    else if (DateTime.Now.Hour == 19 && DateTime.Now.Minute == 00)
    {
        Console.WriteLine("晚上打卡: 当前进程号是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);

        Thread.Sleep(random.Next(min, max) * 60 * 1000);
        su.StartProxyServer();
        su.SetProxyPort();
        var task = Task.Run(() =>
        {
            su.Sign();
        });
        task.Wait();
        Console.WriteLine("晚上打卡结束");
    }
    else
    {
        Console.WriteLine("else: 当前进程号是:" + Process.GetCurrentProcess() + ", 线程号是:" + Thread.CurrentThread.ManagedThreadId);
        Console.WriteLine("当前时间是" + DateTime.Now);
    }

}