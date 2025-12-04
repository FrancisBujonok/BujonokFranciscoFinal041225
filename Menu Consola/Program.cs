using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Tp3_Programacion.Data;
using Tp3_Programacion.Models;

namespace Menu_Consola.App
{
    public class Program
    {
        private static AplicacionDbContext db;

        public static void Main(string[] args)
        {
            db = new AplicacionDbContext();
            db.Database.EnsureCreated();

            Console.Clear();
            Console.WriteLine("GESTIÓN DEPORTIVA - CONSOLA");
            Console.WriteLine("---");

            MenuPrincipal();
        }

        private static void MenuPrincipal()
        {
            bool salir = false;
            while (!salir)
            {
                Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
                Console.WriteLine("1. Registrar nueva Cancha");
                Console.WriteLine("2. Registrar nuevo Socio");
                Console.WriteLine("3. Registrar nueva Reserva");
                Console.WriteLine("4. Listar Reservas Vigentes (REPORTE 1)");
                Console.WriteLine("5. Generar Reporte de Uso por Cancha (REPORTE 2)");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");

                var opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1":
                        RegistrarCancha();
                        break;
                    case "2":
                        RegistrarSocio();
                        break;
                    case "3":
                        RegistrarReserva();
                        break;
                    case "4":
                        ListarReservasVigentes();
                        break;
                    case "5":
                        GenerarReporteUsoCancha();
                        break;
                    case "0":
                        salir = true;
                        Console.WriteLine("Nos vemos!!");
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intenta otra vez.");
                        break;
                }
            }
        }

