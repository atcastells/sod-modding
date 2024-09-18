using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200000E RID: 14
public class fLZtest : MonoBehaviour
{
	// Token: 0x06000064 RID: 100 RVA: 0x00004FA8 File Offset: 0x000031A8
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

	// Token: 0x06000065 RID: 101 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Update()
	{
		if (Input.GetKeyDown(27))
		{
			Application.Quit();
		}
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00005010 File Offset: 0x00003210
	private void OnGUI()
	{
		if (this.downloadDone)
		{
			GUI.Label(new Rect(50f, 0f, 350f, 30f), "package downloaded, ready to extract");
			GUI.Label(new Rect(50f, 30f, 450f, 90f), this.ppath);
		}
		if (this.downloadDone && GUI.Button(new Rect(50f, 150f, 250f, 50f), "start fastLZ test"))
		{
			this.compressionStarted = true;
			new Thread(new ThreadStart(this.DoTests)).Start();
		}
		if (this.compressionStarted)
		{
			GUI.Label(new Rect(50f, 220f, 250f, 40f), "fLZ Compress:    " + this.lz1.ToString() + " bytes");
			GUI.Label(new Rect(300f, 220f, 120f, 40f), this.progress[0].ToString() + "%");
			GUI.Label(new Rect(50f, 260f, 250f, 40f), "fLZ Decompress: " + this.lz2.ToString());
			GUI.Label(new Rect(300f, 260f, 250f, 40f), this.progress2[0].ToString() + "%");
			GUI.Label(new Rect(50f, 300f, 250f, 40f), "Buffer Compress:    " + this.lz3.ToString());
			GUI.Label(new Rect(50f, 340f, 250f, 40f), "Buffer Decompress: " + this.lz4.ToString());
		}
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00005204 File Offset: 0x00003404
	private void DoTests()
	{
		this.lz1 = fLZ.compressFile(this.ppath + "/" + this.myFile, this.ppath + "/" + this.myFile + ".flz", 2, true, this.progress);
		this.lz2 = fLZ.decompressFile(this.ppath + "/" + this.myFile + ".flz", this.ppath + "/" + this.myFile + "B.tif", true, this.progress2, null);
		if (File.Exists(this.ppath + "/" + this.myFile))
		{
			if (fLZ.compressBuffer(File.ReadAllBytes(this.ppath + "/" + this.myFile), ref this.buff, 2, true))
			{
				this.lz3 = 1;
				File.WriteAllBytes(this.ppath + "/buffer1.flzbuf", this.buff);
			}
			byte[] inBuffer = File.ReadAllBytes(this.ppath + "/buffer1.flzbuf");
			if (fLZ.decompressBuffer(inBuffer, ref this.buff, true, 0))
			{
				this.lz4 = 1;
				File.WriteAllBytes(this.ppath + "/buffer1.tif", this.buff);
			}
			int num = fLZ.decompressBufferFixed(inBuffer, ref this.fixedOutBuffer, true, true, 0);
			if (num > 0)
			{
				Debug.Log(" # Decompress Fixed size Buffer: " + num.ToString());
			}
		}
	}

	// Token: 0x06000068 RID: 104 RVA: 0x0000537A File Offset: 0x0000357A
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

	// Token: 0x04000056 RID: 86
	private int lz1;

	// Token: 0x04000057 RID: 87
	private int lz2;

	// Token: 0x04000058 RID: 88
	private int lz3;

	// Token: 0x04000059 RID: 89
	private int lz4;

	// Token: 0x0400005A RID: 90
	private int fbuftest;

	// Token: 0x0400005B RID: 91
	private int nFbuftest;

	// Token: 0x0400005C RID: 92
	private ulong[] progress = new ulong[1];

	// Token: 0x0400005D RID: 93
	private ulong[] progress2 = new ulong[1];

	// Token: 0x0400005E RID: 94
	private string myFile = "testLZ4.tif";

	// Token: 0x0400005F RID: 95
	private string uri = "https://dl.dropbox.com/s/r1ccmnreyd460vr/";

	// Token: 0x04000060 RID: 96
	private string ppath;

	// Token: 0x04000061 RID: 97
	private bool compressionStarted;

	// Token: 0x04000062 RID: 98
	private bool downloadDone;

	// Token: 0x04000063 RID: 99
	private byte[] buff;

	// Token: 0x04000064 RID: 100
	private byte[] fixedOutBuffer = new byte[786432];
}
