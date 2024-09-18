using System;
using UnityEngine;

// Token: 0x0200081B RID: 2075
public abstract class HighlanderSingleton<T> : MonoBehaviour where T : Component
{
	// Token: 0x17000127 RID: 295
	// (get) Token: 0x060026A2 RID: 9890 RVA: 0x001F8238 File Offset: 0x001F6438
	public static T Instance
	{
		get
		{
			if (HighlanderSingleton<T>.instance == null)
			{
				HighlanderSingleton<T>.instance = Object.FindObjectOfType<T>(true);
				if (HighlanderSingleton<T>.instance == null)
				{
					HighlanderSingleton<T>.instance = new GameObject
					{
						name = typeof(T).Name
					}.AddComponent<T>();
				}
			}
			return HighlanderSingleton<T>.instance;
		}
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x001F829D File Offset: 0x001F649D
	protected virtual void Awake()
	{
		if (HighlanderSingleton<T>.instance == null)
		{
			HighlanderSingleton<T>.instance = (this as T);
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x001F82D8 File Offset: 0x001F64D8
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
		HighlanderSingleton<T>.instance = default(T);
	}

	// Token: 0x04004532 RID: 17714
	private static T instance;
}
