using Scf.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Database
{
    //internal class QueryWrapper : IQueryable, IOrderedQueryable
    //{
    //    private IQueryable query;
    //    private IBaseEntitySet entitySet;

    //    internal QueryWrapper(IQueryable innerQuery, IBaseEntitySet entitySet)
    //    {
    //        this.query = innerQuery;
    //        this.entitySet = entitySet;
    //    }

    //    public Type ElementType => query.ElementType;

    //    public Expression Expression => query.Expression;

    //    public IQueryProvider Provider => new QueryProviedWrapper(query.Provider, entitySet);

    //    public IEnumerator GetEnumerator()
    //    {
    //        throw new NotImplementedException("Buraya girmemesi lazım. Giriyor ise EnumeratorWrapper`ın generic olmayan halini implemente etmek gerekiyor");
    //        return query.GetEnumerator();
    //    }
    //}

    internal class QueryWrapper<T> : IQueryable, IOrderedQueryable, IQueryable<T>, IOrderedQueryable<T>
    {
        private readonly IQueryable<T> query;
        private readonly IBaseEntitySet<T> entitySet;

        internal QueryWrapper(IQueryable<T> innerQuery, IBaseEntitySet<T> entitySet) 
        {
            this.query = innerQuery;
            this.entitySet = entitySet;
        }

        public Type ElementType => query.ElementType;

        public Expression Expression => query.Expression;

        public IQueryProvider Provider => new QueryProviedWrapper<T>(query.Provider, entitySet);

        public IEnumerator GetEnumerator()
        {
            return new EnumeratorWrapper<T>(query.GetEnumerator(), this.entitySet);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new EnumeratorWrapper<T>(query.GetEnumerator(), this.entitySet);
        }
    }
}
