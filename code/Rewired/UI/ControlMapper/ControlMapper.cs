using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x0200084D RID: 2125
	[AddComponentMenu("")]
	public class ControlMapper : MonoBehaviour
	{
		// Token: 0x14000063 RID: 99
		// (add) Token: 0x06002A82 RID: 10882 RVA: 0x002008CC File Offset: 0x001FEACC
		// (remove) Token: 0x06002A83 RID: 10883 RVA: 0x002008E5 File Offset: 0x001FEAE5
		public event Action ScreenClosedEvent
		{
			add
			{
				this._ScreenClosedEvent = (Action)Delegate.Combine(this._ScreenClosedEvent, value);
			}
			remove
			{
				this._ScreenClosedEvent = (Action)Delegate.Remove(this._ScreenClosedEvent, value);
			}
		}

		// Token: 0x14000064 RID: 100
		// (add) Token: 0x06002A84 RID: 10884 RVA: 0x002008FE File Offset: 0x001FEAFE
		// (remove) Token: 0x06002A85 RID: 10885 RVA: 0x00200917 File Offset: 0x001FEB17
		public event Action ScreenOpenedEvent
		{
			add
			{
				this._ScreenOpenedEvent = (Action)Delegate.Combine(this._ScreenOpenedEvent, value);
			}
			remove
			{
				this._ScreenOpenedEvent = (Action)Delegate.Remove(this._ScreenOpenedEvent, value);
			}
		}

		// Token: 0x14000065 RID: 101
		// (add) Token: 0x06002A86 RID: 10886 RVA: 0x00200930 File Offset: 0x001FEB30
		// (remove) Token: 0x06002A87 RID: 10887 RVA: 0x00200949 File Offset: 0x001FEB49
		public event Action PopupWindowClosedEvent
		{
			add
			{
				this._PopupWindowClosedEvent = (Action)Delegate.Combine(this._PopupWindowClosedEvent, value);
			}
			remove
			{
				this._PopupWindowClosedEvent = (Action)Delegate.Remove(this._PopupWindowClosedEvent, value);
			}
		}

		// Token: 0x14000066 RID: 102
		// (add) Token: 0x06002A88 RID: 10888 RVA: 0x00200962 File Offset: 0x001FEB62
		// (remove) Token: 0x06002A89 RID: 10889 RVA: 0x0020097B File Offset: 0x001FEB7B
		public event Action PopupWindowOpenedEvent
		{
			add
			{
				this._PopupWindowOpenedEvent = (Action)Delegate.Combine(this._PopupWindowOpenedEvent, value);
			}
			remove
			{
				this._PopupWindowOpenedEvent = (Action)Delegate.Remove(this._PopupWindowOpenedEvent, value);
			}
		}

		// Token: 0x14000067 RID: 103
		// (add) Token: 0x06002A8A RID: 10890 RVA: 0x00200994 File Offset: 0x001FEB94
		// (remove) Token: 0x06002A8B RID: 10891 RVA: 0x002009AD File Offset: 0x001FEBAD
		public event Action InputPollingStartedEvent
		{
			add
			{
				this._InputPollingStartedEvent = (Action)Delegate.Combine(this._InputPollingStartedEvent, value);
			}
			remove
			{
				this._InputPollingStartedEvent = (Action)Delegate.Remove(this._InputPollingStartedEvent, value);
			}
		}

		// Token: 0x14000068 RID: 104
		// (add) Token: 0x06002A8C RID: 10892 RVA: 0x002009C6 File Offset: 0x001FEBC6
		// (remove) Token: 0x06002A8D RID: 10893 RVA: 0x002009DF File Offset: 0x001FEBDF
		public event Action InputPollingEndedEvent
		{
			add
			{
				this._InputPollingEndedEvent = (Action)Delegate.Combine(this._InputPollingEndedEvent, value);
			}
			remove
			{
				this._InputPollingEndedEvent = (Action)Delegate.Remove(this._InputPollingEndedEvent, value);
			}
		}

		// Token: 0x14000069 RID: 105
		// (add) Token: 0x06002A8E RID: 10894 RVA: 0x002009F8 File Offset: 0x001FEBF8
		// (remove) Token: 0x06002A8F RID: 10895 RVA: 0x00200A06 File Offset: 0x001FEC06
		public event UnityAction onScreenClosed
		{
			add
			{
				this._onScreenClosed.AddListener(value);
			}
			remove
			{
				this._onScreenClosed.RemoveListener(value);
			}
		}

		// Token: 0x1400006A RID: 106
		// (add) Token: 0x06002A90 RID: 10896 RVA: 0x00200A14 File Offset: 0x001FEC14
		// (remove) Token: 0x06002A91 RID: 10897 RVA: 0x00200A22 File Offset: 0x001FEC22
		public event UnityAction onScreenOpened
		{
			add
			{
				this._onScreenOpened.AddListener(value);
			}
			remove
			{
				this._onScreenOpened.RemoveListener(value);
			}
		}

		// Token: 0x1400006B RID: 107
		// (add) Token: 0x06002A92 RID: 10898 RVA: 0x00200A30 File Offset: 0x001FEC30
		// (remove) Token: 0x06002A93 RID: 10899 RVA: 0x00200A3E File Offset: 0x001FEC3E
		public event UnityAction onPopupWindowClosed
		{
			add
			{
				this._onPopupWindowClosed.AddListener(value);
			}
			remove
			{
				this._onPopupWindowClosed.RemoveListener(value);
			}
		}

		// Token: 0x1400006C RID: 108
		// (add) Token: 0x06002A94 RID: 10900 RVA: 0x00200A4C File Offset: 0x001FEC4C
		// (remove) Token: 0x06002A95 RID: 10901 RVA: 0x00200A5A File Offset: 0x001FEC5A
		public event UnityAction onPopupWindowOpened
		{
			add
			{
				this._onPopupWindowOpened.AddListener(value);
			}
			remove
			{
				this._onPopupWindowOpened.RemoveListener(value);
			}
		}

		// Token: 0x1400006D RID: 109
		// (add) Token: 0x06002A96 RID: 10902 RVA: 0x00200A68 File Offset: 0x001FEC68
		// (remove) Token: 0x06002A97 RID: 10903 RVA: 0x00200A76 File Offset: 0x001FEC76
		public event UnityAction onInputPollingStarted
		{
			add
			{
				this._onInputPollingStarted.AddListener(value);
			}
			remove
			{
				this._onInputPollingStarted.RemoveListener(value);
			}
		}

		// Token: 0x1400006E RID: 110
		// (add) Token: 0x06002A98 RID: 10904 RVA: 0x00200A84 File Offset: 0x001FEC84
		// (remove) Token: 0x06002A99 RID: 10905 RVA: 0x00200A92 File Offset: 0x001FEC92
		public event UnityAction onInputPollingEnded
		{
			add
			{
				this._onInputPollingEnded.AddListener(value);
			}
			remove
			{
				this._onInputPollingEnded.RemoveListener(value);
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06002A9A RID: 10906 RVA: 0x00200AA0 File Offset: 0x001FECA0
		// (set) Token: 0x06002A9B RID: 10907 RVA: 0x00200AA8 File Offset: 0x001FECA8
		public InputManager rewiredInputManager
		{
			get
			{
				return this._rewiredInputManager;
			}
			set
			{
				this._rewiredInputManager = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06002A9C RID: 10908 RVA: 0x00200AB8 File Offset: 0x001FECB8
		// (set) Token: 0x06002A9D RID: 10909 RVA: 0x00200AC0 File Offset: 0x001FECC0
		public bool dontDestroyOnLoad
		{
			get
			{
				return this._dontDestroyOnLoad;
			}
			set
			{
				if (value != this._dontDestroyOnLoad && value)
				{
					Object.DontDestroyOnLoad(base.transform.gameObject);
				}
				this._dontDestroyOnLoad = value;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06002A9E RID: 10910 RVA: 0x00200AE5 File Offset: 0x001FECE5
		// (set) Token: 0x06002A9F RID: 10911 RVA: 0x00200AED File Offset: 0x001FECED
		public int keyboardMapDefaultLayout
		{
			get
			{
				return this._keyboardMapDefaultLayout;
			}
			set
			{
				this._keyboardMapDefaultLayout = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06002AA0 RID: 10912 RVA: 0x00200AFD File Offset: 0x001FECFD
		// (set) Token: 0x06002AA1 RID: 10913 RVA: 0x00200B05 File Offset: 0x001FED05
		public int mouseMapDefaultLayout
		{
			get
			{
				return this._mouseMapDefaultLayout;
			}
			set
			{
				this._mouseMapDefaultLayout = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06002AA2 RID: 10914 RVA: 0x00200B15 File Offset: 0x001FED15
		// (set) Token: 0x06002AA3 RID: 10915 RVA: 0x00200B1D File Offset: 0x001FED1D
		public int joystickMapDefaultLayout
		{
			get
			{
				return this._joystickMapDefaultLayout;
			}
			set
			{
				this._joystickMapDefaultLayout = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06002AA4 RID: 10916 RVA: 0x00200B2D File Offset: 0x001FED2D
		// (set) Token: 0x06002AA5 RID: 10917 RVA: 0x00200B46 File Offset: 0x001FED46
		public bool showPlayers
		{
			get
			{
				return this._showPlayers && ReInput.players.playerCount > 1;
			}
			set
			{
				this._showPlayers = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06002AA6 RID: 10918 RVA: 0x00200B56 File Offset: 0x001FED56
		// (set) Token: 0x06002AA7 RID: 10919 RVA: 0x00200B5E File Offset: 0x001FED5E
		public bool showControllers
		{
			get
			{
				return this._showControllers;
			}
			set
			{
				this._showControllers = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06002AA8 RID: 10920 RVA: 0x00200B6E File Offset: 0x001FED6E
		// (set) Token: 0x06002AA9 RID: 10921 RVA: 0x00200B76 File Offset: 0x001FED76
		public bool showKeyboard
		{
			get
			{
				return this._showKeyboard;
			}
			set
			{
				this._showKeyboard = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06002AAA RID: 10922 RVA: 0x00200B86 File Offset: 0x001FED86
		// (set) Token: 0x06002AAB RID: 10923 RVA: 0x00200B8E File Offset: 0x001FED8E
		public bool showMouse
		{
			get
			{
				return this._showMouse;
			}
			set
			{
				this._showMouse = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06002AAC RID: 10924 RVA: 0x00200B9E File Offset: 0x001FED9E
		// (set) Token: 0x06002AAD RID: 10925 RVA: 0x00200BA6 File Offset: 0x001FEDA6
		public int maxControllersPerPlayer
		{
			get
			{
				return this._maxControllersPerPlayer;
			}
			set
			{
				this._maxControllersPerPlayer = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06002AAE RID: 10926 RVA: 0x00200BB6 File Offset: 0x001FEDB6
		// (set) Token: 0x06002AAF RID: 10927 RVA: 0x00200BBE File Offset: 0x001FEDBE
		public bool showActionCategoryLabels
		{
			get
			{
				return this._showActionCategoryLabels;
			}
			set
			{
				this._showActionCategoryLabels = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06002AB0 RID: 10928 RVA: 0x00200BCE File Offset: 0x001FEDCE
		// (set) Token: 0x06002AB1 RID: 10929 RVA: 0x00200BD6 File Offset: 0x001FEDD6
		public int keyboardInputFieldCount
		{
			get
			{
				return this._keyboardInputFieldCount;
			}
			set
			{
				this._keyboardInputFieldCount = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06002AB2 RID: 10930 RVA: 0x00200BE6 File Offset: 0x001FEDE6
		// (set) Token: 0x06002AB3 RID: 10931 RVA: 0x00200BEE File Offset: 0x001FEDEE
		public int mouseInputFieldCount
		{
			get
			{
				return this._mouseInputFieldCount;
			}
			set
			{
				this._mouseInputFieldCount = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06002AB4 RID: 10932 RVA: 0x00200BFE File Offset: 0x001FEDFE
		// (set) Token: 0x06002AB5 RID: 10933 RVA: 0x00200C06 File Offset: 0x001FEE06
		public int controllerInputFieldCount
		{
			get
			{
				return this._controllerInputFieldCount;
			}
			set
			{
				this._controllerInputFieldCount = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06002AB6 RID: 10934 RVA: 0x00200C16 File Offset: 0x001FEE16
		// (set) Token: 0x06002AB7 RID: 10935 RVA: 0x00200C1E File Offset: 0x001FEE1E
		public bool showFullAxisInputFields
		{
			get
			{
				return this._showFullAxisInputFields;
			}
			set
			{
				this._showFullAxisInputFields = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06002AB8 RID: 10936 RVA: 0x00200C2E File Offset: 0x001FEE2E
		// (set) Token: 0x06002AB9 RID: 10937 RVA: 0x00200C36 File Offset: 0x001FEE36
		public bool showSplitAxisInputFields
		{
			get
			{
				return this._showSplitAxisInputFields;
			}
			set
			{
				this._showSplitAxisInputFields = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06002ABA RID: 10938 RVA: 0x00200C46 File Offset: 0x001FEE46
		// (set) Token: 0x06002ABB RID: 10939 RVA: 0x00200C4E File Offset: 0x001FEE4E
		public bool allowElementAssignmentConflicts
		{
			get
			{
				return this._allowElementAssignmentConflicts;
			}
			set
			{
				this._allowElementAssignmentConflicts = value;
				this.InspectorPropertyChanged(false);
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06002ABC RID: 10940 RVA: 0x00200C5E File Offset: 0x001FEE5E
		// (set) Token: 0x06002ABD RID: 10941 RVA: 0x00200C66 File Offset: 0x001FEE66
		public bool allowElementAssignmentSwap
		{
			get
			{
				return this._allowElementAssignmentSwap;
			}
			set
			{
				this._allowElementAssignmentSwap = value;
				this.InspectorPropertyChanged(false);
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06002ABE RID: 10942 RVA: 0x00200C76 File Offset: 0x001FEE76
		// (set) Token: 0x06002ABF RID: 10943 RVA: 0x00200C7E File Offset: 0x001FEE7E
		public int actionLabelWidth
		{
			get
			{
				return this._actionLabelWidth;
			}
			set
			{
				this._actionLabelWidth = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06002AC0 RID: 10944 RVA: 0x00200C8E File Offset: 0x001FEE8E
		// (set) Token: 0x06002AC1 RID: 10945 RVA: 0x00200C96 File Offset: 0x001FEE96
		public int keyboardColMaxWidth
		{
			get
			{
				return this._keyboardColMaxWidth;
			}
			set
			{
				this._keyboardColMaxWidth = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06002AC2 RID: 10946 RVA: 0x00200CA6 File Offset: 0x001FEEA6
		// (set) Token: 0x06002AC3 RID: 10947 RVA: 0x00200CAE File Offset: 0x001FEEAE
		public int mouseColMaxWidth
		{
			get
			{
				return this._mouseColMaxWidth;
			}
			set
			{
				this._mouseColMaxWidth = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06002AC4 RID: 10948 RVA: 0x00200CBE File Offset: 0x001FEEBE
		// (set) Token: 0x06002AC5 RID: 10949 RVA: 0x00200CC6 File Offset: 0x001FEEC6
		public int controllerColMaxWidth
		{
			get
			{
				return this._controllerColMaxWidth;
			}
			set
			{
				this._controllerColMaxWidth = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06002AC6 RID: 10950 RVA: 0x00200CD6 File Offset: 0x001FEED6
		// (set) Token: 0x06002AC7 RID: 10951 RVA: 0x00200CDE File Offset: 0x001FEEDE
		public int inputRowHeight
		{
			get
			{
				return this._inputRowHeight;
			}
			set
			{
				this._inputRowHeight = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06002AC8 RID: 10952 RVA: 0x00200CEE File Offset: 0x001FEEEE
		// (set) Token: 0x06002AC9 RID: 10953 RVA: 0x00200CF6 File Offset: 0x001FEEF6
		public int inputColumnSpacing
		{
			get
			{
				return this._inputColumnSpacing;
			}
			set
			{
				this._inputColumnSpacing = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06002ACA RID: 10954 RVA: 0x00200D06 File Offset: 0x001FEF06
		// (set) Token: 0x06002ACB RID: 10955 RVA: 0x00200D0E File Offset: 0x001FEF0E
		public int inputRowCategorySpacing
		{
			get
			{
				return this._inputRowCategorySpacing;
			}
			set
			{
				this._inputRowCategorySpacing = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06002ACC RID: 10956 RVA: 0x00200D1E File Offset: 0x001FEF1E
		// (set) Token: 0x06002ACD RID: 10957 RVA: 0x00200D26 File Offset: 0x001FEF26
		public int invertToggleWidth
		{
			get
			{
				return this._invertToggleWidth;
			}
			set
			{
				this._invertToggleWidth = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06002ACE RID: 10958 RVA: 0x00200D36 File Offset: 0x001FEF36
		// (set) Token: 0x06002ACF RID: 10959 RVA: 0x00200D3E File Offset: 0x001FEF3E
		public int defaultWindowWidth
		{
			get
			{
				return this._defaultWindowWidth;
			}
			set
			{
				this._defaultWindowWidth = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06002AD0 RID: 10960 RVA: 0x00200D4E File Offset: 0x001FEF4E
		// (set) Token: 0x06002AD1 RID: 10961 RVA: 0x00200D56 File Offset: 0x001FEF56
		public int defaultWindowHeight
		{
			get
			{
				return this._defaultWindowHeight;
			}
			set
			{
				this._defaultWindowHeight = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06002AD2 RID: 10962 RVA: 0x00200D66 File Offset: 0x001FEF66
		// (set) Token: 0x06002AD3 RID: 10963 RVA: 0x00200D6E File Offset: 0x001FEF6E
		public float controllerAssignmentTimeout
		{
			get
			{
				return this._controllerAssignmentTimeout;
			}
			set
			{
				this._controllerAssignmentTimeout = value;
				this.InspectorPropertyChanged(false);
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06002AD4 RID: 10964 RVA: 0x00200D7E File Offset: 0x001FEF7E
		// (set) Token: 0x06002AD5 RID: 10965 RVA: 0x00200D86 File Offset: 0x001FEF86
		public float preInputAssignmentTimeout
		{
			get
			{
				return this._preInputAssignmentTimeout;
			}
			set
			{
				this._preInputAssignmentTimeout = value;
				this.InspectorPropertyChanged(false);
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06002AD6 RID: 10966 RVA: 0x00200D96 File Offset: 0x001FEF96
		// (set) Token: 0x06002AD7 RID: 10967 RVA: 0x00200D9E File Offset: 0x001FEF9E
		public float inputAssignmentTimeout
		{
			get
			{
				return this._inputAssignmentTimeout;
			}
			set
			{
				this._inputAssignmentTimeout = value;
				this.InspectorPropertyChanged(false);
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06002AD8 RID: 10968 RVA: 0x00200DAE File Offset: 0x001FEFAE
		// (set) Token: 0x06002AD9 RID: 10969 RVA: 0x00200DB6 File Offset: 0x001FEFB6
		public float axisCalibrationTimeout
		{
			get
			{
				return this._axisCalibrationTimeout;
			}
			set
			{
				this._axisCalibrationTimeout = value;
				this.InspectorPropertyChanged(false);
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06002ADA RID: 10970 RVA: 0x00200DC6 File Offset: 0x001FEFC6
		// (set) Token: 0x06002ADB RID: 10971 RVA: 0x00200DCE File Offset: 0x001FEFCE
		public bool ignoreMouseXAxisAssignment
		{
			get
			{
				return this._ignoreMouseXAxisAssignment;
			}
			set
			{
				this._ignoreMouseXAxisAssignment = value;
				this.InspectorPropertyChanged(false);
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06002ADC RID: 10972 RVA: 0x00200DDE File Offset: 0x001FEFDE
		// (set) Token: 0x06002ADD RID: 10973 RVA: 0x00200DE6 File Offset: 0x001FEFE6
		public bool ignoreMouseYAxisAssignment
		{
			get
			{
				return this._ignoreMouseYAxisAssignment;
			}
			set
			{
				this._ignoreMouseYAxisAssignment = value;
				this.InspectorPropertyChanged(false);
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06002ADE RID: 10974 RVA: 0x00200DF6 File Offset: 0x001FEFF6
		// (set) Token: 0x06002ADF RID: 10975 RVA: 0x00200DFE File Offset: 0x001FEFFE
		public bool universalCancelClosesScreen
		{
			get
			{
				return this._universalCancelClosesScreen;
			}
			set
			{
				this._universalCancelClosesScreen = value;
				this.InspectorPropertyChanged(false);
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06002AE0 RID: 10976 RVA: 0x00200E0E File Offset: 0x001FF00E
		// (set) Token: 0x06002AE1 RID: 10977 RVA: 0x00200E16 File Offset: 0x001FF016
		public bool showInputBehaviorSettings
		{
			get
			{
				return this._showInputBehaviorSettings;
			}
			set
			{
				this._showInputBehaviorSettings = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06002AE2 RID: 10978 RVA: 0x00200E26 File Offset: 0x001FF026
		// (set) Token: 0x06002AE3 RID: 10979 RVA: 0x00200E2E File Offset: 0x001FF02E
		public bool useThemeSettings
		{
			get
			{
				return this._useThemeSettings;
			}
			set
			{
				this._useThemeSettings = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06002AE4 RID: 10980 RVA: 0x00200E3E File Offset: 0x001FF03E
		// (set) Token: 0x06002AE5 RID: 10981 RVA: 0x00200E46 File Offset: 0x001FF046
		public LanguageDataBase language
		{
			get
			{
				return this._language;
			}
			set
			{
				this._language = value;
				if (this._language != null)
				{
					this._language.Initialize();
				}
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06002AE6 RID: 10982 RVA: 0x00200E6F File Offset: 0x001FF06F
		// (set) Token: 0x06002AE7 RID: 10983 RVA: 0x00200E77 File Offset: 0x001FF077
		public bool showPlayersGroupLabel
		{
			get
			{
				return this._showPlayersGroupLabel;
			}
			set
			{
				this._showPlayersGroupLabel = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06002AE8 RID: 10984 RVA: 0x00200E87 File Offset: 0x001FF087
		// (set) Token: 0x06002AE9 RID: 10985 RVA: 0x00200E8F File Offset: 0x001FF08F
		public bool showControllerGroupLabel
		{
			get
			{
				return this._showControllerGroupLabel;
			}
			set
			{
				this._showControllerGroupLabel = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06002AEA RID: 10986 RVA: 0x00200E9F File Offset: 0x001FF09F
		// (set) Token: 0x06002AEB RID: 10987 RVA: 0x00200EA7 File Offset: 0x001FF0A7
		public bool showAssignedControllersGroupLabel
		{
			get
			{
				return this._showAssignedControllersGroupLabel;
			}
			set
			{
				this._showAssignedControllersGroupLabel = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06002AEC RID: 10988 RVA: 0x00200EB7 File Offset: 0x001FF0B7
		// (set) Token: 0x06002AED RID: 10989 RVA: 0x00200EBF File Offset: 0x001FF0BF
		public bool showSettingsGroupLabel
		{
			get
			{
				return this._showSettingsGroupLabel;
			}
			set
			{
				this._showSettingsGroupLabel = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06002AEE RID: 10990 RVA: 0x00200ECF File Offset: 0x001FF0CF
		// (set) Token: 0x06002AEF RID: 10991 RVA: 0x00200ED7 File Offset: 0x001FF0D7
		public bool showMapCategoriesGroupLabel
		{
			get
			{
				return this._showMapCategoriesGroupLabel;
			}
			set
			{
				this._showMapCategoriesGroupLabel = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06002AF0 RID: 10992 RVA: 0x00200EE7 File Offset: 0x001FF0E7
		// (set) Token: 0x06002AF1 RID: 10993 RVA: 0x00200EEF File Offset: 0x001FF0EF
		public bool showControllerNameLabel
		{
			get
			{
				return this._showControllerNameLabel;
			}
			set
			{
				this._showControllerNameLabel = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06002AF2 RID: 10994 RVA: 0x00200EFF File Offset: 0x001FF0FF
		// (set) Token: 0x06002AF3 RID: 10995 RVA: 0x00200F07 File Offset: 0x001FF107
		public bool showAssignedControllers
		{
			get
			{
				return this._showAssignedControllers;
			}
			set
			{
				this._showAssignedControllers = value;
				this.InspectorPropertyChanged(true);
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06002AF4 RID: 10996 RVA: 0x00200F17 File Offset: 0x001FF117
		// (set) Token: 0x06002AF5 RID: 10997 RVA: 0x00200F1F File Offset: 0x001FF11F
		public Action restoreDefaultsDelegate
		{
			get
			{
				return this._restoreDefaultsDelegate;
			}
			set
			{
				this._restoreDefaultsDelegate = value;
			}
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06002AF6 RID: 10998 RVA: 0x00200F28 File Offset: 0x001FF128
		public bool isOpen
		{
			get
			{
				if (!this.initialized)
				{
					return this.references.canvas != null && this.references.canvas.gameObject.activeInHierarchy;
				}
				return this.canvas.activeInHierarchy;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06002AF7 RID: 10999 RVA: 0x00200F68 File Offset: 0x001FF168
		private bool isFocused
		{
			get
			{
				return this.initialized && !this.windowManager.isWindowOpen;
			}
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06002AF8 RID: 11000 RVA: 0x00200F82 File Offset: 0x001FF182
		private bool inputAllowed
		{
			get
			{
				return this.blockInputOnFocusEndTime <= Time.unscaledTime;
			}
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06002AF9 RID: 11001 RVA: 0x00200F94 File Offset: 0x001FF194
		private int inputGridColumnCount
		{
			get
			{
				int num = 1;
				if (this._showKeyboard)
				{
					num++;
				}
				if (this._showMouse)
				{
					num++;
				}
				if (this._showControllers)
				{
					num++;
				}
				return num;
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06002AFA RID: 11002 RVA: 0x00200FC8 File Offset: 0x001FF1C8
		private int inputGridWidth
		{
			get
			{
				return this._actionLabelWidth + (this._showKeyboard ? this._keyboardColMaxWidth : 0) + (this._showMouse ? this._mouseColMaxWidth : 0) + (this._showControllers ? this._controllerColMaxWidth : 0) + (this.inputGridColumnCount - 1) * this._inputColumnSpacing;
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06002AFB RID: 11003 RVA: 0x00201021 File Offset: 0x001FF221
		private Player currentPlayer
		{
			get
			{
				return ReInput.players.GetPlayer(this.currentPlayerId);
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06002AFC RID: 11004 RVA: 0x00201033 File Offset: 0x001FF233
		private InputCategory currentMapCategory
		{
			get
			{
				return ReInput.mapping.GetMapCategory(this.currentMapCategoryId);
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06002AFD RID: 11005 RVA: 0x00201048 File Offset: 0x001FF248
		private ControlMapper.MappingSet currentMappingSet
		{
			get
			{
				if (this.currentMapCategoryId < 0)
				{
					return null;
				}
				for (int i = 0; i < this._mappingSets.Length; i++)
				{
					if (this._mappingSets[i].mapCategoryId == this.currentMapCategoryId)
					{
						return this._mappingSets[i];
					}
				}
				return null;
			}
		}

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06002AFE RID: 11006 RVA: 0x00201092 File Offset: 0x001FF292
		private Joystick currentJoystick
		{
			get
			{
				return ReInput.controllers.GetJoystick(this.currentJoystickId);
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06002AFF RID: 11007 RVA: 0x002010A4 File Offset: 0x001FF2A4
		private bool isJoystickSelected
		{
			get
			{
				return this.currentJoystickId >= 0;
			}
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06002B00 RID: 11008 RVA: 0x002010B2 File Offset: 0x001FF2B2
		private GameObject currentUISelection
		{
			get
			{
				if (!(EventSystem.current != null))
				{
					return null;
				}
				return EventSystem.current.currentSelectedGameObject;
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06002B01 RID: 11009 RVA: 0x002010CD File Offset: 0x001FF2CD
		private bool showSettings
		{
			get
			{
				return this._showInputBehaviorSettings && this._inputBehaviorSettings.Length != 0;
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06002B02 RID: 11010 RVA: 0x002010E3 File Offset: 0x001FF2E3
		private bool showMapCategories
		{
			get
			{
				return this._mappingSets != null && this._mappingSets.Length > 1;
			}
		}

		// Token: 0x06002B03 RID: 11011 RVA: 0x002010FD File Offset: 0x001FF2FD
		private void Awake()
		{
			if (this._dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(base.transform.gameObject);
			}
			this.PreInitialize();
			if (this.isOpen)
			{
				this.Initialize();
				this.Open(true);
			}
		}

		// Token: 0x06002B04 RID: 11012 RVA: 0x00201132 File Offset: 0x001FF332
		private void Start()
		{
			if (this._openOnStart)
			{
				this.Open(false);
			}
		}

		// Token: 0x06002B05 RID: 11013 RVA: 0x00201143 File Offset: 0x001FF343
		private void Update()
		{
			if (!this.isOpen)
			{
				return;
			}
			if (!this.initialized)
			{
				return;
			}
			this.CheckUISelection();
		}

		// Token: 0x06002B06 RID: 11014 RVA: 0x0020115D File Offset: 0x001FF35D
		private void OnDestroy()
		{
			ReInput.ControllerConnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnJoystickConnected);
			ReInput.ControllerDisconnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnJoystickDisconnected);
			ReInput.ControllerPreDisconnectEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnJoystickPreDisconnect);
			this.UnsubscribeMenuControlInputEvents();
		}

		// Token: 0x06002B07 RID: 11015 RVA: 0x00201198 File Offset: 0x001FF398
		private void PreInitialize()
		{
			if (!ReInput.isReady)
			{
				Debug.LogError("Rewired Control Mapper: Rewired has not been initialized! Are you missing a Rewired Input Manager in your scene?");
				return;
			}
			this.SubscribeMenuControlInputEvents();
		}

		// Token: 0x06002B08 RID: 11016 RVA: 0x002011B4 File Offset: 0x001FF3B4
		private void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			if (!ReInput.isReady)
			{
				return;
			}
			if (this._rewiredInputManager == null)
			{
				this._rewiredInputManager = Object.FindObjectOfType<InputManager>();
				if (this._rewiredInputManager == null)
				{
					Debug.LogError("Rewired Control Mapper: A Rewired Input Manager was not assigned in the inspector or found in the current scene! Control Mapper will not function.");
					return;
				}
			}
			if (ControlMapper.Instance != null)
			{
				Debug.LogError("Rewired Control Mapper: Only one ControlMapper can exist at one time!");
				return;
			}
			ControlMapper.Instance = this;
			if (this.prefabs == null || !this.prefabs.Check())
			{
				Debug.LogError("Rewired Control Mapper: All prefabs must be assigned in the inspector!");
				return;
			}
			if (this.references == null || !this.references.Check())
			{
				Debug.LogError("Rewired Control Mapper: All references must be assigned in the inspector!");
				return;
			}
			this.references.inputGridLayoutElement = this.references.inputGridContainer.GetComponent<LayoutElement>();
			if (this.references.inputGridLayoutElement == null)
			{
				Debug.LogError("Rewired Control Mapper: InputGridContainer is missing LayoutElement component!");
				return;
			}
			if (this._showKeyboard && this._keyboardInputFieldCount < 1)
			{
				Debug.LogWarning("Rewired Control Mapper: Keyboard Input Fields must be at least 1!");
				this._keyboardInputFieldCount = 1;
			}
			if (this._showMouse && this._mouseInputFieldCount < 1)
			{
				Debug.LogWarning("Rewired Control Mapper: Mouse Input Fields must be at least 1!");
				this._mouseInputFieldCount = 1;
			}
			if (this._showControllers && this._controllerInputFieldCount < 1)
			{
				Debug.LogWarning("Rewired Control Mapper: Controller Input Fields must be at least 1!");
				this._controllerInputFieldCount = 1;
			}
			if (this._maxControllersPerPlayer < 0)
			{
				Debug.LogWarning("Rewired Control Mapper: Max Controllers Per Player must be at least 0 (no limit)!");
				this._maxControllersPerPlayer = 0;
			}
			if (this._useThemeSettings && this._themeSettings == null)
			{
				Debug.LogWarning("Rewired Control Mapper: To use theming, Theme Settings must be set in the inspector! Theming has been disabled.");
				this._useThemeSettings = false;
			}
			if (this._language == null)
			{
				Debug.LogError("Rawired UI: Language must be set in the inspector!");
				return;
			}
			this._language.Initialize();
			this.inputFieldActivatedDelegate = new Action<InputFieldInfo>(this.OnInputFieldActivated);
			this.inputFieldInvertToggleStateChangedDelegate = new Action<ToggleInfo, bool>(this.OnInputFieldInvertToggleStateChanged);
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnJoystickConnected);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnJoystickDisconnected);
			ReInput.ControllerPreDisconnectEvent += new Action<ControllerStatusChangedEventArgs>(this.OnJoystickPreDisconnect);
			this.playerCount = ReInput.players.playerCount;
			this.canvas = this.references.canvas.gameObject;
			this.windowManager = new ControlMapper.WindowManager(this.prefabs.window, this.prefabs.fader, this.references.canvas.transform);
			this.playerButtons = new List<ControlMapper.GUIButton>();
			this.mapCategoryButtons = new List<ControlMapper.GUIButton>();
			this.assignedControllerButtons = new List<ControlMapper.GUIButton>();
			this.miscInstantiatedObjects = new List<GameObject>();
			this.currentMapCategoryId = this._mappingSets[0].mapCategoryId;
			this.Draw();
			this.CreateInputGrid();
			this.CreateLayout();
			this.SubscribeFixedUISelectionEvents();
			this.initialized = true;
		}

		// Token: 0x06002B09 RID: 11017 RVA: 0x00201472 File Offset: 0x001FF672
		private void OnJoystickConnected(ControllerStatusChangedEventArgs args)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this._showControllers)
			{
				return;
			}
			this.ClearVarsOnJoystickChange();
			this.ForceRefresh();
		}

		// Token: 0x06002B0A RID: 11018 RVA: 0x00201472 File Offset: 0x001FF672
		private void OnJoystickDisconnected(ControllerStatusChangedEventArgs args)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this._showControllers)
			{
				return;
			}
			this.ClearVarsOnJoystickChange();
			this.ForceRefresh();
		}

		// Token: 0x06002B0B RID: 11019 RVA: 0x00201492 File Offset: 0x001FF692
		private void OnJoystickPreDisconnect(ControllerStatusChangedEventArgs args)
		{
			if (!this.initialized)
			{
				return;
			}
			bool showControllers = this._showControllers;
		}

		// Token: 0x06002B0C RID: 11020 RVA: 0x002014A4 File Offset: 0x001FF6A4
		public void OnButtonActivated(ButtonInfo buttonInfo)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.inputAllowed)
			{
				return;
			}
			string identifier = buttonInfo.identifier;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(identifier);
			if (num <= 1656078790U)
			{
				if (num <= 1293854844U)
				{
					if (num != 36291085U)
					{
						if (num != 1293854844U)
						{
							return;
						}
						if (!(identifier == "AssignController"))
						{
							return;
						}
						this.ShowAssignControllerWindow();
						return;
					}
					else
					{
						if (!(identifier == "MapCategorySelection"))
						{
							return;
						}
						this.OnMapCategorySelected(buttonInfo.intData, true);
						return;
					}
				}
				else if (num != 1619204974U)
				{
					if (num != 1656078790U)
					{
						return;
					}
					if (!(identifier == "EditInputBehaviors"))
					{
						return;
					}
					this.ShowEditInputBehaviorsWindow();
					return;
				}
				else
				{
					if (!(identifier == "PlayerSelection"))
					{
						return;
					}
					this.OnPlayerSelected(buttonInfo.intData, true);
					return;
				}
			}
			else if (num <= 2379421585U)
			{
				if (num != 2139278426U)
				{
					if (num != 2379421585U)
					{
						return;
					}
					if (!(identifier == "Done"))
					{
						return;
					}
					this.Close(true);
					return;
				}
				else
				{
					if (!(identifier == "CalibrateController"))
					{
						return;
					}
					this.ShowCalibrateControllerWindow();
					return;
				}
			}
			else if (num != 2857234147U)
			{
				if (num != 3019194153U)
				{
					if (num != 3496297297U)
					{
						return;
					}
					if (!(identifier == "AssignedControllerSelection"))
					{
						return;
					}
					this.OnControllerSelected(buttonInfo.intData);
					return;
				}
				else
				{
					if (!(identifier == "RemoveController"))
					{
						return;
					}
					this.OnRemoveCurrentController();
					return;
				}
			}
			else
			{
				if (!(identifier == "RestoreDefaults"))
				{
					return;
				}
				this.OnRestoreDefaults();
				return;
			}
		}

		// Token: 0x06002B0D RID: 11021 RVA: 0x0020160C File Offset: 0x001FF80C
		public void OnInputFieldActivated(InputFieldInfo fieldInfo)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.inputAllowed)
			{
				return;
			}
			if (this.currentPlayer == null)
			{
				return;
			}
			InputAction action = ReInput.mapping.GetAction(fieldInfo.actionId);
			if (action == null)
			{
				return;
			}
			AxisRange axisRange = (action.type == null) ? fieldInfo.axisRange : 0;
			string actionName = this._language.GetActionName(action.id, axisRange);
			ControllerMap controllerMap = this.GetControllerMap(fieldInfo.controllerType);
			if (controllerMap == null)
			{
				return;
			}
			ActionElementMap actionElementMap = (fieldInfo.actionElementMapId >= 0) ? controllerMap.GetElementMap(fieldInfo.actionElementMapId) : null;
			if (actionElementMap != null)
			{
				this.ShowBeginElementAssignmentReplacementWindow(fieldInfo, action, controllerMap, actionElementMap, actionName);
				return;
			}
			this.ShowCreateNewElementAssignmentWindow(fieldInfo, action, controllerMap, actionName);
		}

		// Token: 0x06002B0E RID: 11022 RVA: 0x002016B4 File Offset: 0x001FF8B4
		public void OnInputFieldInvertToggleStateChanged(ToggleInfo toggleInfo, bool newState)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.inputAllowed)
			{
				return;
			}
			this.SetActionAxisInverted(newState, toggleInfo.controllerType, toggleInfo.actionElementMapId);
		}

		// Token: 0x06002B0F RID: 11023 RVA: 0x002016DB File Offset: 0x001FF8DB
		private void OnPlayerSelected(int playerId, bool redraw)
		{
			if (!this.initialized)
			{
				return;
			}
			this.currentPlayerId = playerId;
			this.ClearVarsOnPlayerChange();
			if (redraw)
			{
				this.Redraw(true, true);
			}
		}

		// Token: 0x06002B10 RID: 11024 RVA: 0x002016FE File Offset: 0x001FF8FE
		private void OnControllerSelected(int joystickId)
		{
			if (!this.initialized)
			{
				return;
			}
			this.currentJoystickId = joystickId;
			this.Redraw(true, true);
		}

		// Token: 0x06002B11 RID: 11025 RVA: 0x00201718 File Offset: 0x001FF918
		private void OnRemoveCurrentController()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (this.currentJoystickId < 0)
			{
				return;
			}
			this.RemoveController(this.currentPlayer, this.currentJoystickId);
			this.ClearVarsOnJoystickChange();
			this.Redraw(false, false);
		}

		// Token: 0x06002B12 RID: 11026 RVA: 0x0020174D File Offset: 0x001FF94D
		private void OnMapCategorySelected(int id, bool redraw)
		{
			if (!this.initialized)
			{
				return;
			}
			this.currentMapCategoryId = id;
			if (redraw)
			{
				this.Redraw(true, true);
			}
		}

		// Token: 0x06002B13 RID: 11027 RVA: 0x0020176A File Offset: 0x001FF96A
		private void OnRestoreDefaults()
		{
			if (!this.initialized)
			{
				return;
			}
			this.ShowRestoreDefaultsWindow();
		}

		// Token: 0x06002B14 RID: 11028 RVA: 0x0020177B File Offset: 0x001FF97B
		private void OnScreenToggleActionPressed(InputActionEventData data)
		{
			if (!this.isOpen)
			{
				this.Open();
				return;
			}
			if (!this.initialized)
			{
				return;
			}
			if (!this.isFocused)
			{
				return;
			}
			this.Close(true);
		}

		// Token: 0x06002B15 RID: 11029 RVA: 0x002017A5 File Offset: 0x001FF9A5
		private void OnScreenOpenActionPressed(InputActionEventData data)
		{
			this.Open();
		}

		// Token: 0x06002B16 RID: 11030 RVA: 0x002017AD File Offset: 0x001FF9AD
		private void OnScreenCloseActionPressed(InputActionEventData data)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.isOpen)
			{
				return;
			}
			if (!this.isFocused)
			{
				return;
			}
			this.Close(true);
		}

		// Token: 0x06002B17 RID: 11031 RVA: 0x002017D1 File Offset: 0x001FF9D1
		private void OnUniversalCancelActionPressed(InputActionEventData data)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.isOpen)
			{
				return;
			}
			if (this._universalCancelClosesScreen)
			{
				if (this.isFocused)
				{
					this.Close(true);
					return;
				}
			}
			else if (this.isFocused)
			{
				return;
			}
			this.CloseAllWindows();
		}

		// Token: 0x06002B18 RID: 11032 RVA: 0x0020180C File Offset: 0x001FFA0C
		private void OnWindowCancel(int windowId)
		{
			if (!this.initialized)
			{
				return;
			}
			if (windowId < 0)
			{
				return;
			}
			this.CloseWindow(windowId);
		}

		// Token: 0x06002B19 RID: 11033 RVA: 0x00201823 File Offset: 0x001FFA23
		private void OnRemoveElementAssignment(int windowId, ControllerMap map, ActionElementMap aem)
		{
			if (map == null || aem == null)
			{
				return;
			}
			map.DeleteElementMap(aem.id);
			this.CloseWindow(windowId);
		}

		// Token: 0x06002B1A RID: 11034 RVA: 0x00201840 File Offset: 0x001FFA40
		private void OnBeginElementAssignment(InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, string actionName)
		{
			if (fieldInfo == null || map == null)
			{
				return;
			}
			this.pendingInputMapping = new ControlMapper.InputMapping(actionName, fieldInfo, map, aem, fieldInfo.controllerType, fieldInfo.controllerId);
			switch (fieldInfo.controllerType)
			{
			case 0:
				this.ShowElementAssignmentPollingWindow();
				return;
			case 1:
				this.ShowElementAssignmentPollingWindow();
				return;
			case 2:
				this.ShowElementAssignmentPrePollingWindow();
				return;
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x06002B1B RID: 11035 RVA: 0x002018AB File Offset: 0x001FFAAB
		private void OnControllerAssignmentConfirmed(int windowId, Player player, int controllerId)
		{
			if (windowId < 0 || player == null || controllerId < 0)
			{
				return;
			}
			this.AssignController(player, controllerId);
			this.CloseWindow(windowId);
		}

		// Token: 0x06002B1C RID: 11036 RVA: 0x002018C8 File Offset: 0x001FFAC8
		private void OnMouseAssignmentConfirmed(int windowId, Player player)
		{
			if (windowId < 0 || player == null)
			{
				return;
			}
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i] != player)
				{
					players[i].controllers.hasMouse = false;
				}
			}
			player.controllers.hasMouse = true;
			this.CloseWindow(windowId);
		}

		// Token: 0x06002B1D RID: 11037 RVA: 0x00201928 File Offset: 0x001FFB28
		private void OnElementAssignmentConflictReplaceConfirmed(int windowId, ControlMapper.InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers, bool allowSwap)
		{
			if (this.currentPlayer == null || mapping == null)
			{
				return;
			}
			ElementAssignmentConflictCheck elementAssignmentConflictCheck;
			if (!this.CreateConflictCheck(mapping, assignment, out elementAssignmentConflictCheck))
			{
				Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
				this.CloseWindow(windowId);
				return;
			}
			ElementAssignmentConflictInfo elementAssignmentConflictInfo = default(ElementAssignmentConflictInfo);
			ActionElementMap actionElementMap = null;
			ActionElementMap actionElementMap2 = null;
			bool flag = false;
			if (allowSwap && mapping.aem != null && this.GetFirstElementAssignmentConflict(elementAssignmentConflictCheck, out elementAssignmentConflictInfo, skipOtherPlayers))
			{
				flag = true;
				actionElementMap2 = new ActionElementMap(mapping.aem);
				actionElementMap = new ActionElementMap(elementAssignmentConflictInfo.elementMap);
			}
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (!skipOtherPlayers || player == this.currentPlayer || player == ReInput.players.SystemPlayer)
				{
					player.controllers.conflictChecking.RemoveElementAssignmentConflicts(elementAssignmentConflictCheck);
				}
			}
			mapping.map.ReplaceOrCreateElementMap(assignment);
			if (allowSwap && flag)
			{
				int actionId = actionElementMap.actionId;
				Pole axisContribution = actionElementMap.axisContribution;
				bool flag2 = actionElementMap.invert;
				AxisRange axisRange = actionElementMap2.axisRange;
				ControllerElementType elementType = actionElementMap2.elementType;
				int elementIdentifierId = actionElementMap2.elementIdentifierId;
				KeyCode keyCode = actionElementMap2.keyCode;
				ModifierKeyFlags modifierKeyFlags = actionElementMap2.modifierKeyFlags;
				if (elementType == actionElementMap.elementType && elementType == null)
				{
					if (axisRange != actionElementMap.axisRange)
					{
						if (axisRange == null)
						{
							axisRange = 1;
						}
						else if (actionElementMap.axisRange == null)
						{
						}
					}
				}
				else if (elementType == null && (actionElementMap.elementType == 1 || (actionElementMap.elementType == null && actionElementMap.axisRange != null)) && axisRange == null)
				{
					axisRange = 1;
				}
				if (elementType != null || axisRange != null)
				{
					flag2 = false;
				}
				int num = 0;
				foreach (ActionElementMap actionElementMap3 in elementAssignmentConflictInfo.controllerMap.ElementMapsWithAction(actionId))
				{
					if (this.SwapIsSameInputRange(elementType, axisRange, axisContribution, actionElementMap3.elementType, actionElementMap3.axisRange, actionElementMap3.axisContribution))
					{
						num++;
					}
				}
				if (num < this.GetControllerInputFieldCount(mapping.controllerType))
				{
					elementAssignmentConflictInfo.controllerMap.ReplaceOrCreateElementMap(ElementAssignment.CompleteAssignment(mapping.controllerType, elementType, elementIdentifierId, axisRange, keyCode, modifierKeyFlags, actionId, axisContribution, flag2));
				}
			}
			this.CloseWindow(windowId);
		}

		// Token: 0x06002B1E RID: 11038 RVA: 0x00201B64 File Offset: 0x001FFD64
		private void OnElementAssignmentAddConfirmed(int windowId, ControlMapper.InputMapping mapping, ElementAssignment assignment)
		{
			if (this.currentPlayer == null || mapping == null)
			{
				return;
			}
			mapping.map.ReplaceOrCreateElementMap(assignment);
			this.CloseWindow(windowId);
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x00201B88 File Offset: 0x001FFD88
		private void OnRestoreDefaultsConfirmed(int windowId)
		{
			if (this._restoreDefaultsDelegate == null)
			{
				IList<Player> players = ReInput.players.Players;
				for (int i = 0; i < players.Count; i++)
				{
					Player player = players[i];
					if (this._showControllers)
					{
						player.controllers.maps.LoadDefaultMaps(2);
					}
					if (this._showKeyboard)
					{
						player.controllers.maps.LoadDefaultMaps(0);
					}
					if (this._showMouse)
					{
						player.controllers.maps.LoadDefaultMaps(1);
					}
				}
			}
			this.CloseWindow(windowId);
			if (this._restoreDefaultsDelegate != null)
			{
				this._restoreDefaultsDelegate.Invoke();
			}
		}

		// Token: 0x06002B20 RID: 11040 RVA: 0x00201C28 File Offset: 0x001FFE28
		private void OnAssignControllerWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			this.InputPollingStarted();
			if (window.timer.finished)
			{
				this.InputPollingStopped();
				this.CloseWindow(windowId);
				return;
			}
			ControllerPollingInfo controllerPollingInfo = ReInput.controllers.polling.PollAllControllersOfTypeForFirstElementDown(2);
			if (!controllerPollingInfo.success)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				return;
			}
			this.InputPollingStopped();
			if (ReInput.controllers.IsControllerAssigned(2, controllerPollingInfo.controllerId) && !this.currentPlayer.controllers.ContainsController(2, controllerPollingInfo.controllerId))
			{
				this.ShowControllerAssignmentConflictWindow(controllerPollingInfo.controllerId);
				return;
			}
			this.OnControllerAssignmentConfirmed(windowId, this.currentPlayer, controllerPollingInfo.controllerId);
		}

		// Token: 0x06002B21 RID: 11041 RVA: 0x00201D04 File Offset: 0x001FFF04
		private void OnElementAssignmentPrePollingWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingInputMapping == null)
			{
				return;
			}
			this.InputPollingStarted();
			if (!window.timer.finished)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				ControllerType controllerType = this.pendingInputMapping.controllerType;
				ControllerPollingInfo controllerPollingInfo;
				if (controllerType > 1)
				{
					if (controllerType != 2)
					{
						throw new NotImplementedException();
					}
					if (this.currentPlayer.controllers.joystickCount == 0)
					{
						return;
					}
					controllerPollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(this.pendingInputMapping.controllerType, this.currentJoystick.id);
				}
				else
				{
					controllerPollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(this.pendingInputMapping.controllerType, 0);
				}
				if (!controllerPollingInfo.success)
				{
					return;
				}
			}
			this.ShowElementAssignmentPollingWindow();
		}

		// Token: 0x06002B22 RID: 11042 RVA: 0x00201DF0 File Offset: 0x001FFFF0
		private void OnJoystickElementAssignmentPollingWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingInputMapping == null)
			{
				return;
			}
			this.InputPollingStarted();
			if (window.timer.finished)
			{
				this.InputPollingStopped();
				this.CloseWindow(windowId);
				return;
			}
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			if (this.currentPlayer.controllers.joystickCount == 0)
			{
				return;
			}
			ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(2, this.currentJoystick.id);
			if (!pollingInfo.success)
			{
				return;
			}
			if (!this.IsAllowedAssignment(this.pendingInputMapping, pollingInfo))
			{
				return;
			}
			ElementAssignment elementAssignment = this.pendingInputMapping.ToElementAssignment(pollingInfo);
			if (!this.HasElementAssignmentConflicts(this.currentPlayer, this.pendingInputMapping, elementAssignment, false))
			{
				this.pendingInputMapping.map.ReplaceOrCreateElementMap(elementAssignment);
				this.InputPollingStopped();
				this.CloseWindow(windowId);
				return;
			}
			this.InputPollingStopped();
			this.ShowElementAssignmentConflictWindow(elementAssignment, false);
		}

		// Token: 0x06002B23 RID: 11043 RVA: 0x00201EFC File Offset: 0x002000FC
		private void OnKeyboardElementAssignmentPollingWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingInputMapping == null)
			{
				return;
			}
			this.InputPollingStarted();
			if (window.timer.finished)
			{
				this.InputPollingStopped();
				this.CloseWindow(windowId);
				return;
			}
			ControllerPollingInfo pollingInfo;
			bool flag;
			ModifierKeyFlags modifierKeyFlags;
			string text;
			this.PollKeyboardForAssignment(out pollingInfo, out flag, out modifierKeyFlags, out text);
			if (flag)
			{
				window.timer.Start(this._inputAssignmentTimeout);
			}
			window.SetContentText(flag ? string.Empty : Mathf.CeilToInt(window.timer.remaining).ToString(), 2);
			window.SetContentText(text, 1);
			if (!pollingInfo.success)
			{
				return;
			}
			if (!this.IsAllowedAssignment(this.pendingInputMapping, pollingInfo))
			{
				return;
			}
			ElementAssignment elementAssignment = this.pendingInputMapping.ToElementAssignment(pollingInfo, modifierKeyFlags);
			if (!this.HasElementAssignmentConflicts(this.currentPlayer, this.pendingInputMapping, elementAssignment, false))
			{
				this.pendingInputMapping.map.ReplaceOrCreateElementMap(elementAssignment);
				this.InputPollingStopped();
				this.CloseWindow(windowId);
				return;
			}
			this.InputPollingStopped();
			this.ShowElementAssignmentConflictWindow(elementAssignment, false);
		}

		// Token: 0x06002B24 RID: 11044 RVA: 0x00202014 File Offset: 0x00200214
		private void OnMouseElementAssignmentPollingWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingInputMapping == null)
			{
				return;
			}
			this.InputPollingStarted();
			if (window.timer.finished)
			{
				this.InputPollingStopped();
				this.CloseWindow(windowId);
				return;
			}
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			ControllerPollingInfo pollingInfo;
			if (this._ignoreMouseXAxisAssignment || this._ignoreMouseYAxisAssignment)
			{
				pollingInfo = default(ControllerPollingInfo);
				using (IEnumerator<ControllerPollingInfo> enumerator = ReInput.controllers.polling.PollControllerForAllElementsDown(1, 0).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ControllerPollingInfo controllerPollingInfo = enumerator.Current;
						if (controllerPollingInfo.elementType != null || ((!this._ignoreMouseXAxisAssignment || controllerPollingInfo.elementIndex != 0) && (!this._ignoreMouseYAxisAssignment || controllerPollingInfo.elementIndex != 1)))
						{
							pollingInfo = controllerPollingInfo;
							break;
						}
					}
					goto IL_F9;
				}
			}
			pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(1, 0);
			IL_F9:
			if (!pollingInfo.success)
			{
				return;
			}
			if (!this.IsAllowedAssignment(this.pendingInputMapping, pollingInfo))
			{
				return;
			}
			ElementAssignment elementAssignment = this.pendingInputMapping.ToElementAssignment(pollingInfo);
			if (!this.HasElementAssignmentConflicts(this.currentPlayer, this.pendingInputMapping, elementAssignment, true))
			{
				this.pendingInputMapping.map.ReplaceOrCreateElementMap(elementAssignment);
				this.InputPollingStopped();
				this.CloseWindow(windowId);
				return;
			}
			this.InputPollingStopped();
			this.ShowElementAssignmentConflictWindow(elementAssignment, true);
		}

		// Token: 0x06002B25 RID: 11045 RVA: 0x00202198 File Offset: 0x00200398
		private void OnCalibrateAxisStep1WindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingAxisCalibration == null || !this.pendingAxisCalibration.isValid)
			{
				return;
			}
			this.InputPollingStarted();
			if (!window.timer.finished)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				if (this.currentPlayer.controllers.joystickCount == 0)
				{
					return;
				}
				if (!this.pendingAxisCalibration.joystick.PollForFirstButtonDown().success)
				{
					return;
				}
			}
			this.pendingAxisCalibration.RecordZero();
			this.CloseWindow(windowId);
			this.ShowCalibrateAxisStep2Window();
		}

		// Token: 0x06002B26 RID: 11046 RVA: 0x00202250 File Offset: 0x00200450
		private void OnCalibrateAxisStep2WindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingAxisCalibration == null || !this.pendingAxisCalibration.isValid)
			{
				return;
			}
			if (!window.timer.finished)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				this.pendingAxisCalibration.RecordMinMax();
				if (this.currentPlayer.controllers.joystickCount == 0)
				{
					return;
				}
				if (!this.pendingAxisCalibration.joystick.PollForFirstButtonDown().success)
				{
					return;
				}
			}
			this.EndAxisCalibration();
			this.InputPollingStopped();
			this.CloseWindow(windowId);
		}

		// Token: 0x06002B27 RID: 11047 RVA: 0x00202308 File Offset: 0x00200508
		private void ShowAssignControllerWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (ReInput.controllers.joystickCount == 0)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.SetUpdateCallback(new Action<int>(this.OnAssignControllerWindowUpdate));
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.assignControllerWindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.assignControllerWindowMessage);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.timer.Start(this._controllerAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B28 RID: 11048 RVA: 0x002023E8 File Offset: 0x002005E8
		private void ShowControllerAssignmentConflictWindow(int controllerId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (ReInput.controllers.joystickCount == 0)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			string otherPlayerName = string.Empty;
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i] != this.currentPlayer && players[i].controllers.ContainsController(2, controllerId))
				{
					otherPlayerName = this._language.GetPlayerName(players[i].id);
					break;
				}
			}
			Joystick joystick = ReInput.controllers.GetJoystick(controllerId);
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.controllerAssignmentConflictWindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetControllerAssignmentConflictWindowMessage(this._language.GetControllerName(joystick), otherPlayerName, this._language.GetPlayerName(this.currentPlayer.id)));
			UnityAction unityAction = delegate()
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, this._language.yes, delegate()
			{
				this.OnControllerAssignmentConfirmed(window.id, this.currentPlayer, controllerId);
			}, unityAction, true);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, this._language.no, unityAction, unityAction, false);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B29 RID: 11049 RVA: 0x002025DC File Offset: 0x002007DC
		private void ShowBeginElementAssignmentReplacementWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, ActionElementMap aem, string actionName)
		{
			ControlMapper.GUIInputField guiinputField = this.inputGrid.GetGUIInputField(this.currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData);
			if (guiinputField == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), guiinputField.GetLabel());
			UnityAction unityAction = delegate()
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, this._language.replace, delegate()
			{
				this.OnBeginElementAssignment(fieldInfo, map, aem, actionName);
			}, unityAction, true);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, this._language.remove, delegate()
			{
				this.OnRemoveElementAssignment(window.id, map, aem);
			}, unityAction, false);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, this._language.cancel, unityAction, unityAction, false);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B2A RID: 11050 RVA: 0x0020279C File Offset: 0x0020099C
		private void ShowCreateNewElementAssignmentWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, string actionName)
		{
			if (this.inputGrid.GetGUIInputField(this.currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData) == null)
			{
				return;
			}
			this.OnBeginElementAssignment(fieldInfo, map, null, actionName);
		}

		// Token: 0x06002B2B RID: 11051 RVA: 0x002027D8 File Offset: 0x002009D8
		private void ShowElementAssignmentPrePollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this.pendingInputMapping.actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.elementAssignmentPrePollingWindowMessage);
			if (this.prefabs.centerStickGraphic != null)
			{
				window.AddContentImage(this.prefabs.centerStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(new Action<int>(this.OnElementAssignmentPrePollingWindowUpdate));
			window.timer.Start(this._preInputAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B2C RID: 11052 RVA: 0x002028E8 File Offset: 0x00200AE8
		private void ShowElementAssignmentPollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			switch (this.pendingInputMapping.controllerType)
			{
			case 0:
				this.ShowKeyboardElementAssignmentPollingWindow();
				return;
			case 1:
				if (this.currentPlayer.controllers.hasMouse)
				{
					this.ShowMouseElementAssignmentPollingWindow();
					return;
				}
				this.ShowMouseAssignmentConflictWindow();
				return;
			case 2:
				this.ShowJoystickElementAssignmentPollingWindow();
				return;
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x06002B2D RID: 11053 RVA: 0x00202954 File Offset: 0x00200B54
		private void ShowJoystickElementAssignmentPollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			string text = (this.pendingInputMapping.axisRange == null && this._showFullAxisInputFields && !this._showSplitAxisInputFields) ? this._language.GetJoystickElementAssignmentPollingWindowMessage_FullAxisFieldOnly(this.pendingInputMapping.actionName) : this._language.GetJoystickElementAssignmentPollingWindowMessage(this.pendingInputMapping.actionName);
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this.pendingInputMapping.actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), text);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(new Action<int>(this.OnJoystickElementAssignmentPollingWindowUpdate));
			window.timer.Start(this._inputAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B2E RID: 11054 RVA: 0x00202A68 File Offset: 0x00200C68
		private void ShowKeyboardElementAssignmentPollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this.pendingInputMapping.actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetKeyboardElementAssignmentPollingWindowMessage(this.pendingInputMapping.actionName));
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -(window.GetContentTextHeight(0) + 50f)), "");
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(new Action<int>(this.OnKeyboardElementAssignmentPollingWindowUpdate));
			window.timer.Start(this._inputAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B2F RID: 11055 RVA: 0x00202B80 File Offset: 0x00200D80
		private void ShowMouseElementAssignmentPollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			string text = (this.pendingInputMapping.axisRange == null && this._showFullAxisInputFields && !this._showSplitAxisInputFields) ? this._language.GetMouseElementAssignmentPollingWindowMessage_FullAxisFieldOnly(this.pendingInputMapping.actionName) : this._language.GetMouseElementAssignmentPollingWindowMessage(this.pendingInputMapping.actionName);
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this.pendingInputMapping.actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), text);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(new Action<int>(this.OnMouseElementAssignmentPollingWindowUpdate));
			window.timer.Start(this._inputAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B30 RID: 11056 RVA: 0x00202C94 File Offset: 0x00200E94
		private void ShowElementAssignmentConflictWindow(ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			bool flag = this.IsBlockingAssignmentConflict(this.pendingInputMapping, assignment, skipOtherPlayers);
			string text = flag ? this._language.GetElementAlreadyInUseBlocked(this.pendingInputMapping.elementName) : this._language.GetElementAlreadyInUseCanReplace(this.pendingInputMapping.elementName, this._allowElementAssignmentConflicts);
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.elementAssignmentConflictWindowMessage);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), text);
			UnityAction unityAction = delegate()
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			if (flag)
			{
				window.CreateButton(this.prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, this._language.okay, unityAction, unityAction, true);
			}
			else
			{
				window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, this._language.replace, delegate()
				{
					this.OnElementAssignmentConflictReplaceConfirmed(window.id, this.pendingInputMapping, assignment, skipOtherPlayers, false);
				}, unityAction, true);
				if (this._allowElementAssignmentConflicts)
				{
					window.CreateButton(this.prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, this._language.add, delegate()
					{
						this.OnElementAssignmentAddConfirmed(window.id, this.pendingInputMapping, assignment);
					}, unityAction, false);
				}
				else if (this.ShowSwapButton(window.id, this.pendingInputMapping, assignment, skipOtherPlayers))
				{
					window.CreateButton(this.prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, this._language.swap, delegate()
					{
						this.OnElementAssignmentConflictReplaceConfirmed(window.id, this.pendingInputMapping, assignment, skipOtherPlayers, true);
					}, unityAction, false);
				}
				window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, this._language.cancel, unityAction, unityAction, false);
			}
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x00202F0C File Offset: 0x0020110C
		private void ShowMouseAssignmentConflictWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			string otherPlayerName = string.Empty;
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i] != this.currentPlayer && players[i].controllers.hasMouse)
				{
					otherPlayerName = this._language.GetPlayerName(players[i].id);
					break;
				}
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.mouseAssignmentConflictWindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetMouseAssignmentConflictWindowMessage(otherPlayerName, this._language.GetPlayerName(this.currentPlayer.id)));
			UnityAction unityAction = delegate()
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, this._language.yes, delegate()
			{
				this.OnMouseAssignmentConfirmed(window.id, this.currentPlayer);
			}, unityAction, true);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, this._language.no, unityAction, unityAction, false);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B32 RID: 11058 RVA: 0x002030C4 File Offset: 0x002012C4
		private void ShowCalibrateControllerWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (this.currentPlayer.controllers.joystickCount == 0)
			{
				return;
			}
			CalibrationWindow calibrationWindow = this.OpenWindow(this.prefabs.calibrationWindow, "CalibrationWindow", true) as CalibrationWindow;
			if (calibrationWindow == null)
			{
				return;
			}
			Joystick currentJoystick = this.currentJoystick;
			calibrationWindow.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.calibrateControllerWindowTitle);
			calibrationWindow.SetJoystick(this.currentPlayer.id, currentJoystick);
			calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Done, new Action<int>(this.CloseWindow));
			calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Calibrate, new Action<int>(this.StartAxisCalibration));
			calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Cancel, new Action<int>(this.CloseWindow));
			this.windowManager.Focus(calibrationWindow);
		}

		// Token: 0x06002B33 RID: 11059 RVA: 0x00203194 File Offset: 0x00201394
		private void ShowCalibrateAxisStep1Window()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.OpenWindow(false);
			if (window == null)
			{
				return;
			}
			if (this.pendingAxisCalibration == null)
			{
				return;
			}
			Joystick joystick = this.pendingAxisCalibration.joystick;
			if (joystick.axisCount == 0)
			{
				return;
			}
			int axisIndex = this.pendingAxisCalibration.axisIndex;
			if (axisIndex < 0 || axisIndex >= joystick.axisCount)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.calibrateAxisStep1WindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetCalibrateAxisStep1WindowMessage(this._language.GetElementIdentifierName(joystick, joystick.AxisElementIdentifiers[axisIndex].id, 0)));
			if (this.prefabs.centerStickGraphic != null)
			{
				window.AddContentImage(this.prefabs.centerStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(new Action<int>(this.OnCalibrateAxisStep1WindowUpdate));
			window.timer.Start(this._axisCalibrationTimeout);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x002032FC File Offset: 0x002014FC
		private void ShowCalibrateAxisStep2Window()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.OpenWindow(false);
			if (window == null)
			{
				return;
			}
			if (this.pendingAxisCalibration == null)
			{
				return;
			}
			Joystick joystick = this.pendingAxisCalibration.joystick;
			if (joystick.axisCount == 0)
			{
				return;
			}
			int axisIndex = this.pendingAxisCalibration.axisIndex;
			if (axisIndex < 0 || axisIndex >= joystick.axisCount)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.calibrateAxisStep2WindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetCalibrateAxisStep2WindowMessage(this._language.GetElementIdentifierName(joystick, joystick.AxisElementIdentifiers[axisIndex].id, 0)));
			if (this.prefabs.moveStickGraphic != null)
			{
				window.AddContentImage(this.prefabs.moveStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(new Action<int>(this.OnCalibrateAxisStep2WindowUpdate));
			window.timer.Start(this._axisCalibrationTimeout);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B35 RID: 11061 RVA: 0x00203464 File Offset: 0x00201664
		private void ShowEditInputBehaviorsWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (this._inputBehaviorSettings == null)
			{
				return;
			}
			InputBehaviorWindow inputBehaviorWindow = this.OpenWindow(this.prefabs.inputBehaviorsWindow, "EditInputBehaviorsWindow", true) as InputBehaviorWindow;
			if (inputBehaviorWindow == null)
			{
				return;
			}
			inputBehaviorWindow.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.inputBehaviorSettingsWindowTitle);
			inputBehaviorWindow.SetData(this.currentPlayer.id, this._inputBehaviorSettings);
			inputBehaviorWindow.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Done, new Action<int>(this.CloseWindow));
			inputBehaviorWindow.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Cancel, new Action<int>(this.CloseWindow));
			this.windowManager.Focus(inputBehaviorWindow);
		}

		// Token: 0x06002B36 RID: 11062 RVA: 0x00203514 File Offset: 0x00201714
		private void ShowRestoreDefaultsWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			this.OpenModal(this._language.restoreDefaultsWindowTitle, this._language.restoreDefaultsWindowMessage, this._language.yes, new Action<int>(this.OnRestoreDefaultsConfirmed), this._language.no, new Action<int>(this.OnWindowCancel), true);
		}

		// Token: 0x06002B37 RID: 11063 RVA: 0x00203578 File Offset: 0x00201778
		private void CreateInputGrid()
		{
			this.InitializeInputGrid();
			this.CreateHeaderLabels();
			this.CreateActionLabelColumn();
			this.CreateKeyboardInputFieldColumn();
			this.CreateMouseInputFieldColumn();
			this.CreateControllerInputFieldColumn();
			this.CreateInputActionLabels();
			this.CreateInputFields();
			this.inputGrid.HideAll();
			this.ResetInputGridScrollBar();
		}

		// Token: 0x06002B38 RID: 11064 RVA: 0x002035C8 File Offset: 0x002017C8
		private void InitializeInputGrid()
		{
			if (this.inputGrid == null)
			{
				this.inputGrid = new ControlMapper.InputGrid();
			}
			else
			{
				this.inputGrid.ClearAll();
			}
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				ControlMapper.MappingSet mappingSet = this._mappingSets[i];
				if (mappingSet != null && mappingSet.isValid)
				{
					InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(mappingSet.mapCategoryId);
					if (mapCategory != null && mapCategory.userAssignable)
					{
						this.inputGrid.AddMapCategory(mappingSet.mapCategoryId);
						if (mappingSet.actionListMode == ControlMapper.MappingSet.ActionListMode.ActionCategory)
						{
							IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
							for (int j = 0; j < actionCategoryIds.Count; j++)
							{
								int num = actionCategoryIds[j];
								InputCategory actionCategory = ReInput.mapping.GetActionCategory(num);
								if (actionCategory != null && actionCategory.userAssignable)
								{
									this.inputGrid.AddActionCategory(mappingSet.mapCategoryId, num);
									foreach (InputAction inputAction in ReInput.mapping.UserAssignableActionsInCategory(num))
									{
										if (inputAction.type == null)
										{
											if (this._showFullAxisInputFields)
											{
												this.inputGrid.AddAction(mappingSet.mapCategoryId, inputAction, 0);
											}
											if (this._showSplitAxisInputFields)
											{
												this.inputGrid.AddAction(mappingSet.mapCategoryId, inputAction, 1);
												this.inputGrid.AddAction(mappingSet.mapCategoryId, inputAction, 2);
											}
										}
										else if (inputAction.type == 1)
										{
											this.inputGrid.AddAction(mappingSet.mapCategoryId, inputAction, 1);
										}
									}
								}
							}
						}
						else
						{
							IList<int> actionIds = mappingSet.actionIds;
							for (int k = 0; k < actionIds.Count; k++)
							{
								InputAction action = ReInput.mapping.GetAction(actionIds[k]);
								if (action != null)
								{
									if (action.type == null)
									{
										if (this._showFullAxisInputFields)
										{
											this.inputGrid.AddAction(mappingSet.mapCategoryId, action, 0);
										}
										if (this._showSplitAxisInputFields)
										{
											this.inputGrid.AddAction(mappingSet.mapCategoryId, action, 1);
											this.inputGrid.AddAction(mappingSet.mapCategoryId, action, 2);
										}
									}
									else if (action.type == 1)
									{
										this.inputGrid.AddAction(mappingSet.mapCategoryId, action, 1);
									}
								}
							}
						}
					}
				}
			}
			this.references.inputGridInnerGroup.GetComponent<HorizontalLayoutGroup>().spacing = (float)this._inputColumnSpacing;
			this.references.inputGridLayoutElement.flexibleWidth = 0f;
			this.references.inputGridLayoutElement.preferredWidth = (float)this.inputGridWidth;
		}

		// Token: 0x06002B39 RID: 11065 RVA: 0x00203884 File Offset: 0x00201A84
		private void RefreshInputGridStructure()
		{
			if (this.currentMappingSet == null)
			{
				return;
			}
			this.inputGrid.HideAll();
			this.inputGrid.Show(this.currentMappingSet.mapCategoryId);
			this.references.inputGridInnerGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(1, this.inputGrid.GetColumnHeight(this.currentMappingSet.mapCategoryId));
		}

		// Token: 0x06002B3A RID: 11066 RVA: 0x002038E8 File Offset: 0x00201AE8
		private void CreateHeaderLabels()
		{
			this.references.inputGridHeader1 = this.CreateNewColumnGroup("ActionsHeader", this.references.inputGridHeadersGroup, this._actionLabelWidth).transform;
			this.CreateLabel(this.prefabs.inputGridHeaderLabel, this._language.actionColumnLabel, this.references.inputGridHeader1, Vector2.zero);
			if (this._showKeyboard)
			{
				this.references.inputGridHeader2 = this.CreateNewColumnGroup("KeybordHeader", this.references.inputGridHeadersGroup, this._keyboardColMaxWidth).transform;
				this.CreateLabel(this.prefabs.inputGridHeaderLabel, this._language.keyboardColumnLabel, this.references.inputGridHeader2, Vector2.zero).SetTextAlignment(514);
			}
			if (this._showMouse)
			{
				this.references.inputGridHeader3 = this.CreateNewColumnGroup("MouseHeader", this.references.inputGridHeadersGroup, this._mouseColMaxWidth).transform;
				this.CreateLabel(this.prefabs.inputGridHeaderLabel, this._language.mouseColumnLabel, this.references.inputGridHeader3, Vector2.zero).SetTextAlignment(514);
			}
			if (this._showControllers)
			{
				this.references.inputGridHeader4 = this.CreateNewColumnGroup("ControllerHeader", this.references.inputGridHeadersGroup, this._controllerColMaxWidth).transform;
				this.CreateLabel(this.prefabs.inputGridHeaderLabel, this._language.controllerColumnLabel, this.references.inputGridHeader4, Vector2.zero).SetTextAlignment(514);
			}
		}

		// Token: 0x06002B3B RID: 11067 RVA: 0x00203A8C File Offset: 0x00201C8C
		private void CreateActionLabelColumn()
		{
			Transform transform = this.CreateNewColumnGroup("ActionLabelColumn", this.references.inputGridInnerGroup, this._actionLabelWidth).transform;
			this.references.inputGridActionColumn = transform;
		}

		// Token: 0x06002B3C RID: 11068 RVA: 0x00203AC7 File Offset: 0x00201CC7
		private void CreateKeyboardInputFieldColumn()
		{
			if (!this._showKeyboard)
			{
				return;
			}
			this.CreateInputFieldColumn("KeyboardColumn", 0, this._keyboardColMaxWidth, this._keyboardInputFieldCount, true);
		}

		// Token: 0x06002B3D RID: 11069 RVA: 0x00203AEB File Offset: 0x00201CEB
		private void CreateMouseInputFieldColumn()
		{
			if (!this._showMouse)
			{
				return;
			}
			this.CreateInputFieldColumn("MouseColumn", 1, this._mouseColMaxWidth, this._mouseInputFieldCount, false);
		}

		// Token: 0x06002B3E RID: 11070 RVA: 0x00203B0F File Offset: 0x00201D0F
		private void CreateControllerInputFieldColumn()
		{
			if (!this._showControllers)
			{
				return;
			}
			this.CreateInputFieldColumn("ControllerColumn", 2, this._controllerColMaxWidth, this._controllerInputFieldCount, false);
		}

		// Token: 0x06002B3F RID: 11071 RVA: 0x00203B34 File Offset: 0x00201D34
		private void CreateInputFieldColumn(string name, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
		{
			Transform transform = this.CreateNewColumnGroup(name, this.references.inputGridInnerGroup, maxWidth).transform;
			switch (controllerType)
			{
			case 0:
				this.references.inputGridKeyboardColumn = transform;
				return;
			case 1:
				this.references.inputGridMouseColumn = transform;
				return;
			case 2:
				this.references.inputGridControllerColumn = transform;
				return;
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x06002B40 RID: 11072 RVA: 0x00203B9C File Offset: 0x00201D9C
		private void CreateInputActionLabels()
		{
			Transform inputGridActionColumn = this.references.inputGridActionColumn;
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				ControlMapper.MappingSet mappingSet = this._mappingSets[i];
				if (mappingSet != null && mappingSet.isValid)
				{
					int num = 0;
					if (mappingSet.actionListMode == ControlMapper.MappingSet.ActionListMode.ActionCategory)
					{
						int num2 = 0;
						IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
						for (int j = 0; j < actionCategoryIds.Count; j++)
						{
							InputCategory actionCategory = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
							if (actionCategory != null && actionCategory.userAssignable && this.CountIEnumerable<InputAction>(ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id)) != 0)
							{
								if (this._showActionCategoryLabels)
								{
									if (num2 > 0)
									{
										num -= this._inputRowCategorySpacing;
									}
									ControlMapper.GUILabel guilabel = this.CreateLabel(this._language.GetActionCategoryName(actionCategory.id), inputGridActionColumn, new Vector2(0f, (float)num));
									guilabel.SetFontStyle(1);
									guilabel.rectTransform.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
									this.inputGrid.AddActionCategoryLabel(mappingSet.mapCategoryId, actionCategory.id, guilabel);
									num -= this._inputRowHeight;
								}
								foreach (InputAction inputAction in ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id, true))
								{
									if (inputAction.type == null)
									{
										if (this._showFullAxisInputFields)
										{
											ControlMapper.GUILabel guilabel2 = this.CreateLabel(this._language.GetActionName(inputAction.id, 0), inputGridActionColumn, new Vector2(0f, (float)num));
											guilabel2.rectTransform.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
											this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, inputAction.id, 0, guilabel2);
											num -= this._inputRowHeight;
										}
										if (this._showSplitAxisInputFields)
										{
											string actionName = this._language.GetActionName(inputAction.id, 1);
											ControlMapper.GUILabel guilabel2 = this.CreateLabel(actionName, inputGridActionColumn, new Vector2(0f, (float)num));
											guilabel2.rectTransform.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
											this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, inputAction.id, 1, guilabel2);
											num -= this._inputRowHeight;
											string actionName2 = this._language.GetActionName(inputAction.id, 2);
											guilabel2 = this.CreateLabel(actionName2, inputGridActionColumn, new Vector2(0f, (float)num));
											guilabel2.rectTransform.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
											this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, inputAction.id, 2, guilabel2);
											num -= this._inputRowHeight;
										}
									}
									else if (inputAction.type == 1)
									{
										ControlMapper.GUILabel guilabel2 = this.CreateLabel(this._language.GetActionName(inputAction.id), inputGridActionColumn, new Vector2(0f, (float)num));
										guilabel2.rectTransform.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
										this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, inputAction.id, 1, guilabel2);
										num -= this._inputRowHeight;
									}
								}
								num2++;
							}
						}
					}
					else
					{
						IList<int> actionIds = mappingSet.actionIds;
						for (int k = 0; k < actionIds.Count; k++)
						{
							InputAction action = ReInput.mapping.GetAction(actionIds[k]);
							if (action != null && action.userAssignable)
							{
								InputCategory actionCategory2 = ReInput.mapping.GetActionCategory(action.categoryId);
								if (actionCategory2 != null && actionCategory2.userAssignable)
								{
									if (action.type == null)
									{
										if (this._showFullAxisInputFields)
										{
											ControlMapper.GUILabel guilabel3 = this.CreateLabel(this._language.GetActionName(action.id, 0), inputGridActionColumn, new Vector2(0f, (float)num));
											guilabel3.rectTransform.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
											this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, 0, guilabel3);
											num -= this._inputRowHeight;
										}
										if (this._showSplitAxisInputFields)
										{
											ControlMapper.GUILabel guilabel3 = this.CreateLabel(this._language.GetActionName(action.id, 1), inputGridActionColumn, new Vector2(0f, (float)num));
											guilabel3.rectTransform.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
											this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, 1, guilabel3);
											num -= this._inputRowHeight;
											guilabel3 = this.CreateLabel(this._language.GetActionName(action.id, 2), inputGridActionColumn, new Vector2(0f, (float)num));
											guilabel3.rectTransform.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
											this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, 2, guilabel3);
											num -= this._inputRowHeight;
										}
									}
									else if (action.type == 1)
									{
										ControlMapper.GUILabel guilabel3 = this.CreateLabel(this._language.GetActionName(action.id), inputGridActionColumn, new Vector2(0f, (float)num));
										guilabel3.rectTransform.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
										this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, 1, guilabel3);
										num -= this._inputRowHeight;
									}
								}
							}
						}
					}
					this.inputGrid.SetColumnHeight(mappingSet.mapCategoryId, (float)(-(float)num));
				}
			}
		}

		// Token: 0x06002B41 RID: 11073 RVA: 0x00204128 File Offset: 0x00202328
		private void CreateInputFields()
		{
			if (this._showControllers)
			{
				this.CreateInputFields(this.references.inputGridControllerColumn, 2, this._controllerColMaxWidth, this._controllerInputFieldCount, false);
			}
			if (this._showKeyboard)
			{
				this.CreateInputFields(this.references.inputGridKeyboardColumn, 0, this._keyboardColMaxWidth, this._keyboardInputFieldCount, true);
			}
			if (this._showMouse)
			{
				this.CreateInputFields(this.references.inputGridMouseColumn, 1, this._mouseColMaxWidth, this._mouseInputFieldCount, false);
			}
		}

		// Token: 0x06002B42 RID: 11074 RVA: 0x002041AC File Offset: 0x002023AC
		private void CreateInputFields(Transform columnXform, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
		{
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				ControlMapper.MappingSet mappingSet = this._mappingSets[i];
				if (mappingSet != null && mappingSet.isValid)
				{
					int fieldWidth = maxWidth / cols;
					int num = 0;
					int num2 = 0;
					if (mappingSet.actionListMode == ControlMapper.MappingSet.ActionListMode.ActionCategory)
					{
						IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
						for (int j = 0; j < actionCategoryIds.Count; j++)
						{
							InputCategory actionCategory = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
							if (actionCategory != null && actionCategory.userAssignable && this.CountIEnumerable<InputAction>(ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id)) != 0)
							{
								if (this._showActionCategoryLabels)
								{
									num -= ((num2 > 0) ? (this._inputRowHeight + this._inputRowCategorySpacing) : this._inputRowHeight);
								}
								foreach (InputAction inputAction in ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id, true))
								{
									if (inputAction.type == null)
									{
										if (this._showFullAxisInputFields)
										{
											this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, inputAction, 0, controllerType, cols, fieldWidth, ref num, disableFullAxis);
										}
										if (this._showSplitAxisInputFields)
										{
											this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, inputAction, 1, controllerType, cols, fieldWidth, ref num, false);
											this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, inputAction, 2, controllerType, cols, fieldWidth, ref num, false);
										}
									}
									else if (inputAction.type == 1)
									{
										this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, inputAction, 1, controllerType, cols, fieldWidth, ref num, false);
									}
									num2++;
								}
							}
						}
					}
					else
					{
						IList<int> actionIds = mappingSet.actionIds;
						for (int k = 0; k < actionIds.Count; k++)
						{
							InputAction action = ReInput.mapping.GetAction(actionIds[k]);
							if (action != null && action.userAssignable)
							{
								InputCategory actionCategory2 = ReInput.mapping.GetActionCategory(action.categoryId);
								if (actionCategory2 != null && actionCategory2.userAssignable)
								{
									if (action.type == null)
									{
										if (this._showFullAxisInputFields)
										{
											this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, 0, controllerType, cols, fieldWidth, ref num, disableFullAxis);
										}
										if (this._showSplitAxisInputFields)
										{
											this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, 1, controllerType, cols, fieldWidth, ref num, false);
											this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, 2, controllerType, cols, fieldWidth, ref num, false);
										}
									}
									else if (action.type == 1)
									{
										this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, 1, controllerType, cols, fieldWidth, ref num, false);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002B43 RID: 11075 RVA: 0x00204460 File Offset: 0x00202660
		private void CreateInputFieldSet(Transform parent, int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int cols, int fieldWidth, ref int yPos, bool disableFullAxis)
		{
			GameObject gameObject = this.CreateNewGUIObject("FieldLayoutGroup", parent, new Vector2(0f, (float)yPos));
			HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
			horizontalLayoutGroup.padding = this._inputRowPadding;
			horizontalLayoutGroup.spacing = (float)this._inputRowFieldSpacing;
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = new Vector2(1f, 1f);
			component.pivot = new Vector2(0f, 1f);
			component.sizeDelta = Vector2.zero;
			component.SetSizeWithCurrentAnchors(1, (float)this._inputRowHeight);
			this.inputGrid.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, gameObject);
			for (int i = 0; i < cols; i++)
			{
				int num = (axisRange == null) ? this._invertToggleWidth : 0;
				ControlMapper.GUIInputField guiinputField = this.CreateInputField(horizontalLayoutGroup.transform, Vector2.zero, "", action.id, axisRange, controllerType, i);
				guiinputField.SetFirstChildObjectWidth(ControlMapper.LayoutElementSizeType.PreferredSize, fieldWidth - num);
				this.inputGrid.AddInputField(mapCategoryId, action, axisRange, controllerType, i, guiinputField);
				if (axisRange == null)
				{
					if (!disableFullAxis)
					{
						ControlMapper.GUIToggle guitoggle = this.CreateToggle(this.prefabs.inputGridFieldInvertToggle, horizontalLayoutGroup.transform, Vector2.zero, "", action.id, axisRange, controllerType, i);
						guitoggle.SetFirstChildObjectWidth(ControlMapper.LayoutElementSizeType.MinSize, num);
						guiinputField.AddToggle(guitoggle);
					}
					else
					{
						guiinputField.SetInteractible(false, false, true);
					}
				}
			}
			yPos -= this._inputRowHeight;
		}

		// Token: 0x06002B44 RID: 11076 RVA: 0x002045DC File Offset: 0x002027DC
		private void PopulateInputFields()
		{
			this.inputGrid.InitializeFields(this.currentMapCategoryId);
			if (this.currentPlayer == null)
			{
				return;
			}
			this.inputGrid.SetFieldsActive(this.currentMapCategoryId, true);
			foreach (ControlMapper.InputActionSet actionSet in this.inputGrid.GetActionSets(this.currentMapCategoryId))
			{
				if (this._showKeyboard)
				{
					ControllerType controllerType = 0;
					int controllerId = 0;
					int layoutId = this._keyboardMapDefaultLayout;
					int maxFields = this._keyboardInputFieldCount;
					ControllerMap controllerMapOrCreateNew = this.GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
					this.PopulateInputFieldGroup(actionSet, controllerMapOrCreateNew, controllerType, controllerId, maxFields);
				}
				if (this._showMouse)
				{
					ControllerType controllerType = 1;
					int controllerId = 0;
					int layoutId = this._mouseMapDefaultLayout;
					int maxFields = this._mouseInputFieldCount;
					ControllerMap controllerMapOrCreateNew2 = this.GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
					if (this.currentPlayer.controllers.hasMouse)
					{
						this.PopulateInputFieldGroup(actionSet, controllerMapOrCreateNew2, controllerType, controllerId, maxFields);
					}
				}
				if (this.isJoystickSelected && this.currentPlayer.controllers.joystickCount > 0)
				{
					ControllerType controllerType = 2;
					int controllerId = this.currentJoystick.id;
					int layoutId = this._joystickMapDefaultLayout;
					int maxFields = this._controllerInputFieldCount;
					ControllerMap controllerMapOrCreateNew3 = this.GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
					this.PopulateInputFieldGroup(actionSet, controllerMapOrCreateNew3, controllerType, controllerId, maxFields);
				}
				else
				{
					this.DisableInputFieldGroup(actionSet, 2, this._controllerInputFieldCount);
				}
			}
		}

		// Token: 0x06002B45 RID: 11077 RVA: 0x00204740 File Offset: 0x00202940
		private void PopulateInputFieldGroup(ControlMapper.InputActionSet actionSet, ControllerMap controllerMap, ControllerType controllerType, int controllerId, int maxFields)
		{
			if (controllerMap == null)
			{
				return;
			}
			int num = 0;
			this.inputGrid.SetFixedFieldData(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId);
			foreach (ActionElementMap actionElementMap in controllerMap.ElementMapsWithAction(actionSet.actionId))
			{
				if (actionElementMap.elementType == 1)
				{
					if (actionSet.axisRange == null)
					{
						continue;
					}
					if (actionSet.axisRange == 1)
					{
						if (actionElementMap.axisContribution == 1)
						{
							continue;
						}
					}
					else if (actionSet.axisRange == 2 && actionElementMap.axisContribution == null)
					{
						continue;
					}
					this.inputGrid.PopulateField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, actionElementMap.id, this._language.GetElementIdentifierName(actionElementMap), false);
				}
				else if (actionElementMap.elementType == null)
				{
					if (actionSet.axisRange == null)
					{
						if (actionElementMap.axisRange != null)
						{
							continue;
						}
						this.inputGrid.PopulateField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, actionElementMap.id, this._language.GetElementIdentifierName(actionElementMap), actionElementMap.invert);
					}
					else if (actionSet.axisRange == 1)
					{
						if ((actionElementMap.axisRange == null && ReInput.mapping.GetAction(actionSet.actionId).type != 1) || actionElementMap.axisContribution == 1)
						{
							continue;
						}
						this.inputGrid.PopulateField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, actionElementMap.id, this._language.GetElementIdentifierName(actionElementMap), false);
					}
					else if (actionSet.axisRange == 2)
					{
						if (actionElementMap.axisRange == null || actionElementMap.axisContribution == null)
						{
							continue;
						}
						this.inputGrid.PopulateField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, actionElementMap.id, this._language.GetElementIdentifierName(actionElementMap), false);
					}
				}
				num++;
				if (num > maxFields)
				{
					break;
				}
			}
		}

		// Token: 0x06002B46 RID: 11078 RVA: 0x00204960 File Offset: 0x00202B60
		private void DisableInputFieldGroup(ControlMapper.InputActionSet actionSet, ControllerType controllerType, int fieldCount)
		{
			for (int i = 0; i < fieldCount; i++)
			{
				ControlMapper.GUIInputField guiinputField = this.inputGrid.GetGUIInputField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, i);
				if (guiinputField != null)
				{
					guiinputField.SetInteractible(false, false);
				}
			}
		}

		// Token: 0x06002B47 RID: 11079 RVA: 0x002049A4 File Offset: 0x00202BA4
		private void ResetInputGridScrollBar()
		{
			this.references.inputGridInnerGroup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			this.references.inputGridVScrollbar.value = 1f;
			this.references.inputGridScrollRect.verticalScrollbarVisibility = 1;
		}

		// Token: 0x06002B48 RID: 11080 RVA: 0x002049F4 File Offset: 0x00202BF4
		private void CreateLayout()
		{
			this.references.playersGroup.gameObject.SetActive(this.showPlayers);
			this.references.controllerGroup.gameObject.SetActive(this._showControllers);
			this.references.assignedControllersGroup.gameObject.SetActive(this._showControllers && this.ShowAssignedControllers());
			this.references.settingsAndMapCategoriesGroup.gameObject.SetActive(this.showSettings || this.showMapCategories);
			this.references.settingsGroup.gameObject.SetActive(this.showSettings);
			this.references.mapCategoriesGroup.gameObject.SetActive(this.showMapCategories);
		}

		// Token: 0x06002B49 RID: 11081 RVA: 0x00204AB9 File Offset: 0x00202CB9
		private void Draw()
		{
			this.DrawPlayersGroup();
			this.DrawControllersGroup();
			this.DrawSettingsGroup();
			this.DrawMapCategoriesGroup();
			this.DrawWindowButtonsGroup();
		}

		// Token: 0x06002B4A RID: 11082 RVA: 0x00204ADC File Offset: 0x00202CDC
		private void DrawPlayersGroup()
		{
			if (!this.showPlayers)
			{
				return;
			}
			this.references.playersGroup.labelText = this._language.playersGroupLabel;
			this.references.playersGroup.SetLabelActive(this._showPlayersGroupLabel);
			for (int i = 0; i < this.playerCount; i++)
			{
				Player player = ReInput.players.GetPlayer(i);
				if (player != null)
				{
					ControlMapper.GUIButton guibutton = new ControlMapper.GUIButton(UITools.InstantiateGUIObject<ButtonInfo>(this.prefabs.button, this.references.playersGroup.content, "Player" + i.ToString() + "Button"));
					guibutton.SetLabel(this._language.GetPlayerName(player.id));
					guibutton.SetButtonInfoData("PlayerSelection", player.id);
					guibutton.SetOnClickCallback(new Action<ButtonInfo>(this.OnButtonActivated));
					guibutton.buttonInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
					this.playerButtons.Add(guibutton);
				}
			}
		}

		// Token: 0x06002B4B RID: 11083 RVA: 0x00204BE8 File Offset: 0x00202DE8
		private void DrawControllersGroup()
		{
			if (!this._showControllers)
			{
				return;
			}
			this.references.controllerSettingsGroup.labelText = this._language.controllerSettingsGroupLabel;
			this.references.controllerSettingsGroup.SetLabelActive(this._showControllerGroupLabel);
			this.references.controllerNameLabel.gameObject.SetActive(this._showControllerNameLabel);
			this.references.controllerGroupLabelGroup.gameObject.SetActive(this._showControllerGroupLabel || this._showControllerNameLabel);
			if (this.ShowAssignedControllers())
			{
				this.references.assignedControllersGroup.labelText = this._language.assignedControllersGroupLabel;
				this.references.assignedControllersGroup.SetLabelActive(this._showAssignedControllersGroupLabel);
			}
			this.references.removeControllerButton.GetComponent<ButtonInfo>().text.text = this._language.removeControllerButtonLabel;
			this.references.calibrateControllerButton.GetComponent<ButtonInfo>().text.text = this._language.calibrateControllerButtonLabel;
			this.references.assignControllerButton.GetComponent<ButtonInfo>().text.text = this._language.assignControllerButtonLabel;
			ControlMapper.GUIButton guibutton = this.CreateButton(this._language.none, this.references.assignedControllersGroup.content, Vector2.zero);
			guibutton.SetInteractible(false, false, true);
			this.assignedControllerButtonsPlaceholder = guibutton;
		}

		// Token: 0x06002B4C RID: 11084 RVA: 0x00204D50 File Offset: 0x00202F50
		private void DrawSettingsGroup()
		{
			if (!this.showSettings)
			{
				return;
			}
			this.references.settingsGroup.labelText = this._language.settingsGroupLabel;
			this.references.settingsGroup.SetLabelActive(this._showSettingsGroupLabel);
			ControlMapper.GUIButton guibutton = this.CreateButton(this._language.inputBehaviorSettingsButtonLabel, this.references.settingsGroup.content, Vector2.zero);
			this.miscInstantiatedObjects.Add(guibutton.gameObject);
			guibutton.buttonInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
			guibutton.SetButtonInfoData("EditInputBehaviors", 0);
			guibutton.SetOnClickCallback(new Action<ButtonInfo>(this.OnButtonActivated));
		}

		// Token: 0x06002B4D RID: 11085 RVA: 0x00204E04 File Offset: 0x00203004
		private void DrawMapCategoriesGroup()
		{
			if (!this.showMapCategories)
			{
				return;
			}
			if (this._mappingSets == null)
			{
				return;
			}
			this.references.mapCategoriesGroup.labelText = this._language.mapCategoriesGroupLabel;
			this.references.mapCategoriesGroup.SetLabelActive(this._showMapCategoriesGroupLabel);
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				ControlMapper.MappingSet mappingSet = this._mappingSets[i];
				if (mappingSet != null)
				{
					InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(mappingSet.mapCategoryId);
					if (mapCategory != null)
					{
						ControlMapper.GUIButton guibutton = new ControlMapper.GUIButton(UITools.InstantiateGUIObject<ButtonInfo>(this.prefabs.button, this.references.mapCategoriesGroup.content, mapCategory.name + "Button"));
						guibutton.SetLabel(this._language.GetMapCategoryName(mapCategory.id));
						guibutton.SetButtonInfoData("MapCategorySelection", mapCategory.id);
						guibutton.SetOnClickCallback(new Action<ButtonInfo>(this.OnButtonActivated));
						guibutton.buttonInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
						this.mapCategoryButtons.Add(guibutton);
					}
				}
			}
		}

		// Token: 0x06002B4E RID: 11086 RVA: 0x00204F28 File Offset: 0x00203128
		private void DrawWindowButtonsGroup()
		{
			this.references.doneButton.GetComponent<ButtonInfo>().text.text = this._language.doneButtonLabel;
			this.references.restoreDefaultsButton.GetComponent<ButtonInfo>().text.text = this._language.restoreDefaultsButtonLabel;
		}

		// Token: 0x06002B4F RID: 11087 RVA: 0x00204F7F File Offset: 0x0020317F
		private void Redraw(bool listsChanged, bool playTransitions)
		{
			this.RedrawPlayerGroup(playTransitions);
			this.RedrawControllerGroup();
			this.RedrawMapCategoriesGroup(playTransitions);
			this.RedrawInputGrid(listsChanged);
			if (this.currentUISelection == null || !this.currentUISelection.activeInHierarchy)
			{
				this.RestoreLastUISelection();
			}
		}

		// Token: 0x06002B50 RID: 11088 RVA: 0x00204FC0 File Offset: 0x002031C0
		private void RedrawPlayerGroup(bool playTransitions)
		{
			if (!this.showPlayers)
			{
				return;
			}
			for (int i = 0; i < this.playerButtons.Count; i++)
			{
				bool state = this.currentPlayerId != this.playerButtons[i].buttonInfo.intData;
				this.playerButtons[i].SetInteractible(state, playTransitions);
			}
		}

		// Token: 0x06002B51 RID: 11089 RVA: 0x00205024 File Offset: 0x00203224
		private void RedrawControllerGroup()
		{
			int num = -1;
			this.references.controllerNameLabel.text = this._language.none;
			UITools.SetInteractable(this.references.removeControllerButton, false, false);
			UITools.SetInteractable(this.references.assignControllerButton, false, false);
			UITools.SetInteractable(this.references.calibrateControllerButton, false, false);
			if (this.ShowAssignedControllers())
			{
				foreach (ControlMapper.GUIButton guibutton in this.assignedControllerButtons)
				{
					if (!(guibutton.gameObject == null))
					{
						if (this.currentUISelection == guibutton.gameObject)
						{
							num = guibutton.buttonInfo.intData;
						}
						Object.Destroy(guibutton.gameObject);
					}
				}
				this.assignedControllerButtons.Clear();
				this.assignedControllerButtonsPlaceholder.SetActive(true);
			}
			Player player = ReInput.players.GetPlayer(this.currentPlayerId);
			if (player == null)
			{
				return;
			}
			if (this.ShowAssignedControllers())
			{
				if (player.controllers.joystickCount > 0)
				{
					this.assignedControllerButtonsPlaceholder.SetActive(false);
				}
				foreach (Joystick joystick in player.controllers.Joysticks)
				{
					ControlMapper.GUIButton guibutton2 = this.CreateButton(this._language.GetControllerName(joystick), this.references.assignedControllersGroup.content, Vector2.zero);
					guibutton2.SetButtonInfoData("AssignedControllerSelection", joystick.id);
					guibutton2.SetOnClickCallback(new Action<ButtonInfo>(this.OnButtonActivated));
					guibutton2.buttonInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
					this.assignedControllerButtons.Add(guibutton2);
					if (joystick.id == this.currentJoystickId)
					{
						guibutton2.SetInteractible(false, true);
					}
				}
				if (player.controllers.joystickCount > 0 && !this.isJoystickSelected)
				{
					this.currentJoystickId = player.controllers.Joysticks[0].id;
					this.assignedControllerButtons[0].SetInteractible(false, false);
				}
				if (num < 0)
				{
					goto IL_2B0;
				}
				using (List<ControlMapper.GUIButton>.Enumerator enumerator = this.assignedControllerButtons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ControlMapper.GUIButton guibutton3 = enumerator.Current;
						if (guibutton3.buttonInfo.intData == num)
						{
							this.SetUISelection(guibutton3.gameObject);
							break;
						}
					}
					goto IL_2B0;
				}
			}
			if (player.controllers.joystickCount > 0 && !this.isJoystickSelected)
			{
				this.currentJoystickId = player.controllers.Joysticks[0].id;
			}
			IL_2B0:
			if (this.isJoystickSelected && player.controllers.joystickCount > 0)
			{
				this.references.removeControllerButton.interactable = true;
				this.references.controllerNameLabel.text = this._language.GetControllerName(this.currentJoystick);
				if (this.currentJoystick.axisCount > 0)
				{
					this.references.calibrateControllerButton.interactable = true;
				}
			}
			int joystickCount = player.controllers.joystickCount;
			int joystickCount2 = ReInput.controllers.joystickCount;
			int maxControllersPerPlayer = this.GetMaxControllersPerPlayer();
			bool flag = maxControllersPerPlayer == 0;
			if (joystickCount2 > 0 && joystickCount < joystickCount2 && (maxControllersPerPlayer == 1 || flag || joystickCount < maxControllersPerPlayer))
			{
				UITools.SetInteractable(this.references.assignControllerButton, true, false);
			}
		}

		// Token: 0x06002B52 RID: 11090 RVA: 0x002053C0 File Offset: 0x002035C0
		private void RedrawMapCategoriesGroup(bool playTransitions)
		{
			if (!this.showMapCategories)
			{
				return;
			}
			for (int i = 0; i < this.mapCategoryButtons.Count; i++)
			{
				bool state = this.currentMapCategoryId != this.mapCategoryButtons[i].buttonInfo.intData;
				this.mapCategoryButtons[i].SetInteractible(state, playTransitions);
			}
		}

		// Token: 0x06002B53 RID: 11091 RVA: 0x00205421 File Offset: 0x00203621
		private void RedrawInputGrid(bool listsChanged)
		{
			if (listsChanged)
			{
				this.RefreshInputGridStructure();
			}
			this.PopulateInputFields();
			if (listsChanged)
			{
				this.ResetInputGridScrollBar();
			}
		}

		// Token: 0x06002B54 RID: 11092 RVA: 0x0020543B File Offset: 0x0020363B
		private void ForceRefresh()
		{
			if (this.windowManager.isWindowOpen)
			{
				this.CloseAllWindows();
				return;
			}
			this.Redraw(false, false);
		}

		// Token: 0x06002B55 RID: 11093 RVA: 0x0020545C File Offset: 0x0020365C
		private void CreateInputCategoryRow(ref int rowCount, InputCategory category)
		{
			this.CreateLabel(this._language.GetMapCategoryName(category.id), this.references.inputGridActionColumn, new Vector2(0f, (float)(rowCount * this._inputRowHeight) * -1f));
			rowCount++;
		}

		// Token: 0x06002B56 RID: 11094 RVA: 0x002054AC File Offset: 0x002036AC
		private ControlMapper.GUILabel CreateLabel(string labelText, Transform parent, Vector2 offset)
		{
			return this.CreateLabel(this.prefabs.inputGridLabel, labelText, parent, offset);
		}

		// Token: 0x06002B57 RID: 11095 RVA: 0x002054C4 File Offset: 0x002036C4
		private ControlMapper.GUILabel CreateLabel(GameObject prefab, string labelText, Transform parent, Vector2 offset)
		{
			GameObject gameObject = this.InstantiateGUIObject(prefab, parent, offset);
			TMP_Text componentInSelfOrChildren = UnityTools.GetComponentInSelfOrChildren<TMP_Text>(gameObject);
			if (componentInSelfOrChildren == null)
			{
				Debug.LogError("Rewired Control Mapper: Label prefab is missing Text component!");
				return null;
			}
			componentInSelfOrChildren.text = labelText;
			return new ControlMapper.GUILabel(gameObject);
		}

		// Token: 0x06002B58 RID: 11096 RVA: 0x00205505 File Offset: 0x00203705
		private ControlMapper.GUIButton CreateButton(string labelText, Transform parent, Vector2 offset)
		{
			ControlMapper.GUIButton guibutton = new ControlMapper.GUIButton(this.InstantiateGUIObject(this.prefabs.button, parent, offset));
			guibutton.SetLabel(labelText);
			return guibutton;
		}

		// Token: 0x06002B59 RID: 11097 RVA: 0x00205526 File Offset: 0x00203726
		private ControlMapper.GUIButton CreateFitButton(string labelText, Transform parent, Vector2 offset)
		{
			ControlMapper.GUIButton guibutton = new ControlMapper.GUIButton(this.InstantiateGUIObject(this.prefabs.fitButton, parent, offset));
			guibutton.SetLabel(labelText);
			return guibutton;
		}

		// Token: 0x06002B5A RID: 11098 RVA: 0x00205548 File Offset: 0x00203748
		private ControlMapper.GUIInputField CreateInputField(Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			ControlMapper.GUIInputField guiinputField = this.CreateInputField(parent, offset);
			guiinputField.SetLabel("");
			guiinputField.SetFieldInfoData(actionId, axisRange, controllerType, fieldIndex);
			guiinputField.SetOnClickCallback(this.inputFieldActivatedDelegate);
			guiinputField.fieldInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
			return guiinputField;
		}

		// Token: 0x06002B5B RID: 11099 RVA: 0x00205599 File Offset: 0x00203799
		private ControlMapper.GUIInputField CreateInputField(Transform parent, Vector2 offset)
		{
			return new ControlMapper.GUIInputField(this.InstantiateGUIObject(this.prefabs.inputGridFieldButton, parent, offset));
		}

		// Token: 0x06002B5C RID: 11100 RVA: 0x002055B3 File Offset: 0x002037B3
		private ControlMapper.GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			ControlMapper.GUIToggle guitoggle = this.CreateToggle(prefab, parent, offset);
			guitoggle.SetToggleInfoData(actionId, axisRange, controllerType, fieldIndex);
			guitoggle.SetOnSubmitCallback(this.inputFieldInvertToggleStateChangedDelegate);
			guitoggle.toggleInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
			return guitoggle;
		}

		// Token: 0x06002B5D RID: 11101 RVA: 0x002055EF File Offset: 0x002037EF
		private ControlMapper.GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset)
		{
			return new ControlMapper.GUIToggle(this.InstantiateGUIObject(prefab, parent, offset));
		}

		// Token: 0x06002B5E RID: 11102 RVA: 0x00205600 File Offset: 0x00203800
		private GameObject InstantiateGUIObject(GameObject prefab, Transform parent, Vector2 offset)
		{
			if (prefab == null)
			{
				Debug.LogError("Rewired Control Mapper: Prefab is null!");
				return null;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(prefab);
			return this.InitializeNewGUIGameObject(gameObject, parent, offset);
		}

		// Token: 0x06002B5F RID: 11103 RVA: 0x00205634 File Offset: 0x00203834
		private GameObject CreateNewGUIObject(string name, Transform parent, Vector2 offset)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = name;
			gameObject.AddComponent<RectTransform>();
			return this.InitializeNewGUIGameObject(gameObject, parent, offset);
		}

		// Token: 0x06002B60 RID: 11104 RVA: 0x00205660 File Offset: 0x00203860
		private GameObject InitializeNewGUIGameObject(GameObject gameObject, Transform parent, Vector2 offset)
		{
			if (gameObject == null)
			{
				Debug.LogError("Rewired Control Mapper: GameObject is null!");
				return null;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			if (component == null)
			{
				Debug.LogError("Rewired Control Mapper: GameObject does not have a RectTransform component!");
				return gameObject;
			}
			if (parent != null)
			{
				component.SetParent(parent, false);
			}
			component.anchoredPosition = offset;
			return gameObject;
		}

		// Token: 0x06002B61 RID: 11105 RVA: 0x002056B8 File Offset: 0x002038B8
		private GameObject CreateNewColumnGroup(string name, Transform parent, int maxWidth)
		{
			GameObject gameObject = this.CreateNewGUIObject(name, parent, Vector2.zero);
			this.inputGrid.AddGroup(gameObject);
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			if (maxWidth >= 0)
			{
				layoutElement.preferredWidth = (float)maxWidth;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 0f);
			component.anchorMax = new Vector2(1f, 0f);
			return gameObject;
		}

		// Token: 0x06002B62 RID: 11106 RVA: 0x00205722 File Offset: 0x00203922
		private Window OpenWindow(bool closeOthers)
		{
			return this.OpenWindow(string.Empty, closeOthers);
		}

		// Token: 0x06002B63 RID: 11107 RVA: 0x00205730 File Offset: 0x00203930
		private Window OpenWindow(string name, bool closeOthers)
		{
			if (closeOthers)
			{
				this.windowManager.CancelAll();
			}
			Window window = this.windowManager.OpenWindow(name, this._defaultWindowWidth, this._defaultWindowHeight);
			if (window == null)
			{
				return null;
			}
			this.ChildWindowOpened();
			return window;
		}

		// Token: 0x06002B64 RID: 11108 RVA: 0x00205776 File Offset: 0x00203976
		private Window OpenWindow(GameObject windowPrefab, bool closeOthers)
		{
			return this.OpenWindow(windowPrefab, string.Empty, closeOthers);
		}

		// Token: 0x06002B65 RID: 11109 RVA: 0x00205788 File Offset: 0x00203988
		private Window OpenWindow(GameObject windowPrefab, string name, bool closeOthers)
		{
			if (closeOthers)
			{
				this.windowManager.CancelAll();
			}
			Window window = this.windowManager.OpenWindow(windowPrefab, name);
			if (window == null)
			{
				return null;
			}
			this.ChildWindowOpened();
			return window;
		}

		// Token: 0x06002B66 RID: 11110 RVA: 0x002057C4 File Offset: 0x002039C4
		private void OpenModal(string title, string message, string confirmText, Action<int> confirmAction, string cancelText, Action<int> cancelAction, bool closeOthers)
		{
			Window window = this.OpenWindow(closeOthers);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, title);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), message);
			UnityAction unityAction = delegate()
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, confirmText, delegate()
			{
				this.OnRestoreDefaultsConfirmed(window.id);
			}, unityAction, false);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, cancelText, unityAction, unityAction, true);
			this.windowManager.Focus(window);
		}

		// Token: 0x06002B67 RID: 11111 RVA: 0x002058CF File Offset: 0x00203ACF
		private void CloseWindow(int windowId)
		{
			if (!this.windowManager.isWindowOpen)
			{
				return;
			}
			this.windowManager.CloseWindow(windowId);
			this.ChildWindowClosed();
		}

		// Token: 0x06002B68 RID: 11112 RVA: 0x002058F1 File Offset: 0x00203AF1
		private void CloseTopWindow()
		{
			if (!this.windowManager.isWindowOpen)
			{
				return;
			}
			this.windowManager.CloseTop();
			this.ChildWindowClosed();
		}

		// Token: 0x06002B69 RID: 11113 RVA: 0x00205912 File Offset: 0x00203B12
		private void CloseAllWindows()
		{
			if (!this.windowManager.isWindowOpen)
			{
				return;
			}
			this.windowManager.CancelAll();
			this.ChildWindowClosed();
			this.InputPollingStopped();
		}

		// Token: 0x06002B6A RID: 11114 RVA: 0x00205939 File Offset: 0x00203B39
		private void ChildWindowOpened()
		{
			if (!this.windowManager.isWindowOpen)
			{
				return;
			}
			this.SetIsFocused(false);
			if (this._PopupWindowOpenedEvent != null)
			{
				this._PopupWindowOpenedEvent.Invoke();
			}
			if (this._onPopupWindowOpened != null)
			{
				this._onPopupWindowOpened.Invoke();
			}
		}

		// Token: 0x06002B6B RID: 11115 RVA: 0x00205976 File Offset: 0x00203B76
		private void ChildWindowClosed()
		{
			if (this.windowManager.isWindowOpen)
			{
				return;
			}
			this.SetIsFocused(true);
			if (this._PopupWindowClosedEvent != null)
			{
				this._PopupWindowClosedEvent.Invoke();
			}
			if (this._onPopupWindowClosed != null)
			{
				this._onPopupWindowClosed.Invoke();
			}
		}

		// Token: 0x06002B6C RID: 11116 RVA: 0x002059B4 File Offset: 0x00203BB4
		private bool HasElementAssignmentConflicts(Player player, ControlMapper.InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (player == null || mapping == null)
			{
				return false;
			}
			ElementAssignmentConflictCheck elementAssignmentConflictCheck;
			if (!this.CreateConflictCheck(mapping, assignment, out elementAssignmentConflictCheck))
			{
				return false;
			}
			if (skipOtherPlayers)
			{
				return ReInput.players.SystemPlayer.controllers.conflictChecking.DoesElementAssignmentConflict(elementAssignmentConflictCheck) || player.controllers.conflictChecking.DoesElementAssignmentConflict(elementAssignmentConflictCheck);
			}
			return ReInput.controllers.conflictChecking.DoesElementAssignmentConflict(elementAssignmentConflictCheck);
		}

		// Token: 0x06002B6D RID: 11117 RVA: 0x00205A20 File Offset: 0x00203C20
		private bool IsBlockingAssignmentConflict(ControlMapper.InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			ElementAssignmentConflictCheck elementAssignmentConflictCheck;
			if (!this.CreateConflictCheck(mapping, assignment, out elementAssignmentConflictCheck))
			{
				return false;
			}
			if (skipOtherPlayers)
			{
				foreach (ElementAssignmentConflictInfo elementAssignmentConflictInfo in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(elementAssignmentConflictCheck))
				{
					if (!elementAssignmentConflictInfo.isUserAssignable)
					{
						return true;
					}
				}
				using (IEnumerator<ElementAssignmentConflictInfo> enumerator = this.currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(elementAssignmentConflictCheck).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ElementAssignmentConflictInfo elementAssignmentConflictInfo2 = enumerator.Current;
						if (!elementAssignmentConflictInfo2.isUserAssignable)
						{
							return true;
						}
					}
					return false;
				}
			}
			foreach (ElementAssignmentConflictInfo elementAssignmentConflictInfo3 in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(elementAssignmentConflictCheck))
			{
				if (!elementAssignmentConflictInfo3.isUserAssignable)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002B6E RID: 11118 RVA: 0x00205B44 File Offset: 0x00203D44
		private IEnumerable<ElementAssignmentConflictInfo> ElementAssignmentConflicts(Player player, ControlMapper.InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (player == null || mapping == null)
			{
				yield break;
			}
			ElementAssignmentConflictCheck conflictCheck;
			if (!this.CreateConflictCheck(mapping, assignment, out conflictCheck))
			{
				yield break;
			}
			if (skipOtherPlayers)
			{
				foreach (ElementAssignmentConflictInfo elementAssignmentConflictInfo in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!elementAssignmentConflictInfo.isUserAssignable)
					{
						yield return elementAssignmentConflictInfo;
					}
				}
				IEnumerator<ElementAssignmentConflictInfo> enumerator = null;
				foreach (ElementAssignmentConflictInfo elementAssignmentConflictInfo2 in player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!elementAssignmentConflictInfo2.isUserAssignable)
					{
						yield return elementAssignmentConflictInfo2;
					}
				}
				enumerator = null;
			}
			else
			{
				foreach (ElementAssignmentConflictInfo elementAssignmentConflictInfo3 in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!elementAssignmentConflictInfo3.isUserAssignable)
					{
						yield return elementAssignmentConflictInfo3;
					}
				}
				IEnumerator<ElementAssignmentConflictInfo> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06002B6F RID: 11119 RVA: 0x00205B74 File Offset: 0x00203D74
		private bool CreateConflictCheck(ControlMapper.InputMapping mapping, ElementAssignment assignment, out ElementAssignmentConflictCheck conflictCheck)
		{
			if (mapping == null || this.currentPlayer == null)
			{
				conflictCheck = default(ElementAssignmentConflictCheck);
				return false;
			}
			conflictCheck = assignment.ToElementAssignmentConflictCheck();
			conflictCheck.playerId = this.currentPlayer.id;
			conflictCheck.controllerType = mapping.controllerType;
			conflictCheck.controllerId = mapping.controllerId;
			conflictCheck.controllerMapId = mapping.map.id;
			conflictCheck.controllerMapCategoryId = mapping.map.categoryId;
			if (mapping.aem != null)
			{
				conflictCheck.elementMapId = mapping.aem.id;
			}
			return true;
		}

		// Token: 0x06002B70 RID: 11120 RVA: 0x00205C08 File Offset: 0x00203E08
		private void PollKeyboardForAssignment(out ControllerPollingInfo pollingInfo, out bool modifierKeyPressed, out ModifierKeyFlags modifierFlags, out string label)
		{
			pollingInfo = default(ControllerPollingInfo);
			label = string.Empty;
			modifierKeyPressed = false;
			modifierFlags = 0;
			int num = 0;
			ControllerPollingInfo controllerPollingInfo = default(ControllerPollingInfo);
			ControllerPollingInfo controllerPollingInfo2 = default(ControllerPollingInfo);
			ModifierKeyFlags modifierKeyFlags = 0;
			foreach (ControllerPollingInfo controllerPollingInfo3 in ReInput.controllers.Keyboard.PollForAllKeys())
			{
				KeyCode keyboardKey = controllerPollingInfo3.keyboardKey;
				if (keyboardKey != 313)
				{
					if (Keyboard.IsModifierKey(controllerPollingInfo3.keyboardKey))
					{
						if (num == 0)
						{
							controllerPollingInfo2 = controllerPollingInfo3;
						}
						modifierKeyFlags |= Keyboard.KeyCodeToModifierKeyFlags(keyboardKey);
						num++;
					}
					else if (controllerPollingInfo.keyboardKey == null)
					{
						controllerPollingInfo = controllerPollingInfo3;
					}
				}
			}
			if (controllerPollingInfo.keyboardKey == null)
			{
				if (num > 0)
				{
					modifierKeyPressed = true;
					if (num == 1)
					{
						if (ReInput.controllers.Keyboard.GetKeyTimePressed(controllerPollingInfo2.keyboardKey) > 1.0)
						{
							pollingInfo = controllerPollingInfo2;
							return;
						}
						label = Keyboard.GetKeyName(controllerPollingInfo2.keyboardKey);
						return;
					}
					else
					{
						label = this._language.ModifierKeyFlagsToString(modifierKeyFlags);
					}
				}
				return;
			}
			if (!ReInput.controllers.Keyboard.GetKeyDown(controllerPollingInfo.keyboardKey))
			{
				return;
			}
			if (num == 0)
			{
				pollingInfo = controllerPollingInfo;
				return;
			}
			pollingInfo = controllerPollingInfo;
			modifierFlags = modifierKeyFlags;
		}

		// Token: 0x06002B71 RID: 11121 RVA: 0x00205D54 File Offset: 0x00203F54
		private bool GetFirstElementAssignmentConflict(ElementAssignmentConflictCheck conflictCheck, out ElementAssignmentConflictInfo conflict, bool skipOtherPlayers)
		{
			if (this.GetFirstElementAssignmentConflict(this.currentPlayer, conflictCheck, out conflict))
			{
				return true;
			}
			if (this.GetFirstElementAssignmentConflict(ReInput.players.SystemPlayer, conflictCheck, out conflict))
			{
				return true;
			}
			if (!skipOtherPlayers)
			{
				IList<Player> players = ReInput.players.Players;
				for (int i = 0; i < players.Count; i++)
				{
					Player player = players[i];
					if (player != this.currentPlayer && this.GetFirstElementAssignmentConflict(player, conflictCheck, out conflict))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06002B72 RID: 11122 RVA: 0x00205DC8 File Offset: 0x00203FC8
		private bool GetFirstElementAssignmentConflict(Player player, ElementAssignmentConflictCheck conflictCheck, out ElementAssignmentConflictInfo conflict)
		{
			using (IEnumerator<ElementAssignmentConflictInfo> enumerator = player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ElementAssignmentConflictInfo elementAssignmentConflictInfo = enumerator.Current;
					conflict = elementAssignmentConflictInfo;
					return true;
				}
			}
			conflict = default(ElementAssignmentConflictInfo);
			return false;
		}

		// Token: 0x06002B73 RID: 11123 RVA: 0x00205E30 File Offset: 0x00204030
		private void StartAxisCalibration(int axisIndex)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (this.currentPlayer.controllers.joystickCount == 0)
			{
				return;
			}
			Joystick currentJoystick = this.currentJoystick;
			if (axisIndex < 0 || axisIndex >= currentJoystick.axisCount)
			{
				return;
			}
			this.pendingAxisCalibration = new ControlMapper.AxisCalibrator(currentJoystick, axisIndex);
			this.ShowCalibrateAxisStep1Window();
		}

		// Token: 0x06002B74 RID: 11124 RVA: 0x00205E81 File Offset: 0x00204081
		private void EndAxisCalibration()
		{
			if (this.pendingAxisCalibration == null)
			{
				return;
			}
			this.pendingAxisCalibration.Commit();
			this.pendingAxisCalibration = null;
		}

		// Token: 0x06002B75 RID: 11125 RVA: 0x00205E9E File Offset: 0x0020409E
		private void SetUISelection(GameObject selection)
		{
			if (EventSystem.current == null)
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(selection);
		}

		// Token: 0x06002B76 RID: 11126 RVA: 0x00205EB9 File Offset: 0x002040B9
		private void RestoreLastUISelection()
		{
			if (this.lastUISelection == null || !this.lastUISelection.activeInHierarchy)
			{
				this.SetDefaultUISelection();
				return;
			}
			this.SetUISelection(this.lastUISelection);
		}

		// Token: 0x06002B77 RID: 11127 RVA: 0x00205EE9 File Offset: 0x002040E9
		private void SetDefaultUISelection()
		{
			if (!this.isOpen)
			{
				return;
			}
			if (this.references.defaultSelection == null)
			{
				this.SetUISelection(null);
				return;
			}
			this.SetUISelection(this.references.defaultSelection.gameObject);
		}

		// Token: 0x06002B78 RID: 11128 RVA: 0x00205F28 File Offset: 0x00204128
		private void SelectDefaultMapCategory(bool redraw)
		{
			this.currentMapCategoryId = this.GetDefaultMapCategoryId();
			this.OnMapCategorySelected(this.currentMapCategoryId, redraw);
			if (!this.showMapCategories)
			{
				return;
			}
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				if (ReInput.mapping.GetMapCategory(this._mappingSets[i].mapCategoryId) != null)
				{
					this.currentMapCategoryId = this._mappingSets[i].mapCategoryId;
					break;
				}
			}
			if (this.currentMapCategoryId < 0)
			{
				return;
			}
			for (int j = 0; j < this._mappingSets.Length; j++)
			{
				bool state = this._mappingSets[j].mapCategoryId != this.currentMapCategoryId;
				this.mapCategoryButtons[j].SetInteractible(state, false);
			}
		}

		// Token: 0x06002B79 RID: 11129 RVA: 0x00205FE2 File Offset: 0x002041E2
		private void CheckUISelection()
		{
			if (!this.isFocused)
			{
				return;
			}
			if (this.currentUISelection == null)
			{
				this.RestoreLastUISelection();
			}
		}

		// Token: 0x06002B7A RID: 11130 RVA: 0x00206001 File Offset: 0x00204201
		private void OnUIElementSelected(GameObject selectedObject)
		{
			this.lastUISelection = selectedObject;
		}

		// Token: 0x06002B7B RID: 11131 RVA: 0x0020600A File Offset: 0x0020420A
		private void SetIsFocused(bool state)
		{
			this.references.mainCanvasGroup.interactable = state;
			if (state)
			{
				this.Redraw(false, false);
				this.RestoreLastUISelection();
				this.blockInputOnFocusEndTime = Time.unscaledTime + 0.1f;
			}
		}

		// Token: 0x06002B7C RID: 11132 RVA: 0x0020603F File Offset: 0x0020423F
		public void Toggle()
		{
			if (this.isOpen)
			{
				this.Close(true);
				return;
			}
			this.Open();
		}

		// Token: 0x06002B7D RID: 11133 RVA: 0x00206057 File Offset: 0x00204257
		public void Open()
		{
			this.Open(false);
		}

		// Token: 0x06002B7E RID: 11134 RVA: 0x00206060 File Offset: 0x00204260
		private void Open(bool force)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			if (!this.initialized)
			{
				return;
			}
			if (!force && this.isOpen)
			{
				return;
			}
			this.Clear();
			this.canvas.SetActive(true);
			this.OnPlayerSelected(0, false);
			this.SelectDefaultMapCategory(false);
			this.SetDefaultUISelection();
			this.Redraw(true, false);
			if (this._ScreenOpenedEvent != null)
			{
				this._ScreenOpenedEvent.Invoke();
			}
			if (this._onScreenOpened != null)
			{
				this._onScreenOpened.Invoke();
			}
		}

		// Token: 0x06002B7F RID: 11135 RVA: 0x002060E8 File Offset: 0x002042E8
		public void Close(bool save)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.isOpen)
			{
				return;
			}
			if (save && ReInput.userDataStore != null)
			{
				ReInput.userDataStore.Save();
			}
			this.Clear();
			this.canvas.SetActive(false);
			this.SetUISelection(null);
			if (this._ScreenClosedEvent != null)
			{
				this._ScreenClosedEvent.Invoke();
			}
			if (this._onScreenClosed != null)
			{
				this._onScreenClosed.Invoke();
			}
		}

		// Token: 0x06002B80 RID: 11136 RVA: 0x0020615A File Offset: 0x0020435A
		private void Clear()
		{
			this.windowManager.CancelAll();
			this.lastUISelection = null;
			this.pendingInputMapping = null;
			this.pendingAxisCalibration = null;
			this.InputPollingStopped();
		}

		// Token: 0x06002B81 RID: 11137 RVA: 0x00206182 File Offset: 0x00204382
		private void ClearCompletely()
		{
			this.Clear();
			this.ClearSpawnedObjects();
			this.ClearAllVars();
		}

		// Token: 0x06002B82 RID: 11138 RVA: 0x00206198 File Offset: 0x00204398
		private void ClearSpawnedObjects()
		{
			this.windowManager.ClearCompletely();
			this.inputGrid.ClearAll();
			foreach (ControlMapper.GUIButton guibutton in this.playerButtons)
			{
				Object.Destroy(guibutton.gameObject);
			}
			this.playerButtons.Clear();
			foreach (ControlMapper.GUIButton guibutton2 in this.mapCategoryButtons)
			{
				Object.Destroy(guibutton2.gameObject);
			}
			this.mapCategoryButtons.Clear();
			foreach (ControlMapper.GUIButton guibutton3 in this.assignedControllerButtons)
			{
				Object.Destroy(guibutton3.gameObject);
			}
			this.assignedControllerButtons.Clear();
			if (this.assignedControllerButtonsPlaceholder != null)
			{
				Object.Destroy(this.assignedControllerButtonsPlaceholder.gameObject);
				this.assignedControllerButtonsPlaceholder = null;
			}
			foreach (GameObject gameObject in this.miscInstantiatedObjects)
			{
				Object.Destroy(gameObject);
			}
			this.miscInstantiatedObjects.Clear();
		}

		// Token: 0x06002B83 RID: 11139 RVA: 0x00206318 File Offset: 0x00204518
		private void ClearVarsOnPlayerChange()
		{
			this.currentJoystickId = -1;
		}

		// Token: 0x06002B84 RID: 11140 RVA: 0x00206318 File Offset: 0x00204518
		private void ClearVarsOnJoystickChange()
		{
			this.currentJoystickId = -1;
		}

		// Token: 0x06002B85 RID: 11141 RVA: 0x00206324 File Offset: 0x00204524
		private void ClearAllVars()
		{
			this.initialized = false;
			ControlMapper.Instance = null;
			this.playerCount = 0;
			this.inputGrid = null;
			this.windowManager = null;
			this.currentPlayerId = -1;
			this.currentMapCategoryId = -1;
			this.playerButtons = null;
			this.mapCategoryButtons = null;
			this.miscInstantiatedObjects = null;
			this.canvas = null;
			this.lastUISelection = null;
			this.currentJoystickId = -1;
			this.pendingInputMapping = null;
			this.pendingAxisCalibration = null;
			this.inputFieldActivatedDelegate = null;
			this.inputFieldInvertToggleStateChangedDelegate = null;
			this.isPollingForInput = false;
		}

		// Token: 0x06002B86 RID: 11142 RVA: 0x002063AE File Offset: 0x002045AE
		public void Reset()
		{
			if (!this.initialized)
			{
				return;
			}
			this.ClearCompletely();
			this.Initialize();
			if (this.isOpen)
			{
				this.Open(true);
			}
		}

		// Token: 0x06002B87 RID: 11143 RVA: 0x002063D4 File Offset: 0x002045D4
		private void SetActionAxisInverted(bool state, ControllerType controllerType, int actionElementMapId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			ControllerMapWithAxes controllerMapWithAxes = this.GetControllerMap(controllerType) as ControllerMapWithAxes;
			if (controllerMapWithAxes == null)
			{
				return;
			}
			ActionElementMap elementMap = controllerMapWithAxes.GetElementMap(actionElementMapId);
			if (elementMap == null)
			{
				return;
			}
			elementMap.invert = state;
		}

		// Token: 0x06002B88 RID: 11144 RVA: 0x00206410 File Offset: 0x00204610
		private ControllerMap GetControllerMap(ControllerType type)
		{
			if (this.currentPlayer == null)
			{
				return null;
			}
			int num = 0;
			switch (type)
			{
			case 0:
			case 1:
				break;
			case 2:
				if (this.currentPlayer.controllers.joystickCount <= 0)
				{
					return null;
				}
				num = this.currentJoystick.id;
				break;
			default:
				throw new NotImplementedException();
			}
			return this.currentPlayer.controllers.maps.GetFirstMapInCategory(type, num, this.currentMapCategoryId);
		}

		// Token: 0x06002B89 RID: 11145 RVA: 0x00206484 File Offset: 0x00204684
		private ControllerMap GetControllerMapOrCreateNew(ControllerType controllerType, int controllerId, int layoutId)
		{
			ControllerMap controllerMap = this.GetControllerMap(controllerType);
			if (controllerMap == null)
			{
				this.currentPlayer.controllers.maps.AddEmptyMap(controllerType, controllerId, this.currentMapCategoryId, layoutId);
				controllerMap = this.currentPlayer.controllers.maps.GetMap(controllerType, controllerId, this.currentMapCategoryId, layoutId);
			}
			return controllerMap;
		}

		// Token: 0x06002B8A RID: 11146 RVA: 0x002064DC File Offset: 0x002046DC
		private int CountIEnumerable<T>(IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				return 0;
			}
			IEnumerator<T> enumerator = enumerable.GetEnumerator();
			if (enumerator == null)
			{
				return 0;
			}
			int num = 0;
			while (enumerator.MoveNext())
			{
				num++;
			}
			return num;
		}

		// Token: 0x06002B8B RID: 11147 RVA: 0x0020650C File Offset: 0x0020470C
		private int GetDefaultMapCategoryId()
		{
			if (this._mappingSets.Length == 0)
			{
				return 0;
			}
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				if (ReInput.mapping.GetMapCategory(this._mappingSets[i].mapCategoryId) != null)
				{
					return this._mappingSets[i].mapCategoryId;
				}
			}
			return 0;
		}

		// Token: 0x06002B8C RID: 11148 RVA: 0x00206560 File Offset: 0x00204760
		private void SubscribeFixedUISelectionEvents()
		{
			if (this.references.fixedSelectableUIElements == null)
			{
				return;
			}
			GameObject[] fixedSelectableUIElements = this.references.fixedSelectableUIElements;
			for (int i = 0; i < fixedSelectableUIElements.Length; i++)
			{
				UIElementInfo component = UnityTools.GetComponent<UIElementInfo>(fixedSelectableUIElements[i]);
				if (!(component == null))
				{
					component.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
				}
			}
		}

		// Token: 0x06002B8D RID: 11149 RVA: 0x002065BC File Offset: 0x002047BC
		private void SubscribeMenuControlInputEvents()
		{
			this.SubscribeRewiredInputEventAllPlayers(this._screenToggleAction, new Action<InputActionEventData>(this.OnScreenToggleActionPressed));
			this.SubscribeRewiredInputEventAllPlayers(this._screenOpenAction, new Action<InputActionEventData>(this.OnScreenOpenActionPressed));
			this.SubscribeRewiredInputEventAllPlayers(this._screenCloseAction, new Action<InputActionEventData>(this.OnScreenCloseActionPressed));
			this.SubscribeRewiredInputEventAllPlayers(this._universalCancelAction, new Action<InputActionEventData>(this.OnUniversalCancelActionPressed));
		}

		// Token: 0x06002B8E RID: 11150 RVA: 0x0020662C File Offset: 0x0020482C
		private void UnsubscribeMenuControlInputEvents()
		{
			this.UnsubscribeRewiredInputEventAllPlayers(this._screenToggleAction, new Action<InputActionEventData>(this.OnScreenToggleActionPressed));
			this.UnsubscribeRewiredInputEventAllPlayers(this._screenOpenAction, new Action<InputActionEventData>(this.OnScreenOpenActionPressed));
			this.UnsubscribeRewiredInputEventAllPlayers(this._screenCloseAction, new Action<InputActionEventData>(this.OnScreenCloseActionPressed));
			this.UnsubscribeRewiredInputEventAllPlayers(this._universalCancelAction, new Action<InputActionEventData>(this.OnUniversalCancelActionPressed));
		}

		// Token: 0x06002B8F RID: 11151 RVA: 0x0020669C File Offset: 0x0020489C
		private void SubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
		{
			if (actionId < 0 || callback == null)
			{
				return;
			}
			if (ReInput.mapping.GetAction(actionId) == null)
			{
				Debug.LogWarning("Rewired Control Mapper: " + actionId.ToString() + " is not a valid Action id!");
				return;
			}
			foreach (Player player in ReInput.players.AllPlayers)
			{
				player.AddInputEventDelegate(callback, 0, 3, actionId);
			}
		}

		// Token: 0x06002B90 RID: 11152 RVA: 0x00206720 File Offset: 0x00204920
		private void UnsubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
		{
			if (actionId < 0 || callback == null)
			{
				return;
			}
			if (!ReInput.isReady)
			{
				return;
			}
			if (ReInput.mapping.GetAction(actionId) == null)
			{
				Debug.LogWarning("Rewired Control Mapper: " + actionId.ToString() + " is not a valid Action id!");
				return;
			}
			foreach (Player player in ReInput.players.AllPlayers)
			{
				player.RemoveInputEventDelegate(callback, 0, 3, actionId);
			}
		}

		// Token: 0x06002B91 RID: 11153 RVA: 0x002067AC File Offset: 0x002049AC
		private int GetMaxControllersPerPlayer()
		{
			if (this._rewiredInputManager.userData.ConfigVars.autoAssignJoysticks)
			{
				return this._rewiredInputManager.userData.ConfigVars.maxJoysticksPerPlayer;
			}
			return this._maxControllersPerPlayer;
		}

		// Token: 0x06002B92 RID: 11154 RVA: 0x002067E1 File Offset: 0x002049E1
		private bool ShowAssignedControllers()
		{
			return this._showControllers && (this._showAssignedControllers || this.GetMaxControllersPerPlayer() != 1);
		}

		// Token: 0x06002B93 RID: 11155 RVA: 0x00206803 File Offset: 0x00204A03
		private void InspectorPropertyChanged(bool reset = false)
		{
			if (reset)
			{
				this.Reset();
			}
		}

		// Token: 0x06002B94 RID: 11156 RVA: 0x00206810 File Offset: 0x00204A10
		private void AssignController(Player player, int controllerId)
		{
			if (player == null)
			{
				return;
			}
			if (player.controllers.ContainsController(2, controllerId))
			{
				return;
			}
			if (this.GetMaxControllersPerPlayer() == 1)
			{
				this.RemoveAllControllers(player);
				this.ClearVarsOnJoystickChange();
			}
			foreach (Player player2 in ReInput.players.Players)
			{
				if (player2 != player)
				{
					this.RemoveController(player2, controllerId);
				}
			}
			player.controllers.AddController(2, controllerId, false);
			if (ReInput.userDataStore != null)
			{
				ReInput.userDataStore.LoadControllerData(player.id, 2, controllerId);
			}
		}

		// Token: 0x06002B95 RID: 11157 RVA: 0x002068B8 File Offset: 0x00204AB8
		private void RemoveAllControllers(Player player)
		{
			if (player == null)
			{
				return;
			}
			IList<Joystick> joysticks = player.controllers.Joysticks;
			for (int i = joysticks.Count - 1; i >= 0; i--)
			{
				this.RemoveController(player, joysticks[i].id);
			}
		}

		// Token: 0x06002B96 RID: 11158 RVA: 0x002068FB File Offset: 0x00204AFB
		private void RemoveController(Player player, int controllerId)
		{
			if (player == null)
			{
				return;
			}
			if (!player.controllers.ContainsController(2, controllerId))
			{
				return;
			}
			if (ReInput.userDataStore != null)
			{
				ReInput.userDataStore.SaveControllerData(player.id, 2, controllerId);
			}
			player.controllers.RemoveController(2, controllerId);
		}

		// Token: 0x06002B97 RID: 11159 RVA: 0x00206937 File Offset: 0x00204B37
		private bool IsAllowedAssignment(ControlMapper.InputMapping pendingInputMapping, ControllerPollingInfo pollingInfo)
		{
			return pendingInputMapping != null && (pendingInputMapping.axisRange != null || this._showSplitAxisInputFields || pollingInfo.elementType != 1);
		}

		// Token: 0x06002B98 RID: 11160 RVA: 0x0020695B File Offset: 0x00204B5B
		private void InputPollingStarted()
		{
			bool flag = this.isPollingForInput;
			this.isPollingForInput = true;
			if (!flag)
			{
				if (this._InputPollingStartedEvent != null)
				{
					this._InputPollingStartedEvent.Invoke();
				}
				if (this._onInputPollingStarted != null)
				{
					this._onInputPollingStarted.Invoke();
				}
			}
		}

		// Token: 0x06002B99 RID: 11161 RVA: 0x00206992 File Offset: 0x00204B92
		private void InputPollingStopped()
		{
			bool flag = this.isPollingForInput;
			this.isPollingForInput = false;
			if (flag)
			{
				if (this._InputPollingEndedEvent != null)
				{
					this._InputPollingEndedEvent.Invoke();
				}
				if (this._onInputPollingEnded != null)
				{
					this._onInputPollingEnded.Invoke();
				}
			}
		}

		// Token: 0x06002B9A RID: 11162 RVA: 0x002069C9 File Offset: 0x00204BC9
		private int GetControllerInputFieldCount(ControllerType controllerType)
		{
			switch (controllerType)
			{
			case 0:
				return this._keyboardInputFieldCount;
			case 1:
				return this._mouseInputFieldCount;
			case 2:
				return this._controllerInputFieldCount;
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x06002B9B RID: 11163 RVA: 0x002069FC File Offset: 0x00204BFC
		private bool ShowSwapButton(int windowId, ControlMapper.InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (this.currentPlayer == null)
			{
				return false;
			}
			if (!this._allowElementAssignmentSwap)
			{
				return false;
			}
			if (mapping == null || mapping.aem == null)
			{
				return false;
			}
			ElementAssignmentConflictCheck elementAssignmentConflictCheck;
			if (!this.CreateConflictCheck(mapping, assignment, out elementAssignmentConflictCheck))
			{
				Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
				return false;
			}
			List<ElementAssignmentConflictInfo> list = new List<ElementAssignmentConflictInfo>();
			list.AddRange(this.currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(elementAssignmentConflictCheck));
			list.AddRange(ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(elementAssignmentConflictCheck));
			if (list.Count == 0)
			{
				return false;
			}
			ActionElementMap aem2 = mapping.aem;
			ElementAssignmentConflictInfo elementAssignmentConflictInfo = list[0];
			int actionId = elementAssignmentConflictInfo.elementMap.actionId;
			Pole axisContribution = elementAssignmentConflictInfo.elementMap.axisContribution;
			AxisRange axisRange = aem2.axisRange;
			ControllerElementType elementType = aem2.elementType;
			if (elementType == elementAssignmentConflictInfo.elementMap.elementType && elementType == null)
			{
				if (axisRange != elementAssignmentConflictInfo.elementMap.axisRange)
				{
					if (axisRange == null)
					{
						axisRange = 1;
					}
					else if (elementAssignmentConflictInfo.elementMap.axisRange == null)
					{
					}
				}
			}
			else if (elementType == null && (elementAssignmentConflictInfo.elementMap.elementType == 1 || (elementAssignmentConflictInfo.elementMap.elementType == null && elementAssignmentConflictInfo.elementMap.axisRange != null)) && axisRange == null)
			{
				axisRange = 1;
			}
			int num = 0;
			if (assignment.actionId == elementAssignmentConflictInfo.actionId && mapping.map == elementAssignmentConflictInfo.controllerMap)
			{
				Controller controller = ReInput.controllers.GetController(mapping.controllerType, mapping.controllerId);
				if (this.SwapIsSameInputRange(elementType, axisRange, axisContribution, controller.GetElementById(assignment.elementIdentifierId).type, assignment.axisRange, assignment.axisContribution))
				{
					num++;
				}
			}
			using (IEnumerator<ActionElementMap> enumerator = elementAssignmentConflictInfo.controllerMap.ElementMapsWithAction(actionId).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ActionElementMap aem = enumerator.Current;
					if (aem.id != aem2.id && list.FindIndex((ElementAssignmentConflictInfo x) => x.elementMapId == aem.id) < 0 && this.SwapIsSameInputRange(elementType, axisRange, axisContribution, aem.elementType, aem.axisRange, aem.axisContribution))
					{
						num++;
					}
				}
			}
			return num < this.GetControllerInputFieldCount(mapping.controllerType);
		}

		// Token: 0x06002B9C RID: 11164 RVA: 0x00206C6C File Offset: 0x00204E6C
		private bool SwapIsSameInputRange(ControllerElementType origElementType, AxisRange origAxisRange, Pole origAxisContribution, ControllerElementType conflictElementType, AxisRange conflictAxisRange, Pole conflictAxisContribution)
		{
			return ((origElementType == 1 || (origElementType == null && origAxisRange != null)) && (conflictElementType == 1 || (conflictElementType == null && conflictAxisRange != null)) && conflictAxisContribution == origAxisContribution) || (origElementType == null && origAxisRange == null && conflictElementType == null && conflictAxisRange == null);
		}

		// Token: 0x06002B9D RID: 11165 RVA: 0x00206C9D File Offset: 0x00204E9D
		public static void ApplyTheme(ThemedElement.ElementInfo[] elementInfo)
		{
			if (ControlMapper.Instance == null)
			{
				return;
			}
			if (ControlMapper.Instance._themeSettings == null)
			{
				return;
			}
			if (!ControlMapper.Instance._useThemeSettings)
			{
				return;
			}
			ControlMapper.Instance._themeSettings.Apply(elementInfo);
		}

		// Token: 0x06002B9E RID: 11166 RVA: 0x00206CDD File Offset: 0x00204EDD
		public static LanguageDataBase GetLanguage()
		{
			if (ControlMapper.Instance == null)
			{
				return null;
			}
			return ControlMapper.Instance._language;
		}

		// Token: 0x040047D1 RID: 18385
		public const int versionMajor = 1;

		// Token: 0x040047D2 RID: 18386
		public const int versionMinor = 1;

		// Token: 0x040047D3 RID: 18387
		public const bool usesTMPro = true;

		// Token: 0x040047D4 RID: 18388
		private const float blockInputOnFocusTimeout = 0.1f;

		// Token: 0x040047D5 RID: 18389
		private const string buttonIdentifier_playerSelection = "PlayerSelection";

		// Token: 0x040047D6 RID: 18390
		private const string buttonIdentifier_removeController = "RemoveController";

		// Token: 0x040047D7 RID: 18391
		private const string buttonIdentifier_assignController = "AssignController";

		// Token: 0x040047D8 RID: 18392
		private const string buttonIdentifier_calibrateController = "CalibrateController";

		// Token: 0x040047D9 RID: 18393
		private const string buttonIdentifier_editInputBehaviors = "EditInputBehaviors";

		// Token: 0x040047DA RID: 18394
		private const string buttonIdentifier_mapCategorySelection = "MapCategorySelection";

		// Token: 0x040047DB RID: 18395
		private const string buttonIdentifier_assignedControllerSelection = "AssignedControllerSelection";

		// Token: 0x040047DC RID: 18396
		private const string buttonIdentifier_done = "Done";

		// Token: 0x040047DD RID: 18397
		private const string buttonIdentifier_restoreDefaults = "RestoreDefaults";

		// Token: 0x040047DE RID: 18398
		[SerializeField]
		[Tooltip("Must be assigned a Rewired Input Manager scene object or prefab.")]
		private InputManager _rewiredInputManager;

		// Token: 0x040047DF RID: 18399
		[Tooltip("Set to True to prevent the Game Object from being destroyed when a new scene is loaded.\n\nNOTE: Changing this value from True to False at runtime will have no effect because Object.DontDestroyOnLoad cannot be undone once set.")]
		[SerializeField]
		private bool _dontDestroyOnLoad;

		// Token: 0x040047E0 RID: 18400
		[Tooltip("Open the control mapping screen immediately on start. Mainly used for testing.")]
		[SerializeField]
		private bool _openOnStart;

		// Token: 0x040047E1 RID: 18401
		[SerializeField]
		[Tooltip("The Layout of the Keyboard Maps to be displayed.")]
		private int _keyboardMapDefaultLayout;

		// Token: 0x040047E2 RID: 18402
		[Tooltip("The Layout of the Mouse Maps to be displayed.")]
		[SerializeField]
		private int _mouseMapDefaultLayout;

		// Token: 0x040047E3 RID: 18403
		[SerializeField]
		[Tooltip("The Layout of the Mouse Maps to be displayed.")]
		private int _joystickMapDefaultLayout;

		// Token: 0x040047E4 RID: 18404
		[SerializeField]
		private ControlMapper.MappingSet[] _mappingSets = new ControlMapper.MappingSet[]
		{
			ControlMapper.MappingSet.Default
		};

		// Token: 0x040047E5 RID: 18405
		[SerializeField]
		[Tooltip("Display a selectable list of Players. If your game only supports 1 player, you can disable this.")]
		private bool _showPlayers = true;

		// Token: 0x040047E6 RID: 18406
		[Tooltip("Display the Controller column for input mapping.")]
		[SerializeField]
		private bool _showControllers = true;

		// Token: 0x040047E7 RID: 18407
		[Tooltip("Display the Keyboard column for input mapping.")]
		[SerializeField]
		private bool _showKeyboard = true;

		// Token: 0x040047E8 RID: 18408
		[SerializeField]
		[Tooltip("Display the Mouse column for input mapping.")]
		private bool _showMouse = true;

		// Token: 0x040047E9 RID: 18409
		[SerializeField]
		[Tooltip("The maximum number of controllers allowed to be assigned to a Player. If set to any value other than 1, a selectable list of currently-assigned controller will be displayed to the user. [0 = infinite]")]
		private int _maxControllersPerPlayer = 1;

		// Token: 0x040047EA RID: 18410
		[Tooltip("Display section labels for each Action Category in the input field grid. Only applies if Action Categories are used to display the Action list.")]
		[SerializeField]
		private bool _showActionCategoryLabels;

		// Token: 0x040047EB RID: 18411
		[SerializeField]
		[Tooltip("The number of input fields to display for the keyboard. If you want to support alternate mappings on the same device, set this to 2 or more.")]
		private int _keyboardInputFieldCount = 2;

		// Token: 0x040047EC RID: 18412
		[Tooltip("The number of input fields to display for the mouse. If you want to support alternate mappings on the same device, set this to 2 or more.")]
		[SerializeField]
		private int _mouseInputFieldCount = 1;

		// Token: 0x040047ED RID: 18413
		[SerializeField]
		[Tooltip("The number of input fields to display for joysticks. If you want to support alternate mappings on the same device, set this to 2 or more.")]
		private int _controllerInputFieldCount = 1;

		// Token: 0x040047EE RID: 18414
		[SerializeField]
		[Tooltip("Display a full-axis input assignment field for every axis-type Action in the input field grid. Also displays an invert toggle for the user  to invert the full-axis assignment direction.\n\n*IMPORTANT*: This field is required if you have made any full-axis assignments in the Rewired Input Manager or in saved XML user data. Disabling this field when you have full-axis assignments will result in the inability for the user to view, remove, or modify these full-axis assignments. In addition, these assignments may cause conflicts when trying to remap the same axes to Actions.")]
		private bool _showFullAxisInputFields = true;

		// Token: 0x040047EF RID: 18415
		[Tooltip("Display a positive and negative input assignment field for every axis-type Action in the input field grid.\n\n*IMPORTANT*: These fields are required to assign buttons, keyboard keys, and hat or D-Pad directions to axis-type Actions. If you have made any split-axis assignments or button/key/D-pad assignments to axis-type Actions in the Rewired Input Manager or in saved XML user data, disabling these fields will result in the inability for the user to view, remove, or modify these assignments. In addition, these assignments may cause conflicts when trying to remap the same elements to Actions.")]
		[SerializeField]
		private bool _showSplitAxisInputFields = true;

		// Token: 0x040047F0 RID: 18416
		[SerializeField]
		[Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to make the conflicting assignment anyway.")]
		private bool _allowElementAssignmentConflicts;

		// Token: 0x040047F1 RID: 18417
		[SerializeField]
		[Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to swap conflicting assignments. This only applies to the first conflicting assignment found. This option will not be displayed if allowElementAssignmentConflicts is true.")]
		private bool _allowElementAssignmentSwap;

		// Token: 0x040047F2 RID: 18418
		[SerializeField]
		[Tooltip("The width in relative pixels of the Action label column.")]
		private int _actionLabelWidth = 200;

		// Token: 0x040047F3 RID: 18419
		[SerializeField]
		[Tooltip("The width in relative pixels of the Keyboard column.")]
		private int _keyboardColMaxWidth = 360;

		// Token: 0x040047F4 RID: 18420
		[SerializeField]
		[Tooltip("The width in relative pixels of the Mouse column.")]
		private int _mouseColMaxWidth = 200;

		// Token: 0x040047F5 RID: 18421
		[SerializeField]
		[Tooltip("The width in relative pixels of the Controller column.")]
		private int _controllerColMaxWidth = 200;

		// Token: 0x040047F6 RID: 18422
		[SerializeField]
		[Tooltip("The height in relative pixels of the input grid button rows.")]
		private int _inputRowHeight = 40;

		// Token: 0x040047F7 RID: 18423
		[SerializeField]
		[Tooltip("The padding of the input grid button rows.")]
		private RectOffset _inputRowPadding = new RectOffset();

		// Token: 0x040047F8 RID: 18424
		[SerializeField]
		[Tooltip("The width in relative pixels of spacing between input fields in a single column.")]
		private int _inputRowFieldSpacing;

		// Token: 0x040047F9 RID: 18425
		[Tooltip("The width in relative pixels of spacing between columns.")]
		[SerializeField]
		private int _inputColumnSpacing = 40;

		// Token: 0x040047FA RID: 18426
		[Tooltip("The height in relative pixels of the space between Action Category sections. Only applicable if Show Action Category Labels is checked.")]
		[SerializeField]
		private int _inputRowCategorySpacing = 20;

		// Token: 0x040047FB RID: 18427
		[Tooltip("The width in relative pixels of the invert toggle buttons.")]
		[SerializeField]
		private int _invertToggleWidth = 40;

		// Token: 0x040047FC RID: 18428
		[Tooltip("The width in relative pixels of generated popup windows.")]
		[SerializeField]
		private int _defaultWindowWidth = 500;

		// Token: 0x040047FD RID: 18429
		[Tooltip("The height in relative pixels of generated popup windows.")]
		[SerializeField]
		private int _defaultWindowHeight = 400;

		// Token: 0x040047FE RID: 18430
		[Tooltip("The time in seconds the user has to press an element on a controller when assigning a controller to a Player. If this time elapses with no user input a controller, the assignment will be canceled.")]
		[SerializeField]
		private float _controllerAssignmentTimeout = 5f;

		// Token: 0x040047FF RID: 18431
		[SerializeField]
		[Tooltip("The time in seconds the user has to press an element on a controller while waiting for axes to be centered before assigning input.")]
		private float _preInputAssignmentTimeout = 5f;

		// Token: 0x04004800 RID: 18432
		[SerializeField]
		[Tooltip("The time in seconds the user has to press an element on a controller when assigning input. If this time elapses with no user input on the target controller, the assignment will be canceled.")]
		private float _inputAssignmentTimeout = 5f;

		// Token: 0x04004801 RID: 18433
		[SerializeField]
		[Tooltip("The time in seconds the user has to press an element on a controller during calibration.")]
		private float _axisCalibrationTimeout = 5f;

		// Token: 0x04004802 RID: 18434
		[Tooltip("If checked, mouse X-axis movement will always be ignored during input assignment. Check this if you don't want the horizontal mouse axis to be user-assignable to any Actions.")]
		[SerializeField]
		private bool _ignoreMouseXAxisAssignment = true;

		// Token: 0x04004803 RID: 18435
		[Tooltip("If checked, mouse Y-axis movement will always be ignored during input assignment. Check this if you don't want the vertical mouse axis to be user-assignable to any Actions.")]
		[SerializeField]
		private bool _ignoreMouseYAxisAssignment = true;

		// Token: 0x04004804 RID: 18436
		[Tooltip("An Action that when activated will alternately close or open the main screen as long as no popup windows are open.")]
		[SerializeField]
		private int _screenToggleAction = -1;

		// Token: 0x04004805 RID: 18437
		[SerializeField]
		[Tooltip("An Action that when activated will open the main screen if it is closed.")]
		private int _screenOpenAction = -1;

		// Token: 0x04004806 RID: 18438
		[SerializeField]
		[Tooltip("An Action that when activated will close the main screen as long as no popup windows are open.")]
		private int _screenCloseAction = -1;

		// Token: 0x04004807 RID: 18439
		[Tooltip("An Action that when activated will cancel and close any open popup window. Use with care because the element assigned to this Action can never be mapped by the user (because it would just cancel his assignment).")]
		[SerializeField]
		private int _universalCancelAction = -1;

		// Token: 0x04004808 RID: 18440
		[SerializeField]
		[Tooltip("If enabled, Universal Cancel will also close the main screen if pressed when no windows are open.")]
		private bool _universalCancelClosesScreen = true;

		// Token: 0x04004809 RID: 18441
		[Tooltip("If checked, controls will be displayed which will allow the user to customize certain Input Behavior settings.")]
		[SerializeField]
		private bool _showInputBehaviorSettings;

		// Token: 0x0400480A RID: 18442
		[Tooltip("Customizable settings for user-modifiable Input Behaviors. This can be used for settings like Mouse Look Sensitivity.")]
		[SerializeField]
		private ControlMapper.InputBehaviorSettings[] _inputBehaviorSettings;

		// Token: 0x0400480B RID: 18443
		[Tooltip("If enabled, UI elements will be themed based on the settings in Theme Settings.")]
		[SerializeField]
		private bool _useThemeSettings = true;

		// Token: 0x0400480C RID: 18444
		[SerializeField]
		[Tooltip("Must be assigned a ThemeSettings object. Used to theme UI elements.")]
		private ThemeSettings _themeSettings;

		// Token: 0x0400480D RID: 18445
		[Tooltip("Must be assigned a LanguageData object. Used to retrieve language entries for UI elements.")]
		[SerializeField]
		private LanguageDataBase _language;

		// Token: 0x0400480E RID: 18446
		[Tooltip("A list of prefabs. You should not have to modify this.")]
		[SerializeField]
		private ControlMapper.Prefabs prefabs;

		// Token: 0x0400480F RID: 18447
		[SerializeField]
		[Tooltip("A list of references to elements in the hierarchy. You should not have to modify this.")]
		private ControlMapper.References references;

		// Token: 0x04004810 RID: 18448
		[SerializeField]
		[Tooltip("Show the label for the Players button group?")]
		private bool _showPlayersGroupLabel = true;

		// Token: 0x04004811 RID: 18449
		[Tooltip("Show the label for the Controller button group?")]
		[SerializeField]
		private bool _showControllerGroupLabel = true;

		// Token: 0x04004812 RID: 18450
		[SerializeField]
		[Tooltip("Show the label for the Assigned Controllers button group?")]
		private bool _showAssignedControllersGroupLabel = true;

		// Token: 0x04004813 RID: 18451
		[SerializeField]
		[Tooltip("Show the label for the Settings button group?")]
		private bool _showSettingsGroupLabel = true;

		// Token: 0x04004814 RID: 18452
		[SerializeField]
		[Tooltip("Show the label for the Map Categories button group?")]
		private bool _showMapCategoriesGroupLabel = true;

		// Token: 0x04004815 RID: 18453
		[SerializeField]
		[Tooltip("Show the label for the current controller name?")]
		private bool _showControllerNameLabel = true;

		// Token: 0x04004816 RID: 18454
		[Tooltip("Show the Assigned Controllers group? If joystick auto-assignment is enabled in the Rewired Input Manager and the max joysticks per player is set to any value other than 1, the Assigned Controllers group will always be displayed.")]
		[SerializeField]
		private bool _showAssignedControllers = true;

		// Token: 0x04004817 RID: 18455
		private Action _ScreenClosedEvent;

		// Token: 0x04004818 RID: 18456
		private Action _ScreenOpenedEvent;

		// Token: 0x04004819 RID: 18457
		private Action _PopupWindowOpenedEvent;

		// Token: 0x0400481A RID: 18458
		private Action _PopupWindowClosedEvent;

		// Token: 0x0400481B RID: 18459
		private Action _InputPollingStartedEvent;

		// Token: 0x0400481C RID: 18460
		private Action _InputPollingEndedEvent;

		// Token: 0x0400481D RID: 18461
		[Tooltip("Event sent when the UI is closed.")]
		[SerializeField]
		private UnityEvent _onScreenClosed;

		// Token: 0x0400481E RID: 18462
		[Tooltip("Event sent when the UI is opened.")]
		[SerializeField]
		private UnityEvent _onScreenOpened;

		// Token: 0x0400481F RID: 18463
		[Tooltip("Event sent when a popup window is closed.")]
		[SerializeField]
		private UnityEvent _onPopupWindowClosed;

		// Token: 0x04004820 RID: 18464
		[Tooltip("Event sent when a popup window is opened.")]
		[SerializeField]
		private UnityEvent _onPopupWindowOpened;

		// Token: 0x04004821 RID: 18465
		[SerializeField]
		[Tooltip("Event sent when polling for input has started.")]
		private UnityEvent _onInputPollingStarted;

		// Token: 0x04004822 RID: 18466
		[SerializeField]
		[Tooltip("Event sent when polling for input has ended.")]
		private UnityEvent _onInputPollingEnded;

		// Token: 0x04004823 RID: 18467
		private static ControlMapper Instance;

		// Token: 0x04004824 RID: 18468
		private bool initialized;

		// Token: 0x04004825 RID: 18469
		private int playerCount;

		// Token: 0x04004826 RID: 18470
		private ControlMapper.InputGrid inputGrid;

		// Token: 0x04004827 RID: 18471
		private ControlMapper.WindowManager windowManager;

		// Token: 0x04004828 RID: 18472
		private int currentPlayerId;

		// Token: 0x04004829 RID: 18473
		private int currentMapCategoryId;

		// Token: 0x0400482A RID: 18474
		private List<ControlMapper.GUIButton> playerButtons;

		// Token: 0x0400482B RID: 18475
		private List<ControlMapper.GUIButton> mapCategoryButtons;

		// Token: 0x0400482C RID: 18476
		private List<ControlMapper.GUIButton> assignedControllerButtons;

		// Token: 0x0400482D RID: 18477
		private ControlMapper.GUIButton assignedControllerButtonsPlaceholder;

		// Token: 0x0400482E RID: 18478
		private List<GameObject> miscInstantiatedObjects;

		// Token: 0x0400482F RID: 18479
		private GameObject canvas;

		// Token: 0x04004830 RID: 18480
		private GameObject lastUISelection;

		// Token: 0x04004831 RID: 18481
		private int currentJoystickId = -1;

		// Token: 0x04004832 RID: 18482
		private float blockInputOnFocusEndTime;

		// Token: 0x04004833 RID: 18483
		private bool isPollingForInput;

		// Token: 0x04004834 RID: 18484
		private ControlMapper.InputMapping pendingInputMapping;

		// Token: 0x04004835 RID: 18485
		private ControlMapper.AxisCalibrator pendingAxisCalibration;

		// Token: 0x04004836 RID: 18486
		private Action<InputFieldInfo> inputFieldActivatedDelegate;

		// Token: 0x04004837 RID: 18487
		private Action<ToggleInfo, bool> inputFieldInvertToggleStateChangedDelegate;

		// Token: 0x04004838 RID: 18488
		private Action _restoreDefaultsDelegate;

		// Token: 0x0200084E RID: 2126
		private abstract class GUIElement
		{
			// Token: 0x170003A8 RID: 936
			// (get) Token: 0x06002BA0 RID: 11168 RVA: 0x00206E6E File Offset: 0x0020506E
			// (set) Token: 0x06002BA1 RID: 11169 RVA: 0x00206E76 File Offset: 0x00205076
			public RectTransform rectTransform { get; private set; }

			// Token: 0x06002BA2 RID: 11170 RVA: 0x00206E80 File Offset: 0x00205080
			public GUIElement(GameObject gameObject)
			{
				if (gameObject == null)
				{
					Debug.LogError("Rewired Control Mapper: gameObject is null!");
					return;
				}
				this.selectable = gameObject.GetComponent<Selectable>();
				if (this.selectable == null)
				{
					Debug.LogError("Rewired Control Mapper: Selectable is null!");
					return;
				}
				this.gameObject = gameObject;
				this.rectTransform = gameObject.GetComponent<RectTransform>();
				this.text = UnityTools.GetComponentInSelfOrChildren<TMP_Text>(gameObject);
				this.uiElementInfo = gameObject.GetComponent<UIElementInfo>();
				this.children = new List<ControlMapper.GUIElement>();
			}

			// Token: 0x06002BA3 RID: 11171 RVA: 0x00206F04 File Offset: 0x00205104
			public GUIElement(Selectable selectable, TMP_Text label)
			{
				if (selectable == null)
				{
					Debug.LogError("Rewired Control Mapper: Selectable is null!");
					return;
				}
				this.selectable = selectable;
				this.gameObject = selectable.gameObject;
				this.rectTransform = this.gameObject.GetComponent<RectTransform>();
				this.text = label;
				this.uiElementInfo = this.gameObject.GetComponent<UIElementInfo>();
				this.children = new List<ControlMapper.GUIElement>();
			}

			// Token: 0x06002BA4 RID: 11172 RVA: 0x00206F72 File Offset: 0x00205172
			public virtual void SetInteractible(bool state, bool playTransition)
			{
				this.SetInteractible(state, playTransition, false);
			}

			// Token: 0x06002BA5 RID: 11173 RVA: 0x00206F80 File Offset: 0x00205180
			public virtual void SetInteractible(bool state, bool playTransition, bool permanent)
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i] != null)
					{
						this.children[i].SetInteractible(state, playTransition, permanent);
					}
				}
				if (this.permanentStateSet)
				{
					return;
				}
				if (this.selectable == null)
				{
					return;
				}
				if (permanent)
				{
					this.permanentStateSet = true;
				}
				if (this.selectable.interactable == state)
				{
					return;
				}
				UITools.SetInteractable(this.selectable, state, playTransition);
			}

			// Token: 0x06002BA6 RID: 11174 RVA: 0x00207004 File Offset: 0x00205204
			public virtual void SetTextWidth(int value)
			{
				if (this.text == null)
				{
					return;
				}
				LayoutElement layoutElement = this.text.GetComponent<LayoutElement>();
				if (layoutElement == null)
				{
					layoutElement = this.text.gameObject.AddComponent<LayoutElement>();
				}
				layoutElement.preferredWidth = (float)value;
			}

			// Token: 0x06002BA7 RID: 11175 RVA: 0x00207050 File Offset: 0x00205250
			public virtual void SetFirstChildObjectWidth(ControlMapper.LayoutElementSizeType type, int value)
			{
				if (this.rectTransform.childCount == 0)
				{
					return;
				}
				Transform child = this.rectTransform.GetChild(0);
				LayoutElement layoutElement = child.GetComponent<LayoutElement>();
				if (layoutElement == null)
				{
					layoutElement = child.gameObject.AddComponent<LayoutElement>();
				}
				if (type == ControlMapper.LayoutElementSizeType.MinSize)
				{
					layoutElement.minWidth = (float)value;
					return;
				}
				if (type == ControlMapper.LayoutElementSizeType.PreferredSize)
				{
					layoutElement.preferredWidth = (float)value;
					return;
				}
				throw new NotImplementedException();
			}

			// Token: 0x06002BA8 RID: 11176 RVA: 0x002070B2 File Offset: 0x002052B2
			public virtual void SetLabel(string label)
			{
				if (this.text == null)
				{
					return;
				}
				this.text.text = label;
			}

			// Token: 0x06002BA9 RID: 11177 RVA: 0x002070CF File Offset: 0x002052CF
			public virtual string GetLabel()
			{
				if (this.text == null)
				{
					return string.Empty;
				}
				return this.text.text;
			}

			// Token: 0x06002BAA RID: 11178 RVA: 0x002070F0 File Offset: 0x002052F0
			public virtual void AddChild(ControlMapper.GUIElement child)
			{
				this.children.Add(child);
			}

			// Token: 0x06002BAB RID: 11179 RVA: 0x002070FE File Offset: 0x002052FE
			public void SetElementInfoData(string identifier, int intData)
			{
				if (this.uiElementInfo == null)
				{
					return;
				}
				this.uiElementInfo.identifier = identifier;
				this.uiElementInfo.intData = intData;
			}

			// Token: 0x06002BAC RID: 11180 RVA: 0x00207127 File Offset: 0x00205327
			public virtual void SetActive(bool state)
			{
				if (this.gameObject == null)
				{
					return;
				}
				this.gameObject.SetActive(state);
			}

			// Token: 0x06002BAD RID: 11181 RVA: 0x00207144 File Offset: 0x00205344
			protected virtual bool Init()
			{
				bool result = true;
				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i] != null && !this.children[i].Init())
					{
						result = false;
					}
				}
				if (this.selectable == null)
				{
					Debug.LogError("Rewired Control Mapper: UI Element is missing Selectable component!");
					result = false;
				}
				if (this.rectTransform == null)
				{
					Debug.LogError("Rewired Control Mapper: UI Element is missing RectTransform component!");
					result = false;
				}
				if (this.uiElementInfo == null)
				{
					Debug.LogError("Rewired Control Mapper: UI Element is missing UIElementInfo component!");
					result = false;
				}
				return result;
			}

			// Token: 0x04004839 RID: 18489
			public readonly GameObject gameObject;

			// Token: 0x0400483A RID: 18490
			protected readonly TMP_Text text;

			// Token: 0x0400483B RID: 18491
			public readonly Selectable selectable;

			// Token: 0x0400483C RID: 18492
			protected readonly UIElementInfo uiElementInfo;

			// Token: 0x0400483D RID: 18493
			protected bool permanentStateSet;

			// Token: 0x0400483E RID: 18494
			protected readonly List<ControlMapper.GUIElement> children;
		}

		// Token: 0x0200084F RID: 2127
		private class GUIButton : ControlMapper.GUIElement
		{
			// Token: 0x170003A9 RID: 937
			// (get) Token: 0x06002BAE RID: 11182 RVA: 0x002071DB File Offset: 0x002053DB
			protected Button button
			{
				get
				{
					return this.selectable as Button;
				}
			}

			// Token: 0x170003AA RID: 938
			// (get) Token: 0x06002BAF RID: 11183 RVA: 0x002071E8 File Offset: 0x002053E8
			public ButtonInfo buttonInfo
			{
				get
				{
					return this.uiElementInfo as ButtonInfo;
				}
			}

			// Token: 0x06002BB0 RID: 11184 RVA: 0x002071F5 File Offset: 0x002053F5
			public GUIButton(GameObject gameObject) : base(gameObject)
			{
				this.Init();
			}

			// Token: 0x06002BB1 RID: 11185 RVA: 0x00207205 File Offset: 0x00205405
			public GUIButton(Button button, TMP_Text label) : base(button, label)
			{
				this.Init();
			}

			// Token: 0x06002BB2 RID: 11186 RVA: 0x00207216 File Offset: 0x00205416
			public void SetButtonInfoData(string identifier, int intData)
			{
				base.SetElementInfoData(identifier, intData);
			}

			// Token: 0x06002BB3 RID: 11187 RVA: 0x00207220 File Offset: 0x00205420
			public void SetOnClickCallback(Action<ButtonInfo> callback)
			{
				if (this.button == null)
				{
					return;
				}
				this.button.onClick.AddListener(delegate()
				{
					callback.Invoke(this.buttonInfo);
				});
			}
		}

		// Token: 0x02000851 RID: 2129
		private class GUIInputField : ControlMapper.GUIElement
		{
			// Token: 0x170003AB RID: 939
			// (get) Token: 0x06002BB6 RID: 11190 RVA: 0x002071DB File Offset: 0x002053DB
			protected Button button
			{
				get
				{
					return this.selectable as Button;
				}
			}

			// Token: 0x170003AC RID: 940
			// (get) Token: 0x06002BB7 RID: 11191 RVA: 0x00207284 File Offset: 0x00205484
			public InputFieldInfo fieldInfo
			{
				get
				{
					return this.uiElementInfo as InputFieldInfo;
				}
			}

			// Token: 0x170003AD RID: 941
			// (get) Token: 0x06002BB8 RID: 11192 RVA: 0x00207291 File Offset: 0x00205491
			public bool hasToggle
			{
				get
				{
					return this.toggle != null;
				}
			}

			// Token: 0x170003AE RID: 942
			// (get) Token: 0x06002BB9 RID: 11193 RVA: 0x0020729C File Offset: 0x0020549C
			// (set) Token: 0x06002BBA RID: 11194 RVA: 0x002072A4 File Offset: 0x002054A4
			public ControlMapper.GUIToggle toggle { get; private set; }

			// Token: 0x170003AF RID: 943
			// (get) Token: 0x06002BBB RID: 11195 RVA: 0x002072AD File Offset: 0x002054AD
			// (set) Token: 0x06002BBC RID: 11196 RVA: 0x002072CA File Offset: 0x002054CA
			public int actionElementMapId
			{
				get
				{
					if (this.fieldInfo == null)
					{
						return -1;
					}
					return this.fieldInfo.actionElementMapId;
				}
				set
				{
					if (this.fieldInfo == null)
					{
						return;
					}
					this.fieldInfo.actionElementMapId = value;
				}
			}

			// Token: 0x170003B0 RID: 944
			// (get) Token: 0x06002BBD RID: 11197 RVA: 0x002072E7 File Offset: 0x002054E7
			// (set) Token: 0x06002BBE RID: 11198 RVA: 0x00207304 File Offset: 0x00205504
			public int controllerId
			{
				get
				{
					if (this.fieldInfo == null)
					{
						return -1;
					}
					return this.fieldInfo.controllerId;
				}
				set
				{
					if (this.fieldInfo == null)
					{
						return;
					}
					this.fieldInfo.controllerId = value;
				}
			}

			// Token: 0x06002BBF RID: 11199 RVA: 0x002071F5 File Offset: 0x002053F5
			public GUIInputField(GameObject gameObject) : base(gameObject)
			{
				this.Init();
			}

			// Token: 0x06002BC0 RID: 11200 RVA: 0x00207205 File Offset: 0x00205405
			public GUIInputField(Button button, TMP_Text label) : base(button, label)
			{
				this.Init();
			}

			// Token: 0x06002BC1 RID: 11201 RVA: 0x00207324 File Offset: 0x00205524
			public void SetFieldInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
			{
				base.SetElementInfoData(string.Empty, intData);
				if (this.fieldInfo == null)
				{
					return;
				}
				this.fieldInfo.actionId = actionId;
				this.fieldInfo.axisRange = axisRange;
				this.fieldInfo.controllerType = controllerType;
			}

			// Token: 0x06002BC2 RID: 11202 RVA: 0x00207374 File Offset: 0x00205574
			public void SetOnClickCallback(Action<InputFieldInfo> callback)
			{
				if (this.button == null)
				{
					return;
				}
				this.button.onClick.AddListener(delegate()
				{
					callback.Invoke(this.fieldInfo);
				});
			}

			// Token: 0x06002BC3 RID: 11203 RVA: 0x002073C0 File Offset: 0x002055C0
			public virtual void SetInteractable(bool state, bool playTransition, bool permanent)
			{
				if (this.permanentStateSet)
				{
					return;
				}
				if (this.hasToggle && !state)
				{
					this.toggle.SetInteractible(state, playTransition, permanent);
				}
				base.SetInteractible(state, playTransition, permanent);
			}

			// Token: 0x06002BC4 RID: 11204 RVA: 0x002073ED File Offset: 0x002055ED
			public void AddToggle(ControlMapper.GUIToggle toggle)
			{
				if (toggle == null)
				{
					return;
				}
				this.toggle = toggle;
			}
		}

		// Token: 0x02000853 RID: 2131
		private class GUIToggle : ControlMapper.GUIElement
		{
			// Token: 0x170003B1 RID: 945
			// (get) Token: 0x06002BC7 RID: 11207 RVA: 0x00207412 File Offset: 0x00205612
			protected Toggle toggle
			{
				get
				{
					return this.selectable as Toggle;
				}
			}

			// Token: 0x170003B2 RID: 946
			// (get) Token: 0x06002BC8 RID: 11208 RVA: 0x0020741F File Offset: 0x0020561F
			public ToggleInfo toggleInfo
			{
				get
				{
					return this.uiElementInfo as ToggleInfo;
				}
			}

			// Token: 0x170003B3 RID: 947
			// (get) Token: 0x06002BC9 RID: 11209 RVA: 0x0020742C File Offset: 0x0020562C
			// (set) Token: 0x06002BCA RID: 11210 RVA: 0x00207449 File Offset: 0x00205649
			public int actionElementMapId
			{
				get
				{
					if (this.toggleInfo == null)
					{
						return -1;
					}
					return this.toggleInfo.actionElementMapId;
				}
				set
				{
					if (this.toggleInfo == null)
					{
						return;
					}
					this.toggleInfo.actionElementMapId = value;
				}
			}

			// Token: 0x06002BCB RID: 11211 RVA: 0x002071F5 File Offset: 0x002053F5
			public GUIToggle(GameObject gameObject) : base(gameObject)
			{
				this.Init();
			}

			// Token: 0x06002BCC RID: 11212 RVA: 0x00207205 File Offset: 0x00205405
			public GUIToggle(Toggle toggle, TMP_Text label) : base(toggle, label)
			{
				this.Init();
			}

			// Token: 0x06002BCD RID: 11213 RVA: 0x00207468 File Offset: 0x00205668
			public void SetToggleInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
			{
				base.SetElementInfoData(string.Empty, intData);
				if (this.toggleInfo == null)
				{
					return;
				}
				this.toggleInfo.actionId = actionId;
				this.toggleInfo.axisRange = axisRange;
				this.toggleInfo.controllerType = controllerType;
			}

			// Token: 0x06002BCE RID: 11214 RVA: 0x002074B8 File Offset: 0x002056B8
			public void SetOnSubmitCallback(Action<ToggleInfo, bool> callback)
			{
				if (this.toggle == null)
				{
					return;
				}
				EventTrigger eventTrigger = this.toggle.GetComponent<EventTrigger>();
				if (eventTrigger == null)
				{
					eventTrigger = this.toggle.gameObject.AddComponent<EventTrigger>();
				}
				EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
				triggerEvent.AddListener(delegate(BaseEventData data)
				{
					PointerEventData pointerEventData = data as PointerEventData;
					if (pointerEventData != null && pointerEventData.button != null)
					{
						return;
					}
					callback.Invoke(this.toggleInfo, this.toggle.isOn);
				});
				EventTrigger.Entry entry = new EventTrigger.Entry
				{
					callback = triggerEvent,
					eventID = 15
				};
				EventTrigger.Entry entry2 = new EventTrigger.Entry
				{
					callback = triggerEvent,
					eventID = 4
				};
				if (eventTrigger.triggers != null)
				{
					eventTrigger.triggers.Clear();
				}
				else
				{
					eventTrigger.triggers = new List<EventTrigger.Entry>();
				}
				eventTrigger.triggers.Add(entry);
				eventTrigger.triggers.Add(entry2);
			}

			// Token: 0x06002BCF RID: 11215 RVA: 0x00207589 File Offset: 0x00205789
			public void SetToggleState(bool state)
			{
				if (this.toggle == null)
				{
					return;
				}
				this.toggle.isOn = state;
			}
		}

		// Token: 0x02000855 RID: 2133
		private class GUILabel
		{
			// Token: 0x170003B4 RID: 948
			// (get) Token: 0x06002BD2 RID: 11218 RVA: 0x002075EE File Offset: 0x002057EE
			// (set) Token: 0x06002BD3 RID: 11219 RVA: 0x002075F6 File Offset: 0x002057F6
			public GameObject gameObject { get; private set; }

			// Token: 0x170003B5 RID: 949
			// (get) Token: 0x06002BD4 RID: 11220 RVA: 0x002075FF File Offset: 0x002057FF
			// (set) Token: 0x06002BD5 RID: 11221 RVA: 0x00207607 File Offset: 0x00205807
			private TMP_Text text { get; set; }

			// Token: 0x170003B6 RID: 950
			// (get) Token: 0x06002BD6 RID: 11222 RVA: 0x00207610 File Offset: 0x00205810
			// (set) Token: 0x06002BD7 RID: 11223 RVA: 0x00207618 File Offset: 0x00205818
			public RectTransform rectTransform { get; private set; }

			// Token: 0x06002BD8 RID: 11224 RVA: 0x00207621 File Offset: 0x00205821
			public GUILabel(GameObject gameObject)
			{
				if (gameObject == null)
				{
					Debug.LogError("Rewired Control Mapper: gameObject is null!");
					return;
				}
				this.text = UnityTools.GetComponentInSelfOrChildren<TMP_Text>(gameObject);
				this.Check();
			}

			// Token: 0x06002BD9 RID: 11225 RVA: 0x00207650 File Offset: 0x00205850
			public GUILabel(TMP_Text label)
			{
				this.text = label;
				this.Check();
			}

			// Token: 0x06002BDA RID: 11226 RVA: 0x00207666 File Offset: 0x00205866
			public void SetSize(int width, int height)
			{
				if (this.text == null)
				{
					return;
				}
				this.rectTransform.SetSizeWithCurrentAnchors(0, (float)width);
				this.rectTransform.SetSizeWithCurrentAnchors(1, (float)height);
			}

			// Token: 0x06002BDB RID: 11227 RVA: 0x00207693 File Offset: 0x00205893
			public void SetWidth(int width)
			{
				if (this.text == null)
				{
					return;
				}
				this.rectTransform.SetSizeWithCurrentAnchors(0, (float)width);
			}

			// Token: 0x06002BDC RID: 11228 RVA: 0x002076B2 File Offset: 0x002058B2
			public void SetHeight(int height)
			{
				if (this.text == null)
				{
					return;
				}
				this.rectTransform.SetSizeWithCurrentAnchors(1, (float)height);
			}

			// Token: 0x06002BDD RID: 11229 RVA: 0x002076D1 File Offset: 0x002058D1
			public void SetLabel(string label)
			{
				if (this.text == null)
				{
					return;
				}
				this.text.text = label;
			}

			// Token: 0x06002BDE RID: 11230 RVA: 0x002076EE File Offset: 0x002058EE
			public void SetFontStyle(FontStyles style)
			{
				if (this.text == null)
				{
					return;
				}
				this.text.fontStyle = style;
			}

			// Token: 0x06002BDF RID: 11231 RVA: 0x0020770B File Offset: 0x0020590B
			public void SetTextAlignment(TextAlignmentOptions alignment)
			{
				if (this.text == null)
				{
					return;
				}
				this.text.alignment = alignment;
			}

			// Token: 0x06002BE0 RID: 11232 RVA: 0x00207728 File Offset: 0x00205928
			public void SetActive(bool state)
			{
				if (this.gameObject == null)
				{
					return;
				}
				this.gameObject.SetActive(state);
			}

			// Token: 0x06002BE1 RID: 11233 RVA: 0x00207748 File Offset: 0x00205948
			private bool Check()
			{
				bool result = true;
				if (this.text == null)
				{
					Debug.LogError("Rewired Control Mapper: Button is missing Text child component!");
					result = false;
				}
				this.gameObject = this.text.gameObject;
				this.rectTransform = this.text.GetComponent<RectTransform>();
				return result;
			}
		}

		// Token: 0x02000856 RID: 2134
		[Serializable]
		public class MappingSet
		{
			// Token: 0x170003B7 RID: 951
			// (get) Token: 0x06002BE2 RID: 11234 RVA: 0x00207794 File Offset: 0x00205994
			public int mapCategoryId
			{
				get
				{
					return this._mapCategoryId;
				}
			}

			// Token: 0x170003B8 RID: 952
			// (get) Token: 0x06002BE3 RID: 11235 RVA: 0x0020779C File Offset: 0x0020599C
			public ControlMapper.MappingSet.ActionListMode actionListMode
			{
				get
				{
					return this._actionListMode;
				}
			}

			// Token: 0x170003B9 RID: 953
			// (get) Token: 0x06002BE4 RID: 11236 RVA: 0x002077A4 File Offset: 0x002059A4
			public IList<int> actionCategoryIds
			{
				get
				{
					if (this._actionCategoryIds == null)
					{
						return null;
					}
					if (this._actionCategoryIdsReadOnly == null)
					{
						this._actionCategoryIdsReadOnly = new ReadOnlyCollection<int>(this._actionCategoryIds);
					}
					return this._actionCategoryIdsReadOnly;
				}
			}

			// Token: 0x170003BA RID: 954
			// (get) Token: 0x06002BE5 RID: 11237 RVA: 0x002077CF File Offset: 0x002059CF
			public IList<int> actionIds
			{
				get
				{
					if (this._actionIds == null)
					{
						return null;
					}
					if (this._actionIdsReadOnly == null)
					{
						this._actionIdsReadOnly = new ReadOnlyCollection<int>(this._actionIds);
					}
					return this._actionIds;
				}
			}

			// Token: 0x170003BB RID: 955
			// (get) Token: 0x06002BE6 RID: 11238 RVA: 0x002077FA File Offset: 0x002059FA
			public bool isValid
			{
				get
				{
					return this._mapCategoryId >= 0 && ReInput.mapping.GetMapCategory(this._mapCategoryId) != null;
				}
			}

			// Token: 0x06002BE7 RID: 11239 RVA: 0x0020781A File Offset: 0x00205A1A
			public MappingSet()
			{
				this._mapCategoryId = -1;
				this._actionCategoryIds = new int[0];
				this._actionIds = new int[0];
				this._actionListMode = ControlMapper.MappingSet.ActionListMode.ActionCategory;
			}

			// Token: 0x06002BE8 RID: 11240 RVA: 0x00207848 File Offset: 0x00205A48
			private MappingSet(int mapCategoryId, ControlMapper.MappingSet.ActionListMode actionListMode, int[] actionCategoryIds, int[] actionIds)
			{
				this._mapCategoryId = mapCategoryId;
				this._actionListMode = actionListMode;
				this._actionCategoryIds = actionCategoryIds;
				this._actionIds = actionIds;
			}

			// Token: 0x170003BC RID: 956
			// (get) Token: 0x06002BE9 RID: 11241 RVA: 0x0020786D File Offset: 0x00205A6D
			public static ControlMapper.MappingSet Default
			{
				get
				{
					return new ControlMapper.MappingSet(0, ControlMapper.MappingSet.ActionListMode.ActionCategory, new int[1], new int[0]);
				}
			}

			// Token: 0x0400484A RID: 18506
			[SerializeField]
			[Tooltip("The Map Category that will be displayed to the user for remapping.")]
			private int _mapCategoryId;

			// Token: 0x0400484B RID: 18507
			[Tooltip("Choose whether you want to list Actions to display for this Map Category by individual Action or by all the Actions in an Action Category.")]
			[SerializeField]
			private ControlMapper.MappingSet.ActionListMode _actionListMode;

			// Token: 0x0400484C RID: 18508
			[SerializeField]
			private int[] _actionCategoryIds;

			// Token: 0x0400484D RID: 18509
			[SerializeField]
			private int[] _actionIds;

			// Token: 0x0400484E RID: 18510
			private IList<int> _actionCategoryIdsReadOnly;

			// Token: 0x0400484F RID: 18511
			private IList<int> _actionIdsReadOnly;

			// Token: 0x02000857 RID: 2135
			public enum ActionListMode
			{
				// Token: 0x04004851 RID: 18513
				ActionCategory,
				// Token: 0x04004852 RID: 18514
				Action
			}
		}

		// Token: 0x02000858 RID: 2136
		[Serializable]
		public class InputBehaviorSettings
		{
			// Token: 0x170003BD RID: 957
			// (get) Token: 0x06002BEA RID: 11242 RVA: 0x00207882 File Offset: 0x00205A82
			public int inputBehaviorId
			{
				get
				{
					return this._inputBehaviorId;
				}
			}

			// Token: 0x170003BE RID: 958
			// (get) Token: 0x06002BEB RID: 11243 RVA: 0x0020788A File Offset: 0x00205A8A
			public bool showJoystickAxisSensitivity
			{
				get
				{
					return this._showJoystickAxisSensitivity;
				}
			}

			// Token: 0x170003BF RID: 959
			// (get) Token: 0x06002BEC RID: 11244 RVA: 0x00207892 File Offset: 0x00205A92
			public bool showMouseXYAxisSensitivity
			{
				get
				{
					return this._showMouseXYAxisSensitivity;
				}
			}

			// Token: 0x170003C0 RID: 960
			// (get) Token: 0x06002BED RID: 11245 RVA: 0x0020789A File Offset: 0x00205A9A
			public string labelLanguageKey
			{
				get
				{
					return this._labelLanguageKey;
				}
			}

			// Token: 0x170003C1 RID: 961
			// (get) Token: 0x06002BEE RID: 11246 RVA: 0x002078A2 File Offset: 0x00205AA2
			public string joystickAxisSensitivityLabelLanguageKey
			{
				get
				{
					return this._joystickAxisSensitivityLabelLanguageKey;
				}
			}

			// Token: 0x170003C2 RID: 962
			// (get) Token: 0x06002BEF RID: 11247 RVA: 0x002078AA File Offset: 0x00205AAA
			public string mouseXYAxisSensitivityLabelLanguageKey
			{
				get
				{
					return this._mouseXYAxisSensitivityLabelLanguageKey;
				}
			}

			// Token: 0x170003C3 RID: 963
			// (get) Token: 0x06002BF0 RID: 11248 RVA: 0x002078B2 File Offset: 0x00205AB2
			public Sprite joystickAxisSensitivityIcon
			{
				get
				{
					return this._joystickAxisSensitivityIcon;
				}
			}

			// Token: 0x170003C4 RID: 964
			// (get) Token: 0x06002BF1 RID: 11249 RVA: 0x002078BA File Offset: 0x00205ABA
			public Sprite mouseXYAxisSensitivityIcon
			{
				get
				{
					return this._mouseXYAxisSensitivityIcon;
				}
			}

			// Token: 0x170003C5 RID: 965
			// (get) Token: 0x06002BF2 RID: 11250 RVA: 0x002078C2 File Offset: 0x00205AC2
			public float joystickAxisSensitivityMin
			{
				get
				{
					return this._joystickAxisSensitivityMin;
				}
			}

			// Token: 0x170003C6 RID: 966
			// (get) Token: 0x06002BF3 RID: 11251 RVA: 0x002078CA File Offset: 0x00205ACA
			public float joystickAxisSensitivityMax
			{
				get
				{
					return this._joystickAxisSensitivityMax;
				}
			}

			// Token: 0x170003C7 RID: 967
			// (get) Token: 0x06002BF4 RID: 11252 RVA: 0x002078D2 File Offset: 0x00205AD2
			public float mouseXYAxisSensitivityMin
			{
				get
				{
					return this._mouseXYAxisSensitivityMin;
				}
			}

			// Token: 0x170003C8 RID: 968
			// (get) Token: 0x06002BF5 RID: 11253 RVA: 0x002078DA File Offset: 0x00205ADA
			public float mouseXYAxisSensitivityMax
			{
				get
				{
					return this._mouseXYAxisSensitivityMax;
				}
			}

			// Token: 0x170003C9 RID: 969
			// (get) Token: 0x06002BF6 RID: 11254 RVA: 0x002078E2 File Offset: 0x00205AE2
			public bool isValid
			{
				get
				{
					return this._inputBehaviorId >= 0 && (this._showJoystickAxisSensitivity || this._showMouseXYAxisSensitivity);
				}
			}

			// Token: 0x04004853 RID: 18515
			[Tooltip("The Input Behavior that will be displayed to the user for modification.")]
			[SerializeField]
			private int _inputBehaviorId = -1;

			// Token: 0x04004854 RID: 18516
			[Tooltip("If checked, a slider will be displayed so the user can change this value.")]
			[SerializeField]
			private bool _showJoystickAxisSensitivity = true;

			// Token: 0x04004855 RID: 18517
			[Tooltip("If checked, a slider will be displayed so the user can change this value.")]
			[SerializeField]
			private bool _showMouseXYAxisSensitivity = true;

			// Token: 0x04004856 RID: 18518
			[Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed as the title for the Input Behavior control set. Otherwise, the name field of the InputBehavior will be used.")]
			[SerializeField]
			private string _labelLanguageKey = string.Empty;

			// Token: 0x04004857 RID: 18519
			[SerializeField]
			[Tooltip("If set to a non-blank value, this name will be displayed above the individual slider control. Otherwise, no name will be displayed.")]
			private string _joystickAxisSensitivityLabelLanguageKey = string.Empty;

			// Token: 0x04004858 RID: 18520
			[SerializeField]
			[Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed above the individual slider control. Otherwise, no name will be displayed.")]
			private string _mouseXYAxisSensitivityLabelLanguageKey = string.Empty;

			// Token: 0x04004859 RID: 18521
			[SerializeField]
			[Tooltip("The icon to display next to the slider. Set to none for no icon.")]
			private Sprite _joystickAxisSensitivityIcon;

			// Token: 0x0400485A RID: 18522
			[SerializeField]
			[Tooltip("The icon to display next to the slider. Set to none for no icon.")]
			private Sprite _mouseXYAxisSensitivityIcon;

			// Token: 0x0400485B RID: 18523
			[Tooltip("Minimum value the user is allowed to set for this property.")]
			[SerializeField]
			private float _joystickAxisSensitivityMin;

			// Token: 0x0400485C RID: 18524
			[SerializeField]
			[Tooltip("Maximum value the user is allowed to set for this property.")]
			private float _joystickAxisSensitivityMax = 2f;

			// Token: 0x0400485D RID: 18525
			[Tooltip("Minimum value the user is allowed to set for this property.")]
			[SerializeField]
			private float _mouseXYAxisSensitivityMin;

			// Token: 0x0400485E RID: 18526
			[Tooltip("Maximum value the user is allowed to set for this property.")]
			[SerializeField]
			private float _mouseXYAxisSensitivityMax = 2f;
		}

		// Token: 0x02000859 RID: 2137
		[Serializable]
		private class Prefabs
		{
			// Token: 0x170003CA RID: 970
			// (get) Token: 0x06002BF8 RID: 11256 RVA: 0x0020795F File Offset: 0x00205B5F
			public GameObject button
			{
				get
				{
					return this._button;
				}
			}

			// Token: 0x170003CB RID: 971
			// (get) Token: 0x06002BF9 RID: 11257 RVA: 0x00207967 File Offset: 0x00205B67
			public GameObject fitButton
			{
				get
				{
					return this._fitButton;
				}
			}

			// Token: 0x170003CC RID: 972
			// (get) Token: 0x06002BFA RID: 11258 RVA: 0x0020796F File Offset: 0x00205B6F
			public GameObject inputGridLabel
			{
				get
				{
					return this._inputGridLabel;
				}
			}

			// Token: 0x170003CD RID: 973
			// (get) Token: 0x06002BFB RID: 11259 RVA: 0x00207977 File Offset: 0x00205B77
			public GameObject inputGridHeaderLabel
			{
				get
				{
					return this._inputGridHeaderLabel;
				}
			}

			// Token: 0x170003CE RID: 974
			// (get) Token: 0x06002BFC RID: 11260 RVA: 0x0020797F File Offset: 0x00205B7F
			public GameObject inputGridFieldButton
			{
				get
				{
					return this._inputGridFieldButton;
				}
			}

			// Token: 0x170003CF RID: 975
			// (get) Token: 0x06002BFD RID: 11261 RVA: 0x00207987 File Offset: 0x00205B87
			public GameObject inputGridFieldInvertToggle
			{
				get
				{
					return this._inputGridFieldInvertToggle;
				}
			}

			// Token: 0x170003D0 RID: 976
			// (get) Token: 0x06002BFE RID: 11262 RVA: 0x0020798F File Offset: 0x00205B8F
			public GameObject window
			{
				get
				{
					return this._window;
				}
			}

			// Token: 0x170003D1 RID: 977
			// (get) Token: 0x06002BFF RID: 11263 RVA: 0x00207997 File Offset: 0x00205B97
			public GameObject windowTitleText
			{
				get
				{
					return this._windowTitleText;
				}
			}

			// Token: 0x170003D2 RID: 978
			// (get) Token: 0x06002C00 RID: 11264 RVA: 0x0020799F File Offset: 0x00205B9F
			public GameObject windowContentText
			{
				get
				{
					return this._windowContentText;
				}
			}

			// Token: 0x170003D3 RID: 979
			// (get) Token: 0x06002C01 RID: 11265 RVA: 0x002079A7 File Offset: 0x00205BA7
			public GameObject fader
			{
				get
				{
					return this._fader;
				}
			}

			// Token: 0x170003D4 RID: 980
			// (get) Token: 0x06002C02 RID: 11266 RVA: 0x002079AF File Offset: 0x00205BAF
			public GameObject calibrationWindow
			{
				get
				{
					return this._calibrationWindow;
				}
			}

			// Token: 0x170003D5 RID: 981
			// (get) Token: 0x06002C03 RID: 11267 RVA: 0x002079B7 File Offset: 0x00205BB7
			public GameObject inputBehaviorsWindow
			{
				get
				{
					return this._inputBehaviorsWindow;
				}
			}

			// Token: 0x170003D6 RID: 982
			// (get) Token: 0x06002C04 RID: 11268 RVA: 0x002079BF File Offset: 0x00205BBF
			public GameObject centerStickGraphic
			{
				get
				{
					return this._centerStickGraphic;
				}
			}

			// Token: 0x170003D7 RID: 983
			// (get) Token: 0x06002C05 RID: 11269 RVA: 0x002079C7 File Offset: 0x00205BC7
			public GameObject moveStickGraphic
			{
				get
				{
					return this._moveStickGraphic;
				}
			}

			// Token: 0x06002C06 RID: 11270 RVA: 0x002079D0 File Offset: 0x00205BD0
			public bool Check()
			{
				return !(this._button == null) && !(this._fitButton == null) && !(this._inputGridLabel == null) && !(this._inputGridHeaderLabel == null) && !(this._inputGridFieldButton == null) && !(this._inputGridFieldInvertToggle == null) && !(this._window == null) && !(this._windowTitleText == null) && !(this._windowContentText == null) && !(this._fader == null) && !(this._calibrationWindow == null) && !(this._inputBehaviorsWindow == null);
			}

			// Token: 0x0400485F RID: 18527
			[SerializeField]
			private GameObject _button;

			// Token: 0x04004860 RID: 18528
			[SerializeField]
			private GameObject _fitButton;

			// Token: 0x04004861 RID: 18529
			[SerializeField]
			private GameObject _inputGridLabel;

			// Token: 0x04004862 RID: 18530
			[SerializeField]
			private GameObject _inputGridHeaderLabel;

			// Token: 0x04004863 RID: 18531
			[SerializeField]
			private GameObject _inputGridFieldButton;

			// Token: 0x04004864 RID: 18532
			[SerializeField]
			private GameObject _inputGridFieldInvertToggle;

			// Token: 0x04004865 RID: 18533
			[SerializeField]
			private GameObject _window;

			// Token: 0x04004866 RID: 18534
			[SerializeField]
			private GameObject _windowTitleText;

			// Token: 0x04004867 RID: 18535
			[SerializeField]
			private GameObject _windowContentText;

			// Token: 0x04004868 RID: 18536
			[SerializeField]
			private GameObject _fader;

			// Token: 0x04004869 RID: 18537
			[SerializeField]
			private GameObject _calibrationWindow;

			// Token: 0x0400486A RID: 18538
			[SerializeField]
			private GameObject _inputBehaviorsWindow;

			// Token: 0x0400486B RID: 18539
			[SerializeField]
			private GameObject _centerStickGraphic;

			// Token: 0x0400486C RID: 18540
			[SerializeField]
			private GameObject _moveStickGraphic;
		}

		// Token: 0x0200085A RID: 2138
		[Serializable]
		private class References
		{
			// Token: 0x170003D8 RID: 984
			// (get) Token: 0x06002C08 RID: 11272 RVA: 0x00207A8E File Offset: 0x00205C8E
			public Canvas canvas
			{
				get
				{
					return this._canvas;
				}
			}

			// Token: 0x170003D9 RID: 985
			// (get) Token: 0x06002C09 RID: 11273 RVA: 0x00207A96 File Offset: 0x00205C96
			public CanvasGroup mainCanvasGroup
			{
				get
				{
					return this._mainCanvasGroup;
				}
			}

			// Token: 0x170003DA RID: 986
			// (get) Token: 0x06002C0A RID: 11274 RVA: 0x00207A9E File Offset: 0x00205C9E
			public Transform mainContent
			{
				get
				{
					return this._mainContent;
				}
			}

			// Token: 0x170003DB RID: 987
			// (get) Token: 0x06002C0B RID: 11275 RVA: 0x00207AA6 File Offset: 0x00205CA6
			public Transform mainContentInner
			{
				get
				{
					return this._mainContentInner;
				}
			}

			// Token: 0x170003DC RID: 988
			// (get) Token: 0x06002C0C RID: 11276 RVA: 0x00207AAE File Offset: 0x00205CAE
			public UIGroup playersGroup
			{
				get
				{
					return this._playersGroup;
				}
			}

			// Token: 0x170003DD RID: 989
			// (get) Token: 0x06002C0D RID: 11277 RVA: 0x00207AB6 File Offset: 0x00205CB6
			public Transform controllerGroup
			{
				get
				{
					return this._controllerGroup;
				}
			}

			// Token: 0x170003DE RID: 990
			// (get) Token: 0x06002C0E RID: 11278 RVA: 0x00207ABE File Offset: 0x00205CBE
			public Transform controllerGroupLabelGroup
			{
				get
				{
					return this._controllerGroupLabelGroup;
				}
			}

			// Token: 0x170003DF RID: 991
			// (get) Token: 0x06002C0F RID: 11279 RVA: 0x00207AC6 File Offset: 0x00205CC6
			public UIGroup controllerSettingsGroup
			{
				get
				{
					return this._controllerSettingsGroup;
				}
			}

			// Token: 0x170003E0 RID: 992
			// (get) Token: 0x06002C10 RID: 11280 RVA: 0x00207ACE File Offset: 0x00205CCE
			public UIGroup assignedControllersGroup
			{
				get
				{
					return this._assignedControllersGroup;
				}
			}

			// Token: 0x170003E1 RID: 993
			// (get) Token: 0x06002C11 RID: 11281 RVA: 0x00207AD6 File Offset: 0x00205CD6
			public Transform settingsAndMapCategoriesGroup
			{
				get
				{
					return this._settingsAndMapCategoriesGroup;
				}
			}

			// Token: 0x170003E2 RID: 994
			// (get) Token: 0x06002C12 RID: 11282 RVA: 0x00207ADE File Offset: 0x00205CDE
			public UIGroup settingsGroup
			{
				get
				{
					return this._settingsGroup;
				}
			}

			// Token: 0x170003E3 RID: 995
			// (get) Token: 0x06002C13 RID: 11283 RVA: 0x00207AE6 File Offset: 0x00205CE6
			public UIGroup mapCategoriesGroup
			{
				get
				{
					return this._mapCategoriesGroup;
				}
			}

			// Token: 0x170003E4 RID: 996
			// (get) Token: 0x06002C14 RID: 11284 RVA: 0x00207AEE File Offset: 0x00205CEE
			public Transform inputGridGroup
			{
				get
				{
					return this._inputGridGroup;
				}
			}

			// Token: 0x170003E5 RID: 997
			// (get) Token: 0x06002C15 RID: 11285 RVA: 0x00207AF6 File Offset: 0x00205CF6
			public Transform inputGridContainer
			{
				get
				{
					return this._inputGridContainer;
				}
			}

			// Token: 0x170003E6 RID: 998
			// (get) Token: 0x06002C16 RID: 11286 RVA: 0x00207AFE File Offset: 0x00205CFE
			public Transform inputGridHeadersGroup
			{
				get
				{
					return this._inputGridHeadersGroup;
				}
			}

			// Token: 0x170003E7 RID: 999
			// (get) Token: 0x06002C17 RID: 11287 RVA: 0x00207B06 File Offset: 0x00205D06
			public Scrollbar inputGridVScrollbar
			{
				get
				{
					return this._inputGridVScrollbar;
				}
			}

			// Token: 0x170003E8 RID: 1000
			// (get) Token: 0x06002C18 RID: 11288 RVA: 0x00207B0E File Offset: 0x00205D0E
			public ScrollRect inputGridScrollRect
			{
				get
				{
					return this._inputGridScrollRect;
				}
			}

			// Token: 0x170003E9 RID: 1001
			// (get) Token: 0x06002C19 RID: 11289 RVA: 0x00207B16 File Offset: 0x00205D16
			public Transform inputGridInnerGroup
			{
				get
				{
					return this._inputGridInnerGroup;
				}
			}

			// Token: 0x170003EA RID: 1002
			// (get) Token: 0x06002C1A RID: 11290 RVA: 0x00207B1E File Offset: 0x00205D1E
			public TMP_Text controllerNameLabel
			{
				get
				{
					return this._controllerNameLabel;
				}
			}

			// Token: 0x170003EB RID: 1003
			// (get) Token: 0x06002C1B RID: 11291 RVA: 0x00207B26 File Offset: 0x00205D26
			public Button removeControllerButton
			{
				get
				{
					return this._removeControllerButton;
				}
			}

			// Token: 0x170003EC RID: 1004
			// (get) Token: 0x06002C1C RID: 11292 RVA: 0x00207B2E File Offset: 0x00205D2E
			public Button assignControllerButton
			{
				get
				{
					return this._assignControllerButton;
				}
			}

			// Token: 0x170003ED RID: 1005
			// (get) Token: 0x06002C1D RID: 11293 RVA: 0x00207B36 File Offset: 0x00205D36
			public Button calibrateControllerButton
			{
				get
				{
					return this._calibrateControllerButton;
				}
			}

			// Token: 0x170003EE RID: 1006
			// (get) Token: 0x06002C1E RID: 11294 RVA: 0x00207B3E File Offset: 0x00205D3E
			public Button doneButton
			{
				get
				{
					return this._doneButton;
				}
			}

			// Token: 0x170003EF RID: 1007
			// (get) Token: 0x06002C1F RID: 11295 RVA: 0x00207B46 File Offset: 0x00205D46
			public Button restoreDefaultsButton
			{
				get
				{
					return this._restoreDefaultsButton;
				}
			}

			// Token: 0x170003F0 RID: 1008
			// (get) Token: 0x06002C20 RID: 11296 RVA: 0x00207B4E File Offset: 0x00205D4E
			public Selectable defaultSelection
			{
				get
				{
					return this._defaultSelection;
				}
			}

			// Token: 0x170003F1 RID: 1009
			// (get) Token: 0x06002C21 RID: 11297 RVA: 0x00207B56 File Offset: 0x00205D56
			public GameObject[] fixedSelectableUIElements
			{
				get
				{
					return this._fixedSelectableUIElements;
				}
			}

			// Token: 0x170003F2 RID: 1010
			// (get) Token: 0x06002C22 RID: 11298 RVA: 0x00207B5E File Offset: 0x00205D5E
			public Image mainBackgroundImage
			{
				get
				{
					return this._mainBackgroundImage;
				}
			}

			// Token: 0x170003F3 RID: 1011
			// (get) Token: 0x06002C23 RID: 11299 RVA: 0x00207B66 File Offset: 0x00205D66
			// (set) Token: 0x06002C24 RID: 11300 RVA: 0x00207B6E File Offset: 0x00205D6E
			public LayoutElement inputGridLayoutElement { get; set; }

			// Token: 0x170003F4 RID: 1012
			// (get) Token: 0x06002C25 RID: 11301 RVA: 0x00207B77 File Offset: 0x00205D77
			// (set) Token: 0x06002C26 RID: 11302 RVA: 0x00207B7F File Offset: 0x00205D7F
			public Transform inputGridActionColumn { get; set; }

			// Token: 0x170003F5 RID: 1013
			// (get) Token: 0x06002C27 RID: 11303 RVA: 0x00207B88 File Offset: 0x00205D88
			// (set) Token: 0x06002C28 RID: 11304 RVA: 0x00207B90 File Offset: 0x00205D90
			public Transform inputGridKeyboardColumn { get; set; }

			// Token: 0x170003F6 RID: 1014
			// (get) Token: 0x06002C29 RID: 11305 RVA: 0x00207B99 File Offset: 0x00205D99
			// (set) Token: 0x06002C2A RID: 11306 RVA: 0x00207BA1 File Offset: 0x00205DA1
			public Transform inputGridMouseColumn { get; set; }

			// Token: 0x170003F7 RID: 1015
			// (get) Token: 0x06002C2B RID: 11307 RVA: 0x00207BAA File Offset: 0x00205DAA
			// (set) Token: 0x06002C2C RID: 11308 RVA: 0x00207BB2 File Offset: 0x00205DB2
			public Transform inputGridControllerColumn { get; set; }

			// Token: 0x170003F8 RID: 1016
			// (get) Token: 0x06002C2D RID: 11309 RVA: 0x00207BBB File Offset: 0x00205DBB
			// (set) Token: 0x06002C2E RID: 11310 RVA: 0x00207BC3 File Offset: 0x00205DC3
			public Transform inputGridHeader1 { get; set; }

			// Token: 0x170003F9 RID: 1017
			// (get) Token: 0x06002C2F RID: 11311 RVA: 0x00207BCC File Offset: 0x00205DCC
			// (set) Token: 0x06002C30 RID: 11312 RVA: 0x00207BD4 File Offset: 0x00205DD4
			public Transform inputGridHeader2 { get; set; }

			// Token: 0x170003FA RID: 1018
			// (get) Token: 0x06002C31 RID: 11313 RVA: 0x00207BDD File Offset: 0x00205DDD
			// (set) Token: 0x06002C32 RID: 11314 RVA: 0x00207BE5 File Offset: 0x00205DE5
			public Transform inputGridHeader3 { get; set; }

			// Token: 0x170003FB RID: 1019
			// (get) Token: 0x06002C33 RID: 11315 RVA: 0x00207BEE File Offset: 0x00205DEE
			// (set) Token: 0x06002C34 RID: 11316 RVA: 0x00207BF6 File Offset: 0x00205DF6
			public Transform inputGridHeader4 { get; set; }

			// Token: 0x06002C35 RID: 11317 RVA: 0x00207C00 File Offset: 0x00205E00
			public bool Check()
			{
				return !(this._canvas == null) && !(this._mainCanvasGroup == null) && !(this._mainContent == null) && !(this._mainContentInner == null) && !(this._playersGroup == null) && !(this._controllerGroup == null) && !(this._controllerGroupLabelGroup == null) && !(this._controllerSettingsGroup == null) && !(this._assignedControllersGroup == null) && !(this._settingsAndMapCategoriesGroup == null) && !(this._settingsGroup == null) && !(this._mapCategoriesGroup == null) && !(this._inputGridGroup == null) && !(this._inputGridContainer == null) && !(this._inputGridHeadersGroup == null) && !(this._inputGridVScrollbar == null) && !(this._inputGridScrollRect == null) && !(this._inputGridInnerGroup == null) && !(this._controllerNameLabel == null) && !(this._removeControllerButton == null) && !(this._assignControllerButton == null) && !(this._calibrateControllerButton == null) && !(this._doneButton == null) && !(this._restoreDefaultsButton == null) && !(this._defaultSelection == null);
			}

			// Token: 0x0400486D RID: 18541
			[SerializeField]
			private Canvas _canvas;

			// Token: 0x0400486E RID: 18542
			[SerializeField]
			private CanvasGroup _mainCanvasGroup;

			// Token: 0x0400486F RID: 18543
			[SerializeField]
			private Transform _mainContent;

			// Token: 0x04004870 RID: 18544
			[SerializeField]
			private Transform _mainContentInner;

			// Token: 0x04004871 RID: 18545
			[SerializeField]
			private UIGroup _playersGroup;

			// Token: 0x04004872 RID: 18546
			[SerializeField]
			private Transform _controllerGroup;

			// Token: 0x04004873 RID: 18547
			[SerializeField]
			private Transform _controllerGroupLabelGroup;

			// Token: 0x04004874 RID: 18548
			[SerializeField]
			private UIGroup _controllerSettingsGroup;

			// Token: 0x04004875 RID: 18549
			[SerializeField]
			private UIGroup _assignedControllersGroup;

			// Token: 0x04004876 RID: 18550
			[SerializeField]
			private Transform _settingsAndMapCategoriesGroup;

			// Token: 0x04004877 RID: 18551
			[SerializeField]
			private UIGroup _settingsGroup;

			// Token: 0x04004878 RID: 18552
			[SerializeField]
			private UIGroup _mapCategoriesGroup;

			// Token: 0x04004879 RID: 18553
			[SerializeField]
			private Transform _inputGridGroup;

			// Token: 0x0400487A RID: 18554
			[SerializeField]
			private Transform _inputGridContainer;

			// Token: 0x0400487B RID: 18555
			[SerializeField]
			private Transform _inputGridHeadersGroup;

			// Token: 0x0400487C RID: 18556
			[SerializeField]
			private Scrollbar _inputGridVScrollbar;

			// Token: 0x0400487D RID: 18557
			[SerializeField]
			private ScrollRect _inputGridScrollRect;

			// Token: 0x0400487E RID: 18558
			[SerializeField]
			private Transform _inputGridInnerGroup;

			// Token: 0x0400487F RID: 18559
			[SerializeField]
			private TMP_Text _controllerNameLabel;

			// Token: 0x04004880 RID: 18560
			[SerializeField]
			private Button _removeControllerButton;

			// Token: 0x04004881 RID: 18561
			[SerializeField]
			private Button _assignControllerButton;

			// Token: 0x04004882 RID: 18562
			[SerializeField]
			private Button _calibrateControllerButton;

			// Token: 0x04004883 RID: 18563
			[SerializeField]
			private Button _doneButton;

			// Token: 0x04004884 RID: 18564
			[SerializeField]
			private Button _restoreDefaultsButton;

			// Token: 0x04004885 RID: 18565
			[SerializeField]
			private Selectable _defaultSelection;

			// Token: 0x04004886 RID: 18566
			[SerializeField]
			private GameObject[] _fixedSelectableUIElements;

			// Token: 0x04004887 RID: 18567
			[SerializeField]
			private Image _mainBackgroundImage;
		}

		// Token: 0x0200085B RID: 2139
		private class InputActionSet
		{
			// Token: 0x170003FC RID: 1020
			// (get) Token: 0x06002C37 RID: 11319 RVA: 0x00207D9B File Offset: 0x00205F9B
			public int actionId
			{
				get
				{
					return this._actionId;
				}
			}

			// Token: 0x170003FD RID: 1021
			// (get) Token: 0x06002C38 RID: 11320 RVA: 0x00207DA3 File Offset: 0x00205FA3
			public AxisRange axisRange
			{
				get
				{
					return this._axisRange;
				}
			}

			// Token: 0x06002C39 RID: 11321 RVA: 0x00207DAB File Offset: 0x00205FAB
			public InputActionSet(int actionId, AxisRange axisRange)
			{
				this._actionId = actionId;
				this._axisRange = axisRange;
			}

			// Token: 0x04004891 RID: 18577
			private int _actionId;

			// Token: 0x04004892 RID: 18578
			private AxisRange _axisRange;
		}

		// Token: 0x0200085C RID: 2140
		private class InputMapping
		{
			// Token: 0x170003FE RID: 1022
			// (get) Token: 0x06002C3A RID: 11322 RVA: 0x00207DC1 File Offset: 0x00205FC1
			// (set) Token: 0x06002C3B RID: 11323 RVA: 0x00207DC9 File Offset: 0x00205FC9
			public string actionName { get; private set; }

			// Token: 0x170003FF RID: 1023
			// (get) Token: 0x06002C3C RID: 11324 RVA: 0x00207DD2 File Offset: 0x00205FD2
			// (set) Token: 0x06002C3D RID: 11325 RVA: 0x00207DDA File Offset: 0x00205FDA
			public InputFieldInfo fieldInfo { get; private set; }

			// Token: 0x17000400 RID: 1024
			// (get) Token: 0x06002C3E RID: 11326 RVA: 0x00207DE3 File Offset: 0x00205FE3
			// (set) Token: 0x06002C3F RID: 11327 RVA: 0x00207DEB File Offset: 0x00205FEB
			public ControllerMap map { get; private set; }

			// Token: 0x17000401 RID: 1025
			// (get) Token: 0x06002C40 RID: 11328 RVA: 0x00207DF4 File Offset: 0x00205FF4
			// (set) Token: 0x06002C41 RID: 11329 RVA: 0x00207DFC File Offset: 0x00205FFC
			public ActionElementMap aem { get; private set; }

			// Token: 0x17000402 RID: 1026
			// (get) Token: 0x06002C42 RID: 11330 RVA: 0x00207E05 File Offset: 0x00206005
			// (set) Token: 0x06002C43 RID: 11331 RVA: 0x00207E0D File Offset: 0x0020600D
			public ControllerType controllerType { get; private set; }

			// Token: 0x17000403 RID: 1027
			// (get) Token: 0x06002C44 RID: 11332 RVA: 0x00207E16 File Offset: 0x00206016
			// (set) Token: 0x06002C45 RID: 11333 RVA: 0x00207E1E File Offset: 0x0020601E
			public int controllerId { get; private set; }

			// Token: 0x17000404 RID: 1028
			// (get) Token: 0x06002C46 RID: 11334 RVA: 0x00207E27 File Offset: 0x00206027
			// (set) Token: 0x06002C47 RID: 11335 RVA: 0x00207E2F File Offset: 0x0020602F
			public ControllerPollingInfo pollingInfo { get; set; }

			// Token: 0x17000405 RID: 1029
			// (get) Token: 0x06002C48 RID: 11336 RVA: 0x00207E38 File Offset: 0x00206038
			// (set) Token: 0x06002C49 RID: 11337 RVA: 0x00207E40 File Offset: 0x00206040
			public ModifierKeyFlags modifierKeyFlags { get; set; }

			// Token: 0x17000406 RID: 1030
			// (get) Token: 0x06002C4A RID: 11338 RVA: 0x00207E4C File Offset: 0x0020604C
			public AxisRange axisRange
			{
				get
				{
					AxisRange result = 1;
					if (this.pollingInfo.elementType == null)
					{
						if (this.fieldInfo.axisRange == null)
						{
							result = 0;
						}
						else
						{
							result = ((this.pollingInfo.axisPole == null) ? 1 : 2);
						}
					}
					return result;
				}
			}

			// Token: 0x17000407 RID: 1031
			// (get) Token: 0x06002C4B RID: 11339 RVA: 0x00207E94 File Offset: 0x00206094
			public string elementName
			{
				get
				{
					if (this.controllerType == null)
					{
						return ControlMapper.GetLanguage().GetElementIdentifierName(this.pollingInfo.keyboardKey, this.modifierKeyFlags);
					}
					return ControlMapper.GetLanguage().GetElementIdentifierName(this.pollingInfo.controller, this.pollingInfo.elementIdentifierId, (this.pollingInfo.axisPole == null) ? 1 : 2);
				}
			}

			// Token: 0x06002C4C RID: 11340 RVA: 0x00207F02 File Offset: 0x00206102
			public InputMapping(string actionName, InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, ControllerType controllerType, int controllerId)
			{
				this.actionName = actionName;
				this.fieldInfo = fieldInfo;
				this.map = map;
				this.aem = aem;
				this.controllerType = controllerType;
				this.controllerId = controllerId;
			}

			// Token: 0x06002C4D RID: 11341 RVA: 0x00207F37 File Offset: 0x00206137
			public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo)
			{
				this.pollingInfo = pollingInfo;
				return this.ToElementAssignment();
			}

			// Token: 0x06002C4E RID: 11342 RVA: 0x00207F46 File Offset: 0x00206146
			public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo, ModifierKeyFlags modifierKeyFlags)
			{
				this.pollingInfo = pollingInfo;
				this.modifierKeyFlags = modifierKeyFlags;
				return this.ToElementAssignment();
			}

			// Token: 0x06002C4F RID: 11343 RVA: 0x00207F5C File Offset: 0x0020615C
			public ElementAssignment ToElementAssignment()
			{
				return new ElementAssignment(this.controllerType, this.pollingInfo.elementType, this.pollingInfo.elementIdentifierId, this.axisRange, this.pollingInfo.keyboardKey, this.modifierKeyFlags, this.fieldInfo.actionId, (this.fieldInfo.axisRange == 2) ? 1 : 0, false, (this.aem != null) ? this.aem.id : -1);
			}
		}

		// Token: 0x0200085D RID: 2141
		private class AxisCalibrator
		{
			// Token: 0x17000408 RID: 1032
			// (get) Token: 0x06002C50 RID: 11344 RVA: 0x00207FDE File Offset: 0x002061DE
			public bool isValid
			{
				get
				{
					return this.axis != null;
				}
			}

			// Token: 0x06002C51 RID: 11345 RVA: 0x00207FEC File Offset: 0x002061EC
			public AxisCalibrator(Joystick joystick, int axisIndex)
			{
				this.data = default(AxisCalibrationData);
				this.joystick = joystick;
				this.axisIndex = axisIndex;
				if (joystick != null && axisIndex >= 0 && joystick.axisCount > axisIndex)
				{
					this.axis = joystick.Axes[axisIndex];
					this.data = joystick.calibrationMap.GetAxis(axisIndex).GetData();
				}
				this.firstRun = true;
			}

			// Token: 0x06002C52 RID: 11346 RVA: 0x0020805C File Offset: 0x0020625C
			public void RecordMinMax()
			{
				if (this.axis == null)
				{
					return;
				}
				float valueRaw = this.axis.valueRaw;
				if (this.firstRun || valueRaw < this.data.min)
				{
					this.data.min = valueRaw;
				}
				if (this.firstRun || valueRaw > this.data.max)
				{
					this.data.max = valueRaw;
				}
				this.firstRun = false;
			}

			// Token: 0x06002C53 RID: 11347 RVA: 0x002080C9 File Offset: 0x002062C9
			public void RecordZero()
			{
				if (this.axis == null)
				{
					return;
				}
				this.data.zero = this.axis.valueRaw;
			}

			// Token: 0x06002C54 RID: 11348 RVA: 0x002080EC File Offset: 0x002062EC
			public void Commit()
			{
				if (this.axis == null)
				{
					return;
				}
				AxisCalibration axisCalibration = this.joystick.calibrationMap.GetAxis(this.axisIndex);
				if (axisCalibration == null)
				{
					return;
				}
				if ((double)Mathf.Abs(this.data.max - this.data.min) < 0.1)
				{
					return;
				}
				axisCalibration.SetData(this.data);
			}

			// Token: 0x0400489B RID: 18587
			public AxisCalibrationData data;

			// Token: 0x0400489C RID: 18588
			public readonly Joystick joystick;

			// Token: 0x0400489D RID: 18589
			public readonly int axisIndex;

			// Token: 0x0400489E RID: 18590
			private Controller.Axis axis;

			// Token: 0x0400489F RID: 18591
			private bool firstRun;
		}

		// Token: 0x0200085E RID: 2142
		private class IndexedDictionary<TKey, TValue>
		{
			// Token: 0x17000409 RID: 1033
			// (get) Token: 0x06002C55 RID: 11349 RVA: 0x00208152 File Offset: 0x00206352
			public int Count
			{
				get
				{
					return this.list.Count;
				}
			}

			// Token: 0x06002C56 RID: 11350 RVA: 0x0020815F File Offset: 0x0020635F
			public IndexedDictionary()
			{
				this.list = new List<ControlMapper.IndexedDictionary<TKey, TValue>.Entry>();
			}

			// Token: 0x1700040A RID: 1034
			public TValue this[int index]
			{
				get
				{
					return this.list[index].value;
				}
			}

			// Token: 0x06002C58 RID: 11352 RVA: 0x00208188 File Offset: 0x00206388
			public TValue Get(TKey key)
			{
				int num = this.IndexOfKey(key);
				if (num < 0)
				{
					throw new Exception("Key does not exist!");
				}
				return this.list[num].value;
			}

			// Token: 0x06002C59 RID: 11353 RVA: 0x002081C0 File Offset: 0x002063C0
			public bool TryGet(TKey key, out TValue value)
			{
				value = default(TValue);
				int num = this.IndexOfKey(key);
				if (num < 0)
				{
					return false;
				}
				value = this.list[num].value;
				return true;
			}

			// Token: 0x06002C5A RID: 11354 RVA: 0x002081FA File Offset: 0x002063FA
			public void Add(TKey key, TValue value)
			{
				if (this.ContainsKey(key))
				{
					throw new Exception("Key " + key.ToString() + " is already in use!");
				}
				this.list.Add(new ControlMapper.IndexedDictionary<TKey, TValue>.Entry(key, value));
			}

			// Token: 0x06002C5B RID: 11355 RVA: 0x0020823C File Offset: 0x0020643C
			public int IndexOfKey(TKey key)
			{
				int count = this.list.Count;
				for (int i = 0; i < count; i++)
				{
					if (EqualityComparer<TKey>.Default.Equals(this.list[i].key, key))
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x06002C5C RID: 11356 RVA: 0x00208284 File Offset: 0x00206484
			public bool ContainsKey(TKey key)
			{
				int count = this.list.Count;
				for (int i = 0; i < count; i++)
				{
					if (EqualityComparer<TKey>.Default.Equals(this.list[i].key, key))
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06002C5D RID: 11357 RVA: 0x002082CA File Offset: 0x002064CA
			public void Clear()
			{
				this.list.Clear();
			}

			// Token: 0x040048A0 RID: 18592
			private List<ControlMapper.IndexedDictionary<TKey, TValue>.Entry> list;

			// Token: 0x0200085F RID: 2143
			private class Entry
			{
				// Token: 0x06002C5E RID: 11358 RVA: 0x002082D7 File Offset: 0x002064D7
				public Entry(TKey key, TValue value)
				{
					this.key = key;
					this.value = value;
				}

				// Token: 0x040048A1 RID: 18593
				public TKey key;

				// Token: 0x040048A2 RID: 18594
				public TValue value;
			}
		}

		// Token: 0x02000860 RID: 2144
		private enum LayoutElementSizeType
		{
			// Token: 0x040048A4 RID: 18596
			MinSize,
			// Token: 0x040048A5 RID: 18597
			PreferredSize
		}

		// Token: 0x02000861 RID: 2145
		private enum WindowType
		{
			// Token: 0x040048A7 RID: 18599
			None,
			// Token: 0x040048A8 RID: 18600
			ChooseJoystick,
			// Token: 0x040048A9 RID: 18601
			JoystickAssignmentConflict,
			// Token: 0x040048AA RID: 18602
			ElementAssignment,
			// Token: 0x040048AB RID: 18603
			ElementAssignmentPrePolling,
			// Token: 0x040048AC RID: 18604
			ElementAssignmentPolling,
			// Token: 0x040048AD RID: 18605
			ElementAssignmentResult,
			// Token: 0x040048AE RID: 18606
			ElementAssignmentConflict,
			// Token: 0x040048AF RID: 18607
			Calibration,
			// Token: 0x040048B0 RID: 18608
			CalibrateStep1,
			// Token: 0x040048B1 RID: 18609
			CalibrateStep2
		}

		// Token: 0x02000862 RID: 2146
		private class InputGrid
		{
			// Token: 0x06002C5F RID: 11359 RVA: 0x002082ED File Offset: 0x002064ED
			public InputGrid()
			{
				this.list = new ControlMapper.InputGridEntryList();
				this.groups = new List<GameObject>();
			}

			// Token: 0x06002C60 RID: 11360 RVA: 0x0020830B File Offset: 0x0020650B
			public void AddMapCategory(int mapCategoryId)
			{
				this.list.AddMapCategory(mapCategoryId);
			}

			// Token: 0x06002C61 RID: 11361 RVA: 0x00208319 File Offset: 0x00206519
			public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				this.list.AddAction(mapCategoryId, action, axisRange);
			}

			// Token: 0x06002C62 RID: 11362 RVA: 0x00208329 File Offset: 0x00206529
			public void AddActionCategory(int mapCategoryId, int actionCategoryId)
			{
				this.list.AddActionCategory(mapCategoryId, actionCategoryId);
			}

			// Token: 0x06002C63 RID: 11363 RVA: 0x00208338 File Offset: 0x00206538
			public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
			{
				this.list.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, fieldSetContainer);
			}

			// Token: 0x06002C64 RID: 11364 RVA: 0x0020834C File Offset: 0x0020654C
			public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, ControlMapper.GUIInputField inputField)
			{
				this.list.AddInputField(mapCategoryId, action, axisRange, controllerType, fieldIndex, inputField);
			}

			// Token: 0x06002C65 RID: 11365 RVA: 0x00208362 File Offset: 0x00206562
			public void AddGroup(GameObject group)
			{
				this.groups.Add(group);
			}

			// Token: 0x06002C66 RID: 11366 RVA: 0x00208370 File Offset: 0x00206570
			public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControlMapper.GUILabel label)
			{
				this.list.AddActionLabel(mapCategoryId, actionId, axisRange, label);
			}

			// Token: 0x06002C67 RID: 11367 RVA: 0x00208382 File Offset: 0x00206582
			public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, ControlMapper.GUILabel label)
			{
				this.list.AddActionCategoryLabel(mapCategoryId, actionCategoryId, label);
			}

			// Token: 0x06002C68 RID: 11368 RVA: 0x00208392 File Offset: 0x00206592
			public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				return this.list.Contains(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
			}

			// Token: 0x06002C69 RID: 11369 RVA: 0x002083A6 File Offset: 0x002065A6
			public ControlMapper.GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				return this.list.GetGUIInputField(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
			}

			// Token: 0x06002C6A RID: 11370 RVA: 0x002083BA File Offset: 0x002065BA
			public IEnumerable<ControlMapper.InputActionSet> GetActionSets(int mapCategoryId)
			{
				return this.list.GetActionSets(mapCategoryId);
			}

			// Token: 0x06002C6B RID: 11371 RVA: 0x002083C8 File Offset: 0x002065C8
			public void SetColumnHeight(int mapCategoryId, float height)
			{
				this.list.SetColumnHeight(mapCategoryId, height);
			}

			// Token: 0x06002C6C RID: 11372 RVA: 0x002083D7 File Offset: 0x002065D7
			public float GetColumnHeight(int mapCategoryId)
			{
				return this.list.GetColumnHeight(mapCategoryId);
			}

			// Token: 0x06002C6D RID: 11373 RVA: 0x002083E5 File Offset: 0x002065E5
			public void SetFieldsActive(int mapCategoryId, bool state)
			{
				this.list.SetFieldsActive(mapCategoryId, state);
			}

			// Token: 0x06002C6E RID: 11374 RVA: 0x002083F4 File Offset: 0x002065F4
			public void SetFieldLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
			{
				this.list.SetLabel(mapCategoryId, actionId, axisRange, controllerType, index, label);
			}

			// Token: 0x06002C6F RID: 11375 RVA: 0x0020840C File Offset: 0x0020660C
			public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
			{
				this.list.PopulateField(mapCategoryId, actionId, axisRange, controllerType, controllerId, index, actionElementMapId, label, invert);
			}

			// Token: 0x06002C70 RID: 11376 RVA: 0x00208433 File Offset: 0x00206633
			public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
			{
				this.list.SetFixedFieldData(mapCategoryId, actionId, axisRange, controllerType, controllerId);
			}

			// Token: 0x06002C71 RID: 11377 RVA: 0x00208447 File Offset: 0x00206647
			public void InitializeFields(int mapCategoryId)
			{
				this.list.InitializeFields(mapCategoryId);
			}

			// Token: 0x06002C72 RID: 11378 RVA: 0x00208455 File Offset: 0x00206655
			public void Show(int mapCategoryId)
			{
				this.list.Show(mapCategoryId);
			}

			// Token: 0x06002C73 RID: 11379 RVA: 0x00208463 File Offset: 0x00206663
			public void HideAll()
			{
				this.list.HideAll();
			}

			// Token: 0x06002C74 RID: 11380 RVA: 0x00208470 File Offset: 0x00206670
			public void ClearLabels(int mapCategoryId)
			{
				this.list.ClearLabels(mapCategoryId);
			}

			// Token: 0x06002C75 RID: 11381 RVA: 0x00208480 File Offset: 0x00206680
			private void ClearGroups()
			{
				for (int i = 0; i < this.groups.Count; i++)
				{
					if (!(this.groups[i] == null))
					{
						Object.Destroy(this.groups[i]);
					}
				}
			}

			// Token: 0x06002C76 RID: 11382 RVA: 0x002084C8 File Offset: 0x002066C8
			public void ClearAll()
			{
				this.ClearGroups();
				this.list.Clear();
			}

			// Token: 0x040048B2 RID: 18610
			private ControlMapper.InputGridEntryList list;

			// Token: 0x040048B3 RID: 18611
			private List<GameObject> groups;
		}

		// Token: 0x02000863 RID: 2147
		private class InputGridEntryList
		{
			// Token: 0x06002C77 RID: 11383 RVA: 0x002084DB File Offset: 0x002066DB
			public InputGridEntryList()
			{
				this.entries = new ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.MapCategoryEntry>();
			}

			// Token: 0x06002C78 RID: 11384 RVA: 0x002084EE File Offset: 0x002066EE
			public void AddMapCategory(int mapCategoryId)
			{
				if (mapCategoryId < 0)
				{
					return;
				}
				if (this.entries.ContainsKey(mapCategoryId))
				{
					return;
				}
				this.entries.Add(mapCategoryId, new ControlMapper.InputGridEntryList.MapCategoryEntry());
			}

			// Token: 0x06002C79 RID: 11385 RVA: 0x00208515 File Offset: 0x00206715
			public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				this.AddActionEntry(mapCategoryId, action, axisRange);
			}

			// Token: 0x06002C7A RID: 11386 RVA: 0x00208524 File Offset: 0x00206724
			private ControlMapper.InputGridEntryList.ActionEntry AddActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				if (action == null)
				{
					return null;
				}
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return null;
				}
				return mapCategoryEntry.AddAction(action, axisRange);
			}

			// Token: 0x06002C7B RID: 11387 RVA: 0x00208550 File Offset: 0x00206750
			public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControlMapper.GUILabel label)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = mapCategoryEntry.GetActionEntry(actionId, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.SetLabel(label);
			}

			// Token: 0x06002C7C RID: 11388 RVA: 0x00208583 File Offset: 0x00206783
			public void AddActionCategory(int mapCategoryId, int actionCategoryId)
			{
				this.AddActionCategoryEntry(mapCategoryId, actionCategoryId);
			}

			// Token: 0x06002C7D RID: 11389 RVA: 0x00208590 File Offset: 0x00206790
			private ControlMapper.InputGridEntryList.ActionCategoryEntry AddActionCategoryEntry(int mapCategoryId, int actionCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return null;
				}
				return mapCategoryEntry.AddActionCategory(actionCategoryId);
			}

			// Token: 0x06002C7E RID: 11390 RVA: 0x002085B8 File Offset: 0x002067B8
			public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, ControlMapper.GUILabel label)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				ControlMapper.InputGridEntryList.ActionCategoryEntry actionCategoryEntry = mapCategoryEntry.GetActionCategoryEntry(actionCategoryId);
				if (actionCategoryEntry == null)
				{
					return;
				}
				actionCategoryEntry.SetLabel(label);
			}

			// Token: 0x06002C7F RID: 11391 RVA: 0x002085EC File Offset: 0x002067EC
			public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, action, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.AddInputFieldSet(controllerType, fieldSetContainer);
			}

			// Token: 0x06002C80 RID: 11392 RVA: 0x00208614 File Offset: 0x00206814
			public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, ControlMapper.GUIInputField inputField)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, action, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.AddInputField(controllerType, fieldIndex, inputField);
			}

			// Token: 0x06002C81 RID: 11393 RVA: 0x0020863B File Offset: 0x0020683B
			public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange)
			{
				return this.GetActionEntry(mapCategoryId, actionId, axisRange) != null;
			}

			// Token: 0x06002C82 RID: 11394 RVA: 0x0020864C File Offset: 0x0020684C
			public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				return actionEntry != null && actionEntry.Contains(controllerType, fieldIndex);
			}

			// Token: 0x06002C83 RID: 11395 RVA: 0x00208674 File Offset: 0x00206874
			public void SetColumnHeight(int mapCategoryId, float height)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				mapCategoryEntry.columnHeight = height;
			}

			// Token: 0x06002C84 RID: 11396 RVA: 0x0020869C File Offset: 0x0020689C
			public float GetColumnHeight(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return 0f;
				}
				return mapCategoryEntry.columnHeight;
			}

			// Token: 0x06002C85 RID: 11397 RVA: 0x002086C8 File Offset: 0x002068C8
			public ControlMapper.GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return null;
				}
				return actionEntry.GetGUIInputField(controllerType, fieldIndex);
			}

			// Token: 0x06002C86 RID: 11398 RVA: 0x002086F0 File Offset: 0x002068F0
			private ControlMapper.InputGridEntryList.ActionEntry GetActionEntry(int mapCategoryId, int actionId, AxisRange axisRange)
			{
				if (actionId < 0)
				{
					return null;
				}
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return null;
				}
				return mapCategoryEntry.GetActionEntry(actionId, axisRange);
			}

			// Token: 0x06002C87 RID: 11399 RVA: 0x0020871D File Offset: 0x0020691D
			private ControlMapper.InputGridEntryList.ActionEntry GetActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				if (action == null)
				{
					return null;
				}
				return this.GetActionEntry(mapCategoryId, action.id, axisRange);
			}

			// Token: 0x06002C88 RID: 11400 RVA: 0x00208732 File Offset: 0x00206932
			public IEnumerable<ControlMapper.InputActionSet> GetActionSets(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					yield break;
				}
				List<ControlMapper.InputGridEntryList.ActionEntry> list = mapCategoryEntry.actionList;
				int count = (list != null) ? list.Count : 0;
				int num;
				for (int i = 0; i < count; i = num + 1)
				{
					yield return list[i].actionSet;
					num = i;
				}
				yield break;
			}

			// Token: 0x06002C89 RID: 11401 RVA: 0x0020874C File Offset: 0x0020694C
			public void SetFieldsActive(int mapCategoryId, bool state)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				List<ControlMapper.InputGridEntryList.ActionEntry> actionList = mapCategoryEntry.actionList;
				int num = (actionList != null) ? actionList.Count : 0;
				for (int i = 0; i < num; i++)
				{
					actionList[i].SetFieldsActive(state);
				}
			}

			// Token: 0x06002C8A RID: 11402 RVA: 0x00208798 File Offset: 0x00206998
			public void SetLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.SetFieldLabel(controllerType, index, label);
			}

			// Token: 0x06002C8B RID: 11403 RVA: 0x002087C0 File Offset: 0x002069C0
			public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.PopulateField(controllerType, controllerId, index, actionElementMapId, label, invert);
			}

			// Token: 0x06002C8C RID: 11404 RVA: 0x002087F0 File Offset: 0x002069F0
			public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.SetFixedFieldData(controllerType, controllerId);
			}

			// Token: 0x06002C8D RID: 11405 RVA: 0x00208818 File Offset: 0x00206A18
			public void InitializeFields(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				List<ControlMapper.InputGridEntryList.ActionEntry> actionList = mapCategoryEntry.actionList;
				int num = (actionList != null) ? actionList.Count : 0;
				for (int i = 0; i < num; i++)
				{
					actionList[i].Initialize();
				}
			}

			// Token: 0x06002C8E RID: 11406 RVA: 0x00208864 File Offset: 0x00206A64
			public void Show(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				mapCategoryEntry.SetAllActive(true);
			}

			// Token: 0x06002C8F RID: 11407 RVA: 0x0020888C File Offset: 0x00206A8C
			public void HideAll()
			{
				for (int i = 0; i < this.entries.Count; i++)
				{
					this.entries[i].SetAllActive(false);
				}
			}

			// Token: 0x06002C90 RID: 11408 RVA: 0x002088C4 File Offset: 0x00206AC4
			public void ClearLabels(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				List<ControlMapper.InputGridEntryList.ActionEntry> actionList = mapCategoryEntry.actionList;
				int num = (actionList != null) ? actionList.Count : 0;
				for (int i = 0; i < num; i++)
				{
					actionList[i].ClearLabels();
				}
			}

			// Token: 0x06002C91 RID: 11409 RVA: 0x0020890E File Offset: 0x00206B0E
			public void Clear()
			{
				this.entries.Clear();
			}

			// Token: 0x040048B4 RID: 18612
			private ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.MapCategoryEntry> entries;

			// Token: 0x02000864 RID: 2148
			private class MapCategoryEntry
			{
				// Token: 0x1700040B RID: 1035
				// (get) Token: 0x06002C92 RID: 11410 RVA: 0x0020891B File Offset: 0x00206B1B
				public List<ControlMapper.InputGridEntryList.ActionEntry> actionList
				{
					get
					{
						return this._actionList;
					}
				}

				// Token: 0x1700040C RID: 1036
				// (get) Token: 0x06002C93 RID: 11411 RVA: 0x00208923 File Offset: 0x00206B23
				public ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.ActionCategoryEntry> actionCategoryList
				{
					get
					{
						return this._actionCategoryList;
					}
				}

				// Token: 0x1700040D RID: 1037
				// (get) Token: 0x06002C94 RID: 11412 RVA: 0x0020892B File Offset: 0x00206B2B
				// (set) Token: 0x06002C95 RID: 11413 RVA: 0x00208933 File Offset: 0x00206B33
				public float columnHeight
				{
					get
					{
						return this._columnHeight;
					}
					set
					{
						this._columnHeight = value;
					}
				}

				// Token: 0x06002C96 RID: 11414 RVA: 0x0020893C File Offset: 0x00206B3C
				public MapCategoryEntry()
				{
					this._actionList = new List<ControlMapper.InputGridEntryList.ActionEntry>();
					this._actionCategoryList = new ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.ActionCategoryEntry>();
				}

				// Token: 0x06002C97 RID: 11415 RVA: 0x0020895C File Offset: 0x00206B5C
				public ControlMapper.InputGridEntryList.ActionEntry GetActionEntry(int actionId, AxisRange axisRange)
				{
					int num = this.IndexOfActionEntry(actionId, axisRange);
					if (num < 0)
					{
						return null;
					}
					return this._actionList[num];
				}

				// Token: 0x06002C98 RID: 11416 RVA: 0x00208984 File Offset: 0x00206B84
				public int IndexOfActionEntry(int actionId, AxisRange axisRange)
				{
					int count = this._actionList.Count;
					for (int i = 0; i < count; i++)
					{
						if (this._actionList[i].Matches(actionId, axisRange))
						{
							return i;
						}
					}
					return -1;
				}

				// Token: 0x06002C99 RID: 11417 RVA: 0x002089C1 File Offset: 0x00206BC1
				public bool ContainsActionEntry(int actionId, AxisRange axisRange)
				{
					return this.IndexOfActionEntry(actionId, axisRange) >= 0;
				}

				// Token: 0x06002C9A RID: 11418 RVA: 0x002089D4 File Offset: 0x00206BD4
				public ControlMapper.InputGridEntryList.ActionEntry AddAction(InputAction action, AxisRange axisRange)
				{
					if (action == null)
					{
						return null;
					}
					if (this.ContainsActionEntry(action.id, axisRange))
					{
						return null;
					}
					this._actionList.Add(new ControlMapper.InputGridEntryList.ActionEntry(action, axisRange));
					return this._actionList[this._actionList.Count - 1];
				}

				// Token: 0x06002C9B RID: 11419 RVA: 0x00208A21 File Offset: 0x00206C21
				public ControlMapper.InputGridEntryList.ActionCategoryEntry GetActionCategoryEntry(int actionCategoryId)
				{
					if (!this._actionCategoryList.ContainsKey(actionCategoryId))
					{
						return null;
					}
					return this._actionCategoryList.Get(actionCategoryId);
				}

				// Token: 0x06002C9C RID: 11420 RVA: 0x00208A3F File Offset: 0x00206C3F
				public ControlMapper.InputGridEntryList.ActionCategoryEntry AddActionCategory(int actionCategoryId)
				{
					if (actionCategoryId < 0)
					{
						return null;
					}
					if (this._actionCategoryList.ContainsKey(actionCategoryId))
					{
						return null;
					}
					this._actionCategoryList.Add(actionCategoryId, new ControlMapper.InputGridEntryList.ActionCategoryEntry(actionCategoryId));
					return this._actionCategoryList.Get(actionCategoryId);
				}

				// Token: 0x06002C9D RID: 11421 RVA: 0x00208A78 File Offset: 0x00206C78
				public void SetAllActive(bool state)
				{
					for (int i = 0; i < this._actionCategoryList.Count; i++)
					{
						this._actionCategoryList[i].SetActive(state);
					}
					for (int j = 0; j < this._actionList.Count; j++)
					{
						this._actionList[j].SetActive(state);
					}
				}

				// Token: 0x040048B5 RID: 18613
				private List<ControlMapper.InputGridEntryList.ActionEntry> _actionList;

				// Token: 0x040048B6 RID: 18614
				private ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.ActionCategoryEntry> _actionCategoryList;

				// Token: 0x040048B7 RID: 18615
				private float _columnHeight;
			}

			// Token: 0x02000865 RID: 2149
			private class ActionEntry
			{
				// Token: 0x06002C9E RID: 11422 RVA: 0x00208AD5 File Offset: 0x00206CD5
				public ActionEntry(InputAction action, AxisRange axisRange)
				{
					this.action = action;
					this.axisRange = axisRange;
					this.actionSet = new ControlMapper.InputActionSet(action.id, axisRange);
					this.fieldSets = new ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.FieldSet>();
				}

				// Token: 0x06002C9F RID: 11423 RVA: 0x00208B08 File Offset: 0x00206D08
				public void SetLabel(ControlMapper.GUILabel label)
				{
					this.label = label;
				}

				// Token: 0x06002CA0 RID: 11424 RVA: 0x00208B11 File Offset: 0x00206D11
				public bool Matches(int actionId, AxisRange axisRange)
				{
					return this.action.id == actionId && this.axisRange == axisRange;
				}

				// Token: 0x06002CA1 RID: 11425 RVA: 0x00208B2F File Offset: 0x00206D2F
				public void AddInputFieldSet(ControllerType controllerType, GameObject fieldSetContainer)
				{
					if (this.fieldSets.ContainsKey(controllerType))
					{
						return;
					}
					this.fieldSets.Add(controllerType, new ControlMapper.InputGridEntryList.FieldSet(fieldSetContainer));
				}

				// Token: 0x06002CA2 RID: 11426 RVA: 0x00208B54 File Offset: 0x00206D54
				public void AddInputField(ControllerType controllerType, int fieldIndex, ControlMapper.GUIInputField inputField)
				{
					if (!this.fieldSets.ContainsKey(controllerType))
					{
						return;
					}
					ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets.Get(controllerType);
					if (fieldSet.fields.ContainsKey(fieldIndex))
					{
						return;
					}
					fieldSet.fields.Add(fieldIndex, inputField);
				}

				// Token: 0x06002CA3 RID: 11427 RVA: 0x00208B9C File Offset: 0x00206D9C
				public ControlMapper.GUIInputField GetGUIInputField(ControllerType controllerType, int fieldIndex)
				{
					if (!this.fieldSets.ContainsKey(controllerType))
					{
						return null;
					}
					if (!this.fieldSets.Get(controllerType).fields.ContainsKey(fieldIndex))
					{
						return null;
					}
					return this.fieldSets.Get(controllerType).fields.Get(fieldIndex);
				}

				// Token: 0x06002CA4 RID: 11428 RVA: 0x00208BEB File Offset: 0x00206DEB
				public bool Contains(ControllerType controllerType, int fieldId)
				{
					return this.fieldSets.ContainsKey(controllerType) && this.fieldSets.Get(controllerType).fields.ContainsKey(fieldId);
				}

				// Token: 0x06002CA5 RID: 11429 RVA: 0x00208C1C File Offset: 0x00206E1C
				public void SetFieldLabel(ControllerType controllerType, int index, string label)
				{
					if (!this.fieldSets.ContainsKey(controllerType))
					{
						return;
					}
					if (!this.fieldSets.Get(controllerType).fields.ContainsKey(index))
					{
						return;
					}
					this.fieldSets.Get(controllerType).fields.Get(index).SetLabel(label);
				}

				// Token: 0x06002CA6 RID: 11430 RVA: 0x00208C70 File Offset: 0x00206E70
				public void PopulateField(ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
				{
					if (!this.fieldSets.ContainsKey(controllerType))
					{
						return;
					}
					if (!this.fieldSets.Get(controllerType).fields.ContainsKey(index))
					{
						return;
					}
					ControlMapper.GUIInputField guiinputField = this.fieldSets.Get(controllerType).fields.Get(index);
					guiinputField.SetLabel(label);
					guiinputField.actionElementMapId = actionElementMapId;
					guiinputField.controllerId = controllerId;
					if (guiinputField.hasToggle)
					{
						guiinputField.toggle.SetInteractible(true, false);
						guiinputField.toggle.SetToggleState(invert);
						guiinputField.toggle.actionElementMapId = actionElementMapId;
					}
				}

				// Token: 0x06002CA7 RID: 11431 RVA: 0x00208D04 File Offset: 0x00206F04
				public void SetFixedFieldData(ControllerType controllerType, int controllerId)
				{
					if (!this.fieldSets.ContainsKey(controllerType))
					{
						return;
					}
					ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets.Get(controllerType);
					int count = fieldSet.fields.Count;
					for (int i = 0; i < count; i++)
					{
						fieldSet.fields[i].controllerId = controllerId;
					}
				}

				// Token: 0x06002CA8 RID: 11432 RVA: 0x00208D58 File Offset: 0x00206F58
				public void Initialize()
				{
					for (int i = 0; i < this.fieldSets.Count; i++)
					{
						ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets[i];
						int count = fieldSet.fields.Count;
						for (int j = 0; j < count; j++)
						{
							ControlMapper.GUIInputField guiinputField = fieldSet.fields[j];
							if (guiinputField.hasToggle)
							{
								guiinputField.toggle.SetInteractible(false, false);
								guiinputField.toggle.SetToggleState(false);
								guiinputField.toggle.actionElementMapId = -1;
							}
							guiinputField.SetLabel("");
							guiinputField.actionElementMapId = -1;
							guiinputField.controllerId = -1;
						}
					}
				}

				// Token: 0x06002CA9 RID: 11433 RVA: 0x00208E04 File Offset: 0x00207004
				public void SetActive(bool state)
				{
					if (this.label != null)
					{
						this.label.SetActive(state);
					}
					int count = this.fieldSets.Count;
					for (int i = 0; i < count; i++)
					{
						this.fieldSets[i].groupContainer.SetActive(state);
					}
				}

				// Token: 0x06002CAA RID: 11434 RVA: 0x00208E54 File Offset: 0x00207054
				public void ClearLabels()
				{
					for (int i = 0; i < this.fieldSets.Count; i++)
					{
						ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets[i];
						int count = fieldSet.fields.Count;
						for (int j = 0; j < count; j++)
						{
							fieldSet.fields[j].SetLabel("");
						}
					}
				}

				// Token: 0x06002CAB RID: 11435 RVA: 0x00208EB4 File Offset: 0x002070B4
				public void SetFieldsActive(bool state)
				{
					for (int i = 0; i < this.fieldSets.Count; i++)
					{
						ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets[i];
						int count = fieldSet.fields.Count;
						for (int j = 0; j < count; j++)
						{
							ControlMapper.GUIInputField guiinputField = fieldSet.fields[j];
							guiinputField.SetInteractible(state, false);
							if (guiinputField.hasToggle && (!state || guiinputField.toggle.actionElementMapId >= 0))
							{
								guiinputField.toggle.SetInteractible(state, false);
							}
						}
					}
				}

				// Token: 0x040048B8 RID: 18616
				private ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.FieldSet> fieldSets;

				// Token: 0x040048B9 RID: 18617
				public ControlMapper.GUILabel label;

				// Token: 0x040048BA RID: 18618
				public readonly InputAction action;

				// Token: 0x040048BB RID: 18619
				public readonly AxisRange axisRange;

				// Token: 0x040048BC RID: 18620
				public readonly ControlMapper.InputActionSet actionSet;
			}

			// Token: 0x02000866 RID: 2150
			private class FieldSet
			{
				// Token: 0x06002CAC RID: 11436 RVA: 0x00208F3C File Offset: 0x0020713C
				public FieldSet(GameObject groupContainer)
				{
					this.groupContainer = groupContainer;
					this.fields = new ControlMapper.IndexedDictionary<int, ControlMapper.GUIInputField>();
				}

				// Token: 0x040048BD RID: 18621
				public readonly GameObject groupContainer;

				// Token: 0x040048BE RID: 18622
				public readonly ControlMapper.IndexedDictionary<int, ControlMapper.GUIInputField> fields;
			}

			// Token: 0x02000867 RID: 2151
			private class ActionCategoryEntry
			{
				// Token: 0x06002CAD RID: 11437 RVA: 0x00208F56 File Offset: 0x00207156
				public ActionCategoryEntry(int actionCategoryId)
				{
					this.actionCategoryId = actionCategoryId;
				}

				// Token: 0x06002CAE RID: 11438 RVA: 0x00208F65 File Offset: 0x00207165
				public void SetLabel(ControlMapper.GUILabel label)
				{
					this.label = label;
				}

				// Token: 0x06002CAF RID: 11439 RVA: 0x00208F6E File Offset: 0x0020716E
				public void SetActive(bool state)
				{
					if (this.label != null)
					{
						this.label.SetActive(state);
					}
				}

				// Token: 0x040048BF RID: 18623
				public readonly int actionCategoryId;

				// Token: 0x040048C0 RID: 18624
				public ControlMapper.GUILabel label;
			}
		}

		// Token: 0x02000869 RID: 2153
		private class WindowManager
		{
			// Token: 0x17000410 RID: 1040
			// (get) Token: 0x06002CB8 RID: 11448 RVA: 0x002090C0 File Offset: 0x002072C0
			public bool isWindowOpen
			{
				get
				{
					for (int i = this.windows.Count - 1; i >= 0; i--)
					{
						if (!(this.windows[i] == null))
						{
							return true;
						}
					}
					return false;
				}
			}

			// Token: 0x17000411 RID: 1041
			// (get) Token: 0x06002CB9 RID: 11449 RVA: 0x002090FC File Offset: 0x002072FC
			public Window topWindow
			{
				get
				{
					for (int i = this.windows.Count - 1; i >= 0; i--)
					{
						if (!(this.windows[i] == null))
						{
							return this.windows[i];
						}
					}
					return null;
				}
			}

			// Token: 0x06002CBA RID: 11450 RVA: 0x00209144 File Offset: 0x00207344
			public WindowManager(GameObject windowPrefab, GameObject faderPrefab, Transform parent)
			{
				this.windowPrefab = windowPrefab;
				this.parent = parent;
				this.windows = new List<Window>();
				this.fader = Object.Instantiate<GameObject>(faderPrefab);
				this.fader.transform.SetParent(parent, false);
				this.fader.GetComponent<RectTransform>().localScale = Vector2.one;
				this.SetFaderActive(false);
			}

			// Token: 0x06002CBB RID: 11451 RVA: 0x002091AF File Offset: 0x002073AF
			public Window OpenWindow(string name, int width, int height)
			{
				Window result = this.InstantiateWindow(name, width, height);
				this.UpdateFader();
				return result;
			}

			// Token: 0x06002CBC RID: 11452 RVA: 0x002091C0 File Offset: 0x002073C0
			public Window OpenWindow(GameObject windowPrefab, string name)
			{
				if (windowPrefab == null)
				{
					Debug.LogError("Rewired Control Mapper: Window Prefab is null!");
					return null;
				}
				Window result = this.InstantiateWindow(name, windowPrefab);
				this.UpdateFader();
				return result;
			}

			// Token: 0x06002CBD RID: 11453 RVA: 0x002091E8 File Offset: 0x002073E8
			public void CloseTop()
			{
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (!(this.windows[i] == null))
					{
						this.DestroyWindow(this.windows[i]);
						this.windows.RemoveAt(i);
						break;
					}
					this.windows.RemoveAt(i);
				}
				this.UpdateFader();
			}

			// Token: 0x06002CBE RID: 11454 RVA: 0x00209255 File Offset: 0x00207455
			public void CloseWindow(int windowId)
			{
				this.CloseWindow(this.GetWindow(windowId));
			}

			// Token: 0x06002CBF RID: 11455 RVA: 0x00209264 File Offset: 0x00207464
			public void CloseWindow(Window window)
			{
				if (window == null)
				{
					return;
				}
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (this.windows[i] == null)
					{
						this.windows.RemoveAt(i);
					}
					else if (!(this.windows[i] != window))
					{
						this.DestroyWindow(this.windows[i]);
						this.windows.RemoveAt(i);
						break;
					}
				}
				this.UpdateFader();
				this.FocusTopWindow();
			}

			// Token: 0x06002CC0 RID: 11456 RVA: 0x002092F8 File Offset: 0x002074F8
			public void CloseAll()
			{
				this.SetFaderActive(false);
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (this.windows[i] == null)
					{
						this.windows.RemoveAt(i);
					}
					else
					{
						this.DestroyWindow(this.windows[i]);
						this.windows.RemoveAt(i);
					}
				}
				this.UpdateFader();
			}

			// Token: 0x06002CC1 RID: 11457 RVA: 0x0020936C File Offset: 0x0020756C
			public void CancelAll()
			{
				if (!this.isWindowOpen)
				{
					return;
				}
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (!(this.windows[i] == null))
					{
						this.windows[i].Cancel();
					}
				}
				this.CloseAll();
			}

			// Token: 0x06002CC2 RID: 11458 RVA: 0x002093C8 File Offset: 0x002075C8
			public Window GetWindow(int windowId)
			{
				if (windowId < 0)
				{
					return null;
				}
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (!(this.windows[i] == null) && this.windows[i].id == windowId)
					{
						return this.windows[i];
					}
				}
				return null;
			}

			// Token: 0x06002CC3 RID: 11459 RVA: 0x00209429 File Offset: 0x00207629
			public bool IsFocused(int windowId)
			{
				return windowId >= 0 && !(this.topWindow == null) && this.topWindow.id == windowId;
			}

			// Token: 0x06002CC4 RID: 11460 RVA: 0x0020944F File Offset: 0x0020764F
			public void Focus(int windowId)
			{
				this.Focus(this.GetWindow(windowId));
			}

			// Token: 0x06002CC5 RID: 11461 RVA: 0x0020945E File Offset: 0x0020765E
			public void Focus(Window window)
			{
				if (window == null)
				{
					return;
				}
				window.TakeInputFocus();
				this.DefocusOtherWindows(window.id);
			}

			// Token: 0x06002CC6 RID: 11462 RVA: 0x0020947C File Offset: 0x0020767C
			private void DefocusOtherWindows(int focusedWindowId)
			{
				if (focusedWindowId < 0)
				{
					return;
				}
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (!(this.windows[i] == null) && this.windows[i].id != focusedWindowId)
					{
						this.windows[i].Disable();
					}
				}
			}

			// Token: 0x06002CC7 RID: 11463 RVA: 0x002094E0 File Offset: 0x002076E0
			private void UpdateFader()
			{
				if (!this.isWindowOpen)
				{
					this.SetFaderActive(false);
					return;
				}
				if (this.topWindow.transform.parent == null)
				{
					return;
				}
				this.SetFaderActive(true);
				this.fader.transform.SetAsLastSibling();
				int siblingIndex = this.topWindow.transform.GetSiblingIndex();
				this.fader.transform.SetSiblingIndex(siblingIndex);
			}

			// Token: 0x06002CC8 RID: 11464 RVA: 0x0020954F File Offset: 0x0020774F
			private void FocusTopWindow()
			{
				if (this.topWindow == null)
				{
					return;
				}
				this.topWindow.TakeInputFocus();
			}

			// Token: 0x06002CC9 RID: 11465 RVA: 0x0020956B File Offset: 0x0020776B
			private void SetFaderActive(bool state)
			{
				this.fader.SetActive(state);
			}

			// Token: 0x06002CCA RID: 11466 RVA: 0x0020957C File Offset: 0x0020777C
			private Window InstantiateWindow(string name, int width, int height)
			{
				if (string.IsNullOrEmpty(name))
				{
					name = "Window";
				}
				GameObject gameObject = UITools.InstantiateGUIObject<Window>(this.windowPrefab, this.parent, name);
				if (gameObject == null)
				{
					return null;
				}
				Window component = gameObject.GetComponent<Window>();
				if (component != null)
				{
					component.Initialize(this.GetNewId(), new Func<int, bool>(this.IsFocused));
					this.windows.Add(component);
					component.SetSize(width, height);
				}
				return component;
			}

			// Token: 0x06002CCB RID: 11467 RVA: 0x002095F4 File Offset: 0x002077F4
			private Window InstantiateWindow(string name, GameObject windowPrefab)
			{
				if (string.IsNullOrEmpty(name))
				{
					name = "Window";
				}
				if (windowPrefab == null)
				{
					return null;
				}
				GameObject gameObject = UITools.InstantiateGUIObject<Window>(windowPrefab, this.parent, name);
				if (gameObject == null)
				{
					return null;
				}
				Window component = gameObject.GetComponent<Window>();
				if (component != null)
				{
					component.Initialize(this.GetNewId(), new Func<int, bool>(this.IsFocused));
					this.windows.Add(component);
				}
				return component;
			}

			// Token: 0x06002CCC RID: 11468 RVA: 0x00209669 File Offset: 0x00207869
			private void DestroyWindow(Window window)
			{
				if (window == null)
				{
					return;
				}
				Object.Destroy(window.gameObject);
			}

			// Token: 0x06002CCD RID: 11469 RVA: 0x00209680 File Offset: 0x00207880
			private int GetNewId()
			{
				int result = this.idCounter;
				this.idCounter++;
				return result;
			}

			// Token: 0x06002CCE RID: 11470 RVA: 0x00209696 File Offset: 0x00207896
			public void ClearCompletely()
			{
				this.CloseAll();
				if (this.fader != null)
				{
					Object.Destroy(this.fader);
				}
			}

			// Token: 0x040048CA RID: 18634
			private List<Window> windows;

			// Token: 0x040048CB RID: 18635
			private GameObject windowPrefab;

			// Token: 0x040048CC RID: 18636
			private Transform parent;

			// Token: 0x040048CD RID: 18637
			private GameObject fader;

			// Token: 0x040048CE RID: 18638
			private int idCounter;
		}
	}
}
