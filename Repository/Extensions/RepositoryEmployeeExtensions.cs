using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using Entities.Models;

namespace Repository.Extensions
{
    public static class RepositoryEmployeeExtensions
    {
        public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee>
            employees, uint minAge, uint maxAge) =>
            employees.Where(e => (e.Age >= minAge && e.Age <= maxAge));

        public static IQueryable<Employee> Search(this IQueryable<Employee> employees,
            string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return employees;

            var lowerCaseTerm = searchTerm.Trim().ToLower();

            return employees.Where(e => e.Name.ToLower().Contains(lowerCaseTerm));
        }

        public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string
            orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return employees.OrderBy(e => e.Name);

            var orderParams = orderByQueryString.Trim().Split(',');
            // Prepare the list of PropertyInfo objects that represent the properties of our Employee class.
            // We need them to be able to check if the field received through
            // the query string really exists in the Employee class
            var propertyInfos = typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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

            if (string.IsNullOrWhiteSpace(orderQuery))
                return employees.OrderBy(e => e.Name);

            // Order our query
            return employees.OrderBy(orderQuery);
        }
    }
}
