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
		public Pen lijn { get; protected set; }
	}

	public class GevuldItem
	{
		public Brush vulling { get; protected set; }
	}

	public class VierhoekigItem
	{
		public Rectangle vierhoek { get; protected set; }
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
		
	public class OmlijndeVierhoek : VierhoekigItem, OmlijndItem
	{
		public static Rectangle maakRectangleVanPunten (Point p1, Point p2)
		{
			return new Rectangle (
				p1.X,
				p2.Y,
				p2.X - p1.X,
				p2.Y - p1.Y);
		}

		public OmlijndeVierhoek (Rectangle rect, Pen pen)
		{
			vierhoek = rect; lijn = pen;
		}

		public void draw (Graphics g)
		{
			g.DrawRectangle (lijn, vierhoek);
		}

		public void isGeklikt (Point klik)
		{
			// TODO
			throw new NotImplementedException ();
		}
	}

	public class GevuldVierhoek : VierhoekigItem, GevuldItem
	{
		public GevuldVierhoek (Rectangle rect, Brush brush)
		{
			vierhoek = rect; vulling = brush;
		}

		public void draw (Graphics g)
		{
			g.FillRectangle (vierhoek);
		}
						
		public bool isGeklikt (Point klik)
		{
			return (klik.X > vierhoek.Left &&
				klik.X < vierhoek.Right &&
				klik.Y > vierhoek.Top &&
				klik.Y < vierhoek.Bottom);
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
			// TODO
			throw new NotImplementedException ();
		}
	}

	public class GevuldOvaal : OvaalItem, GevuldItem
	{
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
			Point middelpunt = new Point (
				(ovaal.Left + ovaal.Right) / 2,
				(ovaal.Top + ovaal.Bottom) / 2
			);

			int afstand = Math.Sqrt (
				Math.Pow (Math.Abs (klik.X - middelpunt.X)),
				Math.Pow (Math.Abs (klik.Y - middelpunt.Y))
			);

			// ... TODO

			throw new NotImplementedException ();
		}
	}
}

