using System;
using System.IO;

namespace FSMGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader ifs = File.OpenText(args[0]))
            {
                using (StreamWriter ofs = File.CreateText(args[1]))
                {
                    FiniteStateMachine.StateMachine.Compile(ifs).GenerateCode(ofs);
                }
            }
        }
    }
}
