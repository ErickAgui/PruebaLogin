
using Microsoft.EntityFrameworkCore;
using PruebaLogin.Models;

namespace PruebaLogin.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
            
        }
        
        public DbSet<Login> logins {get; set;}
    }
}