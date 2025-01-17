using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Windows.Forms;
using static WinFormsEFCoreApp.Program;

namespace WinFormsEFCoreApp
{
    public partial class MainForm : Form
    {
        private readonly AppDbContext _context;

        public MainForm()
        {
            InitializeComponent();
            _context = new AppDbContext();



        }

        private void LoadFiles()
        {
            var files = _context.Files
        .Include(f => f.Classes)
        .ThenInclude(c => c.Methods)
        .ToList();

            dataGridView1.DataSource = files.Select(f => new
            {
                f.FileId,
                f.FileName,
                f.FilePath,
                f.Lines,
                f.AnalysisDate,
                ClassCount = f.Classes.Count,
                MethodCount = f.Classes.Sum(c => c.Methods.Count)
            }).ToList();
        }



        private void MainForm_Load(object sender, EventArgs e)
        {
            var files = _context.Files
                .Include(f => f.Classes)  // �������� ��������� �������
                .ThenInclude(c => c.Methods)  // �������� ������� ��� ������� ������
            .ToList();

            dataGridView1.DataSource = files.Select(f => new
            {
                f.FileId,
                f.FileName,
                f.FilePath,
                f.Lines,
                f.AnalysisDate,
                ClassCount = f.Classes.Count,
                MethodCount = f.Classes.Sum(c => c.Methods.Count)
            }).ToList();

            listBoxTables.Items.Add("Files");
            listBoxTables.Items.Add("Classes");
            listBoxTables.Items.Add("Methods");
        }

