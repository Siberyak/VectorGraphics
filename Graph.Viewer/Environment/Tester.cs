using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using KG.SE2.Utils.Graph;
using IItem = DataLayer.Environment<int, int>.IItem;
using Dependency = DataLayer.Environment<int, int>.Dependency;

namespace DataLayer
{
    public class Tester
    {
        public static void AllTests()
        {
            Test1();

            Test2();

            Test3();

            Test4();

        }

        private static void Test4()
        {
            var environment = new Environment<double, double>
                (
                (time, offset) => time + offset,
                (left, right) => right - left,
                (o1, o2) => o1 + o2
                , x => -x
                );

            var zero = 0.0;
            var one = 1.0;
            var two = one + one;


            var i = 1;

            var i1 = environment.AddItem(i++);
            var i2 = environment.AddItem(i++);
            var i3 = environment.AddItem(i++);
            var i4 = environment.AddItem(i++);

            var tmp = zero;
            // 1 -(1:2)-> 2
            i2.AddPredecessor(i1, one + tmp, two + tmp);

            // 3 -(3:4)-> 4
            i4.AddPredecessor(i3, two + one, two + two);

            // 2 -( :-1)-> 4
            i4.AddPredecessor(i2, right: -one/2);

            var i1C = i1.Critical;
            var i2C = i2.Critical;
            var i3C = i3.Critical;
            var i4C = i4.Critical;
        }




        public static void Test3()
        {
            var environment = new Environment<DateTime, TimeSpan>
                (
                (time, span) => time.Add(span),
                (left, right) => right - left,
                (o1, o2) => o1 + o2
                , x => -x
                );

            var zero = TimeSpan.Zero;
            var one = TimeSpan.FromDays(1);
            var two = one + one;

            var i = 1;

            var i1 = environment.AddItem(i++);
            var i2 = environment.AddItem(i++);
            var i3 = environment.AddItem(i++);
            var i4 = environment.AddItem(i++);
            var i5 = environment.AddItem(i++);

            // 1 -(1:)-> 2
            i2.AddPredecessor(i1, one);
            // 2 -(1:)-> 3
            i3.AddPredecessor(i2, one);
            // 4 -(:1)-> 3
            i3.AddPredecessor(i4, right: one);
            // 5 -(4:)-> 4
            i4.AddPredecessor(i5, two + two);


            CheckItem(i1, zero, two, i1, i3, false);
            CheckItem(i2, one, one, i1, i3, false);
            CheckItem(i3, two + two + one, zero, i5, i3, true);

            CheckItem(i4, two + two, one, i5, i3, true);
            CheckItem(i5, zero, two + two + one, i5, i3, true);


        }

        public static void Test2()
        {
            var environment = new Environment<DateTime, TimeSpan>
                (
                (time, span) => time.Add(span),
                (left, right) => right - left,
                (o1, o2) => o1 + o2
                , x => -x
                );

            var zero = TimeSpan.Zero;
            var one = TimeSpan.FromDays(1);
            var two = one + one;

            var i = 1;

            var i1 = environment.AddItem(i++);
            var i2 = environment.AddItem(i++);
            var i3 = environment.AddItem(i++);
            var i4 = environment.AddItem(i++);
            var i5 = environment.AddItem(i++);

            //----------------------------------
            CheckItem(i1, zero, zero, i1, i1, true);

            //----------------------------------

            i3.AddPredecessor(i1, one);

            CheckItem(i1, zero, one, i1, i3, true);
            CheckItem(i3, one, zero, i1, i3, true);

            //----------------------------------

            i3.AddPredecessor(i2, two);

            CheckItem(i1, zero, one, i1, i3, false);
            CheckItem(i2, zero, two, i2, i3, true);
            CheckItem(i3, two, zero, i2, i3, true);

            //----------------------------------
            i4.AddPredecessor(i3, one);

            CheckItem(i1, zero, one + one, i1, i4, false);
            CheckItem(i2, zero, two + one, i2, i4, true);
            CheckItem(i3, two, one, i2, i4, true);
            CheckItem(i4, two + one, zero, i2, i4, true);

            //----------------------------------
            i5.AddPredecessor(i3, two);

            CheckItem(i1, zero, one + two, i1, i5, false);
            CheckItem(i2, zero, two + two, i2, i5, true);
            CheckItem(i3, two, two, i2, i5, true);
            CheckItem(i4, two + one, zero, i2, i4, false);
            CheckItem(i5, two + two, zero, i2, i5, true);

        }

