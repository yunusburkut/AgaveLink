using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    private Tile startTile;               // İlk seçilen Tile
    private Tile currentTile;             // Şu anda seçili olan Tile
    private List<Tile> linkedTiles = new List<Tile>(); // Bağlı Tile'ların listesi
    private Dictionary<Tile, Vector3> originalPositions = new Dictionary<Tile, Vector3>(); // Komşuların orijinal pozisyonlarını saklar
    private bool isDragging = false;      // Kullanıcının drag yapıp yapmadığını kontrol eder

    public BoardManager boardManager;
    public Color selectedColor = Color.gray; // Seçilen Tile'ların geçici rengi

    public float pushDistance = 0.2f;     // İtilme mesafesi
    public float moveDuration = 0.2f;    // Hareket süresi (smooth hareket için)

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnInputStart();
        }

        if (isDragging)
        {
            OnInputDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnInputEnd();
        }
    }

    private void OnInputStart()
    {
        Tile tile = GetTileUnderMouse();

        if (tile != null && tile.ColorID != -1)
        {
            startTile = tile;                   // İlk seçilen Tile
            currentTile = tile;                 // Şu anda aktif olan Tile
            AddTileToLink(tile);                // Tile'ı link'e ekle
            PushNeighbors();                    // Komşuları ittir
            isDragging = true;                  // Drag işlemini başlat
        }
    }

    private void OnInputDrag()
    {
        Tile tile = GetTileUnderMouse();

        if (tile == currentTile) return; // Aynı Tile'a tekrar işlem yapma

        // Eğer yeni bir Tile seçildiyse
        if (tile != null && tile.ColorID == startTile.ColorID)
        {
            // Eğer Tile, başlangıç Tile'ından bağlantılı değilse veya komşu değilse
            if (!IsTileLinkable(tile))
            {
                return; // Seçime izin verme
            }

            // Geri gitme durumu: Kullanıcı bir önceki Tile'a dönerse
            if (linkedTiles.Count > 1 && tile == linkedTiles[linkedTiles.Count - 2])
            {
                RemoveLastTileFromLink(); // Son Tile'ı listeden çıkar
            }
            // İleri gitme durumu: Kullanıcı komşu ve aynı renkteki bir Tile seçerse
            else if (!linkedTiles.Contains(tile) && currentTile.Neighbors.Contains(tile))
            {
                AddTileToLink(tile);      // Yeni Tile'ı link'e ekle
                PushNeighbors();         // Komşuları ittir
            }

            currentTile = tile; // Şu anda aktif olan Tile'ı güncelle
        }
    }

    private void OnInputEnd()
    {
        isDragging = false;

        // Geçerli bir link oluşturulmuş mu? (en az 3 Tile)
        if (linkedTiles.Count >= 3)
        {
            boardManager.DestroyTiles(linkedTiles); // BoardManager'a link'i gönder ve yok et
        }
        else
        {
            ResetTileColors(); // Geçerli bir link oluşmadıysa renkleri sıfırla
        }

        // Hareket etmiş komşuları sıfırla
        ResetMovedNeighbors();

        // Seçim işlemini sıfırla
        linkedTiles.Clear();
        startTile = null;
        currentTile = null;
    }

    private Tile GetTileUnderMouse()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = boardManager.GetGridPositionFromWorld(mouseWorldPosition);

        if (boardManager.IsInsideBoard(gridPosition))
        {
            return boardManager.GetTileAtPosition(gridPosition);
        }

        return null;
    }

    private void AddTileToLink(Tile tile)
    {
        linkedTiles.Add(tile);                   // Tile'ı listeye ekle
        tile.SetChipTemporaryColor(selectedColor); // Çipin rengini geçici olarak değiştir
    }

    private void RemoveLastTileFromLink()
    {
        if (linkedTiles.Count > 0)
        {
            Tile lastTile = linkedTiles[linkedTiles.Count - 1]; // Son Tile'ı al
            linkedTiles.RemoveAt(linkedTiles.Count - 1);        // Son Tile'ı listeden çıkar
            lastTile.ResetChipColor();                         // Çipin rengini eski haline döndür
        }

        // Son eleman değişmiş olabilir, komşuları yeniden düzenle
        PushNeighbors();
    }

    private void ResetTileColors()
    {
        foreach (Tile tile in linkedTiles)
        {
            tile.ResetChipColor();
        }
    }

    // Son seçilen Tile'ın komşularını ittir
    private void PushNeighbors()
    {
        // Daha önce hareket etmiş komşuları sıfırla
        ResetMovedNeighbors();

        // Eğer linkedTiles listesi boşsa işlem yapma
        if (linkedTiles.Count == 0) return;

        // LinkedTiles listesinin son elemanını al
        Tile lastLinkedTile = linkedTiles[linkedTiles.Count - 1];

        foreach (Tile neighbor in lastLinkedTile.Neighbors)
        {
            // Eğer komşu zaten linklenmişse (linkedTiles listesinde varsa), işlem yapma
            if (linkedTiles.Contains(neighbor))
            {
                continue;
            }

            if (neighbor.CurrentChip != null)
            {
                // Komşunun orijinal pozisyonunu kaydet
                if (!originalPositions.ContainsKey(neighbor))
                {
                    originalPositions[neighbor] = neighbor.CurrentChip.transform.localPosition;
                }

                // Hareket yönünü hesapla (lastLinkedTile merkezine göre normalize)
                Vector3 direction = (neighbor.transform.position - lastLinkedTile.transform.position).normalized;

                // Çipi hareket ettir
                neighbor.CurrentChip.transform.localPosition += direction * pushDistance;
            }
        }
    }

    // Daha önce hareket etmiş komşuların pozisyonlarını sıfırla
    private void ResetMovedNeighbors()
    {
        foreach (var entry in originalPositions)
        {
            Tile neighbor = entry.Key;
            Vector3 originalPosition = entry.Value;

            if (neighbor.CurrentChip != null)
            {
                neighbor.CurrentChip.transform.localPosition = originalPosition; // Orijinal pozisyona dön
            }
        }

        originalPositions.Clear(); // Pozisyon verilerini temizle
    }

    // Başlangıç Tile'ından bağlantılı ve komşu olup olmadığını kontrol eder
    private bool IsTileLinkable(Tile tile)
    {
        if (!IsTileConnected(startTile, tile, new HashSet<Tile>()))
        {
            return false;
        }

        if (!currentTile.Neighbors.Contains(tile))
        {
            return false;
        }

        return true;
    }

    private bool IsTileConnected(Tile origin, Tile target, HashSet<Tile> visited)
    {
        if (origin == target) return true;

        visited.Add(origin);

        foreach (Tile neighbor in origin.Neighbors)
        {
            if (!visited.Contains(neighbor) && neighbor.ColorID == origin.ColorID)
            {
                if (IsTileConnected(neighbor, target, visited))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
