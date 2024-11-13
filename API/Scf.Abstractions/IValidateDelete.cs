using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public interface IValidateDelete
    {
        /// <summary>
        /// Kayıt silinebilir mi? Silinemez ise CanNotDeleteException fırlatır.
        /// </summary>
        void ValidateDelete();
    }
}
