using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x020002B3 RID: 691
public class DataCompressionController : MonoBehaviour
{
	// Token: 0x17000077 RID: 119
	// (get) Token: 0x06000F6A RID: 3946 RVA: 0x000DBF3D File Offset: 0x000DA13D
	public static DataCompressionController Instance
	{
		get
		{
			return DataCompressionController._instance;
		}
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x000DBF44 File Offset: 0x000DA144
	private void Awake()
	{
		if (DataCompressionController._instance != null && DataCompressionController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		DataCompressionController._instance = this;
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x000DBF74 File Offset: 0x000DA174
	public Task<bool> CompressAndSaveDataAsync<T>(T input, string filePath, int compressionQuality = 9) where T : class
	{
		DataCompressionController.<CompressAndSaveDataAsync>d__4<T> <CompressAndSaveDataAsync>d__;
		<CompressAndSaveDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<CompressAndSaveDataAsync>d__.input = input;
		<CompressAndSaveDataAsync>d__.filePath = filePath;
		<CompressAndSaveDataAsync>d__.compressionQuality = compressionQuality;
		<CompressAndSaveDataAsync>d__.<>1__state = -1;
		<CompressAndSaveDataAsync>d__.<>t__builder.Start<DataCompressionController.<CompressAndSaveDataAsync>d__4<T>>(ref <CompressAndSaveDataAsync>d__);
		return <CompressAndSaveDataAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x000DBFC8 File Offset: 0x000DA1C8
	public Task<bool> LoadCompressedDataAsync<T>(string filePath, Action<T> onComplete) where T : class
	{
		DataCompressionController.<LoadCompressedDataAsync>d__5<T> <LoadCompressedDataAsync>d__;
		<LoadCompressedDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<LoadCompressedDataAsync>d__.filePath = filePath;
		<LoadCompressedDataAsync>d__.onComplete = onComplete;
		<LoadCompressedDataAsync>d__.<>1__state = -1;
		<LoadCompressedDataAsync>d__.<>t__builder.Start<DataCompressionController.<LoadCompressedDataAsync>d__5<T>>(ref <LoadCompressedDataAsync>d__);
		return <LoadCompressedDataAsync>d__.<>t__builder.Task;
	}

	// Token: 0x040012C7 RID: 4807
	private static DataCompressionController _instance;
}
