using System;

namespace P.I.G
{
    public static partial class ThenableExtensions
    {
        public static IThenable<TResult, TReason> AsThenable<TResult, TReason>(this IThenable<TResult, TReason> thenable)
        {
            if (thenable == null)
                throw new ArgumentNullException("thenable");

            return thenable;
        }

        #region Then
        public static IThenable<UResult, TReason> Then<UResult, TResult, TReason>(
                this IThenable<TResult, TReason> thenable,
                Func<TResult, IThenable<UResult, TReason>> onFulfilled)
        {
            if (thenable == null)
                throw new ArgumentNullException("thenable");
            if (onFulfilled == null)
                throw new ArgumentNullException("onFulfilled");

            return thenable.Then(
                onFulfilled,
                (TReason _) => _);
        }

        public static IThenable<UResult, TReason> Then<UResult, TResult, TReason>(
                this IThenable<TResult, TReason> thenable,
                Func<TResult, UResult> onFulfilled)
        {
            if (thenable == null)
                throw new ArgumentNullException("thenable");
            if (onFulfilled == null)
                throw new ArgumentNullException("onFulfilled");

            return thenable.Then(
                onFulfilled,
                (TReason _) => _);
        }

        public static IThenable<TResult, TReason> Then<TResult, TReason>(
                this IThenable<TResult, TReason> thenable,
                Action<TResult> onFulfilled)
        {
            if (thenable == null)
                throw new ArgumentNullException("thenable");
            if (onFulfilled == null)
                throw new ArgumentNullException("onFulfilled");

            return thenable.Then(
                result => { onFulfilled(result); return result; });
        }

        public static IThenable<TResult, TReason> Then<TResult, TReason>(
            this IThenable<TResult, TReason> thenable,
            Action<TResult> onFulfilled,
            Action<TReason> onRejected)
        {
            if (thenable == null)
                throw new ArgumentNullException("thenable");
            if (onFulfilled == null)
                throw new ArgumentNullException("onFulfilled");
            if (onRejected == null)
                throw new ArgumentNullException("onRejected");

            return thenable.Then(
                (TResult result) => { onFulfilled(result); return result; },
                (TReason reason) => { onRejected(reason); return reason; });
        }
        #endregion

        #region Catch
        public static IThenable<TResult, UReason> Catch<UReason, TResult, TReason>(
                this IThenable<TResult, TReason> thenable,
                Func<TReason, IThenable<TResult, UReason>> onRejected)
        {
            if (thenable == null)
                throw new ArgumentNullException("thenable");
            if (onRejected == null)
                throw new ArgumentNullException("onRejected");

            return thenable.Then(
                (TResult _) => _,
                onRejected);
        }

        public static IThenable<TResult, UReason> Catch<UReason, TResult, TReason>(
                this IThenable<TResult, TReason> thenable,
                Func<TReason, UReason> onRejected)
        {
            if (thenable == null)
                throw new ArgumentNullException("thenable");
            if (onRejected == null)
                throw new ArgumentNullException("onRejected");

            return thenable.Then(
                (TResult _) => _,
                onRejected);
        }

        public static IThenable<TResult, TReason> Catch<TResult, TReason>(
                this IThenable<TResult, TReason> thenable,
                Action<TReason> onRejected)
        {
            if (thenable == null)
                throw new ArgumentNullException("thenable");
            if (onRejected == null)
                throw new ArgumentNullException("onRejected");

            return thenable.Catch(
                reason => { onRejected(reason); return reason; });
        }
        #endregion
    }
}
