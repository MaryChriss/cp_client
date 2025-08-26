using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// --- Resilience policies (retry + timeout + circuit breaker)
static IAsyncPolicy<HttpResponseMessage> RetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => (int)msg.StatusCode == 429) // rate limit
        .WaitAndRetryAsync(3, retry => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retry)));

static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy() =>
    Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));

static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

// --- HttpClient apontando para BrasilAPI
builder.Services.AddHttpClient("brasilapi", client =>
{
    client.BaseAddress = new Uri("https://brasilapi.com.br/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("ClienteHub-LookupProxy/1.0");
})
.AddPolicyHandler(RetryPolicy())
.AddPolicyHandler(TimeoutPolicy())
.AddPolicyHandler(CircuitBreakerPolicy());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/cep/{cep}", async (string cep, IHttpClientFactory factory, CancellationToken ct) =>
{
    var digits = new string((cep ?? "").Where(char.IsDigit).ToArray());
    if (digits.Length != 8)
        return Results.BadRequest(new { error = "CEP inválido. Use 8 dígitos." });

    var http = factory.CreateClient("brasilapi");
    var resp = await http.GetAsync($"/api/cep/v1/{digits}", ct);
    if (!resp.IsSuccessStatusCode)
        return Results.Problem($"BrasilAPI CEP error: {(int)resp.StatusCode} {resp.StatusCode}");

    var json = await resp.Content.ReadAsStringAsync(ct);
    return Results.Content(json, "application/json");
})
.WithName("LookupCep")
.WithOpenApi();

app.MapGet("/cnpj/{cnpj}", async (string cnpj, IHttpClientFactory factory, CancellationToken ct) =>
{
    var digits = new string((cnpj ?? "").Where(char.IsDigit).ToArray());
    if (digits.Length != 14)
        return Results.BadRequest(new { error = "CNPJ inválido. Use 14 dígitos." });

    var http = factory.CreateClient("brasilapi");
    var resp = await http.GetAsync($"/api/cnpj/v1/{digits}", ct);
    if (!resp.IsSuccessStatusCode)
        return Results.Problem($"BrasilAPI CNPJ error: {(int)resp.StatusCode} {resp.StatusCode}");

    var json = await resp.Content.ReadAsStringAsync(ct);
    return Results.Content(json, "application/json");
})
.WithName("LookupCnpj")
.WithOpenApi();

app.Run();
