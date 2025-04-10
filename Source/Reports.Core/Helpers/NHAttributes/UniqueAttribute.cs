using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.Core.Helpers.NHAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : Attribute
    {
        public string Key { get; set; }

        public UniqueAttribute()
        {
        }

        public UniqueAttribute(string key)
        {
            this.Key = key;
        }
    }
}
