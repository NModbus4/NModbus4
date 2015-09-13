namespace Modbus.Unme.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    internal static class SequenceUtility
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int startIndex, int size)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var enumerable = source as T[] ?? source.ToArray();
            int num = enumerable.Count();
            if (startIndex < 0 || num < startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if (size < 0 || startIndex + size > num)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            return enumerable.Skip(startIndex).Take(size);
        }
    }
}
