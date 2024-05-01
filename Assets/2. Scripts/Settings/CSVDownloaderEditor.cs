#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public class CSVDownloaderEditor : EditorWindow
{
    private static readonly Dictionary<string, string> sheetIds = new Dictionary<string, string>
    {
        {"EnemyInfo", "1189647791"},
        {"FriendInfo", "235049411"},
        {"StageEnemy", "550356149"},
        {"StageInfo", "101577671"}
    };

    [MenuItem("Tools/Download CSVs")]
    public static void DownloadCSVs()
    {
        foreach (var sheet in sheetIds)
        {
            string url = $"https://docs.google.com/spreadsheets/d/1-6LNhGDkOQ_kVCKN_Gsk1cvarjl-fDKHTyxT9GhtcPk/export?format=csv&id=1-6LNhGDkOQ_kVCKN_Gsk1cvarjl-fDKHTyxT9GhtcPk&gid={sheet.Value}";
            string filePath = Path.Combine(Application.dataPath, "Resources/CSV", $"{sheet.Key}.csv");

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                var asyncOperation = webRequest.SendWebRequest();

                // 동기적 대기
                while (!asyncOperation.isDone)
                {
                    System.Threading.Thread.Sleep(100);
                }

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllText(filePath, webRequest.downloadHandler.text);
                    Debug.Log($"Successfully downloaded and saved: {sheet.Key}.csv");
                }
                else
                {
                    Debug.LogError($"Failed to download {sheet.Key}.csv: {webRequest.error}");
                }
            }
        }

        AssetDatabase.Refresh();
    }
}
#endif