using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005E0 RID: 1504
public class ColourPickerController : MonoBehaviour
{
	// Token: 0x14000047 RID: 71
	// (add) Token: 0x06002115 RID: 8469 RVA: 0x001C4770 File Offset: 0x001C2970
	// (remove) Token: 0x06002116 RID: 8470 RVA: 0x001C47A8 File Offset: 0x001C29A8
	public event ColourPickerController.NewColour OnNewColour;

	// Token: 0x06002117 RID: 8471 RVA: 0x001C47DD File Offset: 0x001C29DD
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.wcc = newContentController;
		this.SetPageSize(new Vector2(338f, 300f));
		this.UpdateListDisplay();
		this.isSetup = true;
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x001C4819 File Offset: 0x001C2A19
	public void SetPageSize(Vector2 newSize)
	{
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x06002119 RID: 8473 RVA: 0x001C4840 File Offset: 0x001C2A40
	public void UpdateListDisplay()
	{
		if (InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, 0f);
		List<Color> list = new List<Color>(PlayerApartmentController.Instance.swatches);
		for (int i = 0; i < this.spawnedEntries.Count; i++)
		{
			SwatchController swatchController = this.spawnedEntries[i];
			if (swatchController == null)
			{
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
			else if (list.Contains(swatchController.background.color))
			{
				list.Remove(swatchController.background.color);
				swatchController.VisualUpdate();
			}
			else
			{
				Object.Destroy(swatchController.gameObject);
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
		}
		foreach (Color newColor in list)
		{
			SwatchController component = Object.Instantiate<GameObject>(this.swatchPrefab, this.spawnParent).GetComponent<SwatchController>();
			component.Setup(newColor, this);
			this.spawnedEntries.Add(component);
		}
		for (int j = 0; j < this.spawnedEntries.Count; j++)
		{
			this.spawnedEntries[j].transform.SetAsLastSibling();
		}
		this.entryParent.sizeDelta = new Vector2(this.entryParent.sizeDelta.x, (float)this.spawnedEntries.Count / 6f * 48f + 16f);
		this.SetPageSize(new Vector2(this.rect.sizeDelta.x, this.entryParent.sizeDelta.y + 34f));
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x001C4A38 File Offset: 0x001C2C38
	public void OnPickNewColour(SwatchController swatch)
	{
		this.selectedColor = swatch.baseColour;
		if (this.OnNewColour != null)
		{
			this.OnNewColour(this.selectedColor);
		}
		this.wcc.window.CloseWindow(false);
	}

	// Token: 0x04002B4A RID: 11082
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002B4B RID: 11083
	public WindowContentController wcc;

	// Token: 0x04002B4C RID: 11084
	public RectTransform entryParent;

	// Token: 0x04002B4D RID: 11085
	public RectTransform spawnParent;

	// Token: 0x04002B4E RID: 11086
	public GameObject swatchPrefab;

	// Token: 0x04002B4F RID: 11087
	[Header("State")]
	public bool isSetup;

	// Token: 0x04002B50 RID: 11088
	public Color selectedColor = Color.white;

	// Token: 0x04002B51 RID: 11089
	public List<SwatchController> spawnedEntries = new List<SwatchController>();

	// Token: 0x020005E1 RID: 1505
	// (Invoke) Token: 0x0600211D RID: 8477
	public delegate void NewColour(Color newColour);
}
