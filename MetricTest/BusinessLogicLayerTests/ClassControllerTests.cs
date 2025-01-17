using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinFormsEFCoreApp;
using WinFormsEFCoreApp.Controllers;
using Xunit;

namespace WinFormsEFCoreApp.Tests
{
    public class ClassControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ClassController _controller;

        public ClassControllerTests()
        {
            // Настройка InMemory базы данных для тестов
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestClassDatabase")  // Уникальное имя базы данных для тестов
                .Options;

            _context = new AppDbContext(options);
            _controller = new ClassController(_context);

            // Инициализация тестовых данных
            _context.Classes.AddRange(new List<Class>
            {
                new Class { ClassId = 1, ClassName = "Class1", StartLine = 1, EndLine = 10, MethodsCount = 3, FileId = 1 },
                new Class { ClassId = 2, ClassName = "Class2", StartLine = 11, EndLine = 20, MethodsCount = 5, FileId = 2 }
            });
            _context.SaveChanges();
        }

        [Fact]
        public void GetClasses_ReturnsAllClasses()
        {
            // Act
            var result = _controller.GetClasses();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Class1", result[0].ClassName);
            Assert.Equal("Class2", result[1].ClassName);
        }

        [Fact]
        public void GetClassById_ReturnsCorrectClass()
        {
            // Act
            var result = _controller.GetClassById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ClassId);
            Assert.Equal("Class1", result.ClassName);
        }

        [Fact]
        public void AddClass_AddsNewClass()
        {
            // Arrange
            var newClass = new Class
            {
                ClassName = "Class3",
                StartLine = 21,
                EndLine = 30,
                MethodsCount = 2,
                FileId = 1
            };

            // Act
            _controller.AddClass(newClass);

            // Assert
            var addedClass = _context.Classes.FirstOrDefault(c => c.ClassName == "Class3");
            Assert.NotNull(addedClass);
            Assert.Equal("Class3", addedClass.ClassName);
        }

        [Fact]
        public void DeleteClass_RemovesClass()
        {
            // Act
            _controller.DeleteClass(1);

            // Assert
            var deletedClass = _context.Classes.FirstOrDefault(c => c.ClassId == 1);
            Assert.Null(deletedClass);  // Проверяем, что класс был удален
        }

        [Fact]
        public void UpdateClass_UpdatesClassDetails()
        {
            // Act
            _controller.UpdateClass(1, "UpdatedClass", 5, 15, 4);

            // Assert
            var updatedClass = _context.Classes.FirstOrDefault(c => c.ClassId == 1);
            Assert.NotNull(updatedClass);
            Assert.Equal("UpdatedClass", updatedClass.ClassName);
            Assert.Equal(5, updatedClass.StartLine);
            Assert.Equal(15, updatedClass.EndLine);
            Assert.Equal(4, updatedClass.MethodsCount);
        }

        [Fact]
        public void AddClass_WithParameters_AddsNewClass()
        {
            // Act
            _controller.AddClass("Class4", 1, 31, 40, 6);

            // Assert
            var addedClass = _context.Classes.FirstOrDefault(c => c.ClassName == "Class4");
            Assert.NotNull(addedClass);
            Assert.Equal("Class4", addedClass.ClassName);
        }

        [Fact]
        public void GetClassesByFileId_ReturnsClassesForFile()
        {
            // Act
            var result = _controller.GetClassesByFileId(1);

            // Assert
            Assert.Equal(1, result.Count);  // Должен быть один класс для FileId = 1
            Assert.Equal("Class1", result[0].ClassName);
        }

        // Dispose метод для очистки базы данных после выполнения тестов
        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Удаляем базу данных после тестов
            _context.Dispose();
        }
    }
}
