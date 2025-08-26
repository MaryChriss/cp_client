using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Client.Web.Controllers;

public sealed class CustomersController(IHttpClientFactory http) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var api = http.CreateClient("customer");
        var list = await api.GetFromJsonAsync<List<CustomerViewModel>>("api/customers", ct) ?? new();
        return View(list);
    }

    [HttpGet]
    public IActionResult Create() => View(new CustomerCreateModel());

    [HttpPost]
    public async Task<IActionResult> Create(CustomerCreateModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var api = http.CreateClient("customer");
        var resp = await api.PostAsJsonAsync("api/customers", model, ct);

        if (!resp.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", $"Erro ao salvar: {resp.StatusCode}");
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Cep(string cep, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return BadRequest(new { error = "Informe o CEP" });

        var lookup = http.CreateClient("lookup");
        var resp = await lookup.GetAsync($"cep/{cep}", ct);

        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode, new { error = "Falha ao consultar CEP" });

        var json = await resp.Content.ReadAsStringAsync(ct);
        return Content(json, "application/json");
    }
}

public sealed record CustomerViewModel(
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

public sealed class CustomerCreateModel
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Cep { get; set; } = "";
    public string Street { get; set; } = "";
    public string Neighborhood { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string? Number { get; set; }
    public string? Complement { get; set; }
}
