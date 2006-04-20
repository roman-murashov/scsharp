using System;
using System.IO;
using System.Threading;

using SdlDotNet;
using System.Drawing;

namespace SCSharp
{
	public class LoginScreen : UIScreen
	{
		public LoginScreen (Mpq mpq) : base (mpq, "glue\\PalNl", Builtins.rez_GluLoginBin)
		{
		}

		const int OK_ELEMENT_INDEX = 4;
		const int CANCEL_ELEMENT_INDEX = 5;
		const int NEW_ELEMENT_INDEX = 6;
		const int DELETE_ELEMENT_INDEX = 7;
		const int LISTBOX_ELEMENT_INDEX = 8;

		ListBoxElement listbox;

		string spcdir;
		string[] files;

		void PopulateUIFromDir ()
		{
			files = Directory.GetFiles (spcdir, "*.spc");
		
			for (int i = 0; i < files.Length; i ++)
				listbox.AddItem (Path.GetFileNameWithoutExtension (files[i]));

			listbox.SelectedIndex = 0;
		}

		protected override void ResourceLoader ()
		{
			base.ResourceLoader ();

			for (int i = 0; i < Elements.Count; i ++)
				Console.WriteLine ("{0}: {1} '{2}'", i, Elements[i].Type, Elements[i].Text);

			Elements[OK_ELEMENT_INDEX].Activate +=
				delegate () {
					if (listbox.SelectedIndex == -1)
						return;

					Game.Instance.SwitchToScreen (UIScreenType.RaceSelection);
				};

			Elements[CANCEL_ELEMENT_INDEX].Activate +=
				delegate () {
					Game.Instance.SwitchToScreen (UIScreenType.MainMenu);
				};

			Elements[NEW_ELEMENT_INDEX].Activate +=
				delegate () {
					EntryDialog d = new EntryDialog (this, mpq,
									 GlobalResources.Instance.GluAllTbl.Strings[22]);
					d.Cancel += delegate () { DismissDialog (); };
					d.Ok += delegate () {
						if (listbox.Contains (d.Value)) {
							NameAlreadyExists (d);
						}
						else {
							DismissDialog ();
							listbox.AddItem (d.Value);
						}
					};
					ShowDialog (d);
				};

			Elements[DELETE_ELEMENT_INDEX].Activate +=
				delegate () {
					OkCancelDialog okd = new OkCancelDialog (this, mpq,
										 GlobalResources.Instance.GluAllTbl.Strings[23]);
					okd.Cancel += delegate () { DismissDialog (); };
					okd.Ok += delegate () {
						DismissDialog ();
						/* actually delete the file */
						listbox.RemoveAt (listbox.SelectedIndex);
					};
					ShowDialog (okd);
				};

			listbox = (ListBoxElement)Elements[LISTBOX_ELEMENT_INDEX];

			spcdir = Path.Combine (Game.Instance.RootDirectory, "characters");

			PopulateUIFromDir ();
		}

		public override void KeyboardDown (KeyboardEventArgs args)
		{
			if (args.Key == Key.DownArrow
			    || args.Key == Key.UpArrow) {
				listbox.KeyboardDown (args);
			}
			else
				base.KeyboardDown (args);
		}

		void NameAlreadyExists (EntryDialog d)
		{
			OkDialog okd = new OkDialog (d, mpq,
						     GlobalResources.Instance.GluAllTbl.Strings[24]);
			d.ShowDialog (okd);
		}
	}
}
