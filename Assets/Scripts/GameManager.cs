using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentScore = 0;
    public int scoreGoal = 100;
    public int remainingMoves = 20;

    public Sprite[] chipSprites;

    // UI için referanslar
    public Text scoreText;   // Unity Editor'den atanacak
    public Text movesText;   // Unity Editor'den atanacak

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

        // İlk UI güncellemesi
        UpdateUI();
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

        // UI güncelle
        UpdateUI();

        DecrementMoves();

        if (currentScore >= scoreGoal)
        {
            Debug.Log("You Win!");
        }
    }

    public void DecrementMoves()
    {
        remainingMoves--;
        Debug.Log("Moves Left: " + remainingMoves);

        // UI güncelle
        UpdateUI();

        if (remainingMoves <= 0)
        {
            if (currentScore >= scoreGoal)
            {
                SceneManager.LoadScene("WinEndGameScene");
                FindObjectOfType<BoardManager>().ResetGame();
            }
            else
            {
                SceneManager.LoadScene("LoseEndGameScene");
                FindObjectOfType<BoardManager>().ResetGame();
            }
        }
    }

    private void UpdateUI()
    {
        // UI Text bileşenlerini güncelle
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore} / {scoreGoal}";
        }

        if (movesText != null)
        {
            movesText.text = $"Moves Left: {remainingMoves}";
        }
    }
}
