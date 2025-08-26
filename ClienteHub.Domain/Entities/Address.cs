using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteHub.Domain.Entities;

public sealed class Address
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Cep { get; private set; } = default!;
    public string Street { get; private set; } = default!;
    public string Neighborhood { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string State { get; private set; } = default!;
    public string Number { get; private set; } = "";
    public string Complement { get; private set; } = "";

    private Address() { }

    public Address(string cep, string street, string neighborhood, string city, string state, string? number = null, string? complement = null)
    {
        Cep = new string((cep ?? "").Where(char.IsDigit).ToArray());
        if (Cep.Length != 8) throw new ArgumentException("CEP deve ter 8 dígitos");

        Street = Required(street);
        Neighborhood = Required(neighborhood);
        City = Required(city);
        State = Required(state);
        Number = number?.Trim() ?? "";
        Complement = complement?.Trim() ?? "";
    }

    private static string Required(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Campo obrigatório");
        return value.Trim();
    }
}
