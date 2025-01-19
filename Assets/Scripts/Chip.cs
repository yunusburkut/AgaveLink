using UnityEngine;

public class Chip : MonoBehaviour
{
    public Sprite[] sprites; // Çip görselleri

    private int colorID; // Çipin renk ID'si

    public void Initialize(int colorID)
    {
        this.colorID = colorID; // Renk ID'sini sakla
        GetComponent<SpriteRenderer>().sprite = sprites[colorID]; // Sprite'i güncelle
    }

    public void isSelected(bool isSelected)
    {
        GetComponent<SpriteRenderer>().color =  isSelected ? Color.black  : Color.white;
    }
    public int GetColorID() => colorID; // Çipin renk ID'sini döndür
}