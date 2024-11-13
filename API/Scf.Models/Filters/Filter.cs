using Scf.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Scf.Models.Filters
{
    public class Filter<T> : IFilter<T> where T : class
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public IEnumerable<FilterSort?>? Sorts { get; set; }

        public string? Predicate { get; set; }

        public virtual IQueryable<T> Apply(IQueryable<T> query, out int totalRecordCount)
        {
            if(!string.IsNullOrWhiteSpace(this.Predicate))
                query = query.Where(new ParsingConfig() { CustomTypeProvider = new Utility.ObjectIdTypeProvider() }, this.Predicate);

            totalRecordCount = query.Count();

            if (this.Sorts != null && this.Sorts.Any())
            {
                string orderByQuery = string.Join(", ", this.Sorts.Where(x => !string.IsNullOrWhiteSpace(x?.Field)).Select(x => x?.Field + (x?.Desc == true ? " DESC" : "")));

                if(!string.IsNullOrWhiteSpace(orderByQuery))
                    query = query.OrderBy(orderByQuery);
            }
                

            if (this.PageSize.HasValue && this.PageSize.Value > 0)
            {
                var page = Math.Max(this.Page.GetValueOrDefault(1), 1) - 1;
                query = query.Skip(page * PageSize.Value).Take(PageSize.Value);
            }

            return query;
        }
    }

    public class FilterSearch<T> : Filter<T> where T: class, ISearchableEntity
    {
        public string? Search { get; set; }

        public override IQueryable<T> Apply(IQueryable<T> query, out int totalRecordCount)
        {
            if (!string.IsNullOrWhiteSpace(this.Search))
                query = query.Search(this.Search);

            return base.Apply(query, out totalRecordCount);
        }
    }
}
