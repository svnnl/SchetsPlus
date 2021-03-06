﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsEditor
{
	public class Hoofdscherm : Form
	{
		MenuStrip menuStrip;

		public Hoofdscherm ()
		{
			this.ClientSize = new Size (1200, 850);
			menuStrip = new MenuStrip ();
			this.Controls.Add (menuStrip);
			this.maakFileMenu ();
			this.maakHelpMenu ();
			this.Text = "SchetsPlus";
			this.IsMdiContainer = true;
			this.MainMenuStrip = menuStrip;
		}

		private void maakFileMenu ()
		{
			ToolStripDropDownItem menu;
			menu = new ToolStripMenuItem ("File");
			menu.DropDownItems.Add ("Nieuw", null, this.nieuw);
            menu.DropDownItems.Add ("Open", null, this.open);
			menu.DropDownItems.Add ("Exit", null, this.afsluiten);
			menuStrip.Items.Add (menu);
		}

		private void maakHelpMenu ()
		{
			ToolStripDropDownItem menu;
			menu = new ToolStripMenuItem ("Help");
			menu.DropDownItems.Add ("Over \"Schets\"", null, this.about);
			menuStrip.Items.Add (menu);
		}

		private void about (object o, EventArgs ea)
		{
			MessageBox.Show ("Schets versie 1.1\n(c) UU Informatica 1014 door Sebastiaan & Gerwin 2014"
                           , "Over \"SchetsPlus\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
			);
		}

		private void nieuw (object sender, EventArgs e)
		{
			SchetsWin s = new SchetsWin ();
			s.MdiParent = this;
			s.Show ();
		}

		private void afsluiten (object sender, EventArgs e)
		{
			this.Close ();
		}

        private void open (object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Schets|*.schets";
            
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if (System.IO.File.Exists(fd.FileName))
                {
                    SchetsWin s = new SchetsWin(fd.FileName);
                    s.MdiParent = this;
                    s.Show();
                }
            }
        }
    }
}
