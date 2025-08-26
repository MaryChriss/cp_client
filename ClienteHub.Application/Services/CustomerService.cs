using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClienteHub.Application.DTOs;
using ClienteHub.Application.Services;
using ClienteHub.Domain.Entities;
using ClienteHub.Domain.Repositories;

namespace ClienteHub.Application;

public sealed class CustomerService(ICustomerRepository repo) : ICustomerService
{
    public async Task<string> CreateAsync(CustomerCreateDto dto, CancellationToken ct)
    {
        var normalizedCnpj = new string(dto.Cnpj.Where(char.IsDigit).ToArray());
        var exists = await repo.GetByCnpjAsync(normalizedCnpj, ct);
        if (exists is not null) throw new InvalidOperationException("Cliente com este CNPJ já existe.");

        var address = new Address(dto.Cep, dto.Street, dto.Neighborhood, dto.City, dto.State, dto.Number, dto.Complement);
        var customer = new Customer(dto.Name, dto.Email, dto.Cnpj);
        customer.AttachAddress(address);

        await repo.AddAsync(customer, ct);
        return customer.Id;
    }

    public async Task<CustomerViewDto?> GetAsync(string id, CancellationToken ct)
    {
        var c = await repo.GetByIdAsync(id, ct);
        return c is null ? null : Map(c);
    }

    public async Task<IReadOnlyList<CustomerViewDto>> GetAllAsync(CancellationToken ct)
        => (await repo.ListAsync(ct)).Select(Map).ToList();

    public async Task UpdateAsync(string id, CustomerCreateDto dto, CancellationToken ct)
    {
        var c = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Cliente não encontrado");

        c.SetName(dto.Name);
        c.SetEmail(dto.Email);
        c.SetCnpj(dto.Cnpj);

        var addr = new Address(dto.Cep, dto.Street, dto.Neighborhood, dto.City, dto.State, dto.Number, dto.Complement);
        c.AttachAddress(addr);

        await repo.UpdateAsync(c, ct);
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        var c = await repo.GetByIdAsync(id, ct);
        if (c is null) return;
        await repo.DeleteAsync(c, ct);
    }

    private static CustomerViewDto Map(Customer c) => new(
        c.Id, c.Name, c.Email, c.Cnpj,
        c.Address?.Cep ?? "",
        c.Address?.Street ?? "",
        c.Address?.Neighborhood ?? "",
        c.Address?.City ?? "",
        c.Address?.State ?? "",
        c.Address?.Number ?? "",
        c.Address?.Complement ?? ""
    );
}
