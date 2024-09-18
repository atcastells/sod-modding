using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200001B RID: 27
public class SevenZipTest : MonoBehaviour
{
	// Token: 0x060000D7 RID: 215 RVA: 0x00006FE4 File Offset: 0x000051E4
	private void plog(string t = "")
	{
		this.log = this.log + t + "\n";
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00007000 File Offset: 0x00005200
	private void Start()
	{
		this.ppath = Application.persistentDataPath;
		lzma.persitentDataPath = Application.persistentDataPath;
		this.buff = new byte[0];
		Screen.sleepTimeout = -1;
		if (!File.Exists(this.ppath + "/" + this.myFile))
		{
			base.StartCoroutine(this.Download7ZFile());
			return;
		}
		this.downloadDone = true;
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Update()
	{
		if (Input.GetKeyDown(27))
		{
			Application.Quit();
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00007068 File Offset: 0x00005268
	private void OnGUI()
	{
		if (this.downloadDone)
		{
			GUI.Label(new Rect(50f, 5f, 350f, 30f), "package downloaded, ready to extract");
			GUI.Label(new Rect(350f, 5f, 450f, 40f), this.ppath);
			if (this.th != null)
			{
				GUI.Label(new Rect((float)(Screen.width - 90), 10f, 90f, 50f), this.fileProgress[0].ToString());
				GUI.Label(new Rect((float)(Screen.width - 90), 30f, 90f, 50f), lzma.getBytesWritten().ToString() + " : " + lzma.getBytesRead().ToString());
			}
			GUI.TextArea(new Rect(50f, 120f, (float)(Screen.width - 100), (float)(Screen.height - 135)), this.log);
			if (GUI.Button(new Rect(50f, 55f, 150f, 50f), "start 7z test"))
			{
				if (File.Exists(this.ppath + "/1.txt"))
				{
					File.Delete(this.ppath + "/1.txt");
				}
				if (File.Exists(this.ppath + "/2.txt"))
				{
					File.Delete(this.ppath + "/2.txt");
				}
				this.log = "";
				this.DoDecompression();
			}
			if (GUI.Button(new Rect(210f, 55f, 120f, 50f), "Lzma buffer tests"))
			{
				this.log = "";
				base.StartCoroutine(this.buff2buffTest());
			}
		}
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00007244 File Offset: 0x00005444
	private void DoDecompression()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		this.fileProgress[0] = 0;
		this.plog("7z return code: " + lzma.doDecompress7zip(this.ppath + "/" + this.myFile, this.ppath + "/", this.fileProgress, true, true, null, null).ToString());
		this.plog("Bytes Read: " + lzma.getBytesRead().ToString() + "  Bytes Written: " + lzma.getBytesWritten().ToString());
		this.plog("Headers size: " + lzma.getHeadersSize(this.ppath + "/" + this.myFile, null).ToString());
		ulong num = lzma.get7zSize(this.ppath + "/" + this.myFile, "1.txt", null);
		this.plog(string.Concat(new string[]
		{
			"Extract entry: ",
			lzma.doDecompress7zip(this.ppath + "/" + this.myFile, null, false, false, "1.txt", null).ToString(),
			" progress: ",
			(num / lzma.getBytesWritten() * 100f).ToString(),
			"%"
		}));
		this.tsize = lzma.get7zInfo(this.ppath + "/" + this.myFile, null);
		this.plog("Total Size: " + this.tsize.ToString() + "      trueTotalFiles: " + lzma.trueTotalFiles.ToString());
		if (lzma.ninfo != null)
		{
			for (int i = 0; i < lzma.ninfo.Count; i++)
			{
				this.plog(lzma.ninfo[i] + " - " + lzma.sinfo[i].ToString());
			}
		}
		this.plog("");
		this.plog("Uncompressed Size: " + lzma.get7zSize(this.ppath + "/" + this.myFile, "1.txt", null).ToString());
		lzma.setProps(9, 65536, 3, 0, 2, 32, 2);
		lzma.setProps(9, 65536, 3, 0, 2, 32, 2);
		int num2 = lzma.LzmaUtilEncode(this.ppath + "/1.txt", this.ppath + "/1.txt.lzma");
		if (num2 != 0)
		{
			this.plog("lzma encoded " + num2.ToString());
		}
		this.plog("bytes read: " + lzma.getBytesRead().ToString() + " / bytes written: " + lzma.getBytesWritten().ToString());
		num2 = lzma.LzmaUtilDecode(this.ppath + "/1.txt.lzma", this.ppath + "/1BCD.txt");
		if (num2 != 0)
		{
			this.plog("lzma decoded " + num2.ToString());
		}
		this.plog("bytes read: " + lzma.getBytesRead().ToString() + " / bytes written: " + lzma.getBytesWritten().ToString());
		byte[] array = lzma.decode2Buffer(this.ppath + "/" + this.myFile, "1.txt", null);
		if (array != null)
		{
			File.WriteAllBytes(this.ppath + "/1AAA.txt", array);
			if (array.Length != 0)
			{
				this.plog("Decode2Buffer Size: " + array.Length.ToString());
				this.plog("decoded to buffer: ok");
			}
		}
		this.th = new Thread(new ThreadStart(this.Decompress));
		this.th.Start();
		this.plog("time: " + (Time.realtimeSinceStartup - realtimeSinceStartup).ToString());
	}

	// Token: 0x060000DC RID: 220 RVA: 0x0000763C File Offset: 0x0000583C
	private void Decompress()
	{
		if (lzma.doDecompress7zip(this.ppath + "/" + this.myFile, this.ppath + "/", this.fileProgress, true, true, null, null) == 1)
		{
			this.plog("Multithreaded 7z decompression: ok");
		}
	}

	// Token: 0x060000DD RID: 221 RVA: 0x0000768C File Offset: 0x0000588C
	private IEnumerator Download7ZFile()
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
				this.log = "";
				File.WriteAllBytes(this.ppath + "/" + this.myFile, www.downloadHandler.data);
				Debug.Log("download done");
			}
		}
		UnityWebRequest www = null;
		yield break;
		yield break;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x0000769B File Offset: 0x0000589B
	private IEnumerator buff2buffTest()
	{
		this.plog("Downloading a file...");
		using (UnityWebRequest w = UnityWebRequest.Get("https://dl.dropbox.com/s/3e6i0mri2v3xfdy/google.jpg.lzma"))
		{
			yield return w.SendWebRequest();
			if (w.error == null)
			{
				if (lzma.decompressBuffer(w.downloadHandler.data, ref this.buff, true, 0) == 0)
				{
					this.plog("decompress Buffer: True");
					File.WriteAllBytes(this.ppath + "/google.jpg", this.buff);
				}
				else
				{
					this.plog("Error decompressing www.bytes to buffer");
				}
			}
			else
			{
				this.plog(w.error);
			}
		}
		UnityWebRequest w = null;
		yield return new WaitForSeconds(0.2f);
		if (File.Exists(this.ppath + "/google.jpg"))
		{
			byte[] inBuffer = File.ReadAllBytes(this.ppath + "/google.jpg");
			if (lzma.compressBuffer(inBuffer, ref this.buff, true))
			{
				this.plog("compress Buffer: True");
				File.WriteAllBytes(this.ppath + "/google.jpg.lzma", this.buff);
				this.plog("");
				this.plog("uncompressed size in lzma: " + BitConverter.ToUInt64(this.buff, 5).ToString());
				this.plog("lzma size: " + this.buff.Length.ToString());
			}
			else
			{
				this.plog("could not compress to buffer ...");
			}
			this.plog("");
			int num = lzma.compressBufferFixed(inBuffer, ref this.fixedInBuffer, true, true);
			this.plog(" #-> Compress Fixed size Buffer: " + num.ToString());
			if (num > 0)
			{
				int num2 = lzma.decompressBufferFixed(this.fixedInBuffer, ref this.fixedOutBuffer, true, true, 0);
				if (num2 > 0)
				{
					this.plog(" #-> Decompress Fixed size Buffer: " + num2.ToString());
				}
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x040000A8 RID: 168
	private string myFile = "test.7z";

	// Token: 0x040000A9 RID: 169
	private string uri = "https://dl.dropbox.com/s/16v2ng25fnagiwg/";

	// Token: 0x040000AA RID: 170
	private string ppath;

	// Token: 0x040000AB RID: 171
	private string log = "";

	// Token: 0x040000AC RID: 172
	private bool downloadDone;

	// Token: 0x040000AD RID: 173
	private ulong tsize;

	// Token: 0x040000AE RID: 174
	private byte[] buff;

	// Token: 0x040000AF RID: 175
	private byte[] fixedInBuffer = new byte[262144];

	// Token: 0x040000B0 RID: 176
	private byte[] fixedOutBuffer = new byte[262144];

	// Token: 0x040000B1 RID: 177
	private Thread th;

	// Token: 0x040000B2 RID: 178
	private int[] fileProgress = new int[1];
}
