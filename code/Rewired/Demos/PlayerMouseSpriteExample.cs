using System;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008B7 RID: 2231
	[AddComponentMenu("")]
	public class PlayerMouseSpriteExample : MonoBehaviour
	{
		// Token: 0x06002FC4 RID: 12228 RVA: 0x00211C9C File Offset: 0x0020FE9C
		private void Awake()
		{
			this.pointer = Object.Instantiate<GameObject>(this.pointerPrefab);
			this.pointer.transform.localScale = new Vector3(this.spriteScale, this.spriteScale, this.spriteScale);
			if (this.hideHardwarePointer)
			{
				Cursor.visible = false;
			}
			this.mouse = PlayerMouse.Factory.Create();
			this.mouse.playerId = this.playerId;
			this.mouse.xAxis.actionName = this.horizontalAction;
			this.mouse.yAxis.actionName = this.verticalAction;
			this.mouse.wheel.yAxis.actionName = this.wheelAction;
			this.mouse.leftButton.actionName = this.leftButtonAction;
			this.mouse.rightButton.actionName = this.rightButtonAction;
			this.mouse.middleButton.actionName = this.middleButtonAction;
			this.mouse.pointerSpeed = 1f;
			this.mouse.wheel.yAxis.repeatRate = 5f;
			this.mouse.screenPosition = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
			this.mouse.ScreenPositionChangedEvent += new Action<Vector2>(this.OnScreenPositionChanged);
			this.OnScreenPositionChanged(this.mouse.screenPosition);
		}

		// Token: 0x06002FC5 RID: 12229 RVA: 0x00211E10 File Offset: 0x00210010
		private void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			this.pointer.transform.Rotate(Vector3.forward, this.mouse.wheel.yAxis.value * 20f);
			if (this.mouse.leftButton.justPressed)
			{
				this.CreateClickEffect(new Color(0f, 1f, 0f, 1f));
			}
			if (this.mouse.rightButton.justPressed)
			{
				this.CreateClickEffect(new Color(1f, 0f, 0f, 1f));
			}
			if (this.mouse.middleButton.justPressed)
			{
				this.CreateClickEffect(new Color(1f, 1f, 0f, 1f));
			}
		}

		// Token: 0x06002FC6 RID: 12230 RVA: 0x00211EE8 File Offset: 0x002100E8
		private void OnDestroy()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			this.mouse.ScreenPositionChangedEvent -= new Action<Vector2>(this.OnScreenPositionChanged);
		}

		// Token: 0x06002FC7 RID: 12231 RVA: 0x00211F0C File Offset: 0x0021010C
		private void CreateClickEffect(Color color)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.clickEffectPrefab);
			gameObject.transform.localScale = new Vector3(this.spriteScale, this.spriteScale, this.spriteScale);
			gameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(this.mouse.screenPosition.x, this.mouse.screenPosition.y, this.distanceFromCamera));
			gameObject.GetComponentInChildren<SpriteRenderer>().color = color;
			Object.Destroy(gameObject, 0.5f);
		}

		// Token: 0x06002FC8 RID: 12232 RVA: 0x00211F9C File Offset: 0x0021019C
		private void OnScreenPositionChanged(Vector2 position)
		{
			Vector3 position2 = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, this.distanceFromCamera));
			this.pointer.transform.position = position2;
		}

		// Token: 0x04004A86 RID: 19078
		[Tooltip("The Player that will control the mouse")]
		public int playerId;

		// Token: 0x04004A87 RID: 19079
		[Tooltip("The Rewired Action used for the mouse horizontal axis.")]
		public string horizontalAction = "MouseX";

		// Token: 0x04004A88 RID: 19080
		[Tooltip("The Rewired Action used for the mouse vertical axis.")]
		public string verticalAction = "MouseY";

		// Token: 0x04004A89 RID: 19081
		[Tooltip("The Rewired Action used for the mouse wheel axis.")]
		public string wheelAction = "MouseWheel";

		// Token: 0x04004A8A RID: 19082
		[Tooltip("The Rewired Action used for the mouse left button.")]
		public string leftButtonAction = "MouseLeftButton";

		// Token: 0x04004A8B RID: 19083
		[Tooltip("The Rewired Action used for the mouse right button.")]
		public string rightButtonAction = "MouseRightButton";

		// Token: 0x04004A8C RID: 19084
		[Tooltip("The Rewired Action used for the mouse middle button.")]
		public string middleButtonAction = "MouseMiddleButton";

		// Token: 0x04004A8D RID: 19085
		[Tooltip("The distance from the camera that the pointer will be drawn.")]
		public float distanceFromCamera = 1f;

		// Token: 0x04004A8E RID: 19086
		[Tooltip("The scale of the sprite pointer.")]
		public float spriteScale = 0.05f;

		// Token: 0x04004A8F RID: 19087
		[Tooltip("The pointer prefab.")]
		public GameObject pointerPrefab;

		// Token: 0x04004A90 RID: 19088
		[Tooltip("The click effect prefab.")]
		public GameObject clickEffectPrefab;

		// Token: 0x04004A91 RID: 19089
		[Tooltip("Should the hardware pointer be hidden?")]
		public bool hideHardwarePointer = true;

		// Token: 0x04004A92 RID: 19090
		[NonSerialized]
		private GameObject pointer;

		// Token: 0x04004A93 RID: 19091
		[NonSerialized]
		private PlayerMouse mouse;
	}
}
