using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000563 RID: 1379
public class ElevatorControlsController : MonoBehaviour
{
	// Token: 0x06001DFE RID: 7678 RVA: 0x001A50F4 File Offset: 0x001A32F4
	private void OnEnable()
	{
		this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		this.evidence = this.parentWindow.passedEvidence;
		foreach (RectTransform rectTransform in this.buttons)
		{
			rectTransform.gameObject.SetActive(false);
		}
		Interactable interactable = InteractionController.Instance.currentLookingAtInteractable.interactable;
		if (interactable != null)
		{
			Elevator elevator = interactable.objectRef as Elevator;
			if (elevator != null)
			{
				for (int i = -2; i < this.buttons.Count - 2; i++)
				{
					if (interactable.node.building.floors.ContainsKey(i))
					{
						if (elevator.elevatorFloors.ContainsKey(i))
						{
							this.buttons[i + 2].gameObject.SetActive(true);
						}
						else
						{
							Game.Log("Elevator does not visit floor " + i.ToString(), 2);
							this.buttons[i + 2].gameObject.SetActive(false);
						}
					}
					else
					{
						Game.Log("Elevator does not visit floor " + i.ToString(), 2);
						this.buttons[i + 2].gameObject.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x001A5258 File Offset: 0x001A3458
	public void PressNumberButton(int newInt)
	{
		Interactable interactable = InteractionController.Instance.currentLookingAtInteractable.interactable;
		if (interactable == null)
		{
			return;
		}
		Elevator elevator = interactable.objectRef as Elevator;
		if (elevator == null)
		{
			return;
		}
		bool upButton = false;
		if (newInt > elevator.currentFloor)
		{
			upButton = true;
		}
		elevator.CallElevator(newInt, upButton);
		AudioEvent eventPreset = AudioControls.Instance.keypadButtons[0];
		try
		{
			eventPreset = AudioControls.Instance.keypadButtons[newInt % 10];
		}
		catch
		{
		}
		AudioController.Instance.PlayWorldOneShot(eventPreset, Player.Instance, null, interactable.wPos, null, null, 1f, null, false, null, false);
		this.parentWindow.CloseWindow(true);
	}

	// Token: 0x040027DB RID: 10203
	public InfoWindow parentWindow;

	// Token: 0x040027DC RID: 10204
	public Evidence evidence;

	// Token: 0x040027DD RID: 10205
	public WindowContentController windowContent;

	// Token: 0x040027DE RID: 10206
	public List<RectTransform> buttons = new List<RectTransform>();

	// Token: 0x040027DF RID: 10207
	public TextMeshProUGUI inputText;
}
