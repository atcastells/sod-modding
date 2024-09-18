using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200028B RID: 651
public class ScenePoserController : MonoBehaviour
{
	// Token: 0x06000EA4 RID: 3748 RVA: 0x000D3504 File Offset: 0x000D1704
	public void SetupCitizen(SceneRecorder.ActorCapture newCapture)
	{
		base.transform.position = newCapture.pos;
		base.transform.eulerAngles = newCapture.rot;
		this.node = Toolbox.Instance.FindClosestValidNodeToWorldPosition(newCapture.pos, false, true, false, default(Vector3Int), false, 0, false, 200);
		if (this.spawnedLeft != null && this.spawnedLeft.name != newCapture.lH.i)
		{
			Object.DestroyImmediate(this.spawnedLeft);
		}
		if (this.spawnedRight != null && this.spawnedLeft.name != newCapture.rH.i)
		{
			Object.DestroyImmediate(this.spawnedRight);
		}
		base.gameObject.SetActive(true);
		bool flag = true;
		if (this.human != null && this.human.humanID == newCapture.id)
		{
			flag = false;
		}
		if (Game.Instance.collectDebugData)
		{
			if (!flag)
			{
				string[] array = new string[6];
				array[0] = "Gameplay: Set up poser, old: ";
				int num = 1;
				Human human = this.human;
				array[num] = ((human != null) ? human.ToString() : null);
				array[2] = " new ";
				int num2 = 3;
				Human human2 = newCapture.GetHuman();
				array[num2] = ((human2 != null) ? human2.ToString() : null);
				array[4] = " ";
				int num3 = 5;
				ClothesPreset.OutfitCategory o = (ClothesPreset.OutfitCategory)newCapture.o;
				array[num3] = o.ToString();
				Game.Log(string.Concat(array), 2);
			}
			else
			{
				string text = "Gameplay: Set up poser, new citizen ";
				Human human3 = newCapture.GetHuman();
				string text2 = (human3 != null) ? human3.ToString() : null;
				string text3 = " ";
				ClothesPreset.OutfitCategory o = (ClothesPreset.OutfitCategory)newCapture.o;
				Game.Log(text + text2 + text3 + o.ToString(), 2);
			}
		}
		this.human = newCapture.GetHuman();
		base.name = "Poser: " + this.human.GetCitizenName();
		this.outfit = (ClothesPreset.OutfitCategory)newCapture.o;
		if (this.human.interactableController != null)
		{
			base.transform.localScale = this.human.interactableController.transform.localScale;
		}
		this.outfitController.outfits = new List<CitizenOutfitController.Outfit>();
		foreach (CitizenOutfitController.Outfit outfit in this.human.outfitController.outfits)
		{
			CitizenOutfitController.Outfit outfit2 = new CitizenOutfitController.Outfit();
			outfit2.category = outfit.category;
			foreach (CitizenOutfitController.OutfitClothes outfitClothes in outfit.clothes)
			{
				CitizenOutfitController.OutfitClothes outfitClothes2 = new CitizenOutfitController.OutfitClothes();
				outfitClothes2.clothes = outfitClothes.clothes;
				outfitClothes2.tags = outfitClothes.tags;
				outfitClothes2.baseColor = outfitClothes.baseColor;
				outfitClothes2.color1 = outfitClothes.color1;
				outfitClothes2.color2 = outfitClothes.color2;
				outfitClothes2.color3 = outfitClothes.color3;
				outfitClothes2.borrowed = outfitClothes.borrowed;
				outfit2.clothes.Add(outfitClothes2);
			}
			this.outfitController.outfits.Add(outfit2);
		}
		this.outfitController.human = this.human;
		this.outfitController.SetCurrentOutfit(this.outfit, true, flag, false);
		if (newCapture.limb != null && newCapture.limb.Count > 0)
		{
			Game.Log("Gameplay: Loading limb positions for " + base.name, 2);
			using (List<SceneRecorder.LimbCapture>.Enumerator enumerator3 = newCapture.limb.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					SceneRecorder.LimbCapture limbCapture = enumerator3.Current;
					Transform bodyAnchor = this.outfitController.GetBodyAnchor((CitizenOutfitController.CharacterAnchor)limbCapture.a);
					if (bodyAnchor != null)
					{
						bodyAnchor.rotation = limbCapture.wR;
						bodyAnchor.position = limbCapture.wP;
					}
				}
				goto IL_747;
			}
		}
		AnimationFrameReference.AnimationReference animationReference = AnimationFrameReference.Instance.GetAnimationReference((CitizenAnimationController.IdleAnimationState)newCapture.main, newCapture.pos.ToString());
		if (animationReference != null)
		{
			foreach (AnimationFrameReference.AnimationAnchorRef animationAnchorRef in animationReference.anim)
			{
				Transform bodyAnchor2 = this.outfitController.GetBodyAnchor(animationAnchorRef.anchor);
				if (bodyAnchor2 != null)
				{
					bodyAnchor2.localPosition = animationAnchorRef.localPos;
					bodyAnchor2.localRotation = animationAnchorRef.localRot;
				}
			}
			string text4 = "Gameplay: Loaded idle anim ";
			CitizenAnimationController.IdleAnimationState main = (CitizenAnimationController.IdleAnimationState)newCapture.main;
			Game.Log(text4 + main.ToString(), 2);
		}
		else
		{
			string text5 = "Gameplay: Could not find idle anim ";
			CitizenAnimationController.IdleAnimationState main = (CitizenAnimationController.IdleAnimationState)newCapture.main;
			Game.Log(text5 + main.ToString(), 2);
		}
		if (newCapture.main <= 0)
		{
			if (newCapture.sp == 1)
			{
				AnimationFrameReference.AnimationReference animationReference2 = AnimationFrameReference.Instance.walkingReference[Toolbox.Instance.GetPsuedoRandomNumber(0, AnimationFrameReference.Instance.walkingReference.Count, newCapture.pos.ToString(), false)];
				if (animationReference2 == null)
				{
					goto IL_659;
				}
				using (List<AnimationFrameReference.AnimationAnchorRef>.Enumerator enumerator4 = animationReference2.anim.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						AnimationFrameReference.AnimationAnchorRef animationAnchorRef2 = enumerator4.Current;
						Transform bodyAnchor3 = this.outfitController.GetBodyAnchor(animationAnchorRef2.anchor);
						if (bodyAnchor3 != null)
						{
							bodyAnchor3.localPosition = animationAnchorRef2.localPos;
							bodyAnchor3.localRotation = animationAnchorRef2.localRot;
						}
					}
					goto IL_659;
				}
			}
			if (newCapture.sp == 2)
			{
				AnimationFrameReference.AnimationReference animationReference3 = AnimationFrameReference.Instance.walkingReference[Toolbox.Instance.GetPsuedoRandomNumber(0, AnimationFrameReference.Instance.runningReference.Count, newCapture.pos.ToString(), false)];
				if (animationReference3 != null)
				{
					foreach (AnimationFrameReference.AnimationAnchorRef animationAnchorRef3 in animationReference3.anim)
					{
						Transform bodyAnchor4 = this.outfitController.GetBodyAnchor(animationAnchorRef3.anchor);
						if (bodyAnchor4 != null)
						{
							bodyAnchor4.localPosition = animationAnchorRef3.localPos;
							bodyAnchor4.localRotation = animationAnchorRef3.localRot;
						}
					}
				}
			}
		}
		IL_659:
		if (newCapture.arms > 0)
		{
			AnimationFrameReference.AnimationReference animationReference4 = AnimationFrameReference.Instance.GetAnimationReference((CitizenAnimationController.ArmsBoolSate)newCapture.arms, newCapture.pos.ToString());
			if (animationReference4 != null)
			{
				foreach (AnimationFrameReference.AnimationAnchorRef animationAnchorRef4 in animationReference4.anim)
				{
					Transform bodyAnchor5 = this.outfitController.GetBodyAnchor(animationAnchorRef4.anchor);
					if (bodyAnchor5 != null)
					{
						bodyAnchor5.localPosition = animationAnchorRef4.localPos;
						bodyAnchor5.localRotation = animationAnchorRef4.localRot;
					}
				}
				string text6 = "Gameplay: Loaded arms anim ";
				CitizenAnimationController.ArmsBoolSate arms = (CitizenAnimationController.ArmsBoolSate)newCapture.arms;
				Game.Log(text6 + arms.ToString(), 2);
			}
			else
			{
				string text7 = "Gameplay: Could not find arms anim ";
				CitizenAnimationController.ArmsBoolSate arms = (CitizenAnimationController.ArmsBoolSate)newCapture.arms;
				Game.Log(text7 + arms.ToString(), 2);
			}
		}
		IL_747:
		if (newCapture.lH != null && this.spawnedLeft == null)
		{
			foreach (KeyValuePair<string, InteractablePreset> keyValuePair in Toolbox.Instance.objectPresetDictionary)
			{
				if (keyValuePair.Value.prefab != null && keyValuePair.Value.prefab.name == newCapture.lH.i)
				{
					this.spawnedLeft = Toolbox.Instance.SpawnObject(keyValuePair.Value.prefab, this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandLeft));
					this.spawnedLeft.name = keyValuePair.Value.prefab.name;
					this.spawnedLeft.transform.localPosition = newCapture.lH.wP;
					this.spawnedLeft.transform.localRotation = newCapture.lH.wR;
					LODGroup componentInChildren = this.spawnedLeft.GetComponentInChildren<LODGroup>(true);
					if (componentInChildren != null)
					{
						Object.DestroyImmediate(componentInChildren);
						break;
					}
					break;
				}
			}
		}
		if (newCapture.rH != null && this.spawnedRight == null)
		{
			foreach (KeyValuePair<string, InteractablePreset> keyValuePair2 in Toolbox.Instance.objectPresetDictionary)
			{
				if (keyValuePair2.Value.prefab != null && keyValuePair2.Value.prefab.name == newCapture.rH.i)
				{
					this.spawnedRight = Toolbox.Instance.SpawnObject(keyValuePair2.Value.prefab, this.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.HandRight));
					this.spawnedRight.name = keyValuePair2.Value.prefab.name;
					this.spawnedRight.transform.localPosition = newCapture.rH.wP;
					this.spawnedRight.transform.localRotation = newCapture.rH.wR;
					LODGroup componentInChildren2 = this.spawnedRight.GetComponentInChildren<LODGroup>(true);
					if (componentInChildren2 != null)
					{
						Object.DestroyImmediate(componentInChildren2);
						break;
					}
					break;
				}
			}
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x040011B8 RID: 4536
	[Header("Components")]
	public CitizenOutfitController outfitController;

	// Token: 0x040011B9 RID: 4537
	[Header("State")]
	public Human human;

	// Token: 0x040011BA RID: 4538
	public NewNode node;

	// Token: 0x040011BB RID: 4539
	public ClothesPreset.OutfitCategory outfit;

	// Token: 0x040011BC RID: 4540
	public GameObject spawnedLeft;

	// Token: 0x040011BD RID: 4541
	public GameObject spawnedRight;
}
