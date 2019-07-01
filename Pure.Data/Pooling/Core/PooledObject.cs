using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Pure.Data.Pooling
{
    [Serializable]
    public abstract class PooledObject : IDisposable, IEquatable<PooledObject>
    {
        public OutputActionDelegate LogAction { get; set; }
  
        #region Properties

        /// <summary>
        ///   Core information about this <see cref="PooledObject"/>.
        /// </summary>
        public PooledObjectInfo PooledObjectInfo { get; } = new PooledObjectInfo();

        #endregion Properties

        #region Internal Methods - resource and state management

        /// <summary>
        ///   Releases the object resources. This method will be called by the pool manager when
        ///   there is no need for this object anymore (decreasing pooled objects count, pool is
        ///   being destroyed).
        /// </summary>
        internal bool ReleaseResources()
        {
            if (OnReleaseResources != null)
            {
                try
                {
                    OnReleaseResources(this);
                }
                catch (Exception ex)
                {
                    LogAction("[ObjectPool] An unexpected error occurred while releasing resources", ex, MessageType.Error);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Reset the object state. This method will be called by the pool manager just before the
        ///   object is being returned to the pool.
        /// </summary>
        internal bool ResetState()
        {
            if (!ValidateObject(PooledObjectValidationContext.Inbound(this)))
            {
                return false;
            }
            if (OnResetState != null)
            {
                try
                {
                    OnResetState(this);
                }
                catch (Exception ex)
                {
                    LogAction("[ObjectPool] An unexpected error occurred while resetting state", ex, MessageType.Error);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Validates pooled object state. An invalid object will not get into the pool and it will
        ///   not be returned to consumers.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>True if current pooled object is valid, false otherwise.</returns>
        internal bool ValidateObject(PooledObjectValidationContext validationContext)
        {
            if (OnValidateObject != null)
            {
                try
                {
                    return OnValidateObject.GetInvocationList()
                        .Cast<Func<PooledObjectValidationContext, bool>>()
                        .All(validationDelegate => validationDelegate(validationContext));
                }
                catch (Exception ex)
                {
                    LogAction("[ObjectPool] An unexpected error occurred while validating an object", ex, MessageType.Error);
                    return false;
                }
            }
            return true;
        }

        #endregion Internal Methods - resource and state management

        #region Events - extending resource and state management

        public Action<PooledObject> OnEvictResource { get; set; }
        public Action<PooledObject> OnGetResource { get; set; }
        public Action<PooledObject> OnCreateResource { get; set; }
        public Action<PooledObject> OnReturnResource { get; set; }

        /// <summary>
        ///   Releases the object's resources.
        /// </summary>
        public Action<PooledObject> OnReleaseResources { get; set; }

        /// <summary>
        ///   Reset the object state to allow this object to be re-used by other parts of the application.
        /// </summary>
        public Action<PooledObject> OnResetState { get; set; }

        /// <summary>
        ///   Validates pooled object state. An invalid object will not get into the pool and it will
        ///   not be returned to consumers.
        /// </summary>
        public Func<PooledObjectValidationContext, bool> OnValidateObject { get; set; }

        #endregion Events - extending resource and state management

        #region Returning object to pool - Dispose and Finalizer

        /// <summary>
        ///   PooledObject destructor.
        /// </summary>
        ~PooledObject()
        {
            // Resurrecting the object
            HandleReAddingToPool(true);
        }

        /// <summary>
        ///   See <see cref="IDisposable"/> docs.
        /// </summary>
        public void Dispose()
        {
            // Returning to pool
            HandleReAddingToPool(false);
        }

        private void HandleReAddingToPool(bool reRegisterForFinalization)
        {
            // Only when the object is reserved it can be readded to the pool.
            if (PooledObjectInfo == null ||
                PooledObjectInfo.State == PooledObjectState.Disposed ||
                PooledObjectInfo.State == PooledObjectState.Available)
            {
                return;
            }

            // If there is any case that the re-adding to the pool fails, release the resources and
            // set the internal Disposed flag to true.
            try
            {
                // Notifying the pool that this object is ready for re-adding to the pool.
                PooledObjectInfo.Handle.ReturnObjectToPool(this, reRegisterForFinalization);
            }
            catch (Exception ex)
            {
                LogAction("[ObjectPool] An error occurred while re-adding to pool", ex,  MessageType.Error);
                PooledObjectInfo.State = PooledObjectState.Disposed;
                ReleaseResources();
            }
        }

        #endregion Returning object to pool - Dispose and Finalizer

        #region Formatting and equality

        /// <summary>
        ///   Compares to pooled objects.
        /// </summary>
        /// <param name="left">Left object.</param>
        /// <param name="right">Right object.</param>
        /// <returns>True if given pooled objects are not equal, false otherwise.</returns>
        public static bool operator !=(PooledObject left, PooledObject right) => !Equals(left, right);

        /// <summary>
        ///   Compares to pooled objects.
        /// </summary>
        /// <param name="left">Left object.</param>
        /// <param name="right">Right object.</param>
        /// <returns>True if given pooled objects are equal, false otherwise.</returns>
        public static bool operator ==(PooledObject left, PooledObject right) => Equals(left, right);

        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///   true if the current object is equal to the <paramref name="other"/> parameter;
        ///   otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual bool Equals(PooledObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PooledObjectInfo.Equals(other.PooledObjectInfo);
        }

        /// <summary>
        ///   Determines whether the specified <see cref="object"/> is equal to the current <see cref="PooledObject"/>.
        /// </summary>
        /// <returns>
        ///   true if the specified <see cref="object"/> is equal to the current <see
        ///   cref="PooledObject"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="PooledObject"/>.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as PooledObject);
        }

        /// <summary>
        ///   Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="PooledObject"/>.</returns>
        public override int GetHashCode() => PooledObjectInfo.GetHashCode();

        /// <summary>
        ///   Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => PooledObjectInfo.ToString();

        #endregion Formatting and equality
    }
}
