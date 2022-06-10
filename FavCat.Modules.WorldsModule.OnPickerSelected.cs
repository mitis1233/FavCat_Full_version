using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FavCat.CustomLists;
using FavCat.Database;
using FavCat.Database.Stored;
using MelonLoader;
using UIExpansionKit.API;
using UnityEngine;
using VRC.Core;
using VRC.UI;

namespace FavCat.Modules
{
	// Token: 0x02000045 RID: 69
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public partial class WorldsModule : ExtendedFavoritesModuleBase<StoredWorld>
	{
		// Token: 0x06000151 RID: 337 RVA: 0x00029DD4 File Offset: 0x00027FD4
		protected override void OnPickerSelected(IPickerElement picker)
        {
            if (picker.Id == myLastRequestedWorld) 
                return;
            
            PlaySound();

            myLastRequestedWorld = picker.Id;
            var world = new ApiWorld {id = picker.Id};
            world.Fetch(new Action<ApiContainer>(_ =>
            {
                myLastRequestedWorld = "";
                if (listsParent.gameObject.activeInHierarchy)
                    ScanningReflectionCache.DisplayWorldInfoPage(world, null, false, null);
            }), new Action<ApiContainer>(c =>
            {
                myLastRequestedWorld = "";
                if (MelonDebug.IsEnabled())
                    MelonDebug.Msg("API request errored with " + c.Code + " - " + c.Error);
                if (c.Code == 404 && listsParent.gameObject.activeInHierarchy)
                {
                    var menu = ExpansionKitApi.CreateCustomFullMenuPopup(LayoutDescription.WideSlimList);
                    menu.AddSpacer();
                    menu.AddLabel("This world is not available anymore (deleted)");
                    menu.AddLabel("It has been removed from all favorite lists");
                    menu.AddSpacer();
                    menu.AddSimpleButton("Delete from favorites & close", () => { FavCatMod.Database.CompletelyDeleteWorld(picker.Id); menu.Hide(); });
                    menu.AddSimpleButton("Close", menu.Hide);
                    menu.Show();
                }
            }));
        }
	}
}
