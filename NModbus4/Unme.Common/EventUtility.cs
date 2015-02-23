namespace Modbus.Unme.Common
{
    using System;

    internal static class EventUtility
    {
        public static void Raise(this EventHandler handler, object sender)
        {
            if (handler == null)
                return;

            handler(sender, EventArgs.Empty);
        }

        public static void Raise<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            if (handler == null)
                return;

            handler(sender, e);
        }
    }
}
