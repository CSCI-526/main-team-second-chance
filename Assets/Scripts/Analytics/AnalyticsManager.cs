using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsManager : MonoBehaviour
{
    public struct BoolMetric
    {
        public bool value;
        public BoolMetric(bool val) { value = val; }
    }
    public struct StringMetric
    {
        public string value;
        public StringMetric(string val) { value = val; }
    }

    private struct MetricItem
    {
        public string metricId;
        public object metricData;

        public MetricItem(string id, object data)
        {
            metricId = id;
            metricData = data;
        }
    }

    private const string PROJECT_ID = "team-second-chance";
    private const string API_KEY = "AIzaSyAoXTy6p6rtSSHtduwgQ86zpAdlCNtq08w";
    private static readonly string BASE_URL = $"https://{PROJECT_ID}-default-rtdb.firebaseio.com";

    private const int GAME_VERSION = 2; // increment this as we release new versions of the game

    private const string DEVICE_ID_KEY = "FirebaseMetrics_DeviceId";

    private static AnalyticsManager _instance;

    public static AnalyticsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AnalyticsManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("FirebaseMetrics");
                    _instance = go.AddComponent<AnalyticsManager>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private static string GetDeviceIdentifier()
    {
        if (PlayerPrefs.HasKey(DEVICE_ID_KEY))
        {
            return PlayerPrefs.GetString(DEVICE_ID_KEY);
        }

        string deviceInfo = SystemInfo.deviceUniqueIdentifier;
        string randomGuid = Guid.NewGuid().ToString();
        string combinedString = deviceInfo + randomGuid;

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }
            string deviceId = builder.ToString();

            PlayerPrefs.SetString(DEVICE_ID_KEY, deviceId);
            PlayerPrefs.Save();

            return deviceId;
        }
    }

    public static void SendMetric(string metricId, object metricData)
    {
        Instance.StartCoroutine(_SendMetric(metricId, metricData));
    }

    private static IEnumerator _SendMetric(string metricId, object metricData)
    {
        return _SendMetricInternal(metricId, metricData, null);
    }

    public static void SendMetric(string metricId, object metricData, Action<bool, string> onComplete)
    {
        Instance.StartCoroutine(_SendMetric(metricId, metricData, onComplete));
    }

    private static IEnumerator _SendMetric(string metricId, object metricData, Action<bool, string> onComplete)
    {
        return _SendMetricInternal(metricId, metricData, onComplete);
    }

    private const string SESSION_ID_KEY = "FirebaseMetrics_SessionId";
    private static string _currentSessionId;

    private static string GetSessionId()
    {
        if (string.IsNullOrEmpty(_currentSessionId))
        {
            if (PlayerPrefs.HasKey(SESSION_ID_KEY))
            {
                _currentSessionId = PlayerPrefs.GetString(SESSION_ID_KEY);
            }
            else
            {
                _currentSessionId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString(SESSION_ID_KEY, _currentSessionId);
                PlayerPrefs.Save();
            }
        }
        return _currentSessionId;
    }

    public static void NewSession()
    {
        _currentSessionId = Guid.NewGuid().ToString();
        PlayerPrefs.SetString(SESSION_ID_KEY, _currentSessionId);
        PlayerPrefs.Save();
    }

    private static IEnumerator _SendMetricInternal(string metricId, object metricData, Action<bool, string> onComplete)
    {
        if (Application.isEditor)
        {
            yield break;
        }

        string deviceId = GetDeviceIdentifier();
        string sessionId = GetSessionId();
        string timestamp = DateTime.UtcNow.ToString("o");
        string entryId = Guid.NewGuid().ToString();

        string databasePath = $"{GAME_VERSION}/{deviceId}/{sessionId}/{metricId}/{entryId}.json";
        string fullUrl = $"{BASE_URL}/{databasePath}?auth={API_KEY}";
        string metricDataJson = JsonUtility.ToJson(metricData);

        string jsonPayload = $@"{{
            ""data"": {metricDataJson},
            ""timestamp"": ""{timestamp}""
        }}";

        Debug.Log($"Sending metric to {fullUrl}: {jsonPayload}");

        using (UnityWebRequest request = new UnityWebRequest(fullUrl, "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            bool success = !(request.result == UnityWebRequest.Result.ConnectionError ||
                           request.result == UnityWebRequest.Result.ProtocolError);
            string response = request.downloadHandler.text;

            Debug.Log($"Response: {response}");

            if (!success)
            {
                Debug.LogError($"Failed to send metric data: {request.error}");
                Debug.LogError($"Response: {response}");
            }

            onComplete?.Invoke(success, response);
        }
    }
}