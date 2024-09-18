using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000011 RID: 17
public class LZ4
{
	// Token: 0x06000073 RID: 115
	[DllImport("liblz4")]
	internal static extern int LZ4DecompressFile(string inFile, string outFile, IntPtr bytes, IntPtr FileBuffer, int fileBufferLength);

	// Token: 0x06000074 RID: 116
	[DllImport("liblz4")]
	internal static extern int LZ4CompressFile(string inFile, string outFile, int level, IntPtr percentage, ref float rate);

	// Token: 0x06000075 RID: 117
	[DllImport("liblz4")]
	public static extern void LZ4releaseBuffer(IntPtr buffer);

	// Token: 0x06000076 RID: 118
	[DllImport("liblz4")]
	public static extern IntPtr LZ4Create_Buffer(int size);

	// Token: 0x06000077 RID: 119
	[DllImport("liblz4")]
	private static extern void LZ4AddTo_Buffer(IntPtr destination, int offset, IntPtr buffer, int len);

	// Token: 0x06000078 RID: 120
	[DllImport("liblz4")]
	internal static extern IntPtr LZ4CompressBuffer(IntPtr buffer, int bufferLength, ref int v, int level);

	// Token: 0x06000079 RID: 121
	[DllImport("liblz4")]
	internal static extern int LZ4DecompressBuffer(IntPtr buffer, IntPtr outbuffer, int bufferLength);

