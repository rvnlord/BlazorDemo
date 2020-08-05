using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace BlazorDemo.Common.Extensions.Collections
{
    public static class IEnumerableExtensions
    {
        public static string FirstMessage(this IEnumerable<IdentityError> identityErrors)
        {
            var errors = identityErrors?.ToArray();
            if (identityErrors == null || !errors.Any() || errors.First().Code.IsNullOrWhiteSpace() || errors.First().Description.IsNullOrWhiteSpace())
                return string.Empty;

            return $"[{errors.First().Code}] {errors.First().Description}";
        }

        public static IEnumerable<TSource> SkipLastWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var buffer = new List<TSource>();

            foreach (var item in source)
            {
                if (predicate(item))
                    buffer.Add(item);
                else
                {
                    if (buffer.Count > 0)
                    {
                        foreach (var bufferedItem in buffer)
                            yield return bufferedItem;

                        buffer.Clear();
                    }

                    yield return item;
                }
            }
        }
    }
}
