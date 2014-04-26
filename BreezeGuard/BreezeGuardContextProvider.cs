﻿using Breeze.ContextProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public abstract class BreezeGuardContextProvider<TContext> : ContextProvider where TContext : DbContext
    {
        private static string jsonMetadata = null;
        private static object jsonMetadataLock = new object();

        private TContext context;
        private ApiModelBuilder model;

        protected BreezeGuardContextProvider()
        {
            this.context = null;
            this.model = ApiModelCache.Get(GetType(), OnModelCreating);
        }

        public TContext Context
        {
            get
            {
                if (this.context == null)
                {
                    this.context = CreateContext();
                }

                return this.context;
            }
        }

        public ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this.Context).ObjectContext; }
        }

        protected virtual TContext CreateContext()
        {
            return Activator.CreateInstance<TContext>();
        }

        protected virtual DbContext CreateMetadataContext()
        {
            return MetadataContextHelper.EmitMetadataContext<TContext>();
        }

        protected abstract void OnModelCreating(ApiModelBuilder modelBuilder);

        protected override string BuildJsonMetadata()
        {
            lock (jsonMetadataLock)
            {
                if (jsonMetadata == null)
                {
                    DbContext metadataContext = CreateMetadataContext();
                    jsonMetadata = MetadataContextHelper.GetMetadataFromContext(metadataContext);
                }

                return jsonMetadata;
            }
        }

        public override IDbConnection GetDbConnection()
        {
            return this.ObjectContext.Connection;
        }

        protected override void OpenDbConnection()
        {
            var ec = this.ObjectContext.Connection as EntityConnection;
            if (ec.State == ConnectionState.Closed) ec.Open();
        }

        protected override void CloseDbConnection()
        {
            var ec = this.ObjectContext.Connection as EntityConnection;
            ec.Close();
            ec.Dispose();
        }

        protected override void SaveChangesCore(SaveWorkState saveWorkState)
        {
            Dictionary<Type, List<SaveModel>> saveModelsMap = BuildSaveModelsMap(saveWorkState);

            LoadEntities(saveModelsMap);
            ExecuteSaveHandlers(saveModelsMap);
            ValidateSaveState(saveModelsMap);

            try
            {
                this.Context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                // TODO: Set saveWorkState.EntityErrors
                throw new NotImplementedException();
            }

            // TODO: Set saveWorkState.KeyMappings
            // TODO: Update saveWorkState entities from the ones in this.Context
        }

        private Dictionary<Type, List<SaveModel>> BuildSaveModelsMap(SaveWorkState saveWorkState)
        {
            Dictionary<Type, List<SaveModel>> saveModelsMap = new Dictionary<Type, List<SaveModel>>();

            foreach (var kvp in saveWorkState.SaveMap)
            {
                Type entityType = kvp.Key;
                List<EntityInfo> entityInfos = kvp.Value;

                Type saveModelType = typeof(SaveModel<>).MakeGenericType(entityType);
                List<SaveModel> saveModels = new List<SaveModel>();

                foreach (EntityInfo entityInfo in entityInfos)
                {
                    saveModels.Add((SaveModel)Activator.CreateInstance(saveModelType, entityInfo, this.Context, this.model));
                }

                saveModelsMap.Add(entityType, saveModels);
            }

            return saveModelsMap;
        }

        protected virtual void LoadEntities(Dictionary<Type, List<SaveModel>> saveModelsMap)
        {
            throw new NotImplementedException();
        }

        private void ExecuteSaveHandlers(Dictionary<Type, List<SaveModel>> saveModelsMap)
        {
            List<ISaveHandler> saveHandlers = CreateSaveHandlers();

            foreach (ISaveHandler saveHandler in saveHandlers)
            {
                List<SaveModel> saveModels;

                if (saveModelsMap.TryGetValue(saveHandler.EntityType, out saveModels))
                {
                    foreach (SaveModel saveModel in saveModels)
                    {
                        saveHandler.Save(saveModel);
                    }
                }
            }
        }

        protected virtual List<ISaveHandler> CreateSaveHandlers()
        {
            throw new NotImplementedException();
        }

        protected virtual void ValidateSaveState(Dictionary<Type, List<SaveModel>> saveModelsMap)
        {
            foreach (var kvp in saveModelsMap)
            {
                Type entityType = kvp.Key;
                List<SaveModel> saveModels = kvp.Value;

                foreach (SaveModel saveModel in saveModels)
                {
                    // Check that all added/deleted entities have been processed
                    // TODO: Have they?

                    // Check that all property values have been processed
                    foreach (SaveProperty saveProperty in saveModel.Properties)
                    {
                        if (saveProperty.State == SavePropertyState.Pending &&
                            !saveProperty.IsNewValueEqual(saveModel.Entity))
                        {
                            throw new Exception("Property value was not saved by a SaveHandler.");
                        }
                    }
                }
            }
        }
    }
}
