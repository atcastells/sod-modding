using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020004BA RID: 1210
public class BreakableWindowController : MonoBehaviour
{
	// Token: 0x06001A30 RID: 6704 RVA: 0x0018174C File Offset: 0x0017F94C
	public void InteractableCollision(Collision collision, float damage, Actor brokenBy, Interactable itemThrown)
	{
		if (this.isBroken)
		{
			return;
		}
		Game.Log(string.Concat(new string[]
		{
			"Object: ",
			base.name,
			" Collides with window ",
			base.name,
			" with force: ",
			damage.ToString()
		}), 2);
		if (damage >= this.breakForce)
		{
			this.BreakWindow(collision.GetContact(0).point, collision.relativeVelocity, brokenBy, false);
			if (brokenBy != null && brokenBy.isPlayer)
			{
				InteractionController.Instance.SetIllegalActionActive(true);
			}
			if (AchievementsController.Instance != null && brokenBy != null && brokenBy.isPlayer && itemThrown != null && itemThrown.val >= 200f && Player.Instance.currentBuilding != null && Player.Instance.currentBuilding.preset.presetName == "Hotel")
			{
				AchievementsController.Instance.UnlockAchievement("Diamonds Aren’t Forever", "throw_expensive_item_through_hotel_window");
			}
		}
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x00181864 File Offset: 0x0017FA64
	public Vector3 GetAveragePosition()
	{
		Vector3 vector = Vector3.zero;
		foreach (BreakableWindowController.WindowBreakSetting windowBreakSetting in this.panes)
		{
			if (windowBreakSetting != null && windowBreakSetting.collider != null)
			{
				Bounds bounds = windowBreakSetting.collider.bounds;
				vector += windowBreakSetting.collider.bounds.center;
			}
		}
		vector /= (float)this.panes.Count;
		return vector;
	}

	// Token: 0x06001A32 RID: 6706 RVA: 0x00181904 File Offset: 0x0017FB04
	public void BreakWindow(Vector3 contactPosition, Vector3 relativeVelocity, Actor brokenBy, bool noDebris = false)
	{
		this.SetBroken(true);
		string text = "Shatter glass at ";
		Vector3 vector = contactPosition;
		Game.Log(text + vector.ToString(), 2);
		if (this.breakAudioEvent != null)
		{
			AudioController.Instance.PlayWorldOneShot(this.breakAudioEvent, brokenBy, null, contactPosition, null, null, 1f, null, false, null, false);
		}
		if (!noDebris)
		{
			this.Shatter(contactPosition, relativeVelocity);
		}
		if (brokenBy != null && brokenBy.isPlayer)
		{
			NewAddress address = this.GetAddress();
			if (address != null)
			{
				Vector3 roughPosition = this.GetRoughPosition();
				address.AddVandalism(roughPosition);
				StatusController.Instance.AddFineRecord(address, null, StatusController.CrimeType.vandalism, true, GameplayControls.Instance.breakingWindowsFine, false);
			}
			InteractionController.Instance.SetIllegalActionActive(true);
		}
		for (int i = 0; i < this.bulletHoles.Count; i++)
		{
			if (this.bulletHoles[i] != null)
			{
				this.bulletHoles[i].SafeDelete(false);
			}
		}
		this.bulletHoles.Clear();
		NewWall wall = this.GetWall();
		if (wall != null)
		{
			List<Interactable> list = new List<Interactable>();
			if (wall.node != null)
			{
				foreach (Interactable interactable in wall.node.interactables)
				{
					if (interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.glassBulletHole)
					{
						if (Mathf.Abs(interactable.wPos.x - base.transform.position.x) < 0.1f)
						{
							list.Add(interactable);
						}
						else if (Mathf.Abs(interactable.wPos.y - base.transform.position.y) < 0.1f)
						{
							list.Add(interactable);
						}
					}
				}
			}
			if (wall.otherWall != null)
			{
				foreach (Interactable interactable2 in wall.otherWall.node.interactables)
				{
					if (interactable2.preset.specialCaseFlag == InteractablePreset.SpecialCase.glassBulletHole)
					{
						if (Mathf.Abs(interactable2.wPos.x - base.transform.position.x) < 0.1f)
						{
							list.Add(interactable2);
						}
						else if (Mathf.Abs(interactable2.wPos.y - base.transform.position.y) < 0.1f)
						{
							list.Add(interactable2);
						}
					}
				}
			}
			foreach (Interactable interactable3 in list)
			{
				interactable3.SafeDelete(false);
			}
		}
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x00181BF0 File Offset: 0x0017FDF0
	public void AddBulletHole(MurderWeaponPreset weapon, Vector3 contactPosition, Vector3 relativeVelocity, Actor brokenBy, bool noDebris, Vector3 normal)
	{
		if (this.isBroken)
		{
			return;
		}
		if (Game.Instance.printDebug)
		{
			Game.Log("Adding bullet hole to window with " + this.bulletHoles.Count.ToString() + " existing holes", 2);
		}
		if (this.bulletHoles.Count >= 1)
		{
			this.BreakWindow(contactPosition, relativeVelocity, brokenBy, noDebris);
			return;
		}
		if (this.breakAudioEvent != null)
		{
			AudioController.Instance.PlayWorldOneShot(this.breakAudioEvent, brokenBy, null, contactPosition, null, null, 1f, null, false, null, false);
		}
		Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(weapon.glassBulletHole, null, null, null, contactPosition, Quaternion.FromToRotation(Vector3.forward, normal).eulerAngles, null, null, "");
		if (interactable != null)
		{
			interactable.MarkAsTrash(true, false, 0f);
			this.bulletHoles.Add(interactable);
		}
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x00181CCF File Offset: 0x0017FECF
	private void Start()
	{
		this.SpawnStateCheck();
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x00181CCF File Offset: 0x0017FECF
	private void OnEnable()
	{
		this.SpawnStateCheck();
	}

	// Token: 0x06001A36 RID: 6710 RVA: 0x00181CD8 File Offset: 0x0017FED8
	private void SpawnStateCheck()
	{
		bool flag = false;
		bool flag2 = false;
		Vector3 roughPosition = this.GetRoughPosition();
		this.brokenAt = 0f;
		try
		{
			if (GameplayController.Instance.brokenWindows != null && GameplayController.Instance.brokenWindows.TryGetValue(roughPosition, ref this.brokenAt))
			{
				flag = true;
				if (SessionData.Instance.gameTime >= this.brokenAt + GameplayControls.Instance.brokenWindowBoardTime)
				{
					flag2 = true;
				}
			}
			else
			{
				flag2 = false;
				flag = false;
			}
		}
		catch
		{
			flag2 = false;
			flag = false;
		}
		if (flag != this.isBroken || flag2 != this.isBoarded)
		{
			if (flag != this.isBroken && !this.isBroken)
			{
				NewAddress address = this.GetAddress();
				if (address != null)
				{
					address.RemoveVandalism(this.GetRoughPosition());
				}
			}
			this.isBroken = flag;
			this.isBoarded = flag2;
			this.UpdateBrokenState();
		}
	}

	// Token: 0x06001A37 RID: 6711 RVA: 0x00181DB4 File Offset: 0x0017FFB4
	public void SetBroken(bool val)
	{
		if (val != this.isBroken)
		{
			this.isBroken = val;
			if (this.isBroken)
			{
				GameplayController.Instance.brokenWindows.Add(this.GetRoughPosition(), SessionData.Instance.gameTime);
			}
			else
			{
				this.isBoarded = false;
				NewAddress address = this.GetAddress();
				if (address != null)
				{
					address.RemoveVandalism(this.GetRoughPosition());
				}
			}
			this.UpdateBrokenState();
		}
	}

	// Token: 0x06001A38 RID: 6712 RVA: 0x00181E24 File Offset: 0x00180024
	private Vector3 GetRoughPosition()
	{
		return new Vector3((float)Mathf.RoundToInt(base.transform.position.x * 100f) / 100f, (float)Mathf.RoundToInt(base.transform.position.y * 100f) / 100f, (float)Mathf.RoundToInt(base.transform.position.z * 100f) / 100f);
	}

	// Token: 0x06001A39 RID: 6713 RVA: 0x00181E9C File Offset: 0x0018009C
	public NewAddress GetAddress()
	{
		Vector3 roughPosition = this.GetRoughPosition();
		Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(roughPosition);
		NewNode newNode = null;
		if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
		{
			if (!newNode.gameLocation.IsOutside() && newNode.gameLocation.thisAsAddress != null)
			{
				return newNode.gameLocation.thisAsAddress;
			}
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX8)
			{
				Vector3Int vector3Int2 = vector3Int + new Vector3Int(vector2Int.x, vector2Int.y, 0);
				NewNode newNode2 = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode2) && !newNode2.gameLocation.IsOutside())
				{
					return newNode2.gameLocation.thisAsAddress;
				}
			}
			if (newNode.gameLocation.thisAsAddress != null)
			{
				return newNode.gameLocation.thisAsAddress;
			}
		}
		return null;
	}

	// Token: 0x06001A3A RID: 6714 RVA: 0x00181FA4 File Offset: 0x001801A4
	public NewWall GetWall()
	{
		Vector3 roughPosition = this.GetRoughPosition();
		Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(roughPosition);
		NewNode newNode = null;
		NewWall newWall = null;
		if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
		{
			float num = float.PositiveInfinity;
			foreach (NewWall newWall2 in newNode.walls)
			{
				if (newWall == null)
				{
					newWall = newWall2;
				}
				float num2 = Vector3.Distance(base.transform.position, newWall2.position);
				if (num2 < num)
				{
					num = num2;
					newWall = newWall2;
				}
			}
		}
		return newWall;
	}

	// Token: 0x06001A3B RID: 6715 RVA: 0x00182058 File Offset: 0x00180258
	public void UpdateBrokenState()
	{
		foreach (BreakableWindowController.WindowBreakSetting windowBreakSetting in this.panes)
		{
			if (windowBreakSetting != null)
			{
				if (this.isBroken)
				{
					if (this.isBoarded)
					{
						windowBreakSetting.filter.sharedMesh = windowBreakSetting.boardedMesh;
					}
					else
					{
						windowBreakSetting.filter.sharedMesh = windowBreakSetting.brokenMesh;
					}
				}
				else
				{
					windowBreakSetting.filter.sharedMesh = windowBreakSetting.defaultMesh;
				}
				if (this.isBroken)
				{
					if (this.isBoarded)
					{
						windowBreakSetting.renderer.sharedMaterial = windowBreakSetting.boardedMat;
					}
					else
					{
						windowBreakSetting.renderer.sharedMaterial = windowBreakSetting.brokenMat;
					}
				}
				else
				{
					windowBreakSetting.renderer.sharedMaterial = windowBreakSetting.defaultMat;
				}
				if (windowBreakSetting.collider != null && windowBreakSetting.removeColliderWhenBroken)
				{
					if (this.isBoarded)
					{
						windowBreakSetting.collider.enabled = true;
					}
					else
					{
						windowBreakSetting.collider.enabled = !this.isBroken;
					}
				}
				if (windowBreakSetting.removeWhenBoarded && this.isBoarded)
				{
					windowBreakSetting.renderer.gameObject.SetActive(!this.isBoarded);
				}
			}
		}
	}

	// Token: 0x06001A3C RID: 6716 RVA: 0x001821B4 File Offset: 0x001803B4
	public void Shatter(Vector3 contact, Vector3 velocity)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		foreach (BreakableWindowController.WindowBreakSetting windowBreakSetting in this.panes)
		{
			if (windowBreakSetting != null && windowBreakSetting.shatter)
			{
				Texture2D texture2D = windowBreakSetting.defaultMat.GetTexture("_BaseColorMap") as Texture2D;
				if (texture2D == null)
				{
					Game.Log("Misc Error: Cannot find base Texture for " + windowBreakSetting.defaultMat.name, 2);
				}
				else
				{
					if (!texture2D.isReadable)
					{
						Game.LogError("Texture for " + base.name + " is not readable! Set this if you want to shatter this object...", 2);
					}
					Vector3 shardSize = windowBreakSetting.shardSize;
					int num = 0;
					int shardEveryXPixels = windowBreakSetting.shardEveryXPixels;
					int num2 = 0;
					int num3 = 0;
					for (int i = 0; i < texture2D.width; i++)
					{
						for (int j = 0; j < texture2D.height; j++)
						{
							Color pixel = texture2D.GetPixel(i, j);
							if (pixel != Color.black && pixel != Color.clear)
							{
								if (num2 <= 0)
								{
									Vector2 uv;
									uv..ctor((float)i / (float)texture2D.width, (float)j / (float)texture2D.height);
									GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.glassShard, PrefabControls.Instance.mapContainer);
									gameObject.transform.position = base.transform.TransformPoint(this.UvTo3D(uv, windowBreakSetting.filter.mesh));
									gameObject.transform.localScale = shardSize;
									pixel.a = 0.5f;
									gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", pixel);
									num++;
									num2 = shardEveryXPixels;
									Rigidbody component = gameObject.GetComponent<Rigidbody>();
									component.AddExplosionForce(velocity.magnitude * windowBreakSetting.shatterForceMultiplier * Toolbox.Instance.Rand(0.8f, 1.2f, false), contact, windowBreakSetting.filter.mesh.bounds.size.magnitude);
									float num4 = Vector3.Distance(gameObject.transform.position, contact);
									num4 = Mathf.Clamp(1f - num4, -0.1f, 1f);
									component.AddForce(-velocity * num4 * 0.8f, 2);
								}
								else
								{
									num2--;
								}
								num3++;
							}
						}
					}
					Game.Log("...Created " + num.ToString() + " shards.", 2);
				}
			}
		}
	}

	// Token: 0x06001A3D RID: 6717 RVA: 0x0018247C File Offset: 0x0018067C
	private Vector3 UvTo3D(Vector2 uv, Mesh mesh)
	{
		if (mesh == null || !mesh.isReadable)
		{
			Game.LogError("Mesh is not readable! Fingerprints cannot be gathered as the verts aren't readable...", 2);
			return Vector3.zero;
		}
		int[] triangles = mesh.triangles;
		Vector2[] uv2 = mesh.uv;
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector2 vector = uv2[triangles[i]];
			Vector2 vector2 = uv2[triangles[i + 1]];
			Vector2 vector3 = uv2[triangles[i + 2]];
			float num = this.Area(vector, vector2, vector3);
			if (num != 0f)
			{
				float num2 = this.Area(vector2, vector3, uv) / num;
				if (num2 >= 0f)
				{
					float num3 = this.Area(vector3, vector, uv) / num;
					if (num3 >= 0f)
					{
						float num4 = this.Area(vector, vector2, uv) / num;
						if (num4 >= 0f)
						{
							return num2 * vertices[triangles[i]] + num3 * vertices[triangles[i + 1]] + num4 * vertices[triangles[i + 2]];
						}
					}
				}
			}
		}
		return Vector3.zero;
	}

	// Token: 0x06001A3E RID: 6718 RVA: 0x001825A4 File Offset: 0x001807A4
	private float Area(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		Vector2 vector = p1 - p3;
		Vector2 vector2 = p2 - p3;
		return (vector.x * vector2.y - vector.y * vector2.x) / 2f;
	}

	// Token: 0x06001A3F RID: 6719 RVA: 0x001825E2 File Offset: 0x001807E2
	[Button(null, 0)]
	public void AddRandomBulletHole()
	{
		this.AddBulletHole(CriminalControls.Instance.sniperRifle, base.transform.position, Vector3.zero, null, false, Vector3.zero);
	}

	// Token: 0x040022B9 RID: 8889
	public bool isBroken;

	// Token: 0x040022BA RID: 8890
	public bool isBoarded;

	// Token: 0x040022BB RID: 8891
	[ReadOnly]
	public float brokenAt;

	// Token: 0x040022BC RID: 8892
	public float breakForce = 0.12f;

	// Token: 0x040022BD RID: 8893
	public AudioEvent breakAudioEvent;

	// Token: 0x040022BE RID: 8894
	public List<Interactable> bulletHoles = new List<Interactable>();

	// Token: 0x040022BF RID: 8895
	public List<BreakableWindowController.WindowBreakSetting> panes = new List<BreakableWindowController.WindowBreakSetting>();

	// Token: 0x020004BB RID: 1211
	[Serializable]
	public class WindowBreakSetting
	{
		// Token: 0x040022C0 RID: 8896
		public MeshFilter filter;

		// Token: 0x040022C1 RID: 8897
		public MeshRenderer renderer;

		// Token: 0x040022C2 RID: 8898
		[Space(5f)]
		public Material defaultMat;

		// Token: 0x040022C3 RID: 8899
		public Material brokenMat;

		// Token: 0x040022C4 RID: 8900
		public Material boardedMat;

		// Token: 0x040022C5 RID: 8901
		[Space(5f)]
		public Mesh defaultMesh;

		// Token: 0x040022C6 RID: 8902
		public Mesh brokenMesh;

		// Token: 0x040022C7 RID: 8903
		public Mesh boardedMesh;

		// Token: 0x040022C8 RID: 8904
		[Space(5f)]
		public Collider collider;

		// Token: 0x040022C9 RID: 8905
		public bool removeColliderWhenBroken = true;

		// Token: 0x040022CA RID: 8906
		[Space(5f)]
		public bool removeWhenBoarded;

		// Token: 0x040022CB RID: 8907
		public bool shatter;

		// Token: 0x040022CC RID: 8908
		[EnableIf("shatter")]
		public Vector3 shardSize = new Vector3(0.065f, 0.065f, 0.065f);

		// Token: 0x040022CD RID: 8909
		[EnableIf("shatter")]
		public int shardEveryXPixels = 18;

		// Token: 0x040022CE RID: 8910
		[EnableIf("shatter")]
		public float shatterForceMultiplier = 2.4f;
	}
}
