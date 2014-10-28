using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace SchetsEditor
{
	class Schets
	{
		private Bitmap bitmap;

		private List<SchetsbaarItem> items
			= new List<SchetsbaarItem>();

		public Schets ()
		{
			bitmap = new Bitmap (1, 1);
		}

		public Bitmap Bitmap {
			get { return bitmap; }
		}

		//public Graphics BitmapGraphics {
		//	get { return Graphics.FromImage (bitmap); }
		//}
		// TODO

		public void VeranderAfmeting (Size sz)
		{
			if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
			{
				Bitmap nieuw = new Bitmap (	Math.Max (sz.Width, bitmap.Size.Width),
                                         	Math.Max (sz.Height, bitmap.Size.Height)
				                           );

				Graphics gr = Graphics.FromImage (nieuw);
				gr.FillRectangle (Brushes.White, 0, 0, sz.Width, sz.Height);
				gr.DrawImage (bitmap, 0, 0);

				bitmap = nieuw;
			}
		}

		public void Teken (Graphics gr)
		{


			gr.DrawImage (bitmap, 0, 0);
		}

		public void Schoon ()
		{
			Graphics gr = Graphics.FromImage (bitmap);
			gr.FillRectangle (Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
		}

		public void Roteer ()
		{
			bitmap.RotateFlip (RotateFlipType.Rotate90FlipNone);
		}
	}
}
