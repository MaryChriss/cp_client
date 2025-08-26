using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Microsoft.EntityFrameworkCore.Design;

namespace ClienteHub.Infrastructure.Data;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();

        var conn = Environment.GetEnvironmentVariable("ORACLE_CS");

        if (string.IsNullOrWhiteSpace(conn))
        {
            var cfg = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            conn = cfg.GetConnectionString("Oracle");
        }

        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException("Connection string Oracle não encontrada (ORACLE_CS ou appsettings.json).");

        builder.UseOracle(conn);
        return new AppDbContext(builder.Options);
    }
}
