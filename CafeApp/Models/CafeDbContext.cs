using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CafeApp.Models;

public partial class CafeDbContext : DbContext
{
    public CafeDbContext() { }

    public CafeDbContext(DbContextOptions<CafeDbContext> options) : base(options) { }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientsAmount)
                .HasDefaultValue(1)
                .HasColumnName("clients_amount");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.ShiftId).HasColumnName("shift_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("money")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Shift).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("orders_shift_id_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("orders_table_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shifts_pkey");

            entity.ToTable("shifts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ShiftEnds)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("shift_ends");
            entity.Property(e => e.ShiftStarted)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("shift_started");

            entity.HasMany(d => d.Users).WithMany(p => p.Shifts)
                .UsingEntity<Dictionary<string, object>>(
                    "ShiftEmployee",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("shift_employees_user_id_fkey"),
                    l => l.HasOne<Shift>().WithMany()
                        .HasForeignKey("ShiftId")
                        .HasConstraintName("shift_employees_shift_id_fkey"),
                    j =>
                    {
                        j.HasKey("ShiftId", "UserId").HasName("shift_employees_pkey");
                        j.ToTable("shift_employees");
                        j.IndexerProperty<int>("ShiftId").HasColumnName("shift_id");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                    });
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Number).HasColumnName("number");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContractPhoto).HasColumnName("contract_photo");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserPhoto).HasColumnName("user_photo");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");

            entity.HasMany(d => d.Tables).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "WaiterTable",
                    r => r.HasOne<Table>().WithMany()
                        .HasForeignKey("TableId")
                        .HasConstraintName("waiter_tables_table_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("waiter_tables_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "TableId").HasName("waiter_tables_pkey");
                        j.ToTable("waiter_tables");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("TableId").HasColumnName("table_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
