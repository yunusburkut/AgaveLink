using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    public int Width;
    public int Height;
    private int poolingHeight; /// dinamik yap
    public GameObject tilePrefab;
    public Transform boardParent;

    private Tile[,] board;

    private void Start()
    {
        poolingHeight = Height * 2;
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        board = new Tile[Width, poolingHeight];
        float cameraWidht = Width;
        float cameraHeight = Height;
        cameraWidht /= 2;
        cameraHeight /= 2;
        Vector3 newPosition = new Vector3(cameraWidht, cameraHeight, -10);
        // Ana kameranın pozisyonunu row sayısına göre değiştiriyoruz uzaklıgınıda ona göre ayarlıyoruz
        Camera.main.transform.position = newPosition;
        Camera.main.orthographicSize = cameraHeight;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < poolingHeight; y++)
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
                if (y < (poolingHeight/2)-1) // board'un en üst satırının bi altını alıyo böylelikle komşu hesaplamasına yukarıda spawnnladııgımız chipler hesabaa girmiyor
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
        return position.x >= 0 && position.x < Width && position.y >= 0 && position.y < poolingHeight;
    }

    public Tile GetTileAtPosition(Vector2Int position)
    {
        return board[position.x, position.y];
    }
    
    public void DestroyTiles(List<Tile> tilesToDestroy)
    {
        GameManager.Instance.AddScore(tilesToDestroy.Count);
        
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
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < poolingHeight; y++)
            {
                Tile currentTile = board[x, y];

                if (currentTile.ColorID == -1)
                {
                    for (int k = y; k < poolingHeight; k++)
                    {
                        Tile aboveTile = board[x, k];

                        if (aboveTile.ColorID != -1)
                        {
                            MoveChipDownDOTween(aboveTile, currentTile);

                            currentTile.SetColorIDLoc(aboveTile.ColorID);
                            aboveTile.SetColorIDLoc(-1);

                            break;
                        }
                    }
                }
            }
        }
    }

    private void MoveChipDownDOTween(Tile fromTile, Tile toTile)
    {
        if (fromTile.CurrentChip == null)
        {
            return;
        }

        Chip movingChip = fromTile.CurrentChip;

        // DOTween ile animasyon başlat
        movingChip.transform.DOMove(toTile.transform.position, 0.4f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            // Animasyon tamamlandığında çipi yeni tile ile ilişkilendir
            toTile.setChip(movingChip);
            fromTile.setChip(null);
            ShuffleTiles();
        });
    }


    private void FillEmptyTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < poolingHeight; y++)
            {
                Tile tile = board[x, y];
                if (tile.ColorID == -1)
                {
                    tile.SetColorID(Random.Range(0, 4)); // Rastgele yeni renk ata
                }
            }
        }
    }
 

    public void ResetGame()
    {
        Debug.Log("Game is resetting...");

        // 1. Tüm çipleri sıfırla ve pool'a geri gönder
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < poolingHeight; y++)
            {
                Tile tile = board[x, y];

                if (tile.CurrentChip != null)
                {
                    // Çipi pool'a geri gönder
                    ObjectPooler.Instance.ReleaseChip(tile.CurrentChip);
                    tile.setChip(null); // Tile ile çip ilişiğini kes
                }

                // Tile'ı sıfırla
                tile.SetColorID(-1); // Boş renk (örneğin -1, boş bir state için kullanılabilir)
            }
        }

        // 2. Board'da yapılan tüm hesaplamaları sıfırla (örneğin, bağlantılar)
        foreach (Tile tile in board)
        {
            tile.ClearNeighbors(); // Komşular listesini temizle
        }

        // 3. Yeni bir board başlat
        
    }
    public void ShuffleTiles()
    {
        Debug.Log("Shuffling tiles...");
    
        // 1. Görünür board'daki tüm tile'ları topla
        List<Tile> visibleTiles = new List<Tile>();
        List<int> colorIDs = new List<int>();
    
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++) // Yalnızca görünür kısmı dolaş
            {
                Tile tile = board[x, y];
                if (tile.CurrentChip != null) // Eğer tile'da çip varsa
                {
                    visibleTiles.Add(tile); // Tile'ı ekle
                    colorIDs.Add(tile.ColorID); // Mevcut ColorID'yi listeye ekle
                }
            }
        }
    
        // 2. Potansiyel bir grup bulunana kadar karıştırmayı dene
        bool hasPotentialGroups = false;
    
        // Maksimum deneme sayısı
        int maxAttempts = 50; // Sonsuz döngüyü önlemek için bir sınır koyuyoruz
        int attempts = 0;
    
        while (!hasPotentialGroups && attempts < maxAttempts)
        {
            // ColorID'leri karıştır
            ShuffleList(colorIDs);
    
            // Simüle edilmiş ColorID'leri tile'lara at
            for (int i = 0; i < visibleTiles.Count; i++)
            {
                visibleTiles[i].SetColorIDLoc(colorIDs[i]);
            }
    
            // Potansiyel linklenebilir grup olup olmadığını kontrol et
            hasPotentialGroups = HasPotentialLinkedGroups();
            attempts++;
        }
    
        // Eğer hiçbir linklenebilir grup bulunamazsa işlemi sonlandır
        if (!hasPotentialGroups)
        {
            Debug.LogWarning("No potential groups found after shuffling. Shuffle skipped.");
            return;
        }
    
       
    
        Debug.Log("Tiles shuffled successfully!");
    }
    
    // Rastgele bir listeyi karıştırır (Fisher-Yates Shuffle algoritması)
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // Swap işlemi
        }
    }
    
    // Board'da potansiyel bir linklenebilir grup olup olmadığını kontrol eder
    private bool HasPotentialLinkedGroups()
    {
        HashSet<Tile> visitedTiles = new HashSet<Tile>(); // Ziyaret edilen tile'ları takip eder
    
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile startTile = board[x, y];
    
                // Eğer tile zaten ziyaret edildiyse veya boşsa kontrol etme
                if (visitedTiles.Contains(startTile) || startTile.ColorID == -1)
                    continue;
    
                // Bağlantılı grubu bul
                List<Tile> linkedGroup = GetLinkedGroup(startTile, visitedTiles);
    
                // Eğer bağlantılı grup en az 3 tile içeriyorsa, potansiyel bir grup var
                if (linkedGroup.Count >= 3)
                {
                    return true;
                }
            }
        }
    
        return false; // Hiçbir linklenebilir grup yok
    }
    
    // Tile'lar arasında bağlantılı bir grup bulur
    private List<Tile> GetLinkedGroup(Tile startTile, HashSet<Tile> visitedTiles)
    {
        List<Tile> linkedGroup = new List<Tile>();
        Queue<Tile> tilesToCheck = new Queue<Tile>();
    
        tilesToCheck.Enqueue(startTile);
        visitedTiles.Add(startTile);
    
        while (tilesToCheck.Count > 0)
        {
            Tile currentTile = tilesToCheck.Dequeue();
            linkedGroup.Add(currentTile);
    
            foreach (Tile neighbor in currentTile.Neighbors)
            {
                if (neighbor.ColorID == startTile.ColorID && !visitedTiles.Contains(neighbor))
                {
                    tilesToCheck.Enqueue(neighbor);
                    visitedTiles.Add(neighbor);
                }
            }
        }
        return linkedGroup;
    }
}   
