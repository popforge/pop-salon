using Popsalon.Domain.Entities;

namespace Popsalon.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer, Guid>
{
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default);
}
