#region License

/*
 * Copyright � 2002-2006 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#region Imports

using System;
using NHibernate;
using NHibernate.Type;
using Spring.Dao;

#endregion

namespace Spring.Data.NHibernate
{
    /// <summary>
    /// Interface that specifies a set of Hibernate operations that
    /// are common across versions of Hibernate.
    /// </summary>
    /// <remarks>
    /// Base interface for generic and non generic IHibernateOperations interfaces
    /// Not often used, but a useful option
    /// to enhance testability, as it can easily be mocked or stubbed.
    /// <p>Provides HibernateTemplate's data access methods that mirror
    /// various Session methods. See the NHibernate ISession documentation
    /// for details on those methods.
    /// </p>
    /// </remarks> 
    /// <author>Mark Pollack (.NET)</author>  
    /// <threadsafety statis="true" instance="true"/> 
    /// <version>$Id: ICommonHibernateOperations.cs,v 1.2 2007/09/19 22:58:22 markpollack Exp $</version>
    public interface ICommonHibernateOperations
    {
        /// <summary>
        /// Remove all objects from the Session cache, and cancel all pending saves,
        /// updates and deletes.
        /// </summary>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Clear();


        /// <summary>
        /// Delete the given persistent instance.
        /// </summary>
        /// <param name="entity">The persistent instance to delete.</param>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Delete(object entity);

        /// <summary>
        /// Delete the given persistent instance.
        /// </summary>
        /// <remarks>
        /// Obtains the specified lock mode if the instance exists, implicitly
        /// checking whether the corresponding database entry still exists
        /// (throwing an OptimisticLockingFailureException if not found).
        /// </remarks>
        /// <param name="entity">Tthe persistent instance to delete.</param>
        /// <param name="lockMode">The lock mode to obtain.</param>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Delete(object entity, LockMode lockMode);


        /// <summary>
        /// Delete all objects returned by the query.
        /// </summary>
        /// <param name="queryString">a query expressed in Hibernate's query language.</param>
        /// <returns>The number of entity instances deleted.</returns>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        int Delete(string queryString);

        /// <summary>
        ///  Delete all objects returned by the query.
        /// </summary>
        /// <param name="queryString">a query expressed in Hibernate's query language.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="type">The Hibernate type of the parameter (or <code>null</code>).</param>
        /// <returns>The number of entity instances deleted.</returns>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        int Delete(string queryString, object value, IType type);


        /// <summary>
        /// Delete all objects returned by the query.
        /// </summary>
        /// <param name="queryString">a query expressed in Hibernate's query language.</param>
        /// <param name="values">The values of the parameters.</param>
        /// <param name="types"> Hibernate types of the parameters (or <code>null</code>)</param>
        /// <returns>The number of entity instances deleted.</returns>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        int Delete(String queryString, Object[] values, IType[] types);


        /// <summary>
        /// Flush all pending saves, updates and deletes to the database.
        /// </summary>
        /// <remarks>
        /// Only invoke this for selective eager flushing, for example when ADO.NET code
        /// needs to see certain changes within the same transaction. Else, it's preferable
        /// to rely on auto-flushing at transaction completion.
        /// </remarks>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Flush();

        #region Convenience methods for loading individual objects

        /// <summary>
        /// Load the persistent instance with the given identifier
        /// into the given object, throwing an exception if not found.
        /// </summary>
        /// <param name="entity">Entity the object (of the target class) to load into.</param>
        /// <param name="id">An identifier of the persistent instance.</param>
        /// <exception cref="ObjectRetrievalFailureException">If object not found.</exception>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Load(object entity, object id);


        /// <summary>
        /// Re-read the state of the given persistent instance.
        /// </summary>
        /// <param name="entity">The persistent instance to re-read.</param>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Refresh(object entity);

        /// <summary>
        /// Re-read the state of the given persistent instance.
        /// Obtains the specified lock mode for the instance.
        /// </summary>
        /// <param name="entity">The persistent instance to re-read.</param>
        /// <param name="lockMode">The lock mode to obtain.</param>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Refresh(object entity, LockMode lockMode);

        /// <summary>
        /// Determines whether the given object is in the Session cache.
        /// </summary>
        /// <param name="entity">the persistence instance to check.</param>
        /// <returns>
        /// 	<c>true</c> if session cache contains the specified entity; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        bool Contains(object entity);

        /// <summary>
        /// Remove the given object from the Session cache.
        /// </summary>
        /// <param name="entity">The persistent instance to evict.</param>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Evict(object entity);

        #endregion

        #region Convenience methods for storing individual objects


        /// <summary>
        /// Obtain the specified lock level upon the given object, implicitly
        /// checking whether the corresponding database entry still exists
        /// (throwing an OptimisticLockingFailureException if not found).
        /// </summary>
        /// <param name="entity">The he persistent instance to lock.</param>
        /// <param name="lockMode">The lock mode to obtain.</param>
        /// <exception cref="ObjectOptimisticLockingFailureException">If not found</exception>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Lock(object entity, LockMode lockMode);


        /// <summary>
        /// Persist the given transient instance.
        /// </summary>
        /// <param name="entity">The transient instance to persist.</param>
        /// <returns>The generated identifier.</returns>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        object Save(object entity);

        /// <summary>
        /// Persist the given transient instance with the given identifier.
        /// </summary>
        /// <param name="entity">The transient instance to persist.</param>
        /// <param name="id">The identifier to assign.</param>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Save(object entity, object id);

        /// <summary>
        /// Update the given persistent instance.
        /// </summary>
        /// <param name="entity">The persistent instance to update.</param>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Update(object entity);

        /// <summary>
        /// Update the given persistent instance.
        /// Obtains the specified lock mode if the instance exists, implicitly
        /// checking whether the corresponding database entry still exists
        /// (throwing an OptimisticLockingFailureException if not found).
        /// </summary>
        /// <param name="entity">The persistent instance to update.</param>
        /// <param name="lockMode">The lock mode to obtain.</param>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void Update(object entity, LockMode lockMode);

        /// <summary>
        /// Save or update the given persistent instance,
        /// according to its id (matching the configured "unsaved-value"?).
        /// </summary>
        /// <param name="entity">Tthe persistent instance to save or update
        /// (to be associated with the Hibernate Session).</param>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        void SaveOrUpdate(object entity);

        /// <summary>
        /// Save or update the contents of given persistent object,
        /// according to its id (matching the configured "unsaved-value"?).
        /// Will copy the contained fields to an already loaded instance
        /// with the same id, if appropriate.
        /// </summary>
        /// <param name="entity">The persistent object to save or update.
        /// (<i>not</i> necessarily to be associated with the Hibernate Session)
        /// </param>
        /// <returns>The actually associated persistent object.
        /// (either an already loaded instance with the same id, or the given object)</returns>
        /// <exception cref="DataAccessException">In case of Hibernate errors</exception>
        object SaveOrUpdateCopy(object entity);
        
        
        #endregion


    }
}
