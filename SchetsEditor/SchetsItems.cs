using System;
using System.Drawing;

namespace SchetsEditor
{
	public abstract class SchetsbaarItem
	{
		// Marge die wordt aangehouden voor omlijnde items,
		// binnen deze marge wordt een klik nog steeds
		// geregistreerd.
		//
		protected const int klikMarge = 2;

		// Deze functie moet door de subklasse worden ge-
		// implementeerd zodat die zich tekent op g.
		//
		public abstract void Teken (Graphics g);

		// Deze functie moet door de subklasse worden ge-
		// implementeerd zodat die een bool teruggeeft die
		// aangeeft of het gegeven punt 'raak' is.
		//
		public abstract bool IsGeklikt (Point klik);
	}
		
	public abstract class RechthoekigItem : SchetsbaarItem
	{
		public Rectangle rechthoek { get; protected set; }
	}

	public abstract class OvaalItem : SchetsbaarItem
	{
		public Rectangle ovaal { get; protected set; }
	}

	public class Lijn : SchetsbaarItem
	{
		public Pen lijn { get; protected set; }

		public Point punt1 { get; protected set; }
		public Point punt2 { get; protected set; }

		public Lijn (Point p1, Point p2, Pen pen)
		{
			punt1 = p1; punt2 = p2; lijn = pen;
		}

		public override void Teken (Graphics g)
		{
			g.DrawLine (lijn, punt1, punt2);
		}

		public override bool IsGeklikt (Point klik)
		{
			// Als de afstand van klik tot de lijn lager
			// is dan de klikmarge --> raak!
			return (AfstandTotPunt (klik) < klikMarge);
		}

		// Berekent de afstand tussen het punt p en
		// de lijn tussen punt1 en punt2.
		//
		private double AfstandTotPunt (Point p)
		{
			// Afstand tussen de twee punten van de lijn,
			// in het kwadraat ...
			double dp1p2
				= Math.Sqrt (punt1.X - punt2.X) +
				  Math.Sqrt (punt1.X - punt2.X);

			if (dp1p2 == 0.0)
			{
				// Deze lijn is een punt, dus de afstand
				// is makkelijk te berekenen ...
				return Wiskunde.Afstand (p, punt1);
			}

			// Geeft indicatie van positie van P ten overstaande
			// van de lijn.
			double indicatie
				= Wiskunde.Dot2 (Wiskunde.PuntMin (p, punt1),
								 Wiskunde.PuntMin (punt2, punt1));

			if (indicatie < 0.0)
			{
				// P zit aan deze zijde van het lijnsegment,
				// afstand is als volgt:
				return Wiskunde.Afstand (p, punt1);
			}
			else if (indicatie > 1.0)
			{
				// P zit aan de andere zijde, afstand:
				return Wiskunde.Afstand (p, punt2);
			}
			else
			{
				/*
				 * P ligt ongeveer op de lijn, de afstand
				 * is de afstand tussen punt p en zijn projectie
				 * op de lijn tussen p1 en p2.
				 */

				Point projectie = new Point (
					(int) (punt1.X + indicatie * (punt2.X - punt1.X)),
					(int) (punt1.Y + indicatie * (punt2.Y - punt1.Y))
				);

				return Wiskunde.Afstand (p, projectie);
			}
		}
	}
		
	public class OmlijndeRechthoek : RechthoekigItem
	{
		public Pen lijn { get; protected set; }

		public OmlijndeRechthoek (Rectangle rect, Pen pen)
		{
			rechthoek = rect; lijn = pen;
		}

		public override void Teken (Graphics g)
		{
			g.DrawRectangle (lijn, rechthoek);
		}

		public override bool IsGeklikt (Point klik)
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
				Wiskunde.VergrootRechthoek (rechthoek,
											klikMarge);

			Rectangle kleiner =
				Wiskunde.VergrootRechthoek (rechthoek,
											-klikMarge);

			return (Wiskunde.IsPuntInRechthoek (klik, groter) &&
					!Wiskunde.IsPuntInRechthoek (klik, kleiner));
		}
	}

	public class GevuldRechthoek : RechthoekigItem
	{
		public Brush vulling { get; protected set; }

		public GevuldRechthoek (Rectangle rect, Brush brush)
		{
			rechthoek = rect; vulling = brush;
		}

		public override void Teken (Graphics g)
		{
			g.FillRectangle (vulling, rechthoek);
		}
						
		public override bool IsGeklikt (Point klik)
		{
			return Wiskunde.IsPuntInRechthoek (klik, rechthoek);
		}
	}

	public class OmlijndOvaal : OvaalItem
	{
		public Pen lijn { get; protected set; }

		public OmlijndOvaal (Rectangle rect, Pen pen)
		{
			ovaal = rect; lijn = pen;
		}

		public override void Teken (Graphics g)
		{
			g.DrawEllipse (lijn, ovaal);
		}

		public override bool IsGeklikt (Point klik)
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
				Wiskunde.VergrootRechthoek (ovaal,
											klikMarge);

			Rectangle kleiner =
				Wiskunde.VergrootRechthoek (ovaal,
											-klikMarge);
					
			return (Wiskunde.IsPuntInOvaal (klik, groter) &&
					!Wiskunde.IsPuntInOvaal (klik, kleiner));
		}
	}

	public class GevuldOvaal : OvaalItem
	{
		public Brush vulling { get; protected set; }

		public GevuldOvaal (Rectangle rect, Brush brush)
		{
			ovaal = rect; vulling = brush;
		}

		public override void Teken (Graphics g)
		{
			g.FillEllipse (vulling, ovaal);
		}

		public override bool IsGeklikt (Point klik)
		{
			return Wiskunde.IsPuntInOvaal (klik, ovaal);
		}
	}

	/*
	 * TODO
	 *  - Tekst
	 *  - Normaal getekende lijn
	 *     In het omliggende blok van de klik simpelweg alle
	 *     pixels in een radius van klikMarge controleren.
	 */
}

