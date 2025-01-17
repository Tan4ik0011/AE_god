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
        /// <summary>
        /// Основная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Создание хоста с настройками зависимостей
            var host = CreateHostBuilder().Build();

            // Настройка совместимости с текстовым рендерингом
            Application.SetCompatibleTextRenderingDefault(false);

            // Включение визуальных стилей для приложения
            Application.EnableVisualStyles();

            // Получаем MainForm через DI контейнер
            var mainForm = host.Services.GetRequiredService<MainForm>();

            // Запуск формы
            Application.Run(mainForm);
        }

        // Настройка зависимостей с использованием IHostBuilder
        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    // Настройка строки подключения к базе данных
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(
                            hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    // Добавление контроллеров в DI контейнер
                    services.AddScoped<FileController>();
                    services.AddScoped<ClassController>();
                    services.AddScoped<MethodController>();

                    // Регистрация MainForm в DI контейнере
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


