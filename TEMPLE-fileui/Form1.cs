using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fileui.Models;
using System.IO;
using Newtonsoft.Json;
using TEMPLE.Services;
using System.Threading;

namespace fileui
{
    public partial class Form1 : Form
    {
        public List<FileTag> FileTags { get; set; } = new List<FileTag>();
        public List<Folder> Folders { get; set; } = new List<Folder>();

        public FileDataService _fileService;
        public Form1()
        {
            InitializeComponent();
            _fileService = new FileDataService("localhost","password");
            var dbInitResult = _fileService.DatabaseInitialise().Result;

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
            pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
        }

       
        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Create a local version of the graphics object for the PictureBox.
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            // Draw a line in the PictureBox.
            g.DrawLine(System.Drawing.Pens.Red, pictureBox1.Left, pictureBox1.Top,
                pictureBox1.Right, pictureBox1.Bottom);

            foreach(var line in GraphicsLines)
            {
                g.DrawLine(System.Drawing.Pens.Red, line[0], line[1], line[2], line[3]);
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
                tagbutton.BringToFront();

                var tagButtonLinker = new Button();
                TagButtons.Add(tagButtonLinker);
                tagButtonLinker.Text = "->";
                tagButtonLinker.Name = tag.TagName;
                tagButtonLinker.Location = new Point(x + 95, y);
                tagButtonLinker.Size = new Size(30, 50);
                tagButtonLinker.Visible = true;
                tagButtonLinker.BringToFront();
                tagButtonLinker.MouseDown += new MouseEventHandler(linker_MouseDown);
                //tagButtonLinker.MouseMove += new MouseEventHandler(textbox_MouseMove);
                tagButtonLinker.MouseUp += new MouseEventHandler(linker_MouseUp);
                tag.TagButtonLinker = tagButtonLinker;
                this.Controls.Add(tagButtonLinker);
                tagButtonLinker.BringToFront();

                tagbutton.Click += new EventHandler(Tag_Click);
                tagButtonLinker.Click += new EventHandler(TagLink_Click);
                tag.TagButtonLinker = tagButtonLinker;
            }
        }

        List<Point> points = new List<Point>();

        void linker_MouseUp(object sender, EventArgs e)
        {
            var lastOpenedForm = Application.OpenForms.Cast<Form>().Last();
            var control = FindControlAtCursor(lastOpenedForm);
            if (control is null) return;
            if(control is Button)
            {
                var btn = ((System.Windows.Forms.Button)control);
                Console.WriteLine($"{btn.Text} {btn.Name}");
                if(btn.Name != activeControl.Name)
                {
                    // draw line from first linker to second linker
                    var location = btn.Location;

                    var graphics = this.CreateGraphics();

                    Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
                    int x1 = location.X + activeControl.Width;
                    int y1 = location.Y + (activeControl.Height / 2);
                    int x2 = x1 + 100;
                    int y2 = y1;
                    graphics.DrawLine(pen, x1, y1, x2, y2);
                }
                else
                {
                    Console.WriteLine("This is the same linker button. todo: clear line ?");
                }
                
            }


            activeControl = null;
            Cursor = Cursors.Default;
        }

        public static Control FindControlAtPoint(Control container, Point pos)
        {
            Control child;
            foreach (Control c in container.Controls)
            {
                if (c.Visible && c.Bounds.Contains(pos))
                {
                    return c;
                    //child = FindControlAtPoint(c, new Point(pos.X - c.Left, pos.Y - c.Top));
                    //if (child == null) return c;
                    //else return child;
                }
            }
            return null;
        }

        public static Control FindControlAtCursor(Form form)
        {
            Point pos = Cursor.Position;
            if (form.Bounds.Contains(pos))
                return FindControlAtPoint(form, form.PointToClient(pos));
            return null;
        }

        public List<int[]> GraphicsLines = new List<int[]>();
        void linker_MouseDown(object sender, MouseEventArgs e)
        {
            activeControl = sender as Control;
            previousLocation = e.Location;
            Cursor = Cursors.Hand;

            var location = activeControl.Location;

            var graphics = this.CreateGraphics();
            
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            int x1 = location.X + activeControl.Width;
            int y1 = location.Y + (activeControl.Height /2);
            int x2 = x1 + 100;
            int y2 = y1;

            GraphicsLines.Add(new int[4] { x1, y1, x2, y2 });         
            
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
            var syncResultTask = _fileService.SyncFolderPaths(files);

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

            var syncResult = syncResultTask.Result;

            // don't block UI
            new Thread(() =>
            {
                _ = _fileService.SyncFileHash();
            }).Start();            
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

            // Tag button 
            var location = activeControl.Location;
            location.Offset(e.Location.X - previousLocation.X, e.Location.Y - previousLocation.Y);
            activeControl.Location = location;

            // linker button lookup
            var linkerButton = FileTags.First(x => x.TagName == activeControl.Name).TagButtonLinker;
            var linkerLocation = linkerButton.Location;
            linkerLocation.Offset(e.Location.X - previousLocation.X, e.Location.Y - previousLocation.Y);
            linkerButton.Location = linkerLocation;
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

       
        private void button1_Click_1(object sender, EventArgs e)
        {
            var container = Application.OpenForms.Cast<Form>().Last();
            foreach (var c in container.Controls)
            {
                Console.WriteLine(c.GetType());
                //c.Dispose();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
