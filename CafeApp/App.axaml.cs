using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; }
    
    public new static App Current => (App)Application.Current!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services = ConfigureServices();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    public static IServiceProvider ConfigureServices()
    {
        var connectionString1 =
            "Host=10.30.0.137;Port=5432;Database=gr624_hanvl;Username=gr624_hanvl;Password=Nikita228900440@";

        var connectionString2 =
            "Host=localhost;Port=5432;Database=cafe_app_db;Username=postgres;Password=900440";
        
        var collection = new ServiceCollection();
        collection.AddDbContext<CafeDbContext>(options => options.UseNpgsql(connectionString1));

        var services = collection.BuildServiceProvider();
        
        using (var scope = services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();
            db.Database.Migrate();
        }
        
        return services;
    }
}