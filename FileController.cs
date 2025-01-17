using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WinFormsEFCoreApp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Windows.Forms;

namespace WinFormsEFCoreApp.Controllers
{
    public class FileController
    {
        private readonly AppDbContext _context;

        public FileController(AppDbContext context)
        {
            _context = context;
        }

        // Получить все файлы
        public List<File> GetFiles()
        {
            return _context.Files
                .Include(f => f.Classes)       // Включаем связанные классы
                .ThenInclude(c => c.Methods)   // Включаем методы для классов
                .ToList();
        }

        // Получить файл по ID
        public File GetFileById(int id)
        {
            return _context.Files
                .Include(f => f.Classes)
                .ThenInclude(c => c.Methods)
                .FirstOrDefault(f => f.FileId == id);
        }

        // Добавить новый файл
        public void AddFile(File newFile)
        {
            _context.Files.Add(newFile);
            _context.SaveChanges();
        }

        // Удалить файл по ID
        public void DeleteFile(int fileId)
        {
            var file = _context.Files
                .Include(f => f.Classes)
                .ThenInclude(c => c.Methods)
                .FirstOrDefault(f => f.FileId == fileId);

            if (file != null)
            {
                // Удаляем связанные методы и классы
                _context.Methods.RemoveRange(file.Classes.SelectMany(c => c.Methods));
                _context.Classes.RemoveRange(file.Classes);
                _context.Files.Remove(file);
                _context.SaveChanges();
            }
        }

        // Обновить данные файла
        public void UpdateFile(int fileId, string fileName, string filePath, int lines)
        {
            var file = _context.Files.FirstOrDefault(f => f.FileId == fileId);

            if (file != null)
            {
                file.FileName = fileName;
                file.FilePath = filePath;
                file.Lines = lines;
                _context.SaveChanges();
            }
        }

        // Метод для добавления файла с параметрами
        public void AddFile(string fileName, string filePath, int lines)
        {
            var file = new File
            {
                FileName = fileName,
                FilePath = filePath,
                Lines = lines,
                AnalysisDate = DateTime.Now
            };

            _context.Files.Add(file);
            _context.SaveChanges();
        }

        // Асинхронный метод для анализа файлов в директории
        public async Task AnalyzeDirectory(string directoryPath)
        {
            // Получаем все файлы C# в указанной директории
            var filePaths = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

            foreach (var filePath in filePaths)
            {
                // Читаем содержимое файла
                var fileCode = System.IO.File.ReadAllText(filePath);

                // Парсим файл с использованием Roslyn
                var syntaxTree = CSharpSyntaxTree.ParseText(fileCode);
                var root = await syntaxTree.GetRootAsync();

                // Создаем сущность File и заполняем её
                var newFile = new File
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    Lines = fileCode.Split('\n').Length,
                    AnalysisDate = DateTime.Now
                };

                // Находим все классы в файле
                var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                foreach (var classDeclaration in classDeclarations)
                {
                    // Получаем информацию о классе
                    var className = classDeclaration.Identifier.Text;
                    var classStartLine = classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    var classEndLine = classDeclaration.GetLocation().GetLineSpan().EndLinePosition.Line + 1;

                    // Создаем сущность Class и добавляем её к файлу
                    var newClass = new Class
                    {
                        ClassName = className,
                        StartLine = classStartLine,
                        EndLine = classEndLine
                    };

                    // Находим методы внутри класса
                    var methodDeclarations = classDeclaration.Members.OfType<MethodDeclarationSyntax>().ToList();

                    foreach (var methodDeclaration in methodDeclarations)
                    {
                        // Получаем информацию о методе
                        var methodName = methodDeclaration.Identifier.Text;
                        var methodStartLine = methodDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                        var methodEndLine = methodDeclaration.GetLocation().GetLineSpan().EndLinePosition.Line + 1;

                        // Создаем сущность Method и добавляем её к классу
                        var newMethod = new Method
                        {
                            MethodName = methodName,
                            StartLine = methodStartLine,
                            EndLine = methodEndLine
                        };

                        newClass.Methods.Add(newMethod);
                    }

                    // Обновляем количество методов в классе
                    newClass.MethodsCount = newClass.Methods.Count;

                    // Добавляем класс в файл
                    newFile.Classes.Add(newClass);
                }

                // Сохраняем файл, классы и методы в базу данных
                _context.Files.Add(newFile);
            }

            // Сохраняем все изменения в базу данных
            await _context.SaveChangesAsync();
        }



        public File GetFileById1(int fileId)
        {
            return _context.Files.FirstOrDefault(f => f.FileId == fileId);
        }

    }
}
