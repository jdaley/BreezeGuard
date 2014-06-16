using Breeze.ContextProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    /// <summary>
    /// Holds all entity and property data relating to a BreezeGuard save operation.
    /// </summary>
    public class SaveModel
    {
        private DbContext context;
        private Dictionary<Type, SaveEntitySet> entitySets;

        internal SaveModel(DbContext context)
        {
            this.context = context;
            this.entitySets = new Dictionary<Type, SaveEntitySet>();
        }

        public SaveEntitySet<TEntityType> Set<TEntityType>()
            where TEntityType : class
        {
            return (SaveEntitySet<TEntityType>)GetOrCreateEntitySet(typeof(TEntityType));
        }

        internal void AddRange(Type entityType, IEnumerable<EntityInfo> entityInfos)
        {
            GetOrCreateEntitySet(entityType).AddRange(entityInfos);
        }

        internal SaveEntitySet GetEntitySet(Type entityType)
        {
            SaveEntitySet entitySet;
            this.entitySets.TryGetValue(entityType, out entitySet);
            return entitySet;
        }

        internal IEnumerable<SaveEntitySet> GetEntitySets()
        {
            return this.entitySets.Values;
        }

        private SaveEntitySet GetOrCreateEntitySet(Type entityType)
        {
            SaveEntitySet entitySet;

            if (!this.entitySets.TryGetValue(entityType, out entitySet))
            {
                entitySet = SaveEntitySet.Create(entityType, this.context);
                this.entitySets.Add(entityType, entitySet);
            }

            return entitySet;
        }
    }
}
