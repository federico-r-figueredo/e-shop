using System;
using System.Collections.Generic;
using System.Linq;

namespace eShop.Services.Catalog.API.Extensions {
    internal static class LINQSelectExtensions {
        internal static IEnumerable<SelectTryResult<TSource, TResult>> SelectTry<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> selector) {
            foreach (TSource element in enumerable) {
                SelectTryResult<TSource, TResult> returnedValue;
                try {
                    returnedValue = new SelectTryResult<TSource, TResult>(element, selector(element), null);
                } catch (Exception exception) {
                    returnedValue = new SelectTryResult<TSource, TResult>(element, default(TResult), exception);
                }
                yield return returnedValue;
            }
        }

        internal static IEnumerable<TResult> OnCaughtException<TSource, TResult>(this IEnumerable<SelectTryResult<TSource, TResult>> enumerable, Func<Exception, TResult> exceptionHandler) {
            return enumerable.Select(x => x.CaughtException == null ? x.Result : exceptionHandler(x.CaughtException));
        }

        internal static IEnumerable<TResult> OnCaughtException<TSource, TResult>(this IEnumerable<SelectTryResult<TSource, TResult>> enumerable, Func<TSource, Exception, TResult> exceptionHandler) {
            return enumerable.Select(x => x.CaughtException == null ? x.Result : exceptionHandler(x.Source, x.CaughtException));
        }
    }

    internal class SelectTryResult<TSource, TResult> {
        internal SelectTryResult(TSource source, TResult result, Exception exception) {
            this.Source = source;
            this.Result = result;
            this.CaughtException = exception;
        }

        internal TSource Source { get; private set; }
        internal TResult Result { get; private set; }
        internal Exception CaughtException { get; private set; }
    }
}