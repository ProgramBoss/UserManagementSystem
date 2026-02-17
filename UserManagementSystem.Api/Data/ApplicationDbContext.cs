using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Api.Models;

namespace UserManagementSystem.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<GroupPermission> GroupPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserGroup>()
            .HasKey(ug => new { ug.UserId, ug.GroupId });

        modelBuilder.Entity<UserGroup>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserGroup>()
            .HasOne(ug => ug.Group)
            .WithMany(g => g.UserGroups)
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GroupPermission>()
            .HasKey(gp => new { gp.GroupId, gp.PermissionId });

        modelBuilder.Entity<GroupPermission>()
            .HasOne(gp => gp.Group)
            .WithMany(g => g.GroupPermissions)
            .HasForeignKey(gp => gp.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GroupPermission>()
            .HasOne(gp => gp.Permission)
            .WithMany(p => p.GroupPermissions)
            .HasForeignKey(gp => gp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Group>()
            .HasIndex(g => g.Name)
            .IsUnique();

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>().HasData(
            new Group { Id = 1, Name = "Admin", Description = "System administrators with full access", CreatedDate = DateTime.UtcNow },
            new Group { Id = 2, Name = "Level 1", Description = "Basic access level users", CreatedDate = DateTime.UtcNow },
            new Group { Id = 3, Name = "Level 2", Description = "Intermediate access level users", CreatedDate = DateTime.UtcNow },
            new Group { Id = 4, Name = "Manager", Description = "Team managers with elevated privileges", CreatedDate = DateTime.UtcNow }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "Create", Description = "Can create new records", CreatedDate = DateTime.UtcNow },
            new Permission { Id = 2, Name = "Read", Description = "Can view records", CreatedDate = DateTime.UtcNow },
            new Permission { Id = 3, Name = "Update", Description = "Can modify existing records", CreatedDate = DateTime.UtcNow },
            new Permission { Id = 4, Name = "Delete", Description = "Can delete records", CreatedDate = DateTime.UtcNow },
            new Permission { Id = 5, Name = "ManageUsers", Description = "Can manage user accounts", CreatedDate = DateTime.UtcNow },
            new Permission { Id = 6, Name = "ManageGroups", Description = "Can manage groups", CreatedDate = DateTime.UtcNow },
            new Permission { Id = 7, Name = "ViewReports", Description = "Can view reports", CreatedDate = DateTime.UtcNow },
            new Permission { Id = 8, Name = "ManageSystem", Description = "Can manage system settings", CreatedDate = DateTime.UtcNow }
        );

        modelBuilder.Entity<GroupPermission>().HasData(
            new GroupPermission { GroupId = 1, PermissionId = 1, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 1, PermissionId = 2, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 1, PermissionId = 3, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 1, PermissionId = 4, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 1, PermissionId = 5, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 1, PermissionId = 6, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 1, PermissionId = 7, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 1, PermissionId = 8, GrantedDate = DateTime.UtcNow }
        );

        modelBuilder.Entity<GroupPermission>().HasData(
            new GroupPermission { GroupId = 2, PermissionId = 2, GrantedDate = DateTime.UtcNow }
        );

        modelBuilder.Entity<GroupPermission>().HasData(
            new GroupPermission { GroupId = 3, PermissionId = 1, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 3, PermissionId = 2, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 3, PermissionId = 3, GrantedDate = DateTime.UtcNow }
        );

        modelBuilder.Entity<GroupPermission>().HasData(
            new GroupPermission { GroupId = 4, PermissionId = 1, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 4, PermissionId = 2, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 4, PermissionId = 3, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 4, PermissionId = 4, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = 4, PermissionId = 7, GrantedDate = DateTime.UtcNow }
        );
    }
}
