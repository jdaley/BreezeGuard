using Breeze.ContextProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public abstract class BreezeGuardContextProvider<TContext> : ContextProvider
        where TContext : DbContext
    {
        private static string jsonMetadata = null;
        private static object jsonMetadataLock = new object();

        private TContext context;
        private ApiModelBuilder model;
        private Dictionary<Type, object> resources;

        protected BreezeGuardContextProvider()
        {
            this.context = null;
            this.model = null;
            this.resources = new Dictionary<Type, object>();

            ModelCache.RegisterCreateModelFunc(GetType(), CreateModel);
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

        internal ApiModelBuilder Model
        {
            get
            {
                if (this.model == null)
                {
                    this.model = ModelCache.GetApiModel(GetType());
                }

                return this.model;
            }
        }

        private ApiModelBuilder CreateModel()
        {
            ApiModelBuilder modelBuilder = new ApiModelBuilder(
                typeof(TContext).FullName, this.ObjectContext.MetadataWorkspace);

            OnModelCreating(modelBuilder);

            modelBuilder.Build();

            return modelBuilder;
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

        public void AddResource<TResource>(TResource resource)
        {
            this.resources.Add(typeof(TResource), resource);
        }

        protected override void SaveChangesCore(SaveWorkState saveWorkState)
        {
            SaveModel saveModel = BuildSaveModel(saveWorkState);

            LoadEntities(saveModel);
            ExecuteSaveHandlers(saveModel);
            ValidateSaveState(saveModel);

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

        private SaveModel BuildSaveModel(SaveWorkState saveWorkState)
        {
            SaveModel saveModel = new SaveModel(this.context);

            foreach (var kvp in saveWorkState.SaveMap)
            {
                Type entityType = kvp.Key;
                List<EntityInfo> entityInfos = kvp.Value;

                saveModel.AddRange(entityType, entityInfos);
            }

            return saveModel;
        }

        protected virtual void LoadEntities(SaveModel saveModel)
        {
            foreach (SaveEntitySet saveEntitySet in saveModel.GetEntitySets())
            {
                Type entityType = saveEntitySet.EntityType;
                ApiEntityTypeConfiguration entityTypeConfig;

                if (!this.Model.TryGetEntityTypeConfig(entityType, out entityTypeConfig))
                {
                    throw new Exception("Entity type not supported.");
                }
                
                object resource = this.resources[entityTypeConfig.ResourceType];
                IQueryable queryable = (IQueryable)entityTypeConfig.ResourceAccessor.DynamicInvoke(resource);

                // TODO: It's not always int IDs
                PropertyInfo idPropertyInfo = entityType.GetProperty("Id");
                List<int> ids = saveEntitySet.Where(m => !m.IsAdded).Select(
                    m => (int)idPropertyInfo.GetValue(m.EntityInfo.Entity)).ToList();

                var arg = Expression.Parameter(entityType, "p");

                var body = Expression.Call(Expression.Constant(ids), "Contains", null,
                    Expression.Property(arg, "Id"));

                var predicate = Expression.Lambda(body, arg);

                var where = Expression.Call(typeof(Queryable), "Where",
                    new Type[] { queryable.ElementType },
                    queryable.Expression, Expression.Quote(predicate));

                foreach (object entity in queryable.Provider.CreateQuery(where))
                {
                    int id = (int)idPropertyInfo.GetValue(entity);
                    SaveEntity saveEntity = saveEntitySet.First(m => (int)idPropertyInfo.GetValue(m.EntityInfo.Entity) == id);
                    saveEntity.Entity = entity;
                }
            }
        }

        private void ExecuteSaveHandlers(SaveModel saveModel)
        {
            List<ISaveHandler> saveHandlers = CreateSaveHandlers();

            foreach (ISaveHandler saveHandler in saveHandlers)
            {
                SaveEntitySet saveEntitySet = saveModel.GetEntitySet(saveHandler.EntityType);

                if (saveEntitySet != null)
                {
                    foreach (SaveEntity saveEntity in saveEntitySet)
                    {
                        saveHandler.Save(saveEntity);
                    }
                }
            }
        }

        protected virtual List<ISaveHandler> CreateSaveHandlers()
        {
            throw new NotImplementedException();
        }

        protected virtual void ValidateSaveState(SaveModel saveModel)
        {
            foreach (SaveEntitySet saveEntitySet in saveModel.GetEntitySets())
            {
                foreach (SaveEntity saveEntity in saveEntitySet)
                {
                    // Check that all added/deleted entities have been processed
                    // TODO: Have they?

                    // Check that all property values have been processed
                    foreach (SaveProperty saveProperty in saveEntity.Properties)
                    {
                        if (saveProperty.State == SavePropertyState.Pending &&
                            !saveProperty.IsNewValueEqual(saveEntity.Entity))
                        {
                            throw new Exception("Property value was not saved by a SaveHandler.");
                        }
                    }
                }
            }
        }
    }
}
