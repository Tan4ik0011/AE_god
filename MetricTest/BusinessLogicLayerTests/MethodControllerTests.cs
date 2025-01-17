using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsEFCoreApp;
using WinFormsEFCoreApp.Controllers;
using Xunit;

namespace WinFormsEFCoreApp.Tests
{
    public class MethodControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MethodController _controller;

        public MethodControllerTests()
        {
            // Настройка InMemory базы данных для тестов
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestMethodDatabase")  // Уникальное имя базы данных для тестов
                .Options;

            _context = new AppDbContext(options);
            _controller = new MethodController(_context);

            // Инициализация тестовых данных
            _context.Classes.AddRange(new List<Class>
            {
                new Class { ClassId = 1, ClassName = "Class1", StartLine = 1, EndLine = 10, MethodsCount = 3, FileId = 1 },
                new Class { ClassId = 2, ClassName = "Class2", StartLine = 11, EndLine = 20, MethodsCount = 5, FileId = 2 }
            });
            _context.Methods.AddRange(new List<Method>
            {
                new Method { MethodId = 1, MethodName = "Method1", StartLine = 1, EndLine = 5, ClassId = 1 },
                new Method { MethodId = 2, MethodName = "Method2", StartLine = 6, EndLine = 10, ClassId = 1 },
                new Method { MethodId = 3, MethodName = "Method3", StartLine = 11, EndLine = 15, ClassId = 2 }
            });
            _context.SaveChanges();
        }

        [Fact]
        public void GetMethods_ReturnsAllMethods()
        {
            // Act
            var result = _controller.GetMethods().ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Method1", result[0].MethodName);
            Assert.Equal("Method2", result[1].MethodName);
            Assert.Equal("Method3", result[2].MethodName);
        }

        [Fact]
        public void GetMethodById_ReturnsCorrectMethod()
        {
            // Act
            var result = _controller.GetMethodById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MethodId);
            Assert.Equal("Method1", result.MethodName);
        }

        [Fact]
        public void AddMethod_AddsNewMethod()
        {
            // Act
            _controller.AddMethod("Method4", 1, 16, 20);

            // Assert
            var addedMethod = _context.Methods.FirstOrDefault(m => m.MethodName == "Method4");
            Assert.NotNull(addedMethod);
            Assert.Equal("Method4", addedMethod.MethodName);
        }

        [Fact]
        public void DeleteMethod_RemovesMethod()
        {
            // Act
            _controller.DeleteMethod(1);

            // Assert
            var deletedMethod = _context.Methods.FirstOrDefault(m => m.MethodId == 1);
            Assert.Null(deletedMethod);  // Проверяем, что метод был удален
        }

        [Fact]
        public void UpdateMethod_UpdatesMethodDetails()
        {
            // Act
            _controller.UpdateMethod(1, "UpdatedMethod", 1, 10);

            // Assert
            var updatedMethod = _context.Methods.FirstOrDefault(m => m.MethodId == 1);
            Assert.NotNull(updatedMethod);
            Assert.Equal("UpdatedMethod", updatedMethod.MethodName);
            Assert.Equal(1, updatedMethod.StartLine);
            Assert.Equal(10, updatedMethod.EndLine);
        }

        [Fact]
        public void GetMethodsByClassId_ReturnsMethodsForClass()
        {
            // Act
            var result = _controller.GetMethodsByClassId(1);

            // Assert
            Assert.Equal(2, result.Count);  // Должны быть два метода для ClassId = 1
            Assert.Equal("Method1", result[0].MethodName);
            Assert.Equal("Method2", result[1].MethodName);
        }

        // Dispose метод для очистки базы данных после выполнения тестов
        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Удаляем базу данных после тестов
            _context.Dispose();
        }
    }
}
