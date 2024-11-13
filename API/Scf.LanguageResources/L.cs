using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.LanguageResources
{
    public class L
    {
        public L(string tr, string en)
        {
            this.tr = tr;
            this.en = en;
        }

        [BsonElement]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string tr { get; protected set; }

        [BsonElement]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string en { get; protected set; }
    }

    public class Lc : L
    {
        public Lc(string tr, string en)
            :base(tr, en)
        {
            
        }
    }
}
