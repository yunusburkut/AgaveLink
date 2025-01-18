using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int rows = 8;
    public int columns = 8;
    public GameObject tilePrefab;
    public GameObject chipPrefab;

    private Tile[,] tiles;

    void Start()
    {
        InitializeBoard();
    }

    void InitializeBoard()
    {
        tiles = new Tile[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject tileObject = Instantiate(tilePrefab, new Vector3(col, row, 0), Quaternion.identity);
                Tile tile = tileObject.GetComponent<Tile>();
                tiles[row, col] = tile;

                // Rastgele bir Chip oluştur ve Tile'a yerleştir
                GameObject chipObject = Instantiate(chipPrefab);
                
                tile.SetChip(chipObject,Random.Range(0, 4));
            }
        }
    }

    public void DestroyChipAt(int x, int y)
    {
        // Koordinatları kontrol et (tahta sınırları içinde mi?)
        if (x >= 0 && x < tiles.GetLength(1) && y >= 0 && y < tiles.GetLength(0))
        {
            Tile tile = tiles[y, x]; // Tile'ı matriste bul

            if (tile != null && !tile.IsEmpty()) // Tile boş değilse
            {
                tile.DestroyChip(); // Chip'i yok et
                Debug.Log($"Tile ({x}, {y}) içindeki Chip yok edildi.");
            }
            else
            {
                Debug.LogWarning($"Tile ({x}, {y}) boş veya geçerli değil.");
            }
        }
        else
        {
            Debug.LogWarning($"Koordinatlar ({x}, {y}) tahtanın sınırları dışında!");
        }
    }
}

