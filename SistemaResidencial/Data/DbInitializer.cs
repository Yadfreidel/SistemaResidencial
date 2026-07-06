using System;
using System.Linq;
using SistemaResidencial.Models;

namespace SistemaResidencial.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ResidencialDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Usuarios.Any())
            {
                return;
            }

            var admin = new Usuario
            {
                NombreUsuario = "admin",
                PasswordHash = "admin123", // TODO: Usar hashing con BCrypt cuando Auth lo requiera
                Rol = Rol.Admin
            };
            context.Usuarios.Add(admin);

            var recepcionista = new Usuario
            {
                NombreUsuario = "recepcionista",
                PasswordHash = "recepcionista123",
                Rol = Rol.Recepcionista
            };
            context.Usuarios.Add(recepcionista);

            var usuario = new Usuario
            {
                NombreUsuario = "usuario",
                PasswordHash = "usuario123",
                Rol = Rol.Usuario
            };
            context.Usuarios.Add(usuario);

            var aptos = new Apartamento[]
            {
                new Apartamento { Numero = "101", Piso = 1, Bloque = "A", NumHabitaciones = 2, MetrosCuadrados = 80, PrecioAlquiler = 500, Estado = EstadoApartamento.Disponible, Descripcion = "Apartamento amplio" },
                new Apartamento { Numero = "102", Piso = 1, Bloque = "A", NumHabitaciones = 3, MetrosCuadrados = 100, PrecioAlquiler = 700, Estado = EstadoApartamento.Disponible, Descripcion = "Vista a la calle" },
                new Apartamento { Numero = "201", Piso = 2, Bloque = "A", NumHabitaciones = 1, MetrosCuadrados = 50, PrecioAlquiler = 300, Estado = EstadoApartamento.EnMantenimiento, Descripcion = "Pintura" }
            };
            context.Apartamentos.AddRange(aptos);
            
            context.SaveChanges();
        }
    }
}
