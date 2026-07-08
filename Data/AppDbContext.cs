using Microsoft.EntityFrameworkCore;
using LMS.Models;
using LMS.Controllers;

namespace LMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
     
        }
    }

