using System;
using System.Drawing;

namespace SchetsEditor
{
    /// Algemene klasse die fungeert als uitbreiding op Math in
    /// de zin dat het algemene wiskundige functies implementeerd
    /// die nodig zijn voor 2D berekeningen in dit project.
    ///
    class Wiskunde
    {
        /// Berekent de afstand tussen p1 en p2
        ///
        public static double Afstand(Point p1, Point p2)
        {
            return Math.Sqrt(
                Math.Pow(Math.Abs(p1.X - p2.X), 2) +
                Math.Pow(Math.Abs(p1.Y - p2.Y), 2)
            );
        }

        /*
         * Een aantal veelvoorkomende wiskunde operaties
         * voor Punten geimplementeerd.
         */

        public static Point PuntPlus(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point PuntMin(Point p1, Point p2)
        {
            return PuntPlus(p1, new Point(-p2.X, -p2.Y));
        }

        /// Berekent de afstand tussen het punt p en
        /// de lijn tussen punt1 en punt2.
        ///
        public static double AfstandLijnTotPunt(Point lp1, Point lp2, Point p)
        {
            // Met dit algoritme kan worden berekent waar de projectie
            // van punt p op de lijn (lp1, lp2) ligt.
            double lQ =
                ( (lp2.X-lp1.X)*(p.X-lp1.X) + (lp2.Y-lp1.Y)*(p.Y-lp1.Y) )
                                            /
                ( Math.Pow (lp2.X-lp1.X, 2) + Math.Pow (lp2.Y-lp1.Y, 2) );

            if (lQ <= 0.0)
            {
                // De klik ligt voorbij lp1, bereken die afstand!
                return Afstand(p, lp1);
            }
            else if (lQ >= 1.0)
            {
                // De klik ligt voorbij lp2, bereken die afstand!
                return Afstand(p, lp2);
            }
            else
            {
                // De projectie van punt Pklik op de lijn ligt tussen
                // lp1 en lp2, bereken waar, en bereken afstand
                // tussen Pklik en Pprojectie
                return Math.Sqrt(
                          Math.Pow(p.X-lp1.X-(lQ*(lp2.X-lp1.X)), 2)
                                                +
                          Math.Pow(p.Y-lp1.Y-(lQ*(lp2.Y-lp1.Y)), 2)
                       );
            }
        }

        /// Transformeert twee punten in een Rectangle
        /// waarvan de twee punten tegenoverstaande hoek-
        /// punten zijn.
        ///
        public static Rectangle MaakRectangleVanPunten(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p2.X - p1.X),
                Math.Abs(p2.Y - p1.Y));
        }

        /// Past de coordinaten van de zijdes van rechthoek
        /// r aan met waarde d.
        ///
        public static Rectangle VergrootRechthoek(Rectangle r, int d)
        {
            return new Rectangle(
                r.Left - d, r.Top - d,
                r.Width + (2*d), r.Bottom + (2*d)
            );
        }

        /// Geeft een nieuwe Rectangle terug die 90 graden naar
        /// rechts is gedraaid.
        ///
        public static Rectangle DraaiRechthoek(Rectangle r, Size grootte)
        {
            Point nieuwPunt = new Point(grootte.Height - r.Bottom, r.Left);
            Size nieuweGrootte = new Size(r.Height, r.Width);

            return new Rectangle(nieuwPunt, nieuweGrootte);
        }

        /// Geeft aan of punt p binnen het rechthoek ligt.
        ///
        public static bool IsPuntInRechthoek(Point p, Rectangle rechthoek)
        {
            // Simpele berekening die uitwijst of punt 'p'
            // binnen in rechthoek ligt.
            return (p.X >= rechthoek.Left &&
                    p.X <= rechthoek.Right &&
                    p.Y >= rechthoek.Top &&
                    p.Y <= rechthoek.Bottom);
        }

        /// Geeft aan of punt p binnen het gegeven ovaal
        /// ligt.
        ///
        public static bool IsPuntInOvaal(Point p, Rectangle ovaal)
        {
            // Bereken middelpunt, gernomalizeerd, dus
            // alsof de ovaal 
            Point mp = new Point(
                ovaal.Left + (ovaal.Width / 2),
                ovaal.Top + (ovaal.Height / 2)
            );

            double xComp = (double)(Math.Pow(p.X - mp.X, 2) / Math.Pow(ovaal.Width / 2, 2));
            double yComp = (double)(Math.Pow(p.Y - mp.Y, 2) / Math.Pow(ovaal.Height / 2, 2));

            return (double)(xComp + yComp) < (double)1;
        }

        /// Kan op basis van de hoek, de linksbovenhoek vinden waar
        /// die in Rectangle r op hoek = 0 zou zitten.
        /// 
        public static Point KrijgOrigineleLinksBovenHoek(Rectangle r, int hoek)
        {
            switch (hoek)
            {
                case 0:
                    return new Point(r.X, r.Y);
                case 90:
                    return new Point(r.X + r.Width, r.Y);
                case 180:
                    return new Point(r.X + r.Width, r.Y + r.Height);
                case 270:
                    return new Point(r.X, r.Y + r.Height);
                default: 
                    throw new ArgumentException("Hoek moet een veelvoud van 90 zijn, minder of gelijk aan 270, of 0.");
            }
        }
    }
}

