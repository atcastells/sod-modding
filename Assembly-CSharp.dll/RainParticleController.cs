using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020004C0 RID: 1216
public class RainParticleController : MonoBehaviour
{
	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06001A54 RID: 6740 RVA: 0x00182EFC File Offset: 0x001810FC
	public static RainParticleController Instance
	{
		get
		{
			return RainParticleController._instance;
		}
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x00182F03 File Offset: 0x00181103
	private void Awake()
	{
		if (RainParticleController._instance != null && RainParticleController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			RainParticleController._instance = this;
		}
		this.SetSnowMode(false, true);
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x00182F3A File Offset: 0x0018113A
	public void SetSnowMode(bool val, bool forceUpdate = false)
	{
		if (val != this.snowMode || forceUpdate)
		{
			this.snowMode = val;
		}
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x00182F54 File Offset: 0x00181154
	public void UpdateValidSpawnNodes()
	{
		this.validSpawnNodes.Clear();
		if (SessionData.Instance.startedGame)
		{
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX24)
			{
				Vector3Int vector3Int = Player.Instance.currentNodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
				NewNode newNode = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) && !(newNode.room == null) && !(newNode.room.gameLocation == null))
				{
					try
					{
						if ((newNode.room.gameLocation.thisAsStreet != null || newNode.room.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside) && (newNode.floorType == NewNode.FloorTileType.floorOnly || newNode.floorType == NewNode.FloorTileType.none))
						{
							this.validSpawnNodes.Add(newNode);
						}
					}
					catch
					{
					}
				}
			}
		}
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x00183068 File Offset: 0x00181268
	private void Update()
	{
		if (SessionData.Instance.play && SessionData.Instance.startedGame)
		{
			if (this.validSpawnNodes.Count > 0 && (SessionData.Instance.currentSnow > 0f || SessionData.Instance.currentRain > 0f) && this.particles.Count < this.desiredParticleCount)
			{
				if (this.spawnIntervalTimer <= 0f)
				{
					Vector3 zero = Vector3.zero;
					if (this.snowMode)
					{
						if (this.GetSpawnPosition(out zero))
						{
							GameObject gameObject = Object.Instantiate<GameObject>(this.particlePrefabSnow, base.transform.parent);
							gameObject.transform.position = zero;
							if (!this.billboard)
							{
								gameObject.transform.eulerAngles = base.transform.eulerAngles + new Vector3(Toolbox.Instance.Rand(-this.rotationEulerSnow.x, this.rotationEulerSnow.x, false), Toolbox.Instance.Rand(-this.rotationEulerSnow.y, this.rotationEulerSnow.y, false), Toolbox.Instance.Rand(-this.rotationEulerSnow.z, this.rotationEulerSnow.z, false));
							}
							this.spawnIntervalTimer += Toolbox.Instance.Rand(this.spawnIntervalSnow.x, this.spawnIntervalSnow.y, false);
							this.particles.Add(new RainParticleController.RainParticle
							{
								trans = gameObject.transform,
								speed = Toolbox.Instance.Rand(this.speedSnow.x, this.speedSnow.y, false)
							});
						}
					}
					else if (this.GetSpawnPosition(out zero))
					{
						GameObject gameObject2 = Object.Instantiate<GameObject>(this.particlePrefab, base.transform.parent);
						gameObject2.transform.position = zero;
						gameObject2.transform.eulerAngles = base.transform.eulerAngles + new Vector3(Toolbox.Instance.Rand(-this.rotationEuler.x, this.rotationEuler.x, false), Toolbox.Instance.Rand(-this.rotationEuler.y, this.rotationEuler.y, false), Toolbox.Instance.Rand(-this.rotationEuler.z, this.rotationEuler.z, false));
						this.spawnIntervalTimer += Toolbox.Instance.Rand(this.spawnInterval.x, this.spawnInterval.y, false);
						this.particles.Add(new RainParticleController.RainParticle
						{
							trans = gameObject2.transform,
							speed = Toolbox.Instance.Rand(this.speed.x, this.speed.y, false)
						});
					}
					this.actualParticleCount = this.particles.Count;
				}
				else
				{
					this.spawnIntervalTimer -= Time.deltaTime;
				}
			}
			if (this.snowMode)
			{
				for (int i = 0; i < this.particles.Count; i++)
				{
					RainParticleController.RainParticle rainParticle = this.particles[i];
					if (this.snowMode && this.billboard)
					{
						rainParticle.trans.rotation = CameraController.Instance.cam.transform.rotation * Quaternion.Euler(this.rotationEulerSnow);
						rainParticle.trans.position += Vector3.down * rainParticle.speed * Time.deltaTime * Mathf.Max(SessionData.Instance.currentSnow, 0.7f);
					}
					else
					{
						rainParticle.trans.position += -rainParticle.trans.up * rainParticle.speed * Time.deltaTime * Mathf.Max(SessionData.Instance.currentSnow, 0.7f);
					}
					if (rainParticle.trans.position.y < this.snowFloorWorldYPos)
					{
						Vector3 zero2 = Vector3.zero;
						if (!this.GetSpawnPosition(out zero2) || this.particles.Count > this.desiredParticleCount)
						{
							Object.Destroy(rainParticle.trans.gameObject);
							this.particles.RemoveAt(i);
							i--;
						}
						else
						{
							rainParticle.speed = Toolbox.Instance.Rand(this.speedSnow.x, this.speedSnow.y, false);
							rainParticle.trans.position = zero2;
							if (!this.billboard)
							{
								rainParticle.trans.eulerAngles = base.transform.eulerAngles + new Vector3(Toolbox.Instance.Rand(-this.rotationEulerSnow.x, this.rotationEulerSnow.x, false), Toolbox.Instance.Rand(-this.rotationEulerSnow.y, this.rotationEulerSnow.y, false), Toolbox.Instance.Rand(-this.rotationEulerSnow.z, this.rotationEulerSnow.z, false));
							}
						}
					}
				}
				return;
			}
			for (int j = 0; j < this.particles.Count; j++)
			{
				RainParticleController.RainParticle rainParticle2 = this.particles[j];
				rainParticle2.trans.position += -rainParticle2.trans.up * rainParticle2.speed * Time.deltaTime * Mathf.Max(SessionData.Instance.currentRain, 0.7f);
				if (rainParticle2.trans.position.y < this.rainFloorWorldYPos)
				{
					Vector3 zero3 = Vector3.zero;
					if (!this.GetSpawnPosition(out zero3) || this.particles.Count > this.desiredParticleCount)
					{
						Object.Destroy(rainParticle2.trans.gameObject);
						this.particles.RemoveAt(j);
						j--;
					}
					else
					{
						rainParticle2.speed = Toolbox.Instance.Rand(this.speed.x, this.speed.y, false);
						rainParticle2.trans.position = zero3;
						rainParticle2.trans.eulerAngles = base.transform.eulerAngles + new Vector3(Toolbox.Instance.Rand(-this.rotationEuler.x, this.rotationEuler.x, false), Toolbox.Instance.Rand(-this.rotationEuler.y, this.rotationEuler.y, false), Toolbox.Instance.Rand(-this.rotationEuler.z, this.rotationEuler.z, false));
					}
				}
			}
		}
	}

	// Token: 0x06001A59 RID: 6745 RVA: 0x0018377C File Offset: 0x0018197C
	private bool GetSpawnPosition(out Vector3 spawn)
	{
		spawn = Vector3.zero;
		if (this.validSpawnNodes.Count <= 0)
		{
			return false;
		}
		NewNode newNode = this.validSpawnNodes[Toolbox.Instance.Rand(0, this.validSpawnNodes.Count, false)];
		spawn = newNode.position + new Vector3(Toolbox.Instance.Rand(PathFinder.Instance.nodeSize.x * -0.45f, PathFinder.Instance.nodeSize.x * 0.45f, false), 0f, Toolbox.Instance.Rand(PathFinder.Instance.nodeSize.y * -0.45f, PathFinder.Instance.nodeSize.y * 0.45f, false));
		if (this.snowMode)
		{
			spawn.y = Player.Instance.transform.position.y + this.spawnHeightSnow;
		}
		else
		{
			spawn.y = Player.Instance.transform.position.y + this.spawnHeight;
		}
		return true;
	}

	// Token: 0x040022E2 RID: 8930
	[Header("Rain Settings")]
	[Tooltip("Rain speed")]
	public Vector2 speed = new Vector2(30f, 38f);

	// Token: 0x040022E3 RID: 8931
	[Tooltip("Spawn particles this high (local position)")]
	public float spawnHeight = 20f;

	// Token: 0x040022E4 RID: 8932
	[Tooltip("The world Y position upon which the particles are set to the top of the simulation.")]
	public float rainFloorWorldYPos = -5f;

	// Token: 0x040022E5 RID: 8933
	[Tooltip("Minimum time between spawning particles (seconds)")]
	public Vector2 spawnInterval = new Vector2(1f, 1.5f);

	// Token: 0x040022E6 RID: 8934
	public GameObject particlePrefab;

	// Token: 0x040022E7 RID: 8935
	public Vector3 rotationEuler = new Vector3(0.01f, 0.01f, 0.01f);

	// Token: 0x040022E8 RID: 8936
	[Header("Snow Settings")]
	[Tooltip("Rain speed")]
	public Vector2 speedSnow = new Vector2(30f, 38f);

	// Token: 0x040022E9 RID: 8937
	[Tooltip("Spawn particles this high (local position)")]
	public float spawnHeightSnow = 2f;

	// Token: 0x040022EA RID: 8938
	[Tooltip("The world Y position upon which the particles are set to the top of the simulation.")]
	public float snowFloorWorldYPos = -5f;

	// Token: 0x040022EB RID: 8939
	[Tooltip("Minimum time between spawning particles (seconds)")]
	public Vector2 spawnIntervalSnow = new Vector2(1f, 1.5f);

	// Token: 0x040022EC RID: 8940
	public GameObject particlePrefabSnow;

	// Token: 0x040022ED RID: 8941
	public Vector3 rotationEulerSnow = new Vector3(0.01f, 0.01f, 0.01f);

	// Token: 0x040022EE RID: 8942
	public bool billboard = true;

	// Token: 0x040022EF RID: 8943
	[ReadOnly]
	[Header("State")]
	public int desiredParticleCount = 20;

	// Token: 0x040022F0 RID: 8944
	[ReadOnly]
	public int actualParticleCount;

	// Token: 0x040022F1 RID: 8945
	[ReadOnly]
	public List<RainParticleController.RainParticle> particles = new List<RainParticleController.RainParticle>();

	// Token: 0x040022F2 RID: 8946
	private float spawnIntervalTimer;

	// Token: 0x040022F3 RID: 8947
	public bool snowMode;

	// Token: 0x040022F4 RID: 8948
	[ReadOnly]
	public List<NewNode> validSpawnNodes = new List<NewNode>();

	// Token: 0x040022F5 RID: 8949
	private static RainParticleController _instance;

	// Token: 0x020004C1 RID: 1217
	public class RainParticle
	{
		// Token: 0x040022F6 RID: 8950
		public Transform trans;

		// Token: 0x040022F7 RID: 8951
		public float speed;
	}
}
