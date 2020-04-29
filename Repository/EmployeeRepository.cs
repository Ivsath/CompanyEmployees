using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EmployeeRepository: RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        // Say we need to get the results for the third page of our website, counting 20 as
        // the number of results we want. That would mean we want to skip the first
        // ((3 – 1) * 20) = 40 results, then take the next 20 and return them to the caller.
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                .OrderBy(e => e.Name)
                .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
                .Take(employeeParameters.PageSize)
                .ToListAsync();

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)  =>
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
                    trackChanges)
                .SingleOrDefaultAsync();

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee) => Delete(employee);
    }
}
