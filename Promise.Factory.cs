using System;
using System.Linq;
using System.Collections.Generic;

namespace P.I.G
{
    public static partial class Promise
    {
        public static Promise<TResult, TReason> Resolve<TResult, TReason>(TResult result)
        {
            return new Promise<TResult, TReason>((resolve, _) => resolve(result));
        }

        public static Promise<TResult, TReason> Reject<TResult, TReason>(TReason reason)
        {
            return new Promise<TResult, TReason>((_, reject) => reject(reason));
        }

        public static Promise<IEnumerable<TResult>, TReason> All<TResult, TReason>(IEnumerable<IThenable<TResult, TReason>> thenables)
        {
            return new Promise<IEnumerable<TResult>, TReason>((resolve, reject)
                => {
                    var total = thenables.Count();
                    var count = 0;
                    var results = new List<TResult>();

                    foreach (var thenable in thenables)
                    {
                        thenable.Then(
                            (TResult result)
                                => {
                                    results.Add(result);

                                    if (++count == total) resolve(results);

                                    return result;
                                },
                            (TReason reason) => { reject(reason); return reason; });
                    }
                });
        }

        public static Promise<IEnumerable<TResult>, TReason> All<TResult, TReason>(params IThenable<TResult, TReason>[] thenables)
        {
            return All(thenables.AsEnumerable());
        }

        public static Promise<TResult, TReason> Race<TResult, TReason>(IEnumerable<IThenable<TResult, TReason>> thenables)
        {
            return new Promise<TResult, TReason>((resolve, reject)
                => {
                    foreach (var thenable in thenables)
                        thenable.Then(
                            (TResult result) => { resolve(result); return result; },
                            (TReason reason) => { reject(reason); return reason; });
                });
        }

        public static Promise<TResult, TReason> Race<TResult, TReason>(params IThenable<TResult, TReason>[] thenables)
        {
            return Race(thenables.AsEnumerable());
        }
    }
}
