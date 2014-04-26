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
        public static SaveProperty Create(PropertyInfo propertyInfo, bool hasOriginalValue,
            object originalValue, object newValue)
        {
            Type savePropertyType = typeof(SaveProperty<>).MakeGenericType(propertyInfo.PropertyType);
            return (SaveProperty)Activator.CreateInstance(savePropertyType,
                propertyInfo, hasOriginalValue, originalValue, newValue);
        }

        public PropertyInfo PropertyInfo { get; private set; }
        public bool HasOriginalValue { get; private set; }
        public object OriginalValue { get; private set; }
        public object NewValue { get; private set; }
        public SavePropertyState State { get; protected set; }

        protected SaveProperty(PropertyInfo propertyInfo, bool hasOriginalValue,
            object originalValue, object newValue)
        {
            this.PropertyInfo = propertyInfo;
            this.HasOriginalValue = hasOriginalValue;
            this.OriginalValue = originalValue;
            this.NewValue = newValue;

            if (hasOriginalValue && object.Equals(originalValue, newValue))
            {
                this.State = SavePropertyState.Unchanged;
            }
            else
            {
                this.State = SavePropertyState.Pending;
            }
        }

        public abstract bool IsOriginalValueEqual(object entity);
        public abstract bool IsNewValueEqual(object entity);

        public override string ToString()
        {
            return this.PropertyInfo + " [" + this.State + "]";
        }
    }

    public class SaveProperty<TProperty> : SaveProperty
    {
        public SaveProperty(PropertyInfo propertyInfo, bool hasOriginalValue,
            TProperty originalValue, TProperty newValue)
            : base(propertyInfo, hasOriginalValue, originalValue, newValue) { }

        public new TProperty OriginalValue
        {
            get { return (TProperty)base.OriginalValue; }
        }

        public new TProperty NewValue
        {
            get { return (TProperty)base.NewValue; }
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
