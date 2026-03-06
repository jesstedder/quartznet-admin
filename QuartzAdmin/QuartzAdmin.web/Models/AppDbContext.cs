using Microsoft.EntityFrameworkCore;

namespace QuartzAdmin.web.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<InstanceModel> Instances { get; set; } = null!;
    public DbSet<InstancePropertyModel> InstanceProperties { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InstanceModel>().ToTable("tbl_instances");
        modelBuilder.Entity<InstancePropertyModel>().ToTable("tbl_instanceproperties");

        modelBuilder.Entity<InstancePropertyModel>()
            .HasOne(p => p.ParentInstance)
            .WithMany(i => i.InstanceProperties)
            .HasForeignKey("InstanceID")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
