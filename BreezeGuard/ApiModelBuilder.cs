using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData.Builder;

namespace BreezeGuard
{
    public class ApiModelBuilder
    {
        internal Dictionary<Type, ApiEntityTypeConfiguration> Entities { get; private set; }
        internal IEdmModel ODataModel { get; private set; }

        internal ApiModelBuilder()
        {
            this.Entities = new Dictionary<Type, ApiEntityTypeConfiguration>();
            this.ODataModel = null;
        }

        public ApiEntityTypeConfiguration<TEntityType> Entity<TEntityType>()
            where TEntityType : class
        {
            ApiEntityTypeConfiguration typeConfiguration;

            if (!this.Entities.TryGetValue(typeof(TEntityType), out typeConfiguration))
            {
                typeConfiguration = new ApiEntityTypeConfiguration<TEntityType>();
                this.Entities.Add(typeof(TEntityType), typeConfiguration);
            }

            return (ApiEntityTypeConfiguration<TEntityType>)typeConfiguration;
        }

        internal void Build()
        {
            BuildODataModel();
        }

        private void BuildODataModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            foreach (var entityTypeConfig in this.Entities.Values)
            {
                EntityTypeConfiguration odataEntityTypeConfig = builder.AddEntity(entityTypeConfig.Type);

                foreach (var ignoredPropertyInfo in entityTypeConfig.IgnoredProperties.Keys)
                {
                    odataEntityTypeConfig.RemoveProperty(ignoredPropertyInfo);
                }
            }

            this.ODataModel = builder.GetEdmModel();
        }
    }
}
