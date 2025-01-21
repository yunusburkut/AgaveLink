using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentScore = 0;
    public int scoreGoal = 100;
    public int remainingMoves = 20;

    public Sprite[] chipSprites;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetChipSprite(int colorID)
    {
        if (colorID >= 0 && colorID < chipSprites.Length)
        {
            return chipSprites[colorID];
        }

        return null;
    }

    public void AddScore(int points)
    {
        currentScore += points;
        Debug.Log("Score: " + currentScore);

        if (currentScore >= scoreGoal)
        {
            Debug.Log("You Win!");
        }
    }

    public void DecrementMoves()
    {
        remainingMoves--;
        Debug.Log("Moves Left: " + remainingMoves);

        if (remainingMoves <= 0)
        {
            if (currentScore >= scoreGoal)
            {
                Debug.Log("You Win!");
            }
            else
            {
                Debug.Log("Game Over!");
            }
        }
    }
}