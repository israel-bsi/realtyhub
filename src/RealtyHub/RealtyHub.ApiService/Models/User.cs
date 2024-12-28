using Microsoft.AspNetCore.Identity;

namespace RealtyHub.ApiService.Models;

public class User : IdentityUser<long>
{
    public List<IdentityRole<long>>? Roles { get; set; }
}