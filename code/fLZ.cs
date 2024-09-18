using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200000B RID: 11
public class fLZ
{
	// Token: 0x06000045 RID: 69
	[DllImport("libfastlz")]
	internal static extern int fLZcompressFile(int level, string inFile, string outFile, bool overwrite, IntPtr percent);

	// Token: 0x06000046 RID: 70
	[DllImport("libfastlz")]
	internal static extern int fLZdecompressFile(string inFile, string outFile, bool overwrite, IntPtr percent, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x06000047 RID: 71
	[DllImport("libfastlz")]
	public static extern void fLZreleaseBuffer(IntPtr buffer);

	// Token: 0x06000048 RID: 72
	[DllImport("libfastlz")]
	public static extern IntPtr create_Buffer(int size);

	// Token: 0x06000049 RID: 73
	[DllImport("libfastlz")]
	private static extern void addTo_Buffer(IntPtr destination, int offset, IntPtr buffer, int len);

	// Token: 0x0600004A RID: 74
	[DllImport("libfastlz")]
	internal static extern IntPtr fLZcompressBuffer(IntPtr buffer, int bufferLength, int level, ref int v);

	// Token: 0x0600004B RID: 75
	[DllImport("libfastlz")]
	internal static extern int fLZdecompressBuffer(IntPtr buffer, int bufferLength, IntPtr outbuffer);

	// Token: 0x0600004C RID: 76 RVA: 0x00003884 File Offset: 0x00001A84
	public static int setFilePermissions(string filePath, string _user, string _group, string _other)
	{
		return -1;
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003887 File Offset: 0x00001A87
	internal static GCHandle gcA(object o)
	{
		return GCHandle.Alloc(o, 3);
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00004898 File Offset: 0x00002A98
	private static bool checkObject(object fileBuffer, string filePath, ref GCHandle fbuf, ref IntPtr fileBufferPointer, ref int fileBufferLength)
	{
		if (fileBuffer is byte[])
		{
			byte[] array = (byte[])fileBuffer;
			fbuf = fLZ.gcA(array);
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

	// Token: 0x0600004F RID: 79 RVA: 0x000048EC File Offset: 0x00002AEC
	public static int compressFile(string inFile, string outFile, int level, bool overwrite, ulong[] progress)
	{
		if (level < 1)
		{
			level = 1;
		}
		if (level > 2)
		{
			level = 2;
		}
		if (progress == null)
		{
			progress = new ulong[1];
		}
		GCHandle gchandle = GCHandle.Alloc(progress, 3);
		int result = fLZ.fLZcompressFile(level, inFile, outFile, overwrite, gchandle.AddrOfPinnedObject());
		gchandle.Free();
		return result;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00004934 File Offset: 0x00002B34
	public static int decompressFile(string inFile, string outFile, bool overwrite, ulong[] progress, object fileBuffer = null)
	{
		if (progress == null)
		{
			progress = new ulong[1];
		}
		GCHandle gchandle = GCHandle.Alloc(progress, 3);
		int result = fLZ.fLZdecompressFile(inFile, outFile, overwrite, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
		gchandle.Free();
		return result;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00004970 File Offset: 0x00002B70
	public static bool compressBuffer(byte[] inBuffer, ref byte[] outBuffer, int level, bool includeSize = true)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = 0;
		int num2 = 0;
		byte[] array = null;
		if (includeSize)
		{
			array = new byte[4];
			num2 = 4;
			array = BitConverter.GetBytes(inBuffer.Length);
			if (!fLZ.isle)
			{
				Array.Reverse<byte>(array);
			}
		}
		if (level < 1)
		{
			level = 1;
		}
		if (level > 2)
		{
			level = 2;
		}
		IntPtr intPtr = fLZ.fLZcompressBuffer(gchandle.AddrOfPinnedObject(), inBuffer.Length, level, ref num);
		gchandle.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			fLZ.fLZreleaseBuffer(intPtr);
			return false;
		}
		Array.Resize<byte>(ref outBuffer, num + num2);
		if (includeSize)
		{
			for (int i = 0; i < 4; i++)
			{
				outBuffer[i + num] = array[i];
			}
		}
		Marshal.Copy(intPtr, outBuffer, 0, num);
		fLZ.fLZreleaseBuffer(intPtr);
		return true;
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00004A30 File Offset: 0x00002C30
	public static byte[] compressBuffer(byte[] inBuffer, int level, bool includeSize = true)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = 0;
		int num2 = 0;
		byte[] array = null;
		if (includeSize)
		{
			array = new byte[4];
			num2 = 4;
			array = BitConverter.GetBytes(inBuffer.Length);
			if (!fLZ.isle)
			{
				Array.Reverse<byte>(array);
			}
		}
		if (level < 1)
		{
			level = 1;
		}
		if (level > 2)
		{
			level = 2;
		}
		IntPtr intPtr = fLZ.fLZcompressBuffer(gchandle.AddrOfPinnedObject(), inBuffer.Length, level, ref num);
		gchandle.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			fLZ.fLZreleaseBuffer(intPtr);
			return null;
		}
		byte[] array2 = new byte[num + num2];
		if (includeSize)
		{
			for (int i = 0; i < 4; i++)
			{
				array2[i + num] = array[i];
			}
		}
		Marshal.Copy(intPtr, array2, 0, num);
		fLZ.fLZreleaseBuffer(intPtr);
		return array2;
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00004AF0 File Offset: 0x00002CF0
	public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = true, int customLength = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = inBuffer.Length;
		int num2 = 0;
		int num3;
		if (useFooter)
		{
			num -= 4;
			num3 = BitConverter.ToInt32(inBuffer, num);
			if (inBuffer.Length > num3)
			{
				num2 = inBuffer.Length - num3;
			}
		}
		else
		{
			num3 = customLength;
			if (inBuffer.Length > outBuffer.Length)
			{
				num2 = inBuffer.Length - outBuffer.Length;
			}
		}
		Array.Resize<byte>(ref outBuffer, num3);
		GCHandle gchandle2 = GCHandle.Alloc(outBuffer, 3);
		bool flag = fLZ.fLZdecompressBuffer(gchandle.AddrOfPinnedObject(), num3 + num2, gchandle2.AddrOfPinnedObject()) != 0;
		gchandle.Free();
		gchandle2.Free();
		return !flag;
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00004B78 File Offset: 0x00002D78
	public static int decompressBufferFixed(byte[] inBuffer, ref byte[] outBuffer, bool safe = true, bool useFooter = true, int customLength = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = inBuffer.Length;
		int num2 = 0;
		int num3;
		if (useFooter)
		{
			num -= 4;
			num3 = BitConverter.ToInt32(inBuffer, num);
			if (inBuffer.Length > num3)
			{
				num2 = inBuffer.Length - num3;
			}
		}
		else
		{
			num3 = customLength;
			if (inBuffer.Length > outBuffer.Length)
			{
				num2 = inBuffer.Length - outBuffer.Length;
			}
		}
		if (num3 > outBuffer.Length)
		{
			if (safe)
			{
				return -101;
			}
			num3 = outBuffer.Length;
		}
		GCHandle gchandle2 = GCHandle.Alloc(outBuffer, 3);
		int num4 = fLZ.fLZdecompressBuffer(gchandle.AddrOfPinnedObject(), num3 + num2, gchandle2.AddrOfPinnedObject());
		gchandle.Free();
		gchandle2.Free();
		if (safe && num4 != 0)
		{
			return -101;
		}
		return num3;
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00004C14 File Offset: 0x00002E14
	public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = true, int customLength = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = inBuffer.Length;
		int num2 = 0;
		int num3;
		if (useFooter)
		{
			num -= 4;
			num3 = BitConverter.ToInt32(inBuffer, num);
			if (inBuffer.Length > num3)
			{
				num2 = inBuffer.Length - num3;
			}
		}
		else
		{
			num3 = customLength;
			if (inBuffer.Length > customLength)
			{
				num2 = inBuffer.Length - customLength;
			}
		}
		byte[] array = new byte[num3];
		GCHandle gchandle2 = GCHandle.Alloc(array, 3);
		bool flag = fLZ.fLZdecompressBuffer(gchandle.AddrOfPinnedObject(), num3 + num2, gchandle2.AddrOfPinnedObject()) != 0;
		gchandle.Free();
		gchandle2.Free();
		if (flag)
		{
			return null;
		}
		return array;
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00004C98 File Offset: 0x00002E98
	public static IEnumerator downloadFlzFileNative(string url, Action<bool> downloadDone, Action<IntPtr> pointer = null, Action<int> fileSize = null)
	{
		UnityWebRequest wr = UnityWebRequest.Head(url);
		fLZ.nativeBufferIsBeingUsed = true;
		yield return wr.SendWebRequest();
		string responseHeader = wr.GetResponseHeader("Content-Length");
		fLZ.nativeBufferIsBeingUsed = false;
		if (wr.result == 2 || wr.result == 3)
		{
			Debug.LogError("Error While Getting Length: " + wr.error);
		}
		else if (!fLZ.nativeBufferIsBeingUsed)
		{
			int zipSize = Convert.ToInt32(responseHeader);
			if (zipSize > 0)
			{
				fLZ.nativeBuffer = fLZ.create_Buffer(zipSize);
				fLZ.nativeBufferIsBeingUsed = true;
				byte[] buffer = new byte[2048];
				fLZ.nativeOffset = 0;
				using (UnityWebRequest wwwSK = UnityWebRequest.Get(url))
				{
					wwwSK.downloadHandler = new fLZ.CustomWebRequest3(buffer);
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
							pointer.Invoke(fLZ.nativeBuffer);
							fileSize.Invoke(zipSize);
						}
						fLZ.nativeBufferIsBeingUsed = false;
						fLZ.nativeOffset = 0;
						fLZ.nativeBuffer = IntPtr.Zero;
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

	// Token: 0x04000048 RID: 72
	internal static bool isle = BitConverter.IsLittleEndian;

	// Token: 0x04000049 RID: 73
	private const string libname = "libfastlz";

	// Token: 0x0400004A RID: 74
	public static IntPtr nativeBuffer = IntPtr.Zero;

	// Token: 0x0400004B RID: 75
	public static bool nativeBufferIsBeingUsed = false;

	// Token: 0x0400004C RID: 76
	public static int nativeOffset = 0;

	// Token: 0x0200000C RID: 12
	public class CustomWebRequest3 : DownloadHandlerScript
	{
		// Token: 0x06000059 RID: 89 RVA: 0x00003EDD File Offset: 0x000020DD
		public CustomWebRequest3()
		{
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003EE5 File Offset: 0x000020E5
		public CustomWebRequest3(byte[] buffer) : base(buffer)
		{
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003EEE File Offset: 0x000020EE
		protected override byte[] GetData()
		{
			return null;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004CE0 File Offset: 0x00002EE0
		protected override bool ReceiveData(byte[] bytesFromServer, int dataLength)
		{
			if (bytesFromServer == null || bytesFromServer.Length < 1)
			{
				Debug.Log("CustomWebRequest: Received a null/empty buffer");
				return false;
			}
			GCHandle gchandle = fLZ.gcA(bytesFromServer);
			fLZ.addTo_Buffer(fLZ.nativeBuffer, fLZ.nativeOffset, gchandle.AddrOfPinnedObject(), dataLength);
			fLZ.nativeOffset += dataLength;
			gchandle.Free();
			return true;
		}
	}
}
