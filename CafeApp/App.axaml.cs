using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CafeApp.Helpers;
using CafeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

namespace CafeApp;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; }
    
    public new static App Current => (App)Application.Current!;

    public static User CurrentUser { get; set; } = null!;
    
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
        var collection = new ServiceCollection();
        collection.AddDbContext<CafeDbContext>();
        
        ExcelPackage.License.SetNonCommercialPersonal("Nikotryz");

        var services = collection.BuildServiceProvider();
        
        using (var scope = services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();
            db.Database.Migrate();
            
            var rolesExist = db.Roles.Any();
            
            if (!rolesExist)
            {
                var adminRole = new Role { Name = Roles.ADMIN_ROLE };
                var cookRole = new Role { Name = Roles.COOK_ROLE };
                var waiterRole = new Role { Name = Roles.WAITER_ROLE };
                
                db.Roles.AddRange(adminRole,  cookRole, waiterRole);
                db.SaveChanges();
            }
            
            var admin = db.Users.FirstOrDefault(x => x.Role.Name == Roles.ADMIN_ROLE);
            
            if (admin == null)
            {
                var newAdmin = new User
                {
                    Role = db.Roles.FirstOrDefault(x => x.Name == Roles.ADMIN_ROLE),
                    Login = "Nikotryz",
                    PasswordHash = PasswordHasher.HashPassword("900440"),
                    Name = "Никита",
                    LastName = "Харламов",
                    MiddleName = "Владимирович",
                    Birthday = DateOnly.FromDateTime(DateTime.Parse("05.09.2006")),
                    Status = UserStatuses.USER_WORKED
                };
                
                db.Users.Add(newAdmin);
                db.SaveChanges();
            }
        }
        
        return services;
    }
}