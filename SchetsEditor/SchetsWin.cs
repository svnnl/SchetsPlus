using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Drawing.Imaging;
using System.IO;

namespace SchetsEditor
{
	public class SchetsWin : Form
	{
		MenuStrip 		menuStrip;
		SchetsControl 	schetsControl;
		ISchetsTool 	huidigeTool;
		Panel 			paneel;
		bool 			vast;
        string          bestandsnaam = null;

		ResourceManager resourcemanager
            = new ResourceManager (	"SchetsEditor.Properties.Resources",
									Assembly.GetExecutingAssembly ());

		private void veranderAfmeting (object o, EventArgs ea)
		{
			schetsControl.Size = new Size (this.ClientSize.Width - 70,
                                           this.ClientSize.Height - 50);

			paneel.Location = new Point (64, this.ClientSize.Height - 30);
		}

		private void klikToolMenu (object obj, EventArgs ea)
		{
			this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
		}

		private void klikToolButton (object obj, EventArgs ea)
		{
			this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
		}

        private ImageFormat formaatUitBestandnaam(string bn)
        {
            ImageFormat format = null;
            switch (Path.GetExtension(bn))
            {
                case ".jpg": format = ImageFormat.Jpeg; break;
                case ".bmp": format = ImageFormat.Bmp; break;
                case ".png": format = ImageFormat.Png; break;
                default:
                    return null;
            }

            return format;
        }

        private void handler_OpslaanAls(object obj, EventArgs ea)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "JPG|*.jpg|BMP|*.bmp|PNG|*.png|Schets|*.schets";

            DialogResult res = fd.ShowDialog();

            if (res == DialogResult.OK)
            {
                opslaanAls(fd.FileName);
            }
        }

        private void handler_Opslaan (object obj, EventArgs ea)
        {
            if (bestandsnaam == null)
            {
                handler_OpslaanAls(null, null);
            }
            else
            {
                opslaan();
            }
        }

		private void opslaan ()
		{
            if (bestandsnaam == null)
                return;

            if (Path.GetExtension(bestandsnaam) == ".schets")
            {
                schetsControl.Schets.OpslaanAls(bestandsnaam);
            }
            else
            {
                ImageFormat format = formaatUitBestandnaam(bestandsnaam);
                schetsControl.Bitmap.Save(bestandsnaam, format);
            }
		}

