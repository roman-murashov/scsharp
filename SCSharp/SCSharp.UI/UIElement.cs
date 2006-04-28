//
// SCSharp.UI.UIElement
//
// Authors:
//	Chris Toshok (toshok@hungry.com)
//
// (C) 2006 The Hungry Programmers (http://www.hungry.com/)
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;

using SdlDotNet;

namespace SCSharp.UI
{

	public class UIElement
	{
		BinElement el;
		Surface surface;
		UIScreen screen;
		byte[] palette;
		bool sensitive;
		bool visible;
		Fnt fnt;

		public UIElement (UIScreen screen, BinElement el, byte[] palette)
		{
			this.screen = screen;
			this.el = el;
			this.palette = palette;
			this.sensitive = true;
			this.visible = (el.flags & ElementFlags.Visible) != 0;
		}

		public UIScreen ParentScreen {
			get { return screen; }
		}

		public Mpq Mpq {
			get { return screen.Mpq; }
		}

		public string Text {
			get { return el.text; }
			set { el.text = value;
			      ClearSurface (); }
		}

		public bool Sensitive {
			get { return sensitive; }
			set { sensitive = value;
			      ClearSurface (); }
		}

		public bool Visible {
			get { return visible; }
			set { visible = value;
			      ClearSurface (); }
		}

		public byte[] Palette {
			get { return palette; }
			set { palette = value;
			      ClearSurface (); }
		}

		public Surface Surface {
			get {
				if (surface == null)
					surface = CreateSurface ();

				return surface;
			}
		}

		public Fnt Font {
			get { 
				if (fnt == null) {
					int idx = 2;

					if ((Flags & ElementFlags.FontSmallest) != 0) idx = 0;
					else if ((Flags & ElementFlags.FontSmaller) != 0) idx = 3;
					else if ((Flags & ElementFlags.FontLarger) != 0) idx = 3;
					else if ((Flags & ElementFlags.FontLargest) != 0) idx = 4;

					Console.WriteLine ("index = {0}", idx);

					fnt = GuiUtil.GetFonts(Mpq)[idx];

					if (fnt == null)
						throw new Exception (String.Format ("null font at index {0}..  bad things are afoot", idx));
				}
				return fnt;
			}
			set { fnt = value;
			      ClearSurface (); }
		}

		public ElementFlags Flags { get { return el.flags; } }
		public ElementType Type { get { return el.type; } }

		public ushort X1 {
			get { return el.x1; }
			set { el.x1 = value; }
		}
		public ushort Y1 {
			get { return el.y1; }
			set { el.y1 = value; }
		}
		public ushort Width { get { return el.width; } }
		public ushort Height { get { return el.height; } }

		public Key Hotkey { get { return (Key)el.hotkey; } }

		public event ElementEvent Activate;

		public void OnActivate ()
		{
			if (Activate != null)
				Activate ();
		}

		protected void ClearSurface ()
		{
			surface = null;
		}

		protected virtual Surface CreateSurface ()
		{
			switch (Type) {
			case ElementType.DefaultButton:
			case ElementType.Button:
			case ElementType.ButtonWithoutBorder:
				return GuiUtil.ComposeText (Text, Font, palette, Width, Height,
							    sensitive ? 4 : 24);
			default:
				return null;
			}
		}

		public void Paint (Surface surf, DateTime now)
		{
			if (!visible)
				return;

			if (Surface == null)
				return;

			surf.Blit (surface, new Point (X1, Y1));
		}
		
		public virtual bool PointInside (int x, int y)
		{
			if (x >= X1 && x < X1 + Width &&
			    y >= Y1 && y < Y1 + Height)
				return true;

			return false;
		}

		public virtual void MouseButtonDown (MouseButtonEventArgs args)
		{
		}

		public virtual void MouseButtonUp (MouseButtonEventArgs args)
		{
		}

		public virtual void PointerMotion (MouseMotionEventArgs args)
		{
		}

		public virtual void MouseOver ()
		{
		}
	}

	public delegate void ElementEvent ();
}