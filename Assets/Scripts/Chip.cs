using System;
using UnityEngine;

using DG.Tweening; // DoTween kütüphanesini ekle

public class Chip : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public Tween CurrentTween; // Mevcut animasyonu sakla

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

    public void OnDestroy()
    {
        DOTween.KillAll();
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

    // Çipe bir animasyon uygula ve referansını sakla
    public void AnimateToPosition(Vector3 targetPosition, float duration)
    {
        // Eğer mevcut bir animasyon varsa tamamlanmasını bekle
        if (CurrentTween != null && CurrentTween.IsActive() && CurrentTween.IsPlaying())
        {
            CurrentTween.Kill(); // Mevcut animasyonu iptal et (isteğe bağlı)
        }

        // Yeni animasyonu başlat ve referansını sakla
        CurrentTween = transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad);
    }
}
