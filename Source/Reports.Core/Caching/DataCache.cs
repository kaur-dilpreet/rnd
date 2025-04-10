using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Configuration;

namespace Reports.Core.Caching
{
    /// <summary>
    /// This class will return cached objects to the requesting business layer.  If the object does not exist in cache it will retrieve/return it from the db layer that know how to create it and cache the item.
    /// </summary>
    public sealed class DataCache : IDataCache
    {

        #region Events & Delegates

        /// <summary>
        /// Used by GetItem
        /// </summary>
        /// <returns></returns>
        public delegate object FillCacheMethod();

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DataCache() { }
        
        #endregion

        #region Static Public Methods

        /// <summary>Time, in seconds, at which the inserted object expires and is removed from the cache.</summary>
        private Double AbsoluteExpirationDuration
        {            
            get 
            {
                double expireTime = double.Parse(ConfigurationManager.AppSettings["AbsoluteExpirationDuration"]);

                if (expireTime <= 0)
                    expireTime = 3600;

                return expireTime; 
            }
        }

        /// <summary>
        /// returns an item from cache by key
        /// </summary>
        /// <typeparam name="T">defines the type of the stored object</typeparam>
        /// <param name="cacheKey">cache key of the stored type</param>
        /// <returns>the cached type</returns>
        public T GetItem<T>(string cacheKey)
        {
            return (T)HttpRuntime.Cache[cacheKey];
            //return (T)HttpContext.Current.Cache[cacheKey];
        }

        /// <summary>
        /// Gets item from cache or executes method to get item then adds to cache with no dependencies
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="cacheKey">cachekey of the item to get</param>
        /// <param name="method">method to execute to create object if doen't exist</param>
        /// <returns>item requested</returns>
        public T GetItem<T>(string cacheKey, FillCacheMethod method)
        {
            return GetItem<T>(cacheKey, method, Domain.Enumerations.DependencyType.None, null);
        }

        /// <summary>
        /// Gets item from cache or executes method to get item then adds to cache
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="cacheKey">cachekey of the item to get</param>
        /// <param name="method">method to execute to create object if doen't exist</param>
        /// <param name="dependencyType">type of dependency, file cache or none</param>
        /// <param name="fileOrKeyName">filename of the cache dependency</param>
        /// <returns>item requested</returns>
        public T GetItem<T>(string cacheKey, FillCacheMethod method, Domain.Enumerations.DependencyType dependencyType, string fileOrKeyName)
        {
            T item;

            //get item from cache
            //item = (T)HttpContext.Current.Cache[cacheKey];
            item = (T)HttpRuntime.Cache[cacheKey];
            // item not found
            if (item == null)
            {
                // call the method to create the item
                item = (T)method();

                // does item now exist
                if (item != null)
                {
                    if (dependencyType == Domain.Enumerations.DependencyType.None)
                    {
                        // item created, now add to cache
                        Add(cacheKey, item);
                    }
                    else if (dependencyType == Domain.Enumerations.DependencyType.File || dependencyType == Domain.Enumerations.DependencyType.CacheKey)
                    {
                        // add to cache with file or cacheKey dependency
                        Add(cacheKey, item, dependencyType, fileOrKeyName);
                    }
                }
            }

            return item;
        }

