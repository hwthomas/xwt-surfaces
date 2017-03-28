//
// DrawingSurface.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
//  	 Hywel Thomas <hywel.w.thomas@gmail.com>
//
// Copyright (c) 2013 Xamarin, Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Xwt;
using Xwt.Drawing;

namespace Samples
{
	public class DrawingSurface: VBox
	{
		public DrawingSurface ()
		{
			Button run = new Button ("Run Test");
			PackStart (run);

			Label results = new Label (" Size (1 x 1)\t DrawCalls: 0 \t Time: 0mS");
			PackStart (results);

			var st = new SurfaceTest ();
			PackStart (st, true, true);

			run.Clicked += delegate {
				run.Sensitive = false;
				st.StartTest ();
			};

			st.TestFinished += delegate {
				run.Sensitive = true;
				results.Text = string.Format (" Size ({0}x{1}) \tDrawCalls: {2} \tTime: {3} mS", st.Width, st.Height, st.DrawCalls, st.DrawTime);
				Console.WriteLine (results.Text);
			};
		}
	}


	class SurfaceTest: Canvas
	{
		bool testMode = false;

		Surface cache = null;

		public int DrawCalls { get; private set; }			// number of drawing calls
		public double Width { get; private set; }			// Canvas dimensions
		public double Height { get; private set; }
		public double DrawTime { get; private set; }		// drawing directly to Canvas
		public double BitmapTime { get; private set; }		// 
		public double ImageTime { get; private set; }
		public double SurfaceTime { get; private set; }

		public event EventHandler TestFinished;

		public SurfaceTest ()
		{
			Width = Bounds.Width;
			Height = Bounds.Height;
			WidthRequest = Width;
			HeightRequest = Height;
			DrawCalls = 100;
		}

		public void StartTest ()
		{
			Width = Bounds.Width;
			Height = Bounds.Height;
			testMode = true;
			QueueDraw ();
		}

		protected override void OnBoundsChanged ()
		{
			base.OnBoundsChanged ();
			cache = null;		// ensure new cache is created
		}

		protected override void OnDraw (Context ctx, Rectangle dirtyRect)
		{
			Context sc = null;	// cache Context
			Width = Bounds.Width;
			Height = Bounds.Height;
			// If creating cache from Context ctx, it can only be set up here on the first OnDraw call
			// By initialising it to null when SurfaceTest is created, any type of surface cache can be used
			if (cache == null) {
				Size s = new Size (Width, Height);
				//cache = new Surface(s);       		// default surface
				//cache = new Surface (s, this);		// widget-compatible surface
				cache = new Surface (s, ctx);		// surface compatible with Context (ctx)
				sc = cache.Context;
				DrawScene (sc, Width, Height);		// use context to draw (once) to cache
			}
			ctx.DrawSurface (cache, 0, 0);			// copy from cache to Canvas on first OnDraw call

			if (!testMode)
				return;

			// Various timed drawings/copies for comparison
			//DrawTime = TimedDraw (delegate { DrawScene (ctx, Width, Height);});		// draw scene direct to Canvas

			//DrawTime = TimedDraw (delegate { DrawScene (sc, Width, Height);});		// draw scene to Cache

			//BitmapTime = TimedDraw (delegate { ctx.DrawImage (bitmap, 0, 0);});		// copy image from Bitmap cache
			//ImageTime = TimedDraw (delegate { ctx.DrawImage (vectorImage, 0, 0);});	// copy image from Vector cache

			DrawTime = TimedDraw (delegate { ctx.DrawSurface (cache, 0, 0); });  		// copy from Surface cache

			testMode = false;
			if (TestFinished != null)
				TestFinished (this, EventArgs.Empty);
		}

		double TimedDraw (Action draw)
		{
			var t = DateTime.Now;
			var n = 0;
			while ( n < this.DrawCalls) {
				draw ();
				n++;
			}
			return (DateTime.Now - t).TotalMilliseconds;
		}

		void DrawScene (Context ctx, double width, double height)
		{
			// Draw a fairly complicated 'background' scene

			double iterations = 15;
			double centre;
			double radius;
			double x0;
			double wn;

			ctx.Save ();
			ctx.Scale (1.0, Height/Width);  // scale for width and height
			centre = Width / 2.0;
			ctx.SetLineWidth (1.0);			// Note - this is also scaled
			   
			for (int n = 1; n <= iterations; n += 1) {
				// draw all figures based on width, as height scaling is in place
				// (1) draw rectangles
				ctx.SetColor (Colors.Blue);
				x0 = 0;
				wn = Width * n / iterations;
				ctx.Rectangle (x0, x0, wn, wn );
				ctx.Stroke ();
				// (2) draw ellipses
				ctx.SetColor (Colors.Green);
				x0 = centre;
				radius = 0.5 * Width * n / iterations;
				ctx.Arc (x0, x0, radius, 0, 360);
				ctx.Stroke ();
			}
			ctx.Restore ();

		}
	}
}

