using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public abstract class SchetsbaarItem
    {
        // Marge die wordt aangehouden voor omlijnde items,
        // binnen deze marge wordt een klik nog steeds
        // geregistreerd.
        //
        public const int KlikMarge = 4;

        // Deze functie moet door de subklasse worden ge-
        // implementeerd zodat die zich tekent op g.
        //
        public abstract void Teken(Graphics g);

        // Deze functie moet door de subklasse worden ge-
        // implementeerd zodat die een bool teruggeeft die
        // aangeeft of het gegeven punt 'raak' is.
        //
        public abstract bool IsGeklikt(Point klik);
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

        public Lijn(Point p1, Point p2, Pen pen)
        {
            punt1 = p1;
            punt2 = p2;
            lijn = pen;
        }

        public override void Teken(Graphics g)
        {
            g.DrawLine(lijn, punt1, punt2);
        }

        public override bool IsGeklikt(Point klik)
        {
            // Als de afstand van klik tot de lijn lager
            // is dan de klikmarge --> raak!
            return (AfstandLijnTotPunt(punt1, punt2, klik) < KlikMarge);
        }
    }

    public class OmlijndRechthoek : RechthoekigItem
    {
        public Pen lijn { get; protected set; }

        public OmlijndRechthoek(Rectangle rect, Pen pen)
        {
            rechthoek = rect;
            lijn = pen;
        }

        public override void Teken(Graphics g)
        {
            g.DrawRectangle(lijn, rechthoek);
        }

        public override bool IsGeklikt(Point klik)
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
            //
            Rectangle groter = Wiskunde.VergrootRechthoek(rechthoek, KlikMarge);
            Rectangle kleiner = Wiskunde.VergrootRechthoek(rechthoek, -KlikMarge);

            return (Wiskunde.IsPuntInRechthoek(klik, groter) &&
                    !Wiskunde.IsPuntInRechthoek(klik, kleiner));
        }
    }

    public class GevuldRechthoek : RechthoekigItem
    {
        public Brush vulling { get; protected set; }

        public GevuldRechthoek(Rectangle rect, Brush brush)
        {
            rechthoek = rect;
            vulling = brush;
        }

        public override void Teken(Graphics g)
        {
            g.FillRectangle(vulling, rechthoek);
        }

        public override bool IsGeklikt(Point klik)
        {
            return Wiskunde.IsPuntInRechthoek(klik, rechthoek);
        }
    }

    public class OmlijndOvaal : OvaalItem
    {
        public Pen lijn { get; protected set; }

        public OmlijndOvaal(Rectangle rect, Pen pen)
        {
            ovaal = rect;
            lijn = pen;
        }

        public override void Teken(Graphics g)
        {
            g.DrawEllipse(lijn, ovaal);
        }

        public override bool IsGeklikt(Point klik)
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
            //
            Rectangle groter = Wiskunde.VergrootRechthoek(ovaal, KlikMarge);
            Rectangle kleiner = Wiskunde.VergrootRechthoek(ovaal, -KlikMarge);

            return (Wiskunde.IsPuntInOvaal(klik, groter) &&
                    !Wiskunde.IsPuntInOvaal(klik, kleiner));
        }
    }

    public class GevuldOvaal : OvaalItem
    {
        public Brush vulling { get; protected set; }

        public GevuldOvaal(Rectangle rect, Brush brush)
        {
            ovaal = rect;
            vulling = brush;
        }

        public override void Teken(Graphics g)
        {
            g.FillEllipse(vulling, ovaal);
        }

        public override bool IsGeklikt(Point klik)
        {
            return Wiskunde.IsPuntInOvaal(klik, ovaal);
        }
    }

    public class GetekendeLijn : SchetsbaarItem
    {
        private LinkedList<Lijn> subLijnen = new LinkedList<Lijn>();

        public Point? LaatstePunt
        {
            get
            {
                return (subLijnen.Count > 0) ? subLijnen.Last.Value.punt2 : (Point?)null;
            }
        }

        public GetekendeLijn(LinkedList<Lijn> lijntjes)
        {
            subLijnen = lijntjes;
        }

        public GetekendeLijn()
        {
            subLijnen = new LinkedList<Lijn>();
        }

        public void VoegLijntjeToe(Lijn lijntje)
        {
            subLijnen.AddLast(lijntje);
        }

        public override void Teken(Graphics g)
        {
            foreach (Lijn l in subLijnen)
                l.Teken(g);
        }

        public override bool IsGeklikt(Point klik)
        {
            // Als een van de subLijnen is geklikt, zijn
            // ze allemaal geklikt.
            foreach (Lijn l in subLijnen)
                if (l.IsGeklikt(klik))
                    return true;

            return false;
        }
    }

    public class Letter : RechthoekigItem
    {
        private string tekst;
        private Brush vulling;

        public static Font Lettertype = new Font("Tahoma", 40);

        public Letter(char karakter, Point punt, Color kleur, SizeF kGrootte)
        {
            tekst = karakter.ToString();
            vulling = new SolidBrush(kleur);

            // Sla rechthoek om letter op.
            rechthoek = new Rectangle(punt.X, punt.Y, (int)kGrootte.Width, (int)kGrootte.Height);
        }

        public override void Teken(Graphics g)
        {
            g.DrawString(tekst,
                         Lettertype,
                         vulling,
                         new Point(rechthoek.X, rechthoek.Y),
                         StringFormat.GenericTypographic);

            // Teken een rechthoek om de letter
            g.DrawRectangle(Pens.Gray, rechthoek);
        }

        public override bool IsGeklikt(Point klik)
        {
            return Wiskunde.IsPuntInRechthoek(klik, rechthoek);
        }
    }
}

