using System;
using System.Collections.Generic;

namespace LLRB
{
    class Program
    {
        static void Main(string[] args)
        {
            Tree tree = new Tree();
#if InsertSucceeded

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


#if TestRemove
            for (int i = 4; i <= 80; i += 4)
            {
                tree.Insert(i);
            }
            tree.Insert(42);
            tree.Insert(58);
            tree.Insert(34);
            tree.PrintAction("InitTree", 0);

            tree.Remove(76);
            tree.Remove(44);
            tree.Remove(36);
            tree.Remove(52);
#endif

#if !TestRemove
            //for (int i = 4; i <= 20; i += 4)
            //{
            //    tree.Insert(i);
            //}
            tree.Insert(30);
            tree.Insert(20);
            tree.Insert(10);
            tree.Insert(25);
            tree.PrintAction("InitTree", 3);
            tree.ValidateChildrensParent();

            tree.Remove(10);


            tree.Insert(5);
            tree.Insert(10);
            tree.Insert(15);
            tree.PrintAction("InitTree", 5);
            tree.ValidateChildrensParent();

            tree.Remove(5);

#endif
        }
    }
}
