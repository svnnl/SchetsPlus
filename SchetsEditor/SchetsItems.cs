using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SchetsEditor
{  
    public class SchetsbaarItem
    {
        /// Marge die wordt aangehouden voor omlijnde items,
        /// binnen deze marge wordt een klik nog steeds
        /// geregistreerd.
        ///
        public const int KlikMarge = 4;

        /// Kleur van object
        ///
        protected Color kleur;

        /// Schrijf object weg op stream w.
        ///
        public virtual void Serialiseer(BinaryWriter w)
        {
            w.Write((Int32)kleur.A);
            w.Write((Int32)kleur.R);
            w.Write((Int32)kleur.G);
            w.Write((Int32)kleur.B);
        }

        /// Lees object uit stream r.
        ///
        public virtual void Deserialiseer(BinaryReader r)
        {
            kleur = Color.FromArgb(
                r.ReadInt32(), r.ReadInt32(),
                r.ReadInt32(), r.ReadInt32()
            );
        }

        /// Deze functie moet door de subklasse worden ge-
        /// implementeerd zodat die zich tekent op g.
        ///
        public virtual void Teken(Graphics g) { }

        /// Deze functie moet door de subklasse worden ge-
        /// implementeerd zodat die een bool teruggeeft die
        /// aangeeft of het gegeven punt 'raak' is.
        ///
        public virtual bool IsGeraakt(Point p) { return false;  }

        /// Deze functie zorgt ervoor dat het item 90 graden
        /// naar rechts wordt gedraaid.
        ///
        public virtual void Draai(Size grootte) { }
    }
    
    public abstract class RechthoekigItem : SchetsbaarItem
    {
        public Rectangle rechthoek { get; protected set; }

        public override void Draai(Size grootte)
        {
            rechthoek = Wiskunde.DraaiRechthoek(rechthoek, grootte);
        }

        public override void Serialiseer(BinaryWriter w)
        {
            base.Serialiseer(w);
 	        w.Write((Int32)rechthoek.X);
            w.Write((Int32)rechthoek.Y);
            w.Write((Int32)rechthoek.Width);
            w.Write((Int32)rechthoek.Height);
        }

        public override void Deserialiseer(BinaryReader r)
        {
            base.Deserialiseer(r);
 	        rechthoek = new Rectangle(
                r.ReadInt32(), r.ReadInt32(),
                r.ReadInt32(), r.ReadInt32()
            );
        }
    }
    
    public abstract class EllipsvormigItem : SchetsbaarItem
    {
        public Rectangle ovaal { get; protected set; }

        public override void Draai(Size grootte)
        {
            ovaal = Wiskunde.DraaiRechthoek(ovaal, grootte);
        }

        public override void Serialiseer(BinaryWriter w)
        {
            base.Serialiseer(w);
 	        w.Write((Int32)ovaal.X);
            w.Write((Int32)ovaal.Y);
            w.Write((Int32)ovaal.Width);
            w.Write((Int32)ovaal.Height);
        }

        public override void Deserialiseer(BinaryReader r)
        {
            base.Deserialiseer(r);
 	        ovaal = new Rectangle(
                r.ReadInt32(), r.ReadInt32(),
                r.ReadInt32(), r.ReadInt32()
            );
        }
    }
    
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

        public Lijn(BinaryReader r)
        {
            Deserialiseer(r);
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

        public override void Serialiseer(BinaryWriter w)
        {
            base.Serialiseer(w);
 	        w.Write((Int32)punt1.X);
            w.Write((Int32)punt1.Y);
            w.Write((Int32)punt2.X);
            w.Write((Int32)punt2.Y);
            w.Write((Int32)dikte);
        }

        public override void Deserialiseer(BinaryReader r)
        {
            base.Deserialiseer(r);
            punt1 = new Point(r.ReadInt32(), r.ReadInt32());
            punt2 = new Point(r.ReadInt32(), r.ReadInt32());
            dikte = r.ReadInt32();
        }
    }
    
    public class Rechthoek : RechthoekigItem
    {
        private int dikte;

        public Rechthoek(BinaryReader r)
        {
            Deserialiseer(r);
        }

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

        public override void Serialiseer(BinaryWriter w)
        {
            base.Serialiseer(w);
            w.Write((Int32)dikte);
        }

        public override void Deserialiseer(BinaryReader r)
        {
            base.Deserialiseer(r);
            dikte = r.ReadInt32();
        }
    }
    
    public class VolRechthoek : RechthoekigItem
    {
        public VolRechthoek(BinaryReader r)
        {
            Deserialiseer(r);
        }

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
    
    public class Ellips : EllipsvormigItem
    {
        private int dikte;

        public Ellips(BinaryReader r)
        {
            Deserialiseer(r);
        }

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

        public override void Serialiseer(BinaryWriter w)
        {
            base.Serialiseer(w);
            w.Write((Int32)dikte);
        }

        public override void Deserialiseer(BinaryReader r)
        {
            base.Deserialiseer(r);
            dikte = r.ReadInt32();
        }
    }
 
    public class VolEllips : EllipsvormigItem
    {
        public VolEllips(BinaryReader r)
        {
            Deserialiseer(r);
        }

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
    
    public class GetekendeLijn : SchetsbaarItem
    {
        private LinkedList<Lijn> subLijnen = new LinkedList<Lijn>();

        public Point? LaatstePunt {
            get { return (subLijnen.Count > 0) ? subLijnen.Last.Value.punt2 : (Point?)null; }
        }

        public GetekendeLijn(BinaryReader r)
        {
            Deserialiseer(r);
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

        public override void Serialiseer(BinaryWriter w)
        {
            base.Serialiseer(w);
            w.Write((Int32)subLijnen.Count);
            foreach (Lijn l in subLijnen)
                l.Serialiseer(w);
        }

        public override void Deserialiseer(BinaryReader r)
        {
            base.Deserialiseer(r);
            int n = r.ReadInt32();
            for (int i = 0; i < n; i++)
                subLijnen.AddLast(new Lijn(r));
        }
    }
    
    public class Letter : RechthoekigItem
    {
        public static Font Lettertype = new Font("Tahoma", 40);

        private char karakter;
        private int draaihoek;

        public Letter(BinaryReader r)
        {
            Deserialiseer(r);
        }

        public Letter(char kar, Point punt, Color k, SizeF kGrootte)
        {
            karakter = kar;
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

            g.DrawString(karakter.ToString(),
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

        public override void Serialiseer(BinaryWriter w)
        {
            base.Serialiseer(w);
            w.Write(karakter);
            w.Write((Int32)draaihoek);
        }

        public override void Deserialiseer(BinaryReader r)
        {
            base.Deserialiseer(r);
            karakter = r.ReadChar();
            draaihoek = r.ReadInt32();
        }
    }
}

