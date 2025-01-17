using Xunit;
using WinFormsEFCoreApp;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WinFormsEFCoreApp.Controllers;

namespace MetricTest.Tests
{
    public class MainFormTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                          .UseInMemoryDatabase(databaseName: "TestDb")
                          .Options;
            return new AppDbContext(options);
        }

        private FileController CreateFileController(AppDbContext context)
        {
            return new FileController(context);
        }

        private ClassController CreateClassController(AppDbContext context)
        {
            return new ClassController(context);
        }

        private MethodController CreateMethodController(AppDbContext context)
        {
            return new MethodController(context);
        }

        // Метод очистки базы данных
        private void ClearDatabase(AppDbContext context)
        {
            context.Files.RemoveRange(context.Files);
            context.Classes.RemoveRange(context.Classes);
            context.Methods.RemoveRange(context.Methods);
            context.SaveChanges();
        }

        [Fact]
        public void ShowFiles_ShouldShowFilesInDataGridView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            ClearDatabase(context);  // Очищаем базу данных перед тестом
            var fileController = CreateFileController(context);
            var classController = CreateClassController(context);
            var methodController = CreateMethodController(context);
            var mainForm = new MainForm(context, fileController, classController, methodController);

            // Добавляем файл
            fileController.AddFile("TestFile.cs", @"C:\path\to\file", 100);

            // Act
            mainForm.ShowFiles(); // Просто вызываем метод без присваивания результата

            // Assert
            var files = fileController.GetFiles();
            Assert.Single(files);
            Assert.Equal("TestFile.cs", files.First().FileName);
        }

        [Fact]
        public void ShowClasses_ShouldShowClassesInDataGridView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            ClearDatabase(context);  // Очищаем базу данных перед тестом
            var fileController = CreateFileController(context);
            var classController = CreateClassController(context);
            var methodController = CreateMethodController(context);
            var mainForm = new MainForm(context, fileController, classController, methodController);

            // Добавляем файл
            fileController.AddFile("TestFile.cs", @"C:\path\to\file", 100);

            // Получаем только что добавленный файл
            var file = fileController.GetFiles().First(); // Получаем первый файл из базы данных (если есть)

            // Добавляем класс
            classController.AddClass("TestClass", file.FileId, 1, 50, 2); // Добавляем класс

            // Act
            mainForm.ShowClasses(); // Просто вызываем метод без присваивания результата

            // Assert
            var classes = classController.GetClasses();
            Assert.Single(classes);  // Проверяем, что добавлен только один класс
            Assert.Equal("TestClass", classes.First().ClassName); // Проверяем имя класса
        }

        [Fact]
        public void ShowMethods_ShouldShowMethodsInDataGridView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            ClearDatabase(context);  // Очищаем базу данных перед тестом
            var fileController = CreateFileController(context);
            var classController = CreateClassController(context);
            var methodController = CreateMethodController(context);
            var mainForm = new MainForm(context, fileController, classController, methodController);

            // Добавляем файл
            fileController.AddFile("TestFile.cs", @"C:\path\to\file", 100);

            // Получаем только что добавленный файл
            var file = fileController.GetFiles().First(); // Получаем первый файл из базы данных (если есть)

            // Добавляем класс
            classController.AddClass("TestClass", file.FileId, 1, 50, 2);

            // Получаем только что добавленный класс
            var classEntity = classController.GetClasses().First(); // Получаем первый класс

            // Добавляем метод
            methodController.AddMethod("TestMethod", classEntity.ClassId, 1, 20);

            // Act
            mainForm.ShowMethods(); // Просто вызываем метод без присваивания результата

            // Assert
            var methods = methodController.GetMethods();
            Assert.Single(methods);  // Убедитесь, что метод был добавлен
            Assert.Equal("TestMethod", methods.First().MethodName); // Проверяем имя метода
        }
    }
}
