using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClienteHub.Domain.Entities;

namespace ClienteHub.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(string id, CancellationToken ct);
    Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct);
    Task<Customer?> GetByCnpjAsync(string cnpj, CancellationToken ct);
    Task AddAsync(Customer customer, CancellationToken ct);
    Task UpdateAsync(Customer customer, CancellationToken ct);
    Task DeleteAsync(Customer customer, CancellationToken ct);
}

