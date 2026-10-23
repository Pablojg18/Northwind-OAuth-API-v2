using Microsoft.AspNetCore.Identity;

namespace APIWeb.Models;

public class ApplicantUser : IdentityUser
{
    public string? Name { get; set; }

    public string? Picture { get; set; }
}
