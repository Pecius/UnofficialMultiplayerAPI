using UnityEngine;
using Verse;
using Verse.Steam;

namespace UnofficialMultiplayerAPI
{
	class InfoWindow : Window
	{
		public bool openLogWindow = false;
		private const string subscribeURL = "https://steamcommunity.com/sharedfiles/filedetails/?id=1681596707";
		private const string downloadURL = "https://github.com/Pecius/UnofficialMultiplayerAPI/releases";
		private const string title = "Unofficial Multiplayer API not found!";
		private const string message = "One of the active mods is using the Unofficial Multiplayer API, but it appears to be missing!\nPlease download and/or enable the API to avoid desyncs and broken features in multiplayer.";

		public override Vector2 InitialSize => new Vector2(650f, 250f);

		public InfoWindow()
		{
			optionalTitle = title;
			doCloseX = true;
		}

		public override void PostClose()
		{
			base.PostClose();
			EditWindow_Log.wantsToOpen = openLogWindow;
		}

		public override void DoWindowContents(Rect inRect)
		{
			EditWindow_Log logWidnow = Find.WindowStack.WindowOfType<EditWindow_Log>();
			if(logWidnow != null)
			{
				logWidnow.Close();
				openLogWindow = true;
			}

			Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 40 - 50), message);

			Color clr = GUI.color;
			try
			{
				GUI.color = Color.green;
				if (Widgets.ButtonText(new Rect(inRect.width / 2 - CloseButSize.x - 10, inRect.height - CloseButSize.y, CloseButSize.x, CloseButSize.y),
					SteamManager.Initialized ? "Subscribe" : "Download"))
				{
					SteamUtility.OpenUrl(SteamManager.Initialized ? subscribeURL : downloadURL);
				}
			}
			finally
			{
				GUI.color = clr;
			}

			if (Widgets.ButtonText(new Rect(inRect.width/2 + 10, inRect.height - CloseButSize.y, CloseButSize.x, CloseButSize.y), "CloseButton".Translate()))
				Close();
		}
	}
}
