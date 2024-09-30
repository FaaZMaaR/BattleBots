using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Threading;

class Program
{
    public static void Main()
    {
        int time = 15;
        GameForm myform = new GameForm("BattleBots");
        GameScene scene = new GameScene(time, myform.picbox);
        myform.picbox.Paint += (obj, ea) => {
            scene.Render(ea.Graphics);
        };
        Thread PIthread = new Thread(() => {
            while (true)
            {
                scene.ProcessInput(myform);
                Thread.Sleep(time);
            }
        });
        Thread Uthread = new Thread(() => {
            while (true)
            {
                scene.Update();
                Thread.Sleep(time);
            }
        });
        Thread Rthread = new Thread(() => {
            while (true)
            {
                myform.picbox.Invalidate();
                Thread.Sleep(time);
            }
        });        
        PIthread.Start();
        Uthread.Start();
        Rthread.Start();
        Application.Run(myform);
    }
}