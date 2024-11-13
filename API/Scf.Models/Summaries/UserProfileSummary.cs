using Scf.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Summaries
{
    public class UserProfileSummary : UserSummary
    {
        public UserProfileSummary(User user)
            : base(user)
        {
            this.Language = user.Language;
        }

        public Languages Language { get; }
    }
}
