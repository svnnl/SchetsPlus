using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    /*
     *  TODO: BUG: Soms, wanneer een aantal items op de lijst staan, werkt het verwijderen niet,
     *             vooral bij de items OmlijndRechthoek en OmlijndOvaal.
     */

    [Serializable()]
    public abstract class SchetsbaarItem
    {
        // Marge die wordt aangehouden voor omlijnde items,
        // binnen deze marge wordt een klik nog steeds
        // geregistreerd.
        //
        public const int KlikMarge = 4;

        // Kleur van object
        //
        protected Color kleur;

        // Deze functie moet door de subklasse worden ge-
        // implementeerd zodat die zich tekent op g.
        //
        public abstract void Teken(Graphics g);

        // Deze functie moet door de subklasse worden ge-
        // implementeerd zodat die een bool teruggeeft die
        // aangeeft of het gegeven punt 'raak' is.
        //
        public abstract bool IsGeraakt(Point p);

        // Deze functie zorgt ervoor dat het item 90 graden
        // naar rechts wordt gedraaid.
        //
        public abstract void Draai(Size grootte);
    }

    [Serializable()]
    public abstract class RechthoekigItem : SchetsbaarItem
    {
        public Rectangle rechthoek { get; protected set; }

        public override void Draai(Size grootte)
        {
            rechthoek = Wiskunde.DraaiRechthoek(rechthoek, grootte);
        }
    }

    [Serializable()]
    public abstract class EllipsvormigItem : SchetsbaarItem
    {
        public Rectangle ovaal { get; protected set; }

        public override void Draai(Size grootte)
        {
            ovaal = Wiskunde.DraaiRechthoek(ovaal, grootte);
        }
    }

    [Serializable()]
    public class Lijn : SchetsbaarItem
    {
        public Point punt1 { get; protected set; }
        public Point punt2 { get; protected set; }

        private int dikte;

        public Lijn(Point p1, Point p2, Color k, int d)
        {
            punt1 = p1;
            punt2 = p2;
            kleur = k;
            dikte = d;
        }

        public override void Teken(Graphics g)
        {
            g.DrawLine(new Pen(kleur, dikte), punt1, punt2);
        }

        public override bool IsGeraakt(Point p)
        {
            // Als de afstand van klik tot de lijn lager
            // is dan de klikmarge --> raak!
            return Wiskunde.AfstandLijnTotPunt(punt1, punt2, p) < KlikMarge;
        }

        public override void Draai(Size grootte)
        {
            punt1 = new Point(grootte.Height - punt1.Y, punt1.X);
            punt2 = new Point(grootte.Height - punt2.Y, punt2.X);
        }
    }

    [Serializable()]
    public class Rechthoek : RechthoekigItem
    {
        private int dikte;

        public Rechthoek(Rectangle rect, Color k, int d)
        {
            rechthoek = rect;
            kleur = k;
            dikte = d;
        }

        public override void Teken(Graphics g)
        {
            g.DrawRectangle(new Pen(kleur, dikte), rechthoek);
        }

        public override bool IsGeraakt(Point p)
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

            return (Wiskunde.IsPuntInRechthoek(p, groter) &&
                    !Wiskunde.IsPuntInRechthoek(p, kleiner));
        }
    }

    [Serializable()]
    public class VolRechthoek : RechthoekigItem
    {
        public VolRechthoek(Rectangle rect, Color k)
        {
            rechthoek = rect;
            kleur = k;
        }

        public override void Teken(Graphics g)
        {
            g.FillRectangle(new SolidBrush(kleur), rechthoek);
        }

        public override bool IsGeraakt(Point p)
        {
            return Wiskunde.IsPuntInRechthoek(p, rechthoek);
        }
    }

    [Serializable()]
    public class Ellips : EllipsvormigItem
    {
        private int dikte;

        public Ellips(Rectangle rect, Color k, int d)
        {
            ovaal = rect;
            kleur = k;
            dikte = d;
        }

        public override void Teken(Graphics g)
        {
            g.DrawEllipse(new Pen(kleur, dikte), ovaal);
        }

        public override bool IsGeraakt(Point p)
        {
            /*
             * Bereken dit mbv de groter-kleiner-methode, ook
             * gebruikt in rechthoek.
             */

            Rectangle groter = Wiskunde.VergrootRechthoek(ovaal, KlikMarge);
            Rectangle kleiner = Wiskunde.VergrootRechthoek(ovaal, -KlikMarge);

            return (Wiskunde.IsPuntInOvaal(p, groter) &&
                    !Wiskunde.IsPuntInOvaal(p, kleiner));
        }
    }

    [Serializable()]
    public class VolEllips : EllipsvormigItem
    {
        public VolEllips(Rectangle rect, Color k)
        {
            ovaal = rect;
            kleur = k;
        }

        public override void Teken(Graphics g)
        {
            g.FillEllipse(new SolidBrush(kleur), ovaal);
        }

        public override bool IsGeraakt(Point p)
        {
            return Wiskunde.IsPuntInOvaal(p, ovaal);
        }
    }

    [Serializable()]
    public class GetekendeLijn : SchetsbaarItem
    {
        private LinkedList<Lijn> subLijnen = new LinkedList<Lijn>();

        public Point? LaatstePunt {
            get { return (subLijnen.Count > 0) ? subLijnen.Last.Value.punt2 : (Point?)null; }
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

        public override bool IsGeraakt(Point p)
        {
            // Als een van de subLijnen is geklikt, zijn
            // ze allemaal geklikt.
            foreach (Lijn l in subLijnen)
            {
                if (l.IsGeraakt(p))
                    return true;
            }

            return false;
        }

        public override void Draai(Size grootte)
        {
            foreach (Lijn l in subLijnen)
                l.Draai(grootte);
        }
    }

    [Serializable()]
    public class Letter : RechthoekigItem
    {
        private string tekst;
        private int draaihoek;

        public static Font Lettertype = new Font("Tahoma", 40);

        public Letter(char karakter, Point punt, Color k, SizeF kGrootte)
        {
            tekst = karakter.ToString();
            kleur = k;
            draaihoek = 0;

            // Sla rechthoek om letter op.
            rechthoek = new Rectangle(punt.X, punt.Y, (int)kGrootte.Width, (int)kGrootte.Height);
        }

        public override void Teken(Graphics g)
        {
            // Draai letter onder bepaalde hoek
            g.TranslateTransform(Wiskunde.KrijgOrigineleLinksBovenHoek(rechthoek, draaihoek).X,
                Wiskunde.KrijgOrigineleLinksBovenHoek(rechthoek, draaihoek).Y);
            g.RotateTransform((int)draaihoek);

            g.DrawString(tekst,
                         Lettertype,
                         new SolidBrush(kleur),
                         0, 0,
                         StringFormat.GenericTypographic);

            // Draai alles terug, zodat latere teken-acties hier
            // geen last van hebben.
            g.RotateTransform((int)-draaihoek);
            g.TranslateTransform(-Wiskunde.KrijgOrigineleLinksBovenHoek(rechthoek, draaihoek).X,
                -Wiskunde.KrijgOrigineleLinksBovenHoek(rechthoek, draaihoek).Y);

            // Teken een rechthoek om de letter
            g.DrawRectangle(Pens.Gray, rechthoek);
        }

        public override bool IsGeraakt(Point p)
        {
            return Wiskunde.IsPuntInRechthoek(p, rechthoek);
        }

        public override void Draai(Size grootte)
        {
            rechthoek = Wiskunde.DraaiRechthoek(rechthoek, grootte);
            draaihoek = (draaihoek + 90) % 360;
        }
    }
}

