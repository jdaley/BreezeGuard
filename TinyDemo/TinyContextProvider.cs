using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using BreezeGuard;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TinyDemo
{
    public class TinyContextProvider : EFContextProvider<TinyContext>
    {
        static TinyContextProvider()
        {
            BreezeConfig.Instance.GetJsonSerializerSettings().ContractResolver = new BreezeGuardContractResolver();
        }

        protected override string BuildJsonMetadata()
        {
            return EFContextProvider<TinyMetadataContext>.GetMetadataFromContext(new TinyMetadataContext());
        }
    }
}