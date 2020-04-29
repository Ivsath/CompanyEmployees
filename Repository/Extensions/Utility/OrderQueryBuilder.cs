using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Repository.Extensions.Utility
{
    public static class OrderQueryBuilder
    {
        public static string CreateOrderQuery<T>(string orderByQueryString)
        {
            var orderParams = orderByQueryString.Trim().Split(',');
            // Prepare the list of PropertyInfo objects that represent the properties of our Employee class.
            // We need them to be able to check if the field received through
            // the query string really exists in the Employee class
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                // Run through all the parameters and check for their existence
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi =>
                    pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null)
                    continue;

                // If we do find the property, we return it and additionally check if our parameter
                // contains “desc” at the end of the string.
                var direction = param.EndsWith(" desc") ? "descending" : "ascending";
                // We use the StringBuilder to build our query with each loop
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction}, ");
            }

            // Remove excess commas and do one last check to see if our query indeed has something in it
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

            return orderQuery;
        }
    }
}
