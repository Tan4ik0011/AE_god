using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsEFCoreApp
{
    public partial class EditForm : Form
    {
        private readonly string _tableName;
        private readonly object _record;

        public bool IsSaved { get; private set; } = false; // Указывает, были ли сохранены изменения

        public EditForm(string tableName, object record)
        {
            InitializeComponent();
            _tableName = tableName;
            _record = record;
            PopulateFields();
        }

        private void PopulateFields()
        {
            switch (_tableName)
            {
                case "Files":
                    var file = (File)_record;
                    textBox1.Text = file.FileName;
                    textBox2.Text = file.FilePath;
                    textBox3.Text = file.Lines.ToString();
                    textBox4.Text = file.AnalysisDate.ToString();
                    break;

                case "Classes":
                    var classEntity = (Class)_record;
                    textBox1.Text = classEntity.ClassName;
                    textBox2.Text = classEntity.StartLine.ToString();
                    textBox3.Text = classEntity.EndLine.ToString();
                    break;

                case "Methods":
                    var method = (Method)_record;
                    textBox1.Text = method.MethodName;
                    textBox2.Text = method.StartLine.ToString();
                    textBox3.Text = method.EndLine.ToString();
                    break;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            switch (_tableName)
            {
                case "Files":
                    var file = (File)_record;
                    file.FileName = textBox1.Text;
                    file.FilePath = textBox2.Text;
                    file.Lines = int.Parse(textBox3.Text);
                    file.AnalysisDate = DateTime.Parse(textBox4.Text);
                    break;

                case "Classes":
                    var classEntity = (Class)_record;
                    classEntity.ClassName = textBox1.Text;
                    classEntity.StartLine = int.Parse(textBox2.Text);
                    classEntity.EndLine = int.Parse(textBox3.Text);
                    break;

                case "Methods":
                    var method = (Method)_record;
                    method.MethodName = textBox1.Text;
                    method.StartLine = int.Parse(textBox2.Text);
                    method.EndLine = int.Parse(textBox3.Text);
                    break;
            }

            IsSaved = true; // Указываем, что изменения сохранены
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

