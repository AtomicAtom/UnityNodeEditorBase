using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UNEB.Internal
{
    /// <summary>
    /// Baseclass for everything with Serialization Events
    /// </summary>
    [Serializable]
    public abstract class SerializableObject : 
        ISerializationCallbackReceiver,
        IDisposable
    {
        protected event Action // Events which can be used when custom serialization logic is required
            OnDestroy,
            OnSerialize,
            OnDeserialize;

        /// <summary>
        /// Internal only version of OnSerialize and OnDeserialize so the user
        /// cannot mark them as null or replace their bindings - as any Core
        /// handlers would break in such a case.
        /// </summary>
        internal event Action 
              OnDestroyInternal,
              OnSerializeInternal,
              OnDeserializeInternal;

        bool
            m_IsDestroyed,
            m_IsDestroying;


        public bool IsDestroyed
        {
            get
            {
                return m_IsDestroyed || m_IsDestroying;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (IsDestroyed) return;
            OnDeserializeInternal.TryInvoke();
            OnDeserialize.TryInvoke();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (IsDestroyed) return;
            OnSerialize.TryInvoke();
            OnSerializeInternal.TryInvoke();
        }


        /// <summary>
        /// Throw an exception if <see cref="IsDestroyed"/> is true;
        /// </summary>
        protected void ThrowIfDestroyed()
        {
            if (IsDestroyed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        public void Destroy()
        {
            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            if (IsDestroyed) return;
            m_IsDestroying = true;
            OnDestroy.TryInvoke();
            OnDeserializeInternal.TryInvoke();
            m_IsDestroyed = true;
            GC.SuppressFinalize(this);
        }




        #region Operators

        /// <summary>
        /// Allows these objects to be cast as bool like unity objects.
        /// </summary>
        /// <param name="type"></param>
        public static implicit operator bool(SerializableObject type)
        {
            return type != null && !type.IsDestroyed;
        }

        #endregion

    }

}
