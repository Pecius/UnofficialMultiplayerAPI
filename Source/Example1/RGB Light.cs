using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnofficialMultiplayerAPI;
using UnityEngine;

namespace UMPExample1
{
	public class MultiplayerInit : IMultiplayerInit
	{
		public void Init() //All SyncFields and SyncMethods (if used manually instead of using an attribute) must be initialized here
		{
			// This method initializes and registers a SyncField for type CompGlowerController which points at its member "r"
			CompGlowerController.syncR = MPApi.SyncField(typeof(CompGlowerController), "r").SetBufferChanges();
			CompGlowerController.syncG = MPApi.SyncField(typeof(CompGlowerController), "g").SetBufferChanges();
			CompGlowerController.syncB = MPApi.SyncField(typeof(CompGlowerController), "b").SetBufferChanges();
		}
	}

	public class CompGlowerController : ThingComp
	{
		public int r = 0;	// For purpose of this example, we'll use seperate integers instead of ColorInt 
		public int g = 0;
		public int b = 0;

		public static ISyncField syncR;
		public static ISyncField syncG;
		public static ISyncField syncB;

		private CompGlower glower => parent.TryGetComp<CompGlower>();


		/*
			SyncMethod attribute will mark this method for syncing
			A call to this method will be replicated on other clients (along with arguments)
		*/

		[SyncMethod]
		public void Apply()
		{
			Log.Message($"Applying: r:{r}, g:{g}, b:{b} {MPApi.IsHosting}, {MPApi.IsInMultiplayer}, {MPApi.PlayerName}");

			glower.Props.glowColor.r = r;
			glower.Props.glowColor.g = g;
			glower.Props.glowColor.b = b;

			parent.Map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
			parent.Map.glowGrid.DeRegisterGlower(glower);
			parent.Map.glowGrid.RegisterGlower(glower);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo item in base.CompGetGizmosExtra())
			{
				yield return item;
			}

			yield return new Command_Action
			{
				defaultLabel = "Change color",
				action = delegate
				{
					Find.WindowStack.Add(new ColorWindow(this));
				}
			};

		}


	}

	public class ColorWindow : Window
	{
		public override Vector2 InitialSize => new Vector2(350f, 250f);

		private CompGlowerController parent;

		public ColorWindow(CompGlowerController p)
		{
			parent = p;
			doCloseX = true;
		}

		public override void DoWindowContents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			if (MPApi.IsInMultiplayer)
			{
				MPApi.FieldWatchPrefix();	// This method begins a new stack

				CompGlowerController.syncR.Watch(parent);	// "Watch" method pushes a monitor for field changes in the queue
				CompGlowerController.syncG.Watch(parent);	// It has to be ran before a possible value change
				CompGlowerController.syncB.Watch(parent);
			}

			listing.Label($"r: {parent.r}");
			parent.r = Mathf.RoundToInt(listing.Slider(parent.r, 0, 255));

			listing.Label($"g: {parent.g}");
			parent.g = Mathf.RoundToInt(listing.Slider(parent.g, 0, 255));

			listing.Label($"b: {parent.b}");
			parent.b = Mathf.RoundToInt(listing.Slider(parent.b, 0, 255));

			if (MPApi.IsInMultiplayer)
				MPApi.FieldWatchPostfix();  // Executing the stack, every pushed monitor will be executed depending on it's state

			var clr = GUI.color;

			GUI.color = new Color(parent.r / 255f, parent.g / 255f, parent.b / 255f);
			if (listing.ButtonText("Set"))
				parent.Apply();

			GUI.color = clr;

			listing.End();
		}
	}

	public class CompProperties_GlowerController : CompProperties
	{
		public CompProperties_GlowerController() : base(typeof(CompGlowerController))
		{
		}
	}

}
