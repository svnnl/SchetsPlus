using System;
using System.Drawing;

namespace SchetsEditor
{
	// Algemene klasse die fungeert als uitbreiding op Math in
	// de zin dat het algemene wiskundige functies implementeerd
	// die nodig zijn voor 2D berekeningen in dit project.
	//
	class Wiskunde
	{
		// Berekent de afstand tussen p1 en p2
		//
		public static double Afstand (Point p1, Point p2)
		{
			return Math.Sqrt (
				Math.Pow (Math.Abs (p1.X - p2.X), 2) +
				Math.Pow (Math.Abs (p1.Y - p2.Y), 2)
			);
		}

		/*
		 * Een aantal veelvoorkomende wiskunde operaties
		 * voor Punten geimplementeerd.cvvc
		 */

		public static Point PuntPlus (Point p1, Point p2)
		{
			return new Point (p1.X - p2.X, p1.Y - p2.Y);
		}

		public static Point PuntMin (Point p1, Point p2)
		{
			return PuntPlus (p1, new Point (-p2.X, -p2.Y));
		}

		// Transformeert twee punten in een Rectangle
		// waarvan de twee punten tegenoverstaande hoek-
		// punten zijn.
		//
		public static Rectangle MaakRectangleVanPunten (Point p1, Point p2)
		{
			return new Rectangle (
				Math.Min (p1.X, p2.X),
				Math.Min (p1.Y, p2.Y),
				Math.Abs (p2.X - p1.X),
				Math.Abs (p2.Y - p1.Y));
		}

		// Past de coordinaten van de zijdes van rechthoek
		// r aan met waarde d.
		//
		public static Rectangle VergrootRechthoek (Rectangle r, int d)
		{
			return new Rectangle (
				r.Left - d, r.Top - d,
				r.Width + d, r.Height + d
			);
		}

		// Geeft aan of punt p binnen het rechthoek ligt.
		//
		public static bool IsPuntInRechthoek (Point p, Rectangle rechthoek)
		{
			// Simpele berekening die uitwijst of punt 'p'
			// binnen in rechthoek ligt.
			return (p.X >= rechthoek.Left &&
					p.X <= rechthoek.Right &&
					p.Y >= rechthoek.Top &&
					p.Y <= rechthoek.Bottom);
		}

		// Geeft aan of punt p binnen het gegeven ovaal
		// ligt.
		//
		public static bool IsPuntInOvaal (Point p, Rectangle ovaal)
		{
			// Bereken middelpunt, gernomalizeerd, dus
            // alsof de ovaal 
			Point mp = new Point (
                ovaal.Left + (ovaal.Width / 2),
                ovaal.Top + (ovaal.Height / 2)
			);

            double xComp = (double)(Math.Pow(p.X - mp.X, 2) / Math.Pow(ovaal.Width / 2, 2));
            double yComp = (double)(Math.Pow(p.Y - mp.Y, 2) / Math.Pow(ovaal.Height / 2, 2));

            return (double) (xComp + yComp) < (double) 1;
		}
	}
}

