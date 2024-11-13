using Scf.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Database
{
    internal class QueryProviedWrapper<T> : IQueryProvider
    {
        private readonly IQueryProvider _queryProvider;
        private readonly IBaseEntitySet _entitySet;

        public QueryProviedWrapper(IQueryProvider queryProvider, IBaseEntitySet entitySet) 
        {
            _queryProvider = queryProvider;
            _entitySet = entitySet;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new QueryWrapper<T>(_queryProvider.CreateQuery<T>(expression), (IBaseEntitySet<T>)_entitySet);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new QueryWrapper<TElement>(_queryProvider.CreateQuery<TElement>(expression), (IBaseEntitySet<TElement>)_entitySet);
        }

        public object? Execute(Expression expression)
        {
            var result = _queryProvider?.Execute(expression);
            if (result is IEntity entity)
            {
                entity.Context = _entitySet.Context;
            }
            return result;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var result = _queryProvider.Execute<TResult>(expression);
            if(result is IEntity entity)
            {
                entity.Context = _entitySet.Context;
            }
            return result;
        }
    }
}
