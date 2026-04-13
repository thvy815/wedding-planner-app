using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuanLyTiecCuoi.Data;
using QuanLyTiecCuoi.MVVM.View.Login;
using QuanLyTiecCuoi.MVVM.View.MainVindow;
using QuanLyTiecCuoi.MVVM.ViewModel;
using QuanLyTiecCuoi.Repository;
using QuanLyTiecCuoi.Services;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Windows;
using System.Reflection;


namespace QuanLyTiecCuoi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }
        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    string connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<WeddingDbContext>(options =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

                    var assembly = Assembly.GetExecutingAssembly();

                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsClass && !type.IsAbstract)
                        {
                            if (type.Name.EndsWith("Service") ||
                                type.Name.EndsWith("Repository") ||
                                type.Name.EndsWith("ViewModel") ||
                                type.Name.EndsWith("Window") ||
                                type.Name.EndsWith("Page"))
                            {
                                services.AddTransient(type);
                            }
                        }
                    }
                })
                .Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();

            try
            {
                var loginWindow = AppHost.Services.GetRequiredService<LoginWindow>();

                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.ToString());
            }

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            AppHost.Dispose();
            base.OnExit(e);
        }
    }

}
