﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonQuestionsReader
{
    [System.Serializable]
    public class Question
    {
        public string title;
        public string answer;
        public List<string> wrongAnswer;
    }

    [System.Serializable]
    public class QuestionWrapper
    {
        public List<Question> questions;
    }

    public IEnumerator ReadQuestions(System.Action<List<Question>> callback)
    {
        var path = Path.Combine(Application.streamingAssetsPath, "questions.json");

        // Use UnityWebRequest for WebGL
        if (path.Contains("://") || path.Contains(":///"))
        {
            using var request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonContent = request.downloadHandler.text;
                jsonContent = RemoveBom(jsonContent);
                jsonContent = jsonContent.Trim();
                var wrappedJson = "{ \"questions\": " + jsonContent + " }";
                try
                {
                    var questionWrapper = JsonUtility.FromJson<QuestionWrapper>(wrappedJson);
                    callback?.Invoke(questionWrapper.questions);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON parsing failed: {e.Message}");
                    callback?.Invoke(new List<Question>());
                }
            }
            else
            {
                Debug.LogError($"Error loading file (WebGL): {request.error}");
                callback?.Invoke(new List<Question>());
            }
        }
        else
        {
            // Local file
            if (File.Exists(path))
            {
                var jsonContent = File.ReadAllText(path);

                var wrappedJson = "{ \"questions\": " + jsonContent + " }";
                var questionWrapper = JsonUtility.FromJson<QuestionWrapper>(wrappedJson);

                callback?.Invoke(questionWrapper.questions);
            }
            else
            {
                Debug.LogError($"File not found at path: {path}");
                callback?.Invoke(new List<Question>());
            }
        }
    }
    
    private static string RemoveBom(string jsonContent)
    {
        // Rimuove il BOM UTF-8 (se presente)
        var BOM = System.Text.Encoding.UTF8.GetString(new byte[] { 0xEF, 0xBB, 0xBF });
        return jsonContent.StartsWith(BOM) ? jsonContent.Substring(BOM.Length) : jsonContent;
    }
}