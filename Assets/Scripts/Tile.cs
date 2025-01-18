using UnityEngine;

public class Tile : MonoBehaviour
{
    private Chip chip; // Tile içindeki Chip

    public void SetChip(GameObject newChip,int colorID)
    {
        chip = newChip.GetComponent<Chip>();
        chip.transform.position = transform.position; // Chip'i Tile'ın konumuna taşı
        chip.UpdateChipColor(colorID);
    }

    public Chip GetChip() => chip;

    public bool IsEmpty() => chip == null;

    public void ClearChip()
    {
        chip = null; // Chip referansını temizle
    }

    // Tile içindeki Chip'i yok et
    public void DestroyChip()
    {
        if (chip != null)
        {
            Destroy(chip.gameObject); // Chip'i yok et
            ClearChip(); // Tile'daki Chip referansını temizle
        }
    }
}