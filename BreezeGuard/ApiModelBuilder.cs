using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData.Builder;

namespace BreezeGuard
{
    public class ApiModelBuilder
    {
        private string contextName;
        private MetadataWorkspace dbMetadata;
        private Dictionary<Type, EntityType> dbEntityTypes;
        private Dictionary<Type, ApiEntityTypeConfiguration> entityTypeConfigs;
        private List<Type> ignoredEntityTypes;

        internal ApiModelBuilder(string contextName, MetadataWorkspace dbMetadata)
        {
            this.contextName = contextName;
            this.dbMetadata = dbMetadata;

            ObjectItemCollection objectItemCollection =
                (ObjectItemCollection)dbMetadata.GetItemCollection(DataSpace.OSpace);

            this.dbEntityTypes = dbMetadata.GetItems<EntityType>(DataSpace.OSpace).ToDictionary(
                et => objectItemCollection.GetClrType(et));

            this.entityTypeConfigs = new Dictionary<Type, ApiEntityTypeConfiguration>();
            this.ignoredEntityTypes = null;
        }

        public ApiEntityTypeConfiguration<TEntityType> Entity<TEntityType>()
            where TEntityType : class
        {
            Type entityType = typeof(TEntityType);
            ApiEntityTypeConfiguration entityTypeConfig;

            if (!this.entityTypeConfigs.TryGetValue(entityType, out entityTypeConfig))
            {
                EntityType dbEntityType;

                if (!this.dbEntityTypes.TryGetValue(entityType, out dbEntityType))
                {
                    throw new Exception(string.Format("There is no entity {0} in context {1}.",
                        entityType, contextName));
                }

                entityTypeConfig = new ApiEntityTypeConfiguration<TEntityType>(contextName, dbEntityType);

                this.entityTypeConfigs.Add(entityType, entityTypeConfig);
            }

            return (ApiEntityTypeConfiguration<TEntityType>)entityTypeConfig;
        }

        internal void Build()
        {
            this.ignoredEntityTypes = this.dbEntityTypes.Keys.Except(this.entityTypeConfigs.Keys).ToList();

            // Clear references to EF metadata objects
            this.dbMetadata = null;
            this.dbEntityTypes = null;
        }

        internal IEnumerable<ApiEntityTypeConfiguration> GetEntityTypeConfigs()
        {
            return this.entityTypeConfigs.Values;
        }

        internal bool TryGetEntityTypeConfig(Type type, out ApiEntityTypeConfiguration entityTypeConfig)
        {
            return this.entityTypeConfigs.TryGetValue(type, out entityTypeConfig);
        }

        internal IEnumerable<Type> GetIgnoredEntityTypes()
        {
            return this.ignoredEntityTypes;
        }
    }
}
