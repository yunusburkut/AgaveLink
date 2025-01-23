using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int Width;
    public int Height;
    public int PoolHeight;
    public GameObject tilePrefab;
    public Transform boardParent;

    private Tile[,] board;

    private void Start()
    {
        InitializeBoard();
        LogLinkedChipGroups();
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
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile tile = board[x, y];

                // Komşuların yönlerini kontrol etmek için
                if (y < 7) // 8. satır (y == 7) değilse yukarı komşuya bakılabilir
                {
                    Vector2Int neighborPos = tile.Position + Vector2Int.up;
                    if (IsInsideBoard(neighborPos))
                    {
                        Tile neighbor = GetTileAtPosition(neighborPos);
                        tile.AddNeighbor(neighbor);
                    }
                }

                // Diğer yönler (Aşağı, Sol, Sağ)
                Vector2Int[] otherDirections = new Vector2Int[] 
                {
                    Vector2Int.down, // Aşağı
                    Vector2Int.left, // Sol
                    Vector2Int.right // Sağ
                };

                foreach (Vector2Int dir in otherDirections)
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
    
    public void DestroyTiles(List<Tile> tilesToDestroy)
    {
        FillEmptyTiles();
        foreach (Tile tile in tilesToDestroy)
        {
            tile.SetColorID(-1); // Tile'ı boşalt
        }
        DropTiles();
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
                    for (int k = y ; k < Height; k++)
                    {
                        Tile aboveTile = board[x, k];

                        if (aboveTile.ColorID != -1) // Üstte dolu bir tile bulduk
                        {
                            // Çipleri hareket ettir (animasyonlu olarak)
                            StartCoroutine(MoveChipDown(aboveTile, currentTile));

                            // Aşağıdaki tile'ın ColorID'sini güncelle
                            currentTile.SetColorIDLoc(aboveTile.ColorID);

                            // Üstteki tile'ın ColorID'sini boşalt
                            aboveTile.SetColorIDLoc(-1);

                            break; // Bir kez bir tile'ı kaydırdıktan sonra döngüden çık
                        }
                    }
                }
            }
        }
        LogLinkedChipGroups();
    }
    private IEnumerator MoveChipDown(Tile fromTile, Tile toTile)//tween ekle
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
    public void LogLinkedChipGroups()
    {
        HashSet<Tile> visitedTiles = new HashSet<Tile>(); // Ziyaret edilen tile'ları takip eder ve time complexitysi O(1) oldugu için hash set kullanmayı tercih ettim 
        List<List<Tile>> linkedGroups = new List<List<Tile>>(); // Bulunan bağlantılı gruplar
    
        // Tüm board'u gez
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Tile startTile = board[x, y];
    
                // Eğer bu tile zaten ziyaret edildiyse atla
                if (visitedTiles.Contains(startTile))
                    continue;
    
                // Bu tile'dan başlayarak bağlantılı grubu bul
                List<Tile> linkedGroup = GetLinkedGroup(startTile, visitedTiles);
    
                // Eğer grup en az 3 tile içeriyorsa geçerli olarak kabul et
                if (linkedGroup.Count >= 3)
                {
                    linkedGroups.Add(linkedGroup);
                }
            }
        }
    
        // Toplam geçerli grup sayısını logla
        Debug.Log($"Board'da toplam {linkedGroups.Count} geçerli linklenebilir grup bulundu.");
    }

// Belirtilen tile'dan başlayarak bağlantılı grubu bulur
    private List<Tile> GetLinkedGroup(Tile startTile, HashSet<Tile> visitedTiles)
    {
        List<Tile> linkedGroup = new List<Tile>();
                Queue<Tile> tilesToCheck = new Queue<Tile>();
        
         // Eğer başlangıç tile'ının rengi geçerli değilse (boşsa), grubu boş döndür
         if (startTile.ColorID == -1)
             return linkedGroup;
        
         // Başlangıç tile'ını işleme ekle
         tilesToCheck.Enqueue(startTile);
         visitedTiles.Add(startTile);
        
         while (tilesToCheck.Count > 0)
         {
             Tile currentTile = tilesToCheck.Dequeue();
             linkedGroup.Add(currentTile);
        
             // Komşularını kontrol et
             foreach (Tile neighbor in currentTile.Neighbors)
             {
                 // Eğer bu komşu aynı renge sahipse ve daha önce ziyaret edilmediyse
                 if (neighbor.ColorID == startTile.ColorID && !visitedTiles.Contains(neighbor))
                 {
                     tilesToCheck.Enqueue(neighbor);
                     visitedTiles.Add(neighbor);
                 }
             } 
         }
         return linkedGroup; // Bağlantılı grubu döndür
    }

}
