using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

namespace SyncAssetBundle
{

    public class AssetBundleManager : MonoBehaviour
    {
        private string localVersionPath = Path.Combine(Application.streamingAssetsPath, "Bundles/version.json");
        private string remoteVersionUrl = "https://raw.githubusercontent.com/IceWaterNotIce/VTC_MET4337_Lab05_LocalAndRemoteAssetBundle/main/Assets/StreamingAssets/Bundles/version.json";
        private string downloadPath = Path.Combine(Application.streamingAssetsPath, "Bundles");

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return StartCoroutine(CheckAndUpdateBundles());
            // Load the lobby scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }


        IEnumerator CheckAndUpdateBundles()
        {

            // Load local version
            VersionConfig localConfig = new VersionConfig();

            // Check the local version config file exists
            if (File.Exists(localVersionPath))
            {
                string localJson = File.ReadAllText(localVersionPath);
                localConfig = JsonUtility.FromJson<VersionConfig>(localJson);
            }
            else
            {
                Debug.Log("local version.json not exist.");
            }

            // Fetch remote version
            UnityWebRequest.ClearCookieCache();
            UnityWebRequest request = UnityWebRequest.Get(remoteVersionUrl);
            request.useHttpContinue = false;

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch remote version: " + request.error);
                yield break;
            }

            Debug.Log(request.downloadHandler.text);

            var remoteConfig = JsonUtility.FromJson<VersionConfig>(request.downloadHandler.text);

            // Compare versions and download necessary bundles
            foreach (var remoteBundle in remoteConfig.bundles)
            {
                var localBundle = localConfig == null ? localConfig.bundles.Find(b => b.name == remoteBundle.name) : null;
                if (localBundle == null || float.Parse(remoteBundle.version) > float.Parse(localBundle.version))
                {
                    yield return StartCoroutine(DownloadAssetBundle(remoteBundle.url, remoteBundle.name));
                }
            }

            // Update local version file
            File.WriteAllText(localVersionPath, request.downloadHandler.text);
        }

        IEnumerator DownloadAssetBundle(string url, string bundleName)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            string bundlePath = Path.Combine(downloadPath, bundleName);
            request.downloadHandler = new DownloadHandlerFile(bundlePath);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download {bundleName}: " + request.error);
            }
            else
            {
                Debug.Log($"{bundleName} downloaded successfully.");
            }
        }

    }
    [System.Serializable]
    public class VersionConfig
    {
        [System.Serializable]
        public class AssetBundleInfo
        {
            public string name;
            public string version;
            public string url;
        }
        public List<AssetBundleInfo> bundles;
    }

}