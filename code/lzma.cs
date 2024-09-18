using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000017 RID: 23
public class lzma
{
	// Token: 0x060000A3 RID: 163 RVA: 0x000063C4 File Offset: 0x000045C4
	public static void setProps(int level = 5, int dictSize = 65536, int lc = 3, int lp = 0, int pb = 2, int fb = 32, int numThreads = 2)
	{
		lzma.defaultsSet = true;
		lzma.props[0] = level;
		lzma.props[1] = dictSize;
		lzma.props[2] = lc;
		lzma.props[3] = lp;
		lzma.props[4] = pb;
		lzma.props[5] = fb;
		lzma.props[6] = numThreads;
	}

	// Token: 0x060000A4 RID: 164
	[DllImport("liblzma")]
	internal static extern int decompress7zip(string filePath, string exctractionPath, bool fullPaths, string entry, IntPtr progress, IntPtr FileBuffer, int FileBufferLength);

	// Token: 0x060000A5 RID: 165
	[DllImport("liblzma")]
	internal static extern int decompress7zip2(string filePath, string exctractionPath, bool fullPaths, string entry, IntPtr progress, IntPtr FileBuffer, int FileBufferLength);

	// Token: 0x060000A6 RID: 166
	[DllImport("liblzma")]
	internal static extern IntPtr _getSize(string filePath, IntPtr FileBuffer, int FileBufferLength, bool justParse);

	// Token: 0x060000A7 RID: 167
	[DllImport("liblzma")]
	internal static extern ulong entrySize(string filePath, string entry, IntPtr FileBuffer, int FileBufferLength);

	// Token: 0x060000A8 RID: 168
	[DllImport("liblzma")]
	internal static extern int lzmaUtil(bool encode, string inPath, string outPath, IntPtr Props);

	// Token: 0x060000A9 RID: 169
	[DllImport("liblzma")]
	internal static extern int decode2Buf(string filePath, string entry, IntPtr buffer, IntPtr FileBuffer, int FileBufferLength);

	// Token: 0x060000AA RID: 170
	[DllImport("liblzma")]
	public static extern void _releaseBuffer(IntPtr buffer);

	// Token: 0x060000AB RID: 171
	[DllImport("liblzma")]
	public static extern IntPtr _createBuffer(int size);

	// Token: 0x060000AC RID: 172
	[DllImport("liblzma")]
	private static extern void _addToBuffer(IntPtr destination, int offset, IntPtr buffer, int len);

	// Token: 0x060000AD RID: 173
	[DllImport("liblzma")]
	internal static extern IntPtr Lzma_Compress(IntPtr buffer, int bufferLength, bool makeHeader, ref int v, IntPtr Props);

	// Token: 0x060000AE RID: 174
	[DllImport("liblzma")]
	internal static extern int Lzma_Uncompress(IntPtr buffer, int bufferLength, int uncompressedSize, IntPtr outbuffer, bool useHeader);

	// Token: 0x060000AF RID: 175
	[DllImport("liblzma")]
	public static extern void sevenZcancel();

	// Token: 0x060000B0 RID: 176
	[DllImport("liblzma")]
	public static extern void resetBytesRead();

	// Token: 0x060000B1 RID: 177
	[DllImport("liblzma")]
	public static extern ulong getBytesRead();

	// Token: 0x060000B2 RID: 178
	[DllImport("liblzma")]
	public static extern ulong getBytesWritten();

