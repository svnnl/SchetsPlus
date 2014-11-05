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
            fd.Filter = "Schets|*.schets|JPG|*.jpg|BMP|*.bmp|PNG|*.png";
            fd.FilterIndex = 0;

            DialogResult res = fd.ShowDialog();

            if (res == DialogResult.OK)
            {
                opslaanAls(fd.FileName);
            }
        }

        private void handler_Opslaan (object obj, EventArgs ea)
        {
            if (bestandsnaam == null)
                handler_OpslaanAls(null, null);
            else
                opslaan();
        }

		private void opslaan ()
		{
            if (bestandsnaam == null)
                return;

            opslaanAls (bestandsnaam);
		}

        private void opslaanAls (string bn)
        {
            ImageFormat format = formaatUitBestandnaam(bn);

            if (format != null || Path.GetExtension(bn) == ".schets")
            {
                bestandsnaam = bn;

                if (Path.GetExtension(bestandsnaam) == ".schets")
                    schetsControl.Schets.Serializeer(bestandsnaam);
                else
                    schetsControl.Bitmap.Save(bestandsnaam, format);

                schetsControl.Schets.IsVeranderd = false;
            }
            else
            {
                MessageBox.Show("Selecteer een bestandsformaat", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void handler_Sluiten(object sender, System.ComponentModel.CancelEventArgs cea)
        {
            if(schetsControl.Schets.IsVeranderd)
            {
                DialogResult res = MessageBox.Show(
                    "Het plaatje bevat onopgeslagen wijzigingen, weet u zeker dat u weg wilt?",
                    "Weet u het zeker?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (res == DialogResult.No)
                    cea.Cancel = true;
            }
        }

		private void afsluiten (object obj, EventArgs ea)
		{
			this.Close ();
		}

		public SchetsWin (string bn = "")
		{
			ISchetsTool[] deTools = { new PenTool ()         
                                    , new LijnTool ()
                                    , new RechthoekTool ()
                                    , new VolRechthoekTool ()
                                    , new TekstTool ()
                                    , new GumTool ()
                                    , new EllipsTool ()
                                    , new VolEllipsTool ()
			};

			this.ClientSize = new Size (800, 600);
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
			this.maakAktieMenu ();
			this.maakToolButtons (deTools);
			this.maakAktieButtons ();
			this.Resize += this.veranderAfmeting;
            this.Closing += this.handler_Sluiten;
			this.veranderAfmeting (null, null);

            if (bn != "")
            {
                bestandsnaam = bn;
                schetsControl.Schets.Deserializeer(bestandsnaam);     
                schetsControl.Schets.IsVeranderd = false;
            }
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

		private void maakAktieMenu ()
		{   
			ToolStripMenuItem menu = new ToolStripMenuItem ("Aktie");
			menu.DropDownItems.Add ("Clear", null, schetsControl.Schoon);
			menu.DropDownItems.Add ("Roteer", null, schetsControl.Roteer);

			menuStrip.Items.Add (menu);
		}

		private void maakToolButtons (ICollection<ISchetsTool> tools)
		{
			int t = 0;
            int buttonBreedte = 45;
            int buttonHoogte = 62;
            int buttonMarge = 10;

			foreach (ISchetsTool tool in tools)
			{
				RadioButton b = new RadioButton ();
				b.Appearance = Appearance.Button;
                b.BackColor = Color.White;
                b.Size = new Size(buttonBreedte, buttonHoogte);
                b.Location = new Point(buttonMarge, buttonMarge + t * (buttonHoogte + buttonMarge));
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

		private void maakAktieButtons ()
		{   
			paneel = new Panel ();
			paneel.Size = new Size (600, 24);
			this.Controls.Add (paneel);
            
			Button b;
            Label l;
            NumericUpDown nud;

			b = new Button (); 
			b.Text = "Clear";  
			b.Location = new Point (0, 0); 
			b.Click += schetsControl.Schoon;
			paneel.Controls.Add (b);
            
			b = new Button (); 
			b.Text = "Roteer"; 
			b.Location = new Point (80, 0); 
			b.Click += schetsControl.Roteer; 
			paneel.Controls.Add (b);
            
			b = new Button ();  
			b.Text = "Kleurkiezer"; 
			b.Location = new Point (160, 0); 
			b.Click += (sender, ea) =>
                {
                    ColorDialog cd = new ColorDialog();
                    cd.Color = schetsControl.Kleur;
                    if (cd.ShowDialog() == DialogResult.OK)
                        schetsControl.Kleur = cd.Color;
                };
			paneel.Controls.Add (b);

            l = new Label();
            l.Location = new Point(240, 3);
            l.Text = "Lijn Dikte:";
            l.Width = 60;
            paneel.Controls.Add(l);

            nud = new NumericUpDown();
            nud.Location = new Point(300, 1);
            nud.Width = 50;
            nud.Minimum = 0;
            nud.Maximum = 10;
            nud.Value = schetsControl.LijnDikte;
            nud.ValueChanged += (sender, ea) =>
                {
                    schetsControl.LijnDikte = (int)((NumericUpDown)sender).Value;
                };
            paneel.Controls.Add(nud);
		}
    }
}
