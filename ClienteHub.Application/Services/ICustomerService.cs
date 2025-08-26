using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClienteHub.Application.DTOs;

namespace ClienteHub.Application.Services;

public interface ICustomerService
{
    Task<string> CreateAsync(CustomerCreateDto dto, CancellationToken ct);
    Task<CustomerViewDto?> GetAsync(string id, CancellationToken ct);
    Task<IReadOnlyList<CustomerViewDto>> GetAllAsync(CancellationToken ct);
    Task UpdateAsync(string id, CustomerCreateDto dto, CancellationToken ct);
    Task DeleteAsync(string id, CancellationToken ct);
}
