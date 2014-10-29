using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
	public interface ISchetsTool
	{
		void MuisVast (SchetsControl s, Point p);
		void MuisDrag (SchetsControl s, Point p);
		void MuisLos (SchetsControl s, Point p);
		void Letter (SchetsControl s, char c);
	}

	public abstract class StartpuntTool : ISchetsTool
	{
		protected Point startpunt;
		protected Brush kwast;

		public virtual void MuisVast (SchetsControl s, Point p)
		{
			startpunt = p;
		}

		public virtual void MuisLos (SchetsControl s, Point p)
		{
			kwast = new SolidBrush (s.PenKleur);
		}

		public abstract void MuisDrag (SchetsControl s, Point p);

		public abstract void Letter (SchetsControl s, char c);
	}

	public class TekstTool : StartpuntTool
	{
		public override string ToString ()
		{
			return "tekst";
		}

		public override void MuisDrag (SchetsControl s, Point p)
		{

		}

		public override void Letter (SchetsControl s, char c)
		{

		}

		/*

		public override void Letter (SchetsControl s, char c)
		{
			if (c >= 32) {
				Graphics gr = s.MaakBitmapGraphics ();
				Font font = new Font ("Tahoma", 40);
				string tekst = c.ToString ();
				SizeF sz = 
					gr.MeasureString (tekst, font, this.startpunt, StringFormat.GenericTypographic);
				gr.DrawString (tekst, font, kwast, 
					this.startpunt, StringFormat.GenericTypographic);
				// gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
				startpunt.X += (int)sz.Width;
				s.Invalidate ();
			}
		}

			TODO: Implementeer als SchetsItem.

		*/

	}

	public abstract class TweepuntTool : StartpuntTool
	{
		public static Pen MaakPen (Brush b, int dikte)
		{
			Pen pen = new Pen (b, dikte);
			pen.StartCap = LineCap.Round;
			pen.EndCap = LineCap.Round;

			return pen;
		}

		public override void MuisVast (SchetsControl s, Point p)
		{
			base.MuisVast (s, p);
			kwast = Brushes.Gray;
		}

		public override void MuisDrag (SchetsControl s, Point p)
		{
			s.Refresh ();
			this.Bezig (s.Schets, this.startpunt, p);

            s.Invalidate();
		}

		public override void MuisLos (SchetsControl s, Point p)
		{
			base.MuisLos (s, p);
			this.Compleet (s.Schets, this.startpunt, p);

            s.Invalidate();
		}

		public override void Letter (SchetsControl s, char c)
		{

		}

		public abstract void Bezig (Schets schets, Point p1, Point p2);

        public virtual void Compleet(Schets schets, Point p1, Point p2)
        {
            this.Bezig(schets, p1, p2);

            schets.ZetOverlayItem(null);
        }
	}

	public class RechthoekTool : TweepuntTool
	{
		public override string ToString ()
		{
			return "kader";
		}

		public override void Bezig (Schets schets, Point p1, Point p2)
		{
            schets.ZetOverlayItem(
                new OmlijndeRechthoek(
                    Wiskunde.MaakRectangleVanPunten(p1, p2),
                    new Pen (kwast)));
		}

        public override void Compleet (Schets schets, Point p1, Point p2)
        {
            schets.VoegSchetsbaarItemToe(
                new OmlijndeRechthoek(
                    Wiskunde.MaakRectangleVanPunten(p1, p2),
                    new Pen(kwast)));

            base.Compleet(schets, p1, p2);
        }
	}

	public class EllipsTool : TweepuntTool   // Drawing of circles
	{
		public override string ToString ()
		{
			return "ellipse";
		}

		public override void Bezig (Schets schets, Point p1, Point p2)
		{
            schets.ZetOverlayItem(
                new OmlijndOvaal(
                    Wiskunde.MaakRectangleVanPunten(p1, p2),
                    new Pen(kwast)));
		}

        public override void Compleet(Schets schets, Point p1, Point p2)
        {
            schets.VoegSchetsbaarItemToe(
                new OmlijndOvaal(
                    Wiskunde.MaakRectangleVanPunten(p1, p2),
                    new Pen(kwast)));

            base.Compleet(schets, p1, p2);
        }
	}

	public class VolRechthoekTool : TweepuntTool
	{
		public override string ToString ()
		{
			return "vlak";
		}

        public override void Bezig(Schets schets, Point p1, Point p2)
        {
            schets.ZetOverlayItem(
                new OmlijndeRechthoek(
                    Wiskunde.MaakRectangleVanPunten(p1, p2),
                    new Pen(kwast)));
        }

        public override void Compleet(Schets schets, Point p1, Point p2)
        {
            schets.VoegSchetsbaarItemToe(
                new GevuldRechthoek(
                    Wiskunde.MaakRectangleVanPunten(p1, p2),
                    kwast));

            base.Compleet(schets, p1, p2);
        }
	}

	public class VulEllipsTool : TweepuntTool
	{
		public override string ToString ()
		{
			return "vulEllips";
		}

        public override void Bezig(Schets schets, Point p1, Point p2)
        {
            schets.ZetOverlayItem(
                new OmlijndOvaal(
                    Wiskunde.MaakRectangleVanPunten(p1, p2),
                    new Pen(kwast)));
        }

        public override void Compleet(Schets schets, Point p1, Point p2)
        {
            schets.VoegSchetsbaarItemToe(
                new GevuldOvaal(
                    Wiskunde.MaakRectangleVanPunten(p1, p2),
                    kwast));

            base.Compleet(schets, p1, p2);
        }
	}

	public class LijnTool : TweepuntTool
	{
        private const float lijnDikte = 4;

		public override string ToString ()
		{
			return "lijn";
		}

        public override void Bezig(Schets schets, Point p1, Point p2)
        {
            schets.ZetOverlayItem(
                new Lijn(
                    p1,
                    p2,
                    new Pen(kwast)));
        }

        public override void Compleet(Schets schets, Point p1, Point p2)
        {
            schets.VoegSchetsbaarItemToe(
                new Lijn(
                    p1,
                    p2,
                    new Pen(kwast, lijnDikte)));

            base.Compleet(schets, p1, p2);
        }
	}

	public class PenTool : LijnTool
	{
		public override string ToString ()
		{
			return "pen";
		}

		public override void MuisDrag (SchetsControl s, Point p)
		{
			this.MuisLos (s, p);
			this.MuisVast (s, p);
		}
	}

	public class GumTool : TweepuntTool
	{
		public override string ToString ()
		{
			return "gum";
		}

        public override void Bezig(Schets schets, Point p1, Point p2)
        {
            schets.ZetOverlayItem(
                new OmlijndOvaal(
                    // Rondje om de gum
                    Wiskunde.MaakRectangleVanPunten(
                        new Point(p2.X-2,p2.Y-2),
                        new Point(p2.X+2,p2.Y+2)),
                    new Pen(kwast)));

            schets.VerwijderSchetsbaarItemOpPunt(p2); 
        }
	}
}
