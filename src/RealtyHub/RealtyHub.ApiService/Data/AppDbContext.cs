using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Models;
using System.Reflection;

namespace RealtyHub.ApiService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) :
    IdentityDbContext
    <
        User,
        IdentityRole<long>,
        long,
        IdentityUserClaim<long>,
        IdentityUserRole<long>,
        IdentityUserLogin<long>,
        IdentityRoleClaim<long>,
        IdentityUserToken<long>
    >(options)
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Property> Properties { get; set; } = null!;
    public DbSet<Viewing> Viewing { get; set; } = null!;
    public DbSet<Offer> Offers { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Contract> Contracts { get; set; } = null!;
    public DbSet<PropertyPhoto> PropertyPhotos { get; set; } = null!;
    public DbSet<ContractTemplate> ContractTemplates { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}