        // over load to cache with multiple file or cache dependencies, pass in null for fileOrKeyName
        public T GetItem<T>(string cacheKey, FillCacheMethod method, Domain.Enumerations.DependencyType dependencyType, string fileOrKeyName, string[] dependencyKey)
        {
            T item;

            //get item from cache
            //item = (T)HttpContext.Current.Cache[cacheKey];
            item = (T)HttpRuntime.Cache[cacheKey];
            // item not found
            if (item == null)
            {
                // call the method to create the item
                item = (T)method();

                // does item now exist
                if (item != null)
                {
                    if (dependencyType == Domain.Enumerations.DependencyType.None)
                    {
                        // item created, now add to cache
                        Add(cacheKey, item);
                    }
                    else if (dependencyType == Domain.Enumerations.DependencyType.File || dependencyType == Domain.Enumerations.DependencyType.CacheKey)
                    {
                        // add to cache with file or cacheKey dependency
                        Add(cacheKey, item, dependencyType, dependencyKey);
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// adds an item to the cache
        /// </summary>
        /// <param name="cacheKey">cache key of the item</param>
        /// <param name="cacheObject">the object to cache</param>
        public void Add(string cacheKey, object cacheObject)
        {
            HttpRuntime.Cache.Insert(cacheKey, cacheObject);
        }

        public void Add(string cacheKey, object cacheObject, Int64 absoluteExpiresSeconds)
        {
            // call overloaded add method
            HttpRuntime.Cache.Insert(cacheKey, cacheObject, null, DateTime.UtcNow.AddSeconds(absoluteExpiresSeconds), TimeSpan.Zero);
        }

        /// <summary>
        /// Add item to cache.  Overloaded method requiring key, object, and dependencies
        /// </summary>
        /// <param name="cacheKey">cache key of the item</param>
        /// <param name="cacheObject">the object to cache</param>
        public void Add(string cacheKey, object cacheObject, Domain.Enumerations.DependencyType dependencyType, string fileOrKeyName)
        {
            // call overloaded add method
            Add(cacheKey, cacheObject, dependencyType, fileOrKeyName, DateTime.UtcNow.AddSeconds(this.AbsoluteExpirationDuration), TimeSpan.Zero);
        }

        public void Add(string cacheKey, object cacheObject, Domain.Enumerations.DependencyType dependencyType, string[] fileOrKeyName)
        {            
            // insert cache object with file dependency
            HttpRuntime.Cache.Insert(cacheKey, cacheObject, new CacheDependency(fileOrKeyName), DateTime.UtcNow.AddSeconds(this.AbsoluteExpirationDuration), TimeSpan.Zero);
        }
        
        /// <summary>
        /// Adds Item to cache
        /// NOTE: Only the absolute expiration OR the sliding expiration can be set.
        ///     To use the absolute expiration, set the sliding expiration to TimeSpan.Zero
        ///     To use the sliding expiration, set the absolute expiration to DateTime.Max
        /// </summary>
        /// <param name="cacheKey">cache key of the item</param>
        /// <param name="cacheObject">object to cache</param>
        /// <param name="dependencyType">type of dependency, file cache or none</param>
        /// <param name="fileOrKeyName">filename of the cache dependency</param>
        /// <param name="absoluteExpires">Specific time when cached item expires</param>
        /// <param name="slidingExpires">Specified sliding period of time until cached item expires</param>
        public void Add(string cacheKey, object cacheObject, Domain.Enumerations.DependencyType dependencyType, string fileOrKeyName, DateTime absoluteExpires, TimeSpan slidingExpires)
        {
            if (dependencyType == Domain.Enumerations.DependencyType.None)
            {
                // just insert cache object
                HttpRuntime.Cache.Insert(cacheKey,cacheObject,null,absoluteExpires,slidingExpires);
            }
            else if (dependencyType == Domain.Enumerations.DependencyType.File)
            {
                // insert cache object with file dependency
                HttpRuntime.Cache.Insert(cacheKey, cacheObject, new CacheDependency(fileOrKeyName), absoluteExpires, slidingExpires);
            }
            else if (dependencyType == Domain.Enumerations.DependencyType.CacheKey)
            {
                if (HttpRuntime.Cache[fileOrKeyName] == null)
                    HttpRuntime.Cache[fileOrKeyName] = DateTime.UtcNow;
                // insert cache object with file dependency
                HttpRuntime.Cache.Insert(cacheKey, cacheObject, new CacheDependency(null, new String[] { fileOrKeyName }), absoluteExpires, slidingExpires);
            }
        }

        /// <summary>
        /// checks to see if an item is in cache for a given key
        /// </summary>
        /// <param name="cacheKey">cache key of the item</param>
        /// <returns>bool - if item is in cache</returns>
        public bool Contains(string cacheKey)
        {
            return (HttpRuntime.Cache[cacheKey] != null);
        }

        /// <summary>
        /// Remove all items from the Cache 
        /// </summary>    
        public void RemoveAll()
        {            
            List<string> keyList = new List<string>();            
            IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();

            // Note - should not remove from cache in while loop since enumerator is only valid while collection remains intact/unchanged
            while (cacheEnum.MoveNext())
            {
                keyList.Add(cacheEnum.Key.ToString());
            }
            foreach (string key in keyList)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }

        /// <summary>
        /// remove an item from the cache by key
        /// </summary>
        /// <param name="cacheKey">cache key of the item</param>
        public void Remove(string cacheKey)
        {
           
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                HttpRuntime.Cache.Remove(cacheKey);
            }
        }

        #endregion
    }
}

