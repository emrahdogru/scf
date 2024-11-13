using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public interface IUser
    {
        ObjectId Id { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }

        bool IsActive { get; set; }

        /// <summary>
        /// Kullanıcı hesabının doğrulanması için anahtar oluşturur
        /// </summary>
        /// <returns></returns>
        string GenerateValidationKey();

        /// <summary>
        /// Parametre olarak verilen parola, kullanıcının geçerli parolası mı?
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        bool IsValidPassword(string password);

        /// <summary>
        /// Kullanıcı parolasını değiştirir. Öncesinde parola politikasına uygunluğunu kontrol eder.
        /// </summary>
        /// <param name="newPassword"></param>
        void SetPassword(string newPassword);
    }
}
