using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FavCat.CustomLists;
using FavCat.Database;
using FavCat.Modules;
using HarmonyLib;
using MelonLoader;
using UIExpansionKit.API;
using UnhollowerRuntimeLib;
using UnityEngine;
using VRC.Core;
using VRC.UI;

namespace FavCat
{
	// Token: 0x0200000B RID: 11
	[PatchShield]
	internal partial class FavCatMod : MelonMod
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002A10 File Offset: 0x00000C10
		public override void OnApplicationStart()
		{
			FavCatMod.Instance = this;
			bool flag = !FavCatMod.FeninohDishtkfan || !FavCatMod.Rnyno || FavCatMod.TWSfhdtporEli;
			if (!flag)
			{
				Directory.CreateDirectory("./UserData/FavCatImport");
				ClassInjector.RegisterTypeInIl2Cpp<CustomPickerList>();
				ClassInjector.RegisterTypeInIl2Cpp<CustomPicker>();
				ApiSnifferPatch.DoPatch();
				FavCatSettings.RegisterSettings();
				MelonLogger.Msg("Creating database");
				FavCatMod.Database = new LocalStoreDatabase(FavCatSettings.DatabasePath.Value, FavCatSettings.ImageCachePath.Value);
				FavCatMod.Database.ImageHandler.TrimCache(FavCatSettings.MaxCacheSizeBytes).NoAwait("Task");
				FavCatMod.DoAfterUiManagerInit(new Action(this.OnUiManagerInit));
			}
		}
	}
}