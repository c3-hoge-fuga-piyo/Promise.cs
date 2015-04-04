using System;

namespace P.I.G
{
    public interface IThenable<TResult, TReason>
    {
        IThenable<UResult, UResason> Then<UResult, UResason>(
                Func<TResult, IThenable<UResult, UResason>> onFulfilled,
                Func<TReason, IThenable<UResult, UResason>> onRejected);

        IThenable<UResult, UResason> Then<UResult, UResason>(
                Func<TResult, IThenable<UResult, UResason>> onFulfilled,
                Func<TReason, UResason> onRejected);

        IThenable<UResult, UResason> Then<UResult, UResason>(
                Func<TResult, UResult> onFulfilled,
                Func<TReason, IThenable<UResult, UResason>> onRejected);

        IThenable<UResult, UResason> Then<UResult, UResason>(
                Func<TResult, UResult> onFulfilled,
                Func<TReason, UResason> onRejected);
    }
}
