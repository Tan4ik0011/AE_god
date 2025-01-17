using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using static WinFormsEFCoreApp.Program;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinFormsEFCoreApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        public class AppDbContext : DbContext
        {
            

            public DbSet<Class> Classes { get; set; }
            public DbSet<File> Files { get; set; }
            public DbSet<Method> Methods { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\Local;Database=Test;Integrated Security=True;");
            }

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

public class Analyzer
    {
        private readonly AppDbContext _context;

        internal Analyzer(AppDbContext context) // Конструктор с public доступом
        {
            _context = context;
        }

        // Метод для сканирования директории и анализа файлов
        public void AnalyzeDirectory(string directoryPath)
        {
            var files = new List<File>();

            // Получаем все C# файлы в указанной директории и её подкаталогах
            var csFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

            foreach (var csFile in csFiles)
            {
                // Анализируем каждый файл с использованием Roslyn
                var file = AnalyzeFile(csFile);
                files.Add(file);
            }

            // Сохраняем все данные в базу данных через контекст
            _context.Files.AddRange(files);
            _context.SaveChanges();
        }

        // Метод для анализа одного файла
        private File AnalyzeFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var fileName = fileInfo.Name;
            var fileLines = System.IO.File.ReadAllLines(filePath);
            var linesCount = fileLines.Length;
            var analysisDate = DateTime.Now;

            var fileEntity = new File
            {
                FileName = fileName,
                FilePath = filePath,
                Lines = linesCount,
                AnalysisDate = analysisDate
            };

            var syntaxTree = CSharpSyntaxTree.ParseText(System.IO.File.ReadAllText(filePath));
            var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

            // Извлекаем все классы из файла
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classSyntax in classes)
            {
                var classEntity = new Class
                {
                    ClassName = classSyntax.Identifier.Text,
                    StartLine = classSyntax.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    EndLine = classSyntax.GetLocation().GetLineSpan().EndLinePosition.Line + 1,
                    MethodsCount = classSyntax.Members.OfType<MethodDeclarationSyntax>().Count()
                };

                // Добавляем методы в класс
                foreach (var methodSyntax in classSyntax.Members.OfType<MethodDeclarationSyntax>())
                {
                    var methodEntity = new Method
                    {
                        MethodName = methodSyntax.Identifier.Text,
                        StartLine = methodSyntax.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                        EndLine = methodSyntax.GetLocation().GetLineSpan().EndLinePosition.Line + 1
                    };

                    classEntity.Methods.Add(methodEntity);
                }

                fileEntity.Classes.Add(classEntity);
            }

            return fileEntity;
        }
    }

}