	// Token: 0x0600007A RID: 122 RVA: 0x00003887 File Offset: 0x00001A87
	internal static GCHandle gcA(object o)
	{
		return GCHandle.Alloc(o, 3);
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00003884 File Offset: 0x00001A84
	public static int setFilePermissions(string filePath, string _user, string _group, string _other)
	{
		return -1;
	}

	// Token: 0x0600007C RID: 124 RVA: 0x000055C0 File Offset: 0x000037C0
	private static bool checkObject(object fileBuffer, string filePath, ref GCHandle fbuf, ref IntPtr fileBufferPointer, ref int fileBufferLength)
	{
		if (fileBuffer is byte[])
		{
			byte[] array = (byte[])fileBuffer;
			fbuf = LZ4.gcA(array);
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

	// Token: 0x0600007D RID: 125 RVA: 0x00005614 File Offset: 0x00003814
	public static float compress(string inFile, string outFile, int level, float[] progress)
	{
		if (level < 1)
		{
			level = 1;
		}
		if (level > 9)
		{
			level = 9;
		}
		float result = 0f;
		if (progress == null)
		{
			progress = new float[1];
		}
		progress[0] = 0f;
		GCHandle gchandle = GCHandle.Alloc(progress, 3);
		bool flag = LZ4.LZ4CompressFile(inFile, outFile, level, gchandle.AddrOfPinnedObject(), ref result) != 0;
		gchandle.Free();
		if (flag)
		{
			return -1f;
		}
		return result;
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00005674 File Offset: 0x00003874
	public static int decompress(string inFile, string outFile, ulong[] bytes, object fileBuffer = null)
	{
		if (bytes == null)
		{
			bytes = new ulong[1];
		}
		bytes[0] = 0UL;
		GCHandle gchandle = GCHandle.Alloc(bytes, 3);
		int result = LZ4.LZ4DecompressFile(inFile, outFile, gchandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
		gchandle.Free();
		return result;
	}

	// Token: 0x0600007F RID: 127 RVA: 0x000056B4 File Offset: 0x000038B4
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
			if (!LZ4.isle)
			{
				Array.Reverse<byte>(array);
			}
		}
		if (level < 1)
		{
			level = 1;
		}
		if (level > 9)
		{
			level = 9;
		}
		IntPtr intPtr = LZ4.LZ4CompressBuffer(gchandle.AddrOfPinnedObject(), inBuffer.Length, ref num, level);
		gchandle.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			LZ4.LZ4releaseBuffer(intPtr);
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
		LZ4.LZ4releaseBuffer(intPtr);
		return true;
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00005774 File Offset: 0x00003974
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
			if (!LZ4.isle)
			{
				Array.Reverse<byte>(array);
			}
		}
		if (level < 1)
		{
			level = 1;
		}
		if (level > 9)
		{
			level = 9;
		}
		IntPtr intPtr = LZ4.LZ4CompressBuffer(gchandle.AddrOfPinnedObject(), inBuffer.Length, ref num, level);
		gchandle.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			LZ4.LZ4releaseBuffer(intPtr);
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
		LZ4.LZ4releaseBuffer(intPtr);
		return array2;
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00005838 File Offset: 0x00003A38
	public static int compressBufferPartialFixed(byte[] inBuffer, ref byte[] outBuffer, int outBufferPartialIndex, int level, bool includeSize = true)
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
			if (!LZ4.isle)
			{
				Array.Reverse<byte>(array);
			}
		}
		if (level < 1)
		{
			level = 1;
		}
		if (level > 9)
		{
			level = 9;
		}
		IntPtr intPtr = LZ4.LZ4CompressBuffer(gchandle.AddrOfPinnedObject(), inBuffer.Length, ref num, level);
		gchandle.Free();
		if (num == 0 || intPtr == IntPtr.Zero)
		{
			LZ4.LZ4releaseBuffer(intPtr);
			return 0;
		}
		if (includeSize)
		{
			for (int i = 0; i < 4; i++)
			{
				outBuffer[outBufferPartialIndex + num + i] = array[i];
			}
		}
		Marshal.Copy(intPtr, outBuffer, outBufferPartialIndex, num);
		LZ4.LZ4releaseBuffer(intPtr);
		return num + num2;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x000058F4 File Offset: 0x00003AF4
	public static int decompressBufferPartialFixed(byte[] inBuffer, ref byte[] outBuffer, int partialIndex, int compressedBufferSize, bool safe = true, bool useFooter = true, int customLength = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = partialIndex + compressedBufferSize;
		int num2;
		if (useFooter)
		{
			num -= 4;
			num2 = BitConverter.ToInt32(inBuffer, num);
		}
		else
		{
			num2 = customLength;
		}
		if (num2 > outBuffer.Length)
		{
			if (safe)
			{
				return -101;
			}
			num2 = outBuffer.Length;
		}
		GCHandle gchandle2 = GCHandle.Alloc(outBuffer, 3);
		LZ4.LZ4DecompressBuffer(new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + (long)partialIndex), gchandle2.AddrOfPinnedObject(), num2);
		gchandle.Free();
		gchandle2.Free();
		return num2;
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00005978 File Offset: 0x00003B78
	public static bool decompressBuffer(byte[] inBuffer, ref byte[] outBuffer, bool useFooter = true, int customLength = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = inBuffer.Length;
		int num2;
		if (useFooter)
		{
			num -= 4;
			num2 = BitConverter.ToInt32(inBuffer, num);
		}
		else
		{
			num2 = customLength;
		}
		Array.Resize<byte>(ref outBuffer, num2);
		GCHandle gchandle2 = GCHandle.Alloc(outBuffer, 3);
		int num3 = LZ4.LZ4DecompressBuffer(gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), num2);
		gchandle.Free();
		gchandle2.Free();
		return num3 == num;
	}

	// Token: 0x06000084 RID: 132 RVA: 0x000059E0 File Offset: 0x00003BE0
	public static int decompressBufferFixed(byte[] inBuffer, ref byte[] outBuffer, bool safe = true, bool useFooter = true, int customLength = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = inBuffer.Length;
		int num2;
		if (useFooter)
		{
			num -= 4;
			num2 = BitConverter.ToInt32(inBuffer, num);
		}
		else
		{
			num2 = customLength;
		}
		if (num2 > outBuffer.Length)
		{
			if (safe)
			{
				return -101;
			}
			num2 = outBuffer.Length;
		}
		GCHandle gchandle2 = GCHandle.Alloc(outBuffer, 3);
		int num3 = LZ4.LZ4DecompressBuffer(gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), num2);
		gchandle.Free();
		gchandle2.Free();
		if (safe && num3 != num)
		{
			return -101;
		}
		return num2;
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00005A5C File Offset: 0x00003C5C
	public static byte[] decompressBuffer(byte[] inBuffer, bool useFooter = true, int customLength = 0)
	{
		GCHandle gchandle = GCHandle.Alloc(inBuffer, 3);
		int num = inBuffer.Length;
		int num2;
		if (useFooter)
		{
			num -= 4;
			num2 = BitConverter.ToInt32(inBuffer, num);
		}
		else
		{
			num2 = customLength;
		}
		byte[] array = new byte[num2];
		GCHandle gchandle2 = GCHandle.Alloc(array, 3);
		int num3 = LZ4.LZ4DecompressBuffer(gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), num2);
		gchandle.Free();
		gchandle2.Free();
		if (num3 != num)
		{
			return null;
		}
		return array;
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00005AC2 File Offset: 0x00003CC2
	public static IEnumerator downloadLZ4FileNative(string url, Action<bool> downloadDone, Action<IntPtr> pointer = null, Action<int> fileSize = null)
	{
		UnityWebRequest wr = UnityWebRequest.Head(url);
		LZ4.nativeBufferIsBeingUsed = true;
		yield return wr.SendWebRequest();
		string responseHeader = wr.GetResponseHeader("Content-Length");
		LZ4.nativeBufferIsBeingUsed = false;
		if (wr.result == 2 || wr.result == 3)
		{
			Debug.LogError("Error While Getting Length: " + wr.error);
		}
		else if (!LZ4.nativeBufferIsBeingUsed)
		{
			int zipSize = Convert.ToInt32(responseHeader);
			if (zipSize > 0)
			{
				LZ4.nativeBuffer = LZ4.LZ4Create_Buffer(zipSize);
				LZ4.nativeBufferIsBeingUsed = true;
				byte[] buffer = new byte[2048];
				LZ4.nativeOffset = 0;
				using (UnityWebRequest wwwSK = UnityWebRequest.Get(url))
				{
					wwwSK.downloadHandler = new LZ4.CustomWebRequest4(buffer);
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
							pointer.Invoke(LZ4.nativeBuffer);
							fileSize.Invoke(zipSize);
						}
						LZ4.nativeBufferIsBeingUsed = false;
						LZ4.nativeOffset = 0;
						LZ4.nativeBuffer = IntPtr.Zero;
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

	// Token: 0x04000069 RID: 105
	internal static bool isle = BitConverter.IsLittleEndian;

	// Token: 0x0400006A RID: 106
	private const string libname = "liblz4";

	// Token: 0x0400006B RID: 107
	public static IntPtr nativeBuffer = IntPtr.Zero;

	// Token: 0x0400006C RID: 108
	public static bool nativeBufferIsBeingUsed = false;

	// Token: 0x0400006D RID: 109
	public static int nativeOffset = 0;

	// Token: 0x02000012 RID: 18
	public class CustomWebRequest4 : DownloadHandlerScript
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00003EDD File Offset: 0x000020DD
		public CustomWebRequest4()
		{
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003EE5 File Offset: 0x000020E5
		public CustomWebRequest4(byte[] buffer) : base(buffer)
		{
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003EEE File Offset: 0x000020EE
		protected override byte[] GetData()
		{
			return null;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005B08 File Offset: 0x00003D08
		protected override bool ReceiveData(byte[] bytesFromServer, int dataLength)
		{
			if (bytesFromServer == null || bytesFromServer.Length < 1)
			{
				Debug.Log("CustomWebRequest: Received a null/empty buffer");
				return false;
			}
			GCHandle gchandle = LZ4.gcA(bytesFromServer);
			LZ4.LZ4AddTo_Buffer(LZ4.nativeBuffer, LZ4.nativeOffset, gchandle.AddrOfPinnedObject(), dataLength);
			LZ4.nativeOffset += dataLength;
			gchandle.Free();
			return true;
		}
	}
}
