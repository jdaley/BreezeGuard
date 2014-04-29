using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData.Builder;

namespace BreezeGuard
{
    internal static class ODataModelGenerator
    {
        public static IEdmModel CreateModel(ApiModelBuilder apiModel)
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            foreach (var entityTypeConfig in apiModel.GetEntityTypeConfigs())
            {
                EntityTypeConfiguration odataEntityTypeConfig = builder.AddEntity(entityTypeConfig.Type);

                foreach (var ignoredPropertyInfo in entityTypeConfig.GetIgnoredProperties())
                {
                    odataEntityTypeConfig.RemoveProperty(ignoredPropertyInfo);
                }
            }

            return builder.GetEdmModel();
        }
    }
}
