using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;

        private List<SchetsbaarItem> items = new List<SchetsbaarItem>();
        private SchetsbaarItem overlay = null;

        private bool isVeranderd = true;

        public Schets()
        {
            bitmap = new Bitmap(1, 1);
        }

        public Bitmap Bitmap { get { return bitmap; } }
        public bool IsVeranderd { get { return isVeranderd; } set { isVeranderd = value;  } }

        public void VoegSchetsbaarItemToe(SchetsbaarItem item)
        {
            items.Add(item);
            isVeranderd = true;
        }

        public void VerwijderSchetsbaarItem(Point p)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].IsGeraakt(p))
                {
                    items.RemoveAt(i);
                    isVeranderd = true;

                    // Is het item gevonden? En verwijderd?
                    // Dan zijn we klaar.
                    break;
                }
            }
        }

        public void SchetsbaarItemBovenopLeggen(Point p)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].IsGeraakt(p))
                {
                    items.Add(items[i]);
                    items.RemoveAt(i);
                    isVeranderd = true;

                    // Is het item gevonden? En verwijderd?
                    // Dan zijn we klaar.
                    break;
                }
            }
        }

        public void ZetOverlayItem(SchetsbaarItem item)
        {
            overlay = item;
        }

        public void Serializeer(string bestandsnaam)
        {
            Stream uitStream = File.Open(bestandsnaam, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(uitStream);

            foreach (SchetsbaarItem item in items)
            {
                writer.Write(item.GetType().ToString());
                item.Serialiseer(writer);
                writer.Write('\n');
            }

            writer.Close();
            uitStream.Close();
        }

        public void Deserializeer(string bestandsnaam)
        {
            Stream inStream = File.OpenRead(bestandsnaam);
            BinaryReader reader = new BinaryReader(inStream);

            while(inStream.CanRead)
            {
                try
                {
                    switch (reader.ReadString())
                    {
                        case "SchetsEditor.Lijn": items.Add(new Lijn(reader)); break;
                        case "SchetsEditor.GetekendeLijn": items.Add(new GetekendeLijn(reader)); break;
                        case "SchetsEditor.Rechthoek": items.Add(new Rechthoek(reader)); break;
                        case "SchetsEditor.VolRechthoek": items.Add(new VolRechthoek(reader)); break;
                        case "SchetsEditor.Ellips": items.Add(new Ellips(reader)); break;
                        case "SchetsEditor.VolEllips": items.Add(new VolEllips(reader)); break;
                        case "SchetsEditor.Letter": items.Add(new Letter(reader)); break;
                    }
                }
                catch (EndOfStreamException)
                {
                    break;
                }

                if (reader.ReadChar() != '\n')
                    break;
            }

            reader.Close();
            inStream.Close();
        }

        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap( Math.Max(sz.Width, bitmap.Size.Width)
                                           , Math.Max(sz.Height, bitmap.Size.Height)
                                           );

                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);

                bitmap = nieuw;
            }
        }

        public void Teken(Graphics gr)
        {
            Graphics bitmapGraphics = Graphics.FromImage(bitmap);

            bitmapGraphics.Clear(Color.White);
            bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (SchetsbaarItem item in items)
                item.Teken(bitmapGraphics);

            // Teken de 'overlay' dit is een tijdelijk item dat wordt
            // laten zien tijdens een bewerking als preview van hoe
            // de 'echte' bewerking er op de schets zal uitzien.
            if (overlay != null)
                overlay.Teken(bitmapGraphics);

            /* Alles is op de bitmap getekend, die wordt op het
             * scherm laten zien. */

            gr.DrawImage(bitmap, 0, 0);
        }

        public void Schoon()
        {
            items.Clear();
        }

        public void Roteer()
        {
            foreach (SchetsbaarItem item in items)
                item.Draai(Bitmap.Size);
        }
    }
}
