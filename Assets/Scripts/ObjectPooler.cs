using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    public BoardSettings boardSettings;

    [SerializeField] private GameObject chipPrefab; 
    private int initialPoolSize;

    private Queue<Chip> chipPool = new Queue<Chip>();

    private void Awake()
    {
        Instance = this;
        initialPoolSize = (boardSettings.Width * boardSettings.Height)*2;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewChip();
        }
    }

    private void CreateNewChip()
    {
        GameObject chipObject = Instantiate(chipPrefab);
        chipObject.SetActive(false);

        Chip chip = chipObject.GetComponent<Chip>();
        chipPool.Enqueue(chip);
    }

    public Chip GetChip()
    {
        if (chipPool.Count == 0)
            CreateNewChip();

        Chip chip = chipPool.Dequeue();
        chip.gameObject.SetActive(true);
        return chip;
    }

    public void ReleaseChip(Chip chip)
    {
        chip.gameObject.SetActive(false);
        chipPool.Enqueue(chip);
    }
}