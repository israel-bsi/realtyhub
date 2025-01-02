using Microsoft.AspNetCore.Identity;

namespace RealtyHub.ApiService.Models;

public class User : IdentityUser<long>
{
    public string Creci { get; set; } = string.Empty;
    public List<IdentityRole<long>>? Roles { get; set; }
}