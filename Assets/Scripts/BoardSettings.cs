using UnityEngine;

[CreateAssetMenu(fileName = "BoardSettings", menuName = "ScriptableObjects/BoardSettings", order = 1)]
public class BoardSettings : ScriptableObject
{
    public int Width = 8;  // Varsayılan genişlik
    public int Height = 8; // Varsayılan yükseklik
    public int ColorCount = 4;
    public int currentScore = 0;
    public int scoreGoal = 100;
    public int remainingMoves = 20;

}