using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Employee>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
