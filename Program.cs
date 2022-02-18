using SignSystem;
using System.Timers;

Console.WriteLine(DateTime.Now);
Random random = new();
int min = 0;
int max = 0 ;


System.Timers.Timer timer = new System.Timers.Timer();
timer.Enabled = true;
timer.Interval = 60000;//执行间隔时间,单位为毫秒;此时时间间隔为1分钟  
timer.Start();
timer.Elapsed += new System.Timers.ElapsedEventHandler(Sign);

Console.ReadKey();


void Sign(object? source, ElapsedEventArgs e)
{
    
    if (DateTime.Now.Hour == 19 && DateTime.Now.Minute == 00)  //如果当前时间是10点30分
    {
        EveSign();
    }
    else if (DateTime.Now.Hour == 7 && DateTime.Now.Minute == 00)
    {
        MorningSign();
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
    Thread.Sleep(new Random().Next(min, max) * 60 * 1000);
    SignUtils.Sign();
    Console.WriteLine("晚上打卡结束");
}