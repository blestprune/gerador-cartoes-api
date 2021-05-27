using GeradorCartoesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GeradorCartoesAPI.Context
{
    public class GeradorCartoesContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cartao> Cartoes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=GeradorCartoesDb;Integrated Security=true");
        }
    }
}
