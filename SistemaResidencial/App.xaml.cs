using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SistemaResidencial.Data;
using SistemaResidencial.Services;
using SistemaResidencial.Validators;
using SistemaResidencial.ViewModels;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Repositories;

namespace SistemaResidencial
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddDbContext<ResidencialDbContext>(options =>
            {
                options.UseSqlite("Data Source=residencial.db");
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IApartamentoRepository, ApartamentoRepository>();
            services.AddScoped<IInquilinoRepository, InquilinoRepository>();
            services.AddScoped<IContratoRepository, ContratoRepository>();
            services.AddScoped<IPagoRepository, PagoRepository>();

            services.AddSingleton<SesionService>();
            services.AddSingleton<AuthService>();
            services.AddSingleton<NavigationService>();
            services.AddSingleton<DialogService>();

            services.AddTransient<ApartamentoValidator>();
            services.AddTransient<InquilinoValidator>();
            services.AddTransient<ContratoValidator>();
            services.AddTransient<PagoValidator>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<ApartamentoViewModel>();
            services.AddTransient<InquilinoViewModel>();
            services.AddTransient<ContratoViewModel>();
            services.AddTransient<PagoViewModel>();

            services.AddTransient<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ResidencialDbContext>();
                DbInitializer.Initialize(context);
            }

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
