using Microsoft.EntityFrameworkCore;
using LMS.Models;
using IcampusBoatBackend.Controllers;
namespace LMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
     
        }
    }

