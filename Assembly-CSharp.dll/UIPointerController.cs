using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000601 RID: 1537
public class UIPointerController : MonoBehaviour
{
	// Token: 0x060021E3 RID: 8675 RVA: 0x001CB72C File Offset: 0x001C992C
	public void Setup(Objective newObj)
	{
		if (this.rend == null)
		{
			this.rend = base.gameObject.GetComponent<CanvasRenderer>();
		}
		this.objective = newObj;
		this.img.sprite = this.objective.sprite;
		this.rend.SetAlpha(0f);
		Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(Player.Instance.lookAtThisTransform.position);
		PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref this.node);
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x001CB7BC File Offset: 0x001C99BC
	private void Update()
	{
		if (this.fadeIn < 1f)
		{
			this.fadeIn += Time.deltaTime;
			this.fadeIn = Mathf.Clamp01(this.fadeIn);
		}
		Vector3 pointerPosition = this.objective.queueElement.pointerPosition;
		this.distance = Vector3.Distance(Player.Instance.lookAtThisTransform.position, pointerPosition);
		float num = 1f - Mathf.Clamp01(this.distance / 20f);
		float num2 = 0f;
		if (this.node != null && Player.Instance.currentRoom == this.node.room)
		{
			num2 = 1f;
		}
		this.alpha = num * 0.4f + num2 * 0.4f + 0.2f;
		this.rend.SetAlpha(this.fadeIn * this.alpha);
		float num3 = Mathf.Lerp(InterfaceControls.Instance.uiPointerDistanceRange.x, InterfaceControls.Instance.uiPointerDistanceRange.y, num);
		this.rect.sizeDelta = new Vector2(num3, num3);
		Vector3 zero = Vector3.zero;
		if (Mathf.Abs(Vector3.SignedAngle(pointerPosition - Player.Instance.transform.position, Player.Instance.transform.forward, Vector3.up)) < 75f)
		{
			this.img.enabled = true;
			Vector3 vector = CameraController.Instance.cam.WorldToScreenPoint(pointerPosition);
			Vector2 vector2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(InterfaceController.Instance.firstPersonUI, vector, null, ref vector2);
			this.rect.localPosition = vector2;
			return;
		}
		this.img.enabled = false;
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000516BA File Offset: 0x0004F8BA
	public void Remove()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04002C67 RID: 11367
	public RectTransform rect;

	// Token: 0x04002C68 RID: 11368
	public CanvasRenderer rend;

	// Token: 0x04002C69 RID: 11369
	public float alpha;

	// Token: 0x04002C6A RID: 11370
	public float fadeIn;

	// Token: 0x04002C6B RID: 11371
	public Objective objective;

	// Token: 0x04002C6C RID: 11372
	public Image img;

	// Token: 0x04002C6D RID: 11373
	public float distance;

	// Token: 0x04002C6E RID: 11374
	public NewNode node;
}
