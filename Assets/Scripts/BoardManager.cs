using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int rows = 8;
    public int columns = 8;
    public SimpleObjectPool chipPool; // Çipler için Object Pool
    public Sprite[] ChipSprites; // Çiplerin görselleri
    public GameObject tilePrefab; // Tile prefab

    private Tile[,] tiles; // Oyun tahtasındaki Tile'lar

    void Start()
    {
        InitializeBoard(); // Tahtayı başlat
    }

    // Tahta başlangıç durumu
    void InitializeBoard()
    {
        tiles = new Tile[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Tile oluştur
                Vector3 position = new Vector3(col, row, 0);
                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity);
                Tile tile = tileObject.GetComponent<Tile>();
                tiles[row, col] = tile;

                // Tile'ı doldur
                SpawnChipForTile(tile);
            }
        }
    }

    // Belirli bir Tile için rastgele bir Chip oluştur
    void SpawnChipForTile(Tile tile)
    {
        GameObject chipObject = chipPool.GetObject();
        chipObject.SetActive(true); // Çipi aktif hale getir
        Chip chip = chipObject.GetComponent<Chip>();

        // Rastgele bir renk belirle ve çipi başlat
        int randomColorID = Random.Range(0, 4);
        chip.Initialize(randomColorID);

        // Çipi Tile'a yerleştir
        tile.SetChip(chipObject);
    }



    // Belirtilen koordinattaki Chip'i yok et
    public void DestroyChip(Chip chip)
    {
        chip.isSelected(false); // Çipin seçili durumunu kaldır
        chip.gameObject.SetActive(false); // Çipi pasif hale getir
        chipPool.ReturnObject(chip.gameObject); // Çipi havuza geri gönder
    }


    public Chip GetChipAt(int x, int y)
    {
        if (x >= 0 && x < columns && y >= 0 && y < rows)
        {
            Tile tile = tiles[y, x];
            return tile?.GetChip(); // Tile'daki çipi döndür
        }
        return null;
    }


    // Boş alanları doldur
    public void RefillBoard()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Tile tile = tiles[row, col];
                if (tile != null && tile.IsEmpty()) // Tile boş mu?
                {
                    SpawnChipForTile(tile); // Yeni çip oluştur ve Tile'ı doldur
                }
            }
        }
        Debug.Log("RefillBoard: Boş alanlar dolduruldu.");
    }
}
