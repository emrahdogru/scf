using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public interface IDoBeforeSave
    {
        /// <summary>
        /// Kaydedilmeden önce yapılacak işlemler, düzenlemeler vb. için metod.
        /// Bu metod her Save işleminden önce çalıştırılır.
        /// </summary>
        void DoBeforeSave();
    }
}
