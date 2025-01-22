using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int Width = 8;
    public int Height = 8;
    public GameObject tilePrefab;
    public Transform boardParent;

    private Tile[,] board;

    private void Start()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        board = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                GameObject tileObject = Instantiate(tilePrefab, boardParent);
                tileObject.transform.position = new Vector2(x, y);

                Tile tile = tileObject.GetComponent<Tile>();
                tile.Initialize(new Vector2Int(x, y), Random.Range(0, 4)); // Rastgele renk ata
                board[x, y] = tile;
            }
        }

        // Komşuları hesapla
        CalculateNeighbors();//cache
    }

    private void CalculateNeighbors()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,    // Yukarı
            Vector2Int.down,  // Aşağı
            Vector2Int.left,  // Sol
            Vector2Int.right  // Sağ
        };

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile tile = board[x, y];

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighborPos = tile.Position + dir;
                    if (IsInsideBoard(neighborPos))
                    {
                        Tile neighbor = GetTileAtPosition(neighborPos);
                        tile.AddNeighbor(neighbor);
                    }
                }
            }
        }
    }

    public bool IsInsideBoard(Vector2Int position)
    {
        return position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height;
    }

    public Tile GetTileAtPosition(Vector2Int position)
    {
        return board[position.x, position.y];
    }

    public bool AreNeighbors(Tile a, Tile b)
    {
        return a.Neighbors.Contains(b); // Komşular listesine göre kontrol
    }

    public void DestroyTiles(List<Tile> tilesToDestroy)
    {
        foreach (Tile tile in tilesToDestroy)
        {
            tile.SetColorID(-1); // Tile'ı boşalt
        }
        DropTiles();
        //FillEmptyTiles(); // Boşlukları doldur
    }
    public Vector2Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x);
        int y = Mathf.FloorToInt(worldPosition.y);
        return new Vector2Int(x, y);
    }

    private void DropTiles()
    {
        // Her sütun için kontrol yap
        for (int x = 0; x < Width; x++)
        {
            // En alttan üste doğru ilerleyin
            for (int y = 0; y < Height; y++)
            {
                // Şu anda kontrol edilen tile
                Tile currentTile = board[x, y];

                // Eğer currentTile boşsa (ColorID == -1)
                if (currentTile.ColorID == -1)
                {
                    // Üzerindeki tile'ları kontrol et
                    for (int k = y + 1; k < Height; k++)
                    {
                        Tile aboveTile = board[x, k];

                        if (aboveTile.ColorID != -1) // Üstte dolu bir tile bulduk
                        {
                            // Çipleri hareket ettir (animasyonlu olarak)
                            StartCoroutine(MoveChipDown(aboveTile, currentTile));

                            // Aşağıdaki tile'ın ColorID'sini güncelle
                            currentTile.SetColorIDz(aboveTile.ColorID);

                            // Üstteki tile'ın ColorID'sini boşalt
                            aboveTile.SetColorIDz(-1);

                            break; // Bir kez bir tile'ı kaydırdıktan sonra döngüden çık
                        }
                    }
                }
            }
        }
    }
    private IEnumerator MoveChipDown(Tile fromTile, Tile toTile)
    {
        // Eğer üstte bir çip yoksa işlem yapma
        if (fromTile.CurrentChip == null)
        {
            yield break;
        }

        // Hareket eden çipi al
        Chip movingChip = fromTile.CurrentChip;

        // Başlangıç ve hedef pozisyonları belirle
        Vector3 startPosition = movingChip.transform.position;
        Vector3 endPosition = toTile.transform.position;

        float elapsedTime = 0f;
        float duration = 0.5f; // Hareket süresi

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Çipin pozisyonunu Lerp ile hareket ettir
            movingChip.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Hareket tamamlandığında çipi yeni pozisyona yerleştir
        movingChip.transform.position = endPosition;

        // Yeni tile ile çip ilişkilendirmesi yapılabilir
        toTile.setChip(movingChip);
        fromTile.setChip(null) ;
    }

    private void FillEmptyTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile tile = board[x, y];
                if (tile.ColorID == -1)
                {
                    tile.SetColorID(Random.Range(0, 4)); // Rastgele yeni renk ata
                }
            }
        }
    }
}
