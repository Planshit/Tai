using Core.Servicers.Instances;
using Core.Servicers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider serviceProvider;
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();

        }
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IObserver, Observer>();
            services.AddSingleton<IMain, Main>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var main = serviceProvider.GetService<IMain>();
            main.Run();
        }
    }
}
