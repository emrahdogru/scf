using Scf.Domain;
using Scf.Domain.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public class EmployeeTitleForm : BaseTenantEntityForm<EmployeeTitle>
    {
        public EmployeeTitleForm():base() { }

        public EmployeeTitleForm(EmployeeTitle employeeTitle)
            :base(employeeTitle)
        {
            this.Names = employeeTitle.Names;
        }

        public MultilanguageText Names { get; set; } = new MultilanguageText();

        public override async Task Bind(DomainContext context, User user, EmployeeTitle entity)
        {
            await base.Bind(context, user, entity);
            entity.Names = this.Names;
        }
    }
}
