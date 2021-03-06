// 
// ImageTableCell.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
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
using AppKit;
using CoreGraphics;
using Xwt.Backends;

namespace Xwt.Mac
{
	class ImageTableCell: NSImageCell, ICellRenderer
	{
		bool visible = true;

		public ImageTableCell ()
		{
		}
		
		public ImageTableCell (IntPtr p): base (p)
		{
		}
		
		IImageCellViewFrontend Frontend {
			get { return (IImageCellViewFrontend) Backend.Frontend; }
		}

		public CellViewBackend Backend { get; set; }

		public CompositeCell CellContainer { get; set; }

		public void Fill ()
		{
			ObjectValue = Frontend.Image.ToImageDescription (CellContainer.Context).ToNSImage ();
			visible = Frontend.Visible;
		}
		
		public override CGSize CellSize {
			get {
				NSImage img = ObjectValue as NSImage;
				if (img != null)
					return img.Size;
				else
					return base.CellSize;
			}
		}

		public override CGSize CellSizeForBounds (CGRect bounds)
		{
			if (visible)
				return base.CellSizeForBounds (bounds);
			return CGSize.Empty;
		}

		public override void DrawInteriorWithFrame (CGRect cellFrame, NSView inView)
		{
			if (visible)
				base.DrawInteriorWithFrame (cellFrame, inView);
		}
		
		public void CopyFrom (object other)
		{
			var ob = (ImageTableCell)other;
			Backend = ob.Backend;
		}
	}
}

