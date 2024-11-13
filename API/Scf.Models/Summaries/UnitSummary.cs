using Scf.Domain.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Summaries
{
    public class UnitSummary
    {
        public UnitSummary(Unit unit)
        {
            this.Code = unit.Code;
            this.Name = unit.Name;
        }

        public string Code { get; }
        public string Name { get; }
    }
}
