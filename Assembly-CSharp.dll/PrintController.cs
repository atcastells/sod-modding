using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000418 RID: 1048
public class PrintController : MonoBehaviour
{
	// Token: 0x060017DD RID: 6109 RVA: 0x0016529C File Offset: 0x0016349C
	public void Setup(FingerprintScannerController.Print newPrint)
	{
		this.instancedMaterial = (this.projector.material = Object.Instantiate<Material>(this.fingerprintMaterial));
		this.printData = newPrint;
		base.transform.rotation = Quaternion.FromToRotation(Vector3.up, this.printData.normal);
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, Toolbox.Instance.GetPsuedoRandomNumber(0f, 360f, this.printData.worldPos.ToString(), false), base.transform.localEulerAngles.z);
		Interactable newInteractable = null;
		if (GameplayController.Instance.confirmedPrints.TryGetValue(this.printData.worldPos, ref newInteractable))
		{
			Game.Log("Player: Found existing fingerprint", 2);
			this.scanProgress = 1f;
			this.printConfirmed = true;
			this.printInteractable = base.gameObject.AddComponent<InteractableController>();
			this.printInteractable.Setup(newInteractable);
			InteractionController.Instance.OnPlayerLookAtChange();
			return;
		}
		string text = "Player: Existing fingerprint not found (";
		Vector3 worldPos = this.printData.worldPos;
		Game.Log(text + worldPos.ToString() + ")", 2);
	}

	// Token: 0x060017DE RID: 6110 RVA: 0x001653E0 File Offset: 0x001635E0
	private void Update()
	{
		float num = Vector3.Distance(base.transform.position, FirstPersonItemController.Instance.scannerRayPoint);
		float num2 = Mathf.InverseLerp(0f, FirstPersonItemController.Instance.printDetectionRadius, num);
		this.instancedMaterial.SetColor("_BaseColor", Color.Lerp(this.visibleColour, this.invisibleColour, num2));
	}

	// Token: 0x060017DF RID: 6111 RVA: 0x00165440 File Offset: 0x00163640
	public void ResetScan()
	{
		if (this.printConfirmed)
		{
			return;
		}
		this.scanProgress = 0f;
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x00165458 File Offset: 0x00163658
	public void PrintConfirmed()
	{
		if (!this.printConfirmed)
		{
			this.printConfirmed = true;
			if (GameplayController.Instance.confirmedPrints.ContainsKey(this.printData.worldPos))
			{
				return;
			}
			this.printInteractable = base.gameObject.AddComponent<InteractableController>();
			bool flag = false;
			Human human;
			if (this.printData.dynamicOwner == null)
			{
				List<Human> fingerprintOwnerPool = Toolbox.Instance.GetFingerprintOwnerPool(this.printData.room, this.printData.furniture, this.printData.interactable, this.printData.source, this.printData.worldPos, true);
				human = fingerprintOwnerPool[Toolbox.Instance.GetPsuedoRandomNumber(0, fingerprintOwnerPool.Count, this.printData.worldPos.ToString(), false)];
			}
			else
			{
				human = this.printData.dynamicOwner;
				flag = true;
			}
			Game.Log(string.Concat(new string[]
			{
				"Player: Discovered print belonging to ",
				human.name,
				" (Dynamic: ",
				flag.ToString(),
				")"
			}), 2);
			if (human.fingerprintLoop <= -1)
			{
				human.fingerprintLoop = GameplayController.Instance.printsLetterLoop;
				GameplayController.Instance.printsLetterLoop++;
			}
			Interactable newInteractable = InteractableCreator.Instance.CreateFingerprintInteractable(human, this.printData.worldPos, Vector3.zero, this.printData);
			this.printInteractable.Setup(newInteractable);
			InteractionController.Instance.OnPlayerLookAtChange();
			if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
			{
				ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
				if (chapterIntro != null && chapterIntro.thisCase != null)
				{
					if (chapterIntro.thisCase.currentActiveObjectives.Exists((Objective item) => item.queueElement.entryRef == "Scan for fingerprints"))
					{
						string newMessage = Strings.ComposeText(Strings.Get("ui.gamemessage", "fingerprint_prompt", Strings.Casing.asIs, false, false, false, null), null, Strings.LinkSetting.forceNoLinks, null, null, false);
						InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, newMessage, InterfaceControls.Icon.printScanner, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					}
				}
			}
			if (AchievementsController.Instance != null && Player.Instance.drunk >= 0.8f)
			{
				AchievementsController.Instance.UnlockAchievement("Beer Goggles", "fingerprint_while_drunk");
			}
		}
	}

	// Token: 0x04001D6C RID: 7532
	public FingerprintScannerController.Print printData;

	// Token: 0x04001D6D RID: 7533
	public Material fingerprintMaterial;

	// Token: 0x04001D6E RID: 7534
	public Material instancedMaterial;

	// Token: 0x04001D6F RID: 7535
	public DecalProjector projector;

	// Token: 0x04001D70 RID: 7536
	public Color visibleColour = Color.white;

	// Token: 0x04001D71 RID: 7537
	public Color invisibleColour = Color.clear;

	// Token: 0x04001D72 RID: 7538
	public float scanProgress;

	// Token: 0x04001D73 RID: 7539
	public bool printConfirmed;

	// Token: 0x04001D74 RID: 7540
	public InteractableController printInteractable;
}
