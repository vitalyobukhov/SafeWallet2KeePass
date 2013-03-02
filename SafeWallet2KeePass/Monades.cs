using System;

namespace SafeWallet2KeePass
{
    static class MonadeExtensions
    {
        public static TResult With<TInput, TResult>(this TInput obj, Func<TInput, TResult> evaluator)
            where TResult : class
            where TInput : class
        {
            return obj == null ? null : evaluator(obj);
        }

        public static TResult Return<TInput, TResult>(this TInput obj,
            Func<TInput, TResult> evaluator, TResult defaultValue = default(TResult))
            where TInput : class
        {
            return obj == null ? defaultValue : evaluator(obj);
        }

        public static TInput If<TInput>(this TInput obj, Func<TInput, bool> evaluator)
            where TInput : class
        {
            return obj == null ? null : (evaluator(obj) ? obj : null);
        }

        public static TInput Unless<TInput>(this TInput obj, Func<TInput, bool> evaluator)
            where TInput : class
        {
            return obj == null ? null : (evaluator(obj) ? null : obj);
        }

        public static void Do<TInput>(this TInput obj, Action<TInput> action)
            where TInput : class
        {
            if (obj != null)
            {
                action(obj);
            }
        }

        public static void Do<TInput>(this TInput obj, Action action)
            where TInput : class
        {
            if (obj != null)
            {
                action();
            }
        }

        public static TInput Apply<TInput>(this TInput obj, Action<TInput> action)
            where TInput : class
        {
            if (obj == null)
            {
                return null;
            }

            action(obj);
            return obj;
        }
    }
}