using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteHub.Application.DTOs;

public sealed record CustomerCreateDto(
    string Name,
    string Email,
    string Cnpj,
    string Cep,
    string Street,
    string Neighborhood,
    string City,
    string State,
    string? Number,
    string? Complement
);

public sealed record CustomerViewDto(
    string Id,
    string Name,
    string Email,
    string Cnpj,
    string Cep,
    string Street,
    string Neighborhood,
    string City,
    string State,
    string Number,
    string Complement
);