        // switch 1
        private static void RegistrarCancha()
        {
            Console.WriteLine("\n Registrar nueva cancha");
            var nuevaCancha = new Cancha();

            Console.Write("Nombre de la Cancha: ");
            nuevaCancha.Nombre = Console.ReadLine();

            Console.Write("Tipo (Futbol, Tenis, Padel): ");
            var tipo = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(tipo) || !new[] { "Futbol", "Tenis", "Padel" }.Any(t => t.Equals(tipo, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Tipo no reconocido. Asignando 'Otro'.");
                nuevaCancha.Tipo = "Otro";
            }
            else
            {
                nuevaCancha.Tipo = char.ToUpper(tipo[0]) + tipo.Substring(1).ToLower();
            }

            Console.Write("¿Está activa? (S/N, por defecto S): ");
            var activaInput = Console.ReadLine();
            nuevaCancha.Activa = !string.Equals(activaInput, "N", StringComparison.OrdinalIgnoreCase);

            try
            {
                db.Canchas.Add(nuevaCancha);
                db.SaveChanges();
                Console.WriteLine($"Cancha '{nuevaCancha.Nombre}' registrada exitosamente con ID: {nuevaCancha.CanchaID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar la cancha: {ex.Message}");
            }
        }

        //switch 2
        private static void RegistrarSocio()
        {
            Console.WriteLine("\n Registrar nuevo socio");
            var nuevoSocio = new Socio();

            Console.Write("Nombre completo del Socio: ");
            nuevoSocio.Nombre = Console.ReadLine();

            Console.Write("Número de Documento (DNI/Pasaporte): ");
            nuevoSocio.Documento = Console.ReadLine();

            Console.Write("Correo Electrónico: ");
            nuevoSocio.CorreoElectronico = Console.ReadLine();

            try
            {
                if (db.Socios.Any(s => s.Documento == nuevoSocio.Documento))
                {
                    Console.WriteLine(" Un socio con este Documento ya está registrado. No se puede duplicar.");
                    return;
                }

                db.Socios.Add(nuevoSocio);
                db.SaveChanges();
                Console.WriteLine($" Socio '{nuevoSocio.Nombre}' registrado exitosamente con ID: {nuevoSocio.SocioID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar el socio: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        private static bool TryParseDateTime(string input, out DateTime result)
        {
            if (DateTime.TryParse(input, out result)) return true;
            if (DateTime.TryParseExact(input, "dd-MM-yy HH:mm", null, System.Globalization.DateTimeStyles.None, out result)) return true;
            if (DateTime.TryParseExact(input, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out result)) return true;
            return false;
        }

        // switch 3 
        private static void RegistrarReserva()
        {
            Console.WriteLine("\n Registrar nueva Reserva!");

            var socios = db.Socios.ToList();
            if (!socios.Any())
            {
                Console.WriteLine("Error, No hay socios registrados. Debe registrar un socio primero.");
                return;
            }
            Console.WriteLine("Socios disponibles:");
            foreach (var s in socios)
            {
                Console.WriteLine($"[{s.SocioID}] {s.Nombre}");
            }
            Console.Write("Seleccione el ID del Socio: ");
            if (!int.TryParse(Console.ReadLine(), out int socioId) || !socios.Any(s => s.SocioID == socioId))
            {
                Console.WriteLine("Error! ID de socio no válido.");
                return;
            }

            var canchasActivas = db.Canchas.Where(c => c.Activa).ToList();
            if (!canchasActivas.Any())
            {
                Console.WriteLine("No hay canchas activas disponibles.");
                return;
            }
            Console.WriteLine("\nCanchas activas disponibles:");
            foreach (var c in canchasActivas)
            {
                Console.WriteLine($"[{c.CanchaID}] {c.Nombre} ({c.Tipo})");
            }
            Console.Write("Seleccione el ID de la Cancha: ");
            if (!int.TryParse(Console.ReadLine(), out int canchaId) || !canchasActivas.Any(c => c.CanchaID == canchaId))
            {
                Console.WriteLine("Error, ID de cancha no válido.");
                return;
            }

            Console.Write("\nIngrese Fecha y Hora de la reserva (Ej: 25-12-2025 18:30): ");
            if (!TryParseDateTime(Console.ReadLine(), out DateTime fechaHoraReserva))
            {
                Console.WriteLine("Error! Formato de fecha y hora no válido. Use el formato DD-MM-YYYY HH:MM.");
                return;
            }

            if (fechaHoraReserva < DateTime.Now)
            {
                Console.WriteLine("Error! No se puede reservar una cancha en una fecha pasada.");
                return;
            }

            var inicioBloque = new DateTime(fechaHoraReserva.Year, fechaHoraReserva.Month, fechaHoraReserva.Day, fechaHoraReserva.Hour, 0, 0);

            var reservaExistente = db.Reservas
                .Any(r => r.CanchaID == canchaId && r.FechaHora == inicioBloque);

            if (reservaExistente)
            {
                Console.WriteLine($"Error! La cancha ya está reservada para el horario de {inicioBloque:HH:mm} a {inicioBloque.AddHours(1):HH:mm}.");
                return;
            }

            fechaHoraReserva = inicioBloque;

            var nuevaReserva = new Reserva
            {
                SocioID = socioId,
                CanchaID = canchaId,
                FechaHora = fechaHoraReserva
            };

            try
            {
                db.Reservas.Add(nuevaReserva);
                db.SaveChanges();
                Console.WriteLine($"\n Bien! Reserva ID {nuevaReserva.ReservaID} registrada exitosamente para {nuevaReserva.FechaHora:dd/MM/yyyy HH:mm}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la reserva: {ex.Message}");
            }
        }

        // switch 4
        private static void ListarReservasVigentes()
        {
            Console.WriteLine("\n Reporte 1: Reservas vigentes");

            var fechaReporte = DateTime.Now;
            var hoy = fechaReporte.Date;

            var reservas = db.Reservas
                .Include(r => r.Socio)
                .Include(r => r.Cancha)
                .Where(r => r.FechaHora.Date >= hoy)
                .OrderBy(r => r.FechaHora)
                .ToList();

            if (!reservas.Any())
            {
                Console.WriteLine($"No hay reservas vigentes registradas a partir de hoy ({hoy:dd/MM/yyyy}).");
                return;
            }

            Console.WriteLine($"Reporte generado el: {fechaReporte:dd-MM-yy HH:mm:ss}");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("{0,-10} {1,-20} {2,-20} {3,-20} {4,-15}", "ReservaID", "Socio", "Cancha", "Fecha", "Estado");
            Console.WriteLine("--------------------------------------------------------------------------------");


            foreach (var r in reservas)
            {
                string estado;

                var horaActual = fechaReporte;
                var inicioReserva = r.FechaHora;
                var finReserva = r.FechaHora.AddHours(1);

                if (horaActual >= finReserva)
                {
                    estado = "Finalizado";
                }
                else if (horaActual >= inicioReserva && horaActual < finReserva)
                {
                    estado = "En curso";
                }
                else
                {
                    estado = "Pendiente";
                }

                Console.WriteLine("{0,-10} {1,-20} {2,-20} {3,-20} {4,-15}",
                    r.ReservaID,
                    r.Socio.Nombre.Substring(0, Math.Min(r.Socio.Nombre.Length, 18)),
                    r.Cancha.Nombre.Substring(0, Math.Min(r.Cancha.Nombre.Length, 18)),
                    r.FechaHora.ToString("dd-MM-yy HH:mm:ss"),
                    estado);
            }
            Console.WriteLine("--------------------------------------------------------------------------------");
        }

        // switch 5
        private static void GenerarReporteUsoCancha()
        {
            Console.WriteLine("\n Reporte 2: Uso por cancha");

            var reporteUso = db.Reservas
                .Include(r => r.Cancha)
                .GroupBy(r => r.Cancha)
                .Select(g => new
                {
                    Cancha = g.Key.Nombre,
                    Tipo = g.Key.Tipo,
                    CantidadReservas = g.Count()
                })
                .OrderByDescending(r => r.CantidadReservas)
                .ToList();

            if (!reporteUso.Any())
            {
                Console.WriteLine("No hay reservas para generar el reporte de uso.");
                return;
            }

            Console.WriteLine("-------------------------------------");
            Console.WriteLine("{0,-25} {1,-10} {2,-10}", "Cancha", "Tipo", "Cantidad");
            Console.WriteLine("-------------------------------------");

            foreach (var item in reporteUso)
            {
                Console.WriteLine("{0,-25} {1,-10} {2,-10}", item.Cancha, item.Tipo, item.CantidadReservas);
            }
            Console.WriteLine("-------------------------------------");
        }
    }
}