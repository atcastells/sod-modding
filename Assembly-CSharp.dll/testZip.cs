using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000025 RID: 37
public class testZip : MonoBehaviour
{
	// Token: 0x06000182 RID: 386 RVA: 0x0000B848 File Offset: 0x00009A48
	private void plog(string t = "")
	{
		this.log = this.log + t + "\n";
	}

	// Token: 0x06000183 RID: 387 RVA: 0x0000B864 File Offset: 0x00009A64
	private void Start()
	{
		this.ppath = Application.persistentDataPath;
		Debug.Log("persistentDataPath: " + this.ppath);
		this.reusableBuffer = new byte[4096];
		this.reusableBuffer2 = new byte[0];
		this.reusableBuffer3 = new byte[0];
		Screen.sleepTimeout = -1;
		base.StartCoroutine(this.DownloadZipFile());
	}

	// Token: 0x06000184 RID: 388 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Update()
	{
		if (Input.GetKeyDown(27))
		{
			Application.Quit();
		}
	}

	// Token: 0x06000185 RID: 389 RVA: 0x0000B8CC File Offset: 0x00009ACC
	private void OnGUI()
	{
		if (GUI.Button(new Rect((float)(Screen.width - 100), 90f, 80f, 40f), "Cancel"))
		{
			lzip.setCancel();
		}
		if (this.downloadDone)
		{
			GUI.Label(new Rect(10f, 2f, 250f, 30f), "package downloaded, ready to extract");
			GUI.Label(new Rect(310f, 2f, 650f, 100f), "Path: " + this.ppath);
		}
		if (this.compressionStarted || this.downloadDone2)
		{
			GUI.TextArea(new Rect(10f, 160f, (float)(Screen.width - 20), (float)(Screen.height - 170)), this.log);
			GUI.Label(new Rect((float)(Screen.width - 30), 0f, 80f, 40f), this.progress[0].ToString());
			GUI.Label(new Rect((float)(Screen.width - 140), 0f, 80f, 40f), this.progress2[0].ToString());
		}
		if (this.downloadDone)
		{
			if (GUI.Button(new Rect(10f, 40f, 110f, 50f), "Zip test"))
			{
				this.log = "";
				this.compressionStarted = true;
				this.DoDecompression();
			}
			if (GUI.Button(new Rect(130f, 40f, 110f, 50f), "FileBuffer test"))
			{
				this.log = "";
				this.compressionStarted = true;
				this.DoDecompression_FileBuffer();
			}
			if (GUI.Button(new Rect(10f, 100f, 180f, 50f), "Native FileBuffer tests"))
			{
				this.compressionStarted = true;
				this.downloadDone2 = false;
				base.StartCoroutine(this.NativeFileBufferDownload());
			}
			if (GUI.Button(new Rect(250f, 40f, 110f, 50f), "InMemory Test"))
			{
				this.log = "";
				this.compressionStarted = true;
				this.DoInMemoryTest();
			}
			if (GUI.Button(new Rect(370f, 40f, 110f, 50f), "Merged zip Test"))
			{
				this.log = "";
				this.compressionStarted = true;
				this.DoDecompression_Merged();
			}
			if (GUI.Button(new Rect(490f, 40f, 110f, 50f), "Gzip/Bz2 Test"))
			{
				this.log = "";
				this.compressionStarted = true;
				this.DoGzipBz2Tests();
			}
			if (GUI.Button(new Rect(610f, 40f, 110f, 50f), "Tar Test"))
			{
				this.log = "";
				this.compressionStarted = true;
				this.DoTarTests();
			}
		}
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0000BBC8 File Offset: 0x00009DC8
	private void DoDecompression()
	{
		this.plog("Validate: " + lzip.validateFile(this.ppath + "/testZip.zip", null).ToString());
		this.zres = lzip.decompress_File(this.ppath + "/testZip.zip", this.ppath + "/", this.progress, null, this.progress2, null);
		this.plog("decompress: " + this.zres.ToString());
		this.plog("");
		this.plog("true total files: " + lzip.getTotalFiles(this.ppath + "/testZip.zip", null).ToString());
		this.plog("true total entries: " + lzip.getTotalEntries(this.ppath + "/testZip.zip", null).ToString());
		this.plog("entry exists: " + lzip.entryExists(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", null).ToString());
		this.plog("");
		this.plog("DateTime: " + lzip.entryDateTime(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", null).ToString());
		this.zres = lzip.extract_entry(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", this.ppath + "/test22P.bmp", null, this.progress2, null);
		this.plog("extract entry: " + this.zres.ToString());
		this.plog("");
		List<string> list = new List<string>();
		list.Add("dir1/dir2/test2.bmp");
		list.Add("dir1/dir2/dir3/Unity_1.jpg");
		this.zres = lzip.extract_entries(this.ppath + "/testZip.zip", list.ToArray(), this.ppath + "/entries", null, this.progress2, null);
		this.plog("extract entries: " + this.zres.ToString());
		this.plog("");
		this.zres = lzip.compress_File(9, this.ppath + "/test2Zip.zip", this.ppath + "/dir1/dir2/test2.bmp", false, "dir1/dir2/test2.bmp", null, null, false, 0, this.byteProgress);
		this.plog("compress: " + this.zres.ToString());
		this.zres = lzip.compress_File(0, this.ppath + "/test2Zip.zip", this.ppath + "/dir1/dir2/dir3/Unity_1.jpg", true, "dir1/dir2/dir3/Unity_1.jpg", "ccc", null, false, 0, this.byteProgress);
		this.plog("append: " + this.zres.ToString() + "\nTotal bytes processed: " + this.byteProgress[0].ToString());
		this.byteProgress[0] = 0UL;
		lzip.getFileInfo(this.ppath + "/test2Zip.zip", null);
		int entryIndex = lzip.getEntryIndex("dir1/dir2/dir3/Unity_1.jpg");
		if (entryIndex != -1)
		{
			int num = (int)lzip.uinfo[entryIndex];
			int num2 = (int)lzip.localOffset[entryIndex] + 30 + lzip.ninfo[entryIndex].Length;
			this.plog("Real Offset: " + num2.ToString());
			byte[] array = new byte[num];
			using (BinaryReader binaryReader = new BinaryReader(new FileStream(this.ppath + "/test2Zip.zip", 3)))
			{
				binaryReader.BaseStream.Seek((long)num2, 0);
				binaryReader.Read(array, 0, num);
			}
			File.WriteAllBytes(this.ppath + "/Offset.jpg", array);
			array = null;
		}
		this.plog("");
		this.progress2[0] = 0UL;
		this.zres = lzip.compress_File(9, this.ppath + "/test2ZipSPAN.zip", this.ppath + "/dir1/dir2/test2.bmp", false, "dir1/dir2/test2.bmp", null, null, false, 20000, this.progress2);
		this.plog("compress SPAN: " + this.zres.ToString() + "  progress: " + this.progress2[0].ToString());
		this.zres = lzip.compress_File(9, this.ppath + "/test2ZipSPAN.zip", this.ppath + "/dir1/dir2/dir3/Unity_1.jpg", true, "dir1/dir2/dir3/Unity_1.jpg", null, null, false, 20000, this.progress2);
		this.plog("compress SPAN 2: " + this.zres.ToString() + "  progress: " + this.progress2[0].ToString());
		this.progress2[0] = 0UL;
		this.zres = lzip.decompress_File(this.ppath + "/test2ZipSPAN.zip", this.ppath + "/SPANNED/", this.progress, null, this.progress2, null);
		this.plog("decompress SPAN: " + this.zres.ToString() + "  progress: " + this.progress2[0].ToString());
		this.plog("");
		bool useBz = true;
		List<string> list2 = new List<string>();
		list2.Add(this.ppath + "/test22P.bmp");
		list2.Add(this.ppath + "/dir1/dir2/test2.bmp");
		List<string> list3 = new List<string>();
		list3.Add("NEW_test22P.bmp");
		list3.Add("dir13/dir23/New_test2.bmp");
		this.zres = lzip.compress_File_List(9, this.ppath + "/fileList.zip", list2.ToArray(), this.progress, false, list3.ToArray(), "password", useBz, 0, null);
		this.plog("MultiFile Compress password: " + this.zres.ToString());
		list2.Clear();
		list3.Clear();
		this.zres = lzip.decompress_File(this.ppath + "/fileList.zip", this.ppath + "/", this.progress, null, this.progress2, "password");
		this.plog("decompress password: " + this.zres.ToString());
		this.plog("");
		this.plog("Buffer2File: " + lzip.buffer2File(9, this.ppath + "/test3Zip.zip", "buffer.bin", this.reusableBuffer, false, null, null, false).ToString());
		this.plog("Buffer2File append: " + lzip.buffer2File(9, this.ppath + "/test3Zip.zip", "dir4/buffer.bin", this.reusableBuffer, true, null, null, false).ToString());
		this.plog("");
		this.plog("get entry size: " + lzip.getEntrySize(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", null).ToString());
		this.plog("");
		this.plog("entry2Buffer1: " + lzip.entry2Buffer(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", ref this.reusableBuffer2, null, null).ToString());
		this.plog("");
		this.plog("entry2FixedBuffer: " + lzip.entry2FixedBuffer(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", ref this.fixedBuffer, null, null).ToString());
		this.plog("");
		byte[] array2 = lzip.entry2Buffer(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", null, null);
		this.zres = 0;
		if (array2 != null)
		{
			this.zres = 1;
		}
		this.plog("entry2Buffer2: " + this.zres.ToString());
		this.plog("");
		int num3 = lzip.compressBufferFixed(array2, ref this.fixedInBuffer, 9, true);
		this.plog(" # Compress Fixed size Buffer: " + num3.ToString());
		if (num3 > 0)
		{
			int num4 = lzip.decompressBufferFixed(this.fixedInBuffer, ref this.fixedOutBuffer, true);
			if (num4 > 0)
			{
				this.plog(" # Decompress Fixed size Buffer: " + num4.ToString());
			}
		}
		this.plog("");
		this.pass = lzip.compressBuffer(this.reusableBuffer2, ref this.reusableBuffer3, 9);
		this.plog("compressBuffer1: " + this.pass.ToString());
		if (this.pass)
		{
			File.WriteAllBytes(this.ppath + "/out.bin", this.reusableBuffer3);
		}
		array2 = lzip.compressBuffer(this.reusableBuffer2, 9);
		this.zres = 0;
		if (array2 != null)
		{
			this.zres = 1;
		}
		this.plog("compressBuffer2: " + this.zres.ToString());
		this.plog("");
		this.pass = lzip.decompressBuffer(this.reusableBuffer3, ref this.reusableBuffer2);
		this.plog("decompressBuffer1: " + this.pass.ToString());
		if (this.pass)
		{
			File.WriteAllBytes(this.ppath + "/out.bmp", this.reusableBuffer2);
		}
		this.zres = 0;
		if (array2 != null)
		{
			this.zres = 1;
		}
		array2 = lzip.decompressBuffer(this.reusableBuffer3);
		if (array2 != null)
		{
			this.plog("decompressBuffer2: " + array2.Length.ToString());
		}
		else
		{
			this.plog("decompressBuffer2: Failed");
		}
		if (array2 != null)
		{
			File.WriteAllBytes(this.ppath + "/out2.bmp", array2);
		}
		this.plog("");
		this.plog("total bytes: " + lzip.getFileInfo(this.ppath + "/testZip.zip", null).ToString());
		if (lzip.ninfo != null)
		{
			for (int i = 0; i < lzip.ninfo.Count; i++)
			{
				this.log = string.Concat(new string[]
				{
					this.log,
					lzip.ninfo[i],
					" - ",
					lzip.uinfo[i].ToString(),
					" / ",
					lzip.cinfo[i].ToString(),
					"\n"
				});
			}
		}
		this.plog("");
		int[] array3 = new int[1];
		lzip.compressDir(this.ppath + "/dir1", 9, this.ppath + "/recursive.zip", false, array3, null, false, 0, false, null);
		this.plog("recursive - no. of files: " + array3[0].ToString());
		this.zres = lzip.decompress_File(this.ppath + "/recursive.zip", this.ppath + "/recursive/", this.progress, null, this.progress2, null);
		this.plog("decompress recursive: " + this.zres.ToString());
		new Thread(new ThreadStart(this.decompressFunc)).Start();
		if (File.Exists(this.ppath + "/test-Zip.zip"))
		{
			File.Delete(this.ppath + "/test-Zip.zip");
		}
		if (File.Exists(this.ppath + "/testZip.zip"))
		{
			File.Copy(this.ppath + "/testZip.zip", this.ppath + "/test-Zip.zip");
		}
		byte[] newFileBuffer = lzip.entry2Buffer(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", null, null);
		this.plog("replace entry: " + lzip.replace_entry(this.ppath + "/test-Zip.zip", "dir1/dir2/test2.bmp", newFileBuffer, 9, null, false).ToString());
		this.plog("replace entry 2: " + lzip.replace_entry(this.ppath + "/test-Zip.zip", "dir1/dir2/test2.bmp", this.ppath + "/dir1/dir2/test2.bmp", 9, null, null, false).ToString());
		this.plog("delete entry: " + lzip.delete_entry(this.ppath + "/test-Zip.zip", "dir1/dir2/test2.bmp").ToString());
	}

	// Token: 0x06000187 RID: 391 RVA: 0x0000C8A8 File Offset: 0x0000AAA8
	private void decompressFunc()
	{
		int num = lzip.decompress_File(this.ppath + "/recursive.zip", this.ppath + "/recursive/", this.progress, null, this.progress2, null);
		if (num == 1)
		{
			this.plog("multithreaded ok");
			return;
		}
		this.plog("multithreaded error: " + num.ToString());
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0000C910 File Offset: 0x0000AB10
	private IEnumerator NativeFileBufferDownload()
	{
		if (lzip.nativeBufferIsBeingUsed)
		{
			Debug.Log("Native buffer download is in use");
			yield break;
		}
		this.log = "";
		lzip.inMemory inMemZip = null;
		base.StartCoroutine(lzip.downloadZipFileNative("http://telias.free.fr/temp/" + this.myFile, delegate(bool r)
		{
			this.downloadDone2 = r;
		}, delegate(lzip.inMemory result)
		{
			inMemZip = result;
		}, null, null));
		this.plog("Downloading 1st file");
		this.plog("");
		while (!this.downloadDone2)
		{
			yield return true;
		}
		if (inMemZip.info == null)
		{
			this.log = "";
			this.plog("InMemZip is null. Please redownload to memory.");
		}
		else
		{
			this.plog("File size: " + inMemZip.size().ToString());
			this.zres = lzip.decompress_File(inMemZip.size().ToString(), this.ppath + "/native1/", this.progress, inMemZip.memoryPointer(), this.progress2, null);
			this.plog("Validate 1 " + lzip.validateFile(inMemZip.size().ToString(), inMemZip.memoryPointer()).ToString());
			this.plog("decompress native 1: " + this.zres.ToString());
			this.plog("");
			this.zres = lzip.decompress_File(null, this.ppath + "/native2/", this.progress, inMemZip, this.progress2, null);
			this.plog("Validate 2 " + lzip.validateFile(null, inMemZip).ToString());
			this.plog("decompress native 2: " + this.zres.ToString());
			this.plog("");
			List<string> list = new List<string>();
			list.Add("dir1/dir2/test2.bmp");
			list.Add("dir1/dir2/dir3/Unity_1.jpg");
			this.zres = lzip.extract_entries(inMemZip.size().ToString(), list.ToArray(), this.ppath + "/entriesNative", inMemZip.memoryPointer(), this.progress2, null);
			this.plog("extract entries: " + this.zres.ToString());
			this.plog("");
			lzip.free_inmemory(inMemZip);
		}
		this.downloadDone2 = false;
		IntPtr nativePointer = IntPtr.Zero;
		int zipSize = 0;
		this.plog("");
		this.plog("Downloading 2nd file");
		this.plog("");
		base.StartCoroutine(lzip.downloadZipFileNative("http://telias.free.fr/temp/" + this.myFile, delegate(bool r)
		{
			this.downloadDone2 = r;
		}, null, delegate(IntPtr pointerResult)
		{
			nativePointer = pointerResult;
		}, delegate(int size)
		{
			zipSize = size;
		}));
		while (!this.downloadDone2)
		{
			yield return true;
		}
		this.plog("File size: " + zipSize.ToString());
		this.zres = lzip.decompress_File(zipSize.ToString(), this.ppath + "/native3/", this.progress, nativePointer, this.progress2, null);
		this.plog("Validate 3 " + lzip.validateFile(zipSize.ToString(), nativePointer).ToString());
		this.plog("");
		this.plog("decompress native 3: " + this.zres.ToString());
		this.plog("");
		lzip.releaseBuffer(nativePointer);
		yield break;
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0000C920 File Offset: 0x0000AB20
	private void DoDecompression_FileBuffer()
	{
		byte[] fileBuffer = File.ReadAllBytes(this.ppath + "/testZip.zip");
		this.plog("Validate: " + lzip.validateFile(null, fileBuffer).ToString());
		this.zres = lzip.decompress_File(null, this.ppath + "/", this.progress, fileBuffer, this.progress2, null);
		this.plog("decompress: " + this.zres.ToString() + "  progress: " + this.progress2[0].ToString());
		this.plog("true total files: " + lzip.getTotalFiles(null, fileBuffer).ToString());
		this.plog("total entries: " + lzip.getTotalEntries(null, fileBuffer).ToString());
		this.plog("entry exists: " + lzip.entryExists(null, "dir1/dir2/test2.bmp", fileBuffer).ToString());
		this.zres = lzip.extract_entry(null, "dir1/dir2/test2.bmp", this.ppath + "/test22B.bmp", fileBuffer, this.progress2, null);
		this.plog("extract entry: " + this.zres.ToString() + "  progress: " + this.progress2[0].ToString());
		this.plog("");
		List<string> list = new List<string>();
		list.Add("dir1/dir2/test2.bmp");
		list.Add("dir1/dir2/dir3/Unity_1.jpg");
		this.zres = lzip.extract_entries(null, list.ToArray(), this.ppath + "/entriesFB", fileBuffer, this.progress2, null);
		this.plog("extract entries: " + this.zres.ToString());
		this.plog("");
		this.plog("get entry size: " + lzip.getEntrySize(null, "dir1/dir2/test2.bmp", fileBuffer).ToString());
		this.plog("");
		this.plog("entry2Buffer1: " + lzip.entry2Buffer(null, "dir1/dir2/test2.bmp", ref this.reusableBuffer2, fileBuffer, null).ToString());
		bool flag = lzip.entry2Buffer(null, "dir1/dir2/test2.bmp", fileBuffer, null) != null;
		this.zres = 0;
		if (flag)
		{
			this.zres = 1;
		}
		this.plog("entry2Buffer2: " + this.zres.ToString());
		this.plog("");
		this.plog("total bytes: " + lzip.getFileInfo(null, fileBuffer).ToString());
		if (lzip.ninfo != null)
		{
			for (int i = 0; i < lzip.ninfo.Count; i++)
			{
				this.log = string.Concat(new string[]
				{
					this.log,
					lzip.ninfo[i],
					" - ",
					lzip.uinfo[i].ToString(),
					" / ",
					lzip.cinfo[i].ToString(),
					"\n"
				});
			}
		}
		this.plog("");
	}

	// Token: 0x0600018A RID: 394 RVA: 0x0000CC54 File Offset: 0x0000AE54
	private void DoInMemoryTest()
	{
		if (!File.Exists(this.ppath + "/dir1/dir2/test2.bmp"))
		{
			lzip.decompress_File(this.ppath + "/testZip.zip", null, null, null, null, null);
		}
		if (this.reusableBuffer2.Length == 0)
		{
			lzip.entry2Buffer(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", ref this.reusableBuffer2, null, null);
		}
		lzip.inMemory inMemory = new lzip.inMemory();
		byte[] buffer = File.ReadAllBytes(this.ppath + "/dir1/dir2/dir3/Unity_1.jpg");
		lzip.compress_Buf2Mem(inMemory, 9, buffer, "inmem/Unity_1.jpg", null, "1234", true);
		this.plog("inMemory zip size: " + inMemory.size().ToString());
		lzip.compress_Buf2Mem(inMemory, 9, this.reusableBuffer2, "inmem/test.bmp", null, "1234", true);
		this.plog("inMemory zip size: " + inMemory.info[0].ToString());
		byte[] zipBuffer = inMemory.getZipBuffer();
		File.WriteAllBytes(this.ppath + "/MEM.zip", zipBuffer);
		this.progress2[0] = 0UL;
		this.plog("decompress_Mem2File: " + lzip.decompress_Mem2File(inMemory, this.ppath + "/", null, this.progress2, "1234").ToString() + "  progress: " + this.progress2[0].ToString());
		lzip.getFileInfoMem(inMemory);
		this.plog("");
		if (lzip.ninfo != null)
		{
			for (int i = 0; i < lzip.ninfo.Count; i++)
			{
				this.log = string.Concat(new string[]
				{
					this.log,
					lzip.ninfo[i],
					" - ",
					lzip.uinfo[i].ToString(),
					" / ",
					lzip.cinfo[i].ToString(),
					"\n"
				});
			}
		}
		this.plog("");
		byte[] array = null;
		this.plog("entry2BufferMem: " + lzip.entry2BufferMem(inMemory, "inmem/test.bmp", ref array, "1234").ToString());
		this.plog("entry2FixedBufferMem: " + lzip.entry2FixedBufferMem(inMemory, "inmem/test.bmp", ref array, "1234").ToString());
		byte[] array2 = lzip.entry2BufferMem(inMemory, "inmem/test.bmp", "1234");
		this.plog("entry2BufferMem new buffer: " + array2.Length.ToString());
		lzip.free_inmemory(inMemory);
		this.plog("");
		lzip.inMemory inMemory2 = new lzip.inMemory();
		lzip.inMemoryZipStart(inMemory2);
		buffer = File.ReadAllBytes(this.ppath + "/dir1/dir2/dir3/Unity_1.jpg");
		lzip.inMemoryZipAdd(inMemory2, 9, buffer, "test.jpg", null, null, false);
		lzip.inMemoryZipAdd(inMemory2, 9, this.reusableBuffer2, "directory/test.bmp", null, null, false);
		lzip.inMemoryZipClose(inMemory2);
		lzip.inMemoryZipStart(inMemory2);
		lzip.inMemoryZipAdd(inMemory2, 9, buffer, "newDir/test2.jpg", null, null, false);
		lzip.inMemoryZipAdd(inMemory2, 9, this.reusableBuffer2, "directory2/test2.bmp", null, null, false);
		lzip.inMemoryZipClose(inMemory2);
		this.plog("Size of Low Level inMemory zip: " + inMemory2.size().ToString());
		File.WriteAllBytes(this.ppath + "/MEM2.zip", inMemory2.getZipBuffer());
		lzip.free_inmemory(inMemory2);
		this.plog("");
	}

	// Token: 0x0600018B RID: 395 RVA: 0x0000CFF8 File Offset: 0x0000B1F8
	private void DoGzipBz2Tests()
	{
		if (this.reusableBuffer2.Length < 1)
		{
			lzip.entry2Buffer(this.ppath + "/testZip.zip", "dir1/dir2/test2.bmp", ref this.reusableBuffer2, null, null);
		}
		byte[] array = new byte[this.reusableBuffer2.Length + 18];
		int num = lzip.gzip(this.reusableBuffer2, array, 10, true, true, false);
		this.plog("gzip compressed size: " + num.ToString());
		byte[] array2 = new byte[num];
		Buffer.BlockCopy(array, 0, array2, 0, num);
		File.WriteAllBytes(this.ppath + "/test2.bmp.gz", array2);
		byte[] array3 = new byte[lzip.gzipUncompressedSize(array2)];
		int num2 = lzip.unGzip(array2, array3, true, true);
		if (num2 > 0)
		{
			File.WriteAllBytes(this.ppath + "/test2GZIP.bmp", array3);
			this.plog("gzip decompression: success " + num2.ToString());
		}
		else
		{
			this.plog("gzip decompression error: " + num2.ToString());
		}
		this.plog("");
		ulong[] array4 = new ulong[1];
		this.plog("Gzip file creation: " + lzip.gzipFile(this.ppath + "/test2GZIP.bmp", this.ppath + "/Ftest2GZIP.bmp.gz", 10, array4, true).ToString() + "  progress: " + array4[0].ToString());
		this.plog("Gzip file decompression: " + lzip.ungzipFile(this.ppath + "/Ftest2GZIP.bmp.gz", this.ppath + "/Ftest2GZIP.bmp", array4).ToString() + "  progress: " + array4[0].ToString());
		this.plog("");
		this.plog("bz2 creation: " + lzip.bz2Create(this.ppath + "/Ftest2GZIP.bmp", this.ppath + "/Ftest2GZIP.bmp.bz2", 9, array4).ToString() + "  progress: " + array4[0].ToString());
		this.plog("bz2 extract: " + lzip.bz2Decompress(this.ppath + "/Ftest2GZIP.bmp.bz2", this.ppath + "/Ftest2GZIP-Bz2.bmp", array4).ToString() + "  progress: " + array4[0].ToString());
		this.plog("");
	}

	// Token: 0x0600018C RID: 396 RVA: 0x0000D274 File Offset: 0x0000B474
	private void DoTarTests()
	{
		if (!Directory.Exists(this.ppath + "/mergedTests"))
		{
			this.DoDecompression_Merged();
		}
		this.log = "";
		ulong[] array = new ulong[]
		{
			0UL
		};
		lzip.setTarEncoding(65001U);
		this.plog("Create Tar: " + lzip.tarDir(this.ppath + "/mergedTests", this.ppath + "/out.tar", true, null, array).ToString());
		this.plog("processed: " + array[0].ToString());
		this.plog("");
		array[0] = 0UL;
		this.plog("Extract Tar: " + lzip.tarExtract(this.ppath + "/out.tar", this.ppath + "/tarOut", null, array).ToString());
		this.plog("processed: " + array[0].ToString());
		this.plog("");
		this.plog("Extract Tar entry: " + lzip.tarExtractEntry(this.ppath + "/out.tar", "mergedTests/dir1/dir2/test2.bmp", this.ppath + "/tarOut2", true, array).ToString());
		this.plog("Extract Tar entry absolute Path: " + lzip.tarExtractEntry(this.ppath + "/out.tar", "mergedTests/overriden.jpg", this.ppath + "/outTarAbsolute.jpeg", false, array).ToString());
		this.plog("");
		this.plog("tar.gz creation: " + lzip.gzipFile(this.ppath + "/out.tar", this.ppath + "/out.tar.gz", 10, null, true).ToString());
		this.plog("tar.bz2 creation: " + lzip.bz2Create(this.ppath + "/out.tar", this.ppath + "/out.tar.bz2", 9, null).ToString());
		this.plog("");
		lzip.getTarInfo(this.ppath + "/out.tar");
		if (lzip.ninfo != null && lzip.ninfo.Count > 0)
		{
			for (int i = 0; i < lzip.ninfo.Count; i++)
			{
				this.plog(string.Concat(new string[]
				{
					"Entry no: ",
					(i + 1).ToString(),
					"   ",
					lzip.ninfo[i],
					"  size: ",
					lzip.uinfo[i].ToString()
				}));
			}
		}
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0000D54C File Offset: 0x0000B74C
	private void DoDecompression_Merged()
	{
		if (!File.Exists(this.ppath + "/merged.jpg"))
		{
			if (!File.Exists(this.ppath + "/dir1/dir2/dir3/Unity_1.jpg"))
			{
				lzip.decompress_File(this.ppath + "/testZip.zip", this.ppath + "/", this.progress, null, this.progress2, null);
			}
			byte[] array = File.ReadAllBytes(this.ppath + "/dir1/dir2/dir3/Unity_1.jpg");
			byte[] array2 = File.ReadAllBytes(this.ppath + "/testZip.zip");
			byte[] array3 = new byte[array.Length + array2.Length];
			Array.Copy(array, 0, array3, 0, array.Length);
			Array.Copy(array2, 0, array3, array.Length, array2.Length);
			File.WriteAllBytes(this.ppath + "/merged.jpg", array3);
		}
		this.plog("Get Info of merged zip: " + lzip.getZipInfo(this.ppath + "/merged.jpg").ToString());
		if (lzip.zinfo != null && lzip.zinfo.Count > 0)
		{
			for (int i = 0; i < lzip.zinfo.Count; i++)
			{
				string[] array4 = new string[8];
				array4[0] = "Entry no: ";
				array4[1] = (i + 1).ToString();
				array4[2] = "   ";
				array4[3] = lzip.zinfo[i].filename;
				array4[4] = "  uncompressed: ";
				int num = 5;
				lzip.zipInfo zipInfo = lzip.zinfo[i];
				array4[num] = zipInfo.UncompressedSize.ToString();
				array4[6] = "  compressed: ";
				int num2 = 7;
				zipInfo = lzip.zinfo[i];
				array4[num2] = zipInfo.CompressedSize.ToString();
				this.plog(string.Concat(array4));
			}
		}
		this.plog("");
		int[] array5 = new int[1];
		ulong[] array6 = new ulong[1];
		this.plog("Decompress to disk from merged file: " + lzip.decompressZipMerged(this.ppath + "/merged.jpg", this.ppath + "/mergedTests/", array5, array6, null).ToString() + " progress: " + array6[0].ToString());
		this.plog("Extract entry to disk from merged file: " + lzip.entry2FileMerged(this.ppath + "/merged.jpg", "dir1/dir2/dir3/Unity_1.jpg", this.ppath + "/mergedTests", "overriden.jpg", null).ToString());
		this.plog("");
		byte[] buffer = File.ReadAllBytes(this.ppath + "/merged.jpg");
		this.plog("Get Info of merged zip in Buffer: " + lzip.getZipInfoMerged(buffer).ToString());
		if (lzip.zinfo != null && lzip.zinfo.Count > 0)
		{
			for (int j = 0; j < lzip.zinfo.Count; j++)
			{
				string[] array7 = new string[8];
				array7[0] = "Entry no: ";
				array7[1] = (j + 1).ToString();
				array7[2] = "   ";
				array7[3] = lzip.zinfo[j].filename;
				array7[4] = "  uncompressed: ";
				int num3 = 5;
				lzip.zipInfo zipInfo = lzip.zinfo[j];
				array7[num3] = zipInfo.UncompressedSize.ToString();
				array7[6] = "  compressed: ";
				int num4 = 7;
				zipInfo = lzip.zinfo[j];
				array7[num4] = zipInfo.CompressedSize.ToString();
				this.plog(string.Concat(array7));
			}
		}
		this.plog("");
		this.plog("Decompress to disk from merged buffer: " + lzip.decompressZipMerged(buffer, this.ppath + "/mergedTests/", array5, null, null).ToString());
		this.plog("Entry2File from merged buffer: " + lzip.entry2FileMerged(buffer, "dir1/dir2/dir3/Unity_1.jpg", this.ppath + "/mergedTests", null, null).ToString());
		this.plog("");
		byte[] array8 = lzip.entry2BufferMerged(this.ppath + "/merged.jpg", "dir1/dir2/dir3/Unity_1.jpg", null);
		this.plog("Size of entry in new buffer 1: " + array8.Length.ToString());
		byte[] array9 = new byte[11264];
		this.plog("Size of entry in fixed buffer 1: " + lzip.entry2FixedBufferMerged(this.ppath + "/merged.jpg", "dir1/dir2/dir3/Unity_1.jpg", ref array9, null).ToString());
		this.plog("");
		byte[] array10 = lzip.entry2BufferMerged(buffer, "dir1/dir2/dir3/Unity_1.jpg", null);
		this.plog("Size of entry in new buffer 2: " + array10.Length.ToString());
		this.plog("Size of entry in fixed buffer 2: " + lzip.entry2FixedBufferMerged(buffer, "dir1/dir2/dir3/Unity_1.jpg", ref array9, null).ToString());
		array9 = null;
	}

	// Token: 0x0600018E RID: 398 RVA: 0x0000DA4B File Offset: 0x0000BC4B
	private IEnumerator DownloadZipFile()
	{
		this.myFile = "testZip.zip";
		if (File.Exists(this.ppath + "/" + this.myFile))
		{
			this.downloadDone = true;
			yield break;
		}
		Debug.Log("starting download");
		using (UnityWebRequest www = UnityWebRequest.Get("https://dl.dropboxusercontent.com/s/xve34ldz3pqvmh1/" + this.myFile))
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

	// Token: 0x040000EA RID: 234
	private int zres;

	// Token: 0x040000EB RID: 235
	private string myFile;

	// Token: 0x040000EC RID: 236
	private string log;

	// Token: 0x040000ED RID: 237
	private string ppath;

	// Token: 0x040000EE RID: 238
	private bool compressionStarted;

	// Token: 0x040000EF RID: 239
	private bool pass;

	// Token: 0x040000F0 RID: 240
	private bool downloadDone;

	// Token: 0x040000F1 RID: 241
	private bool downloadDone2;

	// Token: 0x040000F2 RID: 242
	private byte[] reusableBuffer;

	// Token: 0x040000F3 RID: 243
	private byte[] reusableBuffer2;

	// Token: 0x040000F4 RID: 244
	private byte[] reusableBuffer3;

	// Token: 0x040000F5 RID: 245
	private byte[] fixedInBuffer = new byte[262144];

	// Token: 0x040000F6 RID: 246
	private byte[] fixedOutBuffer = new byte[786432];

	// Token: 0x040000F7 RID: 247
	private byte[] fixedBuffer = new byte[1048576];

	// Token: 0x040000F8 RID: 248
	private int[] progress = new int[1];

	// Token: 0x040000F9 RID: 249
	private ulong[] progress2 = new ulong[1];

	// Token: 0x040000FA RID: 250
	private ulong[] byteProgress = new ulong[1];
}
