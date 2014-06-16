using Breeze.ContextProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public abstract class SaveEntitySet : IEnumerable<SaveEntity>
    {
        internal static SaveEntitySet Create(Type entityType, DbContext context)
        {
            Type saveEntitySetType = typeof(SaveEntitySet<>).MakeGenericType(entityType);

            ConstructorInfo saveEntitySetConstructor = saveEntitySetType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(DbContext) }, null);

            return (SaveEntitySet)saveEntitySetConstructor.Invoke(new object[] { context });
        }

        public Type EntityType { get; private set; }
        protected DbContext context;
        protected List<SaveEntity> entities;

        internal SaveEntitySet(Type entityType, DbContext context)
        {
            this.EntityType = entityType;
            this.context = context;
            this.entities = new List<SaveEntity>();
        }

        internal abstract void AddRange(IEnumerable<EntityInfo> entityInfos);

        public IEnumerator<SaveEntity> GetEnumerator()
        {
            return this.entities.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.entities.GetEnumerator();
        }
    }

    public class SaveEntitySet<TEntityType> : SaveEntitySet, IEnumerable<SaveEntity<TEntityType>>
        where TEntityType : class
    {
        internal SaveEntitySet(DbContext context)
            : base(typeof(TEntityType), context) { }

        internal override void AddRange(IEnumerable<EntityInfo> entityInfos)
        {
            Type saveEntityType = typeof(SaveEntity<>).MakeGenericType(this.EntityType);

            ConstructorInfo saveEntityConstructor = saveEntityType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(EntityInfo), typeof(DbContext) }, null);

            this.entities.AddRange(entityInfos.Select(ei =>
                (SaveEntity)saveEntityConstructor.Invoke(new object[] { ei, this.context })));
        }

        public new IEnumerator<SaveEntity<TEntityType>> GetEnumerator()
        {
            return this.entities.Cast<SaveEntity<TEntityType>>().GetEnumerator();
        }
    }
}
