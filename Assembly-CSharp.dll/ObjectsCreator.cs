using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class ObjectsCreator : Creator
{
	// Token: 0x17000048 RID: 72
	// (get) Token: 0x060009D5 RID: 2517 RVA: 0x000962EC File Offset: 0x000944EC
	public static ObjectsCreator Instance
	{
		get
		{
			return ObjectsCreator._instance;
		}
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x000962F3 File Offset: 0x000944F3
	private void Awake()
	{
		if (ObjectsCreator._instance != null && ObjectsCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		ObjectsCreator._instance = this;
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x00096321 File Offset: 0x00094521
	public override void StartLoading()
	{
		Game.Log("CityGen: Load objects...", 2);
		base.StartCoroutine("Load");
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x0009633A File Offset: 0x0009453A
	private IEnumerator Load()
	{
		int cursor = 0;
		using (List<MetaObject>.Enumerator enumerator = CityConstructor.Instance.currentData.metas.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MetaObject metaObject = enumerator.Current;
				if (!CityData.Instance.metaObjectDictionary.ContainsKey(metaObject.id))
				{
					CityData.Instance.metaObjectDictionary.Add(metaObject.id, metaObject);
				}
			}
			goto IL_237;
		}
		IL_90:
		for (int i = 0; i < this.loadChunk; i++)
		{
			if (cursor >= CityConstructor.Instance.currentData.interactables.Count)
			{
				break;
			}
			Interactable inter = CityConstructor.Instance.currentData.interactables[cursor];
			bool flag = false;
			if (CityConstructor.Instance.saveState != null)
			{
				if (CityConstructor.Instance.saveState.removedCityData.Contains(inter.id))
				{
					inter.Delete();
					flag = true;
				}
				else
				{
					int num = CityConstructor.Instance.saveState.interactables.FindIndex((Interactable item) => item.id == inter.id);
					if (num > -1)
					{
						inter = CityConstructor.Instance.saveState.interactables[num];
						CityConstructor.Instance.saveState.interactables.RemoveAt(num);
						inter.wasLoadedFromSave = true;
					}
				}
			}
			if (!flag)
			{
				if (!CityData.Instance.savableInteractableDictionary.ContainsKey(inter.id))
				{
					inter.MainSetupStart();
					inter.OnLoad();
				}
				else
				{
					FurnitureLocation furnitureParent = inter.furnitureParent;
				}
			}
			int num2 = cursor;
			cursor = num2 + 1;
		}
		CityConstructor.Instance.loadingProgress = (float)cursor / (float)CityConstructor.Instance.currentData.interactables.Count;
		yield return null;
		IL_237:
		if (cursor >= CityConstructor.Instance.currentData.interactables.Count)
		{
			base.SetComplete();
			yield break;
		}
		goto IL_90;
	}

	// Token: 0x04000A06 RID: 2566
	public int loadChunk = 100;

	// Token: 0x04000A07 RID: 2567
	private static ObjectsCreator _instance;
}
