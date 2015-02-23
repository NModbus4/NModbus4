namespace Modbus.Unme.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class SequenceUtility
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");
            foreach (T obj in source)
                action(obj);
        }

        public static void ForEachWithIndex<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");
            ForEach(WithIndex(source), x => action(x.Item2, x.Item1));
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int startIndex, int size)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var enumerable = source as T[] ?? source.ToArray();
            int num = enumerable.Count();
            if (startIndex < 0 || num < startIndex)
                throw new ArgumentOutOfRangeException("startIndex");
            if (size < 0 || startIndex + size > num)
                throw new ArgumentOutOfRangeException("size");

            return enumerable.Skip(startIndex).Take(size);
        }

        public static IEnumerable<Tuple<int, T>> WithIndex<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Select((item, index) => Tuple.Create(index, item));
        }
    }
}
