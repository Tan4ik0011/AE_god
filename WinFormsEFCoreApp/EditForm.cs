using System;
using System.Windows.Forms;

namespace WinFormsEFCoreApp
{
    public partial class EditForm : Form
    {
        private readonly object _record;
        private readonly string _table;

        public bool IsSaved { get; private set; }

        public EditForm(string table, object record)
        {
            InitializeComponent();
            _table = table;
            _record = record;

            
            if (_record != null)
            {
                if (_table == "Files")
                {
                    var file = _record as File;
                    textBox1.Text = file.FileName;
                    textBox2.Text = file.FilePath;
                    textBox3.Text = file.Lines.ToString();
                    textBox4.Text = file.AnalysisDate.ToString("yyyy-MM-dd");
                }
                else if (_table == "Classes")
                {
                    var classEntity = _record as Class;
                    textBox1.Text = classEntity.ClassName;
                    textBox2.Text = classEntity.StartLine.ToString();
                    textBox3.Text = classEntity.EndLine.ToString();
                    textBox4.Text = classEntity.MethodsCount.ToString();
                }
                else if (_table == "Methods")
                {
                    var method = _record as Method;
                    textBox1.Text = method.MethodName;
                    textBox2.Text = method.StartLine.ToString();
                    textBox3.Text = method.EndLine.ToString();
                    textBox4.Text = method.Class.ClassName;
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (_table == "Files")
            {
                var file = _record as File;
                file.FileName = textBox1.Text;
                file.FilePath = textBox2.Text;
                file.Lines = int.Parse(textBox3.Text);
                file.AnalysisDate = DateTime.Parse(textBox4.Text);
                IsSaved = true;
            }
            else if (_table == "Classes")
            {
                var classEntity = _record as Class;
                classEntity.ClassName = textBox1.Text;
                classEntity.StartLine = int.Parse(textBox2.Text);
                classEntity.EndLine = int.Parse(textBox3.Text);
                classEntity.MethodsCount = int.Parse(textBox4.Text);
                IsSaved = true;
            }
            else if (_table == "Methods")
            {
                var method = _record as Method;
                method.MethodName = textBox1.Text;
                method.StartLine = int.Parse(textBox2.Text);
                method.EndLine = int.Parse(textBox3.Text);
                method.Class.ClassName = textBox4.Text; 
                IsSaved = true;
            }

            Close();
        }
    }
}
