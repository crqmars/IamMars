using CrqDemo.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrqDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HarmonyDemo.Do();
            QuartzDemo.Init();
            while (true)
            {
                var obj = Console.ReadLine();
                var cmdArr=  obj.Split(':');
                switch (cmdArr[0])
                {
                    case "1":
                        if (cmdArr.Length > 1)
                        {
                            HarmonyDemo.Do(cmdArr[1]);
                        }
                        else {
                            HarmonyDemo.Do();
                        }
                        
                        break;
                    case "2":
                        HarmonyDemo.Patch();
                        break;
                    default:
                        Console.WriteLine($"指令错误");
                        break;
                }

            }
        }
    }
}
