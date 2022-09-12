
namespace fileui
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnNewTag = new System.Windows.Forms.Button();
            this.btnAddFolder = new System.Windows.Forms.Button();
            this.btnTagPlaceHolder = new System.Windows.Forms.Button();
            this.txtNewTag = new System.Windows.Forms.TextBox();
            this.txtDebug = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.comboDebug = new System.Windows.Forms.ComboBox();
            this.comboFolder = new System.Windows.Forms.ComboBox();
            this.txtFileOutput = new System.Windows.Forms.TextBox();
            this.btnSaveTagging = new System.Windows.Forms.Button();
            this.txtTagSet = new System.Windows.Forms.TextBox();
            this.btnLoadTagSet = new System.Windows.Forms.Button();
            this.comboLoadTagSet = new System.Windows.Forms.ComboBox();
            this.btnClearLines = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNewTag
            // 
            this.btnNewTag.Location = new System.Drawing.Point(17, 16);
            this.btnNewTag.Margin = new System.Windows.Forms.Padding(4);
            this.btnNewTag.Name = "btnNewTag";
            this.btnNewTag.Size = new System.Drawing.Size(116, 28);
            this.btnNewTag.TabIndex = 0;
            this.btnNewTag.Text = "New Tag";
            this.btnNewTag.UseVisualStyleBackColor = true;
            this.btnNewTag.Click += new System.EventHandler(this.btnNewTag_Click);
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Location = new System.Drawing.Point(17, 53);
            this.btnAddFolder.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(116, 28);
            this.btnAddFolder.TabIndex = 1;
            this.btnAddFolder.Text = "Add Folder";
            this.btnAddFolder.UseVisualStyleBackColor = true;
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // btnTagPlaceHolder
            // 
            this.btnTagPlaceHolder.Location = new System.Drawing.Point(16, 160);
            this.btnTagPlaceHolder.Margin = new System.Windows.Forms.Padding(4);
            this.btnTagPlaceHolder.Name = "btnTagPlaceHolder";
            this.btnTagPlaceHolder.Size = new System.Drawing.Size(100, 28);
            this.btnTagPlaceHolder.TabIndex = 2;
            this.btnTagPlaceHolder.Text = "place holder";
            this.btnTagPlaceHolder.UseVisualStyleBackColor = true;
            // 
            // txtNewTag
            // 
            this.txtNewTag.Location = new System.Drawing.Point(141, 15);
            this.txtNewTag.Margin = new System.Windows.Forms.Padding(4);
            this.txtNewTag.Name = "txtNewTag";
            this.txtNewTag.Size = new System.Drawing.Size(347, 22);
            this.txtNewTag.TabIndex = 3;
            // 
            // txtDebug
            // 
            this.txtDebug.Location = new System.Drawing.Point(1111, 46);
            this.txtDebug.Margin = new System.Windows.Forms.Padding(4);
            this.txtDebug.Multiline = true;
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.Size = new System.Drawing.Size(351, 672);
            this.txtDebug.TabIndex = 4;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(1361, 10);
            this.btnExecute.Margin = new System.Windows.Forms.Padding(4);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(100, 28);
            this.btnExecute.TabIndex = 5;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // comboDebug
            // 
            this.comboDebug.FormattingEnabled = true;
            this.comboDebug.Location = new System.Drawing.Point(1109, 12);
            this.comboDebug.Margin = new System.Windows.Forms.Padding(4);
            this.comboDebug.Name = "comboDebug";
            this.comboDebug.Size = new System.Drawing.Size(231, 24);
            this.comboDebug.TabIndex = 7;
            this.comboDebug.SelectedIndexChanged += new System.EventHandler(this.comboDebug_SelectedIndexChanged);
            // 
            // comboFolder
            // 
            this.comboFolder.FormattingEnabled = true;
            this.comboFolder.Location = new System.Drawing.Point(141, 55);
            this.comboFolder.Margin = new System.Windows.Forms.Padding(4);
            this.comboFolder.Name = "comboFolder";
            this.comboFolder.Size = new System.Drawing.Size(347, 24);
            this.comboFolder.TabIndex = 8;
            this.comboFolder.SelectedIndexChanged += new System.EventHandler(this.comboFolder_SelectedIndexChanged);
            // 
            // txtFileOutput
            // 
            this.txtFileOutput.Location = new System.Drawing.Point(543, 14);
            this.txtFileOutput.Margin = new System.Windows.Forms.Padding(4);
            this.txtFileOutput.Multiline = true;
            this.txtFileOutput.Name = "txtFileOutput";
            this.txtFileOutput.Size = new System.Drawing.Size(559, 594);
            this.txtFileOutput.TabIndex = 9;
            // 
            // btnSaveTagging
            // 
            this.btnSaveTagging.Location = new System.Drawing.Point(19, 89);
            this.btnSaveTagging.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveTagging.Name = "btnSaveTagging";
            this.btnSaveTagging.Size = new System.Drawing.Size(115, 28);
            this.btnSaveTagging.TabIndex = 10;
            this.btnSaveTagging.Text = "Save Tag Set";
            this.btnSaveTagging.UseVisualStyleBackColor = true;
            this.btnSaveTagging.Click += new System.EventHandler(this.btnSaveTagging_Click);
            // 
            // txtTagSet
            // 
            this.txtTagSet.Location = new System.Drawing.Point(141, 92);
            this.txtTagSet.Margin = new System.Windows.Forms.Padding(4);
            this.txtTagSet.Name = "txtTagSet";
            this.txtTagSet.Size = new System.Drawing.Size(347, 22);
            this.txtTagSet.TabIndex = 11;
            // 
            // btnLoadTagSet
            // 
            this.btnLoadTagSet.Location = new System.Drawing.Point(19, 124);
            this.btnLoadTagSet.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadTagSet.Name = "btnLoadTagSet";
            this.btnLoadTagSet.Size = new System.Drawing.Size(115, 28);
            this.btnLoadTagSet.TabIndex = 12;
            this.btnLoadTagSet.Text = "Load Tag Set";
            this.btnLoadTagSet.UseVisualStyleBackColor = true;
            // 
            // comboLoadTagSet
            // 
            this.comboLoadTagSet.FormattingEnabled = true;
            this.comboLoadTagSet.Location = new System.Drawing.Point(141, 127);
            this.comboLoadTagSet.Margin = new System.Windows.Forms.Padding(4);
            this.comboLoadTagSet.Name = "comboLoadTagSet";
            this.comboLoadTagSet.Size = new System.Drawing.Size(347, 24);
            this.comboLoadTagSet.TabIndex = 13;
            this.comboLoadTagSet.SelectedIndexChanged += new System.EventHandler(this.comboLoadTagSet_SelectedIndexChanged);
            // 
            // btnClearLines
            // 
            this.btnClearLines.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.btnClearLines.Location = new System.Drawing.Point(432, 357);
            this.btnClearLines.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearLines.Name = "btnClearLines";
            this.btnClearLines.Size = new System.Drawing.Size(100, 28);
            this.btnClearLines.TabIndex = 14;
            this.btnClearLines.Text = "Clear lines";
            this.btnClearLines.UseVisualStyleBackColor = true;
            this.btnClearLines.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(-4, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1344, 738);
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1472, 736);
            this.Controls.Add(this.btnClearLines);
            this.Controls.Add(this.comboLoadTagSet);
            this.Controls.Add(this.btnLoadTagSet);
            this.Controls.Add(this.txtTagSet);
            this.Controls.Add(this.btnSaveTagging);
            this.Controls.Add(this.txtFileOutput);
            this.Controls.Add(this.comboFolder);
            this.Controls.Add(this.comboDebug);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtDebug);
            this.Controls.Add(this.txtNewTag);
            this.Controls.Add(this.btnTagPlaceHolder);
            this.Controls.Add(this.btnAddFolder);
            this.Controls.Add(this.btnNewTag);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "temple-fileui";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNewTag;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnTagPlaceHolder;
        private System.Windows.Forms.TextBox txtNewTag;
        private System.Windows.Forms.TextBox txtDebug;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.ComboBox comboDebug;
        private System.Windows.Forms.ComboBox comboFolder;
        private System.Windows.Forms.TextBox txtFileOutput;
        private System.Windows.Forms.Button btnSaveTagging;
        private System.Windows.Forms.TextBox txtTagSet;
        private System.Windows.Forms.Button btnLoadTagSet;
        private System.Windows.Forms.ComboBox comboLoadTagSet;
        private System.Windows.Forms.Button btnClearLines;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

