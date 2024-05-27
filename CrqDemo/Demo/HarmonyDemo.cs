using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrqDemo.Demo
{
    /// <summary>
    /// 参考文章
    /// https://www.cnblogs.com/mq0036/p/17141332.html
    /// </summary>
    public class HarmonyDemo
    {
        public static void Do(string name = "mars")
        {
            Print.PrintText(name,out var kkk);
            Console.WriteLine(Print.GetNameText(name));
            Person p = new Person("chen");
            Console.WriteLine(p.GetName(name));
        }
        public static void Patch()
        {
            #region 静态方法
            var original = typeof(Print).GetMethod("PrintText");
            var prefix = typeof(HarmonyDemo).GetMethod("PrePrintText");

            //prefix:执行真正方法前触发
            Patcher.GetHarmony().Patch(original, prefix: prefix);


            var original2 = typeof(Print).GetMethod("GetNameText");
            var prefix2 = typeof(HarmonyDemo).GetMethod("PreGetNameText");
            var postfix2 = typeof(HarmonyDemo).GetMethod("PostGetNameText");
            Patcher.GetHarmony().Patch(original2, prefix: prefix2, postfix: postfix2);
            #endregion

            #region 实例方法

            var original3 = typeof(Person).GetMethod("GetName");
            var postfix3 = typeof(HarmonyDemo).GetMethod("PreGetName");
            Patcher.GetHarmony().Patch(original3, prefix: postfix3);
            #endregion
        }

        #region 修改静态方法

        /// <summary>
        /// 执行方法前触发
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool PrePrintText(ref string name,out string outStr)
        {
            outStr = "new out";
            Console.WriteLine($"执行PrintText方法前。name={name}");
            if (name == "error")
            {
                Console.WriteLine($"拦截方法执行");
                return false;//返回false不会走到真正的方法
            }

            if (name == "mars")
            {
                //可对参数进行篡改
                name = "fateMars";
            }
            return true;
        }

     

        /// <summary>
        /// 执行方法前
        /// </summary>
        /// <param name="name"></param>
        /// <param name="__result">这个是返回值</param>
        /// <returns></returns>
        public static bool PreGetNameText(ref string name,ref string __result)
        {
            Console.WriteLine($"执行GetNameText方法前。name={name}");
            if (name == "error")
            {
                __result = "出现错误，进行拦截";
                return false;//返回false不会走到真正的方法
            }

            if (name == "mars")
            {
                //可对参数进行篡改
                name = "名称替换为fatemars";
            }
            return true;
        }
        /// <summary>
        /// 执行方法后触发
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void PostGetNameText(ref string name, ref string __result)
        {
            Console.WriteLine($"执行GetNameText方法后。name={name},{__result}");
            if (name == "last")
            {
                __result = $"执行完修改返回值：{__result}";
            }
        }

        #endregion

        #region 修改实例方法

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="__instance">实例对象（参数名固定）</param>
        /// <param name="lastName">参数</param>
        /// <param name="__result">返回值（参数名固定） 值类型记得加ref</param>
        /// <returns></returns>
        public static bool PreGetName(object __instance,ref string __result, ref string lastName)
        {
             var person = (Person)__instance;
            if (lastName == "chen")
            {
                __result = $"不能叫【chen】，进行拦截。{person.FirstName}";
                return false;
            }

            if (lastName.Contains("fuck"))
            {
                lastName = lastName.Replace("fuck", "****");
            }
            return true;
        }

        #endregion
    }



    public class Print
    {
        public static void PrintText(string name,out string outStr)
        {
            outStr = "out";
            Console.WriteLine($"Print Name:{name}");
        }

        public static string GetNameText(string name)
        {
            return $"my name is {name}";
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public Person(string firstName)
        {
            this.FirstName = firstName;
        }


        public string GetName(string lastName)
        {
            return $"{FirstName} {lastName}";
        }
    }

    public static class Patcher
    {
        static Harmony _harmony = null;

        public static Harmony GetHarmony()
        {
            if (_harmony == null)
                _harmony = new Harmony("jpzmg");
            return _harmony;
        }
    }
}
