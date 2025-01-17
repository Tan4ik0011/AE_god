using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WinFormsEFCoreApp.Controllers;

namespace WinFormsEFCoreApp
{
    public partial class MainForm : Form
    {
        private readonly FileController _fileController;
        private readonly ClassController _classController;
        private readonly MethodController _methodController;
        private readonly AppDbContext _context;

        public MainForm(AppDbContext context, FileController fileController, ClassController classController, MethodController methodController)
        {
            InitializeComponent();

            // Инициализация контекста базы данных
            _context = context;
            _fileController = fileController;
            _classController = classController;
            _methodController = methodController;
        }

        // Метод для отображения всех файлов
        private void ShowFiles()
        {
            var files = _fileController.GetFiles();
            dataGridView1.DataSource = files.ToList();
        }

        // Метод для отображения всех классов
        private void ShowClasses()
        {
            var classes = _classController.GetClasses();
            dataGridView1.DataSource = classes.ToList();
        }

        // Метод для отображения всех методов
        private void ShowMethods()
        {
            var methods = _methodController.GetMethods();
            dataGridView1.DataSource = methods.ToList();
        }

        // Кнопка "Show" для отображения данных
        private void ShowButton_Click(object sender, EventArgs e)
        {
            string selectedTable = listBoxTables.SelectedItem?.ToString();

            // Показываем таблицу в зависимости от выбранного пункта в ListBox
            if (selectedTable == "Files")
            {
                ShowFiles();
            }
            else if (selectedTable == "Classes")
            {
                ShowClasses();
            }
            else if (selectedTable == "Methods")
            {
                ShowMethods();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите таблицу для отображения.");
            }
        }

