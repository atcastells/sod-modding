using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BrainFailProductions.PolyFew
{
	// Token: 0x020008EA RID: 2282
	public static class SystemServices
	{
		// Token: 0x060030CC RID: 12492 RVA: 0x00217DF8 File Offset: 0x00215FF8
		private static void SetPatterns()
		{
			SystemServices.regexPatterns.netError = "<neterror>";
			SystemServices.regexPatterns.nullOrEmpty = "<nullorempty>";
			SystemServices.regexPatterns.generalError = "<generalerror>";
			SystemServices.regexPatterns.apiMistmatch = "<apimismatch>";
			SystemServices.regexPatterns.parametersMismatch = "<parametersmismatch>";
			SystemServices.regexPatterns.nothing = "";
		}

		// Token: 0x060030CD RID: 12493 RVA: 0x00217E5F File Offset: 0x0021605F
		public static IEnumerator UnityAsyncGETRequest(string encodedUrl, Action<string, long> callback, int? timeout = null, Dictionary<string, string> headers = null)
		{
			SystemServices.SetPatterns();
			UnityWebRequest webRequest = new UnityWebRequest(encodedUrl);
			webRequest.timeout = ((timeout == null) ? webRequest.timeout : timeout.Value);
			webRequest.method = "GET";
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			webRequest.downloadHandler = downloadHandler;
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					webRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
			yield return webRequest.SendWebRequest();
			long responseCode = webRequest.responseCode;
			if (webRequest.result == 3 || webRequest.result == 2)
			{
				callback.Invoke("<neterror>" + webRequest.error, responseCode);
			}
			else if (string.IsNullOrEmpty(webRequest.downloadHandler.text))
			{
				callback.Invoke("<nullorempty>Error! server returned an empty response.", responseCode);
			}
			else
			{
				callback.Invoke(webRequest.downloadHandler.text, responseCode);
			}
			yield break;
		}

		// Token: 0x060030CE RID: 12494 RVA: 0x00217E84 File Offset: 0x00216084
		public static void UnityBlockingGETRequest(string encodedUrl, Action<string, long> callback, int? timeout = null, Dictionary<string, string> headers = null)
		{
			SystemServices.SetPatterns();
			UnityWebRequest unityWebRequest = new UnityWebRequest(encodedUrl);
			unityWebRequest.timeout = ((timeout == null) ? unityWebRequest.timeout : timeout.Value);
			unityWebRequest.method = "GET";
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			unityWebRequest.downloadHandler = downloadHandler;
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					unityWebRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
			unityWebRequest.SendWebRequest();
			while (!unityWebRequest.isDone)
			{
			}
			long responseCode = unityWebRequest.responseCode;
			if (unityWebRequest.result == 3 || unityWebRequest.result == 2)
			{
				callback.Invoke("<neterror>" + unityWebRequest.error, responseCode);
				return;
			}
			if (string.IsNullOrEmpty(unityWebRequest.downloadHandler.text))
			{
				callback.Invoke("<nullorempty>Error! server returned an empty response.", responseCode);
				return;
			}
			callback.Invoke(unityWebRequest.downloadHandler.text, responseCode);
		}

		// Token: 0x060030CF RID: 12495 RVA: 0x00217F98 File Offset: 0x00216198
		public static void UnityBlockingPOSTRequest(string baseUrl, Action<string, long> callback, byte[] data, int? timeout = null, Dictionary<string, string> headers = null)
		{
			SystemServices.SetPatterns();
			UnityWebRequest unityWebRequest = new UnityWebRequest(baseUrl);
			unityWebRequest.timeout = ((timeout == null) ? unityWebRequest.timeout : timeout.Value);
			unityWebRequest.method = "POST";
			UploadHandlerRaw uploadHandler = new UploadHandlerRaw(data);
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			unityWebRequest.uploadHandler = uploadHandler;
			unityWebRequest.downloadHandler = downloadHandler;
			unityWebRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					unityWebRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
			unityWebRequest.SendWebRequest();
			while (!unityWebRequest.isDone)
			{
			}
			long responseCode = unityWebRequest.responseCode;
			if (unityWebRequest.result == 3 || unityWebRequest.result == 2)
			{
				callback.Invoke("<neterror>" + unityWebRequest.error, responseCode);
				return;
			}
			if (string.IsNullOrEmpty(unityWebRequest.downloadHandler.text))
			{
				callback.Invoke("<nullorempty>Error! server returned an empty response.", responseCode);
				return;
			}
			callback.Invoke(unityWebRequest.downloadHandler.text, responseCode);
		}

		// Token: 0x060030D0 RID: 12496 RVA: 0x002180CC File Offset: 0x002162CC
		public static IEnumerator UnityAsyncPOSTRequest(string baseUrl, Action<string, long> callback, byte[] data, int? timeout = null, Dictionary<string, string> headers = null)
		{
			SystemServices.SetPatterns();
			UnityWebRequest webRequest = new UnityWebRequest(baseUrl);
			webRequest.timeout = ((timeout == null) ? webRequest.timeout : timeout.Value);
			webRequest.method = "POST";
			UploadHandlerRaw uploadHandler = new UploadHandlerRaw(data);
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			webRequest.uploadHandler = uploadHandler;
			webRequest.downloadHandler = downloadHandler;
			webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					webRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
			yield return webRequest.SendWebRequest();
			long responseCode = webRequest.responseCode;
			if (webRequest.result == 3 || webRequest.result == 2)
			{
				callback.Invoke("<neterror>" + webRequest.error, responseCode);
			}
			else if (string.IsNullOrEmpty(webRequest.downloadHandler.text))
			{
				callback.Invoke("<nullorempty>Error! server returned an empty response.", responseCode);
			}
			else
			{
				callback.Invoke(webRequest.downloadHandler.text, responseCode);
			}
			yield break;
		}

		// Token: 0x060030D1 RID: 12497 RVA: 0x002180F8 File Offset: 0x002162F8
		public static Task SendHTTPRequestAsync(string baseUrl, SystemServices.HTTPMethod requestMethod, Action<string, HttpStatusCode?> callback, Dictionary<string, string> requestParameters, byte[] postData, string contentType, int? timeout = null, Dictionary<string, string> header = null)
		{
			SystemServices.<SendHTTPRequestAsync>d__7 <SendHTTPRequestAsync>d__;
			<SendHTTPRequestAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendHTTPRequestAsync>d__.baseUrl = baseUrl;
			<SendHTTPRequestAsync>d__.requestMethod = requestMethod;
			<SendHTTPRequestAsync>d__.callback = callback;
			<SendHTTPRequestAsync>d__.requestParameters = requestParameters;
			<SendHTTPRequestAsync>d__.postData = postData;
			<SendHTTPRequestAsync>d__.timeout = timeout;
			<SendHTTPRequestAsync>d__.header = header;
			<SendHTTPRequestAsync>d__.<>1__state = -1;
			<SendHTTPRequestAsync>d__.<>t__builder.Start<SystemServices.<SendHTTPRequestAsync>d__7>(ref <SendHTTPRequestAsync>d__);
			return <SendHTTPRequestAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060030D2 RID: 12498 RVA: 0x00218170 File Offset: 0x00216370
		public static void SendHTTPRequestBlocking(string baseUrl, SystemServices.HTTPMethod requestMethod, Action<string, HttpStatusCode?> callback, Dictionary<string, string> requestParameters, byte[] postData, string contentType, int? timeout = null, Dictionary<string, string> header = null)
		{
			SystemServices.SetPatterns();
			HttpWebResponse httpWebResponse = null;
			try
			{
				if (requestParameters != null && requestMethod.methodName == "GET")
				{
					string queryStringFromKeyValues = SystemServices.GetQueryStringFromKeyValues(requestParameters);
					baseUrl = baseUrl + "?" + queryStringFromKeyValues;
				}
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl);
				httpWebRequest.Timeout = ((timeout == null) ? 100000 : timeout.Value);
				httpWebRequest.Method = requestMethod.methodName;
				httpWebRequest.Headers = new WebHeaderCollection();
				httpWebRequest.AutomaticDecompression = 3;
				httpWebRequest.ContentType = contentType;
				if (header != null)
				{
					foreach (KeyValuePair<string, string> keyValuePair in header)
					{
						httpWebRequest.Headers.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
				if (requestParameters != null && requestMethod.methodName == "POST")
				{
					string queryStringFromKeyValues2 = SystemServices.GetQueryStringFromKeyValues(requestParameters);
					byte[] bytes = Encoding.ASCII.GetBytes(queryStringFromKeyValues2);
					httpWebRequest.ContentLength = (long)bytes.Length;
					using (Stream requestStream = httpWebRequest.GetRequestStream())
					{
						requestStream.Write(bytes, 0, bytes.Length);
						goto IL_140;
					}
				}
				if (requestParameters == null && requestMethod.methodName != "GET")
				{
					httpWebRequest.ContentLength = 0L;
				}
				IL_140:
				if (requestParameters == null && postData != null && requestMethod.methodName == "POST")
				{
					httpWebRequest.ContentLength = (long)postData.Length;
					using (Stream requestStream2 = httpWebRequest.GetRequestStream())
					{
						requestStream2.Write(postData, 0, postData.Length);
					}
				}
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse.StatusCode != 200)
				{
					callback.Invoke(SystemServices.regexPatterns.netError + "+" + httpWebResponse.StatusDescription, new HttpStatusCode?(httpWebResponse.StatusCode));
				}
				else
				{
					string text = new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
					callback.Invoke(text, new HttpStatusCode?(httpWebResponse.StatusCode));
				}
				httpWebResponse.Dispose();
			}
			catch (Exception ex)
			{
				HttpStatusCode? httpStatusCode = (httpWebResponse == null) ? default(HttpStatusCode?) : new HttpStatusCode?(httpWebResponse.StatusCode);
				if (ex.InnerException is WebException || ex.InnerException is SocketException)
				{
					WebException ex2 = ex as WebException;
					if (ex2.Status == 14)
					{
						callback.Invoke(SystemServices.regexPatterns.generalError + "+" + ex2.ToString(), httpStatusCode);
					}
					else
					{
						callback.Invoke(SystemServices.regexPatterns.netError + "+" + ex2.ToString(), httpStatusCode);
					}
				}
				else
				{
					callback.Invoke(SystemServices.regexPatterns.generalError + "+" + ex.ToString(), httpStatusCode);
				}
			}
		}

		// Token: 0x060030D3 RID: 12499 RVA: 0x0021849C File Offset: 0x0021669C
		public static Task AsyncResourceDownload(string resourceUrl, Action<byte[], string, HttpStatusCode?> callback, int? timeout = null)
		{
			SystemServices.<AsyncResourceDownload>d__9 <AsyncResourceDownload>d__;
			<AsyncResourceDownload>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AsyncResourceDownload>d__.resourceUrl = resourceUrl;
			<AsyncResourceDownload>d__.callback = callback;
			<AsyncResourceDownload>d__.timeout = timeout;
			<AsyncResourceDownload>d__.<>1__state = -1;
			<AsyncResourceDownload>d__.<>t__builder.Start<SystemServices.<AsyncResourceDownload>d__9>(ref <AsyncResourceDownload>d__);
			return <AsyncResourceDownload>d__.<>t__builder.Task;
		}

		// Token: 0x060030D4 RID: 12500 RVA: 0x002184F0 File Offset: 0x002166F0
		public static Task AsyncReachabilityCheck(string testUrl, Action<bool> callback)
		{
			SystemServices.<AsyncReachabilityCheck>d__10 <AsyncReachabilityCheck>d__;
			<AsyncReachabilityCheck>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AsyncReachabilityCheck>d__.testUrl = testUrl;
			<AsyncReachabilityCheck>d__.callback = callback;
			<AsyncReachabilityCheck>d__.<>1__state = -1;
			<AsyncReachabilityCheck>d__.<>t__builder.Start<SystemServices.<AsyncReachabilityCheck>d__10>(ref <AsyncReachabilityCheck>d__);
			return <AsyncReachabilityCheck>d__.<>t__builder.Task;
		}

		// Token: 0x060030D5 RID: 12501 RVA: 0x0021853C File Offset: 0x0021673C
		public static void BlockingReachabilityCheck(string url, Action<bool> callback)
		{
			SystemServices.HTTPMethod requestMethod = new SystemServices.HTTPMethod(SystemServices.HTTPMethod.HTTPMethods.GET);
			SystemServices.SendHTTPRequestBlocking(url, requestMethod, delegate(string response, HttpStatusCode? statusCode)
			{
				if (statusCode != null)
				{
					HttpStatusCode? httpStatusCode = statusCode;
					HttpStatusCode httpStatusCode2 = 200;
					if (httpStatusCode.GetValueOrDefault() == httpStatusCode2 & httpStatusCode != null)
					{
						callback.Invoke(true);
						return;
					}
				}
				callback.Invoke(false);
			}, null, null, "application/json", default(int?), null);
		}

		// Token: 0x060030D6 RID: 12502 RVA: 0x00218584 File Offset: 0x00216784
		public static SystemServices.MessagePatternPair ParseResponseMessage(string message)
		{
			string patternAppended = SystemServices.regexPatterns.nothing;
			string parsedMessage;
			if (Regex.IsMatch(message, SystemServices.regexPatterns.netError, 8))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.netError + "+", "");
				patternAppended = SystemServices.regexPatterns.netError;
			}
			else if (Regex.IsMatch(message, SystemServices.regexPatterns.apiMistmatch, 8))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.apiMistmatch + "+", "");
				patternAppended = SystemServices.regexPatterns.apiMistmatch;
			}
			else if (Regex.IsMatch(message, SystemServices.regexPatterns.generalError, 8))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.generalError + "+", "");
				patternAppended = SystemServices.regexPatterns.generalError;
			}
			else if (Regex.IsMatch(message, SystemServices.regexPatterns.parametersMismatch, 8))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.parametersMismatch + "+", "");
				patternAppended = SystemServices.regexPatterns.parametersMismatch;
			}
			else if (Regex.IsMatch(message, SystemServices.regexPatterns.nullOrEmpty, 8))
			{
				parsedMessage = message.Replace(SystemServices.regexPatterns.nullOrEmpty + "+", "");
				patternAppended = SystemServices.regexPatterns.nullOrEmpty;
			}
			else
			{
				parsedMessage = null;
				patternAppended = SystemServices.regexPatterns.nothing;
			}
			return new SystemServices.MessagePatternPair(patternAppended, parsedMessage);
		}

		// Token: 0x060030D7 RID: 12503 RVA: 0x002186FB File Offset: 0x002168FB
		public static bool IsSuccessStatusCode(long statusCode)
		{
			return (int)statusCode >= 200 && (int)statusCode <= 299;
		}

		// Token: 0x060030D8 RID: 12504 RVA: 0x00218714 File Offset: 0x00216914
		public static string GetQueryStringFromKeyValues(Dictionary<string, string> parameters)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in parameters)
			{
				list.Add(keyValuePair.Key + "=" + Uri.EscapeDataString(keyValuePair.Value));
			}
			return string.Join("&", list);
		}

		// Token: 0x060030D9 RID: 12505 RVA: 0x00218790 File Offset: 0x00216990
		public static Task RunDelayedCommand(float secs, Action command)
		{
			SystemServices.<RunDelayedCommand>d__17 <RunDelayedCommand>d__;
			<RunDelayedCommand>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RunDelayedCommand>d__.secs = secs;
			<RunDelayedCommand>d__.command = command;
			<RunDelayedCommand>d__.<>1__state = -1;
			<RunDelayedCommand>d__.<>t__builder.Start<SystemServices.<RunDelayedCommand>d__17>(ref <RunDelayedCommand>d__);
			return <RunDelayedCommand>d__.<>t__builder.Task;
		}

		// Token: 0x060030DA RID: 12506 RVA: 0x002187DC File Offset: 0x002169DC
		public static byte[] ReadAllBytes(Stream source)
		{
			long position = source.Position;
			source.Position = 0L;
			byte[] result;
			try
			{
				byte[] array = new byte[4096];
				int num = 0;
				int num2;
				while ((num2 = source.Read(array, num, array.Length - num)) > 0)
				{
					num += num2;
					if (num == array.Length)
					{
						int num3 = source.ReadByte();
						if (num3 != -1)
						{
							byte[] array2 = new byte[array.Length * 2];
							Buffer.BlockCopy(array, 0, array2, 0, array.Length);
							Buffer.SetByte(array2, num, (byte)num3);
							array = array2;
							num++;
						}
					}
				}
				byte[] array3 = array;
				if (array.Length != num)
				{
					array3 = new byte[num];
					Buffer.BlockCopy(array, 0, array3, 0, num);
				}
				result = array3;
			}
			finally
			{
				source.Position = position;
			}
			return result;
		}

		// Token: 0x060030DB RID: 12507 RVA: 0x00218898 File Offset: 0x00216A98
		public static Task WriteTextureAsync(Texture2D texture, SystemServices.ImageFormat format, string fileName, string path, Action<string> callback)
		{
			SystemServices.<WriteTextureAsync>d__19 <WriteTextureAsync>d__;
			<WriteTextureAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteTextureAsync>d__.texture = texture;
			<WriteTextureAsync>d__.format = format;
			<WriteTextureAsync>d__.fileName = fileName;
			<WriteTextureAsync>d__.path = path;
			<WriteTextureAsync>d__.callback = callback;
			<WriteTextureAsync>d__.<>1__state = -1;
			<WriteTextureAsync>d__.<>t__builder.Start<SystemServices.<WriteTextureAsync>d__19>(ref <WriteTextureAsync>d__);
			return <WriteTextureAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060030DC RID: 12508 RVA: 0x002188FC File Offset: 0x00216AFC
		public static Task WriteBytesAsync(byte[] data, string fullPath, Action<string> callback)
		{
			SystemServices.<WriteBytesAsync>d__20 <WriteBytesAsync>d__;
			<WriteBytesAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteBytesAsync>d__.data = data;
			<WriteBytesAsync>d__.fullPath = fullPath;
			<WriteBytesAsync>d__.callback = callback;
			<WriteBytesAsync>d__.<>1__state = -1;
			<WriteBytesAsync>d__.<>t__builder.Start<SystemServices.<WriteBytesAsync>d__20>(ref <WriteBytesAsync>d__);
			return <WriteBytesAsync>d__.<>t__builder.Task;
		}

		// Token: 0x04004BB9 RID: 19385
		public static SystemServices.RegexPatterns regexPatterns;

		// Token: 0x020008EB RID: 2283
		[Serializable]
		public struct RegexPatterns
		{
			// Token: 0x04004BBA RID: 19386
			public string netError;

			// Token: 0x04004BBB RID: 19387
			public string nullOrEmpty;

			// Token: 0x04004BBC RID: 19388
			public string generalError;

			// Token: 0x04004BBD RID: 19389
			public string apiMistmatch;

			// Token: 0x04004BBE RID: 19390
			public string parametersMismatch;

			// Token: 0x04004BBF RID: 19391
			public string nothing;
		}

		// Token: 0x020008EC RID: 2284
		public struct MessagePatternPair
		{
			// Token: 0x17000519 RID: 1305
			// (get) Token: 0x060030DE RID: 12510 RVA: 0x00218958 File Offset: 0x00216B58
			// (set) Token: 0x060030DD RID: 12509 RVA: 0x0021894F File Offset: 0x00216B4F
			public string patternAppended { readonly get; private set; }

			// Token: 0x1700051A RID: 1306
			// (get) Token: 0x060030E0 RID: 12512 RVA: 0x00218969 File Offset: 0x00216B69
			// (set) Token: 0x060030DF RID: 12511 RVA: 0x00218960 File Offset: 0x00216B60
			public string parsedMessage { readonly get; private set; }

			// Token: 0x060030E1 RID: 12513 RVA: 0x00218971 File Offset: 0x00216B71
			public MessagePatternPair(string patternAppended, string parsedMessage)
			{
				this.patternAppended = patternAppended;
				this.parsedMessage = parsedMessage;
			}
		}

		// Token: 0x020008ED RID: 2285
		public class HTTPMethod
		{
			// Token: 0x060030E2 RID: 12514 RVA: 0x00218981 File Offset: 0x00216B81
			public HTTPMethod(SystemServices.HTTPMethod.HTTPMethods method)
			{
				this.methodName = Enum.GetName(typeof(SystemServices.HTTPMethod.HTTPMethods), method);
			}

			// Token: 0x04004BC2 RID: 19394
			public readonly string methodName;

			// Token: 0x020008EE RID: 2286
			public enum HTTPMethods
			{
				// Token: 0x04004BC4 RID: 19396
				POST,
				// Token: 0x04004BC5 RID: 19397
				GET
			}
		}

		// Token: 0x020008EF RID: 2287
		public enum ImageFormat
		{
			// Token: 0x04004BC7 RID: 19399
			PNG,
			// Token: 0x04004BC8 RID: 19400
			JPG,
			// Token: 0x04004BC9 RID: 19401
			EXR
		}
	}
}
