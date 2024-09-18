using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000536 RID: 1334
public class MenuNavigationOrderConfig : MonoBehaviour
{
	// Token: 0x06001D23 RID: 7459 RVA: 0x0019E14C File Offset: 0x0019C34C
	[Button(null, 0)]
	public void Configure()
	{
		Dictionary<Transform, List<Button>> dictionary = new Dictionary<Transform, List<Button>>();
		foreach (Transform transform in this.contentParentHierarchy)
		{
			Button[] componentsInChildren = transform.GetComponentsInChildren<Button>(true);
			dictionary.Add(transform, Enumerable.ToList<Button>(componentsInChildren));
		}
		foreach (KeyValuePair<Transform, List<Button>> keyValuePair in dictionary)
		{
			foreach (Button button in keyValuePair.Value)
			{
				Button selectOnUp = null;
				float num = float.PositiveInfinity;
				float num2 = float.PositiveInfinity;
				foreach (Button button2 in keyValuePair.Value)
				{
					if (!(button == button2) && button2.transform.position.y > button.transform.position.y)
					{
						float num3 = (float)Mathf.RoundToInt(Mathf.Abs(button.transform.position.y - button2.transform.position.y) * 100f) / 100f;
						float num4 = (float)Mathf.RoundToInt(Mathf.Abs(button.transform.position.x - button2.transform.position.x) * 100f) / 100f;
						if (num3 < num)
						{
							selectOnUp = button2;
							num = num3;
							num2 = num4;
						}
						else if (num3 == num && num4 <= num2)
						{
							selectOnUp = button2;
							num = num3;
							num2 = num4;
						}
					}
				}
				Button selectOnDown = null;
				num = float.PositiveInfinity;
				num2 = float.PositiveInfinity;
				foreach (Button button3 in keyValuePair.Value)
				{
					if (!(button == button3) && button3.transform.position.y < button.transform.position.y)
					{
						float num5 = (float)Mathf.RoundToInt(Mathf.Abs(button.transform.position.y - button3.transform.position.y) * 100f) / 100f;
						float num6 = (float)Mathf.RoundToInt(Mathf.Abs(button.transform.position.x - button3.transform.position.x) * 100f) / 100f;
						if (num5 < num)
						{
							selectOnDown = button3;
							num = num5;
							num2 = num6;
						}
						else if (num5 == num && num6 <= num2)
						{
							selectOnDown = button3;
							num = num5;
							num2 = num6;
						}
					}
				}
				Button button4 = button.navigation.selectOnLeft as Button;
				if (button4 == null)
				{
					int num7 = this.contentParentHierarchy.IndexOf(keyValuePair.Key);
					if (num7 > 0 && this.leftMovesUpHierarchy)
					{
						Transform transform2 = this.contentParentHierarchy[num7 - 1];
						if (dictionary.ContainsKey(transform2))
						{
							button4 = dictionary[transform2][0];
						}
					}
				}
				Navigation navigation = default(Navigation);
				navigation.mode = 4;
				navigation.selectOnUp = selectOnUp;
				navigation.selectOnDown = selectOnDown;
				navigation.selectOnLeft = button4;
				navigation.selectOnRight = button.navigation.selectOnRight;
				button.navigation = navigation;
			}
		}
	}

	// Token: 0x040026E6 RID: 9958
	public List<Transform> contentParentHierarchy = new List<Transform>();

	// Token: 0x040026E7 RID: 9959
	public bool leftMovesUpHierarchy = true;
}
