using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class SchetsControl : UserControl
    {
        private Schets schets;
        private Color kleur;
        private int lijnDikte;

        public Schets Schets { get { return schets; } }

        public Color Kleur { get { return kleur; } set { kleur = value; } }

        public int LijnDikte { get { return lijnDikte; } set { lijnDikte = value; } }

        public SchetsControl()
        {
            this.kleur = Color.Black;
            this.lijnDikte = 3;
            this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.Teken;
            this.Resize += this.VeranderAfmeting;
            this.VeranderAfmeting(null, null);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

        private void Teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);
        }

        private void VeranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }

        public void Schoon(object o, EventArgs ea)
        {
            schets.Schoon();
            this.Invalidate();
        }

        public void Roteer(object o, EventArgs ea)
        {
            schets.Roteer();
            this.VeranderAfmeting(o, ea);
        }

        public void VeranderKleur(object obj, EventArgs ea)
        {
            string kleurNaam = ((ComboBox)obj).Text;
            kleur = Color.FromName(kleurNaam);
        }

        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {
            string kleurNaam = ((ToolStripMenuItem)obj).Text;
            kleur = Color.FromName(kleurNaam);
        }

        public Bitmap Bitmap
        {
            get { return schets.Bitmap; }
        }
    }
}
