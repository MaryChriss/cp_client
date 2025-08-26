using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClienteHub.Domain.Entities;

public sealed class Customer
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Cnpj { get; private set; } = default!;

    public string? AddressId { get; private set; }
    public Address? Address { get; private set; }

    private Customer() { }

    public Customer(string name, string email, string cnpj)
    {
        SetName(name);
        SetEmail(email);
        SetCnpj(cnpj);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        Name = name.Trim();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required");
        Email = email.Trim();
    }

    public void SetCnpj(string cnpj)
    {
        var digits = new string(cnpj.Where(char.IsDigit).ToArray());
        if (digits.Length != 14) throw new ArgumentException("CNPJ deve ter 14 dígitos");
        Cnpj = digits;
    }

    public void AttachAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        AddressId = address.Id;
    }
}

