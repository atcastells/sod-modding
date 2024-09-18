using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200001F RID: 31
public class lzip
{
	// Token: 0x060000F0 RID: 240
	[DllImport("libzipw")]
	public static extern void setTarEncoding(uint encoding);

	// Token: 0x060000F1 RID: 241
	[DllImport("libzipw")]
	public static extern void setEncoding(uint encoding);

	// Token: 0x060000F2 RID: 242
	[DllImport("libzipw", CharSet = 4)]
	internal static extern bool zipValidateFile([MarshalAs(21)] string zipArchive, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x060000F3 RID: 243
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipGetTotalFiles([MarshalAs(21)] string zipArchive, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x060000F4 RID: 244
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipGetTotalEntries([MarshalAs(21)] string zipArchive, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x060000F5 RID: 245
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipGetInfoA([MarshalAs(21)] string zipArchive, IntPtr total, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x060000F6 RID: 246
	[DllImport("libzipw", CharSet = 4)]
	internal static extern IntPtr zipGetInfo([MarshalAs(21)] string zipArchive, int size, IntPtr unc, IntPtr comp, IntPtr offs, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x060000F7 RID: 247
	[DllImport("libzipw")]
	public static extern void releaseBuffer(IntPtr buffer);

	// Token: 0x060000F8 RID: 248
	[DllImport("libzipw")]
	public static extern IntPtr createBuffer(int size);

	// Token: 0x060000F9 RID: 249
	[DllImport("libzipw")]
	private static extern void addToBuffer(IntPtr destination, int offset, IntPtr buffer, int len);

	// Token: 0x060000FA RID: 250
	[DllImport("libzipw", CharSet = 4)]
	internal static extern ulong zipGetEntrySize([MarshalAs(21)] string zipArchive, [MarshalAs(21)] string entry, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x060000FB RID: 251
	[DllImport("libzipw", CharSet = 4)]
	internal static extern bool zipEntryExists([MarshalAs(21)] string zipArchive, [MarshalAs(21)] string entry, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x060000FC RID: 252
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipCD(int levelOfCompression, [MarshalAs(21)] string zipArchive, [MarshalAs(21)] string inFilePath, [MarshalAs(21)] string fileName, [MarshalAs(21)] string comment, [MarshalAs(20)] string password, bool useBz2, int diskSize, IntPtr bprog);

	// Token: 0x060000FD RID: 253
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipCDList(int levelOfCompression, [MarshalAs(21)] string zipArchive, IntPtr filename, int arrayLength, IntPtr prog, IntPtr filenameForced, [MarshalAs(20)] string password, bool useBz2, int diskSize, IntPtr bprog);

	// Token: 0x060000FE RID: 254
	[DllImport("libzipw", CharSet = 4)]
	internal static extern bool zipBuf2File(int levelOfCompression, [MarshalAs(21)] string zipArchive, [MarshalAs(21)] string arcFilename, IntPtr buffer, int bufferSize, [MarshalAs(21)] string comment, [MarshalAs(20)] string password, bool useBz2);

	// Token: 0x060000FF RID: 255
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipDeleteFile([MarshalAs(21)] string zipArchive, [MarshalAs(21)] string arcFilename, [MarshalAs(21)] string tempArchive);

	// Token: 0x06000100 RID: 256
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipEntry2Buffer([MarshalAs(21)] string zipArchive, [MarshalAs(21)] string entry, IntPtr buffer, int bufferSize, IntPtr FileBuffer, int fileBufferLength, [MarshalAs(20)] string password);

	// Token: 0x06000101 RID: 257
	[DllImport("libzipw")]
	internal static extern IntPtr zipCompressBuffer(IntPtr source, int sourceLen, int levelOfCompression, ref int v);

	// Token: 0x06000102 RID: 258
	[DllImport("libzipw")]
	internal static extern IntPtr zipDecompressBuffer(IntPtr source, int sourceLen, ref int v);

	// Token: 0x06000103 RID: 259
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipEX([MarshalAs(21)] string zipArchive, [MarshalAs(21)] string outPath, IntPtr progress, IntPtr FileBuffer, int fileBufferLength, IntPtr proc, [MarshalAs(20)] string password);

	// Token: 0x06000104 RID: 260
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipEntry([MarshalAs(21)] string zipArchive, [MarshalAs(21)] string arcFilename, [MarshalAs(21)] string outpath, IntPtr FileBuffer, int fileBufferLength, IntPtr proc, [MarshalAs(20)] string password);

	// Token: 0x06000105 RID: 261
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipEntryList([MarshalAs(21)] string zipArchive, IntPtr outpath, IntPtr filename, int arrayLength, IntPtr FileBuffer, int fileBufferLength, IntPtr proc, [MarshalAs(20)] string password);

	// Token: 0x06000106 RID: 262
	[DllImport("libzipw", CharSet = 4)]
	internal static extern uint getEntryDateTime([MarshalAs(21)] string zipArchive, [MarshalAs(21)] string arcFilename, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x06000107 RID: 263
	[DllImport("libzipw")]
	internal static extern int freeMemStruct(IntPtr buffer);

	// Token: 0x06000108 RID: 264
	[DllImport("libzipw", CharSet = 4)]
	internal static extern IntPtr zipCDMem(IntPtr info, IntPtr pnt, int levelOfCompression, IntPtr source, int sourceLen, [MarshalAs(21)] string fileName, [MarshalAs(21)] string comment, [MarshalAs(20)] string password, bool useBz2);

	// Token: 0x06000109 RID: 265
	[DllImport("libzipw")]
	internal static extern IntPtr initMemStruct();

	// Token: 0x0600010A RID: 266
	[DllImport("libzipw")]
	internal static extern IntPtr initFileStruct();

	// Token: 0x0600010B RID: 267
	[DllImport("libzipw")]
	internal static extern int freeMemZ(IntPtr pointer);

	// Token: 0x0600010C RID: 268
	[DllImport("libzipw")]
	internal static extern int freeFileZ(IntPtr pointer);

	// Token: 0x0600010D RID: 269
	[DllImport("libzipw")]
	internal static extern IntPtr zipCDMemStart(IntPtr info, IntPtr pnt, IntPtr fileStruct, IntPtr memStruct);

	// Token: 0x0600010E RID: 270
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int zipCDMemAdd(IntPtr zf, int levelOfCompression, IntPtr source, int sourceLen, [MarshalAs(21)] string fileName, [MarshalAs(21)] string comment, [MarshalAs(20)] string password, bool useBz2);

	// Token: 0x0600010F RID: 271
	[DllImport("libzipw")]
	internal static extern IntPtr zipCDMemClose(IntPtr zf, IntPtr memStruct, IntPtr info, int err);

	// Token: 0x06000110 RID: 272
	[DllImport("libzipw")]
	internal static extern int zipGzip(IntPtr source, int sourceLen, IntPtr outBuffer, int levelOfCompression, bool addHeader, bool addFooter);

	// Token: 0x06000111 RID: 273
	[DllImport("libzipw")]
	internal static extern int zipUnGzip(IntPtr source, int sourceLen, IntPtr outBuffer, int outLen, bool hasHeader, bool hasFooter);

	// Token: 0x06000112 RID: 274
	[DllImport("libzipw")]
	internal static extern int zipUnGzip2(IntPtr source, int sourceLen, IntPtr outBuffer, int outLen);

	// Token: 0x06000113 RID: 275
	[DllImport("libzipw")]
	internal static extern int gzip_File([MarshalAs(21)] string inFile, [MarshalAs(21)] string outFile, int level, IntPtr progress, bool addHeader);

	// Token: 0x06000114 RID: 276
	[DllImport("libzipw")]
	internal static extern int ungzip_File([MarshalAs(21)] string inFile, [MarshalAs(21)] string outFile, IntPtr progress);

	// Token: 0x06000115 RID: 277
	[DllImport("libzipw")]
	public static extern void setCancel();

	// Token: 0x06000116 RID: 278
	[DllImport("libzipw", CharSet = 2)]
	internal static extern int readTarA([MarshalAs(21)] string zipArchive, IntPtr total);

	// Token: 0x06000117 RID: 279
	[DllImport("libzipw", CharSet = 2)]
	internal static extern IntPtr readTar([MarshalAs(21)] string zipArchive, int size, IntPtr unc);

	// Token: 0x06000118 RID: 280
	[DllImport("libzipw", CharSet = 2)]
	internal static extern int createTar([MarshalAs(21)] string outFile, IntPtr filePath, IntPtr filename, int arrayLength, IntPtr prog, IntPtr bprog);

	// Token: 0x06000119 RID: 281
	[DllImport("libzipw", CharSet = 2)]
	internal static extern int extractTar([MarshalAs(21)] string inFile, [MarshalAs(21)] string outDir, [MarshalAs(21)] string entry, IntPtr prog, IntPtr bprog, bool fullPaths);

	// Token: 0x0600011A RID: 282
	[DllImport("libzipw", CharSet = 4)]
	internal static extern int bz2(bool decompress, int level, [MarshalAs(21)] string inFile, [MarshalAs(21)] string outFile, IntPtr byteProgress);

	// Token: 0x0600011B RID: 283 RVA: 0x00003887 File Offset: 0x00001A87
	internal static GCHandle gcA(object o)
	{
		return GCHandle.Alloc(o, 3);
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00007BBC File Offset: 0x00005DBC
	private static bool checkObject(object o, string zipArchive, ref int len, ref IntPtr ptr)
	{
		if (o is IntPtr)
		{
			len = Convert.ToInt32(zipArchive);
			if (len <= 0)
			{
				return false;
			}
			ptr = (IntPtr)o;
		}
		else
		{
			lzip.inMemory inMemory = (lzip.inMemory)o;
			len = inMemory.info[0];
			ptr = inMemory.pointer;
		}
		return true;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00007C04 File Offset: 0x00005E04
	public static ulong getFileInfo(string zipArchive, object fileBuffer = null)
	{
		lzip.ninfo.Clear();
		lzip.uinfo.Clear();
		lzip.cinfo.Clear();
		lzip.localOffset.Clear();
		lzip.zipFiles = 0;
		lzip.zipFolders = 0;
		lzip.totalCompressedSize = 0UL;
		lzip.totalUncompressedSize = 0UL;
		int num = 0;
		int[] array = new int[1];
		GCHandle gchandle = lzip.gcA(array);
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array2 = (byte[])fileBuffer;
				GCHandle gchandle2 = lzip.gcA(array2);
				num = lzip.zipGetInfoA(null, gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), array2.Length);
				gchandle2.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					gchandle.Free();
					return 0UL;
				}
				num = lzip.zipGetInfoA(null, gchandle.AddrOfPinnedObject(), zero, fileBufferLength);
			}
		}
		else
		{
			num = lzip.zipGetInfoA(zipArchive, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
		}
		gchandle.Free();
		if (num <= 0)
		{
			return 0UL;
		}
		IntPtr intPtr = IntPtr.Zero;
		ulong[] array3 = new ulong[array[0]];
		ulong[] array4 = new ulong[array[0]];
		ulong[] array5 = new ulong[array[0]];
		GCHandle gchandle3 = lzip.gcA(array3);
		GCHandle gchandle4 = lzip.gcA(array4);
		GCHandle gchandle5 = lzip.gcA(array5);
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array6 = (byte[])fileBuffer;
				GCHandle gchandle6 = lzip.gcA(array6);
				intPtr = lzip.zipGetInfo(null, num, gchandle3.AddrOfPinnedObject(), gchandle4.AddrOfPinnedObject(), gchandle5.AddrOfPinnedObject(), gchandle6.AddrOfPinnedObject(), array6.Length);
				gchandle6.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero2 = IntPtr.Zero;
				int fileBufferLength2 = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength2, ref zero2))
				{
					gchandle3.Free();
					gchandle4.Free();
					gchandle5.Free();
					return 0UL;
				}
				intPtr = lzip.zipGetInfo(null, num, gchandle3.AddrOfPinnedObject(), gchandle4.AddrOfPinnedObject(), gchandle5.AddrOfPinnedObject(), zero2, fileBufferLength2);
			}
		}
		else
		{
			intPtr = lzip.zipGetInfo(zipArchive, num, gchandle3.AddrOfPinnedObject(), gchandle4.AddrOfPinnedObject(), gchandle5.AddrOfPinnedObject(), IntPtr.Zero, 0);
		}
		if (intPtr == IntPtr.Zero)
		{
			gchandle3.Free();
			gchandle4.Free();
			gchandle5.Free();
			return 0UL;
		}
		StringReader stringReader = new StringReader(Marshal.PtrToStringAuto(intPtr));
		ulong num2 = 0UL;
		for (int i = 0; i < array[0]; i++)
		{
			string text;
			if ((text = stringReader.ReadLine()) != null)
			{
				lzip.ninfo.Add(text);
			}
			if (array3 != null)
			{
				lzip.uinfo.Add(array3[i]);
				num2 += array3[i];
				if (array3[i] > 0UL)
				{
					lzip.zipFiles++;
				}
				else
				{
					lzip.zipFolders++;
				}
			}
			if (array4 != null)
			{
				lzip.cinfo.Add(array4[i]);
				lzip.totalCompressedSize += array4[i];
			}
			if (array5 != null)
			{
				lzip.localOffset.Add(array5[i]);
			}
		}
		stringReader.Close();
		stringReader.Dispose();
		gchandle3.Free();
		gchandle4.Free();
		gchandle5.Free();
		lzip.releaseBuffer(intPtr);
		lzip.totalUncompressedSize = num2;
		return num2;
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00007F34 File Offset: 0x00006134
	public static int getEntryIndex(string entry)
	{
		if (lzip.ninfo == null || lzip.ninfo.Count == 0)
		{
			return -1;
		}
		int result = -1;
		for (int i = 0; i < lzip.ninfo.Count; i++)
		{
			if (entry == lzip.ninfo[i])
			{
				result = i;
				break;
			}
		}
		return result;
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00007F88 File Offset: 0x00006188
	public static int getTotalFiles(string zipArchive, object fileBuffer = null)
	{
		int result = 0;
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle = lzip.gcA(array);
				result = lzip.zipGetTotalFiles(null, gchandle.AddrOfPinnedObject(), array.Length);
				gchandle.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return -3;
				}
				result = lzip.zipGetTotalFiles(null, zero, fileBufferLength);
			}
			return result;
		}
		return lzip.zipGetTotalFiles(zipArchive, IntPtr.Zero, 0);
	}

	// Token: 0x06000120 RID: 288 RVA: 0x0000800C File Offset: 0x0000620C
	public static int getTotalEntries(string zipArchive, object fileBuffer = null)
	{
		int result = 0;
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle = lzip.gcA(array);
				result = lzip.zipGetTotalEntries(null, gchandle.AddrOfPinnedObject(), array.Length);
				gchandle.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return -3;
				}
				result = lzip.zipGetTotalEntries(null, zero, fileBufferLength);
			}
			return result;
		}
		return lzip.zipGetTotalEntries(zipArchive, IntPtr.Zero, 0);
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00008090 File Offset: 0x00006290
	public static ulong getEntrySize(string zipArchive, string entry, object fileBuffer = null)
	{
		ulong result = 0UL;
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle = lzip.gcA(array);
				result = lzip.zipGetEntrySize(null, entry, gchandle.AddrOfPinnedObject(), array.Length);
				gchandle.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return 0UL;
				}
				result = lzip.zipGetEntrySize(null, entry, zero, fileBufferLength);
			}
			return result;
		}
		return lzip.zipGetEntrySize(zipArchive, entry, IntPtr.Zero, 0);
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00008118 File Offset: 0x00006318
	public static bool entryExists(string zipArchive, string entry, object fileBuffer = null)
	{
		bool result = false;
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle = lzip.gcA(array);
				result = lzip.zipEntryExists(null, entry, gchandle.AddrOfPinnedObject(), array.Length);
				gchandle.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return false;
				}
				result = lzip.zipEntryExists(null, entry, zero, fileBufferLength);
			}
			return result;
		}
		return lzip.zipEntryExists(zipArchive, entry, IntPtr.Zero, 0);
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00003884 File Offset: 0x00001A84
	public static int setFilePermissions(string filePath, string _user, string _group, string _other)
	{
		return -1;
	}

	// Token: 0x06000124 RID: 292 RVA: 0x000081A0 File Offset: 0x000063A0
	public static bool buffer2File(int levelOfCompression, string zipArchive, string arcFilename, byte[] buffer, bool append = false, string comment = null, string password = null, bool useBz2 = false)
	{
		if (!append && File.Exists(zipArchive))
		{
			File.Delete(zipArchive);
		}
		GCHandle gchandle = lzip.gcA(buffer);
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 9)
		{
			levelOfCompression = 9;
		}
		if (password == "")
		{
			password = null;
		}
		if (comment == "")
		{
			comment = null;
		}
		bool result = lzip.zipBuf2File(levelOfCompression, zipArchive, arcFilename, gchandle.AddrOfPinnedObject(), buffer.Length, comment, password, useBz2);
		gchandle.Free();
		return result;
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00008218 File Offset: 0x00006418
	public static int delete_entry(string zipArchive, string arcFilename)
	{
		string text = zipArchive + ".tmp";
		int num = lzip.zipDeleteFile(zipArchive, arcFilename, text);
		if (num > 0)
		{
			File.Delete(zipArchive);
			File.Move(text, zipArchive);
			return num;
		}
		if (File.Exists(text))
		{
			File.Delete(text);
		}
		return num;
	}

	// Token: 0x06000126 RID: 294 RVA: 0x0000825C File Offset: 0x0000645C
	public static int replace_entry(string zipArchive, string arcFilename, string newFilePath, int level = 9, string comment = null, string password = null, bool useBz2 = false)
	{
		if (lzip.delete_entry(zipArchive, arcFilename) < 0)
		{
			return -3;
		}
		if (password == "")
		{
			password = null;
		}
		if (comment == "")
		{
			comment = null;
		}
		return lzip.zipCD(level, zipArchive, newFilePath, arcFilename, comment, password, useBz2, 0, IntPtr.Zero);
	}

	// Token: 0x06000127 RID: 295 RVA: 0x000082AD File Offset: 0x000064AD
	public static int replace_entry(string zipArchive, string arcFilename, byte[] newFileBuffer, int level = 9, string password = null, bool useBz2 = false)
	{
		if (lzip.delete_entry(zipArchive, arcFilename) < 0)
		{
			return -5;
		}
		if (lzip.buffer2File(level, zipArchive, arcFilename, newFileBuffer, true, null, password, useBz2))
		{
			return 1;
		}
		return -6;
	}

	// Token: 0x06000128 RID: 296 RVA: 0x000082D4 File Offset: 0x000064D4
	public static int extract_entry(string zipArchive, string arcFilename, string outpath, object fileBuffer = null, ulong[] proc = null, string password = null)
	{
		if (!Directory.Exists(Path.GetDirectoryName(outpath)))
		{
			return -7;
		}
		int result = -1;
		if (proc == null)
		{
			proc = new ulong[1];
		}
		GCHandle gchandle = lzip.gcA(proc);
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle2 = lzip.gcA(array);
				result = lzip.zipEntry(null, arcFilename, outpath, gchandle2.AddrOfPinnedObject(), array.Length, gchandle.AddrOfPinnedObject(), password);
				gchandle2.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return -6;
				}
				result = lzip.zipEntry(null, arcFilename, outpath, zero, fileBufferLength, gchandle.AddrOfPinnedObject(), password);
			}
			gchandle.Free();
			return result;
		}
		result = lzip.zipEntry(zipArchive, arcFilename, outpath, IntPtr.Zero, 0, gchandle.AddrOfPinnedObject(), password);
		gchandle.Free();
		return result;
	}

	// Token: 0x06000129 RID: 297 RVA: 0x000083B0 File Offset: 0x000065B0
	public static int extract_entries(string zipArchive, string[] fileList, string outpath, object fileBuffer = null, ulong[] proc = null, string password = null)
	{
		string text = outpath.Replace("\\", "/");
		if (outpath.Substring(outpath.Length - 1) != "/")
		{
			text += "/";
		}
		if (!Directory.Exists(outpath))
		{
			Directory.CreateDirectory(outpath);
		}
		if (!Directory.Exists(outpath))
		{
			return -7;
		}
		if (fileList == null)
		{
			return -3;
		}
		int num = fileList.Length;
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = text + fileList[i];
		}
		int result = -1;
		if (proc == null)
		{
			proc = new ulong[1];
		}
		GCHandle gchandle = lzip.gcA(proc);
		IntPtr[] array2 = new IntPtr[num];
		IntPtr[] array3 = new IntPtr[num];
		for (int j = 0; j < num; j++)
		{
			array2[j] = Marshal.StringToCoTaskMemAuto(fileList[j]);
			array3[j] = Marshal.StringToCoTaskMemAuto(array[j]);
		}
		GCHandle gchandle2 = lzip.gcA(array2);
		GCHandle gchandle3 = lzip.gcA(array3);
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array4 = (byte[])fileBuffer;
				GCHandle gchandle4 = lzip.gcA(array4);
				result = lzip.zipEntryList(null, gchandle3.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), num, gchandle4.AddrOfPinnedObject(), array4.Length, gchandle.AddrOfPinnedObject(), password);
				gchandle4.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return -6;
				}
				result = lzip.zipEntryList(null, gchandle3.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), num, zero, fileBufferLength, gchandle.AddrOfPinnedObject(), password);
			}
			gchandle.Free();
			for (int k = 0; k < num; k++)
			{
				Marshal.FreeCoTaskMem(array2[k]);
				Marshal.FreeCoTaskMem(array3[k]);
			}
			gchandle2.Free();
			gchandle3.Free();
			return result;
		}
		result = lzip.zipEntryList(zipArchive, gchandle3.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), num, IntPtr.Zero, 0, gchandle.AddrOfPinnedObject(), password);
		gchandle.Free();
		for (int l = 0; l < num; l++)
		{
			Marshal.FreeCoTaskMem(array2[l]);
			Marshal.FreeCoTaskMem(array3[l]);
		}
		gchandle2.Free();
		gchandle3.Free();
		return result;
	}

	// Token: 0x0600012A RID: 298 RVA: 0x000085E8 File Offset: 0x000067E8
	public static int decompress_File(string zipArchive, string outPath = null, int[] progress = null, object fileBuffer = null, ulong[] proc = null, string password = null)
	{
		if (outPath == null)
		{
			outPath = Path.GetDirectoryName(zipArchive);
		}
		if (outPath.Substring(outPath.Length - 1, 1) != "/")
		{
			outPath += "/";
		}
		GCHandle gchandle = lzip.gcA(progress);
		if (proc == null)
		{
			proc = new ulong[1];
		}
		GCHandle gchandle2 = lzip.gcA(proc);
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle3 = lzip.gcA(array);
				int result = lzip.zipEX(null, outPath, gchandle.AddrOfPinnedObject(), gchandle3.AddrOfPinnedObject(), array.Length, gchandle2.AddrOfPinnedObject(), password);
				gchandle3.Free();
				gchandle.Free();
				gchandle2.Free();
				return result;
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					gchandle.Free();
					gchandle2.Free();
					return -6;
				}
				int result2 = lzip.zipEX(null, outPath, gchandle.AddrOfPinnedObject(), zero, fileBufferLength, gchandle2.AddrOfPinnedObject(), password);
				gchandle.Free();
				gchandle2.Free();
				return result2;
			}
		}
		int result3 = lzip.zipEX(zipArchive, outPath, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0, gchandle2.AddrOfPinnedObject(), password);
		gchandle.Free();
		gchandle2.Free();
		return result3;
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00008724 File Offset: 0x00006924
	public static int compress_File(int levelOfCompression, string zipArchive, string inFilePath, bool append = false, string fileName = "", string comment = null, string password = null, bool useBz2 = false, int diskSize = 0, ulong[] byteProgress = null)
	{
		if (!File.Exists(inFilePath))
		{
			return -10;
		}
		if (!append && File.Exists(zipArchive))
		{
			File.Delete(zipArchive);
		}
		if (fileName == null || fileName == "")
		{
			fileName = Path.GetFileName(inFilePath);
		}
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 9)
		{
			levelOfCompression = 9;
		}
		if (password == "")
		{
			password = null;
		}
		if (comment == "")
		{
			comment = null;
		}
		int result;
		if (byteProgress == null)
		{
			result = lzip.zipCD(levelOfCompression, zipArchive, inFilePath, fileName, comment, password, useBz2, diskSize, IntPtr.Zero);
		}
		else
		{
			GCHandle gchandle = lzip.gcA(byteProgress);
			result = lzip.zipCD(levelOfCompression, zipArchive, inFilePath, fileName, comment, password, useBz2, diskSize, gchandle.AddrOfPinnedObject());
			gchandle.Free();
		}
		return result;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x000087E4 File Offset: 0x000069E4
	public static int compress_File_List(int levelOfCompression, string zipArchive, string[] inFilePath, int[] progress = null, bool append = false, string[] fileName = null, string password = null, bool useBz2 = false, int diskSize = 0, ulong[] byteProgress = null)
	{
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 9)
		{
			levelOfCompression = 9;
		}
		if (password == "")
		{
			password = null;
		}
		if (!append && File.Exists(zipArchive))
		{
			File.Delete(zipArchive);
		}
		if (inFilePath == null)
		{
			return -3;
		}
		if (fileName != null && fileName.Length != inFilePath.Length)
		{
			return -4;
		}
		for (int i = 0; i < inFilePath.Length; i++)
		{
			if (!File.Exists(inFilePath[i]))
			{
				return -10;
			}
		}
		IntPtr[] array = new IntPtr[inFilePath.Length];
		IntPtr[] array2 = new IntPtr[inFilePath.Length];
		lzip.fillPointers(zipArchive, fileName, inFilePath, ref array, ref array2);
		if (byteProgress == null)
		{
			byteProgress = new ulong[1];
		}
		if (progress == null)
		{
			progress = new int[1];
		}
		GCHandle gchandle = lzip.gcA(array);
		GCHandle gchandle2 = lzip.gcA(array2);
		GCHandle gchandle3 = lzip.gcA(progress);
		GCHandle gchandle4 = lzip.gcA(byteProgress);
		int result = lzip.zipCDList(levelOfCompression, zipArchive, gchandle.AddrOfPinnedObject(), inFilePath.Length, gchandle3.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), password, useBz2, diskSize, gchandle4.AddrOfPinnedObject());
		for (int j = 0; j < inFilePath.Length; j++)
		{
			Marshal.FreeCoTaskMem(array[j]);
			Marshal.FreeCoTaskMem(array2[j]);
		}
		gchandle.Free();
		array = null;
		gchandle2.Free();
		array2 = null;
		gchandle3.Free();
		gchandle4.Free();
		return result;
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00008924 File Offset: 0x00006B24
	public static int compressDir(string sourceDir, int levelOfCompression, string zipArchive = null, bool includeRoot = false, int[] progress = null, string password = null, bool useBz2 = false, int diskSize = 0, bool append = false, ulong[] byteProgress = null)
	{
		if (!Directory.Exists(sourceDir))
		{
			return 0;
		}
		string text = sourceDir.Replace("\\", "/");
		if (sourceDir.Substring(sourceDir.Length - 1) != "/")
		{
			text += "/";
		}
		if (zipArchive == null)
		{
			zipArchive = sourceDir.Substring(0, sourceDir.Length) + ".zip";
		}
		if (lzip.getAllFiles(text) == 0)
		{
			return 0;
		}
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 9)
		{
			levelOfCompression = 9;
		}
		int result = 0;
		if (Directory.Exists(text))
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			lzip.fillLists(text, includeRoot, ref list, ref list2);
			result = lzip.compress_File_List(levelOfCompression, zipArchive, list.ToArray(), progress, append, list2.ToArray(), password, useBz2, diskSize, byteProgress);
			list.Clear();
			list = null;
			list2.Clear();
			list2 = null;
		}
		return result;
	}

	// Token: 0x0600012E RID: 302 RVA: 0x000089FC File Offset: 0x00006BFC
	private static void fillPointers(string outFile, string[] fileName, string[] inFilePath, ref IntPtr[] fp, ref IntPtr[] np)
	{
		string directoryName = Path.GetDirectoryName(outFile);
		string[] array;
		if (fileName == null)
		{
			array = new string[inFilePath.Length];
			for (int i = 0; i < inFilePath.Length; i++)
			{
				array[i] = inFilePath[i].Replace(directoryName, "");
			}
		}
		else
		{
			array = fileName;
		}
		for (int j = 0; j < inFilePath.Length; j++)
		{
			if (array[j] == null)
			{
				array[j] = inFilePath[j].Replace(directoryName, "");
			}
		}
		for (int k = 0; k < inFilePath.Length; k++)
		{
			inFilePath[k] = inFilePath[k].Replace("\\", "/");
			array[k] = array[k].Replace("\\", "/");
			fp[k] = Marshal.StringToCoTaskMemAuto(inFilePath[k]);
			np[k] = Marshal.StringToCoTaskMemAuto(array[k]);
		}
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00008AC4 File Offset: 0x00006CC4
	private static void fillLists(string fdir, bool includeRoot, ref List<string> inFilePath, ref List<string> fileName)
	{
		string[] array = fdir.Split('/', 0);
		string text = array[array.Length - 1];
		string text2 = text;
		if (array.Length > 1 && includeRoot)
		{
			text = (text2 = array[array.Length - 2] + "/");
		}
		foreach (string text3 in Directory.GetFiles(fdir, "*", 1))
		{
			string text4 = text3.Replace(fdir, text).Replace("\\", "/").Replace("//", "/");
			if (!includeRoot)
			{
				text4 = text4.Substring(text2.Length);
				if (text4.Substring(0, 1) == "/")
				{
					text4 = text4.Substring(1, text4.Length - 1);
				}
			}
			inFilePath.Add(text3);
			fileName.Add(text4);
		}
	}

	// Token: 0x06000130 RID: 304 RVA: 0x000067CA File Offset: 0x000049CA
	public static int getAllFiles(string dir)
	{
		return Directory.GetFiles(dir, "*", 1).Length;
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00008BA4 File Offset: 0x00006DA4
	public static long getFileSize(string file)
	{
		FileInfo fileInfo = new FileInfo(file);
		if (fileInfo.Exists)
		{
			return fileInfo.Length;
		}
		return 0L;
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00008BCC File Offset: 0x00006DCC
	public static ulong getDirSize(string dir)
	{
		string[] files = Directory.GetFiles(dir, "*", 1);
		ulong num = 0UL;
		for (int i = 0; i < files.Length; i++)
		{
			FileInfo fileInfo = new FileInfo(files[i]);
			if (fileInfo.Exists)
			{
				num += (ulong)fileInfo.Length;
			}
		}
		return num;
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00008C14 File Offset: 0x00006E14
	public static int tarExtract(string inFile, string outPath = null, int[] progress = null, ulong[] byteProgress = null)
	{
		if (outPath == null)
		{
			outPath = Path.GetDirectoryName(inFile);
		}
		if (outPath.Substring(outPath.Length - 1, 1) != "/")
		{
			outPath += "/";
		}
		GCHandle gchandle = lzip.gcA(progress);
		GCHandle gchandle2 = lzip.gcA(byteProgress);
		int result = lzip.extractTar(inFile, outPath, null, gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), true);
		gchandle.Free();
		gchandle2.Free();
		return result;
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00008C88 File Offset: 0x00006E88
	public static int tarExtractEntry(string inFile, string entry, string outPath = null, bool fullPaths = true, ulong[] byteProgress = null)
	{
		if (outPath == null)
		{
			outPath = Path.GetDirectoryName(inFile);
		}
		if (fullPaths && outPath.Substring(outPath.Length - 1, 1) != "/")
		{
			outPath += "/";
		}
		if (fullPaths && File.Exists(outPath))
		{
			Debug.Log("There is a file with the same name in the path!");
			return -7;
		}
		if (!fullPaths && Directory.Exists(outPath))
		{
			Debug.Log("There is a directory with the same name in the path!");
			return -8;
		}
		GCHandle gchandle = lzip.gcA(byteProgress);
		int result = lzip.extractTar(inFile, outPath, entry, IntPtr.Zero, gchandle.AddrOfPinnedObject(), fullPaths);
		gchandle.Free();
		return result;
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00008D20 File Offset: 0x00006F20
	public static int tarDir(string sourceDir, string outFile = null, bool includeRoot = false, int[] progress = null, ulong[] byteProgress = null)
	{
		if (!Directory.Exists(sourceDir))
		{
			return 0;
		}
		string text = sourceDir.Replace("\\", "/");
		if (sourceDir.Substring(sourceDir.Length - 1) != "/")
		{
			text += "/";
		}
		if (outFile == null)
		{
			outFile = sourceDir.Substring(0, sourceDir.Length - 1) + ".tar";
		}
		if (lzip.getAllFiles(text) == 0)
		{
			return 0;
		}
		int result = 0;
		if (Directory.Exists(text))
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			lzip.fillLists(text, includeRoot, ref list, ref list2);
			result = lzip.tarList(outFile, list.ToArray(), list2.ToArray(), progress, byteProgress);
			list.Clear();
			list = null;
			list2.Clear();
			list2 = null;
		}
		return result;
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00008DE0 File Offset: 0x00006FE0
	public static int tarList(string outFile, string[] inFilePath, string[] fileName = null, int[] progress = null, ulong[] byteProgress = null)
	{
		if (inFilePath == null)
		{
			return -3;
		}
		if (fileName != null && fileName.Length != inFilePath.Length)
		{
			return -4;
		}
		for (int i = 0; i < inFilePath.Length; i++)
		{
			if (!File.Exists(inFilePath[i]))
			{
				return -10;
			}
		}
		if (File.Exists(outFile))
		{
			File.Delete(outFile);
		}
		IntPtr[] array = new IntPtr[inFilePath.Length];
		IntPtr[] array2 = new IntPtr[inFilePath.Length];
		lzip.fillPointers(outFile, fileName, inFilePath, ref array, ref array2);
		GCHandle gchandle = lzip.gcA(array);
		GCHandle gchandle2 = lzip.gcA(array2);
		GCHandle gchandle3 = lzip.gcA(progress);
		GCHandle gchandle4 = lzip.gcA(byteProgress);
		int result = lzip.createTar(outFile, gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), inFilePath.Length, gchandle3.AddrOfPinnedObject(), gchandle4.AddrOfPinnedObject());
		for (int j = 0; j < inFilePath.Length; j++)
		{
			Marshal.FreeCoTaskMem(array[j]);
			Marshal.FreeCoTaskMem(array2[j]);
		}
		gchandle.Free();
		array = null;
		gchandle2.Free();
		array2 = null;
		gchandle3.Free();
		gchandle4.Free();
		return result;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00008EDC File Offset: 0x000070DC
	public static ulong getTarInfo(string tarArchive)
	{
		lzip.ninfo.Clear();
		lzip.uinfo.Clear();
		lzip.cinfo.Clear();
		lzip.localOffset.Clear();
		lzip.zipFiles = 0;
		lzip.zipFolders = 0;
		lzip.totalCompressedSize = 0UL;
		lzip.totalUncompressedSize = 0UL;
		int[] array = new int[1];
		GCHandle gchandle = lzip.gcA(array);
		int num = lzip.readTarA(tarArchive, gchandle.AddrOfPinnedObject());
		gchandle.Free();
		if (num <= 0)
		{
			return 0UL;
		}
		IntPtr intPtr = IntPtr.Zero;
		ulong[] array2 = new ulong[array[0]];
		GCHandle gchandle2 = lzip.gcA(array2);
		intPtr = lzip.readTar(tarArchive, num, gchandle2.AddrOfPinnedObject());
		if (intPtr == IntPtr.Zero)
		{
			gchandle2.Free();
			return 0UL;
		}
		StringReader stringReader = new StringReader(Marshal.PtrToStringAuto(intPtr));
		ulong num2 = 0UL;
		for (int i = 0; i < array[0]; i++)
		{
			string text;
			if ((text = stringReader.ReadLine()) != null)
			{
				lzip.ninfo.Add(text);
			}
			if (array2 != null)
			{
				lzip.uinfo.Add(array2[i]);
				num2 += array2[i];
				if (array2[i] > 0UL)
				{
					lzip.zipFiles++;
				}
				else
				{
					lzip.zipFolders++;
				}
			}
		}
		stringReader.Close();
		stringReader.Dispose();
		gchandle2.Free();
		lzip.releaseBuffer(intPtr);
		lzip.totalUncompressedSize = num2;
		return num2;
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000903C File Offset: 0x0000723C
	public static DateTime entryDateTime(string zipArchive, string entry, object fileBuffer = null)
	{
		uint num = 0U;
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle = lzip.gcA(array);
				num = lzip.getEntryDateTime(null, entry, gchandle.AddrOfPinnedObject(), array.Length);
				gchandle.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					Debug.Log("Error in getting DateTime: " + num.ToString());
					return DateTime.Now;
				}
				num = lzip.getEntryDateTime(null, entry, zero, fileBufferLength);
			}
		}
		else
		{
			num = lzip.getEntryDateTime(zipArchive, entry, IntPtr.Zero, 0);
		}
		uint num2 = (num & 4294901760U) >> 16;
		uint num3 = num & 65535U;
		uint num4 = (num2 >> 9) + 1980U;
		uint num5 = (num2 & 480U) >> 5;
		uint num6 = num2 & 31U;
		uint num7 = num3 >> 11;
		uint num8 = (num3 & 2016U) >> 5;
		uint num9 = (num3 & 31U) * 2U;
		if (num == 0U || num == 1U || num == 2U)
		{
			Debug.Log("Error in getting DateTime: " + num.ToString());
			return DateTime.Now;
		}
		return new DateTime((int)num4, (int)num5, (int)num6, (int)num7, (int)num8, (int)num9);
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00009160 File Offset: 0x00007360
	public static void free_inmemory(lzip.inMemory t)
	{
		if (t.info == null)
		{
			Debug.Log("inMemory object is null");
			return;
		}
		if (lzip.freeMemStruct(t.pointer) != 1)
		{
			Debug.Log("In memory pointer was not freed");
		}
		t.info = null;
		if (t.memStruct != IntPtr.Zero && lzip.freeMemZ(t.memStruct) != 1)
		{
			Debug.Log("MemStruct was not freed");
		}
		if (t.fileStruct != IntPtr.Zero && lzip.freeFileZ(t.fileStruct) != 1)
		{
			Debug.Log("FileStruct was not freed");
		}
		t = null;
	}

	// Token: 0x0600013A RID: 314 RVA: 0x000091F8 File Offset: 0x000073F8
	public static bool inMemoryZipStart(lzip.inMemory t)
	{
		if (t.info == null)
		{
			Debug.Log("inMemory object is null");
			return false;
		}
		if (t.fileStruct == IntPtr.Zero)
		{
			t.fileStruct = lzip.initFileStruct();
		}
		if (t.memStruct == IntPtr.Zero)
		{
			t.memStruct = lzip.initMemStruct();
		}
		if (!t.isClosed)
		{
			lzip.inMemoryZipClose(t);
		}
		GCHandle gchandle = lzip.gcA(t.info);
		t.zf = lzip.zipCDMemStart(gchandle.AddrOfPinnedObject(), t.pointer, t.fileStruct, t.memStruct);
		gchandle.Free();
		t.isClosed = false;
		return t.zf != IntPtr.Zero;
	}

	// Token: 0x0600013B RID: 315 RVA: 0x000092B8 File Offset: 0x000074B8
	public static int inMemoryZipAdd(lzip.inMemory t, int levelOfCompression, byte[] buffer, string fileName, string comment = null, string password = null, bool useBz2 = false)
	{
		if (t.info == null)
		{
			Debug.Log("inMemory object is null");
			return -1;
		}
		if (t.isClosed)
		{
			Debug.Log("Can't add entry. inMemory zip is closed.");
			return -2;
		}
		if (password == "")
		{
			password = null;
		}
		if (comment == "")
		{
			comment = null;
		}
		if (fileName == null)
		{
			fileName = "";
		}
		GCHandle gchandle = lzip.gcA(buffer);
		int num = lzip.zipCDMemAdd(t.zf, levelOfCompression, gchandle.AddrOfPinnedObject(), buffer.Length, fileName, comment, password, useBz2);
		gchandle.Free();
		t.lastResult = num;
		return num;
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00009350 File Offset: 0x00007550
	public static IntPtr inMemoryZipClose(lzip.inMemory t)
	{
		if (t.info == null)
		{
			Debug.Log("inMemory object is null");
			return IntPtr.Zero;
		}
		if (t.isClosed)
		{
			Debug.Log("Can't close zip. inMemory zip is closed.");
			return t.pointer;
		}
		GCHandle gchandle = lzip.gcA(t.info);
		t.pointer = lzip.zipCDMemClose(t.zf, t.memStruct, gchandle.AddrOfPinnedObject(), t.lastResult);
		gchandle.Free();
		t.isClosed = true;
		return t.pointer;
	}

	// Token: 0x0600013D RID: 317 RVA: 0x000093D4 File Offset: 0x000075D4
	public static IntPtr compress_Buf2Mem(lzip.inMemory t, int levelOfCompression, byte[] buffer, string fileName, string comment = null, string password = null, bool useBz2 = false)
	{
		if (t.info == null)
		{
			Debug.Log("inMemory object is null");
			return IntPtr.Zero;
		}
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 9)
		{
			levelOfCompression = 9;
		}
		if (password == "")
		{
			password = null;
		}
		if (comment == "")
		{
			comment = null;
		}
		if (fileName == null)
		{
			fileName = "";
		}
		if (buffer == null || buffer.Length == 0)
		{
			Debug.Log("Buffer was null or zero size !");
			return t.pointer;
		}
		GCHandle gchandle = lzip.gcA(buffer);
		GCHandle gchandle2 = lzip.gcA(t.info);
		t.pointer = lzip.zipCDMem(gchandle2.AddrOfPinnedObject(), t.pointer, levelOfCompression, gchandle.AddrOfPinnedObject(), buffer.Length, fileName, comment, password, useBz2);
		gchandle.Free();
		gchandle2.Free();
		return t.pointer;
	}

	// Token: 0x0600013E RID: 318 RVA: 0x000094A0 File Offset: 0x000076A0
	public static int decompress_Mem2File(lzip.inMemory t, string outPath, int[] progress = null, ulong[] proc = null, string password = null)
	{
		if (t.info == null)
		{
			Debug.Log("inMemory object is null");
			return -1;
		}
		if (outPath.Substring(outPath.Length - 1, 1) != "/")
		{
			outPath += "/";
		}
		GCHandle gchandle = lzip.gcA(progress);
		if (progress == null)
		{
			progress = new int[1];
		}
		if (proc == null)
		{
			proc = new ulong[1];
		}
		GCHandle gchandle2 = lzip.gcA(proc);
		if (t != null)
		{
			int result = lzip.zipEX(null, outPath, gchandle.AddrOfPinnedObject(), t.pointer, t.info[0], gchandle2.AddrOfPinnedObject(), password);
			gchandle.Free();
			gchandle2.Free();
			return result;
		}
		return 0;
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00009548 File Offset: 0x00007748
	public static int entry2BufferMem(lzip.inMemory t, string entry, ref byte[] buffer, string password = null)
	{
		if (t.info == null)
		{
			return -2;
		}
		int num = 0;
		if (password == "")
		{
			password = null;
		}
		if (t != null)
		{
			num = (int)lzip.zipGetEntrySize(null, entry, t.pointer, t.info[0]);
		}
		if (num <= 0)
		{
			return -18;
		}
		if (buffer == null)
		{
			buffer = new byte[0];
		}
		Array.Resize<byte>(ref buffer, num);
		GCHandle gchandle = lzip.gcA(buffer);
		int result = 0;
		if (t != null)
		{
			result = lzip.zipEntry2Buffer(null, entry, gchandle.AddrOfPinnedObject(), num, t.pointer, t.info[0], password);
		}
		gchandle.Free();
		return result;
	}

	// Token: 0x06000140 RID: 320 RVA: 0x000095DC File Offset: 0x000077DC
	public static byte[] entry2BufferMem(lzip.inMemory t, string entry, string password = null)
	{
		if (t.info == null)
		{
			return null;
		}
		int num = 0;
		if (password == "")
		{
			password = null;
		}
		if (t != null)
		{
			num = (int)lzip.zipGetEntrySize(null, entry, t.pointer, t.info[0]);
		}
		if (num <= 0)
		{
			return null;
		}
		byte[] array = new byte[num];
		GCHandle gchandle = lzip.gcA(array);
		int num2 = 0;
		if (t != null)
		{
			num2 = lzip.zipEntry2Buffer(null, entry, gchandle.AddrOfPinnedObject(), num, t.pointer, t.info[0], password);
		}
		gchandle.Free();
		if (num2 != 1)
		{
			return null;
		}
		return array;
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00009668 File Offset: 0x00007868
	public static int entry2FixedBufferMem(lzip.inMemory t, string entry, ref byte[] fixedBuffer, string password = null)
	{
		if (t.info == null)
		{
			return -2;
		}
		int num = 0;
		if (password == "")
		{
			password = null;
		}
		if (t != null)
		{
			num = (int)lzip.zipGetEntrySize(null, entry, t.pointer, t.info[0]);
		}
		if (num <= 0)
		{
			return -18;
		}
		if (fixedBuffer.Length < num)
		{
			return -19;
		}
		GCHandle gchandle = lzip.gcA(fixedBuffer);
		int num2 = 0;
		if (t != null)
		{
			num2 = lzip.zipEntry2Buffer(null, entry, gchandle.AddrOfPinnedObject(), num, t.pointer, t.info[0], password);
		}
		gchandle.Free();
		if (num2 != 1)
		{
			return num2;
		}
		return num;
	}

	// Token: 0x06000142 RID: 322 RVA: 0x000096F8 File Offset: 0x000078F8
	public static ulong getFileInfoMem(lzip.inMemory t)
	{
		if (t.info == null)
		{
			return 0UL;
		}
		lzip.ninfo.Clear();
		lzip.uinfo.Clear();
		lzip.cinfo.Clear();
		lzip.localOffset.Clear();
		lzip.zipFiles = 0;
		lzip.zipFolders = 0;
		lzip.totalCompressedSize = 0UL;
		lzip.totalUncompressedSize = 0UL;
		int num = 0;
		int[] array = new int[1];
		GCHandle gchandle = lzip.gcA(array);
		if (t != null)
		{
			num = lzip.zipGetInfoA(null, gchandle.AddrOfPinnedObject(), t.pointer, t.info[0]);
		}
		gchandle.Free();
		if (num <= 0)
		{
			return 0UL;
		}
		IntPtr intPtr = IntPtr.Zero;
		ulong[] array2 = new ulong[array[0]];
		ulong[] array3 = new ulong[array[0]];
		ulong[] array4 = new ulong[array[0]];
		GCHandle gchandle2 = lzip.gcA(array2);
		GCHandle gchandle3 = lzip.gcA(array3);
		GCHandle gchandle4 = lzip.gcA(array4);
		if (t != null)
		{
			intPtr = lzip.zipGetInfo(null, num, gchandle2.AddrOfPinnedObject(), gchandle3.AddrOfPinnedObject(), gchandle4.AddrOfPinnedObject(), t.pointer, t.info[0]);
		}
		if (intPtr == IntPtr.Zero)
		{
			gchandle2.Free();
			gchandle3.Free();
			gchandle4.Free();
			return 0UL;
		}
		StringReader stringReader = new StringReader(Marshal.PtrToStringAuto(intPtr));
		ulong num2 = 0UL;
		for (int i = 0; i < array[0]; i++)
		{
			string text;
			if ((text = stringReader.ReadLine()) != null)
			{
				lzip.ninfo.Add(text);
			}
			if (array2 != null)
			{
				lzip.uinfo.Add(array2[i]);
				num2 += array2[i];
				if (array2[i] > 0UL)
				{
					lzip.zipFiles++;
				}
				else
				{
					lzip.zipFolders++;
				}
			}
			if (array3 != null)
			{
				lzip.cinfo.Add(array3[i]);
				lzip.totalCompressedSize += array3[i];
			}
			if (array4 != null)
			{
				lzip.localOffset.Add(array4[i]);
			}
		}
		stringReader.Close();
		stringReader.Dispose();
		gchandle2.Free();
		gchandle3.Free();
		gchandle4.Free();
		lzip.releaseBuffer(intPtr);
		lzip.totalUncompressedSize = num2;
		return num2;
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00009918 File Offset: 0x00007B18
	public static int entry2Buffer(string zipArchive, string entry, ref byte[] buffer, object fileBuffer = null, string password = null)
	{
		int num = 0;
		if (password == "")
		{
			password = null;
		}
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle = lzip.gcA(array);
				num = (int)lzip.zipGetEntrySize(null, entry, gchandle.AddrOfPinnedObject(), array.Length);
				gchandle.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return -6;
				}
				num = (int)lzip.zipGetEntrySize(null, entry, zero, fileBufferLength);
			}
		}
		else
		{
			num = (int)lzip.zipGetEntrySize(zipArchive, entry, IntPtr.Zero, 0);
		}
		if (num <= 0)
		{
			return -18;
		}
		Array.Resize<byte>(ref buffer, num);
		GCHandle gchandle2 = lzip.gcA(buffer);
		int result = 0;
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array2 = (byte[])fileBuffer;
				GCHandle gchandle3 = lzip.gcA(array2);
				result = lzip.zipEntry2Buffer(null, entry, gchandle2.AddrOfPinnedObject(), num, gchandle3.AddrOfPinnedObject(), array2.Length, password);
				gchandle3.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero2 = IntPtr.Zero;
				int fileBufferLength2 = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength2, ref zero2))
				{
					gchandle2.Free();
					return -6;
				}
				result = lzip.zipEntry2Buffer(null, entry, gchandle2.AddrOfPinnedObject(), num, zero2, fileBufferLength2, password);
			}
		}
		else
		{
			result = lzip.zipEntry2Buffer(zipArchive, entry, gchandle2.AddrOfPinnedObject(), num, IntPtr.Zero, 0, password);
		}
		gchandle2.Free();
		return result;
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00009A78 File Offset: 0x00007C78
	public static int entry2FixedBuffer(string zipArchive, string entry, ref byte[] fixedBuffer, object fileBuffer = null, string password = null)
	{
		int num = 0;
		if (password == "")
		{
			password = null;
		}
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle = lzip.gcA(array);
				num = (int)lzip.zipGetEntrySize(null, entry, gchandle.AddrOfPinnedObject(), array.Length);
				gchandle.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return -6;
				}
				num = (int)lzip.zipGetEntrySize(null, entry, zero, fileBufferLength);
			}
		}
		else
		{
			num = (int)lzip.zipGetEntrySize(zipArchive, entry, IntPtr.Zero, 0);
		}
		if (num <= 0)
		{
			return -18;
		}
		if (fixedBuffer.Length < num)
		{
			return -19;
		}
		GCHandle gchandle2 = lzip.gcA(fixedBuffer);
		int num2 = 0;
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array2 = (byte[])fileBuffer;
				GCHandle gchandle3 = lzip.gcA(array2);
				num2 = lzip.zipEntry2Buffer(null, entry, gchandle2.AddrOfPinnedObject(), num, gchandle3.AddrOfPinnedObject(), array2.Length, password);
				gchandle3.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero2 = IntPtr.Zero;
				int fileBufferLength2 = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength2, ref zero2))
				{
					gchandle2.Free();
					return -6;
				}
				num2 = lzip.zipEntry2Buffer(null, entry, gchandle2.AddrOfPinnedObject(), num, zero2, fileBufferLength2, password);
			}
		}
		else
		{
			num2 = lzip.zipEntry2Buffer(zipArchive, entry, gchandle2.AddrOfPinnedObject(), num, IntPtr.Zero, 0, password);
		}
		gchandle2.Free();
		if (num2 != 1)
		{
			return num2;
		}
		return num;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00009BE4 File Offset: 0x00007DE4
	public static byte[] entry2Buffer(string zipArchive, string entry, object fileBuffer = null, string password = null)
	{
		int num = 0;
		if (password == "")
		{
			password = null;
		}
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle = lzip.gcA(array);
				num = (int)lzip.zipGetEntrySize(null, entry, gchandle.AddrOfPinnedObject(), array.Length);
				gchandle.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return null;
				}
				num = (int)lzip.zipGetEntrySize(null, entry, zero, fileBufferLength);
			}
		}
		else
		{
			num = (int)lzip.zipGetEntrySize(zipArchive, entry, IntPtr.Zero, 0);
		}
		if (num <= 0)
		{
			return null;
		}
		byte[] array2 = new byte[num];
		GCHandle gchandle2 = lzip.gcA(array2);
		int num2 = 0;
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array3 = (byte[])fileBuffer;
				GCHandle gchandle3 = lzip.gcA(array3);
				num2 = lzip.zipEntry2Buffer(null, entry, gchandle2.AddrOfPinnedObject(), num, gchandle3.AddrOfPinnedObject(), array3.Length, password);
				gchandle3.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero2 = IntPtr.Zero;
				int fileBufferLength2 = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength2, ref zero2))
				{
					gchandle2.Free();
					return null;
				}
				num2 = lzip.zipEntry2Buffer(null, entry, gchandle2.AddrOfPinnedObject(), num, zero2, fileBufferLength2, password);
			}
		}
		else
		{
			num2 = lzip.zipEntry2Buffer(zipArchive, entry, gchandle2.AddrOfPinnedObject(), num, IntPtr.Zero, 0, password);
		}
		gchandle2.Free();
		if (num2 != 1)
		{
			return null;
		}
		return array2;
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00009D48 File Offset: 0x00007F48
	public static bool validateFile(string zipArchive, object fileBuffer = null)
	{
		bool result = false;
		if (fileBuffer != null)
		{
			if (fileBuffer is byte[])
			{
				byte[] array = (byte[])fileBuffer;
				GCHandle gchandle = lzip.gcA(array);
				result = lzip.zipValidateFile(null, gchandle.AddrOfPinnedObject(), array.Length);
				gchandle.Free();
			}
			if (fileBuffer is IntPtr || fileBuffer is lzip.inMemory)
			{
				IntPtr zero = IntPtr.Zero;
				int fileBufferLength = 0;
				if (!lzip.checkObject(fileBuffer, zipArchive, ref fileBufferLength, ref zero))
				{
					return false;
				}
				result = lzip.zipValidateFile(null, zero, fileBufferLength);
			}
			return result;
		}
		return lzip.zipValidateFile(zipArchive, IntPtr.Zero, 0);
	}

	// Token: 0x06000147 RID: 327 RVA: 0x00009DCC File Offset: 0x00007FCC
	public static bool getZipInfo(string fileName)
	{
		if (!File.Exists(fileName))
		{
			Debug.Log("File not found: " + fileName);
			return false;
		}
		int num = 0;
		int num2 = 0;
		using (FileStream fileStream = File.OpenRead(fileName))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				if (lzip.findPK(binaryReader))
				{
					int num3 = lzip.findEnd(binaryReader, ref num, ref num2);
					if (num3 > 0)
					{
						lzip.getCentralDir(binaryReader, num3);
						return true;
					}
					Debug.Log("No Entries in zip");
					return false;
				}
			}
		}
		return false;
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00009E70 File Offset: 0x00008070
	public static bool getZipInfoMerged(string fileName, ref int pos, ref int size, bool getCentralDirectory = false)
	{
		if (!File.Exists(fileName))
		{
			Debug.Log("File not found: " + fileName);
			return false;
		}
		using (FileStream fileStream = File.OpenRead(fileName))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				if (lzip.findPK(binaryReader))
				{
					int num = lzip.findEnd(binaryReader, ref pos, ref size);
					if (num > 0)
					{
						if (getCentralDirectory)
						{
							lzip.getCentralDir(binaryReader, num);
						}
						return true;
					}
					Debug.Log("No Entries in zip");
					return false;
				}
			}
		}
		return false;
	}

	// Token: 0x06000149 RID: 329 RVA: 0x00009F0C File Offset: 0x0000810C
	public static bool getZipInfoMerged(byte[] buffer, ref int pos, ref int size, bool getCentralDirectory = false)
	{
		if (buffer == null)
		{
			Debug.Log("Buffer is null");
			return false;
		}
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				if (lzip.findPK(binaryReader))
				{
					int num = lzip.findEnd(binaryReader, ref pos, ref size);
					if (num > 0)
					{
						if (getCentralDirectory)
						{
							lzip.getCentralDir(binaryReader, num);
						}
						return true;
					}
					Debug.Log("No Entries in zip");
					return false;
				}
			}
		}
		return false;
	}

	// Token: 0x0600014A RID: 330 RVA: 0x00009F9C File Offset: 0x0000819C
	public static bool getZipInfoMerged(byte[] buffer)
	{
		if (buffer == null)
		{
			Debug.Log("Buffer is null");
			return false;
		}
		int num = 0;
		int num2 = 0;
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				if (lzip.findPK(binaryReader))
				{
					int num3 = lzip.findEnd(binaryReader, ref num, ref num2);
					if (num3 > 0)
					{
						lzip.getCentralDir(binaryReader, num3);
						return true;
					}
					Debug.Log("No Entries in zip");
					return false;
				}
			}
		}
		return false;
	}

	// Token: 0x0600014B RID: 331 RVA: 0x0000A038 File Offset: 0x00008238
	private static bool findPK(BinaryReader reader)
	{
		byte b = reader.ReadByte();
		bool result = false;
		int num = 0;
		while (reader.BaseStream.Position < reader.BaseStream.Length - 3L)
		{
			num++;
			if (b == 80)
			{
				if (reader.ReadByte() == 75 && reader.ReadByte() == 5 && reader.ReadByte() == 6)
				{
					reader.BaseStream.Seek(reader.BaseStream.Position - 4L, 0);
					result = true;
					break;
				}
				reader.BaseStream.Seek((long)num, 0);
			}
			b = reader.ReadByte();
		}
		return result;
	}

	// Token: 0x0600014C RID: 332 RVA: 0x0000A0C8 File Offset: 0x000082C8
	private static int findEnd(BinaryReader reader, ref int pos, ref int size)
	{
		long position = reader.BaseStream.Position;
		int num = 0;
		while (num == 0 && reader.BaseStream.Position < reader.BaseStream.Length)
		{
			byte b = reader.ReadByte();
			while (b != 80 && reader.BaseStream.Position < reader.BaseStream.Length)
			{
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position >= reader.BaseStream.Length)
			{
				break;
			}
			if (reader.ReadByte() == 75 && reader.ReadByte() == 5 && reader.ReadByte() == 6)
			{
				reader.ReadInt16();
				reader.ReadInt16();
				reader.ReadInt16();
				num = (int)reader.ReadInt16();
				int num2 = reader.ReadInt32();
				int num3 = reader.ReadInt32();
				int num4 = (int)reader.ReadInt16();
				reader.ReadBytes(num4);
				pos = (int)reader.BaseStream.Position - (num3 + num2 + 22);
				size = (int)reader.BaseStream.Position - pos;
				reader.BaseStream.Seek((long)(pos + num3), 0);
				break;
			}
		}
		return num;
	}

	// Token: 0x0600014D RID: 333 RVA: 0x0000A1E4 File Offset: 0x000083E4
	private static void getCentralDir(BinaryReader reader, int count)
	{
		if (lzip.zinfo != null && lzip.zinfo.Count > 0)
		{
			lzip.zinfo.Clear();
		}
		lzip.zinfo = new List<lzip.zipInfo>();
		for (int i = 0; i < count; i++)
		{
			if (reader.ReadInt32() == 33639248)
			{
				lzip.zipInfo zipInfo = default(lzip.zipInfo);
				zipInfo.VersionMadeBy = reader.ReadInt16();
				zipInfo.MinimumVersionToExtract = reader.ReadInt16();
				zipInfo.BitFlag = reader.ReadInt16();
				zipInfo.CompressionMethod = reader.ReadInt16();
				zipInfo.FileLastModificationTime = reader.ReadInt16();
				zipInfo.FileLastModificationDate = reader.ReadInt16();
				zipInfo.CRC = reader.ReadInt32();
				zipInfo.CompressedSize = reader.ReadInt32();
				zipInfo.UncompressedSize = reader.ReadInt32();
				short num = reader.ReadInt16();
				short num2 = reader.ReadInt16();
				short num3 = reader.ReadInt16();
				zipInfo.DiskNumberWhereFileStarts = reader.ReadInt16();
				zipInfo.InternalFileAttributes = reader.ReadInt16();
				zipInfo.ExternalFileAttributes = reader.ReadInt32();
				zipInfo.RelativeOffsetOfLocalFileHeader = reader.ReadInt32();
				zipInfo.filename = Encoding.UTF8.GetString(reader.ReadBytes((int)num));
				zipInfo.AbsoluteOffsetOfLocalFileHeaderStore = zipInfo.RelativeOffsetOfLocalFileHeader + 30 + zipInfo.filename.Length;
				byte[] array = reader.ReadBytes((int)num2);
				zipInfo.extraField = Encoding.ASCII.GetString(array);
				zipInfo.fileComment = Encoding.UTF8.GetString(reader.ReadBytes((int)num3));
				lzip.zinfo.Add(zipInfo);
			}
		}
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000A378 File Offset: 0x00008578
	public static byte[] getMergedZip(string filePath, ref int position, ref int siz)
	{
		int num = 0;
		int num2 = 0;
		if (!File.Exists(filePath))
		{
			return null;
		}
		lzip.getZipInfoMerged(filePath, ref num, ref num2, false);
		position = num;
		siz = num2;
		if (num2 == 0)
		{
			return null;
		}
		byte[] array = new byte[num2];
		using (FileStream fileStream = File.OpenRead(filePath))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				binaryReader.BaseStream.Seek((long)num, 0);
				binaryReader.Read(array, 0, num2);
			}
		}
		return array;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000A410 File Offset: 0x00008610
	public static byte[] getMergedZip(string filePath)
	{
		int num = 0;
		int num2 = 0;
		if (!File.Exists(filePath))
		{
			return null;
		}
		lzip.getZipInfoMerged(filePath, ref num, ref num2, false);
		if (num2 == 0)
		{
			return null;
		}
		byte[] array = new byte[num2];
		using (FileStream fileStream = File.OpenRead(filePath))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				binaryReader.BaseStream.Seek((long)num, 0);
				binaryReader.Read(array, 0, num2);
			}
		}
		return array;
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000A4A4 File Offset: 0x000086A4
	public static byte[] getMergedZip(byte[] buffer, ref int position, ref int siz)
	{
		int num = 0;
		int num2 = 0;
		if (buffer == null)
		{
			return null;
		}
		lzip.getZipInfoMerged(buffer, ref num, ref num2, false);
		position = num;
		siz = num2;
		if (num2 == 0)
		{
			return null;
		}
		byte[] array = new byte[num2];
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				binaryReader.BaseStream.Seek((long)num, 0);
				binaryReader.Read(array, 0, num2);
			}
		}
		return array;
	}

	// Token: 0x06000151 RID: 337 RVA: 0x0000A538 File Offset: 0x00008738
	public static byte[] getMergedZip(byte[] buffer)
	{
		int num = 0;
		int num2 = 0;
		if (buffer == null)
		{
			return null;
		}
		lzip.getZipInfoMerged(buffer, ref num, ref num2, false);
		if (num2 == 0)
		{
			return null;
		}
		byte[] array = new byte[num2];
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				binaryReader.BaseStream.Seek((long)num, 0);
				binaryReader.Read(array, 0, num2);
			}
		}
		return array;
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000A5C8 File Offset: 0x000087C8
	public static int decompressZipMerged(string file, string outPath, int[] progress = null, ulong[] proc = null, string password = null)
	{
		if (!File.Exists(file))
		{
			return 0;
		}
		outPath = outPath.Replace("//", "/");
		if (!Directory.Exists(outPath))
		{
			Directory.CreateDirectory(outPath);
		}
		int num = 0;
		int num2 = 0;
		int result = 0;
		byte[] mergedZip = lzip.getMergedZip(file, ref num, ref num2);
		if (mergedZip != null)
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(mergedZip);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64());
			inMemory.info[0] = num2;
			result = lzip.decompress_Mem2File(inMemory, outPath, progress, proc, password);
			gchandle.Free();
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
		}
		return result;
	}

	// Token: 0x06000153 RID: 339 RVA: 0x0000A66C File Offset: 0x0000886C
	public static int decompressZipMerged(byte[] buffer, string outPath, int[] progress = null, ulong[] proc = null, string password = null)
	{
		if (buffer == null)
		{
			return 0;
		}
		outPath = outPath.Replace("//", "/");
		if (!Directory.Exists(outPath))
		{
			Directory.CreateDirectory(outPath);
		}
		int num = 0;
		int num2 = 0;
		int result = 0;
		if (lzip.getZipInfoMerged(buffer, ref num, ref num2, false))
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(buffer);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + (long)num);
			inMemory.info[0] = num2;
			result = lzip.decompress_Mem2File(inMemory, outPath, progress, proc, password);
			gchandle.Free();
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
		}
		return result;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000A70C File Offset: 0x0000890C
	private static void writeFile(byte[] tb, string entry, string outPath, string overrideEntryName, ref int res)
	{
		if (tb != null)
		{
			string text;
			if (overrideEntryName == null)
			{
				if (entry.Contains("/"))
				{
					string[] array = entry.Split('/', 0);
					text = array[array.Length - 1];
				}
				else
				{
					text = entry;
				}
			}
			else
			{
				text = overrideEntryName;
			}
			File.WriteAllBytes(outPath + "/" + text, tb);
			res = 1;
			return;
		}
		Debug.Log("Could not extract entry.");
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000A768 File Offset: 0x00008968
	public static int entry2FileMerged(string file, string entry, string outPath, string overrideEntryName = null, string password = null)
	{
		if (!File.Exists(file))
		{
			return -10;
		}
		outPath = outPath.Replace("//", "/");
		int num = 0;
		int num2 = 0;
		int result = 0;
		byte[] mergedZip = lzip.getMergedZip(file, ref num, ref num2);
		if (mergedZip != null)
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(mergedZip);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64());
			inMemory.info[0] = num2;
			byte[] tb = lzip.entry2BufferMem(inMemory, entry, password);
			gchandle.Free();
			lzip.writeFile(tb, entry, outPath, overrideEntryName, ref result);
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
		}
		return result;
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0000A808 File Offset: 0x00008A08
	public static int entry2FileMerged(byte[] buffer, string entry, string outPath, string overrideEntryName = null, string password = null)
	{
		if (buffer == null)
		{
			return -10;
		}
		outPath = outPath.Replace("//", "/");
		int num = 0;
		int num2 = 0;
		int result = 0;
		if (lzip.getZipInfoMerged(buffer, ref num, ref num2, false))
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(buffer);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + (long)num);
			inMemory.info[0] = num2;
			byte[] tb = lzip.entry2BufferMem(inMemory, entry, password);
			gchandle.Free();
			lzip.writeFile(tb, entry, outPath, overrideEntryName, ref result);
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
		}
		return result;
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000A8A0 File Offset: 0x00008AA0
	public static byte[] entry2BufferMerged(string file, string entry, string password = null)
	{
		if (!File.Exists(file))
		{
			return null;
		}
		int num = 0;
		int num2 = 0;
		byte[] mergedZip = lzip.getMergedZip(file, ref num, ref num2);
		if (mergedZip != null)
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(mergedZip);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64());
			inMemory.info[0] = num2;
			byte[] result = lzip.entry2BufferMem(inMemory, entry, password);
			gchandle.Free();
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
			return result;
		}
		return null;
	}

	// Token: 0x06000158 RID: 344 RVA: 0x0000A924 File Offset: 0x00008B24
	public static int entry2BufferMerged(string file, string entry, ref byte[] refBuffer, string password = null)
	{
		if (!File.Exists(file))
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		byte[] mergedZip = lzip.getMergedZip(file, ref num, ref num2);
		if (mergedZip != null)
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(mergedZip);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64());
			inMemory.info[0] = num2;
			int result = lzip.entry2BufferMem(inMemory, entry, ref refBuffer, password);
			gchandle.Free();
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
			return result;
		}
		return 0;
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0000A9A8 File Offset: 0x00008BA8
	public static int entry2FixedBufferMerged(string file, string entry, ref byte[] fixedBuffer, string password = null)
	{
		if (!File.Exists(file))
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		byte[] mergedZip = lzip.getMergedZip(file, ref num, ref num2);
		if (mergedZip != null)
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(mergedZip);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64());
			inMemory.info[0] = num2;
			int result = lzip.entry2FixedBufferMem(inMemory, entry, ref fixedBuffer, password);
			gchandle.Free();
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
			return result;
		}
		return 0;
	}

	// Token: 0x0600015A RID: 346 RVA: 0x0000AA2C File Offset: 0x00008C2C
	public static byte[] entry2BufferMerged(byte[] buffer, string entry, string password = null)
	{
		if (buffer == null)
		{
			return null;
		}
		int num = 0;
		int num2 = 0;
		if (lzip.getZipInfoMerged(buffer, ref num, ref num2, false))
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(buffer);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + (long)num);
			inMemory.info[0] = num2;
			byte[] result = lzip.entry2BufferMem(inMemory, entry, password);
			gchandle.Free();
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
			return result;
		}
		return null;
	}

	// Token: 0x0600015B RID: 347 RVA: 0x0000AAA8 File Offset: 0x00008CA8
	public static int entry2BufferMerged(byte[] buffer, string entry, ref byte[] refBuffer, string password = null)
	{
		if (buffer == null)
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		if (lzip.getZipInfoMerged(buffer, ref num, ref num2, false))
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(buffer);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + (long)num);
			inMemory.info[0] = num2;
			int result = lzip.entry2BufferMem(inMemory, entry, ref refBuffer, password);
			gchandle.Free();
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
			return result;
		}
		return 0;
	}

	// Token: 0x0600015C RID: 348 RVA: 0x0000AB24 File Offset: 0x00008D24
	public static int entry2FixedBufferMerged(byte[] buffer, string entry, ref byte[] fixedBuffer, string password = null)
	{
		if (buffer == null)
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		if (lzip.getZipInfoMerged(buffer, ref num, ref num2, false))
		{
			lzip.inMemory inMemory = new lzip.inMemory();
			GCHandle gchandle = lzip.gcA(buffer);
			inMemory.pointer = new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + (long)num);
			inMemory.info[0] = num2;
			int result = lzip.entry2FixedBufferMem(inMemory, entry, ref fixedBuffer, password);
			gchandle.Free();
			inMemory.info = null;
			inMemory.pointer = IntPtr.Zero;
			return result;
		}
		return 0;
	}

	// Token: 0x0600015D RID: 349 RVA: 0x0000ABA0 File Offset: 0x00008DA0
	public static bool compressBuffer(byte[] source, ref byte[] outBuffer, int levelOfCompression)
	{
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 10)
		{
			levelOfCompression = 10;
		}
		GCHandle gchandle = lzip.gcA(source);
		int num = 0;
		IntPtr intPtr = lzip.zipCompressBuffer(gchandle.AddrOfPinnedObject(), source.Length, levelOfCompression, ref num);
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			gchandle.Free();
			lzip.releaseBuffer(intPtr);
			return false;
		}
		Array.Resize<byte>(ref outBuffer, num);
		Marshal.Copy(intPtr, outBuffer, 0, num);
		gchandle.Free();
		lzip.releaseBuffer(intPtr);
		return true;
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000AC18 File Offset: 0x00008E18
	public static int compressBufferFixed(byte[] source, ref byte[] outBuffer, int levelOfCompression, bool safe = true)
	{
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 10)
		{
			levelOfCompression = 10;
		}
		GCHandle gchandle = lzip.gcA(source);
		int num = 0;
		IntPtr intPtr = lzip.zipCompressBuffer(gchandle.AddrOfPinnedObject(), source.Length, levelOfCompression, ref num);
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			gchandle.Free();
			lzip.releaseBuffer(intPtr);
			return 0;
		}
		if (num > outBuffer.Length)
		{
			if (safe)
			{
				gchandle.Free();
				lzip.releaseBuffer(intPtr);
				return 0;
			}
			num = outBuffer.Length;
		}
		Marshal.Copy(intPtr, outBuffer, 0, num);
		gchandle.Free();
		lzip.releaseBuffer(intPtr);
		return num;
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000ACA8 File Offset: 0x00008EA8
	public static byte[] compressBuffer(byte[] source, int levelOfCompression)
	{
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 10)
		{
			levelOfCompression = 10;
		}
		GCHandle gchandle = lzip.gcA(source);
		int num = 0;
		IntPtr intPtr = lzip.zipCompressBuffer(gchandle.AddrOfPinnedObject(), source.Length, levelOfCompression, ref num);
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			gchandle.Free();
			lzip.releaseBuffer(intPtr);
			return null;
		}
		byte[] array = new byte[num];
		Marshal.Copy(intPtr, array, 0, num);
		gchandle.Free();
		lzip.releaseBuffer(intPtr);
		return array;
	}

	// Token: 0x06000160 RID: 352 RVA: 0x0000AD20 File Offset: 0x00008F20
	public static bool decompressBuffer(byte[] source, ref byte[] outBuffer)
	{
		GCHandle gchandle = lzip.gcA(source);
		int num = 0;
		IntPtr intPtr = lzip.zipDecompressBuffer(gchandle.AddrOfPinnedObject(), source.Length, ref num);
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			gchandle.Free();
			lzip.releaseBuffer(intPtr);
			return false;
		}
		Array.Resize<byte>(ref outBuffer, num);
		Marshal.Copy(intPtr, outBuffer, 0, num);
		gchandle.Free();
		lzip.releaseBuffer(intPtr);
		return true;
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000AD88 File Offset: 0x00008F88
	public static int decompressBufferFixed(byte[] source, ref byte[] outBuffer, bool safe = true)
	{
		GCHandle gchandle = lzip.gcA(source);
		int num = 0;
		IntPtr intPtr = lzip.zipDecompressBuffer(gchandle.AddrOfPinnedObject(), source.Length, ref num);
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			gchandle.Free();
			lzip.releaseBuffer(intPtr);
			return 0;
		}
		if (num > outBuffer.Length)
		{
			if (safe)
			{
				gchandle.Free();
				lzip.releaseBuffer(intPtr);
				return 0;
			}
			num = outBuffer.Length;
		}
		Marshal.Copy(intPtr, outBuffer, 0, num);
		gchandle.Free();
		lzip.releaseBuffer(intPtr);
		return num;
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000AE08 File Offset: 0x00009008
	public static byte[] decompressBuffer(byte[] source)
	{
		GCHandle gchandle = lzip.gcA(source);
		int num = 0;
		IntPtr intPtr = lzip.zipDecompressBuffer(gchandle.AddrOfPinnedObject(), source.Length, ref num);
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			gchandle.Free();
			lzip.releaseBuffer(intPtr);
			return null;
		}
		byte[] array = new byte[num];
		Marshal.Copy(intPtr, array, 0, num);
		gchandle.Free();
		lzip.releaseBuffer(intPtr);
		return array;
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0000AE70 File Offset: 0x00009070
	public static int gzip(byte[] source, byte[] outBuffer, int level, bool addHeader = true, bool addFooter = true, bool overrideDateTimeWithLength = false)
	{
		if (source == null || outBuffer == null)
		{
			return 0;
		}
		GCHandle gchandle = lzip.gcA(source);
		GCHandle gchandle2 = lzip.gcA(outBuffer);
		if (level < 0)
		{
			level = 0;
		}
		if (level > 10)
		{
			level = 10;
		}
		int num = lzip.zipGzip(gchandle.AddrOfPinnedObject(), source.Length, gchandle2.AddrOfPinnedObject(), level, addHeader, addFooter);
		gchandle.Free();
		gchandle2.Free();
		int num2 = 0;
		if (addHeader)
		{
			num2 += 10;
		}
		if (addFooter)
		{
			num2 += 8;
		}
		int num3 = num + num2;
		if (addHeader && overrideDateTimeWithLength)
		{
			outBuffer[4] = (byte)(num3 & 255);
			outBuffer[5] = (byte)((uint)num3 >> 8 & 255U);
			outBuffer[6] = (byte)((uint)num3 >> 16 & 255U);
			outBuffer[7] = (byte)((uint)num3 >> 24 & 255U);
			outBuffer[9] = 254;
		}
		return num3;
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000AF24 File Offset: 0x00009124
	public static int gzipUncompressedSize(byte[] source)
	{
		if (source == null)
		{
			return 0;
		}
		int num = source.Length;
		return (int)(source[num - 4] & byte.MaxValue) | (int)(source[num - 3] & byte.MaxValue) << 8 | (int)(source[num - 2] & byte.MaxValue) << 16 | (int)(source[num - 1] & byte.MaxValue) << 24;
	}

	// Token: 0x06000165 RID: 357 RVA: 0x0000AF74 File Offset: 0x00009174
	public static int gzipCompressedSize(byte[] source, int offset = 0)
	{
		if (source == null)
		{
			return 0;
		}
		if (source[offset + 9] != 254)
		{
			Debug.Log("Gzip has not been marked to have compressed size stored.");
			return 0;
		}
		int num = offset + 8;
		return (int)(source[num - 4] & byte.MaxValue) | (int)(source[num - 3] & byte.MaxValue) << 8 | (int)(source[num - 2] & byte.MaxValue) << 16 | (int)(source[num - 1] & byte.MaxValue) << 24;
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000AFDC File Offset: 0x000091DC
	public static int findGzStart(byte[] buffer)
	{
		if (buffer == null)
		{
			return 0;
		}
		int result = 0;
		for (int i = 0; i < buffer.Length - 2; i++)
		{
			if (buffer[i] == 31 && buffer[i + 1] == 139 && buffer[i + 2] == 8)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000B020 File Offset: 0x00009220
	public static int unGzip(byte[] source, byte[] outBuffer, bool hasHeader = true, bool hasFooter = true)
	{
		if (source == null || outBuffer == null)
		{
			return 0;
		}
		GCHandle gchandle = lzip.gcA(source);
		GCHandle gchandle2 = lzip.gcA(outBuffer);
		int result = lzip.zipUnGzip(gchandle.AddrOfPinnedObject(), source.Length, gchandle2.AddrOfPinnedObject(), outBuffer.Length, hasHeader, hasFooter);
		gchandle.Free();
		gchandle2.Free();
		return result;
	}

	// Token: 0x06000168 RID: 360 RVA: 0x0000B06C File Offset: 0x0000926C
	public static int unGzip2(object source, byte[] outBuffer, int intPtrLength = 0)
	{
		if (source == null || outBuffer == null)
		{
			return 0;
		}
		if (source is byte[])
		{
			byte[] array = (byte[])source;
			GCHandle gchandle = lzip.gcA(array);
			GCHandle gchandle2 = lzip.gcA(outBuffer);
			int result = lzip.zipUnGzip2(gchandle.AddrOfPinnedObject(), array.Length, gchandle2.AddrOfPinnedObject(), outBuffer.Length);
			gchandle.Free();
			gchandle2.Free();
		}
		if (source is IntPtr && intPtrLength > 0)
		{
			IntPtr source2 = (IntPtr)source;
			GCHandle gchandle3 = lzip.gcA(outBuffer);
			int result = lzip.zipUnGzip2(source2, intPtrLength, gchandle3.AddrOfPinnedObject(), outBuffer.Length);
			gchandle3.Free();
			return result;
		}
		return -11;
	}

	// Token: 0x06000169 RID: 361 RVA: 0x0000B100 File Offset: 0x00009300
	public static int unGzip2Merged(byte[] source, int offset, int bufferLength, byte[] outBuffer)
	{
		if (source == null || outBuffer == null)
		{
			return 0;
		}
		if (bufferLength == 0)
		{
			return 0;
		}
		GCHandle gchandle = lzip.gcA(source);
		GCHandle gchandle2 = lzip.gcA(outBuffer);
		int result = lzip.zipUnGzip2(new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + (long)offset), bufferLength, gchandle2.AddrOfPinnedObject(), outBuffer.Length);
		gchandle.Free();
		gchandle2.Free();
		return result;
	}

	// Token: 0x0600016A RID: 362 RVA: 0x0000B160 File Offset: 0x00009360
	public static int gzipFile(string inFile, string outFile = null, int level = 9, ulong[] progress = null, bool addHeader = true)
	{
		if (level < 1)
		{
			level = 1;
		}
		if (level > 10)
		{
			level = 10;
		}
		if (outFile == null)
		{
			outFile = inFile + ".gz";
		}
		int num;
		if (progress != null)
		{
			GCHandle gchandle = lzip.gcA(progress);
			num = lzip.gzip_File(inFile.Replace("//", "/"), outFile.Replace("//", "/"), level, gchandle.AddrOfPinnedObject(), addHeader);
			gchandle.Free();
		}
		else
		{
			num = lzip.gzip_File(inFile.Replace("//", "/"), outFile.Replace("//", "/"), level, IntPtr.Zero, addHeader);
		}
		if (num == 0)
		{
			return 1;
		}
		return num;
	}

	// Token: 0x0600016B RID: 363 RVA: 0x0000B208 File Offset: 0x00009408
	public static int ungzipFile(string inFile, string outFile = null, ulong[] progress = null)
	{
		if (outFile == null)
		{
			if (inFile.Substring(inFile.Length - 3, 3).ToLower() != ".gz")
			{
				Debug.Log("Input file does not have a .gz extension");
				return -2;
			}
			outFile = inFile.Substring(0, inFile.Length - 3);
		}
		int result;
		if (progress != null)
		{
			GCHandle gchandle = lzip.gcA(progress);
			result = lzip.ungzip_File(inFile.Replace("//", "/"), outFile.Replace("//", "/"), gchandle.AddrOfPinnedObject());
			gchandle.Free();
		}
		else
		{
			result = lzip.ungzip_File(inFile.Replace("//", "/"), outFile.Replace("//", "/"), IntPtr.Zero);
		}
		return result;
	}

	// Token: 0x0600016C RID: 364 RVA: 0x0000B2C4 File Offset: 0x000094C4
	public static int bz2Create(string inFile, string outFile = null, int level = 9, ulong[] byteProgress = null)
	{
		if (outFile == null)
		{
			outFile = inFile + ".bz2";
		}
		int result;
		if (byteProgress != null)
		{
			GCHandle gchandle = lzip.gcA(byteProgress);
			result = lzip.bz2(false, level, inFile.Replace("//", "/"), outFile.Replace("//", "/"), gchandle.AddrOfPinnedObject());
			gchandle.Free();
		}
		else
		{
			result = lzip.bz2(false, level, inFile.Replace("//", "/"), outFile.Replace("//", "/"), IntPtr.Zero);
		}
		return result;
	}

	// Token: 0x0600016D RID: 365 RVA: 0x0000B354 File Offset: 0x00009554
	public static int bz2Decompress(string inFile, string outFile = null, ulong[] byteProgress = null)
	{
		if (outFile == null)
		{
			if (inFile.Substring(inFile.Length - 4, 4).ToLower() != ".bz2")
			{
				Debug.Log("Input file does not have a .bz2 extension");
				return -2;
			}
			outFile = inFile.Substring(0, inFile.Length - 4);
		}
		int result;
		if (byteProgress != null)
		{
			GCHandle gchandle = lzip.gcA(byteProgress);
			result = lzip.bz2(true, 0, inFile.Replace("//", "/"), outFile.Replace("//", "/"), gchandle.AddrOfPinnedObject());
			gchandle.Free();
		}
		else
		{
			result = lzip.bz2(true, 0, inFile.Replace("//", "/"), outFile.Replace("//", "/"), IntPtr.Zero);
		}
		return result;
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0000B415 File Offset: 0x00009615
	public static IEnumerator downloadZipFileNative(string url, Action<bool> downloadDone, Action<lzip.inMemory> inmem, Action<IntPtr> pointer = null, Action<int> fileSize = null)
	{
		UnityWebRequest wr = UnityWebRequest.Head(url);
		lzip.nativeBufferIsBeingUsed = true;
		yield return wr.SendWebRequest();
		string responseHeader = wr.GetResponseHeader("Content-Length");
		lzip.nativeBufferIsBeingUsed = false;
		if (wr.result == 2 || wr.result == 3)
		{
			Debug.LogError("Error While Getting Length: " + wr.error);
		}
		else if (!lzip.nativeBufferIsBeingUsed)
		{
			int zipSize = Convert.ToInt32(responseHeader);
			if (zipSize > 0)
			{
				lzip.nativeBuffer = lzip.createBuffer(zipSize);
				lzip.nativeBufferIsBeingUsed = true;
				byte[] buffer = new byte[2048];
				lzip.nativeOffset = 0;
				using (UnityWebRequest wwwSK = UnityWebRequest.Get(url))
				{
					wwwSK.downloadHandler = new lzip.CustomWebRequest(buffer);
					yield return wwwSK.SendWebRequest();
					if (wwwSK.error != null)
					{
						Debug.Log(wwwSK.error);
					}
					else
					{
						downloadDone.Invoke(true);
						if (inmem != null)
						{
							lzip.inMemory inMemory = new lzip.inMemory();
							inMemory.pointer = lzip.nativeBuffer;
							inMemory.info[0] = zipSize;
							inmem.Invoke(inMemory);
						}
						if (pointer != null)
						{
							pointer.Invoke(lzip.nativeBuffer);
							fileSize.Invoke(zipSize);
						}
						lzip.nativeBufferIsBeingUsed = false;
						lzip.nativeOffset = 0;
						lzip.nativeBuffer = IntPtr.Zero;
					}
				}
				UnityWebRequest wwwSK = null;
			}
		}
		else
		{
			Debug.LogError("Native buffer is being used, or not yet freed!");
		}
		yield break;
		yield break;
	}

	// Token: 0x040000BB RID: 187
	private const string libname = "libzipw";

	// Token: 0x040000BC RID: 188
	public static IntPtr nativeBuffer = IntPtr.Zero;

	// Token: 0x040000BD RID: 189
	public static bool nativeBufferIsBeingUsed = false;

	// Token: 0x040000BE RID: 190
	public static int nativeOffset = 0;

	// Token: 0x040000BF RID: 191
	public static List<string> ninfo = new List<string>();

	// Token: 0x040000C0 RID: 192
	public static List<ulong> uinfo = new List<ulong>();

	// Token: 0x040000C1 RID: 193
	public static List<ulong> cinfo = new List<ulong>();

	// Token: 0x040000C2 RID: 194
	public static List<ulong> localOffset = new List<ulong>();

	// Token: 0x040000C3 RID: 195
	public static int zipFiles;

	// Token: 0x040000C4 RID: 196
	public static int zipFolders;

	// Token: 0x040000C5 RID: 197
	public static ulong totalCompressedSize;

	// Token: 0x040000C6 RID: 198
	public static ulong totalUncompressedSize;

	// Token: 0x040000C7 RID: 199
	public static List<lzip.zipInfo> zinfo;

	// Token: 0x02000020 RID: 32
	public class inMemory
	{
		// Token: 0x06000171 RID: 369 RVA: 0x0000B481 File Offset: 0x00009681
		public int size()
		{
			return this.info[0];
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000B48B File Offset: 0x0000968B
		public IntPtr memoryPointer()
		{
			return this.pointer;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000B494 File Offset: 0x00009694
		public byte[] getZipBuffer()
		{
			if (this.pointer != IntPtr.Zero && this.info[0] > 0)
			{
				byte[] array = new byte[this.info[0]];
				Marshal.Copy(this.pointer, array, 0, this.info[0]);
				return array;
			}
			return null;
		}

		// Token: 0x040000C8 RID: 200
		public IntPtr pointer = IntPtr.Zero;

		// Token: 0x040000C9 RID: 201
		public IntPtr zf = IntPtr.Zero;

		// Token: 0x040000CA RID: 202
		public IntPtr memStruct = IntPtr.Zero;

		// Token: 0x040000CB RID: 203
		public IntPtr fileStruct = IntPtr.Zero;

		// Token: 0x040000CC RID: 204
		public int[] info = new int[3];

		// Token: 0x040000CD RID: 205
		public int lastResult;

		// Token: 0x040000CE RID: 206
		public bool isClosed = true;
	}

	// Token: 0x02000021 RID: 33
	public struct zipInfo
	{
		// Token: 0x040000CF RID: 207
		public short VersionMadeBy;

		// Token: 0x040000D0 RID: 208
		public short MinimumVersionToExtract;

		// Token: 0x040000D1 RID: 209
		public short BitFlag;

		// Token: 0x040000D2 RID: 210
		public short CompressionMethod;

		// Token: 0x040000D3 RID: 211
		public short FileLastModificationTime;

		// Token: 0x040000D4 RID: 212
		public short FileLastModificationDate;

		// Token: 0x040000D5 RID: 213
		public int CRC;

		// Token: 0x040000D6 RID: 214
		public int CompressedSize;

		// Token: 0x040000D7 RID: 215
		public int UncompressedSize;

		// Token: 0x040000D8 RID: 216
		public short DiskNumberWhereFileStarts;

		// Token: 0x040000D9 RID: 217
		public short InternalFileAttributes;

		// Token: 0x040000DA RID: 218
		public int ExternalFileAttributes;

		// Token: 0x040000DB RID: 219
		public int RelativeOffsetOfLocalFileHeader;

		// Token: 0x040000DC RID: 220
		public int AbsoluteOffsetOfLocalFileHeaderStore;

		// Token: 0x040000DD RID: 221
		public string filename;

		// Token: 0x040000DE RID: 222
		public string extraField;

		// Token: 0x040000DF RID: 223
		public string fileComment;
	}

	// Token: 0x02000022 RID: 34
	public class CustomWebRequest : DownloadHandlerScript
	{
		// Token: 0x06000175 RID: 373 RVA: 0x00003EDD File Offset: 0x000020DD
		public CustomWebRequest()
		{
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00003EE5 File Offset: 0x000020E5
		public CustomWebRequest(byte[] buffer) : base(buffer)
		{
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00003EEE File Offset: 0x000020EE
		protected override byte[] GetData()
		{
			return null;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000B538 File Offset: 0x00009738
		protected override bool ReceiveData(byte[] bytesFromServer, int dataLength)
		{
			if (bytesFromServer == null || bytesFromServer.Length < 1)
			{
				Debug.Log("CustomWebRequest: Received a null/empty buffer");
				return false;
			}
			GCHandle gchandle = lzip.gcA(bytesFromServer);
			lzip.addToBuffer(lzip.nativeBuffer, lzip.nativeOffset, gchandle.AddrOfPinnedObject(), dataLength);
			lzip.nativeOffset += dataLength;
			gchandle.Free();
			return true;
		}
	}
}
