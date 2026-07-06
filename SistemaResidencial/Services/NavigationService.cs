using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace SistemaResidencial.Services
{
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        public event Action<ObservableObject>? VistaCambiada;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavegarA<TViewModel>() where TViewModel : ObservableObject
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            VistaCambiada?.Invoke(viewModel);
        }
    }
}
