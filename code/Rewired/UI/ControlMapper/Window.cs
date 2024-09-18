using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x0200089C RID: 2204
	[AddComponentMenu("")]
	[RequireComponent(typeof(CanvasGroup))]
	public class Window : MonoBehaviour
	{
		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06002EBD RID: 11965 RVA: 0x0020D52F File Offset: 0x0020B72F
		public bool hasFocus
		{
			get
			{
				return this._isFocusedCallback != null && this._isFocusedCallback.Invoke(this._id);
			}
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06002EBE RID: 11966 RVA: 0x0020D54C File Offset: 0x0020B74C
		public int id
		{
			get
			{
				return this._id;
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06002EBF RID: 11967 RVA: 0x0020D554 File Offset: 0x0020B754
		public RectTransform rectTransform
		{
			get
			{
				if (this._rectTransform == null)
				{
					this._rectTransform = base.gameObject.GetComponent<RectTransform>();
				}
				return this._rectTransform;
			}
		}

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06002EC0 RID: 11968 RVA: 0x0020D57B File Offset: 0x0020B77B
		public TMP_Text titleText
		{
			get
			{
				return this._titleText;
			}
		}

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06002EC1 RID: 11969 RVA: 0x0020D583 File Offset: 0x0020B783
		public List<TMP_Text> contentText
		{
			get
			{
				return this._contentText;
			}
		}

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06002EC2 RID: 11970 RVA: 0x0020D58B File Offset: 0x0020B78B
		// (set) Token: 0x06002EC3 RID: 11971 RVA: 0x0020D593 File Offset: 0x0020B793
		public GameObject defaultUIElement
		{
			get
			{
				return this._defaultUIElement;
			}
			set
			{
				this._defaultUIElement = value;
			}
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06002EC4 RID: 11972 RVA: 0x0020D59C File Offset: 0x0020B79C
		// (set) Token: 0x06002EC5 RID: 11973 RVA: 0x0020D5A4 File Offset: 0x0020B7A4
		public Action<int> updateCallback
		{
			get
			{
				return this._updateCallback;
			}
			set
			{
				this._updateCallback = value;
			}
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06002EC6 RID: 11974 RVA: 0x0020D5AD File Offset: 0x0020B7AD
		public Window.Timer timer
		{
			get
			{
				return this._timer;
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06002EC7 RID: 11975 RVA: 0x0020D5B5 File Offset: 0x0020B7B5
		// (set) Token: 0x06002EC8 RID: 11976 RVA: 0x0020D5C8 File Offset: 0x0020B7C8
		public int width
		{
			get
			{
				return (int)this.rectTransform.sizeDelta.x;
			}
			set
			{
				Vector2 sizeDelta = this.rectTransform.sizeDelta;
				sizeDelta.x = (float)value;
				this.rectTransform.sizeDelta = sizeDelta;
			}
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06002EC9 RID: 11977 RVA: 0x0020D5F6 File Offset: 0x0020B7F6
		// (set) Token: 0x06002ECA RID: 11978 RVA: 0x0020D60C File Offset: 0x0020B80C
		public int height
		{
			get
			{
				return (int)this.rectTransform.sizeDelta.y;
			}
			set
			{
				Vector2 sizeDelta = this.rectTransform.sizeDelta;
				sizeDelta.y = (float)value;
				this.rectTransform.sizeDelta = sizeDelta;
			}
		}

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06002ECB RID: 11979 RVA: 0x0020D63A File Offset: 0x0020B83A
		protected bool initialized
		{
			get
			{
				return this._initialized;
			}
		}

		// Token: 0x06002ECC RID: 11980 RVA: 0x0020D642 File Offset: 0x0020B842
		private void OnEnable()
		{
			base.StartCoroutine("OnEnableAsync");
		}

		// Token: 0x06002ECD RID: 11981 RVA: 0x0020D650 File Offset: 0x0020B850
		protected virtual void Update()
		{
			if (!this._initialized)
			{
				return;
			}
			if (!this.hasFocus)
			{
				return;
			}
			this.CheckUISelection();
			if (this._updateCallback != null)
			{
				this._updateCallback.Invoke(this._id);
			}
		}

		// Token: 0x06002ECE RID: 11982 RVA: 0x0020D684 File Offset: 0x0020B884
		public virtual void Initialize(int id, Func<int, bool> isFocusedCallback)
		{
			if (this._initialized)
			{
				Debug.LogError("Window is already initialized!");
				return;
			}
			this._id = id;
			this._isFocusedCallback = isFocusedCallback;
			this._timer = new Window.Timer();
			this._contentText = new List<TMP_Text>();
			this._canvasGroup = base.GetComponent<CanvasGroup>();
			this._initialized = true;
		}

		// Token: 0x06002ECF RID: 11983 RVA: 0x0020D6DB File Offset: 0x0020B8DB
		public void SetSize(int width, int height)
		{
			this.rectTransform.sizeDelta = new Vector2((float)width, (float)height);
		}

		// Token: 0x06002ED0 RID: 11984 RVA: 0x0020D6F1 File Offset: 0x0020B8F1
		public void CreateTitleText(GameObject prefab, Vector2 offset)
		{
			this.CreateText(prefab, ref this._titleText, "Title Text", UIPivot.TopCenter, UIAnchor.TopHStretch, offset);
		}

		// Token: 0x06002ED1 RID: 11985 RVA: 0x0020D710 File Offset: 0x0020B910
		public void CreateTitleText(GameObject prefab, Vector2 offset, string text)
		{
			this.CreateTitleText(prefab, offset);
			this.SetTitleText(text);
		}

		// Token: 0x06002ED2 RID: 11986 RVA: 0x0020D724 File Offset: 0x0020B924
		public void AddContentText(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset)
		{
			TMP_Text tmp_Text = null;
			this.CreateText(prefab, ref tmp_Text, "Content Text", pivot, anchor, offset);
			this._contentText.Add(tmp_Text);
		}

		// Token: 0x06002ED3 RID: 11987 RVA: 0x0020D751 File Offset: 0x0020B951
		public void AddContentText(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset, string text)
		{
			this.AddContentText(prefab, pivot, anchor, offset);
			this.SetContentText(text, this._contentText.Count - 1);
		}

		// Token: 0x06002ED4 RID: 11988 RVA: 0x0020D773 File Offset: 0x0020B973
		public void AddContentImage(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset)
		{
			this.CreateImage(prefab, "Image", pivot, anchor, offset);
		}

		// Token: 0x06002ED5 RID: 11989 RVA: 0x0020D785 File Offset: 0x0020B985
		public void AddContentImage(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset, string text)
		{
			this.AddContentImage(prefab, pivot, anchor, offset);
		}

		// Token: 0x06002ED6 RID: 11990 RVA: 0x0020D794 File Offset: 0x0020B994
		public void CreateButton(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset, string buttonText, UnityAction confirmCallback, UnityAction cancelCallback, bool setDefault)
		{
			if (prefab == null)
			{
				return;
			}
			ButtonInfo buttonInfo;
			GameObject gameObject = this.CreateButton(prefab, "Button", anchor, pivot, offset, out buttonInfo);
			if (gameObject == null)
			{
				return;
			}
			Button component = gameObject.GetComponent<Button>();
			if (confirmCallback != null)
			{
				component.onClick.AddListener(confirmCallback);
			}
			CustomButton customButton = component as CustomButton;
			if (cancelCallback != null && customButton != null)
			{
				customButton.CancelEvent += cancelCallback;
			}
			if (buttonInfo.text != null)
			{
				buttonInfo.text.text = buttonText;
			}
			if (setDefault)
			{
				this._defaultUIElement = gameObject;
			}
		}

		// Token: 0x06002ED7 RID: 11991 RVA: 0x0020D822 File Offset: 0x0020BA22
		public string GetTitleText(string text)
		{
			if (this._titleText == null)
			{
				return string.Empty;
			}
			return this._titleText.text;
		}

		// Token: 0x06002ED8 RID: 11992 RVA: 0x0020D843 File Offset: 0x0020BA43
		public void SetTitleText(string text)
		{
			if (this._titleText == null)
			{
				return;
			}
			this._titleText.text = text;
		}

		// Token: 0x06002ED9 RID: 11993 RVA: 0x0020D860 File Offset: 0x0020BA60
		public string GetContentText(int index)
		{
			if (this._contentText == null || this._contentText.Count <= index || this._contentText[index] == null)
			{
				return string.Empty;
			}
			return this._contentText[index].text;
		}

		// Token: 0x06002EDA RID: 11994 RVA: 0x0020D8B0 File Offset: 0x0020BAB0
		public float GetContentTextHeight(int index)
		{
			if (this._contentText == null || this._contentText.Count <= index || this._contentText[index] == null)
			{
				return 0f;
			}
			return this._contentText[index].rectTransform.sizeDelta.y;
		}

		// Token: 0x06002EDB RID: 11995 RVA: 0x0020D908 File Offset: 0x0020BB08
		public void SetContentText(string text, int index)
		{
			if (this._contentText == null || this._contentText.Count <= index || this._contentText[index] == null)
			{
				return;
			}
			this._contentText[index].text = text;
		}

		// Token: 0x06002EDC RID: 11996 RVA: 0x0020D947 File Offset: 0x0020BB47
		public void SetUpdateCallback(Action<int> callback)
		{
			this.updateCallback = callback;
		}

		// Token: 0x06002EDD RID: 11997 RVA: 0x0020D950 File Offset: 0x0020BB50
		public virtual void TakeInputFocus()
		{
			if (EventSystem.current == null)
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(this._defaultUIElement);
			this.Enable();
		}

		// Token: 0x06002EDE RID: 11998 RVA: 0x0020D976 File Offset: 0x0020BB76
		public virtual void Enable()
		{
			this._canvasGroup.interactable = true;
		}

		// Token: 0x06002EDF RID: 11999 RVA: 0x0020D984 File Offset: 0x0020BB84
		public virtual void Disable()
		{
			this._canvasGroup.interactable = false;
		}

		// Token: 0x06002EE0 RID: 12000 RVA: 0x0020D992 File Offset: 0x0020BB92
		public virtual void Cancel()
		{
			if (!this.initialized)
			{
				return;
			}
			if (this.cancelCallback != null)
			{
				this.cancelCallback.Invoke();
			}
		}

		// Token: 0x06002EE1 RID: 12001 RVA: 0x0020D9B0 File Offset: 0x0020BBB0
		private void CreateText(GameObject prefab, ref TMP_Text textComponent, string name, UIPivot pivot, UIAnchor anchor, Vector2 offset)
		{
			if (prefab == null || this.content == null)
			{
				return;
			}
			if (textComponent != null)
			{
				Debug.LogError("Window already has " + name + "!");
				return;
			}
			GameObject gameObject = UITools.InstantiateGUIObject<TMP_Text>(prefab, this.content.transform, name, pivot, anchor.min, anchor.max, offset);
			if (gameObject == null)
			{
				return;
			}
			textComponent = gameObject.GetComponent<TMP_Text>();
		}

		// Token: 0x06002EE2 RID: 12002 RVA: 0x0020DA34 File Offset: 0x0020BC34
		private void CreateImage(GameObject prefab, string name, UIPivot pivot, UIAnchor anchor, Vector2 offset)
		{
			if (prefab == null || this.content == null)
			{
				return;
			}
			UITools.InstantiateGUIObject<Image>(prefab, this.content.transform, name, pivot, anchor.min, anchor.max, offset);
		}

		// Token: 0x06002EE3 RID: 12003 RVA: 0x0020DA84 File Offset: 0x0020BC84
		private GameObject CreateButton(GameObject prefab, string name, UIAnchor anchor, UIPivot pivot, Vector2 offset, out ButtonInfo buttonInfo)
		{
			buttonInfo = null;
			if (prefab == null)
			{
				return null;
			}
			GameObject gameObject = UITools.InstantiateGUIObject<ButtonInfo>(prefab, this.content.transform, name, pivot, anchor.min, anchor.max, offset);
			if (gameObject == null)
			{
				return null;
			}
			buttonInfo = gameObject.GetComponent<ButtonInfo>();
			if (gameObject.GetComponent<Button>() == null)
			{
				Debug.Log("Button prefab is missing Button component!");
				return null;
			}
			if (buttonInfo == null)
			{
				Debug.Log("Button prefab is missing ButtonInfo component!");
				return null;
			}
			return gameObject;
		}

		// Token: 0x06002EE4 RID: 12004 RVA: 0x0020DB0E File Offset: 0x0020BD0E
		private IEnumerator OnEnableAsync()
		{
			yield return 1;
			if (EventSystem.current == null)
			{
				yield break;
			}
			if (this.defaultUIElement != null)
			{
				EventSystem.current.SetSelectedGameObject(this.defaultUIElement);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
			yield break;
		}

		// Token: 0x06002EE5 RID: 12005 RVA: 0x0020DB20 File Offset: 0x0020BD20
		private void CheckUISelection()
		{
			if (!this.hasFocus)
			{
				return;
			}
			if (EventSystem.current == null)
			{
				return;
			}
			if (EventSystem.current.currentSelectedGameObject == null)
			{
				this.RestoreDefaultOrLastUISelection();
			}
			this.lastUISelection = EventSystem.current.currentSelectedGameObject;
		}

		// Token: 0x06002EE6 RID: 12006 RVA: 0x0020DB6C File Offset: 0x0020BD6C
		private void RestoreDefaultOrLastUISelection()
		{
			if (!this.hasFocus)
			{
				return;
			}
			if (this.lastUISelection == null || !this.lastUISelection.activeInHierarchy)
			{
				this.SetUISelection(this._defaultUIElement);
				return;
			}
			this.SetUISelection(this.lastUISelection);
		}

		// Token: 0x06002EE7 RID: 12007 RVA: 0x00205E9E File Offset: 0x0020409E
		private void SetUISelection(GameObject selection)
		{
			if (EventSystem.current == null)
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(selection);
		}

		// Token: 0x040049DE RID: 18910
		public Image backgroundImage;

		// Token: 0x040049DF RID: 18911
		public GameObject content;

		// Token: 0x040049E0 RID: 18912
		private bool _initialized;

		// Token: 0x040049E1 RID: 18913
		private int _id = -1;

		// Token: 0x040049E2 RID: 18914
		private RectTransform _rectTransform;

		// Token: 0x040049E3 RID: 18915
		private TMP_Text _titleText;

		// Token: 0x040049E4 RID: 18916
		private List<TMP_Text> _contentText;

		// Token: 0x040049E5 RID: 18917
		private GameObject _defaultUIElement;

		// Token: 0x040049E6 RID: 18918
		private Action<int> _updateCallback;

		// Token: 0x040049E7 RID: 18919
		private Func<int, bool> _isFocusedCallback;

		// Token: 0x040049E8 RID: 18920
		private Window.Timer _timer;

		// Token: 0x040049E9 RID: 18921
		private CanvasGroup _canvasGroup;

		// Token: 0x040049EA RID: 18922
		public UnityAction cancelCallback;

		// Token: 0x040049EB RID: 18923
		private GameObject lastUISelection;

		// Token: 0x0200089D RID: 2205
		public class Timer
		{
			// Token: 0x170004D8 RID: 1240
			// (get) Token: 0x06002EE9 RID: 12009 RVA: 0x0020DBBA File Offset: 0x0020BDBA
			public bool started
			{
				get
				{
					return this._started;
				}
			}

			// Token: 0x170004D9 RID: 1241
			// (get) Token: 0x06002EEA RID: 12010 RVA: 0x0020DBC2 File Offset: 0x0020BDC2
			public bool finished
			{
				get
				{
					if (!this.started)
					{
						return false;
					}
					if (Time.realtimeSinceStartup < this.end)
					{
						return false;
					}
					this._started = false;
					return true;
				}
			}

			// Token: 0x170004DA RID: 1242
			// (get) Token: 0x06002EEB RID: 12011 RVA: 0x0020DBE5 File Offset: 0x0020BDE5
			public float remaining
			{
				get
				{
					if (!this._started)
					{
						return 0f;
					}
					return this.end - Time.realtimeSinceStartup;
				}
			}

			// Token: 0x06002EED RID: 12013 RVA: 0x0020DC01 File Offset: 0x0020BE01
			public void Start(float length)
			{
				this.end = Time.realtimeSinceStartup + length;
				this._started = true;
			}

			// Token: 0x06002EEE RID: 12014 RVA: 0x0020DC17 File Offset: 0x0020BE17
			public void Stop()
			{
				this._started = false;
			}

			// Token: 0x040049EC RID: 18924
			private bool _started;

			// Token: 0x040049ED RID: 18925
			private float end;
		}
	}
}
