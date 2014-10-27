using System;
using System.Drawing;

namespace SchetsEditor
{
	public interface ISchetsItem
	{
		void draw (Graphics g);

		bool isGeklikt (Point klik);
	}

	public class OmlijndItem : ISchetsItem
	{
		// Marge die wordt aangehouden voor omlijnde items,
		// binnen deze marge wordt een klik nog steeds
		// geregistreerd.
		protected const int klikMarge = 2;

		public Pen lijn { get; protected set; }
	}

	public class GevuldItem
	{
		public Brush vulling { get; protected set; }
	}

	public class RechthoekigItem
	{
		public static Rectangle maakRectangleVanPunten (Point p1, Point p2)
		{
			return new Rectangle (
				p1.X,
				p2.Y,
				p2.X - p1.X,
				p2.Y - p1.Y);
		}

		public static Rectangle vergrootRechthoek (Rectangle r, int d)
		{
			return new Rectangle (
				r.Left + d, r.Top + d,
				r.Width + d, r.Height + d
			);
		}

		public Rectangle rechthoek { get; protected set; }
	}

	public class OvaalItem
	{
		public Rectangle ovaal { get; protected set; }
	}

	public class Lijn : OmlijndItem
	{
		public Point punt1 { get; protected set; }
		public Point punt2 { get; protected set; }

		public Lijn (Point p1, Point p2, Pen pen)
		{
			punt1 = p1; punt2 = p2; lijn = pen;
		}

		public void draw (Graphics g)
		{
			g.DrawLine (lijn, punt1, punt2);
		}

		public Boolean isGeklikt (Point klik)
		{
			double verhoudingP1P2
				= Math.Abs (punt1.X - punt2.X) / Math.Abs (punt1.Y - punt2.Y);
			double verhoudingP1Klik
				= Math.Abs (punt1.X - klik.X) - Math.Abs (punt1.Y - klik.Y);

			// TODO bereken verhoudingMarge
			double verhoudingMarge = 1.0;

			// Als het verschil tussen de verhouding van p1-p2 en p1-klik
			// minder is dan de acceptabele marge, is de klik geregistreerd.
			return 	(verhoudingP1Klik > verhoudingP1P2 - verhoudingMarge) &&
					(verhoudingP1Klik < verhoudingP1P2 + verhoudingMarge);
		}
	}
		
	public class OmlijndeRechthoek : RechthoekigItem, OmlijndItem
	{

		public OmlijndeRechthoek (Rectangle rect, Pen pen)
		{
			rechthoek = rect; lijn = pen;
		}

		public void draw (Graphics g)
		{
			g.DrawRectangle (lijn, rechthoek);
		}

		public void isGeklikt (Point klik)
		{
			// Om dit te berekenen gebruiken we de volgende methode:
			// We gebruiken twee extra rechthoeken, waarvan:
			// 	rechthoek #1 ->  Is gelijk aan het origineel maar met de marge
			//					opgeteld aan alle zijdes,
			//  rechthoek #2 ->  Is gelijk aan het origineel maar met de marge
			//					afgetrokken aan alle zijdes.
			//
			// Wanneer de klik _wel_ binnen rechthoek #1 ligt, maar _niet_
			// binnen rechthoek #2 valt de klik binnen de marge.

			Rectangle groter =
				RechthoekigItem.vergrootRechthoek (rechthoek,
					klikMarge);

			Rectangle kleiner =
				RechthoekigItem.vergrootRechthoek (rechthoek,
					-klikMarge);

			return (GevuldRechthoek.isPuntInRechthoek (klik, groter) &&
					!GevuldRechthoek.isPuntInRechthoek (klik, kleiner));
		}
	}

	public class GevuldRechthoek : RechthoekigItem, GevuldItem
	{
		public static bool isPuntInRechthoek (Point p, Rectangle rechthoek)
		{
			// Simpele berekening die uitwijst of punt 'p'
			// binnen in rechthoek ligt.
			return (p.X > rechthoek.Left &&
					p.X < rechthoek.Right &&
					p.Y > rechthoek.Top &&
					p.Y < rechthoek.Bottom);
		}

		public GevuldRechthoek (Rectangle rect, Brush brush)
		{
			rechthoek = rect; vulling = brush;
		}

		public void draw (Graphics g)
		{
			g.FillRectangle (rechthoek);
		}
						
		public bool isGeklikt (Point klik)
		{
			return isPuntInRechthoek (klik, rechthoek);
		}
	}

	public class OmlijndOvaal : OvaalItem, OmlijndItem
	{
		public OmlijndOvaal (Rectangle rect, Pen pen)
		{
			ovaal = rect; lijn = pen;
		}

		public void draw (Graphics g)
		{
			g.DrawEllipse (lijn, ovaal);
		}

		public bool isGeklikt (Point klik)
		{
			// Om dit te berekenen gebruiken we de volgende methode:
			// We gebruiken twee extra ovalen, waarvan:
			// 	Ovaal #1 -> Is gelijk aan het origineel maar met de marge
			//				opgeteld aan alle zijdes,
			//  Ovaal #2 -> Is gelijk aan het origineel maar met de marge
			//				afgetrokken aan alle zijdes.
			//
			// Wanneer de klik _wel_ binnen ovaal #1 ligt, maar _niet_
			// binnen ovaal #2 valt de klik binnen de marge.

			Rectangle groter =
				RechthoekigItem.vergrootRechthoek (ovaal,
												 klikMarge);

			Rectangle kleiner =
				RechthoekigItem.vergrootRechthoek (ovaal,
												 -klikMarge);
					
			return (GevuldOvaal.isPuntInOvaal (klik, groter) &&
					!GevuldOvaal.isPuntInOvaal (klik, kleiner));
		}
	}

	public class GevuldOvaal : OvaalItem, GevuldItem
	{
		public static void isPuntInOvaal (Point p, Rectangle ovaal)
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

		public GevuldOvaal (Rectangle rect, Brush brush)
		{
			ovaal = rect; vulling = brush;
		}

		public void draw (Graphics g)
		{
			g.FillEllipse (vulling, ovaal);
		}

		public bool isGeklikt (Point klik)
		{
			return isPuntInOvaal (klik, ovaal);
		}
	}
}

