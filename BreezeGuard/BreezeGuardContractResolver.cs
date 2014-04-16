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
            this.model = ApiModelCache.Get(contextProviderType);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            ApiEntityTypeConfiguration entityTypeConfiguration;

            if (this.model.Entities.TryGetValue(member.DeclaringType, out entityTypeConfiguration) &&
                entityTypeConfiguration.IgnoredProperties.Keys.Contains(member))
            {
                property.Ignored = true;
            }

            return property;
        }
    }
}
