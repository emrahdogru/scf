using Scf;
using Scf.Domain;
using Scf.LanguageResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidation
{
    public class DomainAbstractValidator<T> : AbstractValidator<T>
    {
        IEnumerable<Languages>? availableLanguages = null;

        public DomainAbstractValidator(DomainContext domainContext)
        {
            this.DomainContext = domainContext;
        }

        public DomainContext DomainContext { get; }

        public IEnumerable<Languages> AvailableLanguages
        {
            get
            {
                if (availableLanguages == null)
                {
                    var l = DomainContext.LanguageService;

                    if (!l.TenantId.HasValue)
                        throw new TenantRequiredException();

                    availableLanguages = DomainContext.Tenants.FindAsync(l.TenantId.Value).Result?.Settings.AvailableLanguages;

                    if (availableLanguages == null)
                        throw new TenantMismatchException("Tenant için erişilebilir diller tanımsız.");
                }

                return availableLanguages;
            }
        }
    }
}
