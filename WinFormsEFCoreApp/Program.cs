using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using static WinFormsEFCoreApp.Program;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using WinFormsEFCoreApp.Controllers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;


namespace WinFormsEFCoreApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var host = CreateHostBuilder().Build();

            Application.SetCompatibleTextRenderingDefault(false);

            Application.EnableVisualStyles();

            var mainForm = host.Services.GetRequiredService<MainForm>();

            Application.Run(mainForm);
        }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(
                            hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddScoped<FileController>();
                    services.AddScoped<ClassController>();
                    services.AddScoped<MethodController>();

                    services.AddScoped<MainForm>();
                });
    }  

    public class AppDbContext : DbContext
    {
        public DbSet<Class> Classes { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Method> Methods { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>()
                .HasOne(c => c.File)
                .WithMany(f => f.Classes)
                .HasForeignKey(c => c.FileId);

            modelBuilder.Entity<Method>()
                .HasOne(m => m.Class)
                .WithMany(c => c.Methods)
                .HasForeignKey(m => m.ClassId);
        }

    }
}


