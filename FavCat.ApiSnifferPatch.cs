using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FavCat.Database;
using MelonLoader;
using UIExpansionKit.API;
using UnityEngine.Networking;
using VRC.Core;

namespace FavCat
{
	// Token: 0x0200000E RID: 14
	[Nullable(0)]
	[NullableContext(1)]
	public class ApiSnifferPatch
	{
		// Token: 0x06000034 RID: 52 RVA: 0x000037E8 File Offset: 0x000019E8
		static ApiSnifferPatch()
		{
			ApiSnifferPatch.ImageDownloaderClosureType = typeof(ImageDownloader).GetNestedTypes().Single((Type it) => it.GetMethod("_DownloadImageInternal_b__0") != null);
			ApiSnifferPatch.WebRequestField = ApiSnifferPatch.ImageDownloaderClosureType.GetProperties().Single((PropertyInfo it) => it.PropertyType == typeof(UnityWebRequest)).GetMethod;
			PropertyInfo propertyInfo = ApiSnifferPatch.ImageDownloaderClosureType.GetProperties().SingleOrDefault((PropertyInfo it) => it.PropertyType.IsNested && it.PropertyType.DeclaringType == typeof(ImageDownloader));
			ApiSnifferPatch.NestedClosureField = ((propertyInfo != null) ? propertyInfo.GetMethod : null);
			MethodInfo nestedClosureField = ApiSnifferPatch.NestedClosureField;
			Type type = (nestedClosureField != null) ? nestedClosureField.ReturnType : null;
			ApiSnifferPatch.ImageUrlField = ((ApiSnifferPatch.NestedClosureField != null) ? type : ApiSnifferPatch.ImageDownloaderClosureType).GetProperty("imageUrl").GetMethod;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000038E8 File Offset: 0x00001AE8
		public static void DoPatch()
		{
			NativePatchUtils.NativePatch<ApiSnifferPatch.ApiPopulateDelegate>(typeof(ApiModel).GetMethods().Single((MethodInfo it) => it.Name == "SetApiFieldsFromJson" && it.GetParameters().Length == 2), out ApiSnifferPatch.ourOriginalApiPopulate, new ApiSnifferPatch.ApiPopulateDelegate(ApiSnifferPatch.ApiSnifferStatic));
			NativePatchUtils.NativePatch<ApiSnifferPatch.ImageDownloaderOnDoneDelegate>(ApiSnifferPatch.ImageDownloaderClosureType.GetMethod("_DownloadImageInternal_b__0"), out ApiSnifferPatch.ourOriginalOnDone, new ApiSnifferPatch.ImageDownloaderOnDoneDelegate(ApiSnifferPatch.ImageSnifferPatch));
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003968 File Offset: 0x00001B68
		public static void ImageSnifferPatch(IntPtr instancePtr, IntPtr asyncOperationPtr)
		{
			ApiSnifferPatch.ourOriginalOnDone(instancePtr, asyncOperationPtr);
			try
			{
				bool flag = !FavCatSettings.UseLocalImageCache || FavCatMod.Database == null;
				if (!flag)
				{
					object obj = Activator.CreateInstance(ApiSnifferPatch.ImageDownloaderClosureType, new object[]
					{
						instancePtr
					});
					MethodBase imageUrlField = ApiSnifferPatch.ImageUrlField;
					MethodInfo nestedClosureField = ApiSnifferPatch.NestedClosureField;
					string text = (string)imageUrlField.Invoke(((nestedClosureField != null) ? nestedClosureField.Invoke(obj, ApiSnifferPatch.EmptyObjectArray) : null) ?? obj, ApiSnifferPatch.EmptyObjectArray);
					UnityWebRequest unityWebRequest = (UnityWebRequest)ApiSnifferPatch.WebRequestField.Invoke(obj, ApiSnifferPatch.EmptyObjectArray);
					bool flag2 = unityWebRequest.isNetworkError || unityWebRequest.isHttpError;
					if (!flag2)
					{
						bool flag3 = unityWebRequest.downloadedBytes > 1048576UL;
						if (flag3)
						{
							bool flag4 = MelonDebug.IsEnabled();
							if (flag4)
							{
								MelonDebug.Msg("Ignored downloaded image from " + text + " because it's bigger than 1 MB");
							}
						}
						else
						{
							FavCatMod.Database.ImageHandler.StoreImageAsync(text, unityWebRequest.downloadHandler.data).NoAwait("Task");
						}
					}
				}
			}
			catch (Exception arg)
			{
				MelonLogger.Error(string.Format("Exception in image downloader patch: {0}", arg));
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003AA4 File Offset: 0x00001CA4
		public static bool ApiSnifferStatic(IntPtr @this, IntPtr dictionary, IntPtr someRef, IntPtr methodInfo)
		{
			bool result = ApiSnifferPatch.ourOriginalApiPopulate(@this, dictionary, someRef, methodInfo);
			try
			{
				ApiModel apiModel = new ApiModel(@this);
				bool flag = !apiModel.Populated;
				if (flag)
				{
					return result;
				}
				APIUser apiuser = apiModel.TryCast<APIUser>();
				bool flag2 = apiuser != null;
				if (flag2)
				{
					LocalStoreDatabase database = FavCatMod.Database;
					if (database != null)
					{
						database.UpdateStoredPlayer(apiuser);
					}
				}
				ApiWorld apiWorld = apiModel.TryCast<ApiWorld>();
				bool flag3 = apiWorld != null;
				if (flag3)
				{
					LocalStoreDatabase database2 = FavCatMod.Database;
					if (database2 != null)
					{
						database2.UpdateStoredWorld(apiWorld);
					}
				}
				ApiAvatar apiAvatar = apiModel.TryCast<ApiAvatar>();
				bool flag4 = apiAvatar != null;
				if (flag4)
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

		// Token: 0x06000038 RID: 56 RVA: 0x00003B58 File Offset: 0x00001D58
		public ApiSnifferPatch()
		{
		}

		// Token: 0x04000035 RID: 53
		private static ApiSnifferPatch.ApiPopulateDelegate ourOriginalApiPopulate = (IntPtr @this, IntPtr dictionary, IntPtr @ref, IntPtr ref1) => false;

		// Token: 0x04000036 RID: 54
		private static ApiSnifferPatch.ImageDownloaderOnDoneDelegate ourOriginalOnDone = delegate(IntPtr ptr, IntPtr operationPtr)
		{
		};

		// Token: 0x04000037 RID: 55
		private static readonly Type ImageDownloaderClosureType;

		// Token: 0x04000038 RID: 56
		private static readonly MethodInfo WebRequestField;

		// Token: 0x04000039 RID: 57
		private static readonly MethodInfo ImageUrlField;

		// Token: 0x0400003A RID: 58
		[Nullable(2)]
		private static readonly MethodInfo NestedClosureField;

		// Token: 0x0400003B RID: 59
		private static readonly object[] EmptyObjectArray = new object[0];

		// Token: 0x0200000F RID: 15
		// (Invoke) Token: 0x0600003A RID: 58
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NullableContext(0)]
		private delegate bool ApiPopulateDelegate(IntPtr @this, IntPtr dictionary, IntPtr someRef, IntPtr methodRef);

		// Token: 0x02000010 RID: 16
		// (Invoke) Token: 0x0600003E RID: 62
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NullableContext(0)]
		private delegate void ImageDownloaderOnDoneDelegate(IntPtr thisPtr, IntPtr asyncOperationPtr);
	}
}