        private static void CheckItem(Environment<DateTime, TimeSpan>.Item<int> item, TimeSpan toLeft, TimeSpan toRight, Environment<DateTime, TimeSpan>.Item<int> leftItem, Environment<DateTime, TimeSpan>.Item<int> rightItem, bool critical)
        {
            Debug.Assert(item.ToLeft == toLeft);
            Debug.Assert(item.ToRight == toRight);
            Debug.Assert(item.Left == leftItem);
            Debug.Assert(item.Right == rightItem);
            Debug.Assert(item.Critical == critical);
        }

        static void Test1()
        {
            var environment = new Environment<int, int>
                (
                (t, d) => t + d,
                (l, r) => r - l,
                (o1, o2) => o1 + o2
                , x => -x
                );

            var i1 = environment.AddItem("1");
            var i2 = environment.AddItem("2");
            var i3 = environment.AddItem("3");

            var i4 = environment.AddItem("4");
            var i5 = environment.AddItem("5");

            // chain #1: 1 -> 2 -> 4 -> 5

            var d24 = i4.AddPredecessor(i2, 5);
            environment.Check(d24, 5, 5);

            var d12 = i2.AddPredecessor(i1, 1, 2);
            environment.Check(d12, 1, 1);
            environment.Check(d24, 6, 5);

            var d45 = i5.AddPredecessor(i4, 4, 6);
            environment.Check(d12, 1, 1);
            environment.Check(d24, 6, 5);
            environment.Check(d45, 10, 4);



            var p12 = environment.Pathes(i1, i2);
            p12.CheckPathes(new[] { i1, i2 });

            var p14 = environment.Pathes(i1, i4);
            p14.CheckPathes(new [] { i1, i2, i4 });

            var p15 = environment.Pathes(i1, i5);
            p15.CheckPathes(new [] { i1, i2, i4, i5 });

            var p13 = environment.Pathes(i1, i3);
            p13.CheckPathes<IItem, Dependency>();

            // chain #2: 2 -> 3 -> 4

            var d23 = i3.AddPredecessor(i2, 3, 4);
            var d34 = i4.AddPredecessor(i3, 3, 4);

            p13 = environment.Pathes(i1, i3);
            p13.CheckPathes(new [] { i1, i2, i3 });

            p14 = environment.Pathes(i1, i4);
            p14.CheckPathes
                (
                new [] { i1, i2, i4 },
                new [] { i1, i2, i3, i4 }
                );

            var i6 = environment.AddItem("6");
            var i7 = environment.AddItem("7");

            // chain #3: 4 -> 6 -> 7
            i6.AddPredecessor(i4, 5);
            i7.AddPredecessor(i6, 10);

            // chain #4: 8 -> 2
            var i8 = environment.AddItem("8");
            i2.AddPredecessor(i8, 5);


            // chain #5: 9 -> 3
            var i9 = environment.AddItem("9");
            i3.AddPredecessor(i9, 0, right: 10);


            // chain #5: 2 -> 0 -> 4
            var i0 = environment.AddItem("0");
            i0.AddPredecessor(i2, 2, 4);
            i4.AddPredecessor(i0, 3, 10);


            // full graph 
            //          ╔═════╗
            //          ║     ║
            //          ║  9  ║
            //          ║     ║ 
            //          ╚══╤══╝
            // ╔═════╗     │     ╔═════╗           ╔═════╗
            // ║     ║     └────►║ ┌─┐ ║           ║     ║
            // ║  1  ║           ║ │3│ ║     ┌────►║  5  ║
            // ║     ║     ┌────►║ └─┘ ║     │     ║     ║
            // ╚══╤══╝     │     ╚══╤══╝     │     ╚═════╝
            //    │     ╔══╧══╗     │     ╔══╧══╗
            //    └────►║ ┌─┐ ║     └────►║ ┌─┐ ║
            //          ║ │2│ ║           ║ │4│ ║
            //    ┌────►║ └─┘ ║     ┌────►║ └─┘ ║
            //    │     ╚══╤══╝     │     ╚══╤══╝  
            // ╔══╧══╗     │     ╔══╧══╗     │     ╔═════╗     ╔═════╗
            // ║ ┌─┐ ║     │     ║     ║     │     ║ ┌─┐ ║     ║ ┌─┐ ║
            // ║ │8│ ║     └────►║  0  ║     └────►║ │6│ ║────►║ │7│ ║
            // ║ └─┘ ║           ║     ║           ║ └─┘ ║     ║ └─┘ ║
            // ╚═════╝           ╚═════╝           ╚═════╝     ╚═════╝ 
            //

            // critical path
            // 8 -> 2 -> 3 -> 4 -> 6 -> 7

            var cps = environment.CriticalPathes();
        }

    }
}
