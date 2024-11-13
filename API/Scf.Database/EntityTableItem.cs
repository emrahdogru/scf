using Scf.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Database
{
    /// <summary>
    /// Veritabanından çekilen tüm entityler bu nesnede stoklanır. Aynı kayıt veritabanından
    /// tekrar çekilmek istenirse öncesinde burada varlığı kontrol edilir. Amaç, aynı entity'nin
    /// farklı instance'larının oluşmasını engellemek.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class EntityTableItem<T> where T : class, IEntity
    {
        public T Entity { get; set; } = null!;

        /// <summary>
        /// Entity'nin ilk yüklendiğindeki Hash değeri. Eğer SaveChanges() öncesinde entity'de değişiklik olmuşsa
        /// Hash değeri değişecektir. Buradan değişen kayıtları yakalayarak sadece onları kaydedebiliriz.
        /// Eğer yeni kayıt ise oluşturulduğu ve kaydediği andaki hash değerleri aynı olacaktır. Bunun için
        /// yeni kayıtlarda Hash değeri null geçilmelidir.
        /// </summary>
        public byte[]? Hash { get; set; } = null;

        /// <summary>
        /// Kayıt silindi mi?
        /// </summary>
        public bool Deleted { get; set; } = false;
    }
}
