using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000226 RID: 550
public class AnimateTiledTexture : MonoBehaviour
{
	// Token: 0x06000C6D RID: 3181 RVA: 0x000B1909 File Offset: 0x000AFB09
	public void RegisterCallback(AnimateTiledTexture.VoidEvent cbFunction)
	{
		if (this._enableEvents)
		{
			this._voidEventCallbackList.Add(cbFunction);
			return;
		}
		Game.Log("AnimateTiledTexture: You are attempting to register a callback but the events of this object are not enabled!", 2);
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x000B192B File Offset: 0x000AFB2B
	public void UnRegisterCallback(AnimateTiledTexture.VoidEvent cbFunction)
	{
		if (this._enableEvents)
		{
			this._voidEventCallbackList.Remove(cbFunction);
			return;
		}
		Game.Log("AnimateTiledTexture: You are attempting to un-register a callback but the events of this object are not enabled!", 2);
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x000B1950 File Offset: 0x000AFB50
	public void Play()
	{
		if (this._isPlaying)
		{
			base.StopCoroutine("updateTiling");
			this._isPlaying = false;
		}
		base.GetComponent<Renderer>().enabled = true;
		this._index = this._columns;
		base.StartCoroutine(this.updateTiling());
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x000B199C File Offset: 0x000AFB9C
	public void ChangeMaterial(Material newMaterial, bool newInstance = false)
	{
		if (newInstance)
		{
			if (this._hasMaterialInstance)
			{
				Object.Destroy(base.GetComponent<Renderer>().sharedMaterial);
			}
			this._materialInstance = new Material(newMaterial);
			base.GetComponent<Renderer>().sharedMaterial = this._materialInstance;
			this._hasMaterialInstance = true;
		}
		else
		{
			base.GetComponent<Renderer>().sharedMaterial = newMaterial;
		}
		this.CalcTextureSize();
		base.GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", this._textureSize);
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x000B1A17 File Offset: 0x000AFC17
	private void Awake()
	{
		if (this._enableEvents)
		{
			this._voidEventCallbackList = new List<AnimateTiledTexture.VoidEvent>();
		}
		this.ChangeMaterial(base.GetComponent<Renderer>().sharedMaterial, this._newMaterialInstance);
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x000B1A43 File Offset: 0x000AFC43
	private void OnDestroy()
	{
		if (this._hasMaterialInstance)
		{
			Object.Destroy(base.GetComponent<Renderer>().sharedMaterial);
			this._hasMaterialInstance = false;
		}
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x000B1A64 File Offset: 0x000AFC64
	private void HandleCallbacks(List<AnimateTiledTexture.VoidEvent> cbList)
	{
		for (int i = 0; i < cbList.Count; i++)
		{
			cbList[i]();
		}
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x000B1A8E File Offset: 0x000AFC8E
	private void OnEnable()
	{
		this.CalcTextureSize();
		if (this._playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x000B1AA4 File Offset: 0x000AFCA4
	private void CalcTextureSize()
	{
		this._textureSize = new Vector2(1f / (float)this._columns, 1f / (float)this._rows);
		this._textureSize.x = this._textureSize.x / this._scale.x;
		this._textureSize.y = this._textureSize.y / this._scale.y;
		this._textureSize -= this._buffer;
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x000B1B31 File Offset: 0x000AFD31
	private IEnumerator updateTiling()
	{
		this._isPlaying = true;
		int checkAgainst = this._rows * this._columns;
		for (;;)
		{
			if (this._index >= checkAgainst)
			{
				this._index = 0;
				if (this._playOnce)
				{
					if (checkAgainst == this._columns)
					{
						break;
					}
					checkAgainst = this._columns;
				}
			}
			this.ApplyOffset();
			this._index++;
			yield return new WaitForSeconds(1f / this._framesPerSecond);
		}
		if (this._enableEvents)
		{
			this.HandleCallbacks(this._voidEventCallbackList);
		}
		if (this._disableUponCompletion)
		{
			base.gameObject.GetComponent<Renderer>().enabled = false;
		}
		this._isPlaying = false;
		yield break;
		yield break;
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x000B1B40 File Offset: 0x000AFD40
	private void ApplyOffset()
	{
		Vector2 vector;
		vector..ctor((float)this._index / (float)this._columns - (float)(this._index / this._columns), 1f - (float)(this._index / this._columns) / (float)this._rows);
		if (vector.y == 1f)
		{
			vector.y = 0f;
		}
		vector.x += (1f / (float)this._columns - this._textureSize.x) / 2f;
		vector.y += (1f / (float)this._rows - this._textureSize.y) / 2f;
		vector.x += this._offset.x;
		vector.y += this._offset.y;
		base.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", vector);
	}

	// Token: 0x04000E0C RID: 3596
	public int _columns = 2;

	// Token: 0x04000E0D RID: 3597
	public int _rows = 2;

	// Token: 0x04000E0E RID: 3598
	public Vector2 _scale = new Vector3(1f, 1f);

	// Token: 0x04000E0F RID: 3599
	public Vector2 _offset = Vector2.zero;

	// Token: 0x04000E10 RID: 3600
	public Vector2 _buffer = Vector2.zero;

	// Token: 0x04000E11 RID: 3601
	public float _framesPerSecond = 10f;

	// Token: 0x04000E12 RID: 3602
	public bool _playOnce;

	// Token: 0x04000E13 RID: 3603
	public bool _disableUponCompletion;

	// Token: 0x04000E14 RID: 3604
	public bool _enableEvents;

	// Token: 0x04000E15 RID: 3605
	public bool _playOnEnable = true;

	// Token: 0x04000E16 RID: 3606
	public bool _newMaterialInstance;

	// Token: 0x04000E17 RID: 3607
	private int _index;

	// Token: 0x04000E18 RID: 3608
	private Vector2 _textureSize = Vector2.zero;

	// Token: 0x04000E19 RID: 3609
	private Material _materialInstance;

	// Token: 0x04000E1A RID: 3610
	private bool _hasMaterialInstance;

	// Token: 0x04000E1B RID: 3611
	private bool _isPlaying;

	// Token: 0x04000E1C RID: 3612
	private List<AnimateTiledTexture.VoidEvent> _voidEventCallbackList;

	// Token: 0x02000227 RID: 551
	// (Invoke) Token: 0x06000C7A RID: 3194
	public delegate void VoidEvent();
}
