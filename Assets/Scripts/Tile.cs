using UnityEngine;

public class Tile : MonoBehaviour
{
    private Chip chip;

    public void SetChip(GameObject newChip)
    {
        chip = newChip.GetComponent<Chip>();
        chip.transform.position = transform.position;
    }

    public Chip GetChip() => chip;

    public bool IsEmpty() => chip == null;

    public void ClearChip()
    {
        chip = null;
    }
}