using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Rewired.Utils.Libraries.TinyJson;
using UnityEngine;

namespace Rewired.Data
{
	// Token: 0x02000832 RID: 2098
	public class UserDataStore_PlayerPrefs : UserDataStore
	{
		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06002939 RID: 10553 RVA: 0x001FB330 File Offset: 0x001F9530
		// (set) Token: 0x0600293A RID: 10554 RVA: 0x001FB338 File Offset: 0x001F9538
		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				this.isEnabled = value;
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x0600293B RID: 10555 RVA: 0x001FB341 File Offset: 0x001F9541
		// (set) Token: 0x0600293C RID: 10556 RVA: 0x001FB349 File Offset: 0x001F9549
		public bool LoadDataOnStart
		{
			get
			{
				return this.loadDataOnStart;
			}
			set
			{
				this.loadDataOnStart = value;
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x0600293D RID: 10557 RVA: 0x001FB352 File Offset: 0x001F9552
		// (set) Token: 0x0600293E RID: 10558 RVA: 0x001FB35A File Offset: 0x001F955A
		public bool LoadJoystickAssignments
		{
			get
			{
				return this.loadJoystickAssignments;
			}
			set
			{
				this.loadJoystickAssignments = value;
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x0600293F RID: 10559 RVA: 0x001FB363 File Offset: 0x001F9563
		// (set) Token: 0x06002940 RID: 10560 RVA: 0x001FB36B File Offset: 0x001F956B
		public bool LoadKeyboardAssignments
		{
			get
			{
				return this.loadKeyboardAssignments;
			}
			set
			{
				this.loadKeyboardAssignments = value;
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06002941 RID: 10561 RVA: 0x001FB374 File Offset: 0x001F9574
		// (set) Token: 0x06002942 RID: 10562 RVA: 0x001FB37C File Offset: 0x001F957C
		public bool LoadMouseAssignments
		{
			get
			{
				return this.loadMouseAssignments;
			}
			set
			{
				this.loadMouseAssignments = value;
			}
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06002943 RID: 10563 RVA: 0x001FB385 File Offset: 0x001F9585
		// (set) Token: 0x06002944 RID: 10564 RVA: 0x001FB38D File Offset: 0x001F958D
		public string PlayerPrefsKeyPrefix
		{
			get
			{
				return this.playerPrefsKeyPrefix;
			}
			set
			{
				this.playerPrefsKeyPrefix = value;
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06002945 RID: 10565 RVA: 0x001FB396 File Offset: 0x001F9596
		private string playerPrefsKey_controllerAssignments
		{
			get
			{
				return string.Format("{0}_{1}", this.playerPrefsKeyPrefix, "ControllerAssignments");
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06002946 RID: 10566 RVA: 0x001FB3AD File Offset: 0x001F95AD
		private bool loadControllerAssignments
		{
			get
			{
				return this.loadKeyboardAssignments || this.loadMouseAssignments || this.loadJoystickAssignments;
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06002947 RID: 10567 RVA: 0x001FB3C8 File Offset: 0x001F95C8
		private List<int> allActionIds
		{
			get
			{
				if (this.__allActionIds != null)
				{
					return this.__allActionIds;
				}
				List<int> list = new List<int>();
				IList<InputAction> actions = ReInput.mapping.Actions;
				for (int i = 0; i < actions.Count; i++)
				{
					list.Add(actions[i].id);
				}
				this.__allActionIds = list;
				return list;
			}
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06002948 RID: 10568 RVA: 0x001FB420 File Offset: 0x001F9620
		private string allActionIdsString
		{
			get
			{
				if (!string.IsNullOrEmpty(this.__allActionIdsString))
				{
					return this.__allActionIdsString;
				}
				StringBuilder stringBuilder = new StringBuilder();
				List<int> allActionIds = this.allActionIds;
				for (int i = 0; i < allActionIds.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(allActionIds[i]);
				}
				this.__allActionIdsString = stringBuilder.ToString();
				return this.__allActionIdsString;
			}
		}

		// Token: 0x06002949 RID: 10569 RVA: 0x001FB48F File Offset: 0x001F968F
		public override void Save()
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SaveAll();
		}

		// Token: 0x0600294A RID: 10570 RVA: 0x001FB4AB File Offset: 0x001F96AB
		public override void SaveControllerData(int playerId, ControllerType controllerType, int controllerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SaveControllerDataNow(playerId, controllerType, controllerId);
		}

		// Token: 0x0600294B RID: 10571 RVA: 0x001FB4CA File Offset: 0x001F96CA
		public override void SaveControllerData(ControllerType controllerType, int controllerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SaveControllerDataNow(controllerType, controllerId);
		}

		// Token: 0x0600294C RID: 10572 RVA: 0x001FB4E8 File Offset: 0x001F96E8
		public override void SavePlayerData(int playerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SavePlayerDataNow(playerId);
		}

		// Token: 0x0600294D RID: 10573 RVA: 0x001FB505 File Offset: 0x001F9705
		public override void SaveInputBehavior(int playerId, int behaviorId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SaveInputBehaviorNow(playerId, behaviorId);
		}

		// Token: 0x0600294E RID: 10574 RVA: 0x001FB523 File Offset: 0x001F9723
		public override void Load()
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadAll();
		}

		// Token: 0x0600294F RID: 10575 RVA: 0x001FB540 File Offset: 0x001F9740
		public override void LoadControllerData(int playerId, ControllerType controllerType, int controllerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadControllerDataNow(playerId, controllerType, controllerId);
		}

		// Token: 0x06002950 RID: 10576 RVA: 0x001FB560 File Offset: 0x001F9760
		public override void LoadControllerData(ControllerType controllerType, int controllerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadControllerDataNow(controllerType, controllerId);
		}

		// Token: 0x06002951 RID: 10577 RVA: 0x001FB57F File Offset: 0x001F977F
		public override void LoadPlayerData(int playerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadPlayerDataNow(playerId);
		}

		// Token: 0x06002952 RID: 10578 RVA: 0x001FB59D File Offset: 0x001F979D
		public override void LoadInputBehavior(int playerId, int behaviorId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadInputBehaviorNow(playerId, behaviorId);
		}

		// Token: 0x06002953 RID: 10579 RVA: 0x001FB5BC File Offset: 0x001F97BC
		protected override void OnInitialize()
		{
			if (this.loadDataOnStart)
			{
				this.Load();
				if (this.loadControllerAssignments && ReInput.controllers.joystickCount > 0)
				{
					this.wasJoystickEverDetected = true;
					this.SaveControllerAssignments();
				}
			}
		}

		// Token: 0x06002954 RID: 10580 RVA: 0x001FB5F0 File Offset: 0x001F97F0
		protected override void OnControllerConnected(ControllerStatusChangedEventArgs args)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (args.controllerType == 2)
			{
				this.LoadJoystickData(args.controllerId);
				if (this.loadDataOnStart && this.loadJoystickAssignments && !this.wasJoystickEverDetected)
				{
					base.StartCoroutine(this.LoadJoystickAssignmentsDeferred());
				}
				if (this.loadJoystickAssignments && !this.deferredJoystickAssignmentLoadPending)
				{
					this.SaveControllerAssignments();
				}
				this.wasJoystickEverDetected = true;
			}
		}

		// Token: 0x06002955 RID: 10581 RVA: 0x001FB65F File Offset: 0x001F985F
		protected override void OnControllerPreDisconnect(ControllerStatusChangedEventArgs args)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (args.controllerType == 2)
			{
				this.SaveJoystickData(args.controllerId);
			}
		}

		// Token: 0x06002956 RID: 10582 RVA: 0x001FB67F File Offset: 0x001F987F
		protected override void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (this.loadControllerAssignments)
			{
				this.SaveControllerAssignments();
			}
		}

		// Token: 0x06002957 RID: 10583 RVA: 0x001FB69C File Offset: 0x001F989C
		public override void SaveControllerMap(int playerId, ControllerMap controllerMap)
		{
			if (controllerMap == null)
			{
				return;
			}
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return;
			}
			this.SaveControllerMap(player, controllerMap);
		}

		// Token: 0x06002958 RID: 10584 RVA: 0x001FB6C8 File Offset: 0x001F98C8
		public override ControllerMap LoadControllerMap(int playerId, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return null;
			}
			return this.LoadControllerMap(player, controllerIdentifier, categoryId, layoutId);
		}

		// Token: 0x06002959 RID: 10585 RVA: 0x001FB6F4 File Offset: 0x001F98F4
		private int LoadAll()
		{
			int num = 0;
			if (this.loadControllerAssignments && this.LoadControllerAssignmentsNow())
			{
				num++;
			}
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				num += this.LoadPlayerDataNow(allPlayers[i]);
			}
			return num + this.LoadAllJoystickCalibrationData();
		}

		// Token: 0x0600295A RID: 10586 RVA: 0x001FB74D File Offset: 0x001F994D
		private int LoadPlayerDataNow(int playerId)
		{
			return this.LoadPlayerDataNow(ReInput.players.GetPlayer(playerId));
		}

		// Token: 0x0600295B RID: 10587 RVA: 0x001FB760 File Offset: 0x001F9960
		private int LoadPlayerDataNow(Player player)
		{
			if (player == null)
			{
				return 0;
			}
			int num = 0;
			num += this.LoadInputBehaviors(player.id);
			num += this.LoadControllerMaps(player.id, 0, 0);
			num += this.LoadControllerMaps(player.id, 1, 0);
			foreach (Joystick joystick in player.controllers.Joysticks)
			{
				num += this.LoadControllerMaps(player.id, 2, joystick.id);
			}
			this.RefreshLayoutManager(player.id);
			return num;
		}

		// Token: 0x0600295C RID: 10588 RVA: 0x001FB808 File Offset: 0x001F9A08
		private int LoadAllJoystickCalibrationData()
		{
			int num = 0;
			IList<Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int i = 0; i < joysticks.Count; i++)
			{
				num += this.LoadJoystickCalibrationData(joysticks[i]);
			}
			return num;
		}

		// Token: 0x0600295D RID: 10589 RVA: 0x001FB844 File Offset: 0x001F9A44
		private int LoadJoystickCalibrationData(Joystick joystick)
		{
			if (joystick == null)
			{
				return 0;
			}
			if (!joystick.ImportCalibrationMapFromXmlString(this.GetJoystickCalibrationMapXml(joystick)))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x0600295E RID: 10590 RVA: 0x001FB85D File Offset: 0x001F9A5D
		private int LoadJoystickCalibrationData(int joystickId)
		{
			return this.LoadJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
		}

		// Token: 0x0600295F RID: 10591 RVA: 0x001FB870 File Offset: 0x001F9A70
		private int LoadJoystickData(int joystickId)
		{
			int num = 0;
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (player.controllers.ContainsController(2, joystickId))
				{
					num += this.LoadControllerMaps(player.id, 2, joystickId);
					this.RefreshLayoutManager(player.id);
				}
			}
			return num + this.LoadJoystickCalibrationData(joystickId);
		}

		// Token: 0x06002960 RID: 10592 RVA: 0x001FB8DA File Offset: 0x001F9ADA
		private int LoadControllerDataNow(int playerId, ControllerType controllerType, int controllerId)
		{
			int num = 0 + this.LoadControllerMaps(playerId, controllerType, controllerId);
			this.RefreshLayoutManager(playerId);
			return num + this.LoadControllerDataNow(controllerType, controllerId);
		}

		// Token: 0x06002961 RID: 10593 RVA: 0x001FB8F8 File Offset: 0x001F9AF8
		private int LoadControllerDataNow(ControllerType controllerType, int controllerId)
		{
			int num = 0;
			if (controllerType == 2)
			{
				num += this.LoadJoystickCalibrationData(controllerId);
			}
			return num;
		}

		// Token: 0x06002962 RID: 10594 RVA: 0x001FB918 File Offset: 0x001F9B18
		private int LoadControllerMaps(int playerId, ControllerType controllerType, int controllerId)
		{
			int num = 0;
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return num;
			}
			Controller controller = ReInput.controllers.GetController(controllerType, controllerId);
			if (controller == null)
			{
				return num;
			}
			IList<InputMapCategory> mapCategories = ReInput.mapping.MapCategories;
			for (int i = 0; i < mapCategories.Count; i++)
			{
				InputMapCategory inputMapCategory = mapCategories[i];
				if (inputMapCategory.userAssignable)
				{
					IList<InputLayout> list = ReInput.mapping.MapLayouts(controller.type);
					for (int j = 0; j < list.Count; j++)
					{
						InputLayout inputLayout = list[j];
						ControllerMap controllerMap = this.LoadControllerMap(player, controller.identifier, inputMapCategory.id, inputLayout.id);
						if (controllerMap != null)
						{
							player.controllers.maps.AddMap(controller, controllerMap);
							num++;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06002963 RID: 10595 RVA: 0x001FB9F0 File Offset: 0x001F9BF0
		private ControllerMap LoadControllerMap(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId)
		{
			if (player == null)
			{
				return null;
			}
			string controllerMapXml = this.GetControllerMapXml(player, controllerIdentifier, categoryId, layoutId);
			if (string.IsNullOrEmpty(controllerMapXml))
			{
				return null;
			}
			ControllerMap controllerMap = ControllerMap.CreateFromXml(controllerIdentifier.controllerType, controllerMapXml);
			if (controllerMap == null)
			{
				return null;
			}
			List<int> controllerMapKnownActionIds = this.GetControllerMapKnownActionIds(player, controllerIdentifier, categoryId, layoutId);
			this.AddDefaultMappingsForNewActions(controllerIdentifier, controllerMap, controllerMapKnownActionIds);
			return controllerMap;
		}

		// Token: 0x06002964 RID: 10596 RVA: 0x001FBA44 File Offset: 0x001F9C44
		private int LoadInputBehaviors(int playerId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return 0;
			}
			int num = 0;
			IList<InputBehavior> inputBehaviors = ReInput.mapping.GetInputBehaviors(player.id);
			for (int i = 0; i < inputBehaviors.Count; i++)
			{
				num += this.LoadInputBehaviorNow(player, inputBehaviors[i]);
			}
			return num;
		}

		// Token: 0x06002965 RID: 10597 RVA: 0x001FBA98 File Offset: 0x001F9C98
		private int LoadInputBehaviorNow(int playerId, int behaviorId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return 0;
			}
			InputBehavior inputBehavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
			if (inputBehavior == null)
			{
				return 0;
			}
			return this.LoadInputBehaviorNow(player, inputBehavior);
		}

		// Token: 0x06002966 RID: 10598 RVA: 0x001FBAD0 File Offset: 0x001F9CD0
		private int LoadInputBehaviorNow(Player player, InputBehavior inputBehavior)
		{
			if (player == null || inputBehavior == null)
			{
				return 0;
			}
			string inputBehaviorXml = this.GetInputBehaviorXml(player, inputBehavior.id);
			if (inputBehaviorXml == null || inputBehaviorXml == string.Empty)
			{
				return 0;
			}
			if (!inputBehavior.ImportXmlString(inputBehaviorXml))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06002967 RID: 10599 RVA: 0x001FBB14 File Offset: 0x001F9D14
		private bool LoadControllerAssignmentsNow()
		{
			try
			{
				UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo controllerAssignmentSaveInfo = this.LoadControllerAssignmentData();
				if (controllerAssignmentSaveInfo == null)
				{
					return false;
				}
				if (this.loadKeyboardAssignments || this.loadMouseAssignments)
				{
					this.LoadKeyboardAndMouseAssignmentsNow(controllerAssignmentSaveInfo);
				}
				if (this.loadJoystickAssignments)
				{
					this.LoadJoystickAssignmentsNow(controllerAssignmentSaveInfo);
				}
			}
			catch
			{
			}
			return true;
		}

		// Token: 0x06002968 RID: 10600 RVA: 0x001FBB70 File Offset: 0x001F9D70
		private bool LoadKeyboardAndMouseAssignmentsNow(UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo data)
		{
			try
			{
				if (data == null && (data = this.LoadControllerAssignmentData()) == null)
				{
					return false;
				}
				foreach (Player player in ReInput.players.AllPlayers)
				{
					if (data.ContainsPlayer(player.id))
					{
						UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.PlayerInfo playerInfo = data.players[data.IndexOfPlayer(player.id)];
						if (this.loadKeyboardAssignments)
						{
							player.controllers.hasKeyboard = playerInfo.hasKeyboard;
						}
						if (this.loadMouseAssignments)
						{
							player.controllers.hasMouse = playerInfo.hasMouse;
						}
					}
				}
			}
			catch
			{
			}
			return true;
		}

		// Token: 0x06002969 RID: 10601 RVA: 0x001FBC38 File Offset: 0x001F9E38
		private bool LoadJoystickAssignmentsNow(UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo data)
		{
			try
			{
				if (ReInput.controllers.joystickCount == 0)
				{
					return false;
				}
				if (data == null && (data = this.LoadControllerAssignmentData()) == null)
				{
					return false;
				}
				foreach (Player player in ReInput.players.AllPlayers)
				{
					player.controllers.ClearControllersOfType(2);
				}
				List<UserDataStore_PlayerPrefs.JoystickAssignmentHistoryInfo> list = this.loadJoystickAssignments ? new List<UserDataStore_PlayerPrefs.JoystickAssignmentHistoryInfo>() : null;
				foreach (Player player2 in ReInput.players.AllPlayers)
				{
					if (data.ContainsPlayer(player2.id))
					{
						UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.PlayerInfo playerInfo = data.players[data.IndexOfPlayer(player2.id)];
						for (int i = 0; i < playerInfo.joystickCount; i++)
						{
							UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.JoystickInfo joystickInfo2 = playerInfo.joysticks[i];
							if (joystickInfo2 != null)
							{
								Joystick joystick = this.FindJoystickPrecise(joystickInfo2);
								if (joystick != null)
								{
									if (list.Find((UserDataStore_PlayerPrefs.JoystickAssignmentHistoryInfo x) => x.joystick == joystick) == null)
									{
										list.Add(new UserDataStore_PlayerPrefs.JoystickAssignmentHistoryInfo(joystick, joystickInfo2.id));
									}
									player2.controllers.AddController(joystick, false);
								}
							}
						}
					}
				}
				if (this.allowImpreciseJoystickAssignmentMatching)
				{
					foreach (Player player3 in ReInput.players.AllPlayers)
					{
						if (data.ContainsPlayer(player3.id))
						{
							UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.PlayerInfo playerInfo2 = data.players[data.IndexOfPlayer(player3.id)];
							for (int j = 0; j < playerInfo2.joystickCount; j++)
							{
								UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.JoystickInfo joystickInfo = playerInfo2.joysticks[j];
								if (joystickInfo != null)
								{
									Joystick joystick2 = null;
									int num = list.FindIndex((UserDataStore_PlayerPrefs.JoystickAssignmentHistoryInfo x) => x.oldJoystickId == joystickInfo.id);
									if (num >= 0)
									{
										joystick2 = list[num].joystick;
									}
									else
									{
										List<Joystick> list2;
										if (!this.TryFindJoysticksImprecise(joystickInfo, out list2))
										{
											goto IL_298;
										}
										using (List<Joystick>.Enumerator enumerator2 = list2.GetEnumerator())
										{
											while (enumerator2.MoveNext())
											{
												Joystick match = enumerator2.Current;
												if (list.Find((UserDataStore_PlayerPrefs.JoystickAssignmentHistoryInfo x) => x.joystick == match) == null)
												{
													joystick2 = match;
													break;
												}
											}
										}
										if (joystick2 == null)
										{
											goto IL_298;
										}
										list.Add(new UserDataStore_PlayerPrefs.JoystickAssignmentHistoryInfo(joystick2, joystickInfo.id));
									}
									player3.controllers.AddController(joystick2, false);
								}
								IL_298:;
							}
						}
					}
				}
			}
			catch
			{
			}
			if (ReInput.configuration.autoAssignJoysticks)
			{
				ReInput.controllers.AutoAssignJoysticks();
			}
			return true;
		}

		// Token: 0x0600296A RID: 10602 RVA: 0x001FBFA4 File Offset: 0x001FA1A4
		private UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo LoadControllerAssignmentData()
		{
			UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo result;
			try
			{
				if (!PlayerPrefs.HasKey(this.playerPrefsKey_controllerAssignments))
				{
					result = null;
				}
				else
				{
					string @string = PlayerPrefs.GetString(this.playerPrefsKey_controllerAssignments);
					if (string.IsNullOrEmpty(@string))
					{
						result = null;
					}
					else
					{
						UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo controllerAssignmentSaveInfo = JsonParser.FromJson<UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo>(@string);
						if (controllerAssignmentSaveInfo == null || controllerAssignmentSaveInfo.playerCount == 0)
						{
							result = null;
						}
						else
						{
							result = controllerAssignmentSaveInfo;
						}
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600296B RID: 10603 RVA: 0x001FC00C File Offset: 0x001FA20C
		private IEnumerator LoadJoystickAssignmentsDeferred()
		{
			this.deferredJoystickAssignmentLoadPending = true;
			yield return new WaitForEndOfFrame();
			if (!ReInput.isReady)
			{
				yield break;
			}
			this.LoadJoystickAssignmentsNow(null);
			this.SaveControllerAssignments();
			this.deferredJoystickAssignmentLoadPending = false;
			yield break;
		}

		// Token: 0x0600296C RID: 10604 RVA: 0x001FC01C File Offset: 0x001FA21C
		private void SaveAll()
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				this.SavePlayerDataNow(allPlayers[i]);
			}
			this.SaveAllJoystickCalibrationData();
			if (this.loadControllerAssignments)
			{
				this.SaveControllerAssignments();
			}
			PlayerPrefs.Save();
		}

		// Token: 0x0600296D RID: 10605 RVA: 0x001FC06C File Offset: 0x001FA26C
		private void SavePlayerDataNow(int playerId)
		{
			this.SavePlayerDataNow(ReInput.players.GetPlayer(playerId));
			PlayerPrefs.Save();
		}

		// Token: 0x0600296E RID: 10606 RVA: 0x001FC084 File Offset: 0x001FA284
		private void SavePlayerDataNow(Player player)
		{
			if (player == null)
			{
				return;
			}
			PlayerSaveData saveData = player.GetSaveData(true);
			this.SaveInputBehaviors(player, saveData);
			this.SaveControllerMaps(player, saveData);
		}

		// Token: 0x0600296F RID: 10607 RVA: 0x001FC0B0 File Offset: 0x001FA2B0
		private void SaveAllJoystickCalibrationData()
		{
			IList<Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int i = 0; i < joysticks.Count; i++)
			{
				this.SaveJoystickCalibrationData(joysticks[i]);
			}
		}

		// Token: 0x06002970 RID: 10608 RVA: 0x001FC0E6 File Offset: 0x001FA2E6
		private void SaveJoystickCalibrationData(int joystickId)
		{
			this.SaveJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
		}

		// Token: 0x06002971 RID: 10609 RVA: 0x001FC0FC File Offset: 0x001FA2FC
		private void SaveJoystickCalibrationData(Joystick joystick)
		{
			if (joystick == null)
			{
				return;
			}
			JoystickCalibrationMapSaveData calibrationMapSaveData = joystick.GetCalibrationMapSaveData();
			PlayerPrefs.SetString(this.GetJoystickCalibrationMapPlayerPrefsKey(joystick), calibrationMapSaveData.map.ToXmlString());
		}

		// Token: 0x06002972 RID: 10610 RVA: 0x001FC12C File Offset: 0x001FA32C
		private void SaveJoystickData(int joystickId)
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (player.controllers.ContainsController(2, joystickId))
				{
					this.SaveControllerMaps(player.id, 2, joystickId);
				}
			}
			this.SaveJoystickCalibrationData(joystickId);
		}

		// Token: 0x06002973 RID: 10611 RVA: 0x001FC181 File Offset: 0x001FA381
		private void SaveControllerDataNow(int playerId, ControllerType controllerType, int controllerId)
		{
			this.SaveControllerMaps(playerId, controllerType, controllerId);
			this.SaveControllerDataNow(controllerType, controllerId);
			PlayerPrefs.Save();
		}

		// Token: 0x06002974 RID: 10612 RVA: 0x001FC199 File Offset: 0x001FA399
		private void SaveControllerDataNow(ControllerType controllerType, int controllerId)
		{
			if (controllerType == 2)
			{
				this.SaveJoystickCalibrationData(controllerId);
			}
			PlayerPrefs.Save();
		}

		// Token: 0x06002975 RID: 10613 RVA: 0x001FC1AC File Offset: 0x001FA3AC
		private void SaveControllerMaps(Player player, PlayerSaveData playerSaveData)
		{
			foreach (ControllerMapSaveData controllerMapSaveData in playerSaveData.AllControllerMapSaveData)
			{
				this.SaveControllerMap(player, controllerMapSaveData.map);
			}
		}

		// Token: 0x06002976 RID: 10614 RVA: 0x001FC200 File Offset: 0x001FA400
		private void SaveControllerMaps(int playerId, ControllerType controllerType, int controllerId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return;
			}
			if (!player.controllers.ContainsController(controllerType, controllerId))
			{
				return;
			}
			ControllerMapSaveData[] mapSaveData = player.controllers.maps.GetMapSaveData(controllerType, controllerId, true);
			if (mapSaveData == null)
			{
				return;
			}
			for (int i = 0; i < mapSaveData.Length; i++)
			{
				this.SaveControllerMap(player, mapSaveData[i].map);
			}
		}

		// Token: 0x06002977 RID: 10615 RVA: 0x001FC264 File Offset: 0x001FA464
		private void SaveControllerMap(Player player, ControllerMap controllerMap)
		{
			PlayerPrefs.SetString(this.GetControllerMapPlayerPrefsKey(player, controllerMap.controller.identifier, controllerMap.categoryId, controllerMap.layoutId, 2), controllerMap.ToXmlString());
			PlayerPrefs.SetString(this.GetControllerMapKnownActionIdsPlayerPrefsKey(player, controllerMap.controller.identifier, controllerMap.categoryId, controllerMap.layoutId, 2), this.allActionIdsString);
		}

		// Token: 0x06002978 RID: 10616 RVA: 0x001FC2C8 File Offset: 0x001FA4C8
		private void SaveInputBehaviors(Player player, PlayerSaveData playerSaveData)
		{
			if (player == null)
			{
				return;
			}
			InputBehavior[] inputBehaviors = playerSaveData.inputBehaviors;
			for (int i = 0; i < inputBehaviors.Length; i++)
			{
				this.SaveInputBehaviorNow(player, inputBehaviors[i]);
			}
		}

		// Token: 0x06002979 RID: 10617 RVA: 0x001FC2FC File Offset: 0x001FA4FC
		private void SaveInputBehaviorNow(int playerId, int behaviorId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return;
			}
			InputBehavior inputBehavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
			if (inputBehavior == null)
			{
				return;
			}
			this.SaveInputBehaviorNow(player, inputBehavior);
			PlayerPrefs.Save();
		}

		// Token: 0x0600297A RID: 10618 RVA: 0x001FC337 File Offset: 0x001FA537
		private void SaveInputBehaviorNow(Player player, InputBehavior inputBehavior)
		{
			if (player == null || inputBehavior == null)
			{
				return;
			}
			PlayerPrefs.SetString(this.GetInputBehaviorPlayerPrefsKey(player, inputBehavior.id), inputBehavior.ToXmlString());
		}

		// Token: 0x0600297B RID: 10619 RVA: 0x001FC358 File Offset: 0x001FA558
		private bool SaveControllerAssignments()
		{
			try
			{
				UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo controllerAssignmentSaveInfo = new UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo(ReInput.players.allPlayerCount);
				for (int i = 0; i < ReInput.players.allPlayerCount; i++)
				{
					Player player = ReInput.players.AllPlayers[i];
					UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.PlayerInfo playerInfo = new UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.PlayerInfo();
					controllerAssignmentSaveInfo.players[i] = playerInfo;
					playerInfo.id = player.id;
					playerInfo.hasKeyboard = player.controllers.hasKeyboard;
					playerInfo.hasMouse = player.controllers.hasMouse;
					UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.JoystickInfo[] array = new UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.JoystickInfo[player.controllers.joystickCount];
					playerInfo.joysticks = array;
					for (int j = 0; j < player.controllers.joystickCount; j++)
					{
						Joystick joystick = player.controllers.Joysticks[j];
						array[j] = new UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.JoystickInfo
						{
							instanceGuid = joystick.deviceInstanceGuid,
							id = joystick.id,
							hardwareIdentifier = joystick.hardwareIdentifier
						};
					}
				}
				PlayerPrefs.SetString(this.playerPrefsKey_controllerAssignments, JsonWriter.ToJson(controllerAssignmentSaveInfo));
				PlayerPrefs.Save();
			}
			catch
			{
			}
			return true;
		}

		// Token: 0x0600297C RID: 10620 RVA: 0x001FC498 File Offset: 0x001FA698
		private bool ControllerAssignmentSaveDataExists()
		{
			return PlayerPrefs.HasKey(this.playerPrefsKey_controllerAssignments) && !string.IsNullOrEmpty(PlayerPrefs.GetString(this.playerPrefsKey_controllerAssignments));
		}

		// Token: 0x0600297D RID: 10621 RVA: 0x001FC4BE File Offset: 0x001FA6BE
		private string GetBasePlayerPrefsKey(Player player)
		{
			return this.playerPrefsKeyPrefix + "|playerName=" + player.name;
		}

		// Token: 0x0600297E RID: 10622 RVA: 0x001FC4D6 File Offset: 0x001FA6D6
		private string GetControllerMapPlayerPrefsKey(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId, int ppKeyVersion)
		{
			return this.GetBasePlayerPrefsKey(player) + "|dataType=ControllerMap" + UserDataStore_PlayerPrefs.GetControllerMapPlayerPrefsKeyCommonSuffix(player, controllerIdentifier, categoryId, layoutId, ppKeyVersion);
		}

		// Token: 0x0600297F RID: 10623 RVA: 0x001FC4FA File Offset: 0x001FA6FA
		private string GetControllerMapKnownActionIdsPlayerPrefsKey(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId, int ppKeyVersion)
		{
			return this.GetBasePlayerPrefsKey(player) + "|dataType=ControllerMap_KnownActionIds" + UserDataStore_PlayerPrefs.GetControllerMapPlayerPrefsKeyCommonSuffix(player, controllerIdentifier, categoryId, layoutId, ppKeyVersion);
		}

		// Token: 0x06002980 RID: 10624 RVA: 0x001FC520 File Offset: 0x001FA720
		private static string GetControllerMapPlayerPrefsKeyCommonSuffix(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId, int ppKeyVersion)
		{
			string text = "";
			if (ppKeyVersion >= 2)
			{
				text = text + "|kv=" + ppKeyVersion.ToString();
			}
			text = text + "|controllerMapType=" + UserDataStore_PlayerPrefs.GetControllerMapType(controllerIdentifier.controllerType).Name;
			text = string.Concat(new string[]
			{
				text,
				"|categoryId=",
				categoryId.ToString(),
				"|layoutId=",
				layoutId.ToString()
			});
			if (ppKeyVersion >= 2)
			{
				text = text + "|hardwareGuid=" + controllerIdentifier.hardwareTypeGuid.ToString();
				if (controllerIdentifier.hardwareTypeGuid == Guid.Empty)
				{
					text = text + "|hardwareIdentifier=" + controllerIdentifier.hardwareIdentifier;
				}
				if (controllerIdentifier.controllerType == 2)
				{
					text = text + "|duplicate=" + UserDataStore_PlayerPrefs.GetDuplicateIndex(player, controllerIdentifier).ToString();
				}
			}
			else
			{
				text = text + "|hardwareIdentifier=" + controllerIdentifier.hardwareIdentifier;
				if (controllerIdentifier.controllerType == 2)
				{
					text = text + "|hardwareGuid=" + controllerIdentifier.hardwareTypeGuid.ToString();
					if (ppKeyVersion >= 1)
					{
						text = text + "|duplicate=" + UserDataStore_PlayerPrefs.GetDuplicateIndex(player, controllerIdentifier).ToString();
					}
				}
			}
			return text;
		}

		// Token: 0x06002981 RID: 10625 RVA: 0x001FC670 File Offset: 0x001FA870
		private string GetJoystickCalibrationMapPlayerPrefsKey(Joystick joystick)
		{
			return this.playerPrefsKeyPrefix + "|dataType=CalibrationMap" + "|controllerType=" + joystick.type.ToString() + "|hardwareIdentifier=" + joystick.hardwareIdentifier + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
		}

		// Token: 0x06002982 RID: 10626 RVA: 0x001FC6D9 File Offset: 0x001FA8D9
		private string GetInputBehaviorPlayerPrefsKey(Player player, int inputBehaviorId)
		{
			return this.GetBasePlayerPrefsKey(player) + "|dataType=InputBehavior" + "|id=" + inputBehaviorId.ToString();
		}

		// Token: 0x06002983 RID: 10627 RVA: 0x001FC700 File Offset: 0x001FA900
		private string GetControllerMapXml(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId)
		{
			for (int i = 2; i >= 0; i--)
			{
				string controllerMapPlayerPrefsKey = this.GetControllerMapPlayerPrefsKey(player, controllerIdentifier, categoryId, layoutId, i);
				if (PlayerPrefs.HasKey(controllerMapPlayerPrefsKey))
				{
					return PlayerPrefs.GetString(controllerMapPlayerPrefsKey);
				}
			}
			return null;
		}

		// Token: 0x06002984 RID: 10628 RVA: 0x001FC738 File Offset: 0x001FA938
		private List<int> GetControllerMapKnownActionIds(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId)
		{
			List<int> list = new List<int>();
			string text = null;
			bool flag = false;
			for (int i = 2; i >= 0; i--)
			{
				text = this.GetControllerMapKnownActionIdsPlayerPrefsKey(player, controllerIdentifier, categoryId, layoutId, i);
				if (PlayerPrefs.HasKey(text))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return list;
			}
			string @string = PlayerPrefs.GetString(text);
			if (string.IsNullOrEmpty(@string))
			{
				return list;
			}
			string[] array = @string.Split(',', 0);
			for (int j = 0; j < array.Length; j++)
			{
				int num;
				if (!string.IsNullOrEmpty(array[j]) && int.TryParse(array[j], ref num))
				{
					list.Add(num);
				}
			}
			return list;
		}

		// Token: 0x06002985 RID: 10629 RVA: 0x001FC7D0 File Offset: 0x001FA9D0
		private string GetJoystickCalibrationMapXml(Joystick joystick)
		{
			string joystickCalibrationMapPlayerPrefsKey = this.GetJoystickCalibrationMapPlayerPrefsKey(joystick);
			if (!PlayerPrefs.HasKey(joystickCalibrationMapPlayerPrefsKey))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(joystickCalibrationMapPlayerPrefsKey);
		}

		// Token: 0x06002986 RID: 10630 RVA: 0x001FC7FC File Offset: 0x001FA9FC
		private string GetInputBehaviorXml(Player player, int id)
		{
			string inputBehaviorPlayerPrefsKey = this.GetInputBehaviorPlayerPrefsKey(player, id);
			if (!PlayerPrefs.HasKey(inputBehaviorPlayerPrefsKey))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(inputBehaviorPlayerPrefsKey);
		}

		// Token: 0x06002987 RID: 10631 RVA: 0x001FC828 File Offset: 0x001FAA28
		private void AddDefaultMappingsForNewActions(ControllerIdentifier controllerIdentifier, ControllerMap controllerMap, List<int> knownActionIds)
		{
			if (controllerMap == null || knownActionIds == null)
			{
				return;
			}
			if (knownActionIds == null || knownActionIds.Count == 0)
			{
				return;
			}
			ControllerMap controllerMapInstance = ReInput.mapping.GetControllerMapInstance(controllerIdentifier, controllerMap.categoryId, controllerMap.layoutId);
			if (controllerMapInstance == null)
			{
				return;
			}
			List<int> list = new List<int>();
			foreach (int num in this.allActionIds)
			{
				if (!knownActionIds.Contains(num))
				{
					list.Add(num);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			foreach (ActionElementMap actionElementMap in controllerMapInstance.AllMaps)
			{
				if (list.Contains(actionElementMap.actionId) && !controllerMap.DoesElementAssignmentConflict(actionElementMap))
				{
					ElementAssignment elementAssignment;
					elementAssignment..ctor(controllerMap.controllerType, actionElementMap.elementType, actionElementMap.elementIdentifierId, actionElementMap.axisRange, actionElementMap.keyCode, actionElementMap.modifierKeyFlags, actionElementMap.actionId, actionElementMap.axisContribution, actionElementMap.invert);
					controllerMap.CreateElementMap(elementAssignment);
				}
			}
		}

		// Token: 0x06002988 RID: 10632 RVA: 0x001FC964 File Offset: 0x001FAB64
		private Joystick FindJoystickPrecise(UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.JoystickInfo joystickInfo)
		{
			if (joystickInfo == null)
			{
				return null;
			}
			if (joystickInfo.instanceGuid == Guid.Empty)
			{
				return null;
			}
			IList<Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int i = 0; i < joysticks.Count; i++)
			{
				if (joysticks[i].deviceInstanceGuid == joystickInfo.instanceGuid)
				{
					return joysticks[i];
				}
			}
			return null;
		}

		// Token: 0x06002989 RID: 10633 RVA: 0x001FC9C8 File Offset: 0x001FABC8
		private bool TryFindJoysticksImprecise(UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.JoystickInfo joystickInfo, out List<Joystick> matches)
		{
			matches = null;
			if (joystickInfo == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(joystickInfo.hardwareIdentifier))
			{
				return false;
			}
			IList<Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int i = 0; i < joysticks.Count; i++)
			{
				if (string.Equals(joysticks[i].hardwareIdentifier, joystickInfo.hardwareIdentifier, 5))
				{
					if (matches == null)
					{
						matches = new List<Joystick>();
					}
					matches.Add(joysticks[i]);
				}
			}
			return matches != null;
		}

		// Token: 0x0600298A RID: 10634 RVA: 0x001FCA40 File Offset: 0x001FAC40
		private static int GetDuplicateIndex(Player player, ControllerIdentifier controllerIdentifier)
		{
			Controller controller = ReInput.controllers.GetController(controllerIdentifier);
			if (controller == null)
			{
				return 0;
			}
			int num = 0;
			foreach (Controller controller2 in player.controllers.Controllers)
			{
				if (controller2.type == controller.type)
				{
					bool flag = false;
					if (controller.type == 2)
					{
						if ((controller2 as Joystick).hardwareTypeGuid != controller.hardwareTypeGuid)
						{
							continue;
						}
						if (controller.hardwareTypeGuid != Guid.Empty)
						{
							flag = true;
						}
					}
					if (flag || !(controller2.hardwareIdentifier != controller.hardwareIdentifier))
					{
						if (controller2 == controller)
						{
							return num;
						}
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x0600298B RID: 10635 RVA: 0x001FCB10 File Offset: 0x001FAD10
		private void RefreshLayoutManager(int playerId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return;
			}
			player.controllers.maps.layoutManager.Apply();
		}

		// Token: 0x0600298C RID: 10636 RVA: 0x001FCB44 File Offset: 0x001FAD44
		private static Type GetControllerMapType(ControllerType controllerType)
		{
			switch (controllerType)
			{
			case 0:
				return typeof(KeyboardMap);
			case 1:
				return typeof(MouseMap);
			case 2:
				return typeof(JoystickMap);
			default:
				if (controllerType == 20)
				{
					return typeof(CustomControllerMap);
				}
				Debug.LogWarning("Rewired: Unknown ControllerType " + controllerType.ToString());
				return null;
			}
		}

		// Token: 0x04004734 RID: 18228
		private const string thisScriptName = "UserDataStore_PlayerPrefs";

		// Token: 0x04004735 RID: 18229
		private const string logPrefix = "Rewired: ";

		// Token: 0x04004736 RID: 18230
		private const string editorLoadedMessage = "\n***IMPORTANT:*** Changes made to the Rewired Input Manager configuration after the last time XML data was saved WILL NOT be used because the loaded old saved data has overwritten these values. If you change something in the Rewired Input Manager such as a Joystick Map or Input Behavior settings, you will not see these changes reflected in the current configuration. Clear PlayerPrefs using the inspector option on the UserDataStore_PlayerPrefs component.";

		// Token: 0x04004737 RID: 18231
		private const string playerPrefsKeySuffix_controllerAssignments = "ControllerAssignments";

		// Token: 0x04004738 RID: 18232
		private const int controllerMapPPKeyVersion_original = 0;

		// Token: 0x04004739 RID: 18233
		private const int controllerMapPPKeyVersion_includeDuplicateJoystickIndex = 1;

		// Token: 0x0400473A RID: 18234
		private const int controllerMapPPKeyVersion_supportDisconnectedControllers = 2;

		// Token: 0x0400473B RID: 18235
		private const int controllerMapPPKeyVersion_includeFormatVersion = 2;

		// Token: 0x0400473C RID: 18236
		private const int controllerMapPPKeyVersion = 2;

		// Token: 0x0400473D RID: 18237
		[SerializeField]
		[Tooltip("Should this script be used? If disabled, nothing will be saved or loaded.")]
		private bool isEnabled = true;

		// Token: 0x0400473E RID: 18238
		[SerializeField]
		[Tooltip("Should saved data be loaded on start?")]
		private bool loadDataOnStart = true;

		// Token: 0x0400473F RID: 18239
		[Tooltip("Should Player Joystick assignments be saved and loaded? This is not totally reliable for all Joysticks on all platforms. Some platforms/input sources do not provide enough information to reliably save assignments from session to session and reboot to reboot.")]
		[SerializeField]
		private bool loadJoystickAssignments = true;

		// Token: 0x04004740 RID: 18240
		[Tooltip("Should Player Keyboard assignments be saved and loaded?")]
		[SerializeField]
		private bool loadKeyboardAssignments = true;

		// Token: 0x04004741 RID: 18241
		[Tooltip("Should Player Mouse assignments be saved and loaded?")]
		[SerializeField]
		private bool loadMouseAssignments = true;

		// Token: 0x04004742 RID: 18242
		[Tooltip("The PlayerPrefs key prefix. Change this to change how keys are stored in PlayerPrefs. Changing this will make saved data already stored with the old key no longer accessible.")]
		[SerializeField]
		private string playerPrefsKeyPrefix = "RewiredSaveData";

		// Token: 0x04004743 RID: 18243
		[NonSerialized]
		private bool allowImpreciseJoystickAssignmentMatching = true;

		// Token: 0x04004744 RID: 18244
		[NonSerialized]
		private bool deferredJoystickAssignmentLoadPending;

		// Token: 0x04004745 RID: 18245
		[NonSerialized]
		private bool wasJoystickEverDetected;

		// Token: 0x04004746 RID: 18246
		[NonSerialized]
		private List<int> __allActionIds;

		// Token: 0x04004747 RID: 18247
		[NonSerialized]
		private string __allActionIdsString;

		// Token: 0x02000833 RID: 2099
		private class ControllerAssignmentSaveInfo
		{
			// Token: 0x17000333 RID: 819
			// (get) Token: 0x0600298E RID: 10638 RVA: 0x001FCBEE File Offset: 0x001FADEE
			public int playerCount
			{
				get
				{
					if (this.players == null)
					{
						return 0;
					}
					return this.players.Length;
				}
			}

			// Token: 0x0600298F RID: 10639 RVA: 0x00003EBD File Offset: 0x000020BD
			public ControllerAssignmentSaveInfo()
			{
			}

			// Token: 0x06002990 RID: 10640 RVA: 0x001FCC04 File Offset: 0x001FAE04
			public ControllerAssignmentSaveInfo(int playerCount)
			{
				this.players = new UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.PlayerInfo[playerCount];
				for (int i = 0; i < playerCount; i++)
				{
					this.players[i] = new UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.PlayerInfo();
				}
			}

			// Token: 0x06002991 RID: 10641 RVA: 0x001FCC3C File Offset: 0x001FAE3C
			public int IndexOfPlayer(int playerId)
			{
				for (int i = 0; i < this.playerCount; i++)
				{
					if (this.players[i] != null && this.players[i].id == playerId)
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x06002992 RID: 10642 RVA: 0x001FCC77 File Offset: 0x001FAE77
			public bool ContainsPlayer(int playerId)
			{
				return this.IndexOfPlayer(playerId) >= 0;
			}

			// Token: 0x04004748 RID: 18248
			public UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.PlayerInfo[] players;

			// Token: 0x02000834 RID: 2100
			public class PlayerInfo
			{
				// Token: 0x17000334 RID: 820
				// (get) Token: 0x06002993 RID: 10643 RVA: 0x001FCC86 File Offset: 0x001FAE86
				public int joystickCount
				{
					get
					{
						if (this.joysticks == null)
						{
							return 0;
						}
						return this.joysticks.Length;
					}
				}

				// Token: 0x06002994 RID: 10644 RVA: 0x001FCC9C File Offset: 0x001FAE9C
				public int IndexOfJoystick(int joystickId)
				{
					for (int i = 0; i < this.joystickCount; i++)
					{
						if (this.joysticks[i] != null && this.joysticks[i].id == joystickId)
						{
							return i;
						}
					}
					return -1;
				}

				// Token: 0x06002995 RID: 10645 RVA: 0x001FCCD7 File Offset: 0x001FAED7
				public bool ContainsJoystick(int joystickId)
				{
					return this.IndexOfJoystick(joystickId) >= 0;
				}

				// Token: 0x04004749 RID: 18249
				public int id;

				// Token: 0x0400474A RID: 18250
				public bool hasKeyboard;

				// Token: 0x0400474B RID: 18251
				public bool hasMouse;

				// Token: 0x0400474C RID: 18252
				public UserDataStore_PlayerPrefs.ControllerAssignmentSaveInfo.JoystickInfo[] joysticks;
			}

			// Token: 0x02000835 RID: 2101
			public class JoystickInfo
			{
				// Token: 0x0400474D RID: 18253
				public Guid instanceGuid;

				// Token: 0x0400474E RID: 18254
				public string hardwareIdentifier;

				// Token: 0x0400474F RID: 18255
				public int id;
			}
		}

		// Token: 0x02000836 RID: 2102
		private class JoystickAssignmentHistoryInfo
		{
			// Token: 0x06002998 RID: 10648 RVA: 0x001FCCE6 File Offset: 0x001FAEE6
			public JoystickAssignmentHistoryInfo(Joystick joystick, int oldJoystickId)
			{
				if (joystick == null)
				{
					throw new ArgumentNullException("joystick");
				}
				this.joystick = joystick;
				this.oldJoystickId = oldJoystickId;
			}

			// Token: 0x04004750 RID: 18256
			public readonly Joystick joystick;

			// Token: 0x04004751 RID: 18257
			public readonly int oldJoystickId;
		}
	}
}
