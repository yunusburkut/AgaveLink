using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectPool : MonoBehaviour
{
    public GameObject prefab;       // Havuz için kullanılacak prefab
    public int initialPoolSize = 64; // Havuzun başlangıç boyutu

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // Havuzu başlat
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = CreateNewObject();
            pool.Enqueue(obj);
        }
    }

    // Havuzdan nesne al
    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return CreateNewObject();
        }
    }

    // Nesneyi havuza geri gönder
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    // Yeni bir nesne oluştur ve havuza ekle
    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        return obj;
    }
}