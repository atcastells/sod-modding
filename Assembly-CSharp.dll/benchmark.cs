using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000002 RID: 2
public class benchmark : MonoBehaviour
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	private void Start()
	{
		this.ppath = Application.persistentDataPath;
		lzma.persitentDataPath = Application.persistentDataPath;
		Screen.sleepTimeout = -1;
		if (!File.Exists(this.ppath + "/" + this.myFile))
		{
			base.StartCoroutine(this.Download7ZFile());
		}
		else
		{
			this.downloadDone = true;
		}
		this.benchmarkStarted = false;
		this.style = new GUIStyle();
		this.style.richText = true;
		GUI.color = Color.black;
	}

	// Token: 0x06000002 RID: 2 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Update()
	{
		if (Input.GetKeyDown(27))
		{
			Application.Quit();
		}
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000020E4 File Offset: 0x000002E4
	private void OnGUI()
	{
		if (this.downloadDone && !this.benchmarkStarted && GUI.Button(new Rect(10f, 10f, 170f, 50f), "start Benchmark (48 mb)"))
		{
			this.benchmarkStarted = true;
			this.log = "";
			this.lzres = 0;
			this.zipres = 0;
			this.flzres = 0;
			this.lz4res = 0;
			base.StartCoroutine(this.decompressFunc());
		}
		GUI.TextArea(new Rect(10f, 70f, (float)(Screen.width - 20), (float)(Screen.height - 190)), this.log, this.style);
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002197 File Offset: 0x00000397
	private IEnumerator decompressFunc()
	{
		FileInfo fileInfo = new FileInfo(this.ppath + "/testimg2.7z");
		this.log = this.log + "<color=lime>decompressing 7zip ... <color=yellow>(" + ((float)fileInfo.Length / 1024f).ToString("F") + " kb)</color></color>";
		this.t1 = Time.realtimeSinceStartup;
		this.lzres = lzma.doDecompress7zip(this.ppath + "/" + this.myFile, this.ppath + "/", true, true, null, null);
		this.log = this.log + "  <color=white>(" + lzma.getBytesWritten().ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.lzres.ToString(),
			" |  7z time: </color><color=cyan>",
			this.tim.ToString(),
			"   sec</color>\n\n"
		});
		this.log += "<color=orange>compressing lzma ... </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		lzma.setProps(9, 65536, 3, 0, 2, 32, 2);
		if (File.Exists(this.ppath + "/" + this.uncFile + ".lzma"))
		{
			File.Delete(this.ppath + "/" + this.uncFile + ".lzma");
		}
		this.lzres = lzma.LzmaUtilEncode(this.ppath + "/" + this.uncFile, this.ppath + "/" + this.uncFile + ".lzma");
		this.log = this.log + "<color=white>(" + lzma.getBytesRead().ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		fileInfo = new FileInfo(this.ppath + "/" + this.uncFile + ".lzma");
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.lzres.ToString(),
			" |  lzma time: </color><color=teal>",
			this.tim.ToString(),
			"   sec</color>   <color=yellow>(",
			((float)fileInfo.Length / 1024f).ToString("F"),
			" kb)</color>\n\n"
		});
		this.log += "<color=lime>decompressing lzma alone ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.lzres = lzma.LzmaUtilDecode(this.ppath + "/" + this.uncFile + ".lzma", this.ppath + "/" + this.uncFile);
		this.log = this.log + "<color=white>(" + lzma.getBytesWritten().ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.lzres.ToString(),
			" |  lzma time: </color><color=cyan>",
			this.tim.ToString(),
			"   sec</color>\n\n"
		});
		this.log += "<color=orange>compressing zip ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		if (File.Exists(this.ppath + "/" + this.myFile2))
		{
			File.Delete(this.ppath + "/" + this.myFile2);
		}
		this.progress1[0] = 0UL;
		this.zipres = lzip.compress_File(9, this.ppath + "/" + this.myFile2, this.ppath + "/" + this.uncFile, false, null, null, null, false, 0, this.progress1);
		this.log = this.log + "<color=white>(" + this.progress1[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		fileInfo = new FileInfo(this.ppath + "/" + this.myFile2);
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.zipres.ToString(),
			" |  zip time: </color><color=teal>",
			this.tim.ToString(),
			"   sec</color>   <color=yellow>(",
			((float)fileInfo.Length / 1024f).ToString("F"),
			" kb)</color>\n\n"
		});
		this.log += "<color=lime>decompressing zip ... </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.zipres = lzip.decompress_File(this.ppath + "/" + this.myFile2, this.ppath + "/", this.progress, null, this.progress1, null);
		this.log = this.log + "<color=white>(" + this.progress1[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.zipres.ToString(),
			" |  zip time: </color><color=cyan>",
			this.tim.ToString(),
			"   sec</color>\n\n"
		});
		this.log += "<color=orange>Compressing to zip-bz2 ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		if (File.Exists(this.ppath + "/" + this.myFile2 + "bz2.zip"))
		{
			File.Delete(this.ppath + "/" + this.myFile2 + "bz2.zip");
		}
		this.progress1[0] = 0UL;
		this.zipres = lzip.compress_File(9, this.ppath + "/" + this.myFile2 + "bz2.zip", this.ppath + "/" + this.uncFile, false, null, null, null, true, 0, this.progress1);
		this.log = this.log + "<color=white>(" + this.progress1[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		fileInfo = new FileInfo(this.ppath + "/" + this.myFile2 + "bz2.zip");
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.zipres.ToString(),
			" |  zip-bz2 time: </color><color=teal>",
			this.tim.ToString(),
			"   sec</color>   <color=yellow>(",
			((float)fileInfo.Length / 1024f).ToString("F"),
			" kb)</color>\n\n"
		});
		this.log += "<color=lime>decompressing zip-bz2 ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.zipres = lzip.decompress_File(this.ppath + "/" + this.myFile2 + "bz2.zip", this.ppath + "/", this.progress, null, this.progress1, null);
		this.log = this.log + "<color=white>(" + this.progress1[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.zipres.ToString(),
			" |  zip-bz2 time: </color><color=cyan>",
			this.tim.ToString(),
			"   sec</color>\n\n"
		});
		this.log += "<color=orange>Compressing to flz ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.flzres = fLZ.compressFile(this.ppath + "/" + this.uncFile, this.ppath + "/" + this.uncFile + ".flz", 2, true, this.progress2);
		this.log = this.log + "<color=white>(" + this.progress2[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		fileInfo = new FileInfo(this.ppath + "/" + this.uncFile + ".flz");
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.flzres.ToString(),
			" |  flz time: </color><color=teal>",
			this.tim.ToString(),
			"   sec</color>   <color=yellow>(",
			((float)fileInfo.Length / 1024f).ToString("F"),
			" kb)</color>\n\n"
		});
		this.log += "<color=lime>Decompressing flz ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.flzres = fLZ.decompressFile(this.ppath + "/" + this.uncFile + ".flz", this.ppath + "/" + this.uncFile, true, this.progress2, null);
		this.log = this.log + "<color=white>(" + this.progress2[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.flzres.ToString(),
			" |  flz time: </color><color=cyan>",
			this.tim.ToString(),
			"   sec</color>\n\n"
		});
		this.log += "<color=orange>Compressing to LZ4 ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.lz4res = (int)LZ4.compress(this.ppath + "/" + this.uncFile, this.ppath + "/" + this.uncFile + ".lz4", 9, this.progress3);
		this.log = this.log + "<color=white>(" + this.progress3[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		fileInfo = new FileInfo(this.ppath + "/" + this.uncFile + ".lz4");
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.lz4res.ToString(),
			" |  LZ4 time: </color><color=teal>",
			this.tim.ToString(),
			"   sec</color>     <color=yellow>(",
			((float)fileInfo.Length / 1024f).ToString("F"),
			" kb)</color>\n\n"
		});
		this.log += "<color=lime>Decompressing LZ4 ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.lz4res = LZ4.decompress(this.ppath + "/" + this.uncFile + ".lz4", this.ppath + "/" + this.uncFile, this.bytes, null);
		this.log = this.log + "<color=white>(" + this.bytes[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.lz4res.ToString(),
			" |  LZ4 time: </color><color=cyan>",
			this.tim.ToString(),
			"   sec</color>  \n\n"
		});
		this.log += "<color=orange>Compressing to brotli ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.brres = brotli.compressFile(this.ppath + "/" + this.uncFile, this.ppath + "/" + this.uncFile + ".br", this.progress4, 9, 19, 0, 0);
		this.log = this.log + "<color=white>(" + this.progress4[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		fileInfo = new FileInfo(this.ppath + "/" + this.uncFile + ".br");
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.brres.ToString(),
			" |  brotli time: </color><color=teal>",
			this.tim.ToString(),
			"   sec</color>     <color=yellow>(",
			((float)fileInfo.Length / 1024f).ToString("F"),
			" kb)</color>\n\n"
		});
		this.log += "<color=lime>Decompressing brotli ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.progress4[0] = 0UL;
		this.brres = brotli.decompressFile(this.ppath + "/" + this.uncFile + ".br", this.ppath + "/" + this.uncFile, this.progress4, null);
		this.log = this.log + "<color=white>(" + this.progress4[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.brres.ToString(),
			" |  brotli time: </color><color=cyan>",
			this.tim.ToString(),
			"   sec</color>  \n\n"
		});
		this.log += "<color=lime>Compressing gzip ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.gzres = lzip.gzipFile(this.ppath + "/" + this.uncFile, this.ppath + "/" + this.uncFile + ".gz", 10, this.gzProgress, true);
		this.log = this.log + "<color=white>(" + this.gzProgress[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		fileInfo = new FileInfo(this.ppath + "/" + this.uncFile + ".gz");
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.gzres.ToString(),
			" |  gzip time: </color><color=teal>",
			this.tim.ToString(),
			"   sec</color>     <color=yellow>(",
			((float)fileInfo.Length / 1024f).ToString("F"),
			" kb)</color>\n\n"
		});
		this.log += "<color=lime>Decompressing gzip ...  </color>";
		yield return true;
		this.t1 = Time.realtimeSinceStartup;
		this.gzProgress[0] = 0UL;
		this.gzres = lzip.ungzipFile(this.ppath + "/" + this.uncFile + ".gz", this.ppath + "/" + this.uncFile, this.gzProgress);
		this.log = this.log + "<color=white>(" + this.gzProgress[0].ToString() + ")\n</color>";
		this.tim = Time.realtimeSinceStartup - this.t1;
		this.log = string.Concat(new string[]
		{
			this.log,
			"<color=white>status: ",
			this.brres.ToString(),
			" |  gzip time: </color><color=cyan>",
			this.tim.ToString(),
			"   sec</color>  \n\n"
		});
		yield return true;
		Debug.Log(lzma.setFilePermissions(this.ppath + "/" + this.uncFile, "rw", "r", "r"));
		Debug.Log(lzip.setFilePermissions(this.ppath + "/" + this.uncFile, "rw", "r", "r"));
		Debug.Log(fLZ.setFilePermissions(this.ppath + "/" + this.uncFile, "rw", "r", "r"));
		Debug.Log(LZ4.setFilePermissions(this.ppath + "/" + this.uncFile, "rw", "r", "r"));
		Debug.Log(brotli.setFilePermissions(this.ppath + "/" + this.uncFile, "rw", "r", "r"));
		this.benchmarkStarted = false;
		yield break;
	}

	// Token: 0x06000005 RID: 5 RVA: 0x000021A6 File Offset: 0x000003A6
	private IEnumerator Download7ZFile()
	{
		if (File.Exists(this.ppath + "/" + this.myFile))
		{
			File.Delete(this.ppath + "/" + this.myFile);
		}
		Debug.Log("downloading 7zip file");
		this.log += "<color=white>downloading 7zip file ...\n</color>";
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

	// Token: 0x04000001 RID: 1
	private int lzres;

	// Token: 0x04000002 RID: 2
	private int zipres;

	// Token: 0x04000003 RID: 3
	private int flzres;

	// Token: 0x04000004 RID: 4
	private int brres;

	// Token: 0x04000005 RID: 5
	private int lz4res;

	// Token: 0x04000006 RID: 6
	private int gzres;

	// Token: 0x04000007 RID: 7
	private bool pass1;

	// Token: 0x04000008 RID: 8
	private bool pass2;

	// Token: 0x04000009 RID: 9
	private float t1;

	// Token: 0x0400000A RID: 10
	private float tim;

	// Token: 0x0400000B RID: 11
	private string myFile = "testimg2.7z";

	// Token: 0x0400000C RID: 12
	private string myFile2 = "testimg.zip";

	// Token: 0x0400000D RID: 13
	private string uncFile = "testimg.tif";

	// Token: 0x0400000E RID: 14
	private string uri = "https://dl.dropbox.com/s/5r7tlkvff9ba04b/";

	// Token: 0x0400000F RID: 15
	private string ppath;

	// Token: 0x04000010 RID: 16
	private string log = "";

	// Token: 0x04000011 RID: 17
	private bool downloadDone;

	// Token: 0x04000012 RID: 18
	private bool benchmarkStarted;

	// Token: 0x04000013 RID: 19
	private long tsize;

	// Token: 0x04000014 RID: 20
	private GUIStyle style;

	// Token: 0x04000015 RID: 21
	private int[] progress = new int[1];

	// Token: 0x04000016 RID: 22
	private ulong[] progress1 = new ulong[1];

	// Token: 0x04000017 RID: 23
	private ulong[] progress2 = new ulong[1];

	// Token: 0x04000018 RID: 24
	private float[] progress3 = new float[1];

	// Token: 0x04000019 RID: 25
	private ulong[] progress4 = new ulong[1];

	// Token: 0x0400001A RID: 26
	private ulong[] bytes = new ulong[1];

	// Token: 0x0400001B RID: 27
	private ulong[] gzProgress = new ulong[1];
}
