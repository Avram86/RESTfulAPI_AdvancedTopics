using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RESTfulAPI_Aync.Helpers
{
    public static class IEnumerableExtensions
    {

        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            //create a list to hold our ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            //create a list with PropertyInfo objects on TSource.
            //Reflection is expensive, so rather than doing it for each object in the list, we do it once
            //and reuse the results. 
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                //all public porperties should be in the ExpandoObject
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(",");
                foreach(var field in fieldsAfterSplit)
                {
                    var propertyName= field.Trim();

                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} was not found on {typeof(TSource)}");
                    }
                    //add propertyInfo to list
                    propertyInfoList.Add(propertyInfo);
                }
            }

            //run through all the source objects
            foreach(TSource sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();

                foreach(var propertyInfo in propertyInfoList)
                {
                    //GetValue returns the value of the property on the source object 
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    //add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                //add the ExpandoObject to the list
                expandoObjectList.Add(dataShapedObject);
            }

            //return the list
            return expandoObjectList;
        }
    }
}
