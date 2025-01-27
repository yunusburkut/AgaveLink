using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found in the scene!");
        }
    }

    /// <summary>
    /// Kamerayı verilen genişlik ve yükseklik değerlerine göre ayarlar.
    /// </summary>
    /// <param name="boardWidth">Board'un genişliği.</param>
    /// <param name="boardHeight">Board'un yüksekliği.</param>
    public void AdjustCamera(int boardWidth, int boardHeight)
    {
        if (mainCamera == null) return;

        // Kamera pozisyonunu ayarla
        float cameraWidth = boardWidth / 2f ; // Ortalamak için küçük bir kaydırma yapabiliriz
        float cameraHeight = boardHeight / 2f;
        Vector3 newPosition = new Vector3(cameraWidth, cameraHeight, -10f);

        mainCamera.transform.position = newPosition;

        // Kamera uzaklığını (orthographic size) ayarla
        mainCamera.orthographicSize = cameraHeight; // Hafif bir genişlik ekleyerek rahat görünüm sağlıyoruz
    }
}