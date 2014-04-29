using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public class BreezeGuardContractResolver : DefaultContractResolver
    {
        public Type ContextProviderType { get; private set; }
        private ApiModelBuilder model;

        public BreezeGuardContractResolver(Type contextProviderType)
        {
            this.ContextProviderType = contextProviderType;
            this.model = null;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (this.model == null)
            {
                this.model = ModelCache.GetApiModel(this.ContextProviderType);
            }

            ApiEntityTypeConfiguration entityTypeConfig;
            ApiPropertyConfiguration propertyConfig;

            if (this.model.TryGetEntityTypeConfig(member.DeclaringType, out entityTypeConfig) &&
                entityTypeConfig.TryGetPropertyConfig((PropertyInfo)member, out propertyConfig) &&
                propertyConfig.Ignored)
            {
                property.Ignored = true;
            }

            return property;
        }
    }
}
