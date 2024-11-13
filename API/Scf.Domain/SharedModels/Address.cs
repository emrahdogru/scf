using MongoDB.Bson.Serialization.Attributes;
using Scf.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.SharedModels
{
    public class Address : IAddress
    {
        private Country? _country = null;

        public string? ID { get; set; }
        public string? Postbox { get; set; }
        public string? Room { get; set; }
        public string? StreetName { get; set; }
        public string? BlockName { get; set; }
        public string? BuildingName { get; set; }
        public string? BuildingNumber { get; set; }
        public string? CitySubdivisionName { get; set; }
        public string? CityName { get; set; }
        public string? PostalZone { get; set; }
        public string? Region { get; set; }
        public string? District { get; set; }

        [BsonElement]
        protected string? CountryCode { get; set; }

        [BsonIgnore]
        public Country? Country
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.CountryCode))
                    return null;

                if (_country == null || _country.Code != this.CountryCode)
                    _country = Country.FromCode(this.CountryCode);

                return _country;
            }
        }
    }
}
