using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public BoardManager boardManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol tıklama
        {
            // Mouse'un world pozisyonunu al
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // x ve y koordinatlarını yuvarla
            int x = Mathf.FloorToInt(worldPosition.x);
            int y = Mathf.FloorToInt(worldPosition.y);

            // BoardManager'dan Tile'ı al ve içindeki Chip'i yok et
            boardManager.DestroyChipAt(x, y);
        }
    }
}