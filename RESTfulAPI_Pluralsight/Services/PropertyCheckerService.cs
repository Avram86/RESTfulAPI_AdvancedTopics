using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RESTfulAPI_Aync.Services
{
    public class PropertyCheckerService : IPropertyCheckerService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            //fields are spearated by ",", so we split it
            var filedsAfterSplit = fields.Split(",");

            //check if requested fields exist on source
            foreach (var field in filedsAfterSplit)
            {
                var propertyName = field.Trim();

                //use reflection to see if property can be found on T
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                //if it can't, return false
                if (propertyInfo == null)
                {
                    return false;
                }
            }

            //all checks out, return true
            return true;
        }
    }
}
