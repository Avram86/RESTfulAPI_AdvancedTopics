using CourseLibrary.API.Entities;
using RESTfulAPI_Pluralsight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI_Aync.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>() { "Id"})},
                {"MainCategory", new PropertyMappingValue(new List<string>() { "MainCategory"})},
                {"DateOfBirth", new PropertyMappingValue(new List<string>() { "DateOfBirth"}, true)},
                {"FirstName", new PropertyMappingValue(new List<string>() { "FirstName"})},
                {"LastName", new PropertyMappingValue(new List<string>() { "LastName"})}
            };


        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();
        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var filedAfterSplit = fields.Split(",");
            foreach(var field in filedAfterSplit)
            {
                var trimmedFiled = field.Trim();

                var indexOfFirstSpace = trimmedFiled.IndexOf(" ");
                var propertyName = (indexOfFirstSpace == -1) ? trimmedFiled : trimmedFiled.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            //get matching mapping
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }
            throw new Exception($"Cannot find exact property mapping instance for<{typeof(TSource)}, {typeof(TDestination)}>");
        }
    }
}
