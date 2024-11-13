using Scf.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Database
{

    internal class EnumeratorWrapper<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;
        private readonly IBaseEntitySet<T> modelSet;

        public EnumeratorWrapper(IEnumerator<T> enumerator, IBaseEntitySet<T> modelSet)
        {
            this.enumerator = enumerator;
            this.modelSet = modelSet;
        }

        public T Current
        {
            get
            {
                if (enumerator.Current is IEntity entity)
                {
                    entity.Context = modelSet.Context;
                }

                if (modelSet.Context.EnableCaching && !modelSet.IsEntityTableContains(enumerator.Current))
                {
                    modelSet.SetToEntityTable(enumerator.Current);
                }

                return enumerator.Current;
            }
        }


#pragma warning disable CS8603 // Possible null reference return.
        object IEnumerator.Current => this.Current;
#pragma warning restore CS8603 // Possible null reference return.

        public void Dispose()
        {
            enumerator.Dispose();
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }
    }
}
