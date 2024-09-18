using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000802 RID: 2050
public class PrefabControls : MonoBehaviour
{
	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06002617 RID: 9751 RVA: 0x001EA2CC File Offset: 0x001E84CC
	public static PrefabControls Instance
	{
		get
		{
			return PrefabControls._instance;
		}
	}

	// Token: 0x06002618 RID: 9752 RVA: 0x001EA2D3 File Offset: 0x001E84D3
	private void Awake()
	{
		if (PrefabControls._instance != null && PrefabControls._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		PrefabControls._instance = this;
	}

	// Token: 0x04004346 RID: 17222
	[Header("Interface Prefabs")]
	public GameObject progressBarPip;

	// Token: 0x04004347 RID: 17223
	public GameObject buttonAdditionalHighlight;

	// Token: 0x04004348 RID: 17224
	public GameObject contextMenuPanel;

	// Token: 0x04004349 RID: 17225
	public GameObject evidenceContentPageControls;

	// Token: 0x0400434A RID: 17226
	public GameObject speechBubble;

	// Token: 0x0400434B RID: 17227
	public GameObject attackBar;

	// Token: 0x0400434C RID: 17228
	public GameObject gameMessage;

	// Token: 0x0400434D RID: 17229
	public GameObject keyMergeGameMessage;

	// Token: 0x0400434E RID: 17230
	public GameObject socialCreditGameMessage;

	// Token: 0x0400434F RID: 17231
	public GameObject tooltip;

	// Token: 0x04004350 RID: 17232
	public GameObject dialogOption;

	// Token: 0x04004351 RID: 17233
	public GameObject soundIndicatorIcon;

	// Token: 0x04004352 RID: 17234
	public GameObject awarenessIndicator;

	// Token: 0x04004353 RID: 17235
	public GameObject uiPointer;

	// Token: 0x04004354 RID: 17236
	public GameObject crossOut;

	// Token: 0x04004355 RID: 17237
	public GameObject aiReactionIndicator;

	// Token: 0x04004356 RID: 17238
	public GameObject lockpickProgressBar;

	// Token: 0x04004357 RID: 17239
	public GameObject statusElement;

	// Token: 0x04004358 RID: 17240
	public GameObject statusRemovedIcon;

	// Token: 0x04004359 RID: 17241
	public GameObject crosshairReaction;

	// Token: 0x0400435A RID: 17242
	public GameObject upgradesSource;

	// Token: 0x0400435B RID: 17243
	public GameObject upgradesInputButton;

	// Token: 0x0400435C RID: 17244
	public GameObject upgradesOutputButton;

	// Token: 0x0400435D RID: 17245
	public GameObject upgradesConnection;

	// Token: 0x0400435E RID: 17246
	public GameObject radialSelectionSegment;

	// Token: 0x0400435F RID: 17247
	public GameObject objectSelectionIcon;

	// Token: 0x04004360 RID: 17248
	public GameObject revealQuestionObject;

	// Token: 0x04004361 RID: 17249
	public GameObject interfaceVideo;

	// Token: 0x04004362 RID: 17250
	[Header("CityEditor Prefabs")]
	public GameObject[] CityConstructorPrefabs;

	// Token: 0x04004363 RID: 17251
	public GameObject pathfinderPrefab;

	// Token: 0x04004364 RID: 17252
	[Header("CityEditor Target Transforms")]
	public Transform CityConstructorTargetTransform;

	// Token: 0x04004365 RID: 17253
	[Header("Overhead Map")]
	public GameObject doorMapComponent;

	// Token: 0x04004366 RID: 17254
	public GameObject mapButtonComponent;

	// Token: 0x04004367 RID: 17255
	public GameObject mapDuctComponent;

	// Token: 0x04004368 RID: 17256
	public GameObject floorPlanBlockWall;

	// Token: 0x04004369 RID: 17257
	public GameObject streetName;

	// Token: 0x0400436A RID: 17258
	public GameObject districtName;

	// Token: 0x0400436B RID: 17259
	public GameObject playerMarker;

	// Token: 0x0400436C RID: 17260
	public GameObject mapPointer;

	// Token: 0x0400436D RID: 17261
	public GameObject routeLine;

	// Token: 0x0400436E RID: 17262
	public GameObject mapBuildingGraphic;

	// Token: 0x0400436F RID: 17263
	public GameObject tutorialPointer;

	// Token: 0x04004370 RID: 17264
	public GameObject mapLayerCanvas;

	// Token: 0x04004371 RID: 17265
	public GameObject mapRectContainer;

	// Token: 0x04004372 RID: 17266
	public Texture2D drawingBrush;

	// Token: 0x04004373 RID: 17267
	public Texture2D eraseBrush;

	// Token: 0x04004374 RID: 17268
	public GameObject debugAccess;

	// Token: 0x04004375 RID: 17269
	public Sprite mapCharacterMarker;

	// Token: 0x04004376 RID: 17270
	public Color characterMarkerColor = Color.white;

	// Token: 0x04004377 RID: 17271
	public List<Color> motionTrackerColors = new List<Color>();

	// Token: 0x04004378 RID: 17272
	[Header("Buttons")]
	public GameObject evidenceButton;

	// Token: 0x04004379 RID: 17273
	public GameObject factButton;

	// Token: 0x0400437A RID: 17274
	public GameObject factHideToggleButton;

	// Token: 0x0400437B RID: 17275
	public GameObject newCustomFactButton;

	// Token: 0x0400437C RID: 17276
	public GameObject caseFolderClassIconButton;

	// Token: 0x0400437D RID: 17277
	public GameObject contextMenuButton;

	// Token: 0x0400437E RID: 17278
	public GameObject checklistButton;

	// Token: 0x0400437F RID: 17279
	public GameObject linkButton;

	// Token: 0x04004380 RID: 17280
	[Header("Case Panel")]
	public GameObject casePanelObject;

	// Token: 0x04004381 RID: 17281
	public GameObject stringLink;

	// Token: 0x04004382 RID: 17282
	public GameObject customStringLinkSelect;

	// Token: 0x04004383 RID: 17283
	public GameObject boxSelect;

	// Token: 0x04004384 RID: 17284
	public GameObject caseButton;

	// Token: 0x04004385 RID: 17285
	public GameObject quickMenu;

	// Token: 0x04004386 RID: 17286
	[Header("Window Prefabs")]
	public GameObject infoWindow;

	// Token: 0x04004387 RID: 17287
	public GameObject tabButton;

	// Token: 0x04004388 RID: 17288
	public GameObject suspectWindowEntry;

	// Token: 0x04004389 RID: 17289
	public GameObject passcodesEntry;

	// Token: 0x0400438A RID: 17290
	public GameObject passcodesEntryMini;

	// Token: 0x0400438B RID: 17291
	public GameObject phoneNumberEntry;

	// Token: 0x0400438C RID: 17292
	public GameObject drawingControls;

	// Token: 0x0400438D RID: 17293
	public GameObject pagePip;

	// Token: 0x0400438E RID: 17294
	[Header("Debug Prefabs")]
	public GameObject pathfindRoomDebug;

	// Token: 0x0400438F RID: 17295
	public GameObject pathfindNodeDebug;

	// Token: 0x04004390 RID: 17296
	public GameObject pathfindInternalDebug;

	// Token: 0x04004391 RID: 17297
	public GameObject walkPointSphere;

	// Token: 0x04004392 RID: 17298
	public GameObject usePointSphere;

	// Token: 0x04004393 RID: 17299
	public GameObject streetChunkDebug;

	// Token: 0x04004394 RID: 17300
	public GameObject junctionChunkDebug;

	// Token: 0x04004395 RID: 17301
	public GameObject streetAreaChunkDebug;

	// Token: 0x04004396 RID: 17302
	public GameObject streetNodeDebug;

	// Token: 0x04004397 RID: 17303
	public GameObject furnitureDebug;

	// Token: 0x04004398 RID: 17304
	[Header("City")]
	public GameObject neonSign;

	// Token: 0x04004399 RID: 17305
	[Header("Game World")]
	public GameObject citizen;

	// Token: 0x0400439A RID: 17306
	public GameObject floorTile;

	// Token: 0x0400439B RID: 17307
	public GameObject smallFloorTile;

	// Token: 0x0400439C RID: 17308
	public GameObject smallFloorTileVent;

	// Token: 0x0400439D RID: 17309
	public GameObject ceilingTile;

	// Token: 0x0400439E RID: 17310
	public GameObject smallCeilingTile;

	// Token: 0x0400439F RID: 17311
	public GameObject smallCeilingTileVent;

	// Token: 0x040043A0 RID: 17312
	public GameObject wallTile;

	// Token: 0x040043A1 RID: 17313
	public GameObject shortWallTile;

	// Token: 0x040043A2 RID: 17314
	public GameObject corner;

	// Token: 0x040043A3 RID: 17315
	public GameObject quoin;

	// Token: 0x040043A4 RID: 17316
	public GameObject elevator;

	// Token: 0x040043A5 RID: 17317
	public GameObject peekUnderDoor;

	// Token: 0x040043A6 RID: 17318
	public InteractablePreset peekInteractable;

	// Token: 0x040043A7 RID: 17319
	public GameObject exteriorShadowLight;

	// Token: 0x040043A8 RID: 17320
	public InteractablePreset airVent;

	// Token: 0x040043A9 RID: 17321
	public GameObject policeTape;

	// Token: 0x040043AA RID: 17322
	public GameObject damageCollider;

	// Token: 0x040043AB RID: 17323
	public GameObject streetSniperMuzzleFlash;

	// Token: 0x040043AC RID: 17324
	public GameObject lightningStrike;

	// Token: 0x040043AD RID: 17325
	public GameObject fingerprint;

	// Token: 0x040043AE RID: 17326
	public GameObject footprint;

	// Token: 0x040043AF RID: 17327
	public GameObject scenePoserFigure;

	// Token: 0x040043B0 RID: 17328
	public GameObject shatterShard;

	// Token: 0x040043B1 RID: 17329
	public GameObject glassShard;

	// Token: 0x040043B2 RID: 17330
	public GameObject flashBombFlash;

	// Token: 0x040043B3 RID: 17331
	public GameObject incapacitatorFlash;

	// Token: 0x040043B4 RID: 17332
	public InteractablePreset bloodPool;

	// Token: 0x040043B5 RID: 17333
	[Header("Blood Patterns")]
	public GameObject spatterSimulation;

	// Token: 0x040043B6 RID: 17334
	[Header("Modular Location Objects")]
	public GameObject district;

	// Token: 0x040043B7 RID: 17335
	public GameObject block;

	// Token: 0x040043B8 RID: 17336
	public GameObject cityTile;

	// Token: 0x040043B9 RID: 17337
	public GameObject building;

	// Token: 0x040043BA RID: 17338
	public GameObject street;

	// Token: 0x040043BB RID: 17339
	public GameObject floor;

	// Token: 0x040043BC RID: 17340
	public GameObject address;

	// Token: 0x040043BD RID: 17341
	public GameObject room;

	// Token: 0x040043BE RID: 17342
	public GameObject tile;

	// Token: 0x040043BF RID: 17343
	public GameObject node;

	// Token: 0x040043C0 RID: 17344
	public GameObject wall;

	// Token: 0x040043C1 RID: 17345
	public GameObject door;

	// Token: 0x040043C2 RID: 17346
	public GameObject covingShort;

	// Token: 0x040043C3 RID: 17347
	public GameObject covingLong;

	// Token: 0x040043C4 RID: 17348
	public GameObject covingCorner;

	// Token: 0x040043C5 RID: 17349
	[Header("Containers/References")]
	public Transform camHeightParent;

	// Token: 0x040043C6 RID: 17350
	public GameObject cityContainer;

	// Token: 0x040043C7 RID: 17351
	public RectTransform tooltipsContainer;

	// Token: 0x040043C8 RID: 17352
	public RectTransform contextMenuContainer;

	// Token: 0x040043C9 RID: 17353
	public Transform pathfindDebugParent;

	// Token: 0x040043CA RID: 17354
	public RectTransform menuCanvas;

	// Token: 0x040043CB RID: 17355
	public RectTransform tooltipsCanvas;

	// Token: 0x040043CC RID: 17356
	public Transform mapContainer;

	// Token: 0x040043CD RID: 17357
	public RectTransform dialogRect;

	// Token: 0x040043CE RID: 17358
	public RectTransform dialogOptionContainer;

	// Token: 0x040043CF RID: 17359
	public Transform poserContainer;

	// Token: 0x040043D0 RID: 17360
	public RectTransform objectSelectionContainer;

	// Token: 0x040043D1 RID: 17361
	public Transform debugDecorContainer;

	// Token: 0x040043D2 RID: 17362
	[Header("Floor Edit")]
	public GameObject editorTile;

	// Token: 0x040043D3 RID: 17363
	public GameObject entranceArrow;

	// Token: 0x040043D4 RID: 17364
	public GameObject wallTrigger;

	// Token: 0x040043D5 RID: 17365
	public GameObject blueprintWallLong;

	// Token: 0x040043D6 RID: 17366
	public GameObject blueprintWallShort;

	// Token: 0x040043D7 RID: 17367
	public GameObject heldSelector;

	// Token: 0x040043D8 RID: 17368
	public GameObject debugAttemptObject;

	// Token: 0x040043D9 RID: 17369
	public GameObject debugNodeDisplay;

	// Token: 0x040043DA RID: 17370
	private static PrefabControls _instance;
}
