using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Models;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions<DataContext> options) : base(options) {}

	public DbSet<Propietario> Propietarios { get; set; } = null!;
}