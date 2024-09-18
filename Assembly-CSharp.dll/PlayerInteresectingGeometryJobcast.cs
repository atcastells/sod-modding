using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x020002C1 RID: 705
public class PlayerInteresectingGeometryJobcast : MonoBehaviour
{
	// Token: 0x06000F9E RID: 3998 RVA: 0x000DDD20 File Offset: 0x000DBF20
	private void Update()
	{
		if (!SessionData.Instance.play || !SessionData.Instance.startedGame || !this.fpsController.enableMovement || this.fpsController.ghostMovement || CutSceneController.Instance.cutSceneActive || InteractionController.Instance.lockedInInteraction != null || Player.Instance.transitionActive || !this.fpsController.m_CharacterController.enabled)
		{
			return;
		}
		NativeArray<RaycastHit> nativeArray = new NativeArray<RaycastHit>(2, 3, 1);
		NativeArray<RaycastCommand> nativeArray2 = new NativeArray<RaycastCommand>(1, 3, 1);
		this._origin = base.transform.position + new Vector3(0f, this.characterController.height / 2f - 0.2f, 0f);
		float num = this.characterController.height - 0.3f;
		nativeArray2[0] = new RaycastCommand(this._origin, Vector3.down, num, this.mask, 2);
		RaycastCommand.ScheduleBatch(nativeArray2, nativeArray, 1, default(JobHandle)).Complete();
		foreach (RaycastHit raycastHit in nativeArray)
		{
			if (raycastHit.collider != null)
			{
				base.transform.position = raycastHit.point + new Vector3(0f, this.characterController.height / 2f, 0f);
				Debug.Log("PlayerIntersectingGeometry: Hits " + raycastHit.collider.transform.name + ", Reposition player " + base.transform.position.ToString());
			}
		}
		nativeArray.Dispose();
		nativeArray2.Dispose();
	}

	// Token: 0x04001320 RID: 4896
	public CharacterController characterController;

	// Token: 0x04001321 RID: 4897
	public FirstPersonController fpsController;

	// Token: 0x04001322 RID: 4898
	private Vector3 _origin;

	// Token: 0x04001323 RID: 4899
	private Vector3 _direction;

	// Token: 0x04001324 RID: 4900
	public LayerMask mask;
}
