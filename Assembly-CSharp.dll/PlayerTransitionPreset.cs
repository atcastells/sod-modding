using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007AD RID: 1965
[CreateAssetMenu(fileName = "playertransition_data", menuName = "Database/Player Transition Preset")]
public class PlayerTransitionPreset : SoCustomComparison
{
	// Token: 0x04003BBA RID: 15290
	[Tooltip("Transition speed in seconds")]
	[Header("Transition")]
	public float transitionTime = 1f;

	// Token: 0x04003BBB RID: 15291
	[Header("Control")]
	[Tooltip("Player keeps movement control during transition")]
	public bool retainMovementControl;

	// Token: 0x04003BBC RID: 15292
	[Tooltip("If above is true, this movement multiplier is applied")]
	public AnimationCurve controlCurve;

	// Token: 0x04003BBD RID: 15293
	[Tooltip("Mouse look control curve")]
	public AnimationCurve mouseLookControlCurve;

	// Token: 0x04003BBE RID: 15294
	[Header("Height")]
	[Tooltip("Player height multiplier")]
	public float playerHeightMP = 1f;

	// Token: 0x04003BBF RID: 15295
	[Tooltip("Camera height multiplier")]
	public float CamHeightMP = 1f;

	// Token: 0x04003BC0 RID: 15296
	[Tooltip("If true, both the above will be different depending on whether the player is crouched or stood up...")]
	public bool factorInCrouching;

	// Token: 0x04003BC1 RID: 15297
	[Space(5f)]
	public AnimationCurve heightCurve;

	// Token: 0x04003BC2 RID: 15298
	public AnimationCurve camHeightCurve;

	// Token: 0x04003BC3 RID: 15299
	[Space(5f)]
	[Header("Movement")]
	public bool useXMovement = true;

	// Token: 0x04003BC4 RID: 15300
	[EnableIf("useXMovement")]
	public AnimationCurve playerXCurve;

	// Token: 0x04003BC5 RID: 15301
	public bool useYMovement = true;

	// Token: 0x04003BC6 RID: 15302
	[EnableIf("useYMovement")]
	public AnimationCurve playerYCurve;

	// Token: 0x04003BC7 RID: 15303
	public bool useZMovement = true;

	// Token: 0x04003BC8 RID: 15304
	[EnableIf("useZMovement")]
	public AnimationCurve playerZCurve;

	// Token: 0x04003BC9 RID: 15305
	[Space(5f)]
	public PlayerTransitionPreset.TransitionPosition transitionRelativity;

	// Token: 0x04003BCA RID: 15306
	[Space(5f)]
	[Tooltip("Performing this transition won't override a previous stored position...")]
	public bool disableWriteReturnPosition;

	// Token: 0x04003BCB RID: 15307
	[Tooltip("If true then transition to the stored return postion using the curve below.")]
	public bool transitionToSavedReturnPosition;

	// Token: 0x04003BCC RID: 15308
	[Tooltip("If true then transition from the player's current position to the correct position as described in these settings.")]
	public bool transitionFromExistingPosition = true;

	// Token: 0x04003BCD RID: 15309
	[Tooltip("Transition curve for the above")]
	public AnimationCurve positionTransitionCurve;

	// Token: 0x04003BCE RID: 15310
	[Space(5f)]
	[Tooltip("Useful for door interactables")]
	public bool invertXPositionBasedOnRelativePlayerX;

	// Token: 0x04003BCF RID: 15311
	public bool invertYPositionBasedOnRelativePlayerY;

	// Token: 0x04003BD0 RID: 15312
	public bool invertZPositionBasedOnRelativePlayerZ;

	// Token: 0x04003BD1 RID: 15313
	[Tooltip("Use a raycast to check this position is valid")]
	public bool raycastCheck;

	// Token: 0x04003BD2 RID: 15314
	[EnableIf("raycastCheck")]
	public PlayerTransitionPreset onFailUse;

	// Token: 0x04003BD3 RID: 15315
	[Space(7f)]
	[Tooltip("Enable movement upon end transition")]
	public bool allowMovementOnEnd;

	// Token: 0x04003BD4 RID: 15316
	[Tooltip("The movement speed on end")]
	[EnableIf("allowMovementOnEnd")]
	public bool restoreNormalMovementSpeed;

	// Token: 0x04003BD5 RID: 15317
	[DisableIf("restoreNormalMovementSpeed")]
	public float customMovementSpeed = 1f;

	// Token: 0x04003BD6 RID: 15318
	public bool disableGravity;

	// Token: 0x04003BD7 RID: 15319
	public bool disableHeadBob;

	// Token: 0x04003BD8 RID: 15320
	[Header("Mouse Look")]
	public bool useXLook = true;

