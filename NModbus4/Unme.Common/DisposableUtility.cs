namespace Modbus.Unme.Common
{
    using System;

    /// <summary>
    ///
    /// </summary>
    internal static class DisposableUtility
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public static void Dispose<T>(ref T item) where T : class, IDisposable
        {
            if (item == null)
            {
                return;
            }

            item.Dispose();
            item = default(T);
        }
    }
}
