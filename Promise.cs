using System;

namespace P.I.G
{
    public enum PromiseState
    {
        Pending,
        Fulfilled,
        Rejected,
    }

    public class Promise<TResult, TReason> : IThenable<TResult, TReason>
    {
        #region Constructors
        public Promise(Action<Action<TResult>, Action<TReason>> executor)
        {
            this.State = PromiseState.Pending;
            executor(this.Resolve, this.Reject);
        }
        #endregion

        public PromiseState State { get; private set; }

        #region Resolve
        TResult result;
        Action<TResult> onFulfilled;

        void Resolve(TResult result)
        {
            if (this.State == PromiseState.Rejected) return;

            if (this.State == PromiseState.Pending)
            {
                this.State = PromiseState.Fulfilled;
                this.result = result;
            }

            this.InvokeResolve();
        }

        void OnFulfilled(Action<TResult> onFulfilled)
        {
            if (this.State == PromiseState.Rejected) return;

            if (this.onFulfilled == null) this.onFulfilled = onFulfilled;

            if (this.State == PromiseState.Fulfilled) this.InvokeResolve();
        }

        void InvokeResolve()
        {
            if (this.onFulfilled != null) this.onFulfilled(this.result);
        }
        #endregion

        #region Reject
        TReason reason;
        Action<TReason> onRejected;

        void Reject(TReason reason)
        {
            if (this.State == PromiseState.Fulfilled) return;

            if (this.State == PromiseState.Pending)
            {
                this.State = PromiseState.Rejected;
                this.reason = reason;
            }

            this.InvokeReject();
        }

        void OnRejected(Action<TReason> onRejected)
        {
            if (this.State == PromiseState.Fulfilled) return;

            if (this.onRejected == null) this.onRejected = onRejected;

            if (this.State == PromiseState.Rejected) this.InvokeReject();
        }

        void InvokeReject()
        {
            if (this.onRejected != null) this.onRejected(this.reason);
        }
        #endregion

        #region Then
        public Promise<UResult, UResason> Then<UResult, UResason>(
                Func<TResult, IThenable<UResult, UResason>> onFulfilled,
                Func<TReason, IThenable<UResult, UResason>> onRejected)
        {
            return new Promise<UResult, UResason>((resolve, reject)
                => {
                    this.OnFulfilled(result
                        => onFulfilled(result)
                            .Then(
                                (UResult x) => { resolve(x); return x; },
                                (UResason x) => { reject(x); return x; }));

                    this.OnRejected(reason
                        => onRejected(reason)
                            .Then(
                                (UResult x) => { resolve(x); return x; },
                                (UResason x) => { reject(x); return x; }));
                });
        }

        public Promise<UResult, UResason> Then<UResult, UResason>(
                Func<TResult, IThenable<UResult, UResason>> onFulfilled,
                Func<TReason, UResason> onRejected)
        {
            return new Promise<UResult, UResason>((resolve, reject)
                => {
                    this.OnFulfilled(result
                        => onFulfilled(result)
                            .Then(
                                (UResult x) => { resolve(x); return x; },
                                (UResason x) => { reject(x); return x; }));

                    this.OnRejected(reason
                        => reject(onRejected(reason)));
                });
        }

        public Promise<UResult, UResason> Then<UResult, UResason>(
                Func<TResult, UResult> onFulfilled,
                Func<TReason, IThenable<UResult, UResason>> onRejected)
        {
            return new Promise<UResult, UResason>((resolve, reject)
                => {
                    this.OnFulfilled(result
                        => resolve(onFulfilled(result)));

                    this.OnRejected(reason
                        => onRejected(reason)
                            .Then(
                                (UResult x) => { resolve(x); return x; },
                                (UResason x) => { reject(x); return x; }));
                });
        }

        public Promise<UResult, UResason> Then<UResult, UResason>(
                Func<TResult, UResult> onFulfilled,
                Func<TReason, UResason> onRejected)
        {
            return new Promise<UResult, UResason>((resolve, reject)
                => {
                    this.OnFulfilled(result
                        => resolve(onFulfilled(result)));

                    this.OnRejected(reason
                        => reject(onRejected(reason)));
                });
        }
        #endregion

        #region IThenable<TResult, TReason>
        IThenable<UResult, UResason> IThenable<TResult, TReason>.Then<UResult, UResason>(
                Func<TResult, IThenable<UResult, UResason>> onFulfilled,
                Func<TReason, IThenable<UResult, UResason>> onRejected)
        {
            return this.Then(onFulfilled, onRejected);
        }

        IThenable<UResult, UResason> IThenable<TResult, TReason>.Then<UResult, UResason>(
                Func<TResult, IThenable<UResult, UResason>> onFulfilled,
                Func<TReason, UResason> onRejected)
        {
            return this.Then(onFulfilled, onRejected);
        }

        IThenable<UResult, UResason> IThenable<TResult, TReason>.Then<UResult, UResason>(
                Func<TResult, UResult> onFulfilled,
                Func<TReason, IThenable<UResult, UResason>> onRejected)
        {
            return this.Then(onFulfilled, onRejected);
        }

        IThenable<UResult, UResason> IThenable<TResult, TReason>.Then<UResult, UResason>(
                Func<TResult, UResult> onFulfilled,
                Func<TReason, UResason> onRejected)
        {
            return this.Then(onFulfilled, onRejected);
        }
        #endregion
    }
}
