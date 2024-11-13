using Scf.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models
{
    public interface IFilter
    {
        int? Page { get; set; }
        int? PageSize { get; set; }
        string? Predicate { get; set; }
        IEnumerable<FilterSort?>? Sorts { get; set; }
    }

    public class FilterSort
    {
        public FilterSort() { }
        public FilterSort(string? field, bool descending = false)
        {
            Field = field;
            Desc = descending;
        }

        public string? Field { get; set; }
        public bool Desc { get; set; }
    }

    public interface IFilter<T> : IFilter where T : class
    {
        IQueryable<T> Apply(IQueryable<T> query, out int totalRecordCount);
    }
}
