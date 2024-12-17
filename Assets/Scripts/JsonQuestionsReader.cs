using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DefaultNamespace
{
    public class JsonQuestionsReader
    {
        private readonly string _jsonPath;
        private int _index;

        public JsonQuestionsReader(string jsonPath)
        {
            _jsonPath = jsonPath;
            _index = 0;
        }

        public List<Question> Read()
        {
            var result = new List<Question>();

            if (!File.Exists(_jsonPath))
            {
                throw new FileNotFoundException($"File not found: {_jsonPath}");
            }

            string jsonContent = File.ReadAllText(_jsonPath);

            try
            {
                var questionsData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);

                if (questionsData == null)
                {
                    throw new Exception("Invalid JSON format: unable to parse");
                }

                foreach (var questionData in questionsData)
                {
                    var question = ParseQuestion(questionData);
                    ValidateQuestion(question);
                    result.Add(question);
                }
            }
            catch (JsonException ex)
            {
                throw new Exception("Invalid JSON format", ex);
            }

            return result;
        }

        private Question ParseQuestion(Dictionary<string, object> questionData)
        {
            if (!questionData.ContainsKey("question") || !questionData.ContainsKey("Answer") || !questionData.ContainsKey("WrongAnswers"))
            {
                throw new Exception("Missing required fields in JSON object");
            }

            return new Question
            {
                QuestionText = questionData["question"] as string,
                Answer = questionData["Answer"] as string,
                WrongAnswers = (questionData["WrongAnswers"] as IEnumerable<object>)?.Select(o => o as string).ToList()
            };
        }

        private void ValidateQuestion(Question question)
        {
            if (string.IsNullOrEmpty(question.QuestionText))
            {
                throw new Exception("Question text is missing or empty");
            }

            if (string.IsNullOrEmpty(question.Answer))
            {
                throw new Exception("Answer is missing or empty");
            }

            if (question.WrongAnswers == null || question.WrongAnswers.Count != 3)
            {
                throw new Exception("WrongAnswers must be an array of exactly 3 strings");
            }
        }

        public class Question
        {
            public string QuestionText { get; set; }
            public string Answer { get; set; }
            public List<string> WrongAnswers { get; set; }
        }
    }
}
