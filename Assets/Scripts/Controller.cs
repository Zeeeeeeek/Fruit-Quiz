using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    public List<GameObject> fruits;
    public float fruitDistance = 1;
    public float playerDistance = 1;
    public GameObject winCanvas;

    private const float MaxY = 2.6f;
    private const float MinY = -4.6f;
    private const float MaxX = 1.6f;
    private const float MinX = -8.6f;
    private Vector3 _playerPosition;

    private List<JsonQuestionsReader.Question> _questions;
    private int _questionIndex;

    private void Start()
    {
        var jsonReader = new JsonQuestionsReader();

        StartCoroutine(jsonReader.ReadQuestions(questions =>
        {
            _questions = questions;

            if (_questions.Count > 0)
            {
                FindFirstObjectByType<HealthPointsDisplay>().SetHealthPoints(_questions.Count);
                _playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
                Spawn();
            }
            else
            {
                Debug.LogError("No questions were loaded. Check the JSON file.");
            }
        }));
    } 

    private bool IsPositionFarEnoughFromFruit(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Distance(pos1, pos2) > fruitDistance;
    }

    private bool IsFarEnoughFromPlayer(Vector3 newPosition)
    {
        return Vector3.Distance(_playerPosition, newPosition) > playerDistance;
    }

    private Vector3 GetValidPositionFromAllPositions(List<Vector3> positions)
    {
        while (true)
        {
            var newPos = new Vector3(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY), 0);

            if (!IsFarEnoughFromPlayer(newPos)) continue;

            if (positions.All(p => IsPositionFarEnoughFromFruit(p, newPos)))
            {
                return newPos;
            }
        }
    }

    private void Spawn()
    {
        if (fruits == null) return;
        var positions = new List<Vector3>();
        var i = 0;
        var currentQuestion = _questions[_questionIndex];

        var questionTitle = GameObject.FindGameObjectWithTag("Question Title").GetComponent<TMPro.TextMeshProUGUI>();
        questionTitle.text = currentQuestion.title;


        var shuffledFruits = fruits.OrderBy(_ => Random.value).ToList();
        var shuffledWrongAnswersQueue = new Queue<string>(currentQuestion.wrongAnswer.OrderBy(_ => Random.value));

        shuffledFruits.ForEach(fruit =>
        {
            var spawnPosition = GetValidPositionFromAllPositions(positions);
            positions.Add(spawnPosition);
            var spawned = Instantiate(fruit, spawnPosition, Quaternion.identity).GetComponent<Collectible>();
            spawned.onCollect.AddListener(OnFruitCollected);
            var fruitName = fruit.name.First().ToString().ToUpper() + fruit.name[1..];
            var fruitText = GameObject.FindGameObjectWithTag(fruitName + " answer")
                .GetComponent<TMPro.TextMeshProUGUI>();
            if (i == 0)
            {
                Debug.Log(fruitName + " is the correct answer");
                fruitText.text = currentQuestion.answer;
            }
            else
            {
                fruitText.text = shuffledWrongAnswersQueue.Dequeue();
            }

            spawned.index = i++;
        });
    }

    private void OnFruitCollected(int fruitIndex)
    {
        if (fruitIndex == 0)
        {
            Debug.Log("Correct answer collected");
            _questionIndex++;
            if (_questionIndex >= _questions.Count)
            {
                HandleGameOver();
                return;
            }
        }
        else
        {
            Debug.Log("Wrong answer collected");
            var hpDisplay = FindFirstObjectByType<HealthPointsDisplay>();
            hpDisplay.DecreaseHealthPoints();
            if (hpDisplay.healthPoints <= 0)
            {
                HandleGameOver();
                return;
            }
        }
        KillAndRespawnFruits();
    }

    private void HandleGameOver()
    {
        KillAllFruits();
        var playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerMovement.ResetAnimator();
        playerMovement.enabled = false;
        
        var questionTitle = GameObject.FindGameObjectWithTag("Question Title").GetComponent<TMPro.TextMeshProUGUI>();
        questionTitle.text = "";

        foreach (var fruit in fruits)
        {
            var fruitName = fruit.name.First().ToString().ToUpper() + fruit.name[1..];
            var fruitText = GameObject.FindGameObjectWithTag(fruitName + " answer")
                .GetComponent<TMPro.TextMeshProUGUI>();
            fruitText.text = "";
        }
        
        var points = FindFirstObjectByType<HealthPointsDisplay>().healthPoints*100/_questions.Count;
        winCanvas.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
            points >= 60 ? "Hai vinto!" : "Hai perso!";
        winCanvas.SetActive(true);
        if (points < 60) return;
        var confetti = GameObject.FindGameObjectsWithTag("Confetti");
        foreach (var c in confetti)
        {
            c.GetComponent<VFXTrigger>().TriggerVFX();
        }
    }

    public void OnEndButtonClick()
    {
        var points = FindFirstObjectByType<HealthPointsDisplay>().healthPoints*100/_questions.Count;
        #if UNITY_EDITOR
        Debug.Log("Points: " + points);
        #endif
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) KillAndRespawnFruits();
        if (Input.GetKeyDown(KeyCode.Alpha0)) HandleGameOver();
    }
#endif

    private void KillAndRespawnFruits()
    {
        KillAllFruits();
        Spawn();
    }

    private static void KillAllFruits()
    {
        var allFruits = GameObject.FindGameObjectsWithTag("Fruit");
        foreach (var fruit in allFruits)
        {
            Destroy(fruit);
        }
    }
}