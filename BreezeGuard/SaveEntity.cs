using Breeze.ContextProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public abstract class SaveEntity
    {
        public Type EntityType { get; private set; }
        public object Entity { get; set; }
        public EntityInfo EntityInfo { get; private set; } // TODO: Make this internal?
        protected Dictionary<PropertyInfo, SaveProperty> properties;

        protected SaveEntity(Type entityType, EntityInfo entityInfo, DbContext context)
        {
            this.EntityType = entityType;
            this.Entity = null;
            this.EntityInfo = entityInfo;
            this.properties = new Dictionary<PropertyInfo, SaveProperty>();

            EntityType edmEntityType = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace.GetItem<EntityType>(
                entityType.FullName, DataSpace.OSpace);

            foreach (EdmProperty edmProperty in edmEntityType.Properties)
            {
                if (edmProperty.Name == "Id") // TODO
                {
                    continue;
                }

                PropertyInfo propertyInfo = entityType.GetProperty(edmProperty.Name);

                object originalValue;
                bool hasOriginalValue = entityInfo.OriginalValuesMap.TryGetValue(propertyInfo.Name, out originalValue);
                object newValue = propertyInfo.GetValue(entityInfo.Entity);

                this.properties.Add(propertyInfo, SaveProperty.Create(
                    propertyInfo, hasOriginalValue, originalValue, newValue));
            }
        }

        public IEnumerable<SaveProperty> Properties
        {
            get { return properties.Values; }
        }

        public bool IsAdded
        {
            get { return this.EntityInfo.EntityState == Breeze.ContextProvider.EntityState.Added; }
        }

        public bool IsDeleted
        {
            get { return this.EntityInfo.EntityState == Breeze.ContextProvider.EntityState.Deleted; }
        }
    }

    public class SaveEntity<TEntityType> : SaveEntity
    {
        internal SaveEntity(EntityInfo entityInfo, DbContext context)
            : base(typeof(TEntityType), entityInfo, context) { }

        public new TEntityType Entity
        {
            get { return (TEntityType)base.Entity; }
            set { base.Entity = value; }
        }

        public IEnumerable<SaveEntity<TSubEntityType>> Get<TSubEntityType>()
        {
            return Enumerable.Empty<SaveEntity<TSubEntityType>>();
        }

        public SaveProperty<TProperty> Property<TProperty>(Expression<Func<TEntityType, TProperty>> propertyExpression)
        {
            return (SaveProperty<TProperty>)this.properties[ExpressionHelper.ToPropertyInfo(propertyExpression)];
        }

        public void Apply<TProperty>(Expression<Func<TEntityType, TProperty>> propertyExpression)
        {
            if (this.Entity == null)
            {
                throw new Exception("Entity is null.");
            }

            Property(propertyExpression).Apply(this.Entity);
        }

        public void Ignore<TProperty>(Expression<Func<TEntityType, TProperty>> propertyExpression)
        {
            Property(propertyExpression).Ignore();
        }
    }
}
