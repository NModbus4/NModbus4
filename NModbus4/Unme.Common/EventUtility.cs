namespace Modbus.Unme.Common
{
    using System;

    /// <summary>
    ///
    /// </summary>
    internal static class EventUtility
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="sender"></param>
        public static void Raise(this EventHandler handler,
                                 object sender)
        {
            if (handler == null)
            {
                return;
            }

            handler(sender, EventArgs.Empty);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Raise<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            if (handler == null)
            {
                return;
            }

            handler(sender, e);
        }
    }
}
