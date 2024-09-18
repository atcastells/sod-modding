using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class DebugPathfind : MonoBehaviour
{
	// Token: 0x06000BA6 RID: 2982 RVA: 0x000AA2D4 File Offset: 0x000A84D4
	public void Setup(NewNode.NodeAccess newAccess, NewRoom newRoom, List<DebugPathfind.DebugLocationLink> linkList)
	{
		this.access = newAccess;
		this.walkingAccess = this.access.walkingAccess;
		this.employeeDoor = this.access.employeeDoor;
		this.noPassThroughOnFromNode = this.access.fromNode.noPassThrough;
		this.noPassThroughOnToNode = this.access.toNode.noPassThrough;
		this.noAccessOnFromNode = this.access.fromNode.noAccess;
		this.noAccessOnToNode = this.access.toNode.noAccess;
		this.fromNodePos = this.access.fromNode.position;
		this.toNodePos = this.access.toNode.position;
		this.room = newRoom;
		this.gameLocation = newRoom.gameLocation;
		this.locationLinkAttempts = linkList;
		if (this.room == null)
		{
			this.room = this.access.fromNode.room;
		}
		base.gameObject.name = this.room.name + ", " + this.room.gameLocation.name;
		base.transform.position = this.access.worldAccessPoint + new Vector3(0f, 1f, 0f);
		base.transform.localScale = new Vector3(0.25f, 0.25f, 1.8f);
		Vector3 vector = this.access.toNode.position - this.access.fromNode.position;
		base.transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x000AA486 File Offset: 0x000A8686
	[Button(null, 0)]
	public void TeleportPlayer()
	{
		Player.Instance.Teleport(this.access.fromNode, null, true, false);
	}

	// Token: 0x04000CEE RID: 3310
	private NewNode.NodeAccess access;

	// Token: 0x04000CEF RID: 3311
	[Tooltip("Parent to this room")]
	public NewRoom room;

	// Token: 0x04000CF0 RID: 3312
	[Tooltip("Parent to this gamelocation")]
	public NewGameLocation gameLocation;

	// Token: 0x04000CF1 RID: 3313
	[ReadOnly]
	public Vector3 fromNodePos;

	// Token: 0x04000CF2 RID: 3314
	[ReadOnly]
	public Vector3 toNodePos;

	// Token: 0x04000CF3 RID: 3315
	[ReadOnly]
	[Header("Access Details")]
	public bool walkingAccess;

	// Token: 0x04000CF4 RID: 3316
	[ReadOnly]
	public bool employeeDoor;

	// Token: 0x04000CF5 RID: 3317
	[ReadOnly]
	public bool noPassThroughOnFromNode;

	// Token: 0x04000CF6 RID: 3318
	[ReadOnly]
	public bool noPassThroughOnToNode;

	// Token: 0x04000CF7 RID: 3319
	[ReadOnly]
	public bool noAccessOnFromNode;

	// Token: 0x04000CF8 RID: 3320
	[ReadOnly]
	public bool noAccessOnToNode;

	// Token: 0x04000CF9 RID: 3321
	[Space(7f)]
	[Tooltip("Links to other gamelocations")]
	public List<DebugPathfind.DebugLocationLink> locationLinkAttempts = new List<DebugPathfind.DebugLocationLink>();

	// Token: 0x020001FC RID: 508
	[Serializable]
	public class DebugLocationLink
	{
		// Token: 0x06000BA9 RID: 2985 RVA: 0x000AA4B4 File Offset: 0x000A86B4
		public DebugLocationLink(NewNode.NodeAccess acc, string reason)
		{
			this.access = acc;
			this.name = string.Concat(new string[]
			{
				acc.fromNode.gameLocation.name,
				" -> ",
				acc.toNode.gameLocation.name,
				" (",
				reason,
				")"
			});
		}

		// Token: 0x04000CFA RID: 3322
		public string name;

		// Token: 0x04000CFB RID: 3323
		public NewNode.NodeAccess access;
	}
}
