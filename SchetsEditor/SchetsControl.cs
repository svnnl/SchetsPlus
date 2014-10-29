using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsEditor
{
	public class SchetsControl : UserControl
	{
		private Schets schets;
		private Color penkleur;

        public Schets Schets { get { return schets; } }

		public Color PenKleur { get { return penkleur; } }

		public SchetsControl ()
		{
			this.BorderStyle = BorderStyle.Fixed3D;
			this.schets = new Schets ();
			this.Paint += this.Teken;
			this.Resize += this.VeranderAfmeting;
			this.VeranderAfmeting (null, null);
		}

		protected override void OnPaintBackground (PaintEventArgs e)
		{

		}

		private void Teken (object o, PaintEventArgs pea)
		{
			schets.Teken (pea.Graphics);
		}

		private void VeranderAfmeting (object o, EventArgs ea)
		{
			schets.VeranderAfmeting (this.ClientSize);
			this.Invalidate ();
		}

		public void Schoon (object o, EventArgs ea)
		{
			schets.Schoon ();
			this.Invalidate ();
		}

		public void Roteer (object o, EventArgs ea)
		{
			schets.Roteer ();
			this.VeranderAfmeting (o, ea);
		}

		public void VeranderKleur (object obj, EventArgs ea)
		{
			string kleurNaam = ((ComboBox)obj).Text;
			penkleur = Color.FromName (kleurNaam);
		}

		public void VeranderKleurViaMenu (object obj, EventArgs ea)
		{
			string kleurNaam = ((ToolStripMenuItem)obj).Text;
			penkleur = Color.FromName (kleurNaam);
		}

		public Bitmap Bitmap {
			get { return schets.Bitmap; }
		}
	}
}
