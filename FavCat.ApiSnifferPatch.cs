using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FavCat.Database;
using MelonLoader;
using VRC.Core;

namespace FavCat
{
	// Token: 0x0200000C RID: 12
	[Nullable(0)]
	[NullableContext(1)]
	public partial class ApiSnifferPatch
	{
		// Token: 0x06000031 RID: 49
		public static bool ApiSnifferStatic(IntPtr @this, IntPtr dictionary, IntPtr someRef, IntPtr methodInfo)
		{
			bool result = ApiSnifferPatch.ourOriginalApiPopulate(@this, dictionary, someRef, methodInfo);
			try
			{
				ApiModel apiModel = new ApiModel(@this);
				if (!apiModel.Populated)
				{
					return result;
				}
				APIUser apiuser = apiModel.TryCast<APIUser>();
				if (apiuser != null)
				{
					LocalStoreDatabase database = FavCatMod.Database;
					if (database != null)
					{
						database.UpdateStoredPlayer(apiuser);
					}
				}
				ApiWorld apiWorld = apiModel.TryCast<ApiWorld>();
				if (apiWorld != null)
				{
					LocalStoreDatabase database2 = FavCatMod.Database;
					if (database2 != null)
					{
						database2.UpdateStoredWorld(apiWorld);
					}
				}
				ApiAvatar apiAvatar = apiModel.TryCast<ApiAvatar>();
				if (apiAvatar != null)
				{
					LocalStoreDatabase database3 = FavCatMod.Database;
					if (database3 != null)
					{
						database3.UpdateStoredAvatar(apiAvatar);
					}
				}
			}
			catch (Exception arg)
			{
				MelonLogger.Error(string.Format("Exception in API sniffer patch: {0}", arg));
			}
			return result;
		}
	}
}
