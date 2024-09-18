using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020004FE RID: 1278
public class TMP_SelectionController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
{
	// Token: 0x06001B87 RID: 7047 RVA: 0x0018D7FC File Offset: 0x0018B9FC
	private void Awake()
	{
		this.m_TextMeshPro = base.gameObject.GetComponent<TextMeshProUGUI>();
		this.m_Canvas = InterfaceControls.Instance.hudCanvas;
		if (this.m_Canvas.renderMode == null)
		{
			this.m_Camera = null;
		}
		else
		{
			this.m_Camera = this.m_Canvas.worldCamera;
		}
		this.UpdateOriginalFontSettings();
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x0018D857 File Offset: 0x0018BA57
	public void UpdateOriginalFontSettings()
	{
		this.hoverOriginal = this.m_TextMeshPro.color;
		this.originalUseGradient = this.m_TextMeshPro.enableVertexGradient;
		this.originalGradient = this.m_TextMeshPro.colorGradientPreset;
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x0018D88C File Offset: 0x0018BA8C
	private void OnEnable()
	{
		TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<Object>(this.ON_TEXT_CHANGED));
	}

	// Token: 0x06001B8A RID: 7050 RVA: 0x0018D8A4 File Offset: 0x0018BAA4
	private void OnDisable()
	{
		TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<Object>(this.ON_TEXT_CHANGED));
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x0018D8BC File Offset: 0x0018BABC
	private void ON_TEXT_CHANGED(Object obj)
	{
		if (obj == this.m_TextMeshPro)
		{
			this.m_cachedMeshInfoVertexData = this.m_TextMeshPro.textInfo.CopyMeshInfoVertexData();
		}
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x0018D8E2 File Offset: 0x0018BAE2
	private void Start()
	{
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06001B8D RID: 7053 RVA: 0x0018D8F1 File Offset: 0x0018BAF1
	private IEnumerator LateStart()
	{
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < this.m_TextMeshPro.textInfo.linkCount; i++)
		{
			TMP_LinkInfo tmp_LinkInfo = this.m_TextMeshPro.textInfo.linkInfo[i];
			int num = 0;
			if (int.TryParse(tmp_LinkInfo.GetLinkID(), ref num) && !this.markedLinks.ContainsKey(num))
			{
				this.markedLinks.Add(num, false);
			}
		}
		this.UpdateLinkDiscovery();
		yield break;
	}

	// Token: 0x06001B8E RID: 7054 RVA: 0x0018D900 File Offset: 0x0018BB00
	private void LateUpdate()
	{
		if (this.isHoveringObject)
		{
			int num = TMP_TextUtilities.FindIntersectingLink(this.m_TextMeshPro, Input.mousePosition, this.m_Camera);
			if (num == -1 && this.m_selectedLink != -1)
			{
				this.EndHover(this.m_selectedLink);
			}
			if (num != -1 && num != this.m_selectedLink)
			{
				this.NewHover(num);
			}
		}
		if (this.m_TextMeshPro.pageToDisplay != this.lastPage)
		{
			this.RefreshLinkButtons(true);
		}
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x0018D974 File Offset: 0x0018BB74
	public void NewHover(int linkIndex)
	{
		if ((linkIndex == -1 && this.m_selectedLink != -1) || (linkIndex != this.m_selectedLink && this.m_selectedLink != -1))
		{
			this.EndHover(this.m_selectedLink);
		}
		if (linkIndex != -1 && linkIndex != this.m_selectedLink)
		{
			this.m_selectedLink = linkIndex;
			TMP_LinkInfo tmp_LinkInfo = this.m_TextMeshPro.textInfo.linkInfo[linkIndex];
			for (int i = 0; i < tmp_LinkInfo.linkTextLength; i++)
			{
				int num = tmp_LinkInfo.linkTextfirstCharacterIndex + i;
				if (this.m_TextMeshPro.textInfo.characterInfo[num].isVisible)
				{
					int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[num].materialReferenceIndex;
					int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[num].vertexIndex;
					Color32[] colors = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
					colors[vertexIndex] = this.hoverColour;
					colors[vertexIndex + 1] = this.hoverColour;
					colors[vertexIndex + 2] = this.hoverColour;
					colors[vertexIndex + 3] = this.hoverColour;
				}
			}
			this.m_TextMeshPro.UpdateVertexData(255);
		}
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x0018DAD4 File Offset: 0x0018BCD4
	public void EndHover(int linkIndex)
	{
		if (linkIndex == -1 || linkIndex > this.m_TextMeshPro.textInfo.linkCount - 1)
		{
			return;
		}
		TMP_LinkInfo tmp_LinkInfo = this.m_TextMeshPro.textInfo.linkInfo[this.m_selectedLink];
		for (int i = 0; i < tmp_LinkInfo.linkTextLength; i++)
		{
			int num = tmp_LinkInfo.linkTextfirstCharacterIndex + i;
			if (this.m_TextMeshPro.textInfo.characterInfo[num].isVisible)
			{
				int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[num].materialReferenceIndex;
				int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[num].vertexIndex;
				Color32[] colors = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
				colors[vertexIndex] = this.hoverOriginal;
				colors[vertexIndex + 1] = this.hoverOriginal;
				colors[vertexIndex + 2] = this.hoverOriginal;
				colors[vertexIndex + 3] = this.hoverOriginal;
				this.m_TextMeshPro.enableVertexGradient = this.originalUseGradient;
				this.m_TextMeshPro.colorGradientPreset = this.originalGradient;
			}
		}
		this.m_TextMeshPro.UpdateVertexData(255);
		this.m_selectedLink = -1;
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x0018DC38 File Offset: 0x0018BE38
	public void UpdateLinkDiscovery()
	{
		for (int i = 0; i < this.m_TextMeshPro.textInfo.linkCount; i++)
		{
			TMP_LinkInfo tmp_LinkInfo = this.m_TextMeshPro.textInfo.linkInfo[i];
			int num = 0;
			int.TryParse(tmp_LinkInfo.GetLinkID(), ref num);
			"<mark=#" + ColorUtility.ToHtmlStringRGBA(InterfaceControls.Instance.markedLinkColour) + ">";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.m_TextMeshPro.text);
		stringBuilder.Replace("|u", "<u>");
		stringBuilder.Replace("u|", "</u>");
		this.m_TextMeshPro.SetText(stringBuilder);
		this.m_TextMeshPro.ForceMeshUpdate(false, false);
		this.RefreshLinkButtons(true);
	}

	// Token: 0x06001B92 RID: 7058 RVA: 0x0018DD04 File Offset: 0x0018BF04
	public void RefreshLinkButtons(bool updateNavigation = true)
	{
		for (int i = 0; i < this.linkButtons.Count; i++)
		{
			Object.Destroy(this.linkButtons[i].gameObject);
		}
		this.linkButtons = new List<LinkButtonController>();
		for (int j = 0; j < this.m_TextMeshPro.textInfo.linkCount; j++)
		{
			TMP_LinkInfo tmp_LinkInfo = this.m_TextMeshPro.textInfo.linkInfo[j];
			if (this.m_TextMeshPro.pageToDisplay == this.m_TextMeshPro.textInfo.characterInfo[tmp_LinkInfo.linkTextfirstCharacterIndex].pageNumber + 1)
			{
				Vector3 topLeft = this.m_TextMeshPro.textInfo.characterInfo[tmp_LinkInfo.linkTextfirstCharacterIndex].topLeft;
				Vector3 bottomLeft = this.m_TextMeshPro.textInfo.characterInfo[tmp_LinkInfo.linkTextfirstCharacterIndex].bottomLeft;
				Vector3 topRight = this.m_TextMeshPro.textInfo.characterInfo[tmp_LinkInfo.linkTextfirstCharacterIndex].topRight;
				TMP_CharacterInfo[] characterInfo = this.m_TextMeshPro.textInfo.characterInfo;
				int linkTextfirstCharacterIndex = tmp_LinkInfo.linkTextfirstCharacterIndex;
				float num = topLeft.y - bottomLeft.y;
				float width = topRight.x - topLeft.x;
				int k = 1;
				int lineNumber = this.m_TextMeshPro.textInfo.characterInfo[tmp_LinkInfo.linkTextfirstCharacterIndex].lineNumber;
				int num2 = tmp_LinkInfo.linkTextfirstCharacterIndex + k;
				bool flag = false;
				while (k < tmp_LinkInfo.linkTextLength)
				{
					Vector3 topRight2 = this.m_TextMeshPro.textInfo.characterInfo[num2].topRight;
					Vector3 bottomRight = this.m_TextMeshPro.textInfo.characterInfo[num2].bottomRight;
					if (this.m_TextMeshPro.textInfo.characterInfo[num2].lineNumber == lineNumber && this.m_TextMeshPro.pageToDisplay == this.m_TextMeshPro.textInfo.characterInfo[num2].pageNumber + 1)
					{
						flag = true;
						num = Mathf.Max(num, topRight2.y - bottomRight.y);
						width = topRight2.x - topLeft.x;
					}
					else if (flag)
					{
						this.DrawButton(bottomLeft, width, num, tmp_LinkInfo.GetLinkID(), tmp_LinkInfo.GetLinkText());
						flag = true;
						topLeft = this.m_TextMeshPro.textInfo.characterInfo[num2].topLeft;
						bottomLeft = this.m_TextMeshPro.textInfo.characterInfo[num2].bottomLeft;
						num = topLeft.y - bottomLeft.y;
						width = topRight2.x - topLeft.x;
						lineNumber = this.m_TextMeshPro.textInfo.characterInfo[num2].lineNumber;
					}
					if (this.m_TextMeshPro.pageToDisplay != this.m_TextMeshPro.textInfo.characterInfo[num2].pageNumber + 1)
					{
						break;
					}
					k++;
					num2++;
				}
				if (flag)
				{
					this.DrawButton(bottomLeft, width, num, tmp_LinkInfo.GetLinkID(), tmp_LinkInfo.GetLinkText());
				}
			}
		}
		this.lastPage = this.m_TextMeshPro.pageToDisplay;
		if (updateNavigation)
		{
			InfoWindow componentInParent = base.transform.GetComponentInParent<InfoWindow>();
			if (componentInParent != null)
			{
				componentInParent.UpdateControllerNavigationEndOfFrame();
			}
		}
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x0018E074 File Offset: 0x0018C274
	private LinkButtonController DrawButton(Vector3 bottomLeft, float width, float height, string linkID, string buttonName)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.linkButton, base.transform);
		LinkButtonController component = gameObject.GetComponent<LinkButtonController>();
		component.Setup(linkID, this);
		component.rect.localPosition = bottomLeft - new Vector3(0f, 4f);
		component.rect.sizeDelta = new Vector2(width, height + 8f);
		gameObject.name = buttonName;
		this.linkButtons.Add(component);
		return component;
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x0018E0F2 File Offset: 0x0018C2F2
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.isHoveringObject = true;
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x0018E0FB File Offset: 0x0018C2FB
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		this.isHoveringObject = false;
		this.EndHover(this.m_selectedLink);
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x0018E124 File Offset: 0x0018C324
	public void OnPointerClick(PointerEventData eventData)
	{
		int num = TMP_TextUtilities.FindIntersectingLink(this.m_TextMeshPro, Input.mousePosition, this.m_Camera);
		if (num != -1)
		{
			TMP_LinkInfo tmp_LinkInfo = this.m_TextMeshPro.textInfo.linkInfo[num];
			int num2 = 0;
			int.TryParse(tmp_LinkInfo.GetLinkID(), ref num2);
			Strings.LinkData linkData = null;
			if (Strings.Instance.linkIDReference.TryGetValue(num2, ref linkData))
			{
				linkData.OnLink();
			}
		}
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x00002265 File Offset: 0x00000465
	public void OnPointerUp(PointerEventData eventData)
	{
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x0018E190 File Offset: 0x0018C390
	private void RestoreCachedVertexAttributes(int index)
	{
		if (index == -1 || index > this.m_TextMeshPro.textInfo.characterCount - 1)
		{
			return;
		}
		int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[index].materialReferenceIndex;
		int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[index].vertexIndex;
		Vector3[] vertices = this.m_cachedMeshInfoVertexData[materialReferenceIndex].vertices;
		Vector3[] vertices2 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].vertices;
		vertices2[vertexIndex] = vertices[vertexIndex];
		vertices2[vertexIndex + 1] = vertices[vertexIndex + 1];
		vertices2[vertexIndex + 2] = vertices[vertexIndex + 2];
		vertices2[vertexIndex + 3] = vertices[vertexIndex + 3];
		Color32[] colors = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
		Color32[] colors2 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].colors32;
		colors[vertexIndex] = colors2[vertexIndex];
		colors[vertexIndex + 1] = colors2[vertexIndex + 1];
		colors[vertexIndex + 2] = colors2[vertexIndex + 2];
		colors[vertexIndex + 3] = colors2[vertexIndex + 3];
		Vector2[] uvs = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs0;
		Vector2[] uvs2 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs0;
		uvs2[vertexIndex] = uvs[vertexIndex];
		uvs2[vertexIndex + 1] = uvs[vertexIndex + 1];
		uvs2[vertexIndex + 2] = uvs[vertexIndex + 2];
		uvs2[vertexIndex + 3] = uvs[vertexIndex + 3];
		Vector2[] uvs3 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs2;
		Vector2[] uvs4 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs2;
		uvs4[vertexIndex] = uvs3[vertexIndex];
		uvs4[vertexIndex + 1] = uvs3[vertexIndex + 1];
		uvs4[vertexIndex + 2] = uvs3[vertexIndex + 2];
		uvs4[vertexIndex + 3] = uvs3[vertexIndex + 3];
		int num = (vertices.Length / 4 - 1) * 4;
		vertices2[num] = vertices[num];
		vertices2[num + 1] = vertices[num + 1];
		vertices2[num + 2] = vertices[num + 2];
		vertices2[num + 3] = vertices[num + 3];
		colors2 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].colors32;
		Color32[] colors3 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
		colors3[num] = colors2[num];
		colors3[num + 1] = colors2[num + 1];
		colors3[num + 2] = colors2[num + 2];
		colors3[num + 3] = colors2[num + 3];
		uvs = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs0;
		Vector2[] uvs5 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs0;
		uvs5[num] = uvs[num];
		uvs5[num + 1] = uvs[num + 1];
		uvs5[num + 2] = uvs[num + 2];
		uvs5[num + 3] = uvs[num + 3];
		uvs3 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs2;
		Vector2[] uvs6 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs2;
		uvs6[num] = uvs3[num];
		uvs6[num + 1] = uvs3[num + 1];
		uvs6[num + 2] = uvs3[num + 2];
		uvs6[num + 3] = uvs3[num + 3];
		this.m_TextMeshPro.UpdateVertexData(255);
	}

	// Token: 0x0400243E RID: 9278
	public TextMeshProUGUI m_TextMeshPro;

	// Token: 0x0400243F RID: 9279
	private Canvas m_Canvas;

	// Token: 0x04002440 RID: 9280
	private Camera m_Camera;

	// Token: 0x04002441 RID: 9281
	private bool isHoveringObject;

	// Token: 0x04002442 RID: 9282
	private int m_selectedLink = -1;

	// Token: 0x04002443 RID: 9283
	private Matrix4x4 m_matrix;

	// Token: 0x04002444 RID: 9284
	private TMP_MeshInfo[] m_cachedMeshInfoVertexData;

	// Token: 0x04002445 RID: 9285
	public List<LinkButtonController> linkButtons = new List<LinkButtonController>();

	// Token: 0x04002446 RID: 9286
	public Color hoverColour = Color.red;

	// Token: 0x04002447 RID: 9287
	private Color hoverOriginal = Color.black;

	// Token: 0x04002448 RID: 9288
	public Color highlightColour = Color.yellow;

	// Token: 0x04002449 RID: 9289
	private bool originalUseGradient;

	// Token: 0x0400244A RID: 9290
	private TMP_ColorGradient originalGradient;

	// Token: 0x0400244B RID: 9291
	public Dictionary<int, bool> markedLinks = new Dictionary<int, bool>();

	// Token: 0x0400244C RID: 9292
	private int lastPage;
}
