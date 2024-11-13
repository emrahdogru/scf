using Scf.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Database
{
    public class EntitySetIndex<T>
    {
        public EntitySetIndex(Func<IndexKeysDefinitionBuilder<T>, IndexKeysDefinition<T>> keys, CreateIndexOptions<T> options)
        {
            this.Keys = keys;
            this.Options = options;
        }

        public EntitySetIndex(Func<IndexKeysDefinitionBuilder<T>, IndexKeysDefinition<T>> keys)
        {
            this.Keys = keys;
            this.Options = new CreateIndexOptions<T>();
        }

        public Func<IndexKeysDefinitionBuilder<T>, IndexKeysDefinition<T>> Keys { get; }
        public CreateIndexOptions<T> Options { get; }

        public CreateIndexModel<T> GetIndexModel()
        {
            return new CreateIndexModel<T>(Keys.Invoke(new IndexKeysDefinitionBuilder<T>()), this.Options);
        }
    }
}
