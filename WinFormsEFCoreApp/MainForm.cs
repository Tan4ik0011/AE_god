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

            // ������������� ��������� ���� ������
            _context = context;
            _fileController = fileController;
            _classController = classController;
            _methodController = methodController;
        }

        // ����� ��� ����������� ���� ������
        internal void ShowFiles()
        {
            var files = _fileController.GetFiles();
            dataGridView1.DataSource = files.ToList();
        }

        // ����� ��� ����������� ���� �������
        internal void ShowClasses()
        {
            var classes = _classController.GetClasses();
            dataGridView1.DataSource = classes.ToList();
        }

        // ����� ��� ����������� ���� �������
        internal void ShowMethods()
        {
            var methods = _methodController.GetMethods();
            dataGridView1.DataSource = methods.ToList();
        }

        // ������ "Show" ��� ����������� ������
        private void ShowButton_Click(object sender, EventArgs e)
        {
            string selectedTable = listBoxTables.SelectedItem?.ToString();

            // ���������� ������� � ����������� �� ���������� ������ � ListBox
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
                MessageBox.Show("����������, �������� ������� ��� �����������.");
            }
        }

        // ������ "Analyze" ��� �������
        private async void AnalyzeButton_Click(object sender, EventArgs e)
        {
            try
            {
                // �������� ����������� ���� ��� ������ �����
                using (var folderDialog = new FolderBrowserDialog())
                {
                    var result = folderDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        // �������� ��������� ����
                        string directoryPath = folderDialog.SelectedPath;

                        // �������� ������������� ����������
                        await _fileController.AnalyzeDirectory(directoryPath);

                        MessageBox.Show("������ �������� �������!", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // ������ "Read" ��� ������ ������
        private void ReadButton_Click(object sender, EventArgs e)
        {
            // ���������, ������ �� ������������ ������ � DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // �������� ��������� ����
                var selectedRow = dataGridView1.SelectedRows[0];
                var fileId = (int)selectedRow.Cells[0].Value; // ������������, ��� ������������� ����� � ������ �������

                // �������� ���� �� ���� ������ ����� FileController
                var file = _fileController.GetFileById(fileId);

                if (file != null)
                {
                    
                    treeView1.Nodes.Clear();

                    
                    TreeNode fileNode = new TreeNode(file.FileName)
                    {
                        Tag = $"Path: {file.FilePath}, Lines: {file.Lines}"
                    };
                    fileNode.Nodes.Add(new TreeNode($"Path: {file.FilePath}"));
                    fileNode.Nodes.Add(new TreeNode($"Lines: {file.Lines}"));
                    treeView1.Nodes.Add(fileNode);

                    
                    var classes = _classController.GetClassesByFileId(fileId);

                    foreach (var classEntity in classes)
                    {
                        
                        TreeNode classNode = new TreeNode($"{classEntity.ClassName} (Methods: {classEntity.MethodsCount}, Lines: {classEntity.StartLine}-{classEntity.EndLine})")
                        {
                            Tag = $"Lines: {classEntity.StartLine} - {classEntity.EndLine}"
                        };

                        
                        classNode.Nodes.Add(new TreeNode($"Methods Count: {classEntity.MethodsCount}"));
                        classNode.Nodes.Add(new TreeNode($"Lines: {classEntity.StartLine} - {classEntity.EndLine}"));

                        // �������� ������ ������ ����� MethodController
                        var methods = _methodController.GetMethodsByClassId(classEntity.ClassId);

                        foreach (var method in methods)
                        {
                            // ������� ���� ��� ������
                            TreeNode methodNode = new TreeNode($"{method.MethodName} (Lines: {method.StartLine}-{method.EndLine})")
                            {
                                Tag = $"Start Line: {method.StartLine}, End Line: {method.EndLine}"
                            };

                            // ��������� �������������� ������ � ������
                            methodNode.Nodes.Add(new TreeNode($"Start Line: {method.StartLine}"));
                            methodNode.Nodes.Add(new TreeNode($"End Line: {method.EndLine}"));

                            // ��������� ����� � ���� ������
                            classNode.Nodes.Add(methodNode);
                        }

                        // ��������� ����� � ���� �����
                        fileNode.Nodes.Add(classNode);
                    }

                    // ������������� �������� ����, ����� ������� ��� ������
                    treeView1.ExpandAll();
                }
                else
                {
                    MessageBox.Show("���� �� ������!");
                }
            }
            else
            {
                MessageBox.Show("����������, �������� ���� �� DataGridView.");
            }
        }



        // ������ "Delete" ��� �������� ������
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                var id = row.Cells[0].Value;
                string table = listBoxTables.SelectedItem?.ToString();

                // �������� ������ � ����������� �� ��������� �������
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
                    MessageBox.Show($"������ ��� �������� ������: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("����������, �������� ������ ��� ��������.");
            }
        }

        // ������ "Add" ��� ���������� ����� ������
        private void AddButton_Click(object sender, EventArgs e)
        {
            string table = listBoxTables.SelectedItem?.ToString();

            try
            {
                if (table == "Files")
                {
                    // ���������� ������ ����� � ������������ ����������
                    string defaultFileName = "New File";
                    string defaultFilePath = @"C:\default\path";
                    int defaultLines = 100;

                    _fileController.AddFile(defaultFileName, defaultFilePath, defaultLines);
                    ShowFiles();  // ���������� ����������� ������ ������
                }
                else if (table == "Classes")
                {
                    // ���������� ������ ������ � ������������ ����������
                    string defaultClassName = "New Class";
                    int defaultFileId = 1; // ������: ���������� ID ������� �����
                    int defaultStartLine = 1;
                    int defaultEndLine = 10;
                    int defaultMethodsCount = 0;

                    _classController.AddClass(defaultClassName, defaultFileId, defaultStartLine, defaultEndLine, defaultMethodsCount);
                    ShowClasses();  // ���������� ����������� ������ �������
                }
                else if (table == "Methods")
                {
                    // ���������� ������ ������ � ������������ ����������
                    string defaultMethodName = "New Method";
                    int defaultClassId = 1; // ������: ���������� ID ������� ������
                    int defaultStartLine = 1;
                    int defaultEndLine = 10;

                    _methodController.AddMethod(defaultMethodName, defaultClassId, defaultStartLine, defaultEndLine);
                    ShowMethods();  // ���������� ����������� ������ �������
                }
                else
                {
                    MessageBox.Show("����������, �������� ������� ��� ���������� ������.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ���������� ������: {ex.Message}");
            }
        }

        // ������ "Edit" ��� �������������� ������
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
                    MessageBox.Show($"������ ��� �������������� ������: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("����������, �������� ������ ��� ��������������.");
            }
        }

        // �������� �����
        private void MainForm_Load(object sender, EventArgs e)
        {
            // ������������� ������ ������ ��� ������
            listBoxTables.Items.Add("Files");
            listBoxTables.Items.Add("Classes");
            listBoxTables.Items.Add("Methods");
        }
    }
}
