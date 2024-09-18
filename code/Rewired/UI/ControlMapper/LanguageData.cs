using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x0200087D RID: 2173
	[Serializable]
	public class LanguageData : LanguageDataBase
	{
		// Token: 0x06002D96 RID: 11670 RVA: 0x0020B1CA File Offset: 0x002093CA
		public override void Initialize()
		{
			if (this._initialized)
			{
				return;
			}
			this.customDict = LanguageData.CustomEntry.ToDictionary(this._customEntries);
			this._initialized = true;
		}

		// Token: 0x06002D97 RID: 11671 RVA: 0x0020B1F0 File Offset: 0x002093F0
		public override string GetCustomEntry(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return string.Empty;
			}
			string result;
			if (!this.customDict.TryGetValue(key, ref result))
			{
				return string.Empty;
			}
			return result;
		}

		// Token: 0x06002D98 RID: 11672 RVA: 0x0020B222 File Offset: 0x00209422
		public override bool ContainsCustomEntryKey(string key)
		{
			return !string.IsNullOrEmpty(key) && this.customDict.ContainsKey(key);
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06002D99 RID: 11673 RVA: 0x0020B23A File Offset: 0x0020943A
		public override string yes
		{
			get
			{
				return this._yes;
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06002D9A RID: 11674 RVA: 0x0020B242 File Offset: 0x00209442
		public override string no
		{
			get
			{
				return this._no;
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06002D9B RID: 11675 RVA: 0x0020B24A File Offset: 0x0020944A
		public override string add
		{
			get
			{
				return this._add;
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06002D9C RID: 11676 RVA: 0x0020B252 File Offset: 0x00209452
		public override string replace
		{
			get
			{
				return this._replace;
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06002D9D RID: 11677 RVA: 0x0020B25A File Offset: 0x0020945A
		public override string remove
		{
			get
			{
				return this._remove;
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06002D9E RID: 11678 RVA: 0x0020B262 File Offset: 0x00209462
		public override string swap
		{
			get
			{
				return this._swap;
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06002D9F RID: 11679 RVA: 0x0020B26A File Offset: 0x0020946A
		public override string cancel
		{
			get
			{
				return this._cancel;
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06002DA0 RID: 11680 RVA: 0x0020B272 File Offset: 0x00209472
		public override string none
		{
			get
			{
				return this._none;
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06002DA1 RID: 11681 RVA: 0x0020B27A File Offset: 0x0020947A
		public override string okay
		{
			get
			{
				return this._okay;
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06002DA2 RID: 11682 RVA: 0x0020B282 File Offset: 0x00209482
		public override string done
		{
			get
			{
				return this._done;
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06002DA3 RID: 11683 RVA: 0x0020B28A File Offset: 0x0020948A
		public override string default_
		{
			get
			{
				return this._default;
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06002DA4 RID: 11684 RVA: 0x0020B292 File Offset: 0x00209492
		public override string assignControllerWindowTitle
		{
			get
			{
				return this._assignControllerWindowTitle;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06002DA5 RID: 11685 RVA: 0x0020B29A File Offset: 0x0020949A
		public override string assignControllerWindowMessage
		{
			get
			{
				return this._assignControllerWindowMessage;
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06002DA6 RID: 11686 RVA: 0x0020B2A2 File Offset: 0x002094A2
		public override string controllerAssignmentConflictWindowTitle
		{
			get
			{
				return this._controllerAssignmentConflictWindowTitle;
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06002DA7 RID: 11687 RVA: 0x0020B2AA File Offset: 0x002094AA
		public override string elementAssignmentPrePollingWindowMessage
		{
			get
			{
				return this._elementAssignmentPrePollingWindowMessage;
			}
		}

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06002DA8 RID: 11688 RVA: 0x0020B2B2 File Offset: 0x002094B2
		public override string elementAssignmentConflictWindowMessage
		{
			get
			{
				return this._elementAssignmentConflictWindowMessage;
			}
		}

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06002DA9 RID: 11689 RVA: 0x0020B2BA File Offset: 0x002094BA
		public override string mouseAssignmentConflictWindowTitle
		{
			get
			{
				return this._mouseAssignmentConflictWindowTitle;
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06002DAA RID: 11690 RVA: 0x0020B2C2 File Offset: 0x002094C2
		public override string calibrateControllerWindowTitle
		{
			get
			{
				return this._calibrateControllerWindowTitle;
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06002DAB RID: 11691 RVA: 0x0020B2CA File Offset: 0x002094CA
		public override string calibrateAxisStep1WindowTitle
		{
			get
			{
				return this._calibrateAxisStep1WindowTitle;
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06002DAC RID: 11692 RVA: 0x0020B2D2 File Offset: 0x002094D2
		public override string calibrateAxisStep2WindowTitle
		{
			get
			{
				return this._calibrateAxisStep2WindowTitle;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06002DAD RID: 11693 RVA: 0x0020B2DA File Offset: 0x002094DA
		public override string inputBehaviorSettingsWindowTitle
		{
			get
			{
				return this._inputBehaviorSettingsWindowTitle;
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06002DAE RID: 11694 RVA: 0x0020B2E2 File Offset: 0x002094E2
		public override string restoreDefaultsWindowTitle
		{
			get
			{
				return this._restoreDefaultsWindowTitle;
			}
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06002DAF RID: 11695 RVA: 0x0020B2EA File Offset: 0x002094EA
		public override string actionColumnLabel
		{
			get
			{
				return this._actionColumnLabel;
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06002DB0 RID: 11696 RVA: 0x0020B2F2 File Offset: 0x002094F2
		public override string keyboardColumnLabel
		{
			get
			{
				return this._keyboardColumnLabel;
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06002DB1 RID: 11697 RVA: 0x0020B2FA File Offset: 0x002094FA
		public override string mouseColumnLabel
		{
			get
			{
				return this._mouseColumnLabel;
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06002DB2 RID: 11698 RVA: 0x0020B302 File Offset: 0x00209502
		public override string controllerColumnLabel
		{
			get
			{
				return this._controllerColumnLabel;
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06002DB3 RID: 11699 RVA: 0x0020B30A File Offset: 0x0020950A
		public override string removeControllerButtonLabel
		{
			get
			{
				return this._removeControllerButtonLabel;
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06002DB4 RID: 11700 RVA: 0x0020B312 File Offset: 0x00209512
		public override string calibrateControllerButtonLabel
		{
			get
			{
				return this._calibrateControllerButtonLabel;
			}
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06002DB5 RID: 11701 RVA: 0x0020B31A File Offset: 0x0020951A
		public override string assignControllerButtonLabel
		{
			get
			{
				return this._assignControllerButtonLabel;
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06002DB6 RID: 11702 RVA: 0x0020B322 File Offset: 0x00209522
		public override string inputBehaviorSettingsButtonLabel
		{
			get
			{
				return this._inputBehaviorSettingsButtonLabel;
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06002DB7 RID: 11703 RVA: 0x0020B32A File Offset: 0x0020952A
		public override string doneButtonLabel
		{
			get
			{
				return this._doneButtonLabel;
			}
		}

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06002DB8 RID: 11704 RVA: 0x0020B332 File Offset: 0x00209532
		public override string restoreDefaultsButtonLabel
		{
			get
			{
				return this._restoreDefaultsButtonLabel;
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06002DB9 RID: 11705 RVA: 0x0020B33A File Offset: 0x0020953A
		public override string controllerSettingsGroupLabel
		{
			get
			{
				return this._controllerSettingsGroupLabel;
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06002DBA RID: 11706 RVA: 0x0020B342 File Offset: 0x00209542
		public override string playersGroupLabel
		{
			get
			{
				return this._playersGroupLabel;
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06002DBB RID: 11707 RVA: 0x0020B34A File Offset: 0x0020954A
		public override string assignedControllersGroupLabel
		{
			get
			{
				return this._assignedControllersGroupLabel;
			}
		}

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06002DBC RID: 11708 RVA: 0x0020B352 File Offset: 0x00209552
		public override string settingsGroupLabel
		{
			get
			{
				return this._settingsGroupLabel;
			}
		}

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06002DBD RID: 11709 RVA: 0x0020B35A File Offset: 0x0020955A
		public override string mapCategoriesGroupLabel
		{
			get
			{
				return this._mapCategoriesGroupLabel;
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06002DBE RID: 11710 RVA: 0x0020B362 File Offset: 0x00209562
		public override string restoreDefaultsWindowMessage
		{
			get
			{
				if (ReInput.players.playerCount > 1)
				{
					return this._restoreDefaultsWindowMessage_multiPlayer;
				}
				return this._restoreDefaultsWindowMessage_onePlayer;
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06002DBF RID: 11711 RVA: 0x0020B37E File Offset: 0x0020957E
		public override string calibrateWindow_deadZoneSliderLabel
		{
			get
			{
				return this._calibrateWindow_deadZoneSliderLabel;
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06002DC0 RID: 11712 RVA: 0x0020B386 File Offset: 0x00209586
		public override string calibrateWindow_zeroSliderLabel
		{
			get
			{
				return this._calibrateWindow_zeroSliderLabel;
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06002DC1 RID: 11713 RVA: 0x0020B38E File Offset: 0x0020958E
		public override string calibrateWindow_sensitivitySliderLabel
		{
			get
			{
				return this._calibrateWindow_sensitivitySliderLabel;
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06002DC2 RID: 11714 RVA: 0x0020B396 File Offset: 0x00209596
		public override string calibrateWindow_invertToggleLabel
		{
			get
			{
				return this._calibrateWindow_invertToggleLabel;
			}
		}

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06002DC3 RID: 11715 RVA: 0x0020B39E File Offset: 0x0020959E
		public override string calibrateWindow_calibrateButtonLabel
		{
			get
			{
				return this._calibrateWindow_calibrateButtonLabel;
			}
		}

		// Token: 0x06002DC4 RID: 11716 RVA: 0x0020B3A6 File Offset: 0x002095A6
		public override string GetControllerAssignmentConflictWindowMessage(string joystickName, string otherPlayerName, string currentPlayerName)
		{
			return string.Format(this._controllerAssignmentConflictWindowMessage, joystickName, otherPlayerName, currentPlayerName);
		}

		// Token: 0x06002DC5 RID: 11717 RVA: 0x0020B3B6 File Offset: 0x002095B6
		public override string GetJoystickElementAssignmentPollingWindowMessage(string actionName)
		{
			return string.Format(this._joystickElementAssignmentPollingWindowMessage, actionName);
		}

		// Token: 0x06002DC6 RID: 11718 RVA: 0x0020B3C4 File Offset: 0x002095C4
		public override string GetJoystickElementAssignmentPollingWindowMessage_FullAxisFieldOnly(string actionName)
		{
			return string.Format(this._joystickElementAssignmentPollingWindowMessage_fullAxisFieldOnly, actionName);
		}

		// Token: 0x06002DC7 RID: 11719 RVA: 0x0020B3D2 File Offset: 0x002095D2
		public override string GetKeyboardElementAssignmentPollingWindowMessage(string actionName)
		{
			return string.Format(this._keyboardElementAssignmentPollingWindowMessage, actionName);
		}

		// Token: 0x06002DC8 RID: 11720 RVA: 0x0020B3E0 File Offset: 0x002095E0
		public override string GetMouseElementAssignmentPollingWindowMessage(string actionName)
		{
			return string.Format(this._mouseElementAssignmentPollingWindowMessage, actionName);
		}

		// Token: 0x06002DC9 RID: 11721 RVA: 0x0020B3EE File Offset: 0x002095EE
		public override string GetMouseElementAssignmentPollingWindowMessage_FullAxisFieldOnly(string actionName)
		{
			return string.Format(this._mouseElementAssignmentPollingWindowMessage_fullAxisFieldOnly, actionName);
		}

		// Token: 0x06002DCA RID: 11722 RVA: 0x0020B3FC File Offset: 0x002095FC
		public override string GetElementAlreadyInUseBlocked(string elementName)
		{
			return string.Format(this._elementAlreadyInUseBlocked, elementName);
		}

		// Token: 0x06002DCB RID: 11723 RVA: 0x0020B40A File Offset: 0x0020960A
		public override string GetElementAlreadyInUseCanReplace(string elementName, bool allowConflicts)
		{
			if (!allowConflicts)
			{
				return string.Format(this._elementAlreadyInUseCanReplace, elementName);
			}
			return string.Format(this._elementAlreadyInUseCanReplace_conflictAllowed, elementName);
		}

		// Token: 0x06002DCC RID: 11724 RVA: 0x0020B428 File Offset: 0x00209628
		public override string GetMouseAssignmentConflictWindowMessage(string otherPlayerName, string thisPlayerName)
		{
			return string.Format(this._mouseAssignmentConflictWindowMessage, otherPlayerName, thisPlayerName);
		}

		// Token: 0x06002DCD RID: 11725 RVA: 0x0020B437 File Offset: 0x00209637
		public override string GetCalibrateAxisStep1WindowMessage(string axisName)
		{
			return string.Format(this._calibrateAxisStep1WindowMessage, axisName);
		}

		// Token: 0x06002DCE RID: 11726 RVA: 0x0020B445 File Offset: 0x00209645
		public override string GetCalibrateAxisStep2WindowMessage(string axisName)
		{
			return string.Format(this._calibrateAxisStep2WindowMessage, axisName);
		}

		// Token: 0x06002DCF RID: 11727 RVA: 0x0020B453 File Offset: 0x00209653
		public override string GetPlayerName(int playerId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				throw new ArgumentException("Invalid player id: " + playerId.ToString());
			}
			return player.descriptiveName;
		}

		// Token: 0x06002DD0 RID: 11728 RVA: 0x0020B47F File Offset: 0x0020967F
		public override string GetControllerName(Controller controller)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			return controller.name;
		}

		// Token: 0x06002DD1 RID: 11729 RVA: 0x0020B498 File Offset: 0x00209698
		public override string GetElementIdentifierName(ActionElementMap actionElementMap)
		{
			if (actionElementMap == null)
			{
				throw new ArgumentNullException("actionElementMap");
			}
			if (actionElementMap.controllerMap.controllerType == null)
			{
				return this.GetElementIdentifierName(actionElementMap.keyCode, actionElementMap.modifierKeyFlags);
			}
			return this.GetElementIdentifierName(actionElementMap.controllerMap.controller, actionElementMap.elementIdentifierId, actionElementMap.axisRange);
		}

		// Token: 0x06002DD2 RID: 11730 RVA: 0x0020B4F0 File Offset: 0x002096F0
		public override string GetElementIdentifierName(Controller controller, int elementIdentifierId, AxisRange axisRange)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			ControllerElementIdentifier elementIdentifierById = controller.GetElementIdentifierById(elementIdentifierId);
			if (elementIdentifierById == null)
			{
				throw new ArgumentException("Invalid element identifier id: " + elementIdentifierId.ToString());
			}
			Controller.Element elementById = controller.GetElementById(elementIdentifierId);
			if (elementById == null)
			{
				return string.Empty;
			}
			ControllerElementType type = elementById.type;
			if (type == null)
			{
				return elementIdentifierById.GetDisplayName(elementById.type, axisRange);
			}
			if (type != 1)
			{
				return elementIdentifierById.name;
			}
			return elementIdentifierById.name;
		}

		// Token: 0x06002DD3 RID: 11731 RVA: 0x0020B569 File Offset: 0x00209769
		public override string GetElementIdentifierName(KeyCode keyCode, ModifierKeyFlags modifierKeyFlags)
		{
			if (modifierKeyFlags != null)
			{
				return string.Format("{0}{1}{2}", this.ModifierKeyFlagsToString(modifierKeyFlags), this._modifierKeys.separator, Keyboard.GetKeyName(keyCode));
			}
			return Keyboard.GetKeyName(keyCode);
		}

		// Token: 0x06002DD4 RID: 11732 RVA: 0x0020B597 File Offset: 0x00209797
		public override string GetActionName(int actionId)
		{
			InputAction action = ReInput.mapping.GetAction(actionId);
			if (action == null)
			{
				throw new ArgumentException("Invalid action id: " + actionId.ToString());
			}
			return action.descriptiveName;
		}

		// Token: 0x06002DD5 RID: 11733 RVA: 0x0020B5C4 File Offset: 0x002097C4
		public override string GetActionName(int actionId, AxisRange axisRange)
		{
			InputAction action = ReInput.mapping.GetAction(actionId);
			if (action == null)
			{
				throw new ArgumentException("Invalid action id: " + actionId.ToString());
			}
			switch (axisRange)
			{
			case 0:
				return action.descriptiveName;
			case 1:
				if (string.IsNullOrEmpty(action.positiveDescriptiveName))
				{
					return action.descriptiveName + " +";
				}
				return action.positiveDescriptiveName;
			case 2:
				if (string.IsNullOrEmpty(action.negativeDescriptiveName))
				{
					return action.descriptiveName + " -";
				}
				return action.negativeDescriptiveName;
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x06002DD6 RID: 11734 RVA: 0x0020B661 File Offset: 0x00209861
		public override string GetMapCategoryName(int id)
		{
			InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(id);
			if (mapCategory == null)
			{
				throw new ArgumentException("Invalid map category id: " + id.ToString());
			}
			return mapCategory.descriptiveName;
		}

		// Token: 0x06002DD7 RID: 11735 RVA: 0x0020B68D File Offset: 0x0020988D
		public override string GetActionCategoryName(int id)
		{
			InputCategory actionCategory = ReInput.mapping.GetActionCategory(id);
			if (actionCategory == null)
			{
				throw new ArgumentException("Invalid action category id: " + id.ToString());
			}
			return actionCategory.descriptiveName;
		}

		// Token: 0x06002DD8 RID: 11736 RVA: 0x0020B6B9 File Offset: 0x002098B9
		public override string GetLayoutName(ControllerType controllerType, int id)
		{
			InputLayout layout = ReInput.mapping.GetLayout(controllerType, id);
			if (layout == null)
			{
				throw new ArgumentException("Invalid " + controllerType.ToString() + " layout id: " + id.ToString());
			}
			return layout.descriptiveName;
		}

		// Token: 0x06002DD9 RID: 11737 RVA: 0x0020B6F8 File Offset: 0x002098F8
		public override string ModifierKeyFlagsToString(ModifierKeyFlags flags)
		{
			int num = 0;
			string text = string.Empty;
			if (Keyboard.ModifierKeyFlagsContain(flags, 1))
			{
				text += this._modifierKeys.control;
				num++;
			}
			if (Keyboard.ModifierKeyFlagsContain(flags, 4))
			{
				if (num > 0 && !string.IsNullOrEmpty(this._modifierKeys.separator))
				{
					text += this._modifierKeys.separator;
				}
				text += this._modifierKeys.command;
				num++;
			}
			if (Keyboard.ModifierKeyFlagsContain(flags, 2))
			{
				if (num > 0 && !string.IsNullOrEmpty(this._modifierKeys.separator))
				{
					text += this._modifierKeys.separator;
				}
				text += this._modifierKeys.alt;
				num++;
			}
			if (num >= 3)
			{
				return text;
			}
			if (Keyboard.ModifierKeyFlagsContain(flags, 3))
			{
				if (num > 0 && !string.IsNullOrEmpty(this._modifierKeys.separator))
				{
					text += this._modifierKeys.separator;
				}
				text += this._modifierKeys.shift;
				num++;
			}
			return text;
		}

		// Token: 0x04004933 RID: 18739
		[SerializeField]
		private string _yes = "Yes";

		// Token: 0x04004934 RID: 18740
		[SerializeField]
		private string _no = "No";

		// Token: 0x04004935 RID: 18741
		[SerializeField]
		private string _add = "Add";

		// Token: 0x04004936 RID: 18742
		[SerializeField]
		private string _replace = "Replace";

		// Token: 0x04004937 RID: 18743
		[SerializeField]
		private string _remove = "Remove";

		// Token: 0x04004938 RID: 18744
		[SerializeField]
		private string _swap = "Swap";

		// Token: 0x04004939 RID: 18745
		[SerializeField]
		private string _cancel = "Cancel";

		// Token: 0x0400493A RID: 18746
		[SerializeField]
		private string _none = "None";

		// Token: 0x0400493B RID: 18747
		[SerializeField]
		private string _okay = "Okay";

		// Token: 0x0400493C RID: 18748
		[SerializeField]
		private string _done = "Done";

		// Token: 0x0400493D RID: 18749
		[SerializeField]
		private string _default = "Default";

		// Token: 0x0400493E RID: 18750
		[SerializeField]
		private string _assignControllerWindowTitle = "Choose Controller";

		// Token: 0x0400493F RID: 18751
		[SerializeField]
		private string _assignControllerWindowMessage = "Press any button or move an axis on the controller you would like to use.";

		// Token: 0x04004940 RID: 18752
		[SerializeField]
		private string _controllerAssignmentConflictWindowTitle = "Controller Assignment";

		// Token: 0x04004941 RID: 18753
		[SerializeField]
		[Tooltip("{0} = Joystick Name\n{1} = Other Player Name\n{2} = This Player Name")]
		private string _controllerAssignmentConflictWindowMessage = "{0} is already assigned to {1}. Do you want to assign this controller to {2} instead?";

		// Token: 0x04004942 RID: 18754
		[SerializeField]
		private string _elementAssignmentPrePollingWindowMessage = "First center or zero all sticks and axes and press any button or wait for the timer to finish.";

		// Token: 0x04004943 RID: 18755
		[SerializeField]
		[Tooltip("{0} = Action Name")]
		private string _joystickElementAssignmentPollingWindowMessage = "Now press a button or move an axis to assign it to {0}.";

		// Token: 0x04004944 RID: 18756
		[SerializeField]
		[Tooltip("This text is only displayed when split-axis fields have been disabled and the user clicks on the full-axis field. Button/key/D-pad input cannot be assigned to a full-axis field.\n{0} = Action Name")]
		private string _joystickElementAssignmentPollingWindowMessage_fullAxisFieldOnly = "Now move an axis to assign it to {0}.";

		// Token: 0x04004945 RID: 18757
		[SerializeField]
		[Tooltip("{0} = Action Name")]
		private string _keyboardElementAssignmentPollingWindowMessage = "Press a key to assign it to {0}. Modifier keys may also be used. To assign a modifier key alone, hold it down for 1 second.";

		// Token: 0x04004946 RID: 18758
		[SerializeField]
		[Tooltip("{0} = Action Name")]
		private string _mouseElementAssignmentPollingWindowMessage = "Press a mouse button or move an axis to assign it to {0}.";

		// Token: 0x04004947 RID: 18759
		[SerializeField]
		[Tooltip("This text is only displayed when split-axis fields have been disabled and the user clicks on the full-axis field. Button/key/D-pad input cannot be assigned to a full-axis field.\n{0} = Action Name")]
		private string _mouseElementAssignmentPollingWindowMessage_fullAxisFieldOnly = "Move an axis to assign it to {0}.";

		// Token: 0x04004948 RID: 18760
		[SerializeField]
		private string _elementAssignmentConflictWindowMessage = "Assignment Conflict";

		// Token: 0x04004949 RID: 18761
		[SerializeField]
		[Tooltip("{0} = Element Name")]
		private string _elementAlreadyInUseBlocked = "{0} is already in use cannot be replaced.";

		// Token: 0x0400494A RID: 18762
		[SerializeField]
		[Tooltip("{0} = Element Name")]
		private string _elementAlreadyInUseCanReplace = "{0} is already in use. Do you want to replace it?";

		// Token: 0x0400494B RID: 18763
		[SerializeField]
		[Tooltip("{0} = Element Name")]
		private string _elementAlreadyInUseCanReplace_conflictAllowed = "{0} is already in use. Do you want to replace it? You may also choose to add the assignment anyway.";

		// Token: 0x0400494C RID: 18764
		[SerializeField]
		private string _mouseAssignmentConflictWindowTitle = "Mouse Assignment";

		// Token: 0x0400494D RID: 18765
		[SerializeField]
		[Tooltip("{0} = Other Player Name\n{1} = This Player Name")]
		private string _mouseAssignmentConflictWindowMessage = "The mouse is already assigned to {0}. Do you want to assign the mouse to {1} instead?";

		// Token: 0x0400494E RID: 18766
		[SerializeField]
		private string _calibrateControllerWindowTitle = "Calibrate Controller";

		// Token: 0x0400494F RID: 18767
		[SerializeField]
		private string _calibrateAxisStep1WindowTitle = "Calibrate Zero";

		// Token: 0x04004950 RID: 18768
		[SerializeField]
		[Tooltip("{0} = Axis Name")]
		private string _calibrateAxisStep1WindowMessage = "Center or zero {0} and press any button or wait for the timer to finish.";

		// Token: 0x04004951 RID: 18769
		[SerializeField]
		private string _calibrateAxisStep2WindowTitle = "Calibrate Range";

		// Token: 0x04004952 RID: 18770
		[Tooltip("{0} = Axis Name")]
		[SerializeField]
		private string _calibrateAxisStep2WindowMessage = "Move {0} through its entire range then press any button or wait for the timer to finish.";

		// Token: 0x04004953 RID: 18771
		[SerializeField]
		private string _inputBehaviorSettingsWindowTitle = "Sensitivity Settings";

		// Token: 0x04004954 RID: 18772
		[SerializeField]
		private string _restoreDefaultsWindowTitle = "Restore Defaults";

		// Token: 0x04004955 RID: 18773
		[SerializeField]
		[Tooltip("Message for a single player game.")]
		private string _restoreDefaultsWindowMessage_onePlayer = "This will restore the default input configuration. Are you sure you want to do this?";

		// Token: 0x04004956 RID: 18774
		[SerializeField]
		[Tooltip("Message for a multi-player game.")]
		private string _restoreDefaultsWindowMessage_multiPlayer = "This will restore the default input configuration for all players. Are you sure you want to do this?";

		// Token: 0x04004957 RID: 18775
		[SerializeField]
		private string _actionColumnLabel = "Actions";

		// Token: 0x04004958 RID: 18776
		[SerializeField]
		private string _keyboardColumnLabel = "Keyboard";

		// Token: 0x04004959 RID: 18777
		[SerializeField]
		private string _mouseColumnLabel = "Mouse";

		// Token: 0x0400495A RID: 18778
		[SerializeField]
		private string _controllerColumnLabel = "Controller";

		// Token: 0x0400495B RID: 18779
		[SerializeField]
		private string _removeControllerButtonLabel = "Remove";

		// Token: 0x0400495C RID: 18780
		[SerializeField]
		private string _calibrateControllerButtonLabel = "Calibrate";

		// Token: 0x0400495D RID: 18781
		[SerializeField]
		private string _assignControllerButtonLabel = "Assign Controller";

		// Token: 0x0400495E RID: 18782
		[SerializeField]
		private string _inputBehaviorSettingsButtonLabel = "Sensitivity";

		// Token: 0x0400495F RID: 18783
		[SerializeField]
		private string _doneButtonLabel = "Done";

		// Token: 0x04004960 RID: 18784
		[SerializeField]
		private string _restoreDefaultsButtonLabel = "Restore Defaults";

		// Token: 0x04004961 RID: 18785
		[SerializeField]
		private string _playersGroupLabel = "Players:";

		// Token: 0x04004962 RID: 18786
		[SerializeField]
		private string _controllerSettingsGroupLabel = "Controller:";

		// Token: 0x04004963 RID: 18787
		[SerializeField]
		private string _assignedControllersGroupLabel = "Assigned Controllers:";

		// Token: 0x04004964 RID: 18788
		[SerializeField]
		private string _settingsGroupLabel = "Settings:";

		// Token: 0x04004965 RID: 18789
		[SerializeField]
		private string _mapCategoriesGroupLabel = "Categories:";

		// Token: 0x04004966 RID: 18790
		[SerializeField]
		private string _calibrateWindow_deadZoneSliderLabel = "Dead Zone:";

		// Token: 0x04004967 RID: 18791
		[SerializeField]
		private string _calibrateWindow_zeroSliderLabel = "Zero:";

		// Token: 0x04004968 RID: 18792
		[SerializeField]
		private string _calibrateWindow_sensitivitySliderLabel = "Sensitivity:";

		// Token: 0x04004969 RID: 18793
		[SerializeField]
		private string _calibrateWindow_invertToggleLabel = "Invert";

		// Token: 0x0400496A RID: 18794
		[SerializeField]
		private string _calibrateWindow_calibrateButtonLabel = "Calibrate";

		// Token: 0x0400496B RID: 18795
		[SerializeField]
		private LanguageData.ModifierKeys _modifierKeys;

		// Token: 0x0400496C RID: 18796
		[SerializeField]
		private LanguageData.CustomEntry[] _customEntries;

		// Token: 0x0400496D RID: 18797
		private bool _initialized;

		// Token: 0x0400496E RID: 18798
		private Dictionary<string, string> customDict;

		// Token: 0x0200087E RID: 2174
		[Serializable]
		protected class CustomEntry
		{
			// Token: 0x06002DDB RID: 11739 RVA: 0x00003EBD File Offset: 0x000020BD
			public CustomEntry()
			{
			}

			// Token: 0x06002DDC RID: 11740 RVA: 0x0020BA83 File Offset: 0x00209C83
			public CustomEntry(string key, string value)
			{
				this.key = key;
				this.value = value;
			}

			// Token: 0x06002DDD RID: 11741 RVA: 0x0020BA9C File Offset: 0x00209C9C
			public static Dictionary<string, string> ToDictionary(LanguageData.CustomEntry[] array)
			{
				if (array == null)
				{
					return new Dictionary<string, string>();
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null && !string.IsNullOrEmpty(array[i].key) && !string.IsNullOrEmpty(array[i].value))
					{
						if (dictionary.ContainsKey(array[i].key))
						{
							Debug.LogError("Key \"" + array[i].key + "\" is already in dictionary!");
						}
						else
						{
							dictionary.Add(array[i].key, array[i].value);
						}
					}
				}
				return dictionary;
			}

			// Token: 0x0400496F RID: 18799
			public string key;

			// Token: 0x04004970 RID: 18800
			public string value;
		}

		// Token: 0x0200087F RID: 2175
		[Serializable]
		protected class ModifierKeys
		{
			// Token: 0x04004971 RID: 18801
			public string control = "Control";

			// Token: 0x04004972 RID: 18802
			public string alt = "Alt";

			// Token: 0x04004973 RID: 18803
			public string shift = "Shift";

			// Token: 0x04004974 RID: 18804
			public string command = "Command";

			// Token: 0x04004975 RID: 18805
			public string separator = " + ";
		}
	}
}
