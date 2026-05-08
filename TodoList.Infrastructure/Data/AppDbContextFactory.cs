using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure;

/// <summary>
/// Factory for creating the DbContext during design time (migrations).
/// This allows EF tools to work even if environment variables are missing.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // OPCIÓN 1: Usar una cadena hardcodeada solo para generar el código de la migración
        // No importa si la base de datos no existe en este paso, EF solo quiere leer tus clases.
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=MigrationDb;Trusted_Connection=True;";

        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}