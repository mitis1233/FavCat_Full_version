// FavCat.Database.LocalStoreDatabase
// Token: 0x0600018F RID: 399 RVA: 0x0000B79C File Offset: 0x0000999C
public void UpdateStoredAvatar(ApiAvatar avatar)
{
	string id = avatar.id;
	bool flag = string.IsNullOrEmpty(id) || string.IsNullOrEmpty(avatar.name);
	if (!flag)
	{
		StoredAvatar storedAvatar = new StoredAvatar
		{
			AvatarId = id,
			AuthorId = avatar.authorId,
			Name = avatar.name,
			Description = avatar.description,
			AuthorName = avatar.authorName,
			ThumbnailUrl = avatar.thumbnailImageUrl,
			ImageUrl = avatar.imageUrl,
			ReleaseStatus = avatar.releaseStatus,
			Platform = avatar.platform,
			CreatedAt = DateTime.FromBinary(avatar.created_at.ToBinary()),
			UpdatedAt = DateTime.FromBinary(avatar.updated_at.ToBinary()),
			SupportedPlatforms = avatar.supportedPlatforms
		};
		this.myUpdateThreadQueue.Enqueue(delegate
		{
			StoredAvatar avatar2 = this.GetAvatar(id);
			bool flag2 = avatar2 != null;
			StoredAvatar storedAvatar;
			if (flag2)
			{
				bool flag3 = avatar2.UpdatedAt > storedAvatar.UpdatedAt;
				if (flag3)
				{
					return;
				}
				bool flag4 = avatar.assetUrl == null;
				if (flag4)
				{
					storedAvatar.SupportedPlatforms = avatar2.SupportedPlatforms;
				}
				storedAvatar = storedAvatar;
				if (storedAvatar.Description == null)
				{
					storedAvatar.Description = avatar2.Description;
				}
			}
			this.myStoredAvatars.Upsert(storedAvatar);
		});
	}
}