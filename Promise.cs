using System;

namespace P.I.G
{
    using E = Exception;

    public class Promise<T> : IThenable<T>
    {
        #region Constructors
        Promise()
        {
            this.State = PromiseStatus.Pending;
        }

        public Promise(Action<Action<T>, Action<E>> executor)
            : this()
        {
            if (executor == null) throw new ArgumentNullException("executor");

            try
            {
                executor(this.Resolve, this.Reject);
            }
            catch (Exception reason)
            {
                this.Reject(reason);
            }
        }

        public Promise(IThenable<T> thenable)
            : this()
        {
            if (thenable == null) throw new ArgumentNullException("thenable");

            thenable.Then(
                result => { this.Resolve(result); return default(T); },
                reason => { this.Reject(reason); return default(T); });
        }
        #endregion

        #region Promise States
        public PromiseStatus State { get; private set; }
        #endregion

        #region Then
        public Promise<U> Then<U>(
            Func<T, Promise<U>> onFulfilled,
            Func<E, Promise<U>> onRejected)
        {
            if (onFulfilled == null) throw new ArgumentNullException("onFulfilled");
            if (onRejected == null) throw new ArgumentNullException("onRejected");

            var next = new Promise<U>();

            this.Handle(
                result => onFulfilled(result).Handle(next.Resolve, next.Reject),
                reason => onRejected(reason).Handle(next.Resolve, next.Reject));

            return next;
        }

        public Promise<U> Then<U>(
            Func<T, Promise<U>> onFulfilled,
            Func<E, U> onRejected = null)
        {
            if (onFulfilled == null) throw new ArgumentNullException("onFulfilled");

            var next = new Promise<U>();

            this.Handle(
                result => onFulfilled(result).Handle(next.Resolve, next.Reject),
                reason
                    => {
                        if (onRejected != null)
                            next.Resolve(onRejected(reason));
                        else
                            next.Reject(reason);
                    });

            return next;
        }

        public Promise<U> Then<U>(
            Func<T, U> onFulfilled,
            Func<E, Promise<U>> onRejected)
        {
            if (onFulfilled == null) throw new ArgumentNullException("onFulfilled");
            if (onRejected == null) throw new ArgumentNullException("onRejected");

            var next = new Promise<U>();

            this.Handle(
                result => next.Resolve(onFulfilled(result)),
                reason => onRejected(reason).Handle(next.Resolve, next.Reject));

            return next;
        }

        public Promise<U> Then<U>(
            Func<T, U> onFulfilled,
            Func<E, U> onRejected = null)
        {
            if (onFulfilled == null) throw new ArgumentNullException("onFulfilled");

            var next = new Promise<U>();

            this.Handle(
                result => next.Resolve(onFulfilled(result)),
                reason
                    => {
                        if (onRejected != null)
                            next.Resolve(onRejected(reason));
                        else
                            next.Reject(reason);
                    });

            return next;
        }
        #endregion

        #region Promise Resolution Procedure
        T result;
        E reason;

        Action<T> deferredFulfilledHandler;
        Action<E> deferredRejectedHandler;

        void Handle(Action<T> onFulfilled = null, Action<E> onRejected = null)
        {
            var hasFulfilledHandler = onFulfilled != null;
            var hasRejectedHandler = onRejected != null;

            switch (this.State)
            {
                case PromiseStatus.Pending:
                {
                    if (hasFulfilledHandler) this.deferredFulfilledHandler += onFulfilled;
                    if (hasRejectedHandler) this.deferredRejectedHandler += onRejected;
                    break;
                }
                case PromiseStatus.Fulfilled:
                {
                    if (hasFulfilledHandler)
                    {
                        try
                        {
                            onFulfilled(this.result);
                        }
                        catch (Exception reason)
                        {
                            this.Reject(reason);
                        }
                    }
                    break;
                }
                case PromiseStatus.Rejected:
                {
                    if (hasRejectedHandler) onRejected(this.reason);
                    break;
                }
            }
        }

        void InvokeDeferredHandler()
        {
            this.Handle(
                this.deferredFulfilledHandler,
                this.deferredRejectedHandler);

            this.deferredFulfilledHandler = null;
            this.deferredRejectedHandler = null;
        }

        void Resolve(T result)
        {
            if (this.State == PromiseStatus.Pending)
            {
                this.State = PromiseStatus.Fulfilled;
                this.result = result;
            }
            this.InvokeDeferredHandler();
        }

        void Reject(Exception reason)
        {
            if (this.State == PromiseStatus.Pending)
            {
                this.State = PromiseStatus.Rejected;
                this.reason = reason;;
            }
            this.InvokeDeferredHandler();
        }
        #endregion

        #region IThenable<T>
        IThenable<U> IThenable<T>.Then<U>(
            Func<T, IThenable<U>> onFulfilled,
            Func<E, IThenable<U>> onRejected)
        {
            return this.Then(
                result => new Promise<U>(onFulfilled(result)),
                reason => new Promise<U>(onRejected(reason)));
        }

        IThenable<U> IThenable<T>.Then<U>(
            Func<T, IThenable<U>> onFulfilled,
            Func<E, U> onRejected)
        {
            return this.Then(
                result => new Promise<U>(onFulfilled(result)),
                onRejected);
        }

        IThenable<U> IThenable<T>.Then<U>(
            Func<T, U> onFulfilled,
            Func<E, IThenable<U>> onRejected)
        {
            return this.Then(
                onFulfilled,
                reason => new Promise<U>(onRejected(reason)));
        }

        IThenable<U> IThenable<T>.Then<U>(
            Func<T, U> onFulfilled,
            Func<E, U> onRejected)
        {
            return this.Then(
                onFulfilled,
                onRejected);
        }
        #endregion
    }
}