	// Token: 0x060000B3 RID: 179 RVA: 0x00003887 File Offset: 0x00001A87
	internal static GCHandle gcA(object o)
	{
		return GCHandle.Alloc(o, 3);
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00003884 File Offset: 0x00001A84
	public static int setFilePermissions(string filePath, string _user, string _group, string _other)
	{
		return -1;
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00006414 File Offset: 0x00004614
	private static bool checkObject(object fileBuffer, string filePath, ref GCHandle fbuf, ref IntPtr fileBufferPointer, ref int fileBufferLength)
	{
		if (fileBuffer is byte[])
		{
			byte[] array = (byte[])fileBuffer;
			fbuf = lzma.gcA(array);
			fileBufferPointer = fbuf.AddrOfPinnedObject();
			fileBufferLength = array.Length;
			return true;
		}
		if (fileBuffer is IntPtr)
		{
			fileBufferPointer = (IntPtr)fileBuffer;
			fileBufferLength = Convert.ToInt32(filePath);
		}
		return false;
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00006468 File Offset: 0x00004668
	public static int doDecompress7zip(string filePath, string exctractionPath = null, int[] progress = null, bool largeFiles = false, bool fullPaths = true, string entry = null, object fileBuffer = null)
	{
		if (exctractionPath == null)
		{
			exctractionPath = Path.GetDirectoryName(filePath);
		}
		if (exctractionPath.Substring(exctractionPath.Length - 1, 1) != "/")
		{
			exctractionPath += "/";
		}
		if (!Directory.Exists(exctractionPath))
		{
			Directory.CreateDirectory(exctractionPath);
		}
		if (entry == "")
		{
			entry = null;
		}
		if (progress == null)
		{
			progress = new int[1];
		}
		GCHandle gchandle = lzma.gcA(progress);
		if (largeFiles)
		{
			int result = lzma.decompress7zip(filePath, exctractionPath, fullPaths, entry, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
			gchandle.Free();
			return result;
		}
		int result2 = lzma.decompress7zip2(filePath, exctractionPath, fullPaths, entry, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
		gchandle.Free();
		return result2;
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x0000651C File Offset: 0x0000471C
	public static int doDecompress7zip(string filePath, string exctractionPath = null, bool largeFiles = false, bool fullPaths = true, string entry = null, object fileBuffer = null)
	{
		if (exctractionPath == null)
		{
			exctractionPath = Path.GetDirectoryName(filePath);
		}
		if (exctractionPath.Substring(exctractionPath.Length - 1, 1) != "/")
		{
			exctractionPath += "/";
		}
		if (!Directory.Exists(exctractionPath))
		{
			Directory.CreateDirectory(exctractionPath);
		}
		if (entry == "")
		{
			entry = null;
		}
		GCHandle gchandle = lzma.gcA(new int[1]);
		if (largeFiles)
		{
			int result = lzma.decompress7zip(filePath, exctractionPath, fullPaths, entry, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
			gchandle.Free();
			return result;
		}
		int result2 = lzma.decompress7zip2(filePath, exctractionPath, fullPaths, entry, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
		gchandle.Free();
		return result2;
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000065C8 File Offset: 0x000047C8
	public static int LzmaUtilEncode(string inPath, string outPath)
	{
		if (!lzma.defaultsSet)
		{
			lzma.setProps(5, 65536, 3, 0, 2, 32, 2);
		}
		GCHandle gchandle = lzma.gcA(lzma.props);
		int result = lzma.lzmaUtil(true, inPath, outPath, gchandle.AddrOfPinnedObject());
		gchandle.Free();
		return result;
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x0000660E File Offset: 0x0000480E
	public static int LzmaUtilDecode(string inPath, string outPath)
	{
		return lzma.lzmaUtil(false, inPath, outPath, IntPtr.Zero);
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00006620 File Offset: 0x00004820
	public static ulong get7zInfo(string filePath, object fileBuffer = null)
	{
		lzma.ninfo.Clear();
		lzma.sinfo.Clear();
		lzma.trueTotalFiles = 0;
		IntPtr intPtr = IntPtr.Zero;
		intPtr = lzma._getSize(filePath, IntPtr.Zero, 0, false);
		if (intPtr == IntPtr.Zero)
		{
			return 0UL;
		}
		StringReader stringReader = new StringReader(Marshal.PtrToStringAuto(intPtr));
		ulong num = 0UL;
		ulong num2 = 0UL;
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			string[] array = text.Split('|', 0);
			if (array.Length != 0)
			{
				lzma.ninfo.Add(array[0]);
			}
			else
			{
				lzma.ninfo.Add("null");
			}
			if (array.Length > 1)
			{
				ulong.TryParse(array[1], ref num);
				num2 += num;
				lzma.sinfo.Add(num);
				if (num > 0UL)
				{
					lzma.trueTotalFiles++;
				}
			}
			else
			{
				lzma.sinfo.Add(0UL);
			}
		}
		stringReader.Close();
		stringReader.Dispose();
		lzma._releaseBuffer(intPtr);
		return num2;
	}

	// Token: 0x060000BB RID: 187 RVA: 0x0000670F File Offset: 0x0000490F
	public static ulong get7zSize(string filePath = null, string entry = null, object fileBuffer = null)
	{
		if (entry == "")
		{
			entry = null;
		}
		return lzma.entrySize(filePath, entry, IntPtr.Zero, 0);
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00006730 File Offset: 0x00004930
	public static uint getHeadersSize(string filePath, object fileBuffer = null)
	{
		IntPtr intPtr = IntPtr.Zero;
		intPtr = lzma._getSize(filePath, IntPtr.Zero, 0, true);
		if (intPtr != IntPtr.Zero)
		{
			lzma._releaseBuffer(intPtr);
		}
		return (uint)lzma.getBytesRead();
	}

	// Token: 0x060000BD RID: 189 RVA: 0x0000676C File Offset: 0x0000496C
	public static byte[] decode2Buffer(string filePath, string entry, object fileBuffer = null)
	{
		int num = (int)lzma.get7zSize(filePath, entry, fileBuffer);
		if (num <= 0)
		{
			return null;
		}
		byte[] array = new byte[num];
		if (entry == "")
		{
			entry = null;
		}
		GCHandle gchandle = lzma.gcA(array);
		int num2 = lzma.decode2Buf(filePath, entry, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
		gchandle.Free();
		if (num2 == 1)
		{
			return array;
		}
		return null;
	}

	// Token: 0x060000BE RID: 190 RVA: 0x000067CA File Offset: 0x000049CA
	public static int getAllFiles(string dir)
	{
		return Directory.GetFiles(dir, "*", 1).Length;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x000067DC File Offset: 0x000049DC
	public static long getFileSize(string file)
	{
		FileInfo fileInfo = new FileInfo(file);
		if (fileInfo.Exists)
		{
			return fileInfo.Length;
		}
		return 0L;
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00006804 File Offset: 0x00004A04
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

	// Token: 0x060000C1 RID: 193 RVA: 0x0000684C File Offset: 0x00004A4C
	public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool makeHeader = true)
	{
		if (!lzma.defaultsSet)
		{
			lzma.setProps(5, 65536, 3, 0, 2, 32, 2);
		}
		GCHandle gchandle = lzma.gcA(lzma.props);
		GCHandle gchandle2 = lzma.gcA(inBuffer);
		int num = 0;
		IntPtr intPtr = lzma.Lzma_Compress(gchandle2.AddrOfPinnedObject(), inBuffer.Length, makeHeader, ref num, gchandle.AddrOfPinnedObject());
		gchandle2.Free();
		gchandle.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			lzma._releaseBuffer(intPtr);
			return false;
		}
		Array.Resize<byte>(ref outBuffer, num);
		Marshal.Copy(intPtr, outBuffer, 0, num);
		lzma._releaseBuffer(intPtr);
		return true;
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x000068E0 File Offset: 0x00004AE0
	public static byte[] compressBuffer(byte[] inBuffer, bool makeHeader = true)
	{
		if (!lzma.defaultsSet)
		{
			lzma.setProps(5, 65536, 3, 0, 2, 32, 2);
		}
		GCHandle gchandle = lzma.gcA(lzma.props);
		GCHandle gchandle2 = lzma.gcA(inBuffer);
		int num = 0;
		IntPtr intPtr = lzma.Lzma_Compress(gchandle2.AddrOfPinnedObject(), inBuffer.Length, makeHeader, ref num, gchandle.AddrOfPinnedObject());
		gchandle2.Free();
		gchandle.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			lzma._releaseBuffer(intPtr);
			return null;
		}
		byte[] array = new byte[num];
		Marshal.Copy(intPtr, array, 0, num);
		lzma._releaseBuffer(intPtr);
		return array;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00006974 File Offset: 0x00004B74
	public static bool compressBufferPartial(byte[] inBuffer, int inBufferPartialIndex, int inBufferPartialLength, ref byte[] outBuffer, bool makeHeader = true)
	{
		if (inBufferPartialIndex + inBufferPartialLength > inBuffer.Length)
		{
			return false;
		}
		if (!lzma.defaultsSet)
		{
			lzma.setProps(5, 65536, 3, 0, 2, 32, 2);
		}
		GCHandle gchandle = lzma.gcA(lzma.props);
		GCHandle gchandle2 = lzma.gcA(inBuffer);
		int num = 0;
		IntPtr intPtr = lzma.Lzma_Compress(new IntPtr(gchandle2.AddrOfPinnedObject().ToInt64() + (long)inBufferPartialIndex), inBufferPartialLength, makeHeader, ref num, gchandle.AddrOfPinnedObject());
		gchandle2.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			lzma._releaseBuffer(intPtr);
			return false;
		}
		Array.Resize<byte>(ref outBuffer, num);
		Marshal.Copy(intPtr, outBuffer, 0, num);
		lzma._releaseBuffer(intPtr);
		return true;
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x00006A18 File Offset: 0x00004C18
	public static int compressBufferPartialFixed(byte[] inBuffer, int inBufferPartialIndex, int inBufferPartialLength, ref byte[] outBuffer, bool safe = true, bool makeHeader = true)
	{
		if (inBufferPartialIndex + inBufferPartialLength > inBuffer.Length)
		{
			return 0;
		}
		if (!lzma.defaultsSet)
		{
			lzma.setProps(5, 65536, 3, 0, 2, 32, 2);
		}
		GCHandle gchandle = lzma.gcA(lzma.props);
		GCHandle gchandle2 = lzma.gcA(inBuffer);
		int num = 0;
		IntPtr intPtr = lzma.Lzma_Compress(new IntPtr(gchandle2.AddrOfPinnedObject().ToInt64() + (long)inBufferPartialIndex), inBufferPartialLength, makeHeader, ref num, gchandle.AddrOfPinnedObject());
		gchandle2.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			lzma._releaseBuffer(intPtr);
			return 0;
		}
		if (num > outBuffer.Length)
		{
			if (safe)
			{
				lzma._releaseBuffer(intPtr);
				return 0;
			}
			num = outBuffer.Length;
		}
		Marshal.Copy(intPtr, outBuffer, 0, num);
		lzma._releaseBuffer(intPtr);
		return num;
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00006AD0 File Offset: 0x00004CD0
	public static int compressBufferFixed(byte[] inBuffer, ref byte[] outBuffer, bool safe = true, bool makeHeader = true)
	{
		if (!lzma.defaultsSet)
		{
			lzma.setProps(5, 65536, 3, 0, 2, 32, 2);
		}
		GCHandle gchandle = lzma.gcA(lzma.props);
		GCHandle gchandle2 = lzma.gcA(inBuffer);
		int num = 0;
		IntPtr intPtr = lzma.Lzma_Compress(gchandle2.AddrOfPinnedObject(), inBuffer.Length, makeHeader, ref num, gchandle.AddrOfPinnedObject());
		gchandle2.Free();
		gchandle.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			lzma._releaseBuffer(intPtr);
			return 0;
		}
		if (num > outBuffer.Length)
		{
			if (safe)
			{
				lzma._releaseBuffer(intPtr);
				return 0;
			}
			num = outBuffer.Length;
		}
		Marshal.Copy(intPtr, outBuffer, 0, num);
		lzma._releaseBuffer(intPtr);
		return num;
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x00006B74 File Offset: 0x00004D74
	public static int decompressBuffer(byte[] inBuffer, ref byte[] outbuffer, bool useHeader = true, int customLength = 0)
	{
		GCHandle gchandle = lzma.gcA(inBuffer);
		int num;
		if (useHeader)
		{
			num = (int)BitConverter.ToUInt64(inBuffer, 5);
		}
		else
		{
			num = customLength;
		}
		Array.Resize<byte>(ref outbuffer, num);
		GCHandle gchandle2 = lzma.gcA(outbuffer);
		int result = lzma.Lzma_Uncompress(gchandle.AddrOfPinnedObject(), inBuffer.Length, num, gchandle2.AddrOfPinnedObject(), useHeader);
		gchandle.Free();
		gchandle2.Free();
		return result;
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00006BD0 File Offset: 0x00004DD0
	public static byte[] decompressBuffer(byte[] inBuffer, bool useHeader = true, int customLength = 0)
	{
		GCHandle gchandle = lzma.gcA(inBuffer);
		int num;
		if (useHeader)
		{
			num = (int)BitConverter.ToUInt64(inBuffer, 5);
		}
		else
		{
			num = customLength;
		}
		byte[] array = new byte[num];
		GCHandle gchandle2 = lzma.gcA(array);
		bool flag = lzma.Lzma_Uncompress(gchandle.AddrOfPinnedObject(), inBuffer.Length, num, gchandle2.AddrOfPinnedObject(), useHeader) != 0;
		gchandle.Free();
		gchandle2.Free();
		if (flag)
		{
			return null;
		}
		return array;
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00006C30 File Offset: 0x00004E30
	public static int decompressBufferFixed(byte[] inBuffer, ref byte[] outbuffer, bool safe = true, bool useHeader = true, int customLength = 0)
	{
		int num;
		if (useHeader)
		{
			num = (int)BitConverter.ToUInt64(inBuffer, 5);
		}
		else
		{
			num = customLength;
		}
		if (num > outbuffer.Length)
		{
			if (safe)
			{
				return -101;
			}
			num = outbuffer.Length;
		}
		GCHandle gchandle = lzma.gcA(inBuffer);
		GCHandle gchandle2 = lzma.gcA(outbuffer);
		int num2 = lzma.Lzma_Uncompress(gchandle.AddrOfPinnedObject(), inBuffer.Length, num, gchandle2.AddrOfPinnedObject(), useHeader);
		gchandle.Free();
		gchandle2.Free();
		if (num2 != 0)
		{
			return -num2;
		}
		return num;
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00006C9F File Offset: 0x00004E9F
	public static IEnumerator download7zFileNative(string url, Action<bool> downloadDone, Action<IntPtr> pointer = null, Action<int> fileSize = null)
	{
		UnityWebRequest wr = UnityWebRequest.Head(url);
		lzma.nativeBufferIsBeingUsed = true;
		yield return wr.SendWebRequest();
		string responseHeader = wr.GetResponseHeader("Content-Length");
		lzma.nativeBufferIsBeingUsed = false;
		if (wr.result == 2 || wr.result == 3)
		{
			Debug.LogError("Error While Getting Length: " + wr.error);
		}
		else if (!lzma.nativeBufferIsBeingUsed)
		{
			int zipSize = Convert.ToInt32(responseHeader);
			if (zipSize > 0)
			{
				lzma.nativeBuffer = lzma._createBuffer(zipSize);
				lzma.nativeBufferIsBeingUsed = true;
				byte[] buffer = new byte[2048];
				lzma.nativeOffset = 0;
				using (UnityWebRequest wwwSK = UnityWebRequest.Get(url))
				{
					wwwSK.downloadHandler = new lzma.CustomWebRequest2(buffer);
					yield return wwwSK.SendWebRequest();
					if (wwwSK.error != null)
					{
						Debug.Log(wwwSK.error);
					}
					else
					{
						downloadDone.Invoke(true);
						if (pointer != null)
						{
							pointer.Invoke(lzma.nativeBuffer);
							fileSize.Invoke(zipSize);
						}
						lzma.nativeBufferIsBeingUsed = false;
						lzma.nativeOffset = 0;
						lzma.nativeBuffer = IntPtr.Zero;
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

	// Token: 0x0400008A RID: 138
	public static string persitentDataPath = "";

	// Token: 0x0400008B RID: 139
	internal static int[] props = new int[7];

	// Token: 0x0400008C RID: 140
	internal static bool defaultsSet = false;

	// Token: 0x0400008D RID: 141
	private const string libname = "liblzma";

	// Token: 0x0400008E RID: 142
	public static List<string> ninfo = new List<string>();

	// Token: 0x0400008F RID: 143
	public static List<ulong> sinfo = new List<ulong>();

	// Token: 0x04000090 RID: 144
	public static int trueTotalFiles = 0;

	// Token: 0x04000091 RID: 145
	public static IntPtr nativeBuffer = IntPtr.Zero;

	// Token: 0x04000092 RID: 146
	public static bool nativeBufferIsBeingUsed = false;

	// Token: 0x04000093 RID: 147
	public static int nativeOffset = 0;

	// Token: 0x02000018 RID: 24
	public enum dic
	{
		// Token: 0x04000095 RID: 149
		K0004 = 4096,
		// Token: 0x04000096 RID: 150
		K0008 = 8192,
		// Token: 0x04000097 RID: 151
		K0016 = 16384,
		// Token: 0x04000098 RID: 152
		K0032 = 32768,
		// Token: 0x04000099 RID: 153
		K0064 = 65536,
		// Token: 0x0400009A RID: 154
		K0128 = 131072,
		// Token: 0x0400009B RID: 155
		K0256 = 262144,
		// Token: 0x0400009C RID: 156
		K0512 = 524288,
		// Token: 0x0400009D RID: 157
		K1024 = 1048576,
		// Token: 0x0400009E RID: 158
		K2048 = 2097152
	}

	// Token: 0x02000019 RID: 25
	public class CustomWebRequest2 : DownloadHandlerScript
	{
		// Token: 0x060000CC RID: 204 RVA: 0x00003EDD File Offset: 0x000020DD
		public CustomWebRequest2()
		{
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00003EE5 File Offset: 0x000020E5
		public CustomWebRequest2(byte[] buffer) : base(buffer)
		{
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00003EEE File Offset: 0x000020EE
		protected override byte[] GetData()
		{
			return null;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00006D1C File Offset: 0x00004F1C
		protected override bool ReceiveData(byte[] bytesFromServer, int dataLength)
		{
			if (bytesFromServer == null || bytesFromServer.Length < 1)
			{
				Debug.Log("CustomWebRequest: Received a null/empty buffer");
				return false;
			}
			GCHandle gchandle = lzma.gcA(bytesFromServer);
			lzma._addToBuffer(lzma.nativeBuffer, lzma.nativeOffset, gchandle.AddrOfPinnedObject(), dataLength);
			lzma.nativeOffset += dataLength;
			gchandle.Free();
			return true;
		}
	}
}
