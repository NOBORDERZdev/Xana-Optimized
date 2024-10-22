using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public class YoutubeDownloader : MonoBehaviour
{
    public string Data;
    private async void Start()
    {
        string playerScriptUrl = await GetPlayerInfoByVideoId("EG4gFxczkH8");
        string PlayerScriptCode = await DownloadRawPlayerScript(playerScriptUrl);
       var decoded = ExtractDecipherOpcode(PlayerScriptCode);
        foreach (var code in decoded)
        {
            Debug.LogError("Youtube  ..  " + code);
        }
        Debug.LogError("Youtube  ..  Done" );
    }
    public  async Task<string> GetPlayerInfoByVideoId(string videoID)
    {
        string data = await LoadURL("https://www.youtube.com/watch?v=" + videoID);
        Debug.LogError("https://www.youtube.com/watch?v=" + videoID);
        Debug.LogError(data);
        var splitData = data.Split(new[] { "/s/player" }, StringSplitOptions.None);

        if (splitData.Length < 2)
        {
            throw new Exception($"Failed to retrieve player script for video 1 id: {videoID}");
        }

        data = splitData[1].Split('"')[0];
        string playerURL = "https://www.youtube.com/s/player" + data;

        try
        {
            string[] playerID = data.Split('/');
            playerID = playerID[playerID.Length - 1].Split('-');
            return  playerURL;
        }
        catch (Exception)
        {
            throw new Exception($"Failed to retrieve player script for video 2 id: {videoID}");
        }
    }

    public static async Task<string> DownloadRawPlayerScript(string playerURL)
    {
        return await LoadURL(playerURL);
    }

    public static Dictionary<string, string> ExtractDecipherOpcode(string decipherScript)
    {
        if (string.IsNullOrEmpty(decipherScript))
        {
            Debug.LogError("No decipher script was provided. Abort.");
            return null;
        }

        var decipherPatterns = decipherScript.Split(new[] { ".split(\"\")" }, StringSplitOptions.None);
        if (decipherPatterns.Length < 2)
        {
            Debug.LogError("Failed to extract decipher patterns.");
            return null;
        }

        string pattern = decipherPatterns[1].Split(new[] { ".join(\"\")" }, StringSplitOptions.None)[0];
        var operations = new List<string>(pattern.Split(';'));
        operations.RemoveAt(0);
        operations.RemoveAt(operations.Count - 1);

        string joinedPattern = string.Join(";", operations);

        var regex = new Regex("(?<=;).*?(?=\\[|\\.)");
        var matches = regex.Matches(joinedPattern);

        if (matches.Count < 2)
        {
            Debug.LogError("Failed to get deciphers function");
            return null;
        }

        string decipherObjectVar = matches[0].Value;
        string[] decipherParts = decipherScript.Split(new[] { $"{decipherObjectVar}={{" }, StringSplitOptions.None);
        string decipher = decipherParts[1].Split(new[] { "}};" }, StringSplitOptions.None)[0];

        string[] decipherFunctions = decipher.Split(new[] { "}," }, StringSplitOptions.None);

        Dictionary<string, string> deciphers = new Dictionary<string, string>();

        foreach (string function in decipherFunctions)
        {
            string[] parts = function.Split(new[] { ":function" }, StringSplitOptions.None);
            string key = parts[0];
            string value = parts[1].Split(new[] { "){" }, StringSplitOptions.None)[1];
            deciphers[key] = value;
        }

        return deciphers;
    }

    public static string ExecuteSignaturePattern(List<string> patterns, Dictionary<string, string> deciphers, string signature)
    {
        var processSignature = new List<char>(signature.ToCharArray());

        for (int i = 0; i < patterns.Count; i++)
        {
            string[] executes = patterns[i].Split(new[] { "->" }, StringSplitOptions.None);
            int number = int.Parse(executes[1].Replace("(", "").Replace(")", ""));

            if (!deciphers.TryGetValue(executes[0], out string execute))
            {
                Debug.LogError("Decipher dictionary was not found.");
                return null;
            }

            switch (execute)
            {
                case "a.reverse()":
                    processSignature.Reverse();
                    break;

                case "var c=a[0];a[0]=a[b%a.length];a[b]=c":
                    char c = processSignature[0];
                    processSignature[0] = processSignature[number % processSignature.Count];
                    processSignature[number] = c;
                    break;

                case "a.splice(0,b)":
                    processSignature.RemoveRange(0, number);
                    break;

                default:
                    Debug.LogError("Unknown command found in the decipher pattern.");
                    break;
            }
        }

        return new string(processSignature.ToArray());
    }

    private static async Task<string> LoadURL(string url)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
             // client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");

                // Set a reasonable timeout
                client.Timeout = TimeSpan.FromSeconds(10);

                // Send request and read response asynchronously
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throw if not successful

                // Read the response content as a string
                return await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading URL: {ex.Message}");
            return string.Empty; // Return an empty string if an error occurs
        }
    }
}
