using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Results
{
    public class FilterResult<T> where T : class
    {
        public FilterResult(IFilter filter)
        {
            this.Filter = filter;
        }

        public T[]? Result { get; set; }
        public int TotalRecordCount { get; set; }

        public IFilter Filter { get; set; }
    }
}
