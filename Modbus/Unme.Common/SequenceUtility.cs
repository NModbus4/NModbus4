namespace Modbus.Unme.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class SequenceUtility
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, IEnumerable<T> second,
            params IEnumerable<T>[] additionalItems)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");
            if (additionalItems == null)
                throw new ArgumentNullException("additionalItems");

            var enumerable = first as T[] ?? first.ToArray();
            var result = Enumerable.Concat(enumerable, second);

            return additionalItems.Aggregate(result, Enumerable.Concat);
        }

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

        public static IEnumerable<T> ToSequence<T>(this T element)
        {
            if ((object) element == null)
                throw new ArgumentNullException("element");

            return new[] {element};
        }

        public static IEnumerable<T> ToSequence<T>(T element, params T[] additional)
        {
            var sequence = element.ToSequence().ToArray();

            if (additional != null && additional.Length > 0)
                return Enumerable.Concat(sequence, additional);

            return sequence;
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