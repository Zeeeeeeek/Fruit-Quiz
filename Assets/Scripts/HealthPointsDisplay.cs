using System;
using UnityEngine;

public class HealthPointsDisplay : MonoBehaviour
{
    public int healthPoints = 3;
    public TMPro.TextMeshProUGUI healthPointsText;
    
    public void SetHealthPoints(int points)
    {
        healthPoints = points;
        UpdateHealthPointsText();
    }
    
    public void DecreaseHealthPoints()
    {
        if (healthPoints <= 0) return;
        healthPoints--;
        UpdateHealthPointsText();
    }
    
    private void UpdateHealthPointsText()
    {
        if (healthPointsText)
        {
            healthPointsText.text = healthPoints.ToString();
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            DecreaseHealthPoints();
        }
    }
}
