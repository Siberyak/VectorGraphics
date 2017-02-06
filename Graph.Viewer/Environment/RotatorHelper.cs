using System;
using System.Collections.Generic;

namespace DataLayer
{
    /// <summary>
    /// �����������.
    /// </summary>
    internal static class RotatorHelper
    {
        /// <summary>
        /// ���������.
        /// </summary>
        /// <typeparam name="T">��� ��������� ������������ ������.</typeparam>
        /// <param name="items">����������� �����.</param>
        /// <param name="predicate">�������� ����������� ��������� ��������.</param>
        /// <param name="action">��������, ����������� ��� ����������.</param>
        /// <param name="comparer">�����������</param>
        /// <returns>���������� ������ ���������, ���� �� ��, ������������� ��������, ���� ��������.</returns>
        public static IEnumerable<T> Process<T>(IEnumerable<T> items, Func<T, bool> predicate, Action<T> action, IComparer<T> comparer = null)
        {
            var queue = new Queue<T>(items);

            return Process(queue, predicate, action, comparer);

        }

        private static IEnumerable<T> Process<T>(Queue<T> queue, Func<T, bool> predicate, Action<T> action, IComparer<T> comparer)
        {
            while (queue.Count > 0)
            {
                List<T> items = new List<T>();

                T[] queueitems = queue.ToArray();

                queue.Clear();

                foreach (T item in queueitems)
                {
                    if (predicate(item))
                        items.Add(item);
                    else
                        queue.Enqueue(item);
                }

                if (items.Count > 0)
                {
                    if (comparer != null)
                        items.Sort(comparer);

                    foreach (T itemForAction in items)
                    {
                        action(itemForAction);
                    }
                }

                if (items.Count == 0)
                    break;
            }

            return queue;
        }

        private static IEnumerable<T> ProcessOld<T>(Queue<T> queue, Func<T, bool> predicate, Action<T> action)
        {
            var current = default(T);

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                if (Process(item, predicate, action))
                {
                    current = default(T);
                }
                else
                {
                    queue.Enqueue(item);

                    if (Equals(item, current))
                    {
                        return queue.ToArray();
                    }

                    if (Equals(current, default(T)))
                    {
                        current = item;
                    }
                }
            }

            return new T[0];
        }

        private static bool Process<T>(T item, Func<T, bool> predicate, Action<T> action)
        {
            var result = predicate(item);
            if (result)
                action(item);

            return result;
        }
    }
}