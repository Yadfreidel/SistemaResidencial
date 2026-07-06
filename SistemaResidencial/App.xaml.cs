using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SistemaResidencial.Data;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Repositories;
using SistemaResidencial.Services;
using SistemaResidencial.Validators;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // ─── Base de datos ────────────────────────────────────────────────
            // Usamos AddDbContextFactory para evitar conflictos de ciclo de vida
            // entre servicios Singleton (AuthService) y el DbContext (Scoped).
            services.AddDbContext<ResidencialDbContext>(options =>
                options.UseSqlite("Data Source=residencial.db"),
                ServiceLifetime.Singleton);

            // ─── Repositorios (Singleton para coincidir con el DbContext) ─────
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<IApartamentoRepository, ApartamentoRepository>();
            services.AddSingleton<IInquilinoRepository, InquilinoRepository>();
            services.AddSingleton<IContratoRepository, ContratoRepository>();
            services.AddSingleton<IPagoRepository, PagoRepository>();

            // ─── Servicios (todos Singleton — una instancia por sesión de app) ─
            services.AddSingleton<SesionService>();
            services.AddSingleton<AuthService>();
            services.AddSingleton<NavigationService>();
            services.AddSingleton<DialogService>();

            // ─── Validadores (Transient — nuevos por cada validación) ─────────
            services.AddTransient<ApartamentoValidator>();
            services.AddTransient<InquilinoValidator>();
            services.AddTransient<ContratoValidator>();
            services.AddTransient<PagoValidator>();

            // ─── ViewModels de Yadfreidel ─────────────────────────────────────
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();

            // Dashboards según rol
            services.AddTransient<DashboardAdminViewModel>();
            services.AddTransient<DashboardRecepcionistaViewModel>();
            services.AddTransient<DashboardUsuarioViewModel>();
            services.AddTransient<DashboardViewModel>(); // Stub — conservado

            // Módulos CRUD
            services.AddTransient<ApartamentoViewModel>();
            services.AddTransient<InquilinoViewModel>();
            services.AddTransient<ContratoViewModel>();
            services.AddTransient<PagoViewModel>();

            // Cambio de contraseña
            services.AddTransient<CambiarPasswordViewModel>();

            // ─── Ventana principal ────────────────────────────────────────────
            services.AddTransient<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            // ─── Inicializar base de datos y seed data ────────────────────────
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ResidencialDbContext>();
                DbInitializer.Initialize(context);
            }

            // ─── Arrancar la aplicación mostrando el Login ────────────────────
            // Se crea primero la ventana (y su MainViewModel) para que quede
            // suscrita al evento de navegación antes de navegar al Login.
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            var navigationService = ServiceProvider.GetRequiredService<NavigationService>();
            navigationService.Navegar("Login");

            mainWindow.Show();
        }
    }
}
