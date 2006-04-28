//
// SCSharp.UI.GlobalResources
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
using System.IO;
using System.Threading;

using SdlDotNet;

namespace SCSharp.UI
{
	public class GlobalResources
	{
		Mpq mpq;

		IScriptBin iscriptBin;
		ImagesDat imagesDat;
		SpritesDat spritesDat;
		SfxDataDat sfxDataDat;
		UnitsDat unitsDat;
		FlingyDat flingyDat;

		Tbl imagesTbl;
		Tbl sfxDataTbl;
		Tbl spritesTbl;
		Tbl gluAllTbl;

		static GlobalResources instance;
		public static GlobalResources Instance {
			get { return instance; }
		}

		public GlobalResources (Mpq mpq)
		{
			if (instance != null)
				throw new Exception ("There can only be one GlobalResources");

			this.mpq = mpq;
			instance = this;
		}

		public void Load ()
		{
			ThreadPool.QueueUserWorkItem (ResourceLoader);
		}

		public Tbl ImagesTbl {
			get { return imagesTbl; }
		}

		public Tbl SfxDataTbl {
			get { return sfxDataTbl; }
		}

		public Tbl SpritesTbl {
			get { return spritesTbl; }
		}

		public Tbl GluAllTbl {
			get { return gluAllTbl; }
		}

		public ImagesDat ImagesDat {
			get { return imagesDat; }
		}

		public SpritesDat SpritesDat {
			get { return spritesDat; }
		}

		public SfxDataDat SfxDataDat {
			get { return sfxDataDat; }
		}

		public IScriptBin IScriptBin {
			get { return iscriptBin; }
		}

		public UnitsDat UnitsDat {
			get { return unitsDat; }
		}

		public FlingyDat FlingyDat {
			get { return flingyDat; }
		}

		void ResourceLoader (object state)
		{
			try {
				Console.WriteLine ("loading images.tbl");
				imagesTbl = (Tbl)mpq.GetResource (Builtins.ImagesTbl);

				Console.WriteLine ("loading sfxdata.tbl");
				sfxDataTbl = (Tbl)mpq.GetResource (Builtins.SfxDataTbl);

				Console.WriteLine ("loading sprites.tbl");
				spritesTbl = (Tbl)mpq.GetResource (Builtins.SpritesTbl);

				Console.WriteLine ("loading gluAll.tbl");
				gluAllTbl = (Tbl)mpq.GetResource (Builtins.rez_GluAllTbl);

				Console.WriteLine ("loading images.dat");
				imagesDat = (ImagesDat)mpq.GetResource (Builtins.ImagesDat);

				Console.WriteLine ("loading sfxdata.dat");
				sfxDataDat = (SfxDataDat)mpq.GetResource (Builtins.SfxDataDat);

				Console.WriteLine ("loading sprites.dat");
				spritesDat = (SpritesDat)mpq.GetResource (Builtins.SpritesDat);

				Console.WriteLine ("loading iscript.bin");
				iscriptBin = (IScriptBin)mpq.GetResource (Builtins.IScriptBin);

				Console.WriteLine ("loading units.dat");
				unitsDat = (UnitsDat)mpq.GetResource (Builtins.UnitsDat);

				Console.WriteLine ("loading flingy.dat");
				flingyDat = (FlingyDat)mpq.GetResource (Builtins.FlingyDat);

				// notify we're ready to roll
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (FinishedLoading)));
			}
			catch (Exception e) {
				Console.WriteLine ("Global Resource loader failed: {0}", e);
				Events.PushUserEvent (new UserEventArgs (new ReadyDelegate (Events.QuitApplication)));
			}
		}

		void FinishedLoading ()
		{
			if (Ready != null)
				Ready ();
		}

		public event ReadyDelegate Ready;
	}
}