        // Кнопка "Analyze" для анализа
        private async void AnalyzeButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Открытие диалогового окна для выбора папки
                using (var folderDialog = new FolderBrowserDialog())
                {
                    var result = folderDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        // Получаем выбранный путь
                        string directoryPath = folderDialog.SelectedPath;

                        // Начинаем анализировать директорию
                        await _fileController.AnalyzeDirectory(directoryPath);

                        MessageBox.Show("Анализ завершен успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Кнопка "Read" для чтения данных
        private void ReadButton_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрал ли пользователь строку в DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем выбранный файл
                var selectedRow = dataGridView1.SelectedRows[0];
                var fileId = (int)selectedRow.Cells[0].Value; // Предполагаем, что идентификатор файла в первом столбце

                // Получаем файл из базы данных через FileController
                var file = _fileController.GetFileById(fileId);

                if (file != null)
                {
                    // Очищаем TreeView перед добавлением нового содержимого
                    treeView1.Nodes.Clear();

                    // Создаем корневой узел для файла
                    TreeNode fileNode = new TreeNode(file.FileName)
                    {
                        Tag = $"Path: {file.FilePath}, Lines: {file.Lines}"
                    };
                    fileNode.Nodes.Add(new TreeNode($"Path: {file.FilePath}"));
                    fileNode.Nodes.Add(new TreeNode($"Lines: {file.Lines}"));
                    treeView1.Nodes.Add(fileNode);

                    // Получаем классы через ClassController
                    var classes = _classController.GetClassesByFileId(fileId);

                    foreach (var classEntity in classes)
                    {
                        // Создаем узел для класса
                        TreeNode classNode = new TreeNode($"{classEntity.ClassName} (Methods: {classEntity.MethodsCount}, Lines: {classEntity.StartLine}-{classEntity.EndLine})")
                        {
                            Tag = $"Lines: {classEntity.StartLine} - {classEntity.EndLine}"
                        };

                        // Добавляем информацию о классе
                        classNode.Nodes.Add(new TreeNode($"Methods Count: {classEntity.MethodsCount}"));
                        classNode.Nodes.Add(new TreeNode($"Lines: {classEntity.StartLine} - {classEntity.EndLine}"));

                        // Получаем методы класса через MethodController
                        var methods = _methodController.GetMethodsByClassId(classEntity.ClassId);

                        foreach (var method in methods)
                        {
                            // Создаем узел для метода
                            TreeNode methodNode = new TreeNode($"{method.MethodName} (Lines: {method.StartLine}-{method.EndLine})")
                            {
                                Tag = $"Start Line: {method.StartLine}, End Line: {method.EndLine}"
                            };

                            // Добавляем дополнительные данные о методе
                            methodNode.Nodes.Add(new TreeNode($"Start Line: {method.StartLine}"));
                            methodNode.Nodes.Add(new TreeNode($"End Line: {method.EndLine}"));

                            // Добавляем метод в узел класса
                            classNode.Nodes.Add(methodNode);
                        }

                        // Добавляем класс в узел файла
                        fileNode.Nodes.Add(classNode);
                    }

                    // Разворачиваем корневой узел, чтобы увидеть все данные
                    treeView1.ExpandAll();
                }
                else
                {
                    MessageBox.Show("Файл не найден!");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите файл из DataGridView.");
            }
        }



        // Кнопка "Delete" для удаления записи
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                var id = row.Cells[0].Value;
                string table = listBoxTables.SelectedItem?.ToString();

                // Удаление записи в зависимости от выбранной таблицы
                try
                {
                    if (table == "Files")
                    {
                        _fileController.DeleteFile((int)id);
                        ShowFiles();
                    }
                    else if (table == "Classes")
                    {
                        _classController.DeleteClass((int)id);
                        ShowClasses();
                    }
                    else if (table == "Methods")
                    {
                        _methodController.DeleteMethod((int)id);
                        ShowMethods();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
            }
        }

        // Кнопка "Add" для добавления новой записи
        private void AddButton_Click(object sender, EventArgs e)
        {
            string table = listBoxTables.SelectedItem?.ToString();

            try
            {
                if (table == "Files")
                {
                    // Добавление нового файла с стандартными значениями
                    string defaultFileName = "New File";
                    string defaultFilePath = @"C:\default\path";
                    int defaultLines = 100;

                    _fileController.AddFile(defaultFileName, defaultFilePath, defaultLines);
                    ShowFiles();  // Обновление отображения списка файлов
                }
                else if (table == "Classes")
                {
                    // Добавление нового класса с стандартными значениями
                    string defaultClassName = "New Class";
                    int defaultFileId = 1; // Пример: используем ID первого файла
                    int defaultStartLine = 1;
                    int defaultEndLine = 10;
                    int defaultMethodsCount = 0;

                    _classController.AddClass(defaultClassName, defaultFileId, defaultStartLine, defaultEndLine, defaultMethodsCount);
                    ShowClasses();  // Обновление отображения списка классов
                }
                else if (table == "Methods")
                {
                    // Добавление нового метода с стандартными значениями
                    string defaultMethodName = "New Method";
                    int defaultClassId = 1; // Пример: используем ID первого класса
                    int defaultStartLine = 1;
                    int defaultEndLine = 10;

                    _methodController.AddMethod(defaultMethodName, defaultClassId, defaultStartLine, defaultEndLine);
                    ShowMethods();  // Обновление отображения списка методов
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите таблицу для добавления записи.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
            }
        }

        // Кнопка "Edit" для редактирования записи
        private void EditButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                var id = row.Cells[0].Value;
                string table = listBoxTables.SelectedItem?.ToString();

                try
                {
                    if (table == "Files")
                    {
                        var file = _fileController.GetFileById1((int)id);
                        var editForm = new EditForm("Files", file);
                        if (editForm.ShowDialog() == DialogResult.OK && editForm.IsSaved)
                        {
                            ShowFiles();
                        }
                    }
                    else if (table == "Classes")
                    {
                        var classEntity = _classController.GetClassById((int)id);
                        var editForm = new EditForm("Classes", classEntity);
                        if (editForm.ShowDialog() == DialogResult.OK && editForm.IsSaved)
                        {
                            ShowClasses();
                        }
                    }
                    else if (table == "Methods")
                    {
                        var method = _methodController.GetMethodById((int)id);
                        var editForm = new EditForm("Methods", method);
                        if (editForm.ShowDialog() == DialogResult.OK && editForm.IsSaved)
                        {
                            ShowMethods();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при редактировании записи: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования.");
            }
        }

        // Загрузка формы
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Инициализация списка таблиц для выбора
            listBoxTables.Items.Add("Files");
            listBoxTables.Items.Add("Classes");
            listBoxTables.Items.Add("Methods");
        }
    }
}
