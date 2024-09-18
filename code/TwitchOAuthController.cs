using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020004A3 RID: 1187
public class TwitchOAuthController : MonoBehaviour
{
	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x0600193C RID: 6460 RVA: 0x00174115 File Offset: 0x00172315
	public static TwitchOAuthController Instance
	{
		get
		{
			return TwitchOAuthController._instance;
		}
	}

	// Token: 0x0600193D RID: 6461 RVA: 0x0017411C File Offset: 0x0017231C
	private void Awake()
	{
		if (TwitchOAuthController._instance != null && TwitchOAuthController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		TwitchOAuthController._instance = this;
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x0017414A File Offset: 0x0017234A
	private void QueueAuthorizationToken()
	{
		this._tokenQueue.Enqueue(this._authToken);
		if (string.IsNullOrEmpty(this._authToken))
		{
			Debug.LogWarning("Twitch authorization token returned null or empty");
		}
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x00174174 File Offset: 0x00172374
	public string GetAuthToken()
	{
		if (string.IsNullOrEmpty(this._authToken))
		{
			Debug.LogWarning("Trying to retrieve empty authToken");
		}
		return this._authToken;
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x00174193 File Offset: 0x00172393
	public string GetClientID()
	{
		if (string.IsNullOrEmpty("bq0wyxhwa7xjlyomjjdv2o6wun6l2t"))
		{
			Debug.LogWarning("Trying to retrieve empty authToken");
		}
		return "bq0wyxhwa7xjlyomjjdv2o6wun6l2t";
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x001741B0 File Offset: 0x001723B0
	public void TryTwitchAuthorization()
	{
		this.InitiateTwitchAuth();
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x001741B8 File Offset: 0x001723B8
	public void InitiateTwitchAuth()
	{
		string[] array = new string[]
		{
			"chat:read",
			"moderator:read:chatters",
			"moderation:read",
			"channel:read:vips",
			"moderator:read:followers"
		};
		this._twitchAuthStateVerify = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
		string text = string.Concat(new string[]
		{
			"client_id=bq0wyxhwa7xjlyomjjdv2o6wun6l2t&redirect_uri=",
			UnityWebRequest.EscapeURL("http://localhost:8085/"),
			"&state=",
			this._twitchAuthStateVerify,
			"&response_type=token&scope=",
			string.Join("+", array)
		});
		this.StartLocalWebserver();
		Application.OpenURL("https://id.twitch.tv/oauth2/authorize?" + text);
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x00174284 File Offset: 0x00172484
	private void StartLocalWebserver()
	{
		HttpListener httpListener = new HttpListener();
		httpListener.Prefixes.Add("http://localhost:8085/");
		httpListener.Start();
		httpListener.BeginGetContext(new AsyncCallback(this.IncomingHttpRequest), httpListener);
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x001742C4 File Offset: 0x001724C4
	private void IncomingHttpRequest(IAsyncResult result)
	{
		Debug.Log("Incoming Http");
		HttpListener httpListener = (HttpListener)result.AsyncState;
		HttpListenerContext httpListenerContext = httpListener.EndGetContext(result);
		httpListener.BeginGetContext(new AsyncCallback(this.IncomingAuth), httpListener);
		HttpListenerRequest request = httpListenerContext.Request;
		HttpListenerResponse response = httpListenerContext.Response;
		string rawUrl = request.RawUrl;
		Group group = new Regex("\\berror=access_denied\\b").Match(rawUrl);
		string text = "";
		string text2;
		if (!group.Success)
		{
			text2 = TwitchAuthLandingPages.SuccessLandingPage;
			text = "document.addEventListener(\"DOMContentLoaded\", () =>{setTimeout(function() {window.close()}, 5000);});";
		}
		else
		{
			text2 = TwitchAuthLandingPages.RejectedLandingPage;
		}
		string text3 = string.Concat(new string[]
		{
			text2,
			"<script type=\"text/javascript\">var xhr = new XMLHttpRequest(); xhr.open(\"POST\", \"",
			UnityWebRequest.EscapeURL("http://localhost:8085/"),
			"\");xhr.send(window.location);",
			text,
			"</script></body></html>"
		});
		byte[] bytes = Encoding.UTF8.GetBytes(text3);
		response.ContentLength64 = (long)bytes.Length;
		Stream outputStream = response.OutputStream;
		outputStream.Write(bytes, 0, bytes.Length);
		outputStream.Close();
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x001743C0 File Offset: 0x001725C0
	private void IncomingAuth(IAsyncResult ar)
	{
		Debug.Log("Incoming auth");
		HttpListener httpListener = (HttpListener)ar.AsyncState;
		HttpListenerRequest request = httpListener.EndGetContext(ar).Request;
		string text;
		using (StreamReader streamReader = new StreamReader(request.InputStream, request.ContentEncoding))
		{
			text = streamReader.ReadToEnd();
			Debug.Log(text);
		}
		Match match = new Regex(".+#access_token=(.+)&scope.*state=(\\d+)").Match(text);
		if (match.Success)
		{
			this._hasAuth = true;
		}
		if (match.Groups[2].Value != this._twitchAuthStateVerify)
		{
			httpListener.Stop();
			return;
		}
		this._authToken = match.Groups[1].Value;
		if (!string.IsNullOrEmpty(this._authToken))
		{
			this.QueueAuthorizationToken();
		}
		httpListener.Stop();
	}

	// Token: 0x04002201 RID: 8705
	private static TwitchOAuthController _instance;

	// Token: 0x04002202 RID: 8706
	private const string TwitchAuthUrl = "https://id.twitch.tv/oauth2/authorize";

	// Token: 0x04002203 RID: 8707
	private const string ClientID = "bq0wyxhwa7xjlyomjjdv2o6wun6l2t";

	// Token: 0x04002204 RID: 8708
	private const string TwitchRedirectURL = "http://localhost:8085/";

	// Token: 0x04002205 RID: 8709
	private string _twitchAuthStateVerify;

	// Token: 0x04002206 RID: 8710
	private string _authToken;

	// Token: 0x04002207 RID: 8711
	private Queue<string> _tokenQueue = new Queue<string>();

	// Token: 0x04002208 RID: 8712
	public bool _hasAuth;

	// Token: 0x04002209 RID: 8713
	private bool _tryingValidation;
}
