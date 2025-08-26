using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClienteHub.Domain.Entities;
using ClienteHub.Domain.Repositories;
using ClienteHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClienteHub.Infrastructure.Repositories;

public sealed class EfCustomerRepository(AppDbContext db) : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(string id, CancellationToken ct) =>
        db.Customers.Include(x => x.Address)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct) =>
        await db.Customers.Include(x => x.Address)
                          .AsNoTracking()
                          .OrderBy(x => x.Name)
                          .ToListAsync(ct);

    public Task<Customer?> GetByCnpjAsync(string cnpj, CancellationToken ct) =>
        db.Customers.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Cnpj == cnpj, ct);

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        if (customer.Address is not null)
            await db.Addresses.AddAsync(customer.Address, ct);

        await db.Customers.AddAsync(customer, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken ct)
    {
        if (customer.Address is not null)
            db.Addresses.Update(customer.Address);

        db.Customers.Update(customer);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken ct)
    {
        db.Customers.Remove(customer);
        await db.SaveChangesAsync(ct);
    }
}

