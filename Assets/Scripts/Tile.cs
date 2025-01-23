using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public int ColorID { get; private set; } = -1; // -1 boş bir Tile'ı temsil eder ve çeşitlendirilip özel chipler için kullanılabilir
    public Vector2Int Position { get; private set; } // Tahtadaki pozisyon
    public Chip CurrentChip { get; private set; } // Bu Tile üzerindeki Chip
    public List<Tile> Neighbors; // Komşu Tile'lar

    public void Initialize(Vector2Int position, int colorID)
    {
        Position = position;
        SetColorID(colorID);
    }

    public void setChip(Chip chip)
    {
        CurrentChip = chip;
    }
    public void SetColorIDLoc(int colorID)
    {
        ColorID = colorID;
    }
    public void SetColorID(int colorID)
    {
        ResetChipColor();
        ColorID = colorID;
        if (colorID == -1 && CurrentChip != null)
        {
            // Eğer -1 ise çipi kaldır
            ObjectPooler.Instance.ReleaseChip(CurrentChip);
            CurrentChip = null;
        }
        else if (colorID != -1)
        {
            // Yeni bir Chip at ve rengi güncelle
            CurrentChip = ObjectPooler.Instance.GetChip();
            CurrentChip.SetChipColor(colorID);
            CurrentChip.transform.position = transform.position;
        }
    }
    public void SetChipTemporaryColor(Color color)
    {
        if (CurrentChip != null)
        {
            CurrentChip.SetTemporaryColor(color);
        }
    }
    public void SetTemporaryColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
    public void ResetChipColor()
    {
        if (CurrentChip != null)
        {
            CurrentChip.ResetColor();
        }
    }

    // Komşu Tile'ları listeye ekle
    public void AddNeighbor(Tile neighbor)
    {
        if (!Neighbors.Contains(neighbor))
        {
            Neighbors.Add(neighbor);
        }
    }
}