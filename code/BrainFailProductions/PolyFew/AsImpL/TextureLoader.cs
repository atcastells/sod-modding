using System;
using System.IO;
using UnityEngine;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x02000927 RID: 2343
	public class TextureLoader : MonoBehaviour
	{
		// Token: 0x060031CE RID: 12750 RVA: 0x00221BBF File Offset: 0x0021FDBF
		public static Texture2D LoadTextureFromUrl(string url)
		{
			if (url.StartsWith("file:///"))
			{
				url = url.Substring("file:///".Length);
			}
			else
			{
				url = Path.GetFullPath(url);
			}
			return TextureLoader.LoadTexture(url);
		}

		// Token: 0x060031CF RID: 12751 RVA: 0x00221BF0 File Offset: 0x0021FDF0
		public static Texture2D LoadTexture(string fileName)
		{
			string text = Path.GetExtension(fileName).ToLower();
			if (text == ".png" || text == ".jpg")
			{
				Texture2D texture2D = new Texture2D(1, 1);
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(fileName));
				return texture2D;
			}
			if (text == ".dds")
			{
				return TextureLoader.LoadDDSManual(fileName);
			}
			if (text == ".tga")
			{
				return TextureLoader.LoadTGA(fileName);
			}
			Debug.Log("texture not supported : " + fileName);
			return null;
		}

		// Token: 0x060031D0 RID: 12752 RVA: 0x00221C74 File Offset: 0x0021FE74
		public static Texture2D LoadTGA(string fileName)
		{
			Texture2D result;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				result = TextureLoader.LoadTGA(fileStream);
			}
			return result;
		}

		// Token: 0x060031D1 RID: 12753 RVA: 0x00221CAC File Offset: 0x0021FEAC
		public static Texture2D LoadDDSManual(string ddsPath)
		{
			Texture2D result;
			try
			{
				byte[] array = File.ReadAllBytes(ddsPath);
				if (array[4] != 124)
				{
					throw new Exception("Invalid DDS DXTn texture. Unable to read");
				}
				int num = (int)array[13] * 256 + (int)array[12];
				int num2 = (int)array[17] * 256 + (int)array[16];
				byte b = array[87];
				TextureFormat textureFormat = 12;
				if (b == 49)
				{
					textureFormat = 10;
				}
				if (b == 53)
				{
					textureFormat = 12;
				}
				int num3 = 128;
				byte[] array2 = new byte[array.Length - num3];
				Buffer.BlockCopy(array, num3, array2, 0, array.Length - num3);
				FileInfo fileInfo = new FileInfo(ddsPath);
				Texture2D texture2D = new Texture2D(num2, num, textureFormat, false);
				texture2D.LoadRawTextureData(array2);
				texture2D.Apply();
				texture2D.name = fileInfo.Name;
				result = texture2D;
			}
			catch (Exception ex)
			{
				string text = "Could not load DDS: ";
				Exception ex2 = ex;
				Debug.LogError(text + ((ex2 != null) ? ex2.ToString() : null));
				result = new Texture2D(8, 8);
			}
			return result;
		}

		// Token: 0x060031D2 RID: 12754 RVA: 0x00221D94 File Offset: 0x0021FF94
		public static Texture2D LoadTGA(Stream TGAStream)
		{
			Texture2D result;
			try
			{
				using (BinaryReader binaryReader = new BinaryReader(TGAStream))
				{
					TextureLoader.TgaHeader tgaHeader = TextureLoader.LoadTgaHeader(binaryReader);
					short num = (short)tgaHeader.width;
					short num2 = (short)tgaHeader.height;
					int bits = (int)tgaHeader.bits;
					bool flag = (tgaHeader.descriptor & 32) == 32;
					Texture2D texture2D = new Texture2D((int)num, (int)num2);
					Color32[] array = new Color32[(int)(num * num2)];
					int num3 = (int)(num * num2);
					if (bits == 32)
					{
						for (int i = 1; i <= (int)num2; i++)
						{
							for (int j = 0; j < (int)num; j++)
							{
								byte b = binaryReader.ReadByte();
								byte b2 = binaryReader.ReadByte();
								byte b3 = binaryReader.ReadByte();
								byte b4 = binaryReader.ReadByte();
								int num4;
								if (flag)
								{
									num4 = num3 - i * (int)num + j;
								}
								else
								{
									num4 = num3 - ((int)num2 - i + 1) * (int)num + j;
								}
								array[num4] = new Color32(b3, b2, b, b4);
							}
						}
					}
					else
					{
						if (bits != 24)
						{
							throw new Exception("TGA texture had non 32/24 bit depth.");
						}
						for (int k = 1; k <= (int)num2; k++)
						{
							for (int l = 0; l < (int)num; l++)
							{
								byte b5 = binaryReader.ReadByte();
								byte b6 = binaryReader.ReadByte();
								byte b7 = binaryReader.ReadByte();
								int num5;
								if (flag)
								{
									num5 = num3 - k * (int)num + l;
								}
								else
								{
									num5 = num3 - ((int)num2 - k + 1) * (int)num + l;
								}
								array[num5] = new Color32(b7, b6, b5, byte.MaxValue);
							}
						}
					}
					texture2D.SetPixels32(array);
					texture2D.Apply();
					result = texture2D;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex);
				result = null;
			}
			return result;
		}

		// Token: 0x060031D3 RID: 12755 RVA: 0x00221F54 File Offset: 0x00220154
		private static TextureLoader.TgaHeader LoadTgaHeader(BinaryReader r)
		{
			TextureLoader.TgaHeader tgaHeader = new TextureLoader.TgaHeader();
			r.BaseStream.Seek(0L, 1);
			tgaHeader.identSize = r.ReadByte();
			tgaHeader.colorMapType = r.ReadByte();
			tgaHeader.imageType = r.ReadByte();
			tgaHeader.colorMapStart = r.ReadUInt16();
			tgaHeader.colorMapLength = r.ReadUInt16();
			tgaHeader.colorMapBits = r.ReadByte();
			tgaHeader.xStart = r.ReadUInt16();
			tgaHeader.ySstart = r.ReadUInt16();
			tgaHeader.width = r.ReadUInt16();
			tgaHeader.height = r.ReadUInt16();
			tgaHeader.bits = r.ReadByte();
			tgaHeader.descriptor = r.ReadByte();
			Debug.LogFormat("TGA descriptor = {0}", new object[]
			{
				tgaHeader.descriptor
			});
			if (tgaHeader.imageType == 0)
			{
				new Exception("TGA image contains no data.");
			}
			if (tgaHeader.imageType > 10)
			{
				new Exception("compressed TGA not supported.");
			}
			if (tgaHeader.imageType == 1 || tgaHeader.imageType == 9)
			{
				new Exception("color indexed TGA not supported.");
			}
			if (tgaHeader.bits != 24 && tgaHeader.bits != 32)
			{
				throw new Exception("only 24/32 bits TGA supported.");
			}
			if (tgaHeader.width <= 0 || tgaHeader.height <= 0)
			{
				throw new Exception("TGA texture has invalid size.");
			}
			r.BaseStream.Seek((long)((ulong)tgaHeader.identSize), 1);
			return tgaHeader;
		}

		// Token: 0x02000928 RID: 2344
		private class TgaHeader
		{
			// Token: 0x04004D4A RID: 19786
			public byte identSize;

			// Token: 0x04004D4B RID: 19787
			public byte colorMapType;

			// Token: 0x04004D4C RID: 19788
			public byte imageType;

			// Token: 0x04004D4D RID: 19789
			public ushort colorMapStart;

			// Token: 0x04004D4E RID: 19790
			public ushort colorMapLength;

			// Token: 0x04004D4F RID: 19791
			public byte colorMapBits;

			// Token: 0x04004D50 RID: 19792
			public ushort xStart;

			// Token: 0x04004D51 RID: 19793
			public ushort ySstart;

			// Token: 0x04004D52 RID: 19794
			public ushort width;

			// Token: 0x04004D53 RID: 19795
			public ushort height;

			// Token: 0x04004D54 RID: 19796
			public byte bits;

			// Token: 0x04004D55 RID: 19797
			public byte descriptor;
		}
	}
}
