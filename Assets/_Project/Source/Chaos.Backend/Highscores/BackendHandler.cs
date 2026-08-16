using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Chaos.Backend.Highscores
{
    public static class BackendHandler
    {
        private static string apiUrl = "http://localhost:3000/highscores";
        
        public static IEnumerator GetScoresRoutine()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = request.downloadHandler.text;
                    
                    Debug.Log($"<color=lime>Highscores received: {jsonResponse}</color>");
                }
                else
                {
                    Debug.LogError($"Get error: {request.error}");
                }
            } 
        }
        
        public static IEnumerator PostScore(int score)
        {
            ScoreData scoreData = new ScoreData { playerName = "Benner Niceman", score = score };
            
            string json = JsonUtility.ToJson(scoreData);

            using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("<color=cyan>The new highscore has been sent!</color>");
                }
                else
                {
                    string errorJson = request.downloadHandler.text;
                    
                    Debug.LogError($"Send error: {errorJson}");
                }
            }
        }
    }
}
