using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HeadHunter.Models;

public class HeadHunterDb : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<Module> Modules { get; set; }
    
    public HeadHunterDb(DbContextOptions<HeadHunterDb> options) : base(options) { }

}