	// Token: 0x04003BD9 RID: 15321
	[EnableIf("useXLook")]
	public AnimationCurve playerXLookCurve;

	// Token: 0x04003BDA RID: 15322
	public bool useYLook = true;

	// Token: 0x04003BDB RID: 15323
	[EnableIf("useYLook")]
	public AnimationCurve playerYLookCurve;

	// Token: 0x04003BDC RID: 15324
	public bool useZLook = true;

	// Token: 0x04003BDD RID: 15325
	[EnableIf("useZLook")]
	public AnimationCurve playerZLookCurve;

	// Token: 0x04003BDE RID: 15326
	[Space(5f)]
	public PlayerTransitionPreset.TransitionPosition lookRelativity;

	// Token: 0x04003BDF RID: 15327
	[Tooltip("If the above is set to relative to player, use the forward direction multiplied by this to determin the look position. This must be higher than the distance the player can move forward.")]
	public float forwardPositionModifier = 2f;

	// Token: 0x04003BE0 RID: 15328
	[Tooltip("Multiply the X, Y and Z movement by this. Useful for when relative to player and adjusting the forward position modifier...")]
	public float lookMovementMultiplier = 1f;

	// Token: 0x04003BE1 RID: 15329
	[Space(5f)]
	public bool applyCameraRoll;

	// Token: 0x04003BE2 RID: 15330
	public AnimationCurve cameraRoll;

	// Token: 0x04003BE3 RID: 15331
	public float rollMultiplier = 27.5f;

	// Token: 0x04003BE4 RID: 15332
	[Tooltip("Make sure camera roll is reset at the end of the transition")]
	public bool resetCameraRoll = true;

	// Token: 0x04003BE5 RID: 15333
	[Space(5f)]
	[Tooltip("If true then transition from the player's current mouse position to the correct position as described in these settings.")]
	public bool transitionFromExistingMouse = true;

	// Token: 0x04003BE6 RID: 15334
	[Tooltip("Transition curve for the above")]
	public AnimationCurve mouseTransitionCurve;

	// Token: 0x04003BE7 RID: 15335
	[Header("VFX")]
	public bool useChromaticAberration;

	// Token: 0x04003BE8 RID: 15336
	[EnableIf("useChromaticAberration")]
	public AnimationCurve chromaticAberrationCurve;

	// Token: 0x04003BE9 RID: 15337
	public bool useGain;

	// Token: 0x04003BEA RID: 15338
	[EnableIf("useGain")]
	public AnimationCurve gainCurve;

	// Token: 0x04003BEB RID: 15339
	[Header("SFX")]
	[ReorderableList]
	public List<PlayerTransitionPreset.SFXSetting> sfx = new List<PlayerTransitionPreset.SFXSetting>();

	// Token: 0x04003BEC RID: 15340
	[Header("First Person")]
	[Tooltip("Force selection of nothing upon transition begin")]
	public bool forceHolsterOnTransition = true;

	// Token: 0x04003BED RID: 15341
	[Tooltip("Restore first person item after transition has ended")]
	[EnableIf("forceHolsterOnTransition")]
	public bool restoreHolsterOnTransitionEnd;

	// Token: 0x04003BEE RID: 15342
	[Tooltip("Allow weapon selection after transition has finished")]
	public bool allowWeaponSwitchingAfterTransition;

	// Token: 0x04003BEF RID: 15343
	[Tooltip("A recoil state can be switched to manually in the transition to add these in addition to the normal look curves")]
	[Space(7f)]
	public AnimationCurve playerXRecoilLookCurve;

	// Token: 0x04003BF0 RID: 15344
	public AnimationCurve playerYRecoilLookCurve;

	// Token: 0x04003BF1 RID: 15345
	public AnimationCurve playerZRecoilLookCurve;

	// Token: 0x04003BF2 RID: 15346
	[Header("Return")]
	public bool useCustomReturnPosition;

	// Token: 0x04003BF3 RID: 15347
	[Tooltip("Return position relative to the interactable")]
	[EnableIf("useCustomReturnPosition")]
	public Vector3 returnPostion;

	// Token: 0x020007AE RID: 1966
	public enum TransitionPosition
	{
		// Token: 0x04003BF5 RID: 15349
		relativeToInteractable,
		// Token: 0x04003BF6 RID: 15350
		relativeToPlayer
	}

	// Token: 0x020007AF RID: 1967
	[Serializable]
	public class SFXSetting
	{
		// Token: 0x04003BF7 RID: 15351
		public AudioEvent soundEvent;

		// Token: 0x04003BF8 RID: 15352
		public float atProgress;
	}
}
