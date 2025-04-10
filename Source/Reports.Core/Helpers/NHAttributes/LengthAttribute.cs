using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.Core.Helpers.NHAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LengthAttribute : Attribute
    {
        public int Length { get; set; }

        public LengthAttribute(int length)
        {
            this.Length = length;
        }
    }
}
