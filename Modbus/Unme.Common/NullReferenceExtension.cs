namespace Modbus.Unme.Common
{
    using System;

    internal static class NullReferenceExtension
    {
        public static void IfNotNull<T>(this T element, Action<T> action)
        {
            if ((object) element == null)
                return;
            action(element);
        }

        public static TResult IfNotNull<T, TResult>(this T element, Func<T, TResult> func)
        {
            if ((object) element == null)
                return default(TResult);

            return func(element);
        }

        public static TResult IfNotNull<T, TResult>(this T element, Func<T, TResult> func, TResult defaultValue)
        {
            if ((object) element == null)
                return defaultValue;

            return func(element);
        }

        public static void Is<TTarget>(this object element, Action<TTarget> action) where TTarget : class
        {
            if (action == null)
                throw new ArgumentNullException("action");

            (element as TTarget).IfNotNull(action);
        }
    }
}