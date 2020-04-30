using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;

namespace CompanyEmployees.Utility
{
    public class EmployeeLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeDto> _dataShaper;

        public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto,
            string fields,
            Guid companyId,
            HttpContext httpContext)
        {
            var shapedEmployees = ShapeData(employeesDto, fields);

            // If the httpContext contains the required media type, we add links to the response
            if (ShouldGenerateLinks(httpContext))
                return ReturnLinkdedEmployees(employeesDto, fields, companyId, httpContext, shapedEmployees);

            return ReturnShapedEmployees(shapedEmployees);
        }

        // Executes data shaping and extracts only the entity part without the Id property
        private List<Entity> ShapeData(IEnumerable<EmployeeDto> employeesDto, string fields) =>
                _dataShaper.ShapeData(employeesDto, fields)
                    .Select(e => e.Entity)
                    .ToList();

        // We extract the media type from the httpContext.
        // If that media type ends with hateoas, the method returns true
        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }

        // Returns a new LinkResponse with the ShapedEntities property populated.
        // By default, the HasLinks property is false
        private LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees) =>
            new LinkResponse { ShapedEntities = shapedEmployees };

        private LinkResponse ReturnLinkdedEmployees(IEnumerable<EmployeeDto> employeesDto,
            string fields, Guid companyId, HttpContext httpContext, List<Entity> shapedEmployees)
        {
            var employeeDtoList = employeesDto.ToList();

            for (var index = 0; index < employeeDtoList.Count(); index++)
            {
                // We create links for each employee
                var employeeLinks = CreateLinksForEmployee(httpContext, companyId, employeeDtoList[index].Id, fields);
                shapedEmployees[index].Add("Links", employeeLinks);
            }

            // We wrap the collection and create links that are important for the entire collection
            var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);
            var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);

            return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
        }

        private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId,
            Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(
                    _linkGenerator.GetUriByAction(httpContext, "GetEmployeeForCompany", "Employees",
                        values: new {companyId, id, fields}), "self", "GET"),
                new Link(
                    _linkGenerator.GetUriByAction(httpContext, "DeleteEmployeeForCompany", "Employees",
                        values: new {companyId, id}), "delete_employee", "DELETE"),
                new Link(
                    _linkGenerator.GetUriByAction(httpContext, "UpdateEmployeeForCompany", "Employees",
                        values: new {companyId, id}), "update_employee", "PUT"),
                new Link(
                    _linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateEmployeeForCompany", "Employees",
                        values: new {companyId, id}), "partially_update_employee", "PATCH")
            };

            return links;
        }
        private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext,
            LinkCollectionWrapper<Entity> employeesWrapper)
        {
            employeesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext,
                    "GetEmployeesForCompany", "Employees", values: new { }), "self", "GET"));

            return employeesWrapper;
        }
    }
}
