using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using endMension.Models;
using System.IO;
using Newtonsoft.Json;

namespace endMension
{
    public partial class Form1 : Form
    {
        public List<FileTag> FileTags { get; set; } = new List<FileTag>();
        public List<Folder> Folders { get; set; } = new List<Folder>();
        public Form1()
        {
            InitializeComponent();

            comboDebug.Items.Add("Tags Save");
            comboDebug.Items.Add("Tags Read");
            comboDebug.Items.Add("Folders Read");
            comboDebug.Items.Add("Folders Save");
            comboDebug.Items.Add("debug3");

            if (File.Exists("FileTags.json"))
            {
                var fileTagsRaw = File.ReadAllText("FileTags.json");
                FileTags = JsonConvert.DeserializeObject<List<FileTag>>(fileTagsRaw);

                foreach(var tagSetName in FileTags.Select(x => x.TagSetName).Distinct())
                {
                    comboLoadTagSet.Items.Add(tagSetName);
                }
            }
            if (File.Exists("Folders.json"))
            {
                var foldersRaw = File.ReadAllText("Folders.json");
                Folders = JsonConvert.DeserializeObject<List<Folder>>(foldersRaw);
            }
            foreach (var folder in Folders) 
            {
                comboFolder.Items.Add(folder.FilePath);
            }
            
        }

        public List<Button> TagButtons = new List<Button>();
        private void DrawTagButtons()
        {
            foreach (var button in TagButtons)
            {
                button.Enabled = false;
                button.Dispose();
            }

            btnTagPlaceHolder.Visible = false;
            var x = btnTagPlaceHolder.Location.X;
            var y = btnTagPlaceHolder.Location.Y;
            foreach (var tag in FileTags)
            {
                y += 50;
                var tagbutton = new Button();
                TagButtons.Add(tagbutton);
                tagbutton.Text = tag.TagName;
                tagbutton.Name = tag.TagName;
                tagbutton.Location = new Point(x, y);
                tagbutton.Size = new Size(100, 50);
                tagbutton.Visible = true;
                tagbutton.BringToFront();

                tagbutton.MouseDown += new MouseEventHandler(textbox_MouseDown);
                tagbutton.MouseMove += new MouseEventHandler(textbox_MouseMove);
                tagbutton.MouseUp += new MouseEventHandler(textbox_MouseUp);
                tag.TagButton = tagbutton;
                this.Controls.Add(tagbutton);

                var tagButtonLinker = new Button();
                TagButtons.Add(tagButtonLinker);
                tagButtonLinker.Text = "->";
                tagButtonLinker.Name = tag.TagName;
                tagButtonLinker.Location = new Point(x + 95, y);
                tagButtonLinker.Size = new Size(30, 50);
                tagButtonLinker.Visible = true;
                tagButtonLinker.BringToFront();
                tagButtonLinker.MouseDown += new MouseEventHandler(textbox_MouseDown);
                tagButtonLinker.MouseMove += new MouseEventHandler(textbox_MouseMove);
                tagButtonLinker.MouseUp += new MouseEventHandler(textbox_MouseUp);
                tag.TagButtonLinker = tagButtonLinker;
                this.Controls.Add(tagButtonLinker);

                tagbutton.Click += new EventHandler(Tag_Click);
                tagButtonLinker.Click += new EventHandler(TagLink_Click);
                tag.TagButtonLinker = tagButtonLinker;
            }
        }

        private void TagLink_Click(object sender, EventArgs e)
        {
            //if (FileTags.Any(x => x.TagName == ((System.Windows.Forms.Button)sender).Name))
            //{
            //    if (((System.Windows.Forms.Button)sender).BackColor != Color.LimeGreen)
            //    {
            //        ((System.Windows.Forms.Button)sender).BackColor = Color.LimeGreen;

            //        FileTags.First(x => x.TagName == ((System.Windows.Forms.Button)sender).Text).Enabled = true;
            //    }
            //    else
            //    {
            //        ((System.Windows.Forms.Button)sender).BackColor = Color.LightGray;
            //        FileTags.First(x => x.TagName == ((System.Windows.Forms.Button)sender).Text).Enabled = false;
            //    }
            //}
            txtDebug.Text = JsonConvert.SerializeObject(FileTags, Formatting.Indented);
        }

        private void Tag_Click(object sender, EventArgs e)
        {
            if ( FileTags.Any(x=>x.TagName == ((System.Windows.Forms.Button)sender).Name))
            {
                if(((System.Windows.Forms.Button)sender).BackColor != Color.LimeGreen)
                {                
                    ((System.Windows.Forms.Button)sender).BackColor = Color.LimeGreen;

                    FileTags.First(x => x.TagName == ((System.Windows.Forms.Button)sender).Text).Enabled = true;
                }
                else
                {
                    ((System.Windows.Forms.Button)sender).BackColor = Color.LightGray;
                    FileTags.First(x => x.TagName == ((System.Windows.Forms.Button)sender).Text).Enabled = false;
                }
            }
            txtDebug.Text = JsonConvert.SerializeObject(FileTags, Formatting.Indented);
        }


