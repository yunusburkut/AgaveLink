using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    private Tile startTile;               // İlk seçilen Tile
    private Tile currentTile;             // Şu anda seçili olan Tile
    private List<Tile> linkedTiles = new List<Tile>(); // Bağlı Tile'ların listesi
    private bool isDragging = false;      // Kullanıcının drag yapıp yapmadığını kontrol eder

    public BoardManager boardManager;
    public Color selectedColor = Color.gray; // Seçilen Tile'ların geçici rengi

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
            // Eğer Tile, linklenebilir durumda değilse (başlangıç Tile'ından bağlantılı değilse)
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
                AddTileToLink(tile); // Yeni Tile'ı link'e ekle
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
    }

    private void ResetTileColors()
    {
        foreach (Tile tile in linkedTiles)
        {
            tile.ResetChipColor();
        }
    }

    // Başlangıç Tile'ından bağlantılı olup olmadığını kontrol eder
    private bool IsTileLinkable(Tile tile)
    {
        return IsTileConnected(startTile, tile, new HashSet<Tile>());
    }

    // Rekürsif olarak bir Tile'ın başlangıç Tile'ından bağlantılı olup olmadığını kontrol eder
    private bool IsTileConnected(Tile origin, Tile target, HashSet<Tile> visited)
    {
        if (origin == target) return true; // Eğer aynı Tile ise bağlantılıdır

        visited.Add(origin); // Bu Tile'ı ziyaret edilmiş olarak işaretle

        foreach (Tile neighbor in origin.Neighbors)
        {
            // Eğer komşu Tile daha önce ziyaret edilmediyse ve aynı renkteyse
            if (!visited.Contains(neighbor) && neighbor.ColorID == origin.ColorID)
            {
                // Rekürsif olarak bağlantıyı kontrol et
                if (IsTileConnected(neighbor, target, visited))
                {
                    return true;
                }
            }
        }

        return false; // Bağlantı bulunamadı
    }
}
