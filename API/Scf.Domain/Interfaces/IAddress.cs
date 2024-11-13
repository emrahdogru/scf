using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Interfaces
{
    public interface IAddress
    {
        /// <summary>
        /// Mahalle, meydan, bulvar, cadde, sokak ve küme evlere karşılık gelecek şekilde, standart sayısal eşdeğer olarak TÜİK tarafından verilmiş olan “sabit tanımlama numarası”
        /// </summary>
        string? ID { get; set; }

        /// <summary>
        /// Posta kutusu
        /// </summary>
        string? Postbox { get; set; }

        /// <summary>
        /// İç kapı numarası
        /// </summary>
        string? Room { get; set; }

        /// <summary>
        /// Meydan/bulvar/cadde/sokak/küme evler/site adı bilgileri
        /// </summary>
        string? StreetName { get; set; }

        /// <summary>
        /// Blok adı
        /// </summary>
        string? BlockName { get; set; }

        /// <summary>
        /// Bina adı
        /// </summary>
        string? BuildingName { get; set; }

        /// <summary>
        /// Bina veya bloğa ait dış kapı numarası
        /// </summary>
        string? BuildingNumber { get; set; }

        /// <summary>
        /// İlçe/Semt adı
        /// </summary>
        string? CitySubdivisionName { get; set; }

        /// <summary>
        /// İl adı
        /// </summary>
        string? CityName { get; set;}

        /// <summary>
        /// Posta kod numarasu
        /// </summary>
        string? PostalZone { get; set; }

        /// <summary>
        /// Kasaba/köy/mezra/mevkii
        /// </summary>
        string? Region { get; set; }

        /// <summary>
        /// Mahalle adı
        /// </summary>
        string? District { get; set; }
    }
}
