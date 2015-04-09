using System;
using System.Linq;
using System.Collections.Generic;

namespace P.I.G
{
    public static partial class Promise
    {
        #region Resolve
        public static Promise<T> Resolve<T>(T result)
        {
            return new Promise<T>((resolve, _) => resolve(result));
        }

        public static Promise<T> Resolve<T>(Promise<T> promise)
        {
            if (promise == null) throw new ArgumentNullException("promises");

            return promise;
        }

        public static Promise<T> Resolve<T>(IThenable<T> thenable)
        {
            if (thenable == null) throw new ArgumentNullException("thenable");

            return new Promise<T>(thenable);
        }
        #endregion

        #region Reject
        public static Promise<T> Reject<T>(Exception reason)
        {
            if (reason == null) throw new ArgumentNullException("reason");

            return new Promise<T>((_, reject) => reject(reason));
        }
        #endregion

        #region All
        public static Promise<IEnumerable<T>> All<T>(IEnumerable<Promise<T>> promises)
        {
            if (promises == null) throw new ArgumentNullException("promises");

            return new Promise<IEnumerable<T>>((resolve, reject)
                => {
                    var total = promises.Count();
                    var current = 0;
                    var done = 0;
                    var results = new T[total];

                    foreach (var p in promises)
                    {
                        var i = current++;

                        p.Then<T>(
                            result => {
                                results[i] = result;

                                if (++done == total) resolve(results);

                                return default(T);
                            },
                            reason => { reject(reason); return default(T); });
                    }
                });
        }

        public static Promise<IEnumerable<T>> All<T>(params Promise<T>[] promises)
        {
            return All(promises.AsEnumerable());
        }
        #endregion

        #region Race
        public static Promise<T> Race<T>(IEnumerable<Promise<T>> promises)
        {
            if (promises == null) throw new ArgumentNullException("promises");

            return new Promise<T>((resolve, reject)
                => {
                    foreach (var p in promises)
                        p.Then<T>(
                            result => { resolve(result); return default(T); },
                            reason => { reject(reason); return default(T); });
                });
        }

        public static Promise<T> Race<T>(params Promise<T>[] promises)
        {
            return Race(promises.AsEnumerable());
        }
        #endregion
    }
}
