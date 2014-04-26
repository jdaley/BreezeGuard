using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public abstract class SaveProperty
    {
        public PropertyInfo PropertyInfo { get; private set; }
        public bool HasOriginalValue { get; private set; }
        public SavePropertyState State { get; protected set; }

        protected SaveProperty(PropertyInfo propertyInfo, bool hasOriginalValue, bool valuesAreEqual)
        {
            this.PropertyInfo = propertyInfo;
            this.HasOriginalValue = hasOriginalValue;
            this.State = (hasOriginalValue && valuesAreEqual) ? SavePropertyState.Unchanged : SavePropertyState.Pending;
        }

        public abstract bool IsOriginalValueEqual(object entity);
        public abstract bool IsNewValueEqual(object entity);
    }

    public class SaveProperty<TProperty> : SaveProperty
    {
        public TProperty OriginalValue { get; private set; }
        public TProperty NewValue { get; private set; }

        public SaveProperty(PropertyInfo propertyInfo, bool hasOriginalValue, TProperty originalValue, TProperty newValue)
            : base(propertyInfo, hasOriginalValue, object.Equals(originalValue, newValue))
        {
            this.OriginalValue = originalValue;
            this.NewValue = newValue;
        }

        public override bool IsOriginalValueEqual(object entity)
        {
            return object.Equals(this.PropertyInfo.GetValue(entity), this.OriginalValue);
        }

        public override bool IsNewValueEqual(object entity)
        {
            return object.Equals(this.PropertyInfo.GetValue(entity), this.NewValue);
        }

        public void Apply(object entity)
        {
            this.PropertyInfo.SetValue(entity, this.NewValue);
            this.State = SavePropertyState.Applied;
        }

        public void Ignore()
        {
            this.State = SavePropertyState.Ignored;
        }
    }
}
