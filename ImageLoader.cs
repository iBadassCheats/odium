using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class ImageLoader
{
	private static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

	public static IEnumerator LoadSpriteFromURL(string url, Action<Sprite> onComplete, Action<string> onError = null)
	{
		if (spriteCache.ContainsKey(url))
		{
			onComplete?.Invoke(spriteCache[url]);
			yield break;
		}
		byte[] imageData = null;
		string errorMessage = null;
		bool completed = false;
		Task.Run(delegate
		{
			try
			{
				using (WebClient webClient = new WebClient())
				{
					imageData = webClient.DownloadData(url);
				}
				completed = true;
			}
			catch (Exception ex2)
			{
				errorMessage = ex2.Message;
				completed = true;
			}
		});
		while (!completed)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (errorMessage != null)
		{
			onError?.Invoke("Failed to download: " + errorMessage);
			yield break;
		}
		if (imageData != null)
		{
			try
			{
				Texture2D texture = new Texture2D(2, 2);
				if (ImageConversion.LoadImage(texture, imageData))
				{
					Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
					spriteCache[url] = sprite;
					onComplete?.Invoke(sprite);
				}
				else
				{
					onError?.Invoke("Failed to load image data into texture");
				}
				yield break;
			}
			catch (Exception ex)
			{
				onError?.Invoke("Error creating sprite: " + ex.Message);
				yield break;
			}
		}
		onError?.Invoke("No image data received");
	}

	public static IEnumerator LoadSpriteFromURLWebRequest(string url, Action<Sprite> onComplete, Action<string> onError = null)
	{
		if (spriteCache.ContainsKey(url))
		{
			onComplete?.Invoke(spriteCache[url]);
			yield break;
		}
		UnityWebRequest request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			try
			{
				byte[] imageData = request.downloadHandler.data;
				Texture2D texture = new Texture2D(2, 2);
				if (ImageConversion.LoadImage(texture, imageData))
				{
					Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
					spriteCache[url] = sprite;
					onComplete?.Invoke(sprite);
				}
				else
				{
					onError?.Invoke("Failed to load image data into texture");
				}
				yield break;
			}
			catch (Exception ex)
			{
				onError?.Invoke("Error creating sprite: " + ex.Message);
				yield break;
			}
		}
		onError?.Invoke("Network error: " + request.error);
	}

	public static async Task<Sprite> LoadSpriteFromURLAsync(string url)
	{
		if (spriteCache.ContainsKey(url))
		{
			return spriteCache[url];
		}
		try
		{
			using WebClient webClient = new WebClient();
			byte[] imageData = await webClient.DownloadDataTaskAsync(url);
			Texture2D texture = new Texture2D(2, 2);
			if (ImageConversion.LoadImage(texture, imageData))
			{
				Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
				spriteCache[url] = sprite;
				return sprite;
			}
			throw new Exception("Failed to load image data into texture");
		}
		catch (Exception ex)
		{
			throw new Exception("Failed to download image: " + ex.Message);
		}
	}

	public static Sprite LoadSpriteFromURLSync(string url)
	{
		if (spriteCache.ContainsKey(url))
		{
			return spriteCache[url];
		}
		try
		{
			using WebClient webClient = new WebClient();
			byte[] array = webClient.DownloadData(url);
			Texture2D texture2D = new Texture2D(2, 2);
			if (ImageConversion.LoadImage(texture2D, array))
			{
				Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
				spriteCache[url] = sprite;
				return sprite;
			}
			throw new Exception("Failed to load image data into texture");
		}
		catch (Exception ex)
		{
			throw new Exception("Failed to download image: " + ex.Message);
		}
	}

	public static IEnumerator LoadSpriteFromURLManual(string url, Action<Sprite> onComplete, Action<string> onError = null)
	{
		if (spriteCache.ContainsKey(url))
		{
			onComplete?.Invoke(spriteCache[url]);
			yield break;
		}
		byte[] imageData = null;
		string errorMessage = null;
		bool completed = false;
		HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
		if (request != null)
		{
			request.Method = "GET";
			request.UserAgent = "Unity";
			request.BeginGetResponse(delegate(IAsyncResult ar)
			{
				try
				{
					WebResponse webResponse = request.EndGetResponse(ar);
					using (Stream stream = webResponse.GetResponseStream())
					{
						MemoryStream memoryStream = new MemoryStream();
						byte[] array = new byte[4096];
						int count;
						while ((count = stream.Read(array, 0, array.Length)) > 0)
						{
							memoryStream.Write(array, 0, count);
						}
						imageData = memoryStream.ToArray();
					}
					completed = true;
				}
				catch (Exception ex2)
				{
					errorMessage = ex2.Message;
					completed = true;
				}
			}, null);
			float timeout = 30f;
			float elapsed = 0f;
			while (!completed && elapsed < timeout)
			{
				elapsed += 0.1f;
				yield return new WaitForSeconds(0.1f);
			}
			if (!completed)
			{
				onError?.Invoke("Request timed out");
				yield break;
			}
			if (errorMessage != null)
			{
				onError?.Invoke("Failed to download: " + errorMessage);
				yield break;
			}
			if (imageData != null && imageData.Length != 0)
			{
				try
				{
					Texture2D texture = new Texture2D(2, 2);
					if (ImageConversion.LoadImage(texture, imageData))
					{
						Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
						spriteCache[url] = sprite;
						onComplete?.Invoke(sprite);
					}
					else
					{
						onError?.Invoke("Failed to load image data into texture");
					}
					yield break;
				}
				catch (Exception ex)
				{
					onError?.Invoke("Error creating sprite: " + ex.Message);
					yield break;
				}
			}
			onError?.Invoke("No image data received");
		}
		else
		{
			onError?.Invoke("Failed to create HTTP request");
		}
	}

	public static void ClearCache()
	{
		foreach (Sprite value in spriteCache.Values)
		{
			if (value != null && value.texture != null)
			{
				UnityEngine.Object.Destroy(value.texture);
				UnityEngine.Object.Destroy(value);
			}
		}
		spriteCache.Clear();
	}

	public static void RemoveFromCache(string url)
	{
		if (spriteCache.ContainsKey(url))
		{
			Sprite sprite = spriteCache[url];
			if (sprite != null && sprite.texture != null)
			{
				UnityEngine.Object.Destroy(sprite.texture);
				UnityEngine.Object.Destroy(sprite);
			}
			spriteCache.Remove(url);
		}
	}
}
