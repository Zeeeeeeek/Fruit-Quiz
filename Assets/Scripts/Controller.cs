using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    public List<GameObject> fruits;
    public float fruitDistance = 1;
    public float playerDistance = 1;

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
        _questions = jsonReader.ReadQuestions();
        _playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Spawn();
    }

    private bool IsPositionValid(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Distance(pos1, pos2) > fruitDistance && Vector3.Distance(_playerPosition, pos2) > playerDistance;
    }

    private Vector3 GetValidPositionFromAllPositions(List<Vector3> positions)
    {
        while (true)
        {
            var newPos = new Vector3(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY), 0);
            if (positions.All(p => IsPositionValid(p, newPos)))
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
        
        Debug.Log(currentQuestion.title);
        Debug.Log("--------------");
        
        var shuffledFruits = fruits.OrderBy(_ => Random.value).ToList();
        
        shuffledFruits.ForEach(fruit =>
        {
            var spawnPosition = GetValidPositionFromAllPositions(positions);
            positions.Add(spawnPosition);
            var spawned = Instantiate(fruit, spawnPosition, Quaternion.identity).GetComponent<Collectible>();
            spawned.onCollect.AddListener(OnFruitCollected);
            spawned.index = i;
            if (i == 0)
            {
                Debug.Log(fruit.name + " is the correct answer");
            }
            i++;
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
        }

        KillAndRespawnFruits();
    }
    
    private void HandleGameOver()
    {
        Debug.Log("You won the game!");
        KillAllFruits();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) KillAndRespawnFruits();
    }

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