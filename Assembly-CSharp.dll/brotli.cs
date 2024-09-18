using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000005 RID: 5
public class brotli
{
	// Token: 0x06000014 RID: 20
	[DllImport("libbrotli")]
	internal static extern int brCompress(string inFile, string outFile, IntPtr proc, int quality, int lgwin, int lgblock, int mode);

	// Token: 0x06000015 RID: 21
	[DllImport("libbrotli")]
	internal static extern int brDecompresss(string inFile, string outFile, IntPtr proc, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x06000016 RID: 22
	[DllImport("libbrotli")]
	public static extern void brReleaseBuffer(IntPtr buffer);

	// Token: 0x06000017 RID: 23
	[DllImport("libbrotli")]
	public static extern IntPtr brCreate_Buffer(int size);

	// Token: 0x06000018 RID: 24
	[DllImport("libbrotli")]
	private static extern void brAddTo_Buffer(IntPtr destination, int offset, IntPtr buffer, int len);

	// Token: 0x06000019 RID: 25
	[DllImport("libbrotli")]
	internal static extern IntPtr brCompressBuffer(int bufferLength, IntPtr buffer, IntPtr encodedSize, IntPtr proc, int quality, int lgwin, int lgblock, int mode);

	// Token: 0x0600001A RID: 26
	[DllImport("libbrotli")]
	internal static extern int brGetDecodedSize(int bufferLength, IntPtr buffer);

	// Token: 0x0600001B RID: 27
	[DllImport("libbrotli")]
	internal static extern int brDecompressBuffer(int bufferLength, IntPtr buffer, int outLength, IntPtr outbuffer);

	// Token: 0x0600001C RID: 28 RVA: 0x00003884 File Offset: 0x00001A84
	public static int setFilePermissions(string filePath, string _user, string _group, string _other)
	{
		return -1;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00003887 File Offset: 0x00001A87
	internal static GCHandle gcA(object o)
	{
		return GCHandle.Alloc(o, 3);
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00003890 File Offset: 0x00001A90
	private static bool checkObject(object fileBuffer, string filePath, ref GCHandle fbuf, ref IntPtr fileBufferPointer, ref int fileBufferLength)
	{
		if (fileBuffer is byte[])
		{
			byte[] array = (byte[])fileBuffer;
			fbuf = brotli.gcA(array);
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

	// Token: 0x0600001F RID: 31 RVA: 0x000038E4 File Offset: 0x00001AE4
	public static int compressFile(string inFile, string outFile, ulong[] proc, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0)
	{
		if (!File.Exists(inFile))
		{
			return -5;
		}
		if (quality < 0)
		{
			quality = 1;
		}
		if (quality > 11)
		{
			quality = 11;
		}
		if (lgwin < 10)
		{
			lgwin = 10;
		}
		if (lgwin > 24)
		{
			lgwin = 24;
		}
		if (proc == null)
		{
			proc = new ulong[1];
		}
		GCHandle gchandle = GCHandle.Alloc(proc, 3);
		int result = brotli.brCompress(inFile, outFile, gchandle.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);
		gchandle.Free();
		return result;
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00003950 File Offset: 0x00001B50
	public static int decompressFile(string inFile, string outFile, ulong[] proc, object fileBuffer = null)
	{
		if (fileBuffer == null && !File.Exists(inFile))
		{
			return -5;
		}
		if (proc == null)
		{
			proc = new ulong[1];
		}
		GCHandle gchandle = GCHandle.Alloc(proc, 3);
		int result = brotli.brDecompresss(inFile, outFile, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
		gchandle.Free();
		return result;
	}

	// Token: 0x06000021 RID: 33 RVA: 0x0000399C File Offset: 0x00001B9C
	public static int getDecodedSize(byte[] inBuffer)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int result = brotli.brGetDecodedSize(inBuffer.Length, gchandle.AddrOfPinnedObject());
		gchandle.Free();
		return result;
	}

	// Token: 0x06000022 RID: 34 RVA: 0x000039C8 File Offset: 0x00001BC8
	public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, ulong[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0)
	{
		if (quality < 0)
		{
			quality = 1;
		}
		if (quality > 11)
		{
			quality = 11;
		}
		if (lgwin < 10)
		{
			lgwin = 10;
		}
		if (lgwin > 24)
		{
			lgwin = 24;
		}
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = 0;
		byte[] array = null;
		int[] array2 = new int[1];
		GCHandle gchandle2 = GCHandle.Alloc(array2, 3);
		if (includeSize)
		{
			array = new byte[4];
			num = 4;
			array = BitConverter.GetBytes(inBuffer.Length);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse<byte>(array);
			}
		}
		if (proc == null)
		{
			proc = new ulong[1];
		}
		GCHandle gchandle3 = GCHandle.Alloc(proc, 3);
		IntPtr intPtr = brotli.brCompressBuffer(inBuffer.Length, gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), gchandle3.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);
		gchandle.Free();
		gchandle2.Free();
		gchandle3.Free();
		if (intPtr == IntPtr.Zero)
		{
			brotli.brReleaseBuffer(intPtr);
			return false;
		}
		Array.Resize<byte>(ref outBuffer, array2[0] + num);
		if (includeSize)
		{
			for (int i = 0; i < 4; i++)
			{
				outBuffer[i + array2[0]] = array[i];
			}
		}
		Marshal.Copy(intPtr, outBuffer, 0, array2[0]);
		brotli.brReleaseBuffer(intPtr);
		return true;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00003AEC File Offset: 0x00001CEC
	public static byte[] compressBuffer(byte[] inBuffer, int[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0)
	{
		if (quality < 0)
		{
			quality = 1;
		}
		if (quality > 11)
		{
			quality = 11;
		}
		if (lgwin < 10)
		{
			lgwin = 10;
		}
		if (lgwin > 24)
		{
			lgwin = 24;
		}
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = 0;
		byte[] array = null;
		int[] array2 = new int[1];
		GCHandle gchandle2 = GCHandle.Alloc(array2, 3);
		if (includeSize)
		{
			array = new byte[4];
			num = 4;
			array = BitConverter.GetBytes(inBuffer.Length);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse<byte>(array);
			}
		}
		if (proc == null)
		{
			proc = new int[1];
		}
		GCHandle gchandle3 = GCHandle.Alloc(proc, 3);
		IntPtr intPtr = brotli.brCompressBuffer(inBuffer.Length, gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), gchandle3.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);
		gchandle.Free();
		gchandle2.Free();
		gchandle3.Free();
		if (intPtr == IntPtr.Zero)
		{
			brotli.brReleaseBuffer(intPtr);
			return null;
		}
		byte[] array3 = new byte[array2[0] + num];
		if (includeSize)
		{
			for (int i = 0; i < 4; i++)
			{
				array3[i + array2[0]] = array[i];
			}
		}
		Marshal.Copy(intPtr, array3, 0, array2[0]);
		brotli.brReleaseBuffer(intPtr);
		return array3;
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00003C10 File Offset: 0x00001E10
	public static int compressBuffer(byte[] inBuffer, byte[] outBuffer, int[] proc, bool includeSize = false, int quality = 9, int lgwin = 19, int lgblock = 0, int mode = 0)
	{
		if (quality < 0)
		{
			quality = 1;
		}
		if (quality > 11)
		{
			quality = 11;
		}
		if (lgwin < 10)
		{
			lgwin = 10;
		}
		if (lgwin > 24)
		{
			lgwin = 24;
		}
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = 0;
		byte[] array = null;
		int[] array2 = new int[1];
		GCHandle gchandle2 = GCHandle.Alloc(array2, 3);
		if (includeSize)
		{
			array = new byte[4];
			num = 4;
			array = BitConverter.GetBytes(inBuffer.Length);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse<byte>(array);
			}
		}
		if (proc == null)
		{
			proc = new int[1];
		}
		GCHandle gchandle3 = GCHandle.Alloc(proc, 3);
		IntPtr intPtr = brotli.brCompressBuffer(inBuffer.Length, gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), gchandle3.AddrOfPinnedObject(), quality, lgwin, lgblock, mode);
		gchandle.Free();
		gchandle2.Free();
		gchandle3.Free();
		int num2 = array2[0];
		if (intPtr == IntPtr.Zero || outBuffer.Length < array2[0] + num)
		{
			brotli.brReleaseBuffer(intPtr);
			return 0;
		}
		Marshal.Copy(intPtr, outBuffer, 0, array2[0]);
		if (includeSize)
		{
			for (int i = 0; i < 4; i++)
			{
				outBuffer[i + array2[0]] = array[i];
			}
		}
		brotli.brReleaseBuffer(intPtr);
		return num2 + num;
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00003D3C File Offset: 0x00001F3C
	public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = false, int unCompressedSize = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = inBuffer.Length;
		int num2;
		if (unCompressedSize == 0)
		{
			if (useFooter)
			{
				num -= 4;
				num2 = BitConverter.ToInt32(inBuffer, num);
			}
			else
			{
				num2 = brotli.getDecodedSize(inBuffer);
			}
		}
		else
		{
			num2 = unCompressedSize;
		}
		Array.Resize<byte>(ref outBuffer, num2);
		GCHandle gchandle2 = GCHandle.Alloc(outBuffer, 3);
		int num3 = brotli.brDecompressBuffer(inBuffer.Length, gchandle.AddrOfPinnedObject(), num2, gchandle2.AddrOfPinnedObject());
		gchandle.Free();
		gchandle2.Free();
		return num3 == 1;
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00003DB4 File Offset: 0x00001FB4
	public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = false, int unCompressedSize = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = inBuffer.Length;
		int num2;
		if (unCompressedSize == 0)
		{
			if (useFooter)
			{
				num -= 4;
				num2 = BitConverter.ToInt32(inBuffer, num);
			}
			else
			{
				num2 = brotli.getDecodedSize(inBuffer);
			}
		}
		else
		{
			num2 = unCompressedSize;
		}
		byte[] array = new byte[num2];
		GCHandle gchandle2 = GCHandle.Alloc(array, 3);
		int num3 = brotli.brDecompressBuffer(inBuffer.Length, gchandle.AddrOfPinnedObject(), num2, gchandle2.AddrOfPinnedObject());
		gchandle.Free();
		gchandle2.Free();
		if (num3 == 1)
		{
			return array;
		}
		return null;
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00003E2C File Offset: 0x0000202C
	public static int decompressBuffer(byte[] inBuffer, byte[] outBuffer, bool useFooter = false, int unCompressedSize = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = inBuffer.Length;
		int num2;
		if (unCompressedSize == 0)
		{
			if (useFooter)
			{
				num -= 4;
				num2 = BitConverter.ToInt32(inBuffer, num);
			}
			else
			{
				num2 = brotli.getDecodedSize(inBuffer);
			}
		}
		else
		{
			num2 = unCompressedSize;
		}
		GCHandle gchandle2 = GCHandle.Alloc(outBuffer, 3);
		int num3 = brotli.brDecompressBuffer(inBuffer.Length, gchandle.AddrOfPinnedObject(), num2, gchandle2.AddrOfPinnedObject());
		gchandle.Free();
		gchandle2.Free();
		if (num3 == 1)
		{
			return num2;
		}
		return 0;
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00003E99 File Offset: 0x00002099
	public static IEnumerator downloadBrFileNative(string url, Action<bool> downloadDone, Action<IntPtr> pointer = null, Action<int> fileSize = null)
	{
		UnityWebRequest wr = UnityWebRequest.Head(url);
		brotli.nativeBufferIsBeingUsed = true;
		yield return wr.SendWebRequest();
		string responseHeader = wr.GetResponseHeader("Content-Length");
		brotli.nativeBufferIsBeingUsed = false;
		if (wr.result == 2 || wr.result == 3)
		{
			Debug.LogError("Error While Getting Length: " + wr.error);
		}
		else if (!brotli.nativeBufferIsBeingUsed)
		{
			int zipSize = Convert.ToInt32(responseHeader);
			if (zipSize > 0)
			{
				brotli.nativeBuffer = brotli.brCreate_Buffer(zipSize);
				brotli.nativeBufferIsBeingUsed = true;
				byte[] buffer = new byte[2048];
				brotli.nativeOffset = 0;
				using (UnityWebRequest wwwSK = UnityWebRequest.Get(url))
				{
					wwwSK.downloadHandler = new brotli.CustomWebRequest5(buffer);
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
							pointer.Invoke(brotli.nativeBuffer);
							fileSize.Invoke(zipSize);
						}
						brotli.nativeBufferIsBeingUsed = false;
						brotli.nativeOffset = 0;
						brotli.nativeBuffer = IntPtr.Zero;
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

	// Token: 0x04000023 RID: 35
	private const string libname = "libbrotli";

	// Token: 0x04000024 RID: 36
	public static IntPtr nativeBuffer = IntPtr.Zero;

	// Token: 0x04000025 RID: 37
	public static bool nativeBufferIsBeingUsed = false;

	// Token: 0x04000026 RID: 38
	public static int nativeOffset = 0;

	// Token: 0x02000006 RID: 6
	public class CustomWebRequest5 : DownloadHandlerScript
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00003EDD File Offset: 0x000020DD
		public CustomWebRequest5()
		{
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003EE5 File Offset: 0x000020E5
		public CustomWebRequest5(byte[] buffer) : base(buffer)
		{
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003EEE File Offset: 0x000020EE
		protected override byte[] GetData()
		{
			return null;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003EF4 File Offset: 0x000020F4
		protected override bool ReceiveData(byte[] bytesFromServer, int dataLength)
		{
			if (bytesFromServer == null || bytesFromServer.Length < 1)
			{
				Debug.Log("CustomWebRequest: Received a null/empty buffer");
				return false;
			}
			GCHandle gchandle = brotli.gcA(bytesFromServer);
			brotli.brAddTo_Buffer(brotli.nativeBuffer, brotli.nativeOffset, gchandle.AddrOfPinnedObject(), dataLength);
			brotli.nativeOffset += dataLength;
			gchandle.Free();
			return true;
		}
	}
}
