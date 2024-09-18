using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000008 RID: 8
public class brotlitest : MonoBehaviour
{
	// Token: 0x06000036 RID: 54 RVA: 0x000041BC File Offset: 0x000023BC
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

	// Token: 0x06000037 RID: 55 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Update()
	{
		if (Input.GetKeyDown(27))
		{
			Application.Quit();
		}
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00004224 File Offset: 0x00002424
	private void OnGUI()
	{
		if (this.downloadDone)
		{
			GUI.Label(new Rect(50f, 0f, 350f, 30f), "package downloaded, ready to extract");
			GUI.Label(new Rect(50f, 30f, 450f, 90f), this.ppath);
			if (GUI.Button(new Rect(50f, 150f, 250f, 50f), "start brotli test"))
			{
				this.compressionStarted = true;
				this.lz1 = 0;
				this.lz2 = 0;
				this.progress[0] = 0UL;
				this.progress2[0] = 0UL;
				this.progress3[0] = 0UL;
				this.progress4[0] = 0UL;
				new Thread(new ThreadStart(this.DoTests)).Start();
			}
		}
		else if (this.downloadError)
		{
			GUI.Label(new Rect(50f, 150f, 250f, 50f), "Download Error!");
		}
		if (this.compressionStarted)
		{
			GUI.Label(new Rect(50f, 220f, 250f, 40f), "brotli Compress:    " + this.lz1.ToString() + "  : " + this.progress[0].ToString());
			GUI.Label(new Rect(50f, 260f, 250f, 40f), "brotli Decompress: " + this.lz2.ToString() + "  : " + this.progress2[0].ToString());
			GUI.Label(new Rect(50f, 300f, 250f, 40f), "Buffer Compress:    " + this.lz3.ToString() + "  : " + this.progress3[0].ToString());
			GUI.Label(new Rect(50f, 340f, 250f, 40f), "Buffer Decompress: " + this.lz4.ToString());
		}
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00004444 File Offset: 0x00002644
	private void DoTests()
	{
		this.lz1 = brotli.compressFile(this.ppath + "/" + this.myFile, this.ppath + "/" + this.myFile + ".br", this.progress, 9, 19, 0, 0);
		this.lz2 = brotli.decompressFile(this.ppath + "/" + this.myFile + ".br", this.ppath + "/" + this.myFile + ".Br.tif", this.progress2, null);
		if (File.Exists(this.ppath + "/" + this.myFile))
		{
			this.bt = File.ReadAllBytes(this.ppath + "/" + this.myFile);
			if (brotli.compressBuffer(this.bt, ref this.buff, this.progress3, true, 9, 19, 0, 0))
			{
				this.lz3 = 1;
				File.WriteAllBytes(this.ppath + "/buffer1.brbuf", this.buff);
			}
			this.bt2 = File.ReadAllBytes(this.ppath + "/buffer1.brbuf");
			if (brotli.decompressBuffer(this.bt2, ref this.buff, true, 0))
			{
				this.lz4 = 1;
				File.WriteAllBytes(this.ppath + "/buffer1.tif", this.buff);
			}
			int num = brotli.decompressBuffer(this.bt2, this.fixedOutBuffer, true, 0);
			if (num > 0)
			{
				Debug.Log(" # Decompress Fixed size Buffer: " + num.ToString());
			}
			byte[] array = brotli.decompressBuffer(this.bt2, true, 0);
			if (array != null)
			{
				File.WriteAllBytes(this.ppath + "/buffer1NEW.tif", array);
				Debug.Log(" # new Buffer: " + array.Length.ToString());
			}
		}
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00004627 File Offset: 0x00002827
	private IEnumerator DownloadTestFile()
	{
		if (File.Exists(this.ppath + "/" + this.myFile))
		{
			File.Delete(this.ppath + "/" + this.myFile);
		}
		Debug.Log("starting download");
		string text = this.uri + this.myFile;
		using (UnityWebRequest www = UnityWebRequest.Get(text))
		{
			yield return www.SendWebRequest();
			if (www.error != null)
			{
				Debug.Log(www.error);
				this.downloadError = true;
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

	// Token: 0x04000030 RID: 48
	private int lz1;

	// Token: 0x04000031 RID: 49
	private int lz2;

	// Token: 0x04000032 RID: 50
	private int lz3;

	// Token: 0x04000033 RID: 51
	private int lz4;

	// Token: 0x04000034 RID: 52
	private int fbuftest;

	// Token: 0x04000035 RID: 53
	private int nFbuftest;

	// Token: 0x04000036 RID: 54
	private ulong[] progress = new ulong[1];

	// Token: 0x04000037 RID: 55
	private ulong[] progress2 = new ulong[1];

	// Token: 0x04000038 RID: 56
	private ulong[] progress3 = new ulong[1];

	// Token: 0x04000039 RID: 57
	private ulong[] progress4 = new ulong[1];

	// Token: 0x0400003A RID: 58
	private string myFile = "testLZ4.tif";

	// Token: 0x0400003B RID: 59
	private string uri = "https://dl.dropbox.com/s/r1ccmnreyd460vr/";

	// Token: 0x0400003C RID: 60
	private string ppath;

	// Token: 0x0400003D RID: 61
	private bool compressionStarted;

	// Token: 0x0400003E RID: 62
	private bool downloadDone;

	// Token: 0x0400003F RID: 63
	private bool downloadError;

	// Token: 0x04000040 RID: 64
	private byte[] buff;

	// Token: 0x04000041 RID: 65
	private byte[] bt;

	// Token: 0x04000042 RID: 66
	private byte[] bt2;

	// Token: 0x04000043 RID: 67
	private byte[] fixedOutBuffer = new byte[2359296];
}
