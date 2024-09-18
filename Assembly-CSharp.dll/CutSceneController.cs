using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x020002B0 RID: 688
public class CutSceneController : MonoBehaviour
{
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x06000F57 RID: 3927 RVA: 0x000DAC95 File Offset: 0x000D8E95
	public static CutSceneController Instance
	{
		get
		{
			return CutSceneController._instance;
		}
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x000DAC9C File Offset: 0x000D8E9C
	private void Awake()
	{
		if (CutSceneController._instance != null && CutSceneController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CutSceneController._instance = this;
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x000DACCC File Offset: 0x000D8ECC
	public void PlayCutScene(CutScenePreset newPreset)
	{
		if (this.cutSceneActive)
		{
			Game.Log("Gameplay: Cut scene " + this.preset.name + " is already active...", 2);
			return;
		}
		MusicController.Instance.SetGameState(MusicCue.MusicTriggerGameState.inCutscene);
		this.preset = newPreset;
		this.cursor = 0;
		this.sceneTimer = 0f;
		this.previousElement = null;
		if (this.preset.fadeIn)
		{
			InterfaceController.Instance.fade = 1f;
			InterfaceController.Instance.Fade(0f, this.preset.fadeInTime, true);
		}
		if (this.preset.fadeOut)
		{
			this.triggeredFadeOut = false;
			for (int i = 0; i < this.preset.elementList.Count; i++)
			{
				if (this.preset.elementList[i].elementType == CutScenePreset.ElementType.newShot)
				{
					this.finalShot = this.preset.elementList[i];
				}
			}
		}
		this.SetActive(true);
		this.displayImage.sprite = this.preset.displayImage;
		this.displayImageRend.SetAlpha(0f);
		this.imageFadeIn = 0f;
		this.imageFadeOut = 0f;
		this.triggeredImage = false;
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x000DAE0C File Offset: 0x000D900C
	private void Update()
	{
		if (this.cutSceneActive)
		{
			if (SessionData.Instance.play)
			{
				if (this.cursor >= this.preset.elementList.Count)
				{
					this.SetActive(false);
					return;
				}
				CutScenePreset.CutSceneElement cutSceneElement = this.preset.elementList[this.cursor];
				this.sceneTimer += Time.deltaTime;
				while (cutSceneElement.disable && this.cursor < this.preset.elementList.Count)
				{
					this.cursor++;
					cutSceneElement = this.preset.elementList[this.cursor];
				}
				bool updateMixing = false;
				if (this.previousElement != cutSceneElement)
				{
					if (cutSceneElement.elementType == CutScenePreset.ElementType.newShot)
					{
						this.currentShotTimer = 0f;
						updateMixing = true;
						this.currentCamMovement = new List<CutScenePreset.CameraMovement>(cutSceneElement.movement);
						this.currentCamMovement.Sort((CutScenePreset.CameraMovement p1, CutScenePreset.CameraMovement p2) => p1.atDuration.CompareTo(p2.atDuration));
						this.currentFrom = this.currentCamMovement[0];
						Game.Log("Gameplay: Loading current shot with " + this.currentCamMovement.Count.ToString() + " movement nodes...", 2);
						if (this.currentCamMovement.Count > 1)
						{
							this.currentTo = this.currentCamMovement[1];
						}
						else
						{
							this.currentTo = null;
						}
						if (cutSceneElement.ddsMessage == null || cutSceneElement.ddsMessage.Length <= 0)
						{
							goto IL_2C4;
						}
						List<int> list;
						using (List<string>.Enumerator enumerator = Player.Instance.ParseDDSMessage(cutSceneElement.ddsMessage, null, out list, false, null, false).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string text = enumerator.Current;
								Game.Log("Gameplay: Trigger cut scene text: " + text, 2);
								SpeechController speechController = Player.Instance.speechController;
								string dictionary = "dds.blocks";
								string speechEntryRef = text;
								bool useParsing = true;
								bool shout = false;
								bool interupt = true;
								Color white = Color.white;
								speechController.Speak(dictionary, speechEntryRef, useParsing, shout, interupt, cutSceneElement.messageDelay, true, white, null, false, false, null, null, null, null);
							}
							goto IL_2C4;
						}
					}
					if (cutSceneElement.elementType == CutScenePreset.ElementType.ddsMessage)
					{
						if (cutSceneElement.ddsMessage != null && cutSceneElement.ddsMessage.Length > 0)
						{
							List<int> list;
							foreach (string text2 in Player.Instance.ParseDDSMessage(cutSceneElement.ddsMessage, null, out list, false, null, false))
							{
								Game.Log("Gameplay: Trigger cut scene text: " + text2, 2);
								SpeechController speechController2 = Player.Instance.speechController;
								string dictionary2 = "dds.blocks";
								string speechEntryRef2 = text2;
								bool useParsing2 = true;
								bool shout2 = false;
								bool interupt2 = true;
								Color white = Color.white;
								speechController2.Speak(dictionary2, speechEntryRef2, useParsing2, shout2, interupt2, cutSceneElement.messageDelay, true, white, null, false, false, null, null, null, null);
							}
						}
						this.cursor++;
					}
					IL_2C4:
					this.previousElement = cutSceneElement;
				}
				if (cutSceneElement.elementType == CutScenePreset.ElementType.newShot)
				{
					this.currentShotTimer += Time.deltaTime;
					if (this.currentFrom != null && this.currentTo != null)
					{
						if (this.currentShotTimer <= this.currentTo.atDuration)
						{
							float num = Mathf.InverseLerp(this.currentFrom.atDuration, this.currentTo.atDuration, this.currentShotTimer);
							Vector3 vector = this.currentFrom.camPos;
							Vector3 vector2 = this.currentTo.camPos;
							Vector3 vector3 = CityData.Instance.CityTileToRealpos(Vector2.zero);
							if (this.currentFrom.anchor == CutScenePreset.AnchorType.blockCorner)
							{
								vector += vector3;
							}
							if (this.currentTo.anchor == CutScenePreset.AnchorType.blockCorner)
							{
								vector2 += vector3;
							}
							float num2;
							if (this.currentTo.overridePosGraph)
							{
								num2 = this.currentTo.lerpPositionGraphOverride.Evaluate(num);
							}
							else
							{
								num2 = cutSceneElement.lerpPositionGraph.Evaluate(num);
							}
							float num3;
							if (this.currentTo.overrideRotGraph)
							{
								num3 = this.currentTo.lerpRotationGraphOverride.Evaluate(num);
							}
							else
							{
								num3 = cutSceneElement.lerpRotationGraph.Evaluate(num);
							}
							Vector3 position = Vector3.Lerp(vector, vector2, num2);
							Quaternion rotation = Quaternion.Slerp(Quaternion.Euler(this.currentFrom.camEuler), Quaternion.Euler(this.currentTo.camEuler), num3);
							this.UpdateCam(position, rotation, updateMixing);
							if (this.preset.fadeOut && !this.triggeredFadeOut && cutSceneElement == this.finalShot && this.currentTo == this.currentCamMovement[this.currentCamMovement.Count - 1] && this.currentShotTimer >= this.currentTo.atDuration - this.preset.fadeOutTime)
							{
								InterfaceController.Instance.Fade(1f, this.preset.fadeOutTime, true);
								this.triggeredFadeOut = true;
							}
						}
						else
						{
							this.currentFrom = this.currentTo;
							int num4 = this.currentCamMovement.IndexOf(this.currentTo);
							int num5 = num4 + 1;
							Game.Log("Gameplay: Movement finished, searching for next at " + num5.ToString() + "/" + this.currentCamMovement.Count.ToString(), 2);
							if (num5 < this.currentCamMovement.Count)
							{
								Game.Log("Gameplay: Setting next movement at index " + num5.ToString() + "/" + this.currentCamMovement.Count.ToString(), 2);
								this.currentTo = this.currentCamMovement[num5];
							}
							else
							{
								Game.Log("Gameplay: Shot has finished " + (num4 + 1).ToString() + "/" + this.currentCamMovement.Count.ToString(), 2);
								this.currentTo = null;
								this.currentFrom = null;
								this.cursor++;
							}
						}
					}
				}
				if (this.preset.displayImage != null)
				{
					if (!this.triggeredImage)
					{
						if (this.sceneTimer >= this.preset.imageFadeIn)
						{
							this.triggeredImage = true;
							return;
						}
					}
					else
					{
						if (this.imageFadeIn < 1f)
						{
							this.imageFadeIn += Time.deltaTime / this.preset.imageFadeInSpeed;
							this.imageFadeIn = Mathf.Clamp01(this.imageFadeIn);
							this.imageFadeOut = this.imageFadeIn;
							this.displayImageRend.SetAlpha(this.imageFadeIn);
							return;
						}
						if (this.sceneTimer >= this.preset.imageFadeOut && this.imageFadeOut > 0f)
						{
							this.imageFadeOut -= Time.deltaTime / this.preset.imageFadeOutSpeed;
							this.imageFadeOut = Mathf.Clamp01(this.imageFadeOut);
							this.displayImageRend.SetAlpha(this.imageFadeOut);
							return;
						}
					}
				}
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x000DB50C File Offset: 0x000D970C
	private void UpdateCam(Vector3 position, Quaternion rotation, bool updateMixing)
	{
		Player.Instance.transform.position = position;
		CameraController.Instance.cam.transform.rotation = rotation;
		Player.Instance.UpdateMovementPhysics(false);
		if (updateMixing)
		{
			AudioController.Instance.UpdateMixing();
		}
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x000DB54C File Offset: 0x000D974C
	private void SetActive(bool val)
	{
		if (this.cutSceneActive != val)
		{
			this.cutSceneActive = val;
			Game.Log("Gameplay: Set cut scene active: " + this.cutSceneActive.ToString(), 2);
			if (this.cutSceneActive)
			{
				SessionData.Instance.motionBlur.active = false;
				this.playerSavedPosition = Player.Instance.transform.position;
				this.camSavedLocalQuat = CameraController.Instance.cam.transform.localRotation;
				this.savedFreeCam = Game.Instance.freeCam;
				this.savedInaudible = Game.Instance.inaudiblePlayer;
				this.savedInvincible = Game.Instance.invinciblePlayer;
				this.savedInvisible = Game.Instance.invisiblePlayer;
				this.savedPhotoMode = Game.Instance.screenshotMode;
				MapController.Instance.directionalArrowContainer.SetActive(false);
				Game.Instance.SetFreeCamMode(true);
				Game.Instance.SetScreenshotMode(true, true);
				Game.Instance.inaudiblePlayer = true;
				Game.Instance.invisiblePlayer = true;
				Game.Instance.invinciblePlayer = true;
				Player.Instance.EnablePlayerMovement(false, true);
				Player.Instance.EnablePlayerMouseLook(false, true);
				this.displayImage.gameObject.SetActive(true);
				InterfaceController.Instance.RemoveAllMouseInteractionComponents();
				base.enabled = true;
				return;
			}
			Game.Instance.SetFreeCamMode(this.savedFreeCam);
			Game.Instance.SetScreenshotMode(this.savedPhotoMode, false);
			Game.Instance.invisiblePlayer = this.savedInvisible;
			Game.Instance.inaudiblePlayer = this.savedInaudible;
			Game.Instance.invinciblePlayer = this.savedInvincible;
			Player.Instance.transform.position = this.playerSavedPosition;
			CameraController.Instance.cam.transform.localRotation = this.camSavedLocalQuat;
			Player.Instance.fps.InitialiseController(true, false);
			if (Game.Instance.enableDirectionalArrow)
			{
				MapController.Instance.directionalArrowContainer.SetActive(MapController.Instance.displayDirectionArrow);
			}
			else
			{
				MapController.Instance.directionalArrowContainer.SetActive(false);
			}
			if (!Player.Instance.transitionActive && Player.Instance.answeringPhone == null)
			{
				Player.Instance.EnablePlayerMovement(true, true);
				Player.Instance.EnablePlayerMouseLook(true, false);
			}
			PlayerPrefsController.GameSetting gameSetting = PlayerPrefsController.Instance.gameSettingControls.Find((PlayerPrefsController.GameSetting item) => item.identifier.ToLower() == "motionblur");
			SessionData.Instance.motionBlur.active = Convert.ToBoolean(gameSetting.intValue);
			this.displayImage.gameObject.SetActive(false);
			if (this.preset.onEnd == CutScenePreset.OnEndScene.resumeGameplay && this.preset.fadeOut)
			{
				InterfaceController.Instance.Fade(0f, 2f, true);
			}
			if (this.preset.onEnd == CutScenePreset.OnEndScene.startGame)
			{
				MusicController.Instance.SetGameState(MusicCue.MusicTriggerGameState.inGame);
				AudioController.Instance.PlayOneShotDelayed(0f, AudioControls.Instance.spawnPlayer, null, null, Player.Instance.transform.position, null, 1f, null, false);
				AudioController.Instance.StopSound(Player.Instance.onlyMusicSnapshot, AudioController.StopType.fade, "Game started, game audio fading in");
				if (this.preset.fadeOut)
				{
					InterfaceController.Instance.Fade(0f, 2f, true);
				}
				CityConstructor.Instance.TriggerStartEvent();
				return;
			}
			if (this.preset.onEnd == CutScenePreset.OnEndScene.endGame)
			{
				RestartSafeController.Instance.loadFromDirty = false;
				AudioController.Instance.StopAllSounds();
				InputController.Instance.SetCursorLock(false);
				InputController.Instance.SetCursorVisible(true);
				SceneManager.LoadScene("Main");
			}
		}
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x000DB8FD File Offset: 0x000D9AFD
	[Button(null, 0)]
	public void PlayScene()
	{
		if (this.debugLoad != null)
		{
			this.PlayCutScene(this.debugLoad);
		}
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x000DB91C File Offset: 0x000D9B1C
	[Button(null, 0)]
	public void StopScene()
	{
		if (this.cutSceneActive)
		{
			this.SetActive(false);
			for (int i = 0; i < Player.Instance.speechController.speechQueue.Count; i++)
			{
				if (!Player.Instance.speechController.speechQueue[i].isObjective)
				{
					Player.Instance.speechController.speechQueue.RemoveAt(i);
					i--;
				}
			}
		}
	}

	// Token: 0x040012A5 RID: 4773
	[Header("Components")]
	public Image displayImage;

	// Token: 0x040012A6 RID: 4774
	public CanvasRenderer displayImageRend;

	// Token: 0x040012A7 RID: 4775
	[Header("State")]
	public bool cutSceneActive;

	// Token: 0x040012A8 RID: 4776
	public float sceneTimer;

	// Token: 0x040012A9 RID: 4777
	public CutScenePreset preset;

	// Token: 0x040012AA RID: 4778
	public int cursor;

	// Token: 0x040012AB RID: 4779
	private CutScenePreset.CutSceneElement previousElement;

	// Token: 0x040012AC RID: 4780
	private List<CutScenePreset.CameraMovement> currentCamMovement;

	// Token: 0x040012AD RID: 4781
	private CutScenePreset.CameraMovement currentFrom;

	// Token: 0x040012AE RID: 4782
	private CutScenePreset.CameraMovement currentTo;

	// Token: 0x040012AF RID: 4783
	public float currentShotTimer;

	// Token: 0x040012B0 RID: 4784
	private Vector3 playerSavedPosition;

	// Token: 0x040012B1 RID: 4785
	private Quaternion camSavedLocalQuat;

	// Token: 0x040012B2 RID: 4786
	private bool savedFreeCam;

	// Token: 0x040012B3 RID: 4787
	private bool savedInaudible;

	// Token: 0x040012B4 RID: 4788
	private bool savedInvisible;

	// Token: 0x040012B5 RID: 4789
	private bool savedInvincible;

	// Token: 0x040012B6 RID: 4790
	private bool savedPhotoMode;

	// Token: 0x040012B7 RID: 4791
	private bool triggeredFadeOut;

	// Token: 0x040012B8 RID: 4792
	private CutScenePreset.CutSceneElement finalShot;

	// Token: 0x040012B9 RID: 4793
	private bool triggeredImage;

	// Token: 0x040012BA RID: 4794
	private float imageFadeIn;

	// Token: 0x040012BB RID: 4795
	private float imageFadeOut = 1f;

	// Token: 0x040012BC RID: 4796
	[Header("Debug")]
	public CutScenePreset debugLoad;

	// Token: 0x040012BD RID: 4797
	private static CutSceneController _instance;
}
