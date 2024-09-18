using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004BC RID: 1212
public class BugController : MonoBehaviour
{
	// Token: 0x06001A42 RID: 6722 RVA: 0x00182670 File Offset: 0x00180870
	public void Setup(NewRoom newRoom)
	{
		this.room = newRoom;
		this.nodes = new List<NewNode>();
		foreach (NewNode newNode in this.room.nodes)
		{
			if (newNode.floorType == NewNode.FloorTileType.floorAndCeiling || newNode.floorType == NewNode.FloorTileType.floorOnly)
			{
				this.nodes.Add(newNode);
			}
		}
		if (this.nodes.Count > 0)
		{
			NewNode newNode2 = this.nodes[Toolbox.Instance.Rand(0, this.nodes.Count, false)];
			base.transform.position = newNode2.position + new Vector3(Toolbox.Instance.Rand(-0.7f, 0.7f, false), 0f, Toolbox.Instance.Rand(-0.7f, 0.7f, false));
			this.newJourney = true;
			return;
		}
		base.enabled = false;
	}

	// Token: 0x06001A43 RID: 6723 RVA: 0x0018277C File Offset: 0x0018097C
	private void Update()
	{
		if (!SessionData.Instance.startedGame || !SessionData.Instance.play || this.nodes.Count <= 0)
		{
			return;
		}
		if (this.newJourney)
		{
			List<NewNode> list = new List<NewNode>();
			foreach (NewNode newNode in this.nodes)
			{
				if (newNode.floorType == NewNode.FloorTileType.floorOnly || newNode.floorType == NewNode.FloorTileType.floorAndCeiling)
				{
					list.Add(newNode);
				}
			}
			if (list.Count > 0)
			{
				this.destinationNode = list[Toolbox.Instance.Rand(0, list.Count, false)];
				this.destinationPos = this.destinationNode.position + new Vector3(Toolbox.Instance.Rand(-0.7f, 0.7f, false), 0f, Toolbox.Instance.Rand(-0.7f, 0.7f, false));
				this.newJourney = false;
				return;
			}
		}
		else if (this.destinationNode != null)
		{
			Quaternion quaternion = Quaternion.identity;
			Vector3 vector = this.destinationPos - base.transform.position;
			vector.y = 0f;
			if (vector != Vector3.zero)
			{
				quaternion = Quaternion.LookRotation(vector, Vector3.up);
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.turnSpeed * Time.deltaTime);
			float num = Vector3.Distance(base.transform.position, this.destinationPos);
			if (num <= 0.05f)
			{
				this.newJourney = true;
				return;
			}
			this.speed = Mathf.Clamp(1f, 0.3f, num / 0.5f);
			base.transform.position += base.transform.forward * this.speed * Time.deltaTime;
		}
	}

	// Token: 0x040022CF RID: 8911
	public NewRoom room;

	// Token: 0x040022D0 RID: 8912
	private List<NewNode> nodes;

	// Token: 0x040022D1 RID: 8913
	public float speed = 1f;

	// Token: 0x040022D2 RID: 8914
	public float turnSpeed = 6f;

	// Token: 0x040022D3 RID: 8915
	private bool newJourney = true;

	// Token: 0x040022D4 RID: 8916
	private NewNode destinationNode;

	// Token: 0x040022D5 RID: 8917
	private Vector3 destinationPos;
}
