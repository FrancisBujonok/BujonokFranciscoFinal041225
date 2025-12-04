using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Tp3_Programacion.Models;

namespace Tp3_Programacion.Data
{
    public class AplicacionDbContext : DbContext
    {
        public DbSet<Cancha> Canchas { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=FinalProgramacion;Trusted_Connection=True;TrustServerCertificate=True"
            );
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Socio>()
                .HasIndex(s => s.Documento)
                .IsUnique();

            modelBuilder.Entity<Cancha>().HasData(
                new Cancha { CanchaID = 1, Nombre = "Cancha Central", Tipo = "Tenis", Activa = true },
                new Cancha { CanchaID = 2, Nombre = "Cancha Norte", Tipo = "Futbol", Activa = true },
                new Cancha { CanchaID = 3, Nombre = "Cancha 3", Tipo = "Padel", Activa = true }
            );
        }
    }
}