        private void opslaanAls (string bn)
        {
            ImageFormat format = formaatUitBestandnaam(bn);

            if (format != null || Path.GetExtension(bn) == ".schets")
            {
                bestandsnaam = bn;
                opslaan();
            }
            else
            {
                MessageBox.Show("Selecteer een bestandsformaat", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

		private void afsluiten (object obj, EventArgs ea)
		{
			this.Close ();
		}

		public SchetsWin ()
		{
			ISchetsTool[] deTools = { new PenTool ()         
                                    , new LijnTool ()
                                    , new RechthoekTool ()
                                    , new VolRechthoekTool ()
                                    , new TekstTool ()
                                    , new GumTool ()
                                    , new EllipsTool ()
                                    , new VulEllipsTool ()
			};

			String[] deKleuren = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan", "Brown"
                                 , "Orange"
			};

			this.ClientSize = new Size (700, 500);
			huidigeTool = deTools [0];

			schetsControl = new SchetsControl ();
			schetsControl.Location = new Point (64, 10);
			schetsControl.MouseDown += (object o, MouseEventArgs mea) => {
				vast = true;  
				huidigeTool.MuisVast (schetsControl, mea.Location); 
			};

			schetsControl.MouseMove +=
				(object o, MouseEventArgs mea) => {
					if (vast)
						huidigeTool.MuisDrag (schetsControl, mea.Location); 
				};

			schetsControl.MouseUp +=
				(object o, MouseEventArgs mea) => {
					vast = false; 
					huidigeTool.MuisLos (schetsControl, mea.Location); 
				};

			schetsControl.KeyPress +=
				(object o, KeyPressEventArgs kpea) => {
					huidigeTool.Letter (schetsControl, kpea.KeyChar); 
				};

			this.Controls.Add (schetsControl);

			menuStrip = new MenuStrip ();
			menuStrip.Visible = false;

			this.Controls.Add (menuStrip);
			this.maakFileMenu ();
			this.maakToolMenu (deTools);
			this.maakAktieMenu (deKleuren);
			this.maakToolButtons (deTools);
			this.maakAktieButtons (deKleuren);
			this.Resize += this.veranderAfmeting;
			this.veranderAfmeting (null, null);
		}

		private void maakFileMenu ()
		{   
			ToolStripMenuItem menu = new ToolStripMenuItem ("File");
			menu.MergeAction = MergeAction.MatchOnly;
			menu.DropDownItems.Add ("Opslaan", null, this.handler_Opslaan);
            menu.DropDownItems.Add ("Opslaan Als", null, this.handler_OpslaanAls);
			menu.DropDownItems.Add ("Sluiten File", null, this.afsluiten);

			menuStrip.Items.Add (menu);
		}

		private void maakToolMenu (ICollection<ISchetsTool> tools)
		{   
			ToolStripMenuItem menu = new ToolStripMenuItem ("Tool");
			foreach (ISchetsTool tool in tools)
			{
				ToolStripItem item = new ToolStripMenuItem ();
				item.Tag = tool;
				item.Text = tool.ToString ();
				item.Image = (Image)resourcemanager.GetObject (tool.ToString ());
				item.Click += this.klikToolMenu;
				menu.DropDownItems.Add (item);
			}

			menuStrip.Items.Add (menu);
		}

		private void maakAktieMenu (String[] kleuren)
		{   
			ToolStripMenuItem menu = new ToolStripMenuItem ("Aktie");
			menu.DropDownItems.Add ("Clear", null, schetsControl.Schoon);
			menu.DropDownItems.Add ("Roteer", null, schetsControl.Roteer);

			ToolStripMenuItem submenu = new ToolStripMenuItem ("Kies kleur");
			foreach (string k in kleuren)
				submenu.DropDownItems.Add (k, null, schetsControl.VeranderKleurViaMenu);
			menu.DropDownItems.Add (submenu);

			menuStrip.Items.Add (menu);
		}

		private void maakToolButtons (ICollection<ISchetsTool> tools)
		{
			int t = 0;
			foreach (ISchetsTool tool in tools)
			{
				RadioButton b = new RadioButton ();
				b.Appearance = Appearance.Button;
				b.Size = new Size (45, 62);
				b.Location = new Point (10, 10 + t * 62);
				b.Tag = tool;
				b.Text = tool.ToString ();
				b.Image = (Image)resourcemanager.GetObject (tool.ToString ());
				b.TextAlign = ContentAlignment.TopCenter;
				b.ImageAlign = ContentAlignment.BottomCenter;
				b.Click += this.klikToolButton;
				this.Controls.Add (b);
				if (t == 0)
					b.Select ();
				t++;
			}
		}

		private void maakAktieButtons (String[] kleuren)
		{   
			paneel = new Panel ();
			paneel.Size = new Size (600, 24);
			this.Controls.Add (paneel);
            
			Button b;
			Label l;
			ComboBox cbb;

			b = new Button (); 
			b.Text = "Clear";  
			b.Location = new Point (0, 0); 
			b.Click += schetsControl.Schoon; 
			paneel.Controls.Add (b);
            
			b = new Button (); 
			b.Text = "Rotate"; 
			b.Location = new Point (80, 0); 
			b.Click += schetsControl.Roteer; 
			paneel.Controls.Add (b);
            
			l = new Label ();  
			l.Text = "Penkleur:"; 
			l.Location = new Point (180, 3); 
			l.AutoSize = true;               
			paneel.Controls.Add (l);
            
			cbb = new ComboBox ();
			cbb.Location = new Point (240, 0); 
			cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
			cbb.SelectedValueChanged += schetsControl.VeranderKleur;
			foreach (string k in kleuren)
				cbb.Items.Add (k);
			cbb.SelectedIndex = 0;
			paneel.Controls.Add (cbb);
		}
    }
}
