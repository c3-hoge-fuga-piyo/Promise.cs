using System;

namespace P.I.G
{
    using E = Exception;

    public static partial class PromiseExtensions
    {
        public static Promise<T> Then<T>(
            this Promise<T> promise,
            Action<T> onFulfilled,
            Action<E> onRejected)
        {
            if (promise == null) throw new ArgumentNullException("promise");
            if (onFulfilled == null) throw new ArgumentNullException("onFulfilled");
            if (onRejected == null) throw new ArgumentNullException("onRejected");

            return promise.Then<T>(
                result => { onFulfilled(result); return result; },
                reason => { onRejected(reason); return Promise.Reject<T>(reason); });
        }

        public static Promise<T> Then<T>(
            this Promise<T> promise,
            Action<T> onFulfilled)
        {
            if (promise == null) throw new ArgumentNullException("promise");
            if (onFulfilled == null) throw new ArgumentNullException("onFulfilled");

            return promise.Then<T>(
                result => { onFulfilled(result); return result; });
        }

        public static Promise<T> Catch<T>(
            this Promise<T> promise,
            Action<E> onRejected)
        {
            if (promise == null) throw new ArgumentNullException("promise");
            if (onRejected == null) throw new ArgumentNullException("onRejected");

            return promise.Then<T>(
                _ => _,
                reason => { onRejected(reason); return Promise.Reject<T>(reason); });
        }

        public static Promise<T> Catch<T>(
            this Promise<T> promise,
            Func<E, Promise<T>> onRejected)
        {
            if (promise == null) throw new ArgumentNullException("promise");
            if (onRejected == null) throw new ArgumentNullException("onRejected");

            return promise.Then<T>(
                _ => _,
                onRejected);
        }

        public static Promise<T> Catch<T>(
            this Promise<T> promise,
            Func<E, T> onRejected)
        {
            if (promise == null) throw new ArgumentNullException("promise");
            if (onRejected == null) throw new ArgumentNullException("onRejected");

            return promise.Then<T>(
                _ => _,
                onRejected);
        }
    }
}
