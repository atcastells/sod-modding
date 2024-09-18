using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000014 RID: 20
public class lz4test : MonoBehaviour
{
	// Token: 0x06000094 RID: 148 RVA: 0x00005DD0 File Offset: 0x00003FD0
	private void Start()
	{
		this.ppath = Application.persistentDataPath;
		this.buff = new byte[0];
		Debug.Log(this.ppath);
		Screen.sleepTimeout = -1;
		if (!File.Exists(this.ppath + "/" + this.myFile))
		{
			base.StartCoroutine(this.DownloadTestFile());
			return;
		}
		this.downloadDone = true;
	}

	// Token: 0x06000095 RID: 149 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Update()
	{
		if (Input.GetKeyDown(27))
		{
			Application.Quit();
		}
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00005E38 File Offset: 0x00004038
	private void OnGUI()
	{
		if (this.downloadDone)
		{
			GUI.Label(new Rect(50f, 0f, 350f, 30f), "package downloaded, ready to extract");
			GUI.Label(new Rect(50f, 30f, 450f, 90f), this.ppath);
		}
		if (this.downloadDone && GUI.Button(new Rect(50f, 150f, 250f, 50f), "start LZ4 test"))
		{
			this.compressionStarted = true;
			new Thread(new ThreadStart(this.DoTests)).Start();
		}
		if (this.compressionStarted)
		{
			GUI.Label(new Rect(50f, 220f, 250f, 40f), "LZ4 Compress:    " + this.lz1.ToString() + "%");
			GUI.Label(new Rect(300f, 220f, 120f, 40f), this.progress[0].ToString() + "%");
			GUI.Label(new Rect(50f, 260f, 250f, 40f), "LZ4 Decompress: " + (this.lz2 + 1).ToString());
			GUI.Label(new Rect(300f, 260f, 250f, 40f), this.bytes[0].ToString());
			GUI.Label(new Rect(50f, 300f, 250f, 40f), "Buffer Compress:    " + this.lz3.ToString());
			GUI.Label(new Rect(50f, 340f, 250f, 40f), "Buffer Decompress: " + this.lz4.ToString());
		}
	}

	// Token: 0x06000097 RID: 151 RVA: 0x00006028 File Offset: 0x00004228
	private void DoTests()
	{
		this.lz1 = LZ4.compress(this.ppath + "/" + this.myFile, this.ppath + "/" + this.myFile + ".lz4", 9, this.progress);
		this.lz2 = LZ4.decompress(this.ppath + "/" + this.myFile + ".lz4", this.ppath + "/" + this.myFile + "B.tif", this.bytes, null);
		if (File.Exists(this.ppath + "/" + this.myFile))
		{
			if (LZ4.compressBuffer(File.ReadAllBytes(this.ppath + "/" + this.myFile), ref this.buff, 9, true))
			{
				this.lz3 = 1;
				File.WriteAllBytes(this.ppath + "/buffer1.lz4buf", this.buff);
			}
			byte[] inBuffer = File.ReadAllBytes(this.ppath + "/buffer1.lz4buf");
			if (LZ4.decompressBuffer(inBuffer, ref this.buff, true, 0))
			{
				this.lz4 = 1;
				File.WriteAllBytes(this.ppath + "/buffer1D.tif", this.buff);
			}
			int num = LZ4.decompressBufferFixed(inBuffer, ref this.fixedOutBuffer, true, true, 0);
			if (num > 0)
			{
				Debug.Log(" # Decompress Fixed size Buffer: " + num.ToString());
			}
		}
	}

	// Token: 0x06000098 RID: 152 RVA: 0x0000619E File Offset: 0x0000439E
	private IEnumerator DownloadTestFile()
	{
		if (File.Exists(this.ppath + "/" + this.myFile))
		{
			File.Delete(this.ppath + "/" + this.myFile);
		}
		Debug.Log("starting download");
		using (UnityWebRequest www = UnityWebRequest.Get(this.uri + this.myFile))
		{
			yield return www.SendWebRequest();
			if (www.error != null)
			{
				Debug.Log(www.error);
			}
			else
			{
				this.downloadDone = true;
				File.WriteAllBytes(this.ppath + "/" + this.myFile, www.downloadHandler.data);
				Debug.Log("download done");
			}
		}
		UnityWebRequest www = null;
		yield break;
		yield break;
	}

	// Token: 0x04000077 RID: 119
	private float lz1;

	// Token: 0x04000078 RID: 120
	private int lz2 = -1;

	// Token: 0x04000079 RID: 121
	private int lz3;

	// Token: 0x0400007A RID: 122
	private int lz4;

	// Token: 0x0400007B RID: 123
	private int fbuftest;

	// Token: 0x0400007C RID: 124
	private int nFbuftest;

	// Token: 0x0400007D RID: 125
	private ulong[] bytes = new ulong[1];

	// Token: 0x0400007E RID: 126
	private float[] progress = new float[1];

	// Token: 0x0400007F RID: 127
	private string myFile = "testLZ4.tif";

	// Token: 0x04000080 RID: 128
	private string uri = "https://dl.dropbox.com/s/r1ccmnreyd460vr/";

	// Token: 0x04000081 RID: 129
	private string ppath;

	// Token: 0x04000082 RID: 130
	private bool compressionStarted;

	// Token: 0x04000083 RID: 131
	private bool downloadDone;

	// Token: 0x04000084 RID: 132
	private byte[] buff;

	// Token: 0x04000085 RID: 133
	private byte[] fixedOutBuffer = new byte[786432];
}
