using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public BoardManager boardManager;

    private Stack<Chip> selectedChips = new Stack<Chip>(); // Seçilen çipler için Stack
    private int currentColorID; // Seçilen çiplerin renk ID'si
    private bool isDragging = false; // Mouse basılı mı?
    private Vector2Int? lastTilePosition = null; // Son ziyaret edilen Tile'ın pozisyonu

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Mouse sol tuşuna basıldı
        {
            StartDragging();
        }

        if (Input.GetMouseButton(0) && isDragging) // Mouse basılı tutuluyor
        {
            ContinueDragging();
        }

        if (Input.GetMouseButtonUp(0) && isDragging) // Mouse sol tuşu bırakıldı
        {
            StopDragging();
        }
    }

    void StartDragging()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.FloorToInt(worldPosition.x);
        int y = Mathf.FloorToInt(worldPosition.y);

        Chip startChip = boardManager.GetChipAt(x, y);
        if (startChip != null)
        {
            currentColorID = startChip.GetColorID(); // İlk çipin renk ID'sini belirle
            AddChipToStack(startChip); // İlk çipi Stack'e ekle
            lastTilePosition = new Vector2Int(x, y); // İlk Tile pozisyonunu kaydet
            isDragging = true;
        }
    }

    void ContinueDragging()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.FloorToInt(worldPosition.x);
        int y = Mathf.FloorToInt(worldPosition.y);

        // Eğer yeni bir Tile'a geçilmediyse işlemi durdur
        if (lastTilePosition != null && lastTilePosition.Value == new Vector2Int(x, y))
        {
            return; // Aynı Tile, işlemi durdur
        }

        Vector2Int currentTilePosition = new Vector2Int(x, y);
        Chip chip = boardManager.GetChipAt(x, y);

        if (chip != null && chip.GetColorID() == currentColorID)
        {
            // Eğer iki çip arasında link varsa, aradaki çipleri de ekle
            if (AddIntermediateChips(lastTilePosition.Value, currentTilePosition))
            {
                AddChipToStack(chip); // Çipi Stack'e ekle
                lastTilePosition = currentTilePosition; // Yeni Tile pozisyonunu kaydet
            }
            else
            {
                Debug.LogWarning("Bağlantı zinciri bulunamadı. Seçim yapılamaz.");
            }
        }
    }

    void StopDragging()
    {
        if (selectedChips.Count > 1) // En az iki çip seçilmişse
        {
            foreach (Chip chip in selectedChips)
            {
                boardManager.DestroyChip(chip); // Çipleri yok et
            }
        }
        else
        {
            Debug.LogWarning("Yeterli çip seçilmedi, işlem iptal edildi!");
        }

        ClearSelection(); // Stack'i temizle
    }

    void AddChipToStack(Chip chip)
    {
        chip.isSelected(true); // Çipi seçili olarak işaretle
        selectedChips.Push(chip); // Çipi Stack'e ekle
        Debug.Log($"Çip seçildi: {chip.transform.position}");
    }

    void ClearSelection()
    {
        while (selectedChips.Count > 0)
        {
            Chip chip = selectedChips.Pop();
            chip.isSelected(false); // Tüm çiplerin seçimini kaldır
        }
        lastTilePosition = null; // Son Tile pozisyonunu sıfırla
        isDragging = false;
        currentColorID = -1;
    }

    bool AddIntermediateChips(Vector2Int start, Vector2Int end)
    {
        bool canLink = true;

        // Yalnızca yatay veya dikey hareketler kontrol edilir
        if (start.x == end.x) // Dikey hareket
        {
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            for (int y = minY + 1; y < maxY; y++) // Aradaki çipleri kontrol et
            {
                Chip chip = boardManager.GetChipAt(start.x, y);
                if (chip == null || chip.GetColorID() != currentColorID) // Aynı renk değilse
                {
                    canLink = false; // Link mümkün değil
                    break;
                }
                if (!selectedChips.Contains(chip)) // Daha önce seçilmediyse
                {
                    AddChipToStack(chip); // Çipi Stack'e ekle
                }
            }
        }
        else if (start.y == end.y) // Yatay hareket
        {
            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);

            for (int x = minX + 1; x < maxX; x++) // Aradaki çipleri kontrol et
            {
                Chip chip = boardManager.GetChipAt(x, start.y);
                if (chip == null || chip.GetColorID() != currentColorID) // Aynı renk değilse
                {
                    canLink = false; // Link mümkün değil
                    break;
                }
                if (!selectedChips.Contains(chip)) // Daha önce seçilmediyse
                {
                    AddChipToStack(chip); // Çipi Stack'e ekle
                }
            }
        }
        else
        {
            canLink = false; // Köşegen hareketler desteklenmez
        }

        return canLink;
    }
}
