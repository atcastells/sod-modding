using System;
using Rewired;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001DC RID: 476
public class CityEditorInputController : MonoBehaviour
{
	// Token: 0x06000B59 RID: 2905 RVA: 0x000A8BB4 File Offset: 0x000A6DB4
	private void Awake()
	{
		this.editorCam = HighlanderSingleton<CityEditorController>.Instance.cityEditorCam;
		this._player = ReInput.players.GetPlayer(0);
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x000A8BD8 File Offset: 0x000A6DD8
	private void Start()
	{
		this.targetZoomPos = new Vector3(0f, this.editorCam.transform.localPosition.y, this.editorCam.transform.localPosition.z);
		this.curZoomPos = this.targetZoomPos;
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x000A8C2B File Offset: 0x000A6E2B
	private void Update()
	{
		if (this.IsUserUIFocused())
		{
			return;
		}
		this.HandleCameraInputs();
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x000A8C3C File Offset: 0x000A6E3C
	private void HandleCameraInputs()
	{
		if (this._player.GetButton("MouseEnableCamRotate"))
		{
			this.cameraPivot.transform.rotation *= Quaternion.Euler(new Vector3(0f, Input.GetAxis("Mouse X") * this.rotateSpeed, 0f));
		}
		this.ConstrainCameraPivotPosition();
		Vector3 vector = new Vector3(this._player.GetAxis("CamMoveX"), 0f, this._player.GetAxis("CamMoveZ")) * this.flySpeed;
		this.cameraPivot.transform.position += this.cameraPivot.transform.forward * vector.z;
		this.cameraPivot.transform.position += this.cameraPivot.transform.right * vector.x;
		this.ConstrainCameraZoom();
		this.targetZoomPos += new Vector3(0f, -this._player.GetAxis("CamMoveY"), this._player.GetAxis("CamMoveY")) * this.zoomFactor;
		this.curZoomPos = Vector3.Lerp(this.curZoomPos, this.targetZoomPos, this.zoomSpeed * Time.deltaTime);
		this.editorCam.transform.localPosition = this.curZoomPos;
		Quaternion localRotation = this.cameraPitch.localRotation;
		Quaternion quaternion = Quaternion.Euler(this.tgtRot);
		this.cameraPitch.localRotation = Quaternion.Slerp(localRotation, quaternion, 4f * Time.deltaTime);
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x000A8E00 File Offset: 0x000A7000
	private void ConstrainCameraPivotPosition()
	{
		float num = Mathf.Clamp(this.cameraPivot.position.x, -256f, 256f);
		float num2 = Mathf.Clamp(this.cameraPivot.position.z, -256f, 256f);
		this.cameraPivot.position = new Vector3(num, 0f, num2);
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x000A8E64 File Offset: 0x000A7064
	private void ConstrainCameraZoom()
	{
		float num = Mathf.Clamp(this.targetZoomPos.y, this.minZoom, this.maxZoom);
		float num2 = Mathf.Clamp(this.targetZoomPos.z, -this.maxZoom, -this.minZoom);
		this.targetZoomPos = new Vector3(0f, num, num2);
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x000A8EBF File Offset: 0x000A70BF
	private bool IsUserUIFocused()
	{
		return PopupMessageController.Instance.active;
	}

	// Token: 0x04000BEA RID: 3050
	[Header("Editor Camera Settings")]
	public Camera editorCam;

	// Token: 0x04000BEB RID: 3051
	public Transform cameraPitch;

	// Token: 0x04000BEC RID: 3052
	public Transform cameraPivot;

	// Token: 0x04000BED RID: 3053
	[FormerlySerializedAs("movementSpeed")]
	public float rotateSpeed = 5f;

	// Token: 0x04000BEE RID: 3054
	public float flySpeed = 5f;

	// Token: 0x04000BEF RID: 3055
	public float minZoom = 35f;

	// Token: 0x04000BF0 RID: 3056
	public float maxZoom = 80f;

	// Token: 0x04000BF1 RID: 3057
	public float zoomFactor;

	// Token: 0x04000BF2 RID: 3058
	public float zoomSpeed;

	// Token: 0x04000BF3 RID: 3059
	private Player _player;

	// Token: 0x04000BF4 RID: 3060
	private Vector3 targetZoomPos;

	// Token: 0x04000BF5 RID: 3061
	private Vector3 curZoomPos;

	// Token: 0x04000BF6 RID: 3062
	public Vector3 curRot;

	// Token: 0x04000BF7 RID: 3063
	public Vector3 tgtRot;
}
