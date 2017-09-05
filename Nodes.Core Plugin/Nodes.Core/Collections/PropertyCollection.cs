using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UNEB.Internal;


namespace UNEB.Collections
{
    /// <summary>
    /// Serializable Property Collection.
    /// </summary>
    [Serializable]
    public class PropertyCollection : MixedTypeSerializableList<Property>
    {
        public void SetProperty<T>(string propertyName, T value)
        {
            Property p = Find<T>(propertyName);
            if (!p)
            {
                p = new Property(propertyName);
                Add(p);
            }
            p.Value = value;
        }

        public T GetProperty<T>(string propertyName, T defaultValue = default(T))
        {
            Property p = Find<T>(propertyName);
            if (p) return (T)p.Value;
            return defaultValue;
        }

        #region Helpers






        protected Property Find<T>(string name)
        { 
            name = Property.SafeName(name);
            return this.FirstOrDefault((Property p)=>
            {
                return p && p.IsType<T>() && p.Name == name;
            });
        }

        protected IEnumerable<Property> GetPropertiesByNameAndType<T>(string name)
        {
            name = Property.SafeName(name);
            return this.Where((Property p) =>
            {
                return p && p.IsType<T>() && p.Name == name;
            });
        }

        protected IEnumerable<Property> GetPropertiesByType<T>()
        {
            return this.Where((Property p) =>
            {
                return p && p.IsType<T>();
            });
        }

        protected IEnumerable<Property> GetPropertiesByName(string name)
        {
            name = Property.SafeName(name);
            return this.Where((Property p) =>
            {
                return p && p.Name == name;
            });
        }

#endregion

        #region Hide List methods

        new private int IndexOf(Property item)
        {
            return base.IndexOf(item);
        }

        new private void Add(Property item)
        {
            base.Add(item);
        }

        new private void Remove(Property item)
        {
            base.Remove(item);
        }

        new private void RemoveAt(int index)
        {
            base.RemoveAt(index);
        }

        new private void Insert(int index, Property item)
        {
            base.Insert(index, item);
        }

        new private Property this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }

#endregion

    }
}
