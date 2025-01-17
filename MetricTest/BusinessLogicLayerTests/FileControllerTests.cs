using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinFormsEFCoreApp.Controllers;
using WinFormsEFCoreApp;
using Xunit;

namespace WinFormsEFCoreApp.Tests
{
    public class FileControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly FileController _controller;

        public FileControllerTests()
        {
            // Настройка InMemory базы данных для тестов
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")  // Уникальное имя базы данных для тестов
                .Options;

            _context = new AppDbContext(options);
            _controller = new FileController(_context);

            // Инициализация тестовых данных
            _context.Files.AddRange(new List<File>
            {
                new File { FileId = 1, FileName = "File1.cs", FilePath = "/path/to/File1.cs", Lines = 10 },
                new File { FileId = 2, FileName = "File2.cs", FilePath = "/path/to/File2.cs", Lines = 20 }
            });
            _context.SaveChanges();
        }

        [Fact]
        public void GetFiles_ReturnsAllFiles()
        {
            // Act
            var result = _controller.GetFiles();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("File1.cs", result[0].FileName);
            Assert.Equal("File2.cs", result[1].FileName);
        }

        [Fact]
        public void GetFileById_ReturnsCorrectFile()
        {
            // Act
            var result = _controller.GetFileById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.FileId);
            Assert.Equal("File1.cs", result.FileName);
        }

        [Fact]
        public void AddFile_AddsNewFile()
        {
            // Arrange
            var newFile = new File
            {
                FileName = "NewFile.cs",
                FilePath = "/path/to/NewFile.cs",
                Lines = 15
            };

            // Act
            _controller.AddFile(newFile);

            // Assert
            var addedFile = _context.Files.FirstOrDefault(f => f.FileName == "NewFile.cs");
            Assert.NotNull(addedFile);
            Assert.Equal("NewFile.cs", addedFile.FileName);
        }

        [Fact]
        public void DeleteFile_RemovesFile()
        {
            // Act
            _controller.DeleteFile(1);

            // Assert
            var file = _context.Files.FirstOrDefault(f => f.FileId == 1);
            Assert.Null(file);  // Проверяем, что файл был удален
        }

        [Fact]
        public void UpdateFile_UpdatesFileDetails()
        {
            // Act
            _controller.UpdateFile(1, "UpdatedFile.cs", "/path/to/UpdatedFile.cs", 30);

            // Assert
            var updatedFile = _context.Files.FirstOrDefault(f => f.FileId == 1);
            Assert.NotNull(updatedFile);
            Assert.Equal("UpdatedFile.cs", updatedFile.FileName);
            Assert.Equal("/path/to/UpdatedFile.cs", updatedFile.FilePath);
            Assert.Equal(30, updatedFile.Lines);
        }

        [Fact]
        public async Task AnalyzeDirectory_AnalyzesFilesAndSavesToDb()
        {
            // Arrange
            var directoryPath = "/some/path";

            // Для тестирования можно добавить несколько файлов в контекст
            var newFile1 = new File
            {
                FileName = "NewFile1.cs",
                FilePath = "/path/to/NewFile1.cs",
                Lines = 25
            };
            var newFile2 = new File
            {
                FileName = "NewFile2.cs",
                FilePath = "/path/to/NewFile2.cs",
                Lines = 40
            };

            // Act
            await _controller.AnalyzeDirectory(directoryPath);

            // Assert
            var filesInDb = _context.Files.ToList();
            Assert.True(filesInDb.Count > 2); // Ожидаем, что будет больше 2 файлов в базе
        }

        // Dispose метод для очистки базы данных после выполнения тестов
        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Удаляем базу данных после тестов
            _context.Dispose();
        }
    }
}
