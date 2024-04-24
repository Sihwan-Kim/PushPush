using Microsoft.Extensions.DependencyInjection;
using PushPush.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PushPush
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App:Application
    {
        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddTransient(typeof(MainViewModel));

            return services.BuildServiceProvider();
        }
    }
}