        private void dataGridView1_CellContentClick(object sender, EventArgs e)
        {

        }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            var directoryPath = @"C:\Users\user\Desktop\AE\WinFormsEFCoreApp"; // ������� ���� � ������ �������
            var analyzer = new Analyzer(_context); // �������� �������� ���� ������
            analyzer.AnalyzeDirectory(directoryPath); // ������ �������
        }
        //DELETE
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && listBoxTables.SelectedItem != null)
            {
                // �������� ������� �������
                var selectedTable = listBoxTables.SelectedItem.ToString();

                // ������� ������ � ����������� �� ��������� �������
                switch (selectedTable)
                {
                    case "Files":
                        var fileId = (int)dataGridView1.SelectedRows[0].Cells["FileId"].Value;
                        DeleteFile(fileId);
                        LoadFiles(); // �������� ������ � DataGridView
                        break;

                    case "Classes":
                        var classId = (int)dataGridView1.SelectedRows[0].Cells["ClassId"].Value;
                        DeleteClass(classId);
                        LoadClassesTable(); // �������� ������ � DataGridView
                        break;

                    case "Methods":
                        var methodId = (int)dataGridView1.SelectedRows[0].Cells["MethodId"].Value;
                        DeleteMethod(methodId);
                        LoadMethodsTable(); // �������� ������ � DataGridView
                        break;

                    default:
                        MessageBox.Show("����������� �������. �������� ������� ��� ��������.");
                        break;
                }
            }
            else
            {
                MessageBox.Show("����������, �������� ������ ��� ��������.");
            }
        }

        private void DeleteFile(int fileId)
        {
            var file = _context.Files
                .Include(f => f.Classes) // ��������� ��������� ������
                .ThenInclude(c => c.Methods) // ��������� ������
                .FirstOrDefault(f => f.FileId == fileId);

            if (file != null)
            {
                // ������� ��� ��������� ��������
                _context.Methods.RemoveRange(file.Classes.SelectMany(c => c.Methods));
                _context.Classes.RemoveRange(file.Classes);
                _context.Files.Remove(file);
                _context.SaveChanges();
            }
            else
            {
                MessageBox.Show("���� �� ������.");
            }
        }

        private void DeleteClass(int classId)
        {
            var classEntity = _context.Classes
                .Include(c => c.Methods) // ��������� ��������� ������
                .FirstOrDefault(c => c.ClassId == classId);

            if (classEntity != null)
            {
                // ������� ��������� ������
                _context.Methods.RemoveRange(classEntity.Methods);
                _context.Classes.Remove(classEntity);
                _context.SaveChanges();
            }
            else
            {
                MessageBox.Show("����� �� ������.");
            }
        }


        private void DeleteMethod(int methodId)
        {
            var method = _context.Methods.FirstOrDefault(m => m.MethodId == methodId);

            if (method != null)
            {
                _context.Methods.Remove(method);
                _context.SaveChanges();
            }
            else
            {
                MessageBox.Show("����� �� ������.");
            }
        }



        //READ
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ReadButton_Click(object sender, EventArgs e)
        {
            LoadTreeView();
        }

        private void LoadTreeView()
        {
            treeView1.Nodes.Clear();

            var files = _context.Files
                .Include(f => f.Classes)
                .ThenInclude(c => c.Methods)
                .ToList();

            foreach (var file in files)
            {
                var fileNode = new TreeNode($"File: {file.FileName} ({file.FilePath})");

                foreach (var classEntity in file.Classes)
                {
                    var classNode = new TreeNode($"Class: {classEntity.ClassName} (Lines {classEntity.StartLine}-{classEntity.EndLine})");

                    foreach (var method in classEntity.Methods)
                    {
                        var methodNode = new TreeNode($"Method: {method.MethodName} (Lines {method.StartLine}-{method.EndLine})");
                        classNode.Nodes.Add(methodNode);
                    }

                    fileNode.Nodes.Add(classNode);
                }

                treeView1.Nodes.Add(fileNode);
            }

            treeView1.ExpandAll();
        }
        //SHOW
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ShowButton_Click(object sender, EventArgs e)
        {
            // ���������, ��� ������ ������� � ListBox
            if (listBoxTables.SelectedItem == null)
            {
                MessageBox.Show("�������� �������.");
                return;
            }

            // �������� �������� ��������� �������
            var selectedTable = listBoxTables.SelectedItem.ToString();

            // ��������� ������ � ����������� �� ��������� �������
            switch (selectedTable)
            {
                case "Files":
                    LoadFilesTable();
                    break;
                case "Classes":
                    LoadClassesTable();
                    break;
                case "Methods":
                    LoadMethodsTable();
                    break;
                default:
                    MessageBox.Show("����������� �������.");
                    break;
            }
        }


        private void LoadClassesTable()
        {
            var classes = _context.Classes
                .Include(c => c.File) // ���� ����� ��������� ������
                .ToList();

            dataGridView1.DataSource = classes.Select(c => new
            {
                c.ClassId,
                c.ClassName,
                FileName = c.File.FileName,
                c.StartLine,
                c.EndLine,
                c.MethodsCount
            }).ToList();
        }

        private void LoadFilesTable()
        {
            var files = _context.Files
                .Include(f => f.Classes) // ���� ����� ��������� ������
                .ToList();

            dataGridView1.DataSource = files.Select(f => new
            {
                f.FileId,
                f.FileName,
                f.FilePath,
                f.Lines,
                f.AnalysisDate,
                ClassCount = f.Classes.Count
            }).ToList();
        }

        private void LoadMethodsTable()
        {
            var methods = _context.Methods
                .Include(m => m.Class) // ���� ����� ��������� ������
                .ToList();

            dataGridView1.DataSource = methods.Select(m => new
            {
                m.MethodId,
                m.MethodName,
                ClassName = m.Class.ClassName,
                m.StartLine,
                m.EndLine
            }).ToList();
        }
        //////////////////////////////////////////////////////////////////////////////////////////
        ///

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && listBoxTables.SelectedItem != null)
            {
                var selectedTable = listBoxTables.SelectedItem.ToString();

                switch (selectedTable)
                {
                    case "Files":
                        UpdateFile(e.RowIndex);
                        break;

                    case "Classes":
                        UpdateClass(e.RowIndex);
                        break;

                    case "Methods":
                        UpdateMethod(e.RowIndex);
                        break;

                    default:
                        MessageBox.Show("����������� ������� ��� ��������������.");
                        break;
                }
            }
        }

        private void UpdateFile(int rowIndex)
        {
            var fileId = (int)dataGridView1.Rows[rowIndex].Cells["FileId"].Value;
            var file = _context.Files.FirstOrDefault(f => f.FileId == fileId);

            if (file != null)
            {
                file.FileName = dataGridView1.Rows[rowIndex].Cells["FileName"].Value.ToString();
                file.FilePath = dataGridView1.Rows[rowIndex].Cells["FilePath"].Value.ToString();
                file.Lines = (int)dataGridView1.Rows[rowIndex].Cells["Lines"].Value;
                file.AnalysisDate = DateTime.Parse(dataGridView1.Rows[rowIndex].Cells["AnalysisDate"].Value.ToString());

                _context.SaveChanges();
            }
        }

        private void UpdateClass(int rowIndex)
        {
            var classId = (int)dataGridView1.Rows[rowIndex].Cells["ClassId"].Value;
            var classEntity = _context.Classes.FirstOrDefault(c => c.ClassId == classId);

            if (classEntity != null)
            {
                classEntity.ClassName = dataGridView1.Rows[rowIndex].Cells["ClassName"].Value.ToString();
                classEntity.StartLine = (int)dataGridView1.Rows[rowIndex].Cells["StartLine"].Value;
                classEntity.EndLine = (int)dataGridView1.Rows[rowIndex].Cells["EndLine"].Value;
                classEntity.MethodsCount = (int)dataGridView1.Rows[rowIndex].Cells["MethodsCount"].Value;

                _context.SaveChanges();
            }
        }

        private void UpdateMethod(int rowIndex)
        {
            var methodId = (int)dataGridView1.Rows[rowIndex].Cells["MethodId"].Value;
            var method = _context.Methods.FirstOrDefault(m => m.MethodId == methodId);

            if (method != null)
            {
                method.MethodName = dataGridView1.Rows[rowIndex].Cells["MethodName"].Value.ToString();
                method.StartLine = (int)dataGridView1.Rows[rowIndex].Cells["StartLine"].Value;
                method.EndLine = (int)dataGridView1.Rows[rowIndex].Cells["EndLine"].Value;

                _context.SaveChanges();
            }
        }


        //ADD
        //////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void AddButton_Click(object sender, EventArgs e)
        {
            if (listBoxTables.SelectedItem == null)
            {
                MessageBox.Show("�������� ������� ��� ���������� ������.");
                return;
            }

            var selectedTable = listBoxTables.SelectedItem.ToString();

            switch (selectedTable)
            {
                case "Files":
                    AddFile();
                    break;

                case "Classes":
                    AddClass();
                    break;

                case "Methods":
                    AddMethod();
                    break;

                default:
                    MessageBox.Show("����������� �������.");
                    break;
            }

            // ��������� DataGridView ����� ����������
            ShowButton_Click(null, null);
        }

        private void AddFile()
        {
            var file = new File
            {
                FileName = "����� ����",
                FilePath = "���� � �����",
                Lines = 0,
                AnalysisDate = DateTime.Now
            };

            _context.Files.Add(file);
            _context.SaveChanges();

            MessageBox.Show($"���� '{file.FileName}' ������� ��������!");
        }


        private void AddClass()
        {
            var files = _context.Files.ToList();
            if (files.Count == 0)
            {
                MessageBox.Show("������� �������� ���� �� ���� ����.");
                return;
            }

            // ��������� ����� � ������� �����
            var classEntity = new Class
            {
                FileId = files[0].FileId, // ID ������� ����� (����� ������� ����� ����� UI)
                ClassName = "����� �����",
                StartLine = 1,
                EndLine = 1,
                MethodsCount = 0
            };

            _context.Classes.Add(classEntity);
            _context.SaveChanges();

            MessageBox.Show($"����� '{classEntity.ClassName}' ������� ��������!");
        }

        private void AddMethod()
        {
            var classes = _context.Classes.ToList();
            if (classes.Count == 0)
            {
                MessageBox.Show("������� �������� ���� �� ���� �����.");
                return;
            }

            // ��������� ����� � ������� ������
            var method = new Method
            {
                ClassId = classes[0].ClassId, // ID ������� ������ (����� ������� ����� ����� UI)
                MethodName = "����� �����",
                StartLine = 1,
                EndLine = 1
            };

            _context.Methods.Add(method);
            _context.SaveChanges();

            MessageBox.Show($"����� '{method.MethodName}' ������� ��������!");
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("�������� ������ ��� ��������������.");
                return;
            }

            var selectedTable = listBoxTables.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("�������� �������.");
                return;
            }

            object record = null;

            switch (selectedTable)
            {
                case "Files":
                    var fileId = (int)dataGridView1.SelectedRows[0].Cells["FileId"].Value;
                    record = _context.Files.FirstOrDefault(f => f.FileId == fileId);
                    break;

                case "Classes":
                    var classId = (int)dataGridView1.SelectedRows[0].Cells["ClassId"].Value;
                    record = _context.Classes.FirstOrDefault(c => c.ClassId == classId);
                    break;

                case "Methods":
                    var methodId = (int)dataGridView1.SelectedRows[0].Cells["MethodId"].Value;
                    record = _context.Methods.FirstOrDefault(m => m.MethodId == methodId);
                    break;
            }

            if (record == null)
            {
                MessageBox.Show("������ �� �������.");
                return;
            }

            var editForm = new EditForm(selectedTable, record);
            editForm.ShowDialog();

            if (editForm.IsSaved)
            {
                _context.SaveChanges(); // ��������� ��������� � ���� ������
                ShowButton_Click(null, null); // ��������� DataGridView
                MessageBox.Show("������ ������� ���������!");
            }
        }
    }

}
