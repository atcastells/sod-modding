using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Token: 0x020005EC RID: 1516
public class MaterialKeyController : MonoBehaviour
{
	// Token: 0x14000048 RID: 72
	// (add) Token: 0x0600215A RID: 8538 RVA: 0x001C6B74 File Offset: 0x001C4D74
	// (remove) Token: 0x0600215B RID: 8539 RVA: 0x001C6BAC File Offset: 0x001C4DAC
	public event MaterialKeyController.ColourKeyUpdate OnColourKeyUpdate;

	// Token: 0x0600215C RID: 8540 RVA: 0x001C6BE4 File Offset: 0x001C4DE4
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.wcc = newContentController;
		this.SetPageSize(new Vector2(640f, 350f));
		this.placementButton.text.text = string.Empty;
		this.mainColourSelectText.text = Strings.Get("ui.interface", "Colours", Strings.Casing.asIs, false, false, false, null);
		this.detailsColourSelectText.text = Strings.Get("ui.interface", "Multiply", Strings.Casing.asIs, false, false, false, null);
		this.grubSelectText.text = Strings.Get("ui.interface", "Dirt", Strings.Casing.asIs, false, false, false, null);
		this.isSetup = true;
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x001C6C97 File Offset: 0x001C4E97
	public void SetPageSize(Vector2 newSize)
	{
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x001C6CBC File Offset: 0x001C4EBC
	public void UpdateButtonsBasedOnFurniture(FurniturePreset furn)
	{
		this.sliderType = MaterialKeyController.SliderPickerType.grub;
		if (furn.isArt)
		{
			this.sliderType = MaterialKeyController.SliderPickerType.artPortrait + (int)furn.artOrientation;
		}
		else if (furn.isPlant)
		{
			this.sliderType = MaterialKeyController.SliderPickerType.plants;
		}
		List<MeshRenderer> list = Enumerable.ToList<MeshRenderer>(furn.prefab.GetComponentsInChildren<MeshRenderer>());
		bool flag = false;
		foreach (MeshRenderer meshRenderer in list)
		{
			if (!meshRenderer.transform.CompareTag("NoMatColour"))
			{
				this.UpdateButtonsBasedOnMaterial(meshRenderer.sharedMaterial, true, this.sliderType);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.UpdateButtonsBasedOnMaterial(null, true, this.sliderType);
		}
		this.UpdatePlacementText();
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x001C6D84 File Offset: 0x001C4F84
	public void UpdateButtonsBasedOnMaterial(Material mat, bool setColour, MaterialKeyController.SliderPickerType sliderType = MaterialKeyController.SliderPickerType.grub)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		Game.Log("Decor: Updating buttons based on " + ((mat != null) ? mat.ToString() : null) + ", set colours: " + setColour.ToString(), 2);
		if (mat != null && mat.HasProperty("_ColourMap"))
		{
			Texture2D texture2D = mat.GetTexture("_ColourMap") as Texture2D;
			if (texture2D != null)
			{
				if (texture2D.isReadable)
				{
					for (int i = 0; i < texture2D.width; i++)
					{
						for (int j = 0; j < texture2D.height; j++)
						{
							Color pixel = texture2D.GetPixel(i, j);
							if (pixel.r > 0.01f)
							{
								flag = true;
							}
							if (pixel.g > 0.01f)
							{
								flag2 = true;
							}
							if (pixel.b > 0.01f)
							{
								flag3 = true;
							}
							if (flag && flag2 && flag3)
							{
								break;
							}
						}
						if (flag && flag2 && flag3)
						{
							break;
						}
					}
					Game.Log(string.Concat(new string[]
					{
						"Decor: Colour map on material ",
						mat.name,
						" uses colours ",
						flag.ToString(),
						" ",
						flag2.ToString(),
						" ",
						flag3.ToString()
					}), 2);
				}
				else
				{
					flag = true;
					flag2 = true;
					flag3 = true;
					Game.Log("Decor: IMPORTANT/TODO: Colour map on material " + mat.name + " is set to unreadable. By default, enabling all 3 editable colours. To make this work as intended, set the material to readable!", 2);
				}
			}
			else
			{
				Game.Log("Decor: Colour map on material " + mat.name + " is missing...", 2);
			}
		}
		else
		{
			Game.Log("Decor: Material " + ((mat != null) ? mat.ToString() : null) + " does not use the colour shader...", 2);
		}
		this.mainColourButton.SetButtonBaseColour(Color.white);
		this.mainColourButton.SetInteractable(true);
		this.mainColourUnused.gameObject.SetActive(false);
		if (setColour && mat != null)
		{
			this.matKey.mainColour = mat.GetColor("_BaseColor");
			this.mainColourButton.SetButtonBaseColour(this.matKey.mainColour);
			Game.Log("Decor: Set Button main Colour: " + this.matKey.mainColour.ToString(), 2);
		}
		if (flag)
		{
			this.colour1Button.SetInteractable(true);
			this.colour1Unused.gameObject.SetActive(false);
			if (setColour)
			{
				this.matKey.colour1 = mat.GetColor("_Color1");
				this.colour1Button.SetButtonBaseColour(this.matKey.colour1);
				Game.Log("Decor: Set Button 1 Colour: " + this.matKey.colour1.ToString(), 2);
			}
		}
		else
		{
			this.matKey.colour1 = Color.white;
			this.colour1Button.SetButtonBaseColour(Color.white);
			this.colour1Button.SetInteractable(false);
			this.colour1Unused.gameObject.SetActive(true);
		}
		if (flag2)
		{
			this.colour2Button.SetInteractable(true);
			this.colour2Unused.gameObject.SetActive(false);
			if (setColour)
			{
				this.matKey.colour2 = mat.GetColor("_Color2");
				this.colour2Button.SetButtonBaseColour(this.matKey.colour2);
				Game.Log("Decor: Set Button 2 Colour: " + this.matKey.colour2.ToString(), 2);
			}
		}
		else
		{
			this.matKey.colour2 = Color.white;
			this.colour2Button.SetButtonBaseColour(Color.white);
			this.colour2Button.SetInteractable(false);
			this.colour2Unused.gameObject.SetActive(true);
		}
		if (flag3)
		{
			this.colour3Button.SetInteractable(true);
			this.colour3Unused.gameObject.SetActive(false);
			if (setColour)
			{
				this.matKey.colour3 = mat.GetColor("_Color3");
				this.colour3Button.SetButtonBaseColour(this.matKey.colour3);
				Game.Log("Decor: Set Button 3 Colour: " + this.matKey.colour3.ToString(), 2);
			}
		}
		else
		{
			this.matKey.colour3 = Color.white;
			this.colour3Button.SetButtonBaseColour(Color.white);
			this.colour3Button.SetInteractable(false);
			this.colour3Unused.gameObject.SetActive(true);
		}
		if (sliderType == MaterialKeyController.SliderPickerType.grub)
		{
			this.grubSelectText.text = Strings.Get("ui.interface", "Dirt", Strings.Casing.asIs, false, false, false, null);
			if (mat != null && mat.HasProperty("_GrubAmount"))
			{
				if (setColour)
				{
					this.grubSlider.slider.SetValueWithoutNotify((float)Mathf.RoundToInt(mat.GetFloat("_GrubAmount") * 10f));
				}
			}
			else
			{
				this.grubSlider.slider.SetValueWithoutNotify(0f);
			}
		}
		else if (sliderType == MaterialKeyController.SliderPickerType.plants)
		{
			this.grubSelectText.text = Strings.Get("ui.interface", "Plants", Strings.Casing.asIs, false, false, false, null);
		}
		else
		{
			this.grubSelectText.text = Strings.Get("ui.interface", "Art", Strings.Casing.asIs, false, false, false, null);
			ArtPreset.ArtOrientation orientation = (ArtPreset.ArtOrientation)(sliderType - 2);
			List<ArtPreset> list = Toolbox.Instance.allArt.FindAll((ArtPreset item) => item.orientationCompatibility.Contains(orientation) && !item.disable);
			this.grubSlider.slider.minValue = 0f;
			this.grubSlider.slider.maxValue = (float)(list.Count - 1);
		}
		if (setColour)
		{
			this.ChangeColourKey();
		}
		this.UpdatePlacementText();
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x001C7318 File Offset: 0x001C5518
	public void SetButtonsToKey(Toolbox.MaterialKey key)
	{
		if (key.mainColour != Color.clear)
		{
			this.matKey.mainColour = key.mainColour;
			this.mainColourButton.SetButtonBaseColour(key.mainColour);
			Game.Log("Decor: Set Button main Colour: " + key.mainColour.ToString(), 2);
		}
		else
		{
			this.mainColourButton.SetButtonBaseColour(Color.white);
		}
		if (key.colour1 != Color.clear)
		{
			this.matKey.colour1 = key.colour1;
			this.colour1Button.SetButtonBaseColour(this.matKey.colour1);
			Game.Log("Decor: Set Button 3 Colour: " + key.colour1.ToString(), 2);
		}
		else
		{
			this.colour1Button.SetButtonBaseColour(Color.white);
		}
		if (key.colour2 != Color.clear)
		{
			this.matKey.colour2 = key.colour2;
			this.colour2Button.SetButtonBaseColour(this.matKey.colour2);
			Game.Log("Decor: Set Button 3 Colour: " + key.colour2.ToString(), 2);
		}
		else
		{
			this.colour2Button.SetButtonBaseColour(Color.white);
		}
		if (key.colour3 != Color.clear)
		{
			this.matKey.colour3 = key.colour3;
			this.colour3Button.SetButtonBaseColour(this.matKey.colour3);
			Game.Log("Decor: Set Button 3 Colour: " + key.colour3.ToString(), 2);
		}
		else
		{
			this.colour3Button.SetButtonBaseColour(Color.white);
		}
		if (this.sliderType == MaterialKeyController.SliderPickerType.grub)
		{
			this.matKey.grubiness = key.grubiness;
			this.grubSlider.slider.SetValueWithoutNotify((float)Mathf.RoundToInt(this.matKey.grubiness * 10f));
		}
		else
		{
			this.matKey.grubiness = 0f;
		}
		this.ChangeColourKey();
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x001C7528 File Offset: 0x001C5728
	public void ColourSelectButton(int val)
	{
		if (this.colourWindow == null)
		{
			this.colourEdit = val;
			this.colourWindow = InterfaceController.Instance.SpawnWindow(null, Evidence.DataKey.name, null, "ColourPicker", false, true, default(Vector2), null, null, null, true);
			this.colourPick = this.colourWindow.GetComponentInChildren<ColourPickerController>(true);
			this.colourPick.OnNewColour += this.OnNewColourSelect;
		}
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x001C759C File Offset: 0x001C579C
	public void OnNewColourSelect(Color newColour)
	{
		Game.Log("Decor: New colour select: " + newColour.ToString(), 2);
		if (this.colourEdit == 0)
		{
			this.matKey.mainColour = newColour;
			this.mainColourButton.SetButtonBaseColour(newColour);
			Game.Log("Decor: Set Button main Colour: " + newColour.ToString(), 2);
		}
		else if (this.colourEdit == 1)
		{
			this.matKey.colour1 = newColour;
			this.colour1Button.SetButtonBaseColour(newColour);
			Game.Log("Decor: Set Button 1 Colour: " + newColour.ToString(), 2);
		}
		else if (this.colourEdit == 2)
		{
			this.matKey.colour2 = newColour;
			this.colour2Button.SetButtonBaseColour(newColour);
			Game.Log("Decor: Set Button 2 Colour: " + newColour.ToString(), 2);
		}
		else if (this.colourEdit == 3)
		{
			this.matKey.colour3 = newColour;
			this.colour3Button.SetButtonBaseColour(newColour);
			Game.Log("Decor: Set Button 3 Colour: " + newColour.ToString(), 2);
		}
		if (this.colourPick != null)
		{
			this.colourPick.OnNewColour -= this.OnNewColourSelect;
		}
		this.colourWindow.CloseWindow(false);
		this.ChangeColourKey();
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x001C76FD File Offset: 0x001C58FD
	public void OnGrubUpdate()
	{
		if (this.sliderType == MaterialKeyController.SliderPickerType.grub)
		{
			this.matKey.grubiness = this.grubSlider.slider.value / 10f;
		}
		else
		{
			MaterialKeyController.SliderPickerType sliderPickerType = this.sliderType;
		}
		this.ChangeColourKey();
	}

	// Token: 0x06002164 RID: 8548 RVA: 0x001C773C File Offset: 0x001C593C
	public void ChangeColourKey()
	{
		if (this.sliderType >= MaterialKeyController.SliderPickerType.artPortrait)
		{
			ArtPreset.ArtOrientation orientation = (ArtPreset.ArtOrientation)(this.sliderType - 2);
			List<ArtPreset> list = Toolbox.Instance.allArt.FindAll((ArtPreset item) => item.orientationCompatibility.Contains(orientation) && !item.disable);
			PlayerApartmentController.Instance.furnPlacement.art = list[Mathf.RoundToInt(this.grubSlider.slider.value)];
			string text = "Decor: Selected art ";
			ArtPreset art = PlayerApartmentController.Instance.furnPlacement.art;
			Game.Log(text + ((art != null) ? art.ToString() : null), 2);
		}
		else
		{
			PlayerApartmentController.Instance.furnPlacement.art = null;
		}
		PlayerApartmentController.Instance.furnPlacement.materialKey = this.matKey;
		PlayerApartmentController.Instance.UpdatePlacementColourKey();
		PlayerApartmentController.Instance.decoratingKey = this.matKey;
		PlayerApartmentController.Instance.UpdateDecorColourKey();
		if (this.OnColourKeyUpdate != null)
		{
			this.OnColourKeyUpdate();
		}
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x001C7838 File Offset: 0x001C5A38
	public void PlacementButton()
	{
		SessionData.Instance.ResumeGame();
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x001C7844 File Offset: 0x001C5A44
	public void UpdatePlacementText()
	{
		if (PlayerApartmentController.Instance.furniturePlacementMode)
		{
			this.placementButton.text.text = Strings.Get("ui.interface", "Preview", Strings.Casing.asIs, false, false, false, null) + " " + Strings.Get("evidence.names", PlayerApartmentController.Instance.furnPlacement.preset.name, Strings.Casing.asIs, false, false, false, null);
			return;
		}
		if (PlayerApartmentController.Instance.decoratingMode)
		{
			this.placementButton.text.text = Strings.Get("ui.interface", "Preview", Strings.Casing.asIs, false, false, false, null) + " " + Strings.Get("evidence.names", PlayerApartmentController.Instance.decoratingMaterial.name, Strings.Casing.asIs, false, false, false, null);
		}
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x001C7907 File Offset: 0x001C5B07
	public void CancelButton()
	{
		this.wcc.window.CloseWindow(false);
		InteractionController.Instance.StartDecorEdit();
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x001C7924 File Offset: 0x001C5B24
	private void OnDestroy()
	{
		if (PlayerApartmentController.Instance.furniturePlacementMode)
		{
			Game.Log("Decor: Material key window is being closed, cancelling decor modes...", 2);
			PlayerApartmentController.Instance.SetFurniturePlacementMode(false, null, null, false, false);
			InteractionController.Instance.StartDecorEdit();
		}
		if (PlayerApartmentController.Instance.decoratingMode)
		{
			Game.Log("Decor: Material key window is being closed, cancelling decor modes...", 2);
			PlayerApartmentController.Instance.SetDecoratingMode(false, null, MaterialGroupPreset.MaterialType.walls, null, null);
			InteractionController.Instance.StartDecorEdit();
		}
		if (this.colourPick != null)
		{
			this.colourPick.OnNewColour -= this.OnNewColourSelect;
		}
		if (this.colourWindow != null)
		{
			this.colourWindow.CloseWindow(false);
		}
	}

	// Token: 0x04002B9D RID: 11165
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002B9E RID: 11166
	public WindowContentController wcc;

	// Token: 0x04002B9F RID: 11167
	public ButtonController placementButton;

	// Token: 0x04002BA0 RID: 11168
	public InfoWindow colourWindow;

	// Token: 0x04002BA1 RID: 11169
	public ColourPickerController colourPick;

	// Token: 0x04002BA2 RID: 11170
	[Header("Colour Select")]
	public TextMeshProUGUI mainColourSelectText;

	// Token: 0x04002BA3 RID: 11171
	public ButtonController mainColourButton;

	// Token: 0x04002BA4 RID: 11172
	public RectTransform mainColourUnused;

	// Token: 0x04002BA5 RID: 11173
	public ButtonController colour1Button;

	// Token: 0x04002BA6 RID: 11174
	public RectTransform colour1Unused;

	// Token: 0x04002BA7 RID: 11175
	public ButtonController colour2Button;

	// Token: 0x04002BA8 RID: 11176
	public RectTransform colour2Unused;

	// Token: 0x04002BA9 RID: 11177
	public ButtonController colour3Button;

	// Token: 0x04002BAA RID: 11178
	public RectTransform colour3Unused;

	// Token: 0x04002BAB RID: 11179
	[Header("Details Select")]
	public TextMeshProUGUI detailsColourSelectText;

	// Token: 0x04002BAC RID: 11180
	[Header("Grub Select")]
	public MaterialKeyController.SliderPickerType sliderType;

	// Token: 0x04002BAD RID: 11181
	public TextMeshProUGUI grubSelectText;

	// Token: 0x04002BAE RID: 11182
	public SliderController grubSlider;

	// Token: 0x04002BAF RID: 11183
	[Header("State")]
	public bool isSetup;

	// Token: 0x04002BB0 RID: 11184
	public Toolbox.MaterialKey matKey = new Toolbox.MaterialKey();

	// Token: 0x04002BB1 RID: 11185
	private int colourEdit;

	// Token: 0x020005ED RID: 1517
	public enum SliderPickerType
	{
		// Token: 0x04002BB4 RID: 11188
		grub,
		// Token: 0x04002BB5 RID: 11189
		plants,
		// Token: 0x04002BB6 RID: 11190
		artPortrait,
		// Token: 0x04002BB7 RID: 11191
		artLandscape,
		// Token: 0x04002BB8 RID: 11192
		artSquare,
		// Token: 0x04002BB9 RID: 11193
		artPoster,
		// Token: 0x04002BBA RID: 11194
		artLitter,
		// Token: 0x04002BBB RID: 11195
		artWallGrimeTop,
		// Token: 0x04002BBC RID: 11196
		artWallGrimeBottom,
		// Token: 0x04002BBD RID: 11197
		artDynamicClue,
		// Token: 0x04002BBE RID: 11198
		artGraffiti
	}

	// Token: 0x020005EE RID: 1518
	// (Invoke) Token: 0x0600216B RID: 8555
	public delegate void ColourKeyUpdate();
}
