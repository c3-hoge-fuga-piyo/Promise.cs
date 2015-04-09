using System;

namespace P.I.G
{
    using E = Exception;

    public interface IThenable<T>
    {
        IThenable<U> Then<U>(
            Func<T, IThenable<U>> onFulfilled,
            Func<E, IThenable<U>> onRejected);

        IThenable<U> Then<U>(
            Func<T, IThenable<U>> onFulfilled,
            Func<E, U> onRejected = null);

        IThenable<U> Then<U>(
            Func<T, U> onFulfilled,
            Func<E, IThenable<U>> onRejected);

        IThenable<U> Then<U>(
            Func<T, U> onFulfilled,
            Func<E, U> onRejected = null);
    }
}
