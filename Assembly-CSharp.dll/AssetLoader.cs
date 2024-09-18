using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x0200022F RID: 559
public class AssetLoader
{
	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000C94 RID: 3220 RVA: 0x000B1F57 File Offset: 0x000B0157
	public static AssetLoader Instance
	{
		get
		{
			if (AssetLoader.instance == null)
			{
				AssetLoader.instance = new AssetLoader();
			}
			return AssetLoader.instance;
		}
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x000B1F70 File Offset: 0x000B0170
	private void SortScriptableObject(ScriptableObject scriptableObject)
	{
		if (scriptableObject is BuildingPreset)
		{
			this.allBuildingData.Add((BuildingPreset)scriptableObject);
			return;
		}
		if (scriptableObject is ChapterPreset)
		{
			this.allChapters.Add((ChapterPreset)scriptableObject);
			return;
		}
		if (scriptableObject is AIActionPreset)
		{
			this.allActions.Add((AIActionPreset)scriptableObject);
			return;
		}
		if (scriptableObject is FurniturePreset)
		{
			this.allFurniture.Add((FurniturePreset)scriptableObject);
			return;
		}
		if (scriptableObject is InteractablePreset)
		{
			this.allInteractables.Add((InteractablePreset)scriptableObject);
			return;
		}
		if (scriptableObject is ClothesPreset)
		{
			this.allClothes.Add((ClothesPreset)scriptableObject);
			return;
		}
		if (scriptableObject is AmbientZone)
		{
			this.allAmbientZones.Add((AmbientZone)scriptableObject);
			return;
		}
		if (scriptableObject is MusicCue)
		{
			this.allMusicCues.Add((MusicCue)scriptableObject);
		}
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x000B204C File Offset: 0x000B024C
	private static float TimeDiff(float time)
	{
		return Time.time - time;
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x000B2058 File Offset: 0x000B0258
	private static string TimeDiffStr(float time)
	{
		return AssetLoader.TimeDiff(time).ToString();
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x000B2074 File Offset: 0x000B0274
	public Task PerformInitialLoadAsync()
	{
		AssetLoader.<PerformInitialLoadAsync>d__34 <PerformInitialLoadAsync>d__;
		<PerformInitialLoadAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<PerformInitialLoadAsync>d__.<>4__this = this;
		<PerformInitialLoadAsync>d__.<>1__state = -1;
		<PerformInitialLoadAsync>d__.<>t__builder.Start<AssetLoader.<PerformInitialLoadAsync>d__34>(ref <PerformInitialLoadAsync>d__);
		return <PerformInitialLoadAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x000B20B8 File Offset: 0x000B02B8
	public List<ScriptableObject> GetAllPresets()
	{
		if (this.allPresets == null)
		{
			this.allPresets = (List<ScriptableObject>)Addressables.LoadAssetsAsync<ScriptableObject>(AssetLoader.DATA_GROUP, null).WaitForCompletion();
		}
		return this.allPresets;
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x000B20F4 File Offset: 0x000B02F4
	public List<AmbientZone> GetAllAmbientZones()
	{
		if (this.allAmbientZones == null)
		{
			this.allAmbientZones = (List<AmbientZone>)Addressables.LoadAssetsAsync<AmbientZone>(AssetLoader.AMBIENT_ZONES_GROUP, null).WaitForCompletion();
		}
		return this.allAmbientZones;
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x000B2130 File Offset: 0x000B0330
	public List<MusicCue> GetAllMusicCues()
	{
		if (this.allMusicCues == null)
		{
			this.allMusicCues = (List<MusicCue>)Addressables.LoadAssetsAsync<MusicCue>(AssetLoader.MUSIC_CUES_GROUP, null).WaitForCompletion();
		}
		return this.allMusicCues;
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x000B216C File Offset: 0x000B036C
	public List<ChapterPreset> GetAllChapters()
	{
		if (this.allChapters == null)
		{
			this.allChapters = (List<ChapterPreset>)Addressables.LoadAssetsAsync<ChapterPreset>(AssetLoader.CHAPTERS_GROUP, null).WaitForCompletion();
		}
		return this.allChapters;
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x000B21A8 File Offset: 0x000B03A8
	public List<AIActionPreset> GetAllActions()
	{
		if (this.allActions == null)
		{
			this.allActions = (List<AIActionPreset>)Addressables.LoadAssetsAsync<AIActionPreset>(AssetLoader.ACTIONS_GROUP, null).WaitForCompletion();
		}
		return this.allActions;
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x000B21E4 File Offset: 0x000B03E4
	public List<TextAsset> GetAllFloorData()
	{
		if (this.allFloorData == null)
		{
			this.allFloorData = (List<TextAsset>)Addressables.LoadAssetsAsync<TextAsset>(AssetLoader.FLOOR_DATA_GROUP, null).WaitForCompletion();
		}
		return this.allFloorData;
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x000B2220 File Offset: 0x000B0420
	public List<BuildingPreset> GetAllBuildingPresets()
	{
		if (this.allBuildingData == null)
		{
			this.allBuildingData = (List<BuildingPreset>)Addressables.LoadAssetsAsync<BuildingPreset>(AssetLoader.BUILDING_DATA_GROUP, null).WaitForCompletion();
		}
		return this.allBuildingData;
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x000B225C File Offset: 0x000B045C
	public List<FurniturePreset> GetAllFurniture()
	{
		if (this.allFurniture == null)
		{
			this.allFurniture = (List<FurniturePreset>)Addressables.LoadAssetsAsync<FurniturePreset>(AssetLoader.FURNITURE_GROUP, null).WaitForCompletion();
		}
		return this.allFurniture;
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x000B2298 File Offset: 0x000B0498
	public List<InteractablePreset> GetAllInteractables()
	{
		if (this.allInteractables == null)
		{
			this.allInteractables = (List<InteractablePreset>)Addressables.LoadAssetsAsync<InteractablePreset>(AssetLoader.INTERACTABLES_GROUP, null).WaitForCompletion();
		}
		return this.allInteractables;
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x000B22D4 File Offset: 0x000B04D4
	public List<ClothesPreset> GetAllClothes()
	{
		if (this.allClothes == null)
		{
			this.allClothes = (List<ClothesPreset>)Addressables.LoadAssetsAsync<ClothesPreset>(AssetLoader.CLOTHES_GROUP, null).WaitForCompletion();
		}
		return this.allClothes;
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x000B2310 File Offset: 0x000B0510
	public List<LayoutConfiguration> GetAllLayoutConfigurations()
	{
		if (this.allLayoutConfigurations == null)
		{
			this.allLayoutConfigurations = (List<LayoutConfiguration>)Addressables.LoadAssetsAsync<LayoutConfiguration>(AssetLoader.LAYOUT_CONFIG_GROUP, null).WaitForCompletion();
		}
		return this.allLayoutConfigurations;
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x000B234C File Offset: 0x000B054C
	public List<RoomConfiguration> GetAllRoomConfigurations()
	{
		if (this.allRoomConfigurations == null)
		{
			this.allRoomConfigurations = (List<RoomConfiguration>)Addressables.LoadAssetsAsync<RoomConfiguration>(AssetLoader.ROOM_CONFIG_GROUP, null).WaitForCompletion();
		}
		return this.allRoomConfigurations;
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x000B2388 File Offset: 0x000B0588
	public List<RoomTypePreset> GetAllRoomTypePresets()
	{
		if (this.allRoomTypePresets == null)
		{
			this.allRoomTypePresets = (List<RoomTypePreset>)Addressables.LoadAssetsAsync<RoomTypePreset>(AssetLoader.ROOM_PRESETS_GROUP, null).WaitForCompletion();
		}
		return this.allRoomTypePresets;
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x000B23C4 File Offset: 0x000B05C4
	public List<DoorPairPreset> GetAllDoorPairPresets()
	{
		if (this.allDoorPairPresets == null)
		{
			this.allDoorPairPresets = (List<DoorPairPreset>)Addressables.LoadAssetsAsync<DoorPairPreset>(AssetLoader.DOOR_PAIR_PRESETS_GROUP, null).WaitForCompletion();
		}
		return this.allDoorPairPresets;
	}

	// Token: 0x04000E34 RID: 3636
	public static readonly string DATA_GROUP = "data";

	// Token: 0x04000E35 RID: 3637
	public static readonly string AMBIENT_ZONES_GROUP = "ambient_zones";

	// Token: 0x04000E36 RID: 3638
	public static readonly string MUSIC_CUES_GROUP = "music_cues";

	// Token: 0x04000E37 RID: 3639
	public static readonly string CHAPTERS_GROUP = "chapters";

	// Token: 0x04000E38 RID: 3640
	public static readonly string ACTIONS_GROUP = "actions";

	// Token: 0x04000E39 RID: 3641
	public static readonly string FLOOR_DATA_GROUP = "floor_data";

	// Token: 0x04000E3A RID: 3642
	public static readonly string BUILDING_DATA_GROUP = "building_data";

	// Token: 0x04000E3B RID: 3643
	public static readonly string FURNITURE_GROUP = "furniture";

	// Token: 0x04000E3C RID: 3644
	public static readonly string INTERACTABLES_GROUP = "interactables";

	// Token: 0x04000E3D RID: 3645
	public static readonly string CLOTHES_GROUP = "clothes";

	// Token: 0x04000E3E RID: 3646
	public static readonly string LAYOUT_CONFIG_GROUP = "layout_config";

	// Token: 0x04000E3F RID: 3647
	public static readonly string ROOM_CONFIG_GROUP = "room_config";

	// Token: 0x04000E40 RID: 3648
	public static readonly string ROOM_PRESETS_GROUP = "room_type_presets";

	// Token: 0x04000E41 RID: 3649
	public static readonly string DOOR_PAIR_PRESETS_GROUP = "door_pair_presets";

	// Token: 0x04000E42 RID: 3650
	private static AssetLoader instance = null;

	// Token: 0x04000E43 RID: 3651
	private List<ScriptableObject> allPresets;

	// Token: 0x04000E44 RID: 3652
	private List<AmbientZone> allAmbientZones;

	// Token: 0x04000E45 RID: 3653
	private List<MusicCue> allMusicCues;

	// Token: 0x04000E46 RID: 3654
	private List<ChapterPreset> allChapters;

	// Token: 0x04000E47 RID: 3655
	private List<AIActionPreset> allActions;

	// Token: 0x04000E48 RID: 3656
	private List<TextAsset> allFloorData;

	// Token: 0x04000E49 RID: 3657
	private List<BuildingPreset> allBuildingData;

	// Token: 0x04000E4A RID: 3658
	private List<FurniturePreset> allFurniture;

	// Token: 0x04000E4B RID: 3659
	private List<InteractablePreset> allInteractables;

	// Token: 0x04000E4C RID: 3660
	private List<ClothesPreset> allClothes;

	// Token: 0x04000E4D RID: 3661
	private List<LayoutConfiguration> allLayoutConfigurations;

	// Token: 0x04000E4E RID: 3662
	private List<RoomConfiguration> allRoomConfigurations;

	// Token: 0x04000E4F RID: 3663
	private List<RoomTypePreset> allRoomTypePresets;

	// Token: 0x04000E50 RID: 3664
	private List<DoorPairPreset> allDoorPairPresets;
}
