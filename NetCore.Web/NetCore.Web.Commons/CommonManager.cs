using NetCore.Framework.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Web.Commons
{
    public class CommonManager
    {
        private static readonly object lockobj = new object();
        private static volatile CacheManager _cache = null;
        /// <summary>
        /// Cache
        /// </summary>
        public static CacheManager CacheObj
        {
            get
            {
                if (_cache == null)
                {
                    lock (lockobj)
                    {
                        if (_cache == null)
                            _cache = new CacheManager();
                    }
                }
                return _cache;
            }
        }
    }
}
