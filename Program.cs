using System;
using System.Collections.Generic;

namespace LLRB
{
    class Program
    {
        static void Main(string[] args)
        {
            Tree tree = new Tree();
#if !Wrong

            tree.Insert(10);
            tree.Insert(20);
            tree.Insert(30);
            tree.Insert(40);
            //tree.Insert(50);

            tree.Insert(15);
            tree.Insert(60);

            for (int i = 59; i >= 50; i--)
            {
                tree.Insert(i);
            }

            for (int i = 11; i <= 14; i++)
            {
                tree.Insert(i);
            }
#endif

#if TestSucceeded
            tree.Insert(3);
            tree.Insert(2);
            tree.Print();
            tree.Insert(1);
            tree.Print();
#endif
        }
    }
}
