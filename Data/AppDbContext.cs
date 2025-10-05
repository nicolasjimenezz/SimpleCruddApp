using Microsoft.EntityFrameworkCore;
using SimpleCrudApp.Models;

namespace SimpleCrudApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
