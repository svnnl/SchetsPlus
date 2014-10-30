﻿using System;
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
            if (c >= 32)
            {
                Graphics graphics = Graphics.FromImage(s.Bitmap);
                SizeF karakterGrootte =
                    graphics.MeasureString ( c.ToString()
                                           , SchetsEditor.Letter.Lettertype
                                           , startpunt
                                           , StringFormat.GenericTypographic
                                           );

                Letter letter = new Letter (c, startpunt, s.PenKleur, karakterGrootte);

                startpunt.X += letter.rechthoek.Width;

                s.Schets.VoegSchetsbaarItemToe(letter);
                s.Invalidate();
            }
		}
	}

	public abstract class TweepuntTool : StartpuntTool
	{
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
            OmlijndRechthoek rechthoek = new OmlijndRechthoek(Wiskunde.MaakRectangleVanPunten(p1, p2), new Pen(kwast));
            schets.ZetOverlayItem(rechthoek);
		}

        public override void Compleet (Schets schets, Point p1, Point p2)
        {
            base.Compleet(schets, p1, p2);

            OmlijndRechthoek rechthoek = new OmlijndRechthoek(Wiskunde.MaakRectangleVanPunten(p1, p2), new Pen(kwast));
            schets.VoegSchetsbaarItemToe(rechthoek);
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
            OmlijndOvaal ovaal = new OmlijndOvaal(Wiskunde.MaakRectangleVanPunten(p1, p2), new Pen(kwast));
            schets.ZetOverlayItem(ovaal);
		}

        public override void Compleet(Schets schets, Point p1, Point p2)
        {
            base.Compleet(schets, p1, p2);

            OmlijndOvaal ovaal = new OmlijndOvaal(Wiskunde.MaakRectangleVanPunten(p1, p2), new Pen(kwast));
            schets.VoegSchetsbaarItemToe(ovaal);
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
            OmlijndRechthoek rechthoek = new OmlijndRechthoek(Wiskunde.MaakRectangleVanPunten(p1, p2), new Pen(kwast));
            schets.ZetOverlayItem(rechthoek);
        }

        public override void Compleet(Schets schets, Point p1, Point p2)
        {
            base.Compleet(schets, p1, p2);

            GevuldRechthoek rechthoek = new GevuldRechthoek(Wiskunde.MaakRectangleVanPunten(p1, p2), kwast);
            schets.VoegSchetsbaarItemToe(rechthoek);
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
            OmlijndOvaal ovaal = new OmlijndOvaal(Wiskunde.MaakRectangleVanPunten(p1, p2), new Pen(kwast));
            schets.ZetOverlayItem(ovaal);
        }

        public override void Compleet(Schets schets, Point p1, Point p2)
        {
            base.Compleet(schets, p1, p2);

            GevuldOvaal ovaal = new GevuldOvaal(Wiskunde.MaakRectangleVanPunten(p1, p2), kwast);
            schets.VoegSchetsbaarItemToe(ovaal);
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
            Lijn lijn = new Lijn(p1, p2, new Pen(kwast, lijnDikte));
            schets.ZetOverlayItem(lijn);
        }

        public override void Compleet(Schets schets, Point p1, Point p2)
        {
            base.Compleet(schets, p1, p2);

            Lijn lijn = new Lijn(p1, p2, new Pen(kwast, lijnDikte));
            schets.VoegSchetsbaarItemToe(lijn);
        }
	}

	public class PenTool : TweepuntTool
	{
        private GetekendeLijn lijnTotNu = null;

		public override string ToString ()
		{
			return "pen";
		}

        public override void MuisVast(SchetsControl s, Point p)
        {
            base.MuisVast(s, p);

            // Voor deze tool hebben we liever dat de kleur tijdens het
            // tekenen al de gekozen kleur is.
            kwast = new SolidBrush(s.PenKleur);
        }

        public override void Bezig(Schets schets, Point p1, Point p2)
        {
            if (lijnTotNu == null)
            {
                lijnTotNu = new GetekendeLijn();
            }

            /* Voeg een nieuwe lijn to vanaf het laatste punt tot dit punt, laat die
             * ook zien op de overlay, maar voeg hem nog niet echt toe aan de schets */

            Point laatstePunt = lijnTotNu.LaatstePunt.HasValue ? (Point) lijnTotNu.LaatstePunt : p1;
            Lijn subLijn = new Lijn(laatstePunt, p2, new Pen(kwast));

            lijnTotNu.VoegLijntjeToe(subLijn);

            schets.ZetOverlayItem(lijnTotNu);
        }

        public override void Compleet(Schets schets, Point p1, Point p2)
        {
            base.Compleet(schets, p1, p2);

            schets.VoegSchetsbaarItemToe (lijnTotNu);
            lijnTotNu = null; // Begin opnieuw
        }
	}

	public class GumTool : TweepuntTool
	{
        private const int gumDikte = SchetsbaarItem.KlikMarge;

		public override string ToString ()
		{
			return "gum";
		}

        public override void Bezig(Schets schets, Point p1, Point p2)
        {
            Point pLinksBoven = new Point(p2.X - gumDikte, p2.Y - gumDikte);
            Point pRechtsOnder = new Point(p2.X + gumDikte, p2.Y + gumDikte);

            Rectangle rechthoekOmOvaal = Wiskunde.MaakRectangleVanPunten(pLinksBoven, pRechtsOnder);

            OmlijndOvaal ovaal = new OmlijndOvaal(rechthoekOmOvaal, new Pen(kwast));

            schets.ZetOverlayItem(ovaal); // Rondje op de plek van de gum
            schets.VerwijderSchetsbaarItemOpPunt(p2);
        }
	}
}
