using GeradorCartoesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GeradorCartoesAPI.Context
{
    public class GeradorCartoesContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cartao> Cartoes { get; set; }

        public GeradorCartoesContext(DbContextOptions<GeradorCartoesContext> options) : base(options)
        {

        }
    }
}
