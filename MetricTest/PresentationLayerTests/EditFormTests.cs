using System;
using System.Windows.Forms;
using Xunit;
using WinFormsEFCoreApp;

namespace WinFormsEFCoreApp.Tests
{
    public class EditFormTests
    {
        [Fact]
        public void EditForm_ShouldInitializeFileRecord()
        {
            // Arrange
            var file = new File
            {
                FileName = "TestFile.cs",
                FilePath = @"C:\path\to\file",
                Lines = 100,
                AnalysisDate = DateTime.Parse("2025-01-17")
            };

            // Act
            var form = new EditForm("Files", file);

            // Assert
            var textBox1 = form.Controls["textBox1"] as TextBox;
            var textBox2 = form.Controls["textBox2"] as TextBox;
            var textBox3 = form.Controls["textBox3"] as TextBox;
            var textBox4 = form.Controls["textBox4"] as TextBox;

            Assert.NotNull(textBox1);
            Assert.NotNull(textBox2);
            Assert.NotNull(textBox3);
            Assert.NotNull(textBox4);

            Assert.Equal("TestFile.cs", textBox1.Text);
            Assert.Equal(@"C:\path\to\file", textBox2.Text);
            Assert.Equal("100", textBox3.Text);
            Assert.Equal("2025-01-17", textBox4.Text);
        }

        [Fact]
        public void EditForm_ShouldInitializeClassRecord()
        {
            // Arrange
            var classEntity = new Class
            {
                ClassName = "TestClass",
                StartLine = 1,
                EndLine = 50,
                MethodsCount = 2
            };

            // Act
            var form = new EditForm("Classes", classEntity);

            // Assert
            var textBox1 = form.Controls["textBox1"] as TextBox;
            var textBox2 = form.Controls["textBox2"] as TextBox;
            var textBox3 = form.Controls["textBox3"] as TextBox;
            var textBox4 = form.Controls["textBox4"] as TextBox;

            Assert.NotNull(textBox1);
            Assert.NotNull(textBox2);
            Assert.NotNull(textBox3);
            Assert.NotNull(textBox4);

            Assert.Equal("TestClass", textBox1.Text);
            Assert.Equal("1", textBox2.Text);
            Assert.Equal("50", textBox3.Text);
            Assert.Equal("2", textBox4.Text);
        }

        [Fact]
        public void EditForm_ShouldInitializeMethodRecord()
        {
            // Arrange
            var classEntity = new Class
            {
                ClassName = "TestClass",
                StartLine = 1,
                EndLine = 50,
                MethodsCount = 2
            };

            var method = new Method
            {
                MethodName = "TestMethod",
                StartLine = 1,
                EndLine = 20,
                Class = classEntity
            };

            // Act
            var form = new EditForm("Methods", method);

            // Assert
            var textBox1 = form.Controls["textBox1"] as TextBox;
            var textBox2 = form.Controls["textBox2"] as TextBox;
            var textBox3 = form.Controls["textBox3"] as TextBox;
            var textBox4 = form.Controls["textBox4"] as TextBox;

            Assert.NotNull(textBox1);
            Assert.NotNull(textBox2);
            Assert.NotNull(textBox3);
            Assert.NotNull(textBox4);

            Assert.Equal("TestMethod", textBox1.Text);
            Assert.Equal("1", textBox2.Text);
            Assert.Equal("20", textBox3.Text);
            Assert.Equal("TestClass", textBox4.Text);
        }

        
    }
}
