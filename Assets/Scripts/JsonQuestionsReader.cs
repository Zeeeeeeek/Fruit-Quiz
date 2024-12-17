using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public List<Question> ReadQuestions()
    {
        var path = Path.Combine(Application.streamingAssetsPath, "questions.json");

        if (File.Exists(path))
        {
            var jsonContent = File.ReadAllText(path);

            var wrappedJson = "{ \"questions\": " + jsonContent + " }";

            var questionWrapper = JsonUtility.FromJson<QuestionWrapper>(wrappedJson);

            return questionWrapper.questions;
        }

        Debug.LogError($"File not found at path: {path}");
        return new List<Question>();
    }
}