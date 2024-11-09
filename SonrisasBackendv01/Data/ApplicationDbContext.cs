using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Data
{
	public class ApplicationDbContext : IdentityDbContext<AppUsuario>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Configuración de la relación entre Cita y Odontologo con eliminación en cascada
			builder.Entity<Cita>()
				.HasOne(c => c.Odontologo)
				.WithMany(o => o.Citas)
				.HasForeignKey(c => c.OdontologoId)
				.OnDelete(DeleteBehavior.Cascade);  // Mantener la eliminación en cascada para esta relación

			// Configuración de la relación entre Cita y Paciente sin eliminación en cascada
			builder.Entity<Cita>()
				.HasOne(c => c.Paciente)
				.WithMany(p => p.Citas)
				.HasForeignKey(c => c.PacienteId)
				.OnDelete(DeleteBehavior.Restrict);  // Evitar la eliminación en cascada para evitar ciclos

			// Configuración de la relación entre Historial y Paciente
			builder.Entity<Historial>()
				.HasOne(h => h.Paciente)
				.WithMany(p => p.Historiales)
				.HasForeignKey(h => h.PacienteId)
				.OnDelete(DeleteBehavior.Restrict);  // Evitar cascada

			// Configuración de la relación entre Historial y Odontologo
			builder.Entity<Historial>()
				.HasOne(h => h.Odontologo)
				.WithMany(o => o.Historiales)
				.HasForeignKey(h => h.OdontologoId)
				.OnDelete(DeleteBehavior.Restrict);  // Evitar cascada
		}

		public DbSet<Paciente> Pacientes { get; set; }
		public DbSet<Odontologo> Odontologos { get; set; }
		public DbSet<Cita> Citas { get; set; }
		public DbSet<Consultorio> Consultorios { get; set; }
		public DbSet<Historial> Historiales { get; set; }
		public DbSet<AppUsuario> AppUsuario { get; set; }
		public DbSet<Radiografia> Radiografias { get; set; }
	}
}
