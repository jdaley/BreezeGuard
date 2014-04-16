using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    internal class ApiModelCache
    {
        private static object cacheLock = new object();
        private static Dictionary<Type, ApiModelBuilder> models = new Dictionary<Type, ApiModelBuilder>();

        internal static ApiModelBuilder Get(Type contextProviderType,
            Action<ApiModelBuilder> onModelCreatingAction)
        {
            lock (cacheLock)
            {
                ApiModelBuilder model;

                if (!models.TryGetValue(contextProviderType, out model))
                {
                    model = new ApiModelBuilder();
                    onModelCreatingAction(model);
                    model.Build();
                    models.Add(contextProviderType, model);
                }

                return model;
            }
        }

        internal static ApiModelBuilder Get(Type contextProviderType)
        {
            lock (cacheLock)
            {
                ApiModelBuilder model;

                if (!models.TryGetValue(contextProviderType, out model))
                {
                    // BreezeGuardContextProvider constructor will add the model to the cache
                    Activator.CreateInstance(contextProviderType);

                    model = models[contextProviderType];
                }

                return model;
            }
        }
    }
}
