using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Configuration;

namespace Reports.Core.Caching
{
    public interface IDataCache
    {
        T GetItem<T>(string cacheKey);
        T GetItem<T>(string cacheKey, DataCache.FillCacheMethod method);
        T GetItem<T>(string cacheKey, DataCache.FillCacheMethod method, Domain.Enumerations.DependencyType dependencyType, string fileOrKeyName);
        T GetItem<T>(string cacheKey, DataCache.FillCacheMethod method, Domain.Enumerations.DependencyType dependencyType, string fileOrKeyName, string[] dependencyKey);
        void Add(string cacheKey, object cacheObject);
        void Add(string cacheKey, object cacheObject, Int64 absoluteExpiresSeconds);
        void Add(string cacheKey, object cacheObject, Domain.Enumerations.DependencyType dependencyType, string fileOrKeyName);
        void Add(string cacheKey, object cacheObject, Domain.Enumerations.DependencyType dependencyType, string[] fileOrKeyName);
        void Add(string cacheKey, object cacheObject, Domain.Enumerations.DependencyType dependencyType, string fileOrKeyName, DateTime absoluteExpires, TimeSpan slidingExpires);
        bool Contains(string cacheKey);
        void RemoveAll();
        void Remove(string cacheKey);
    }
}
