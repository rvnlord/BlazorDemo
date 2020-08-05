﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemo.Common.Extensions.Collections
{
    public static class DbSetExtensions
    {
        public static Expression<Func<T, bool>> ConvertToWhereClause<T>(this Expression<Func<T, object>> exp, T o) where T : class, new()
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            var memberExp = (MemberExpression)exp.Body;
            var objPropExp = Expression.PropertyOrField(Expression.Constant(o), memberExp.Member.Name);
            var equalExp = Expression.Equal(exp.Body, objPropExp);
            var exp2 = Expression.Lambda<Func<T, bool>>(equalExp, exp.Parameters);
            return exp2;
        }

        public static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class
        {
            return (DbContext) dbSet?
                .GetType().GetTypeInfo()?
                .GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(dbSet);
        }

        public static void AddOrUpdate<T>(this DbSet<T> dbSet, T data, params Expression<Func<T, object>>[] wheres) where T : class, new()
        {
            AddOrUpdate(dbSet, new List<T> { data }, wheres);
        }

        public static void AddOrUpdate<T>(this DbSet<T> dbSet, List<T> data, params Expression<Func<T, object>>[] wheres) where T : class, new()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var context = dbSet.GetContext();
            foreach (var item in data)
            {
                var query = context.Set<T>().AsNoTracking();
                T entity;
                if (wheres.Any())
                {
                    query = wheres.Aggregate(query, (current, @where) => current.Where(@where.ConvertToWhereClause(item)));
                    entity = query.FirstOrDefault();
                }
                else
                    entity = query.FirstOrDefault(e => e.Equals(item));

                if (entity == null)
                    dbSet.Add(item);
                else
                {
                    var ids = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name);
                    var keyFields = typeof(T).GetProperties().Where(x => ids.Contains(x.Name)).ToList();
                    foreach (var k in keyFields)
                        k.SetValue(item, k.GetValue(entity));
                    dbSet.Update(item);
                }
            }
        }

        public static void RemoveBy<TSource>(this DbSet<TSource> dbSet, Func<TSource, bool> selector) where TSource : class
        {
            var set = dbSet.ToArray();
            foreach (var entity in set)
                if (selector(entity))
                    dbSet.Remove(entity);
        }
    }
}
