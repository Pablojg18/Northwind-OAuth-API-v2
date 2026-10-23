using APIWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Data;

/// <summary>
/// Contexto exclusivo de Identity (usuarios, roles, tokens).
/// Se mantiene separado de Northwind para poder crear las tablas AspNet*
/// sin tocar el esquema de Northwind ya existente.
/// </summary>
public class AppIdentityDbContext : IdentityDbContext<ApplicantUser>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
    {
    }
}
