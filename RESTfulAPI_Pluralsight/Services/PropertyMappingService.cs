using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI_Aync.Services
{
    public class PropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> authorPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>() { "Id"})},
                {"MainCategory", new PropertyMappingValue(new List<string>() { "MainCategory"})},
                {"DateOfBirth", new PropertyMappingValue(new List<string>() { "DateOfBirth"}, true)},
                {"FirstName", new PropertyMappingValue(new List<string>() { "FirstName"})},
                {"LastName", new PropertyMappingValue(new List<string>() { "LastName"})}
            };

        //public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        //{

        //}
    }
}
