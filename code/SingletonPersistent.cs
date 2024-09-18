using System;
using UnityEngine;

// Token: 0x0200081C RID: 2076
public class SingletonPersistent<T> : MonoBehaviour where T : Component
{
	// Token: 0x17000128 RID: 296
	// (get) Token: 0x060026A6 RID: 9894 RVA: 0x001F82F0 File Offset: 0x001F64F0
	// (set) Token: 0x060026A7 RID: 9895 RVA: 0x001F82F7 File Offset: 0x001F64F7
	public static T Instance { get; private set; }

	// Token: 0x060026A8 RID: 9896 RVA: 0x001F82FF File Offset: 0x001F64FF
	public virtual void Awake()
	{
		if (SingletonPersistent<T>.Instance == null)
		{
			SingletonPersistent<T>.Instance = (this as T);
			Object.DontDestroyOnLoad(this);
			return;
		}
		Object.Destroy(base.gameObject);
	}
}
