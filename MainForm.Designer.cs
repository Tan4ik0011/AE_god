namespace WinFormsEFCoreApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            AnalyzeButton = new Button();
            DeleteButton = new Button();
            treeView1 = new TreeView();
            ReadButton = new Button();
            listBoxTables = new ListBox();
            ShowButton = new Button();
            AddButton = new Button();
            EditButton = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 12);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(993, 665);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // AnalyzeButton
            // 
            AnalyzeButton.Location = new Point(1049, 28);
            AnalyzeButton.Name = "AnalyzeButton";
            AnalyzeButton.Size = new Size(94, 29);
            AnalyzeButton.TabIndex = 1;
            AnalyzeButton.Text = "Analyze";
            AnalyzeButton.UseVisualStyleBackColor = true;
            AnalyzeButton.Click += AnalyzeButton_Click;
            // 
            // DeleteButton
            // 
            DeleteButton.Location = new Point(1049, 150);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new Size(94, 29);
            DeleteButton.TabIndex = 2;
            DeleteButton.Text = "Delete";
            DeleteButton.UseVisualStyleBackColor = true;
            DeleteButton.Click += DeleteButton_Click;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(1024, 254);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(454, 423);
            treeView1.TabIndex = 4;
            // 
            // ReadButton
            // 
            ReadButton.Location = new Point(1049, 185);
            ReadButton.Name = "ReadButton";
            ReadButton.Size = new Size(94, 29);
            ReadButton.TabIndex = 5;
            ReadButton.Text = "Read";
            ReadButton.UseVisualStyleBackColor = true;
            ReadButton.Click += ReadButton_Click;
            // 
            // listBoxTables
            // 
            listBoxTables.FormattingEnabled = true;
            listBoxTables.Location = new Point(1328, 12);
            listBoxTables.Name = "listBoxTables";
            listBoxTables.Size = new Size(150, 104);
            listBoxTables.TabIndex = 6;
            // 
            // ShowButton
            // 
            ShowButton.Location = new Point(1384, 122);
            ShowButton.Name = "ShowButton";
            ShowButton.Size = new Size(94, 29);
            ShowButton.TabIndex = 7;
            ShowButton.Text = "Show";
            ShowButton.UseVisualStyleBackColor = true;
            ShowButton.Click += ShowButton_Click;
            // 
            // AddButton
            // 
            AddButton.Location = new Point(1384, 157);
            AddButton.Name = "AddButton";
            AddButton.Size = new Size(94, 29);
            AddButton.TabIndex = 8;
            AddButton.Text = "Add";
            AddButton.UseVisualStyleBackColor = true;
            AddButton.Click += AddButton_Click;
            // 
            // EditButton
            // 
            EditButton.Location = new Point(1384, 219);
            EditButton.Name = "EditButton";
            EditButton.Size = new Size(94, 29);
            EditButton.TabIndex = 9;
            EditButton.Text = "Edit";
            EditButton.UseVisualStyleBackColor = true;
            EditButton.Click += EditButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1490, 689);
            Controls.Add(EditButton);
            Controls.Add(AddButton);
            Controls.Add(ShowButton);
            Controls.Add(listBoxTables);
            Controls.Add(ReadButton);
            Controls.Add(treeView1);
            Controls.Add(DeleteButton);
            Controls.Add(AnalyzeButton);
            Controls.Add(dataGridView1);
            Name = "MainForm";
            Text = "MainForm";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private Button AnalyzeButton;
        private Button DeleteButton;
        private TreeView treeView1;
        private Button ReadButton;
        private ListBox listBoxTables;
        private Button ShowButton;
        private Button AddButton;
        private Button EditButton;
    }
}
