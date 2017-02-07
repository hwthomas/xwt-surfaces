//
// Surface.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
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
using Xwt.Backends;

namespace Xwt.Drawing
{
	/// <summary>
	/// A surface is an off-screen drawing buffer, from which a Drawing Context can be obtained
	/// </summary>
	public class Surface: XwtObject, IDisposable
	{
		Context ctx;
		Size size;


		// Create default surface with specified size and scalefactor
		public Surface (Size size, double scaleFactor = 1)
		{
			Init (ToolkitEngine.SurfaceBackendHandler.CreateSurface (size.Width, size.Height, scaleFactor), size);
		}

		// Create default surface with specified width, height, and scalefactor
		public Surface (double width, double height, double scaleFactor = 1): this (new Size (width, height), scaleFactor)
		{
		}


		// Create surface with specified size, and compatible with supplied Canvas (widget)
		public Surface (Size size, Canvas canvas)
		{
			Init (ToolkitEngine.SurfaceBackendHandler.CreateSurfaceCompatibleWithWidget (canvas.GetBackend (), size.Width, size.Height), size);
		}

		public Surface (double width, double height, Canvas canvas): this (new Size (width, height), canvas)
		{
		}


		// Create surface with specified size, and compatible with specified Surface 
		public Surface (Size size, Surface surface)
		{
			Init (ToolkitEngine.SurfaceBackendHandler.CreateSurfaceCompatibleWithSurface (surface.GetBackend (), size.Width, size.Height), size);
		}

		public Surface (double width, double height, Surface surface): this (new Size (width, height), surface)
		{
		}


		// Create surface with specified size, and compatible with surface from supplied Context 
		public Surface (Size size, Context ctx)
		{
			Init (ToolkitEngine.SurfaceBackendHandler.CreateSurfaceCompatibleWithContext (ctx.GetBackend (), size.Width, size.Height), size);
			//var contextBackend = ctx.GetBackend ();
			//var o = ToolkitEngine.SurfaceBackendHandler.CreateSurfaceCompatibleWithContext (contextBackend, size.Width, size.Height);
			//Init (o, size);
		}


		// Create drawing context for surface if not already set up
		public Context Context {
			get {
				if (this.ctx == null)
					this.ctx = new Context (ToolkitEngine.SurfaceBackendHandler.CreateContext (Backend), ToolkitEngine);
				return this.ctx;
			}
		}



		internal void Init (object backend, Size s)
		{
			size = s;
			Backend = backend;
			if (ToolkitEngine.SurfaceBackendHandler.DisposeHandleOnUiThread)
				ResourceManager.RegisterResource (backend, ToolkitEngine.SurfaceBackendHandler.Dispose);
			else
				GC.SuppressFinalize (this);
		}

		public double Width {
			get { return size.Width; }
		}

		public double Height {
			get { return size.Height; }
		}

		public Size Size {
			get { return size; }
		}

		~Surface ()
		{
			ResourceManager.FreeResource (Backend);
		}

		public void Dispose ()
		{
			if (ToolkitEngine.SurfaceBackendHandler.DisposeHandleOnUiThread) {
				GC.SuppressFinalize (this);
				ResourceManager.FreeResource (Backend);
			} else
				ToolkitEngine.SurfaceBackendHandler.Dispose (Backend);
		}
	}
}

