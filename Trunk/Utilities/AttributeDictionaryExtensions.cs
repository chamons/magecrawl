﻿using System.Collections.Generic;

namespace Magecrawl.Utilities
{
    public static class AttributeDictionaryExtensions
    {
        public static int GetNumbericIfAny(this Dictionary<string,string> d, string attribute)
        {
            if (!d.ContainsKey(attribute))
                return 0;
            return int.Parse(d[attribute]);
        }
    }
}