        private void btnNewTag_Click(object sender, EventArgs e)
        {
            var last = FileTags.LastOrDefault();
            var tag = new FileTag() { TagName = txtNewTag.Text, Order = last?.Order + 1 ?? 1};
            
            if (!FileTags.Any(x=>x.TagName == tag.TagName))
            {
                FileTags.Add(tag);
            }

            txtDebug.Text = JsonConvert.SerializeObject(FileTags, Formatting.Indented);
            File.WriteAllText("FileTags.json", JsonConvert.SerializeObject(FileTags, Formatting.Indented));
            DrawTagButtons();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            var option = comboDebug.SelectedItem.ToString();
            if (option == "Tags Save")
            {
                File.WriteAllText("FileTags.Json", txtDebug.Text);
                var fileTagsRaw = File.ReadAllText("FileTags.json");
                FileTags = JsonConvert.DeserializeObject<List<FileTag>>(fileTagsRaw);
                DrawTagButtons();
            }
            if(option == "Tags Read")
            {
                txtDebug.Text = JsonConvert.SerializeObject(FileTags, Formatting.Indented);
            }
            if(option == "Folders Read")
            {
                txtDebug.Text = JsonConvert.SerializeObject(Folders, Formatting.Indented);
            }
            if (option == "Folders Save")
            {
                File.WriteAllText("Folders.json", txtDebug.Text);
                var folders = File.ReadAllText("Folders.json");
                Folders = JsonConvert.DeserializeObject<List<Folder>>(folders);

                txtDebug.Text = JsonConvert.SerializeObject(Folders, Formatting.Indented);
            }
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    var folder = new Folder() { FilePath = fbd.SelectedPath };                    
                    if (!Folders.Any(x => x.FilePath == folder.FilePath))
                    {
                        Folders.Add(folder);
                        File.WriteAllText("Folders.json", JsonConvert.SerializeObject(Folders, Formatting.Indented));
                    }                    
                }
            }
            txtDebug.Text = JsonConvert.SerializeObject(Folders, Formatting.Indented);
            UpdateFolderList();
        }

        private void UpdateFolderList()
        {
            for(int i = comboFolder.Items.Count; i > 0; i--)
            {
                comboFolder.Items.RemoveAt(i-1);
            }

            foreach (var folder in Folders)
            {
                comboFolder.Items.Add(folder.FilePath);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboDebug_SelectedIndexChanged(object sender, EventArgs e)
        {
            var option = comboDebug.SelectedItem.ToString();
           
            if (option == "Tags Read")
            {
                txtDebug.Text = JsonConvert.SerializeObject(FileTags, Formatting.Indented);
            }
            if (option == "Folders Read")
            {
                txtDebug.Text = JsonConvert.SerializeObject(Folders, Formatting.Indented);
            }          
            DrawTagButtons();
        }

        private void comboFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            var folderPath = comboFolder.SelectedItem.ToString();
            var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                txtFileOutput.Text = "No files found";
                return;
            }
            var sb = new StringBuilder();
            foreach(var file in files)
            {
                sb.Append(file + System.Environment.NewLine);
               
            }
            txtFileOutput.Text = sb.ToString();
            DrawTagButtons();
        }

        private Control activeControl;
        private Point previousLocation;

        private void button1_Click(object sender, EventArgs e)
        {
            //var textbox = new TextBox();
            //textbox.Location = new Point(50, 50);
            //textbox.MouseDown += new MouseEventHandler(textbox_MouseDown);
            //textbox.MouseMove += new MouseEventHandler(textbox_MouseMove);
            //textbox.MouseUp += new MouseEventHandler(textbox_MouseUp);

            //this.Controls.Add(textbox);
        }
        void textbox_MouseDown(object sender, MouseEventArgs e)
        {
            activeControl = sender as Control;
            previousLocation = e.Location;
            Cursor = Cursors.Hand;
        }

        void textbox_MouseMove(object sender, MouseEventArgs e)
        {
            if (activeControl == null || activeControl != sender)
                return;

            var location = activeControl.Location;
            location.Offset(e.Location.X - previousLocation.X, e.Location.Y - previousLocation.Y);
            activeControl.Location = location;
        }

        void textbox_MouseUp(object sender, MouseEventArgs e)
        {
            activeControl = null;
            Cursor = Cursors.Default;
        }

        private void btnSaveTagging_Click(object sender, EventArgs e)
        {
            foreach(var tag in FileTags)
            {
                tag.TagSetName = txtTagSet.Text; 
            }
            File.WriteAllText("FileTags.json", JsonConvert.SerializeObject(FileTags, Formatting.Indented));
            txtDebug.Text = JsonConvert.SerializeObject(FileTags, Formatting.Indented);
        }

        private void comboLoadTagSet_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
