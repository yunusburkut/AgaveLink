using System.Collections.Generic;
using UnityEngine;

public class TileManager
{
    private BoardManager boardManager;

    public TileManager(BoardManager boardManager)
    {
        this.boardManager = boardManager;
    }

    public bool IsTileLinkable(Tile startTile, Tile currentTile, Tile targetTile)
    {
        if (!IsTileConnected(startTile, targetTile, new HashSet<Tile>()))
        {
            return false;
        }

        if (!currentTile.Neighbors.Contains(targetTile))
        {
            return false;
        }

        return true;
    }

    public bool IsTileConnected(Tile origin, Tile target, HashSet<Tile> visited)
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

    public void ResetTileColors(List<Tile> linkedTiles)
    {
        foreach (Tile tile in linkedTiles)
        {
            tile.ResetChipColor();
        }
    }

    public void PushNeighbors(Tile lastLinkedTile, List<Tile> linkedTiles, float pushDistance, Dictionary<Tile, Vector3> originalPositions)
    {
        if (lastLinkedTile == null)
        {
            Debug.LogError("LastLinkedTile is null in PushNeighbors!");
            return;
        }

        if (linkedTiles == null || linkedTiles.Count == 0)
        {
            Debug.LogError("LinkedTiles list is null or empty in PushNeighbors!");
            return;
        }

        foreach (Tile neighbor in lastLinkedTile.Neighbors)
        {
            // Eğer komşu zaten bağlıysa, işlem yapma
            if (linkedTiles.Contains(neighbor))
            {
                continue;
            }

            // Eğer komşuda çip yoksa, işlem yapma
            if (neighbor.CurrentChip == null)
            {
                continue;
            }

            // Komşunun orijinal pozisyonunu kaydet
            if (!originalPositions.ContainsKey(neighbor))
            {
                originalPositions[neighbor] = neighbor.CurrentChip.transform.localPosition;
            }

            // Hareket yönünü hesapla
            Vector3 direction = (neighbor.transform.position - lastLinkedTile.transform.position).normalized;

            // Eğer yön doğru hesaplanmamışsa (örneğin, Vector3.zero ise), işlem yapma
            if (direction == Vector3.zero)
            {
                Debug.LogWarning("Direction is zero for neighbor, skipping push.");
                continue;
            }

            // Çipi hareket ettir
            neighbor.CurrentChip.transform.localPosition += direction * pushDistance;
        }
    }


    public void ResetMovedNeighbors(Dictionary<Tile, Vector3> originalPositions)
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
}
