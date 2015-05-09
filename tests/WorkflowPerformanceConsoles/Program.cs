using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkflowPerformanceConsoles
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new ParallelController();
            t.RunWorkflow();

            //var st = new System.Diagnostics.StackTrace();
            //var str = st.ToString();
            //Console.WriteLine(str);

            Console.WriteLine("===END===");
            Console.ReadLine();
        }
    }
}
