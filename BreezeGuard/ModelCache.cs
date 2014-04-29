using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    internal static class ModelCache
    {
        private static object cacheLock = new object();
        private static Dictionary<Type, Entry> entries = new Dictionary<Type, Entry>();

        internal static void RegisterCreateModelFunc(Type contextProviderType,
            Func<ApiModelBuilder> createModelFunc)
        {
            lock (cacheLock)
            {
                Entry entry = GetOrCreateEntry(contextProviderType);

                if (entry.apiModel == null)
                {
                    entry.createModelFunc = createModelFunc;
                }
            }
        }

        internal static ApiModelBuilder GetApiModel(Type contextProviderType)
        {
            lock (cacheLock)
            {
                Entry entry = GetOrCreateEntry(contextProviderType);

                if (entry.apiModel == null)
                {
                    if (entry.createModelFunc == null)
                    {
                        // BreezeGuardContextProvider constructor will register the createModelFunc
                        Activator.CreateInstance(contextProviderType);

                        // If it's still null, contextProviderType isn't a BreezeGuardContextProvider
                        if (entry.createModelFunc == null)
                        {
                            throw new ArgumentException(
                                "contextProviderType must be a type that extends BreezeGuardContextProvider.");
                        }
                    }

                    entry.apiModel = entry.createModelFunc();
                    entry.createModelFunc = null;
                }

                return entry.apiModel;
            }
        }

        internal static IEdmModel GetODataModel(Type contextProviderType)
        {
            lock (cacheLock)
            {
                Entry entry = GetOrCreateEntry(contextProviderType);

                if (entry.odataModel == null)
                {
                    ApiModelBuilder apiModel = GetApiModel(contextProviderType);
                    entry.odataModel = ODataModelGenerator.CreateModel(apiModel);
                }

                return entry.odataModel;
            }
        }

        private static Entry GetOrCreateEntry(Type contextProviderType)
        {
            Entry entry;

            if (!entries.TryGetValue(contextProviderType, out entry))
            {
                entry = new Entry();
                entries.Add(contextProviderType, entry);
            }

            return entry;
        }

        private class Entry
        {
            public Func<ApiModelBuilder> createModelFunc;
            public ApiModelBuilder apiModel;
            public IEdmModel odataModel;
        }
    }
}
