using UnityEngine;

public class Chip : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetChipColor(int colorID)
    {
        // ColorID'ye göre sprite'ı ayarla
        spriteRenderer.sprite = GameManager.Instance.GetChipSprite(colorID);

        // Orijinal rengi sakla
        originalColor = spriteRenderer.color;
    }

    // Geçici bir renk uygula
    public void SetTemporaryColor(Color color)
    {
        spriteRenderer.color = color;
    }

    // Rengi eski haline döndür
    public void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }
}