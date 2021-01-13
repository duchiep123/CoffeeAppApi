using Coffee_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.Cache
{
    public static class JWTTokenCache
    {
        private static Dictionary<string, DateTime> cache;
        private static object cacheLock = new object();
        public static Dictionary<string, DateTime> AppCache
        {
            get
            {
                lock (cacheLock)
                {
                    if (cache == null)
                    {
                        cache = new Dictionary<string, DateTime>();
                    }
                    return cache;
                }
            }
        }
    }
}
