using Scf.LanguageResources;
using FluentValidation;
using FluentValidation.Validators;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public class MultilanguageText
    {
        public MultilanguageText() { }

        public MultilanguageText(string tr, string en) {
            this.tr = tr;
            this.en = en;
        }

        [BsonIgnoreIfNull]
        public string? tr { get; set; } = null;

        [BsonIgnoreIfNull]
        public string? en { get; set; } = null;

        /// <summary>
        /// UI tarafında MultiLanguageText objelerini yakalayabilmemiz için var.
        /// </summary>
        [BsonIgnore]
        [JsonProperty(PropertyName = "isML")]
        public bool IsMuiltilanguageText { get => true; }

        public override string ToString()
        {
            return string.Join(" ", this.tr, this.en);
        }

        public string? ToString(Languages language)
        {
            return language switch
            {
                Languages.English => this.en,
                _ => this.tr,
            };
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is MultilanguageText oml && oml.ToString() == this.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
