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
    private Vector2Int? direction = null; // Hareket yönü

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
            if (selectedChips.Contains(chip)) // Daha önce seçilmiş bir çip mi?
            {
                RemoveChipsUntil(chip); // Geri dönülen çipin üstündeki çipleri iptal et
            }
            else if (IsNeighbor(currentTilePosition)) // Komşu ve yeni bir çip mi?
            {
                AddChipToStack(chip); // Çipi Stack'e ekle
                lastTilePosition = currentTilePosition; // Yeni Tile pozisyonunu kaydet
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

    void RemoveChipsUntil(Chip targetChip)
    {
        // Stack'in tepesindeki çip, hedef çip olana kadar çipleri kaldır
        while (selectedChips.Count > 0 && selectedChips.Peek() != targetChip)
        {
            Chip removedChip = selectedChips.Pop();
            removedChip.isSelected(false); // Çipin seçimini kaldır
            Debug.Log($"Çip geri alındı: {removedChip.transform.position}");
        }
    }

    void ClearSelection()
    {
        while (selectedChips.Count > 0)
        {
            Chip chip = selectedChips.Pop();
            chip.isSelected(false); // Tüm çiplerin seçimini kaldır
        }
        lastTilePosition = null; // Son Tile pozisyonunu sıfırla
        direction = null; // Hareket yönünü sıfırla
        isDragging = false;
        currentColorID = -1;
    }

    bool IsNeighbor(Vector2Int currentTilePosition)
    {
        if (selectedChips.Count == 0) return false;

        Chip lastChip = selectedChips.Peek();
        Vector3 lastChipPosition = lastChip.transform.position;

        int lastX = Mathf.FloorToInt(lastChipPosition.x);
        int lastY = Mathf.FloorToInt(lastChipPosition.y);

        // Yukarı, aşağı, sağ, sol komşuları kontrol et
        return (currentTilePosition.x == lastX && Mathf.Abs(currentTilePosition.y - lastY) == 1) || // Yukarı veya aşağı
               (currentTilePosition.y == lastY && Mathf.Abs(currentTilePosition.x - lastX) == 1);   // Sağ veya sol
    }
}
