using Matoya.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Matoya.Common {
    public sealed class CloudLogging : MonoBehaviour {
        [Serializable]
        private sealed class LogElement {
            [SerializeField]
            private string logType;

            [SerializeField]
            private string timestamp;

            [SerializeField]
            private string logString;

            [SerializeField]
            private string stackTrace;

            public LogElement(string timestamp, string logString, string stackTrace, LogType logType) {
                this.timestamp = timestamp;
                this.logString = logString;
                this.stackTrace = stackTrace;
                this.logType = logType.ToString();
            }
        }

        [Serializable]
        private sealed class JsonLog {
            [SerializeField]
            private string timestamp;

            [SerializeField]
            private LogElement[] logs;

            public JsonLog(string timestamp, List<LogElement> logElements) {
                this.timestamp = timestamp;
                logs = logElements.ToArray();
            }
        }

        private static CloudLogging instance;
        private static readonly Mutex mutex = new();
        private static readonly List<LogElement> logElements = new();

        [RuntimeInitializeOnLoadMethod]
        public static void EnableCloudLogging() {
            if(Application.platform != RuntimePlatform.Android) {
                Debug.Log("CloudLogging: Cloud logging only allowed on Android.");
                return;
            }
            if(instance) {
                Debug.Log("CloudLogging: Cloud logging is already enabled.");
                return;
            }
            GameObject cloudLoggingGameObject = new("Cloud Logging");
            cloudLoggingGameObject.AddComponent<CloudLogging>();
        }

        private void Awake() {
            if(instance) {
                Destroy(gameObject);
                return;
            }
            instance = this;
            //Application.logMessageReceivedThreaded += SaveLog;
        }

        public void SaveLog(string logString, string stackTrace, LogType logType) {
            string timestamp = GetTimestampString("[HH:mm:ss.fff]");
            mutex.WaitOne();
            logElements.Add(new LogElement(timestamp, logString, stackTrace, logType));
            mutex.ReleaseMutex();
        }

        private string GetTimestampString(string format) {
            return DateTime.Now.ToString(format);
        }

        public async void UploadLogs() {
            mutex.WaitOne();
            string timestamp = GetTimestampString("yyyy-MM-dd HH:mm:ss");
            JsonLog jsonLog = new(timestamp, logElements);
            logElements.Clear();
            mutex.ReleaseMutex();
            string filename = await CreateFilename(timestamp);
            instance.StartCoroutine(SendLogWebRequest(filename, jsonLog));
        }

        private async Task<string> CreateFilename(string timestamp) {
            return $"Maria-Zelenika-Masterthesis-{timestamp}.json";
        }

        //private async Task<string> GetLocalPlayerName() {
        //    try {
        //        MetaIntegrationManager.UserData loggedInUser = await MetaIntegrationManager.GrabLoggedInUserAsync();
        //        return loggedInUser.username;
        //    }
        //    catch {
        //        return "unknownPlayer";
        //    }
        //}

        private static IEnumerator SendLogWebRequest(string filename, JsonLog jsonLog) {
            string uploadUrl = $"https://log-storage-2.ew.r.appspot.com/logs/masterThesis/Maria/{filename}";
            byte[] fullContent = Encoding.UTF8.GetBytes(JsonUtility.ToJson(jsonLog));
            using UnityWebRequest request = UnityWebRequest.Put(uploadUrl, fullContent);
            Debug.Log("CloudLogging: Sending log to cloud.");
            float preSendTime = Time.unscaledTime;
            yield return request.SendWebRequest();
            float rtt = Time.unscaledTime - preSendTime;
            if(request.result == UnityWebRequest.Result.Success) {
                Debug.Log($"CloudLogging: Log uploaded successfully in {rtt:0.000} seconds.");
            }
            else {
                Debug.Log($"CloudLogging: Log upload failed with code {request.responseCode} after {rtt:0.000} seconds.");
            }
        }

        private void OnDestroy() {
            Application.logMessageReceivedThreaded -= SaveLog;
        }
    }
}
