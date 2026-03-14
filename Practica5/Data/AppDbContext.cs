using Microsoft.EntityFrameworkCore;
using Practica5.Models;


namespace Practica5.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enforce unique Correo at DB level too (important!)
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        // Relación Producto -> Categoria (Muchos a Uno)
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.IdCategoria)
            .OnDelete(DeleteBehavior.Restrict); // Evitar borrado en cascada

        // Relación Producto -> Proveedor (Muchos a Uno)
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Proveedor)
            .WithMany(prov => prov.Productos)
            .HasForeignKey(p => p.IdProveedor)
            .OnDelete(DeleteBehavior.Restrict); // Evitar borrado en cascada

        // Índice único para categoría (opcional, para evitar duplicados)
        modelBuilder.Entity<Categoria>()
            .HasIndex(c => c.Nombre)
            .IsUnique();

        // Índice único para proveedor (opcional)
        modelBuilder.Entity<Proveedor>()
            .HasIndex(p => p.Nombre)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}