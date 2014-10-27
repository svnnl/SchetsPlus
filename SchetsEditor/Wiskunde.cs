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
		// Wiskundige operator die voor twee vectoren
		// v1 en v2 (met lengte n) de som berekent van
		// de vermenigvuldiging van v1[i] en v2[i], i
		// loopt van 0 tot n.
		//
		// In deze implementatie: n == 2
		//
		public static double dot2 (Point p1, Point p2)
		{
			return (p1.X * p2.X) + (p1.Y * p2.Y);
		}

		// Berekent de afstand tussen p1 en p2
		//
		public static double afstand (Point p1, Point p2)
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

		public static Point pplus (Point p1, Point p2)
		{
			return new Point (p1.X - p2.X, p1.Y - p2.Y);
		}

		public static Point pmin (Point p1, Point p2)
		{
			return pplus (p1, new Point (-p2.X, -p2.Y));
		}

		public static Point pabs (Point p)
		{
			return new Point (Math.Abs (p.X), Math.Abs (p.Y));
		}

		// Transformeert twee punten in een Rectangle
		// waarvan de twee punten tegenoverstaande hoek-
		// punten zijn.
		//
		public static Rectangle maakRectangleVanPunten (Point p1, Point p2)
		{
			return new Rectangle (
				p1.X,
				p2.Y,
				p2.X - p1.X,
				p2.Y - p1.Y);
		}

		// Past de coordinaten van de zijdes van rechthoek
		// r aan met waarde d.
		//
		public static Rectangle vergrootRechthoek (Rectangle r, int d)
		{
			return new Rectangle (
				r.Left + d, r.Top + d,
				r.Width + d, r.Height + d
			);
		}

		// Geeft aan of punt p binnen het rechthoek ligt.
		//
		public static bool isPuntInRechthoek (Point p, Rectangle rechthoek)
		{
			// Simpele berekening die uitwijst of punt 'p'
			// binnen in rechthoek ligt.
			return (p.X > rechthoek.Left &&
					p.X < rechthoek.Right &&
					p.Y > rechthoek.Top &&
					p.Y < rechthoek.Bottom);
		}

		// Geeft aan of punt p binnen het gegeven ovaal
		// ligt.
		//
		public static bool isPuntInOvaal (Point p, Rectangle ovaal)
		{
			// Ja, als de volgende vergelijking waar is:
			//
			// (x - h)^2     (y - k)^2
			// ---------  +  ---------  <= 1
			//    w^2           h^2
			//
			// waarbij
			// 	- (x, y) = punt om te testen
			//  - (h, k) = middelpunt ovaal
			//  - w		 = breedte bounding rechthoek
			//  - h		 = hoogte bounding rechthoek
			//

			// Bereken middelpunt
			Point mp = new Point (
				(ovaal.Left + ovaal.Right) / 2,
				(ovaal.Top + ovaal.Bottom) / 2
			);

			return  ( (Math.Pow (p.X - mp.X, 2) / Math.Pow (ovaal.Width, 2)  ) +
					  (Math.Pow (p.Y - mp.Y, 2) / Math.Pow (ovaal.Height, 2) )
					) <= 1;
		}
	}
}

