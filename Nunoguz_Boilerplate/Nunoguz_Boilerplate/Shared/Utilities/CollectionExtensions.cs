﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace Nunoguz_Boilerplate.Shared.Utilities
{
    public static class CollectionExtensions
    {
        // Good practice for conditional filtering search
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);

            return source;
        }
    }
}
