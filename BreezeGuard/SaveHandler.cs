using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public interface ISaveHandler
    {
        Type EntityType { get; }
        DbContext Context { get; set; }
        void Save(SaveModel saveModel);
    }

    public abstract class SaveHandler<TEntityType, TContext> : ISaveHandler
        where TEntityType : class
        where TContext : DbContext
    {
        public TContext Context { get; set; }

        public Type EntityType
        {
            get { return typeof(TEntityType); }
        }

        public abstract void Save(SaveModel<TEntityType> saveModel);

        DbContext ISaveHandler.Context
        {
            get { return this.Context; }
            set { this.Context = (TContext)value; }
        }

        void ISaveHandler.Save(SaveModel saveModel)
        {
            Save((SaveModel<TEntityType>)saveModel);
        }
    }
}
