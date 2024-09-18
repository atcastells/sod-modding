using System;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000880 RID: 2176
	[Serializable]
	public abstract class LanguageDataBase : ScriptableObject
	{
		// Token: 0x06002DDF RID: 11743
		public abstract void Initialize();

		// Token: 0x06002DE0 RID: 11744
		public abstract string GetCustomEntry(string key);

		// Token: 0x06002DE1 RID: 11745
		public abstract bool ContainsCustomEntryKey(string key);

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06002DE2 RID: 11746
		public abstract string yes { get; }

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06002DE3 RID: 11747
		public abstract string no { get; }

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06002DE4 RID: 11748
		public abstract string add { get; }

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06002DE5 RID: 11749
		public abstract string replace { get; }

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06002DE6 RID: 11750
		public abstract string remove { get; }

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06002DE7 RID: 11751
		public abstract string swap { get; }

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06002DE8 RID: 11752
		public abstract string cancel { get; }

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06002DE9 RID: 11753
		public abstract string none { get; }

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06002DEA RID: 11754
		public abstract string okay { get; }

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06002DEB RID: 11755
		public abstract string done { get; }

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06002DEC RID: 11756
		public abstract string default_ { get; }

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06002DED RID: 11757
		public abstract string assignControllerWindowTitle { get; }

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06002DEE RID: 11758
		public abstract string assignControllerWindowMessage { get; }

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06002DEF RID: 11759
		public abstract string controllerAssignmentConflictWindowTitle { get; }

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06002DF0 RID: 11760
		public abstract string elementAssignmentPrePollingWindowMessage { get; }

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06002DF1 RID: 11761
		public abstract string elementAssignmentConflictWindowMessage { get; }

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06002DF2 RID: 11762
		public abstract string mouseAssignmentConflictWindowTitle { get; }

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06002DF3 RID: 11763
		public abstract string calibrateControllerWindowTitle { get; }

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06002DF4 RID: 11764
		public abstract string calibrateAxisStep1WindowTitle { get; }

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06002DF5 RID: 11765
		public abstract string calibrateAxisStep2WindowTitle { get; }

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06002DF6 RID: 11766
		public abstract string inputBehaviorSettingsWindowTitle { get; }

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06002DF7 RID: 11767
		public abstract string restoreDefaultsWindowTitle { get; }

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06002DF8 RID: 11768
		public abstract string actionColumnLabel { get; }

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06002DF9 RID: 11769
		public abstract string keyboardColumnLabel { get; }

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06002DFA RID: 11770
		public abstract string mouseColumnLabel { get; }

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06002DFB RID: 11771
		public abstract string controllerColumnLabel { get; }

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06002DFC RID: 11772
		public abstract string removeControllerButtonLabel { get; }

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06002DFD RID: 11773
		public abstract string calibrateControllerButtonLabel { get; }

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06002DFE RID: 11774
		public abstract string assignControllerButtonLabel { get; }

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06002DFF RID: 11775
		public abstract string inputBehaviorSettingsButtonLabel { get; }

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06002E00 RID: 11776
		public abstract string doneButtonLabel { get; }

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06002E01 RID: 11777
		public abstract string restoreDefaultsButtonLabel { get; }

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06002E02 RID: 11778
		public abstract string controllerSettingsGroupLabel { get; }

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06002E03 RID: 11779
		public abstract string playersGroupLabel { get; }

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06002E04 RID: 11780
		public abstract string assignedControllersGroupLabel { get; }

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06002E05 RID: 11781
		public abstract string settingsGroupLabel { get; }

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06002E06 RID: 11782
		public abstract string mapCategoriesGroupLabel { get; }

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06002E07 RID: 11783
		public abstract string restoreDefaultsWindowMessage { get; }

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06002E08 RID: 11784
		public abstract string calibrateWindow_deadZoneSliderLabel { get; }

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06002E09 RID: 11785
		public abstract string calibrateWindow_zeroSliderLabel { get; }

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06002E0A RID: 11786
		public abstract string calibrateWindow_sensitivitySliderLabel { get; }

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06002E0B RID: 11787
		public abstract string calibrateWindow_invertToggleLabel { get; }

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06002E0C RID: 11788
		public abstract string calibrateWindow_calibrateButtonLabel { get; }

		// Token: 0x06002E0D RID: 11789
		public abstract string GetControllerAssignmentConflictWindowMessage(string joystickName, string otherPlayerName, string currentPlayerName);

		// Token: 0x06002E0E RID: 11790
		public abstract string GetJoystickElementAssignmentPollingWindowMessage(string actionName);

		// Token: 0x06002E0F RID: 11791
		public abstract string GetJoystickElementAssignmentPollingWindowMessage_FullAxisFieldOnly(string actionName);

		// Token: 0x06002E10 RID: 11792
		public abstract string GetKeyboardElementAssignmentPollingWindowMessage(string actionName);

		// Token: 0x06002E11 RID: 11793
		public abstract string GetMouseElementAssignmentPollingWindowMessage(string actionName);

		// Token: 0x06002E12 RID: 11794
		public abstract string GetMouseElementAssignmentPollingWindowMessage_FullAxisFieldOnly(string actionName);

		// Token: 0x06002E13 RID: 11795
		public abstract string GetElementAlreadyInUseBlocked(string elementName);

		// Token: 0x06002E14 RID: 11796
		public abstract string GetElementAlreadyInUseCanReplace(string elementName, bool allowConflicts);

		// Token: 0x06002E15 RID: 11797
		public abstract string GetMouseAssignmentConflictWindowMessage(string otherPlayerName, string thisPlayerName);

		// Token: 0x06002E16 RID: 11798
		public abstract string GetCalibrateAxisStep1WindowMessage(string axisName);

		// Token: 0x06002E17 RID: 11799
		public abstract string GetCalibrateAxisStep2WindowMessage(string axisName);

		// Token: 0x06002E18 RID: 11800
		public abstract string GetPlayerName(int playerId);

		// Token: 0x06002E19 RID: 11801
		public abstract string GetControllerName(Controller controller);

		// Token: 0x06002E1A RID: 11802
		public abstract string GetElementIdentifierName(ActionElementMap actionElementMap);

		// Token: 0x06002E1B RID: 11803
		public abstract string GetElementIdentifierName(Controller controller, int elementIdentifierId, AxisRange axisRange);

		// Token: 0x06002E1C RID: 11804
		public abstract string GetElementIdentifierName(KeyCode keyCode, ModifierKeyFlags modifierKeyFlags);

		// Token: 0x06002E1D RID: 11805
		public abstract string GetActionName(int actionId);

		// Token: 0x06002E1E RID: 11806
		public abstract string GetActionName(int actionId, AxisRange axisRange);

		// Token: 0x06002E1F RID: 11807
		public abstract string GetMapCategoryName(int id);

		// Token: 0x06002E20 RID: 11808
		public abstract string GetActionCategoryName(int id);

		// Token: 0x06002E21 RID: 11809
		public abstract string GetLayoutName(ControllerType controllerType, int id);

		// Token: 0x06002E22 RID: 11810
		public abstract string ModifierKeyFlagsToString(ModifierKeyFlags flags);
	}
}
