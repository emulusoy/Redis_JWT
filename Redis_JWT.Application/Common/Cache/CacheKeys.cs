using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis_JWT.Application.Common.Cache
{
    public static class CacheKeys
    {
        public const string ProductsAll = "products:all";
        public static string ProductItem(int id) => $"products:{id}";

    }
}
