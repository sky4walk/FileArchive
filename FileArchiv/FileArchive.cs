// (C) 2004 André Betz
// http://www.andrebetz.de

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

namespace FileArchiv
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FileArchive : System.Windows.Forms.Form
	{
		#region variables
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button Button_GO;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ListBox FileList;
		private System.Windows.Forms.RadioButton Pack;
		private System.Windows.Forms.RadioButton UnPack;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private string PackDirectory;
		private string PackArchive;
		private Packer packingTool;
		private Thread PackThread;

		#endregion
		#region Windowsgeschnoesel
		public FileArchive()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.Pack.Checked = true;
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new FileArchive());
		}
		#endregion
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.Button_GO = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.UnPack = new System.Windows.Forms.RadioButton();
			this.Pack = new System.Windows.Forms.RadioButton();
			this.button2 = new System.Windows.Forms.Button();
			this.FileList = new System.Windows.Forms.ListBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 72);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(152, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Choose Un/Pack Directory";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Button_GO
			// 
			this.Button_GO.Location = new System.Drawing.Point(8, 136);
			this.Button_GO.Name = "Button_GO";
			this.Button_GO.Size = new System.Drawing.Size(152, 23);
			this.Button_GO.TabIndex = 1;
			this.Button_GO.Text = "Go";
			this.Button_GO.Click += new System.EventHandler(this.Button_GO_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.UnPack);
			this.groupBox1.Controls.Add(this.Pack);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(168, 56);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pack/Unpack";
			// 
			// UnPack
			// 
			this.UnPack.Location = new System.Drawing.Point(80, 24);
			this.UnPack.Name = "UnPack";
			this.UnPack.Size = new System.Drawing.Size(64, 24);
			this.UnPack.TabIndex = 1;
			this.UnPack.Text = "UnPack";
			// 
			// Pack
			// 
			this.Pack.Location = new System.Drawing.Point(8, 24);
			this.Pack.Name = "Pack";
			this.Pack.Size = new System.Drawing.Size(56, 24);
			this.Pack.TabIndex = 0;
			this.Pack.Text = "Pack";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(8, 104);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(152, 23);
			this.button2.TabIndex = 5;
			this.button2.Text = "Choose Un/Pack File";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// FileList
			// 
			this.FileList.HorizontalScrollbar = true;
			this.FileList.Location = new System.Drawing.Point(200, 16);
			this.FileList.Name = "FileList";
			this.FileList.Size = new System.Drawing.Size(264, 134);
			this.FileList.TabIndex = 6;
			// 
			// FileArchive
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 165);
			this.Controls.Add(this.FileList);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.Button_GO);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FileArchive";
			this.Text = "File Archive (C) www.andrebetz.de";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#region ButtonHandler
		private void button1_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog fbd= new FolderBrowserDialog();
			if(fbd.ShowDialog(this) == DialogResult.OK)
			{
				PackDirectory = fbd.SelectedPath;
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			if(this.Pack.Checked==true)
			{
				SaveFileDialog sfd = new SaveFileDialog();
				if(sfd.ShowDialog(this) == DialogResult.OK)
				{
					PackArchive = sfd.FileName;
				}
			}
			else
			{
				OpenFileDialog ofd = new OpenFileDialog();
				if(ofd.ShowDialog(this) == DialogResult.OK)
				{
					PackArchive = ofd.FileName;
				}
			}
		}

		public void ShowFiles(string FileName)
		{
			if(FileName==null)
			{
				this.button1.Enabled = true;
				this.button2.Enabled = true;
				this.Button_GO.Enabled = true;
			}
			else
			{
				this.FileList.Items.Add(FileName);
			}
		}

		internal void ShowErrorBox(string Msg)
		{
			MessageBox.Show(Msg);
		}

		private void StartPacking()
		{
			this.button1.Enabled = false;
			this.button2.Enabled = false;
			this.Button_GO.Enabled = false;
			this.FileList.Items.Clear();
			packingTool = new Packer();
			packingTool.SetCallBacks(new Packer.ActualFile(ShowFiles),new Packer.Error(ShowErrorBox));
			packingTool.PackFiles(PackDirectory,PackArchive);
		}

		private void StartUnPacking()
		{
			this.button1.Enabled = false;
			this.button2.Enabled = false;
			this.Button_GO.Enabled = false;
			this.FileList.Items.Clear();
			packingTool = new Packer();
			packingTool.SetCallBacks(new Packer.ActualFile(ShowFiles),new Packer.Error(ShowErrorBox));
			packingTool.UnPackFiles(PackDirectory,PackArchive);
		}
		private void Button_GO_Click(object sender, System.EventArgs e)
		{
			if(PackDirectory==null)
			{
				MessageBox.Show("Kein Verzeichnis ausgewählt");
				return;
			}
			if(PackArchive==null)
			{
				MessageBox.Show("Kein Archivname ausgewählt");
				return;
			}
			if(this.Pack.Checked==true)
			{
				PackThread = new Thread(new ThreadStart(StartPacking));
				PackThread.Name = "Pack Files";
				PackThread.Start();
			}			
			else
			{
				PackThread = new Thread(new ThreadStart(StartUnPacking));
				PackThread.Name = "UnPack Files";
				PackThread.Start();
			}
		}
		#endregion
	}
}
