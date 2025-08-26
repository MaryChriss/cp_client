using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClienteHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClienteHub.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        var c = modelBuilder.Entity<Customer>();
        c.ToTable("TB_CUSTOMERS");
        c.HasKey(x => x.Id);
        c.Property(x => x.Id).HasMaxLength(36).IsRequired();
        c.Property(x => x.Name).HasMaxLength(200).IsRequired();
        c.Property(x => x.Email).HasMaxLength(200).IsRequired();
        c.Property(x => x.Cnpj).HasMaxLength(14).IsRequired();
        c.HasIndex(x => x.Cnpj).IsUnique();

        c.Property(x => x.AddressId).HasMaxLength(36);
        c.HasOne(x => x.Address)
         .WithOne()
         .HasForeignKey<Customer>(x => x.AddressId)
         .OnDelete(DeleteBehavior.Cascade);

        var a = modelBuilder.Entity<Address>();
        a.ToTable("TB_ADDRESSES");
        a.HasKey(x => x.Id);
        a.Property(x => x.Id).HasMaxLength(36).IsRequired();
        a.Property(x => x.Cep).HasMaxLength(8).IsRequired();
        a.Property(x => x.Street).HasMaxLength(200).IsRequired();
        a.Property(x => x.Neighborhood).HasMaxLength(120).IsRequired();
        a.Property(x => x.City).HasMaxLength(120).IsRequired();
        a.Property(x => x.State).HasMaxLength(2).IsRequired();
        a.Property(x => x.Number).HasMaxLength(20);
        a.Property(x => x.Complement).HasMaxLength(200);
    }
}

