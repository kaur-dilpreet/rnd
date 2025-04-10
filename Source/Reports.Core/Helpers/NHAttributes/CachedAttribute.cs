using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.Core.Helpers.NHAttributes
{
    /// <summary>
    /// Specify the 2nd-level caching strategy for this class.
    /// See http://www.nhforge.org/doc/nh/en/index.html#performance-cache-mapping
    /// </summary>
    public enum CacheType {
        /// <summary>
        /// If the application needs to update data, a read-write cache might be appropriate.
        /// This cache strategy should never be used if serializable transaction isolation level is required.
        /// You should ensure that the transaction is completed when ISession.Close() or ISession.Disconnect() is called.
        /// If you wish to use this strategy in a cluster,
        /// you should ensure that the underlying cache implementation supports locking.
        /// The built-in cache providers do not. 
        /// </summary>
        read_write,
        /// <summary>
        /// If the application only occasionally needs to update data
        /// (ie. if it is extremely unlikely that two transactions would try to update the same item simultaneously)
        /// and strict transaction isolation is not required,
        /// a nonstrict-read-write cache might be appropriate.
        /// When using this strategy you should ensure that
        /// the transaction is completed when ISession.Close() or ISession.Disconnect() is called. 
        /// </summary>
        nonstrict_read_write,
        /// <summary>
        /// If your application needs to read but never modify instances of a persistent class,
        /// a read-only cache may be used.
        /// This is the simplest and best performing strategy.
        /// Its even perfectly safe for use in a cluster. 
        /// </summary>
        read_only
    };

    /// <summary>
    /// Specify to Fluent NHibernate automapping that a class should be cached
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class CachedAttribute : Attribute
    {
        /// <summary>
        /// Specify the 2nd-level caching strategy for this class.
        /// See 
        /// </summary>
        public CacheType cacheType { get; set; }

        /// <summary>
        /// An identifier specifying a grouping of objects in the cache, that can be flushed separately
        /// </summary>
        public string cacheRegion { get; set; }
    }
}
