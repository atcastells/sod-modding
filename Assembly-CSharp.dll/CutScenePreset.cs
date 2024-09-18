using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006F8 RID: 1784
[CreateAssetMenu(fileName = "cutscene_data", menuName = "Database/Cut Scene")]
public class CutScenePreset : SoCustomComparison
{
	// Token: 0x0600252A RID: 9514 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void RecordCurrentPositionToNewShot()
	{
	}

	// Token: 0x04003336 RID: 13110
	[Header("Timeline")]
	public List<CutScenePreset.CutSceneElement> elementList = new List<CutScenePreset.CutSceneElement>();

	// Token: 0x04003337 RID: 13111
	[Space(7f)]
	public bool fadeIn = true;

	// Token: 0x04003338 RID: 13112
	[ShowIf("fadeIn")]
	public float fadeInTime = 2f;

	// Token: 0x04003339 RID: 13113
	public bool fadeOut = true;

	// Token: 0x0400333A RID: 13114
	[ShowIf("fadeOut")]
	public float fadeOutTime = 2f;

	// Token: 0x0400333B RID: 13115
	[Space(7f)]
	public Sprite displayImage;

	// Token: 0x0400333C RID: 13116
	public float imageFadeIn;

	// Token: 0x0400333D RID: 13117
	public float imageFadeInSpeed = 1f;

	// Token: 0x0400333E RID: 13118
	public float imageFadeOut;

	// Token: 0x0400333F RID: 13119
	public float imageFadeOutSpeed;

	// Token: 0x04003340 RID: 13120
	[Header("Settings")]
	public bool disableAISpeech = true;

	// Token: 0x04003341 RID: 13121
	public CutScenePreset.OnEndScene onEnd;

	// Token: 0x020006F9 RID: 1785
	[Serializable]
	public class CutSceneElement
	{
		// Token: 0x04003342 RID: 13122
		public string name;

		// Token: 0x04003343 RID: 13123
		public bool disable;

		// Token: 0x04003344 RID: 13124
		public CutScenePreset.ElementType elementType;

		// Token: 0x04003345 RID: 13125
		[Space(5f)]
		public List<CutScenePreset.CameraMovement> movement;

		// Token: 0x04003346 RID: 13126
		public AnimationCurve lerpPositionGraph;

		// Token: 0x04003347 RID: 13127
		public AnimationCurve lerpRotationGraph;

		// Token: 0x04003348 RID: 13128
		[Space(7f)]
		public string ddsMessage;

		// Token: 0x04003349 RID: 13129
		public float messageDelay;
	}

	// Token: 0x020006FA RID: 1786
	public enum ElementType
	{
		// Token: 0x0400334B RID: 13131
		newShot,
		// Token: 0x0400334C RID: 13132
		ddsMessage
	}

	// Token: 0x020006FB RID: 1787
	public enum OnEndScene
	{
		// Token: 0x0400334E RID: 13134
		resumeGameplay,
		// Token: 0x0400334F RID: 13135
		startGame,
		// Token: 0x04003350 RID: 13136
		endGame
	}

	// Token: 0x020006FC RID: 1788
	public enum AnchorType
	{
		// Token: 0x04003352 RID: 13138
		blockCorner,
		// Token: 0x04003353 RID: 13139
		middle
	}

	// Token: 0x020006FD RID: 1789
	[Serializable]
	public class CameraMovement
	{
		// Token: 0x04003354 RID: 13140
		public float atDuration;

		// Token: 0x04003355 RID: 13141
		public Vector3 camPos;

		// Token: 0x04003356 RID: 13142
		public Vector3 camEuler;

		// Token: 0x04003357 RID: 13143
		public CutScenePreset.AnchorType anchor;

		// Token: 0x04003358 RID: 13144
		public bool overridePosGraph;

		// Token: 0x04003359 RID: 13145
		[ShowIf("overridePosGraph")]
		public AnimationCurve lerpPositionGraphOverride;

		// Token: 0x0400335A RID: 13146
		public bool overrideRotGraph;

		// Token: 0x0400335B RID: 13147
		[ShowIf("overrideRotGraph")]
		public AnimationCurve lerpRotationGraphOverride;
	}
}
