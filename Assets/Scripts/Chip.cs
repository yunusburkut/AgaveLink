using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour
{
    public Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    public void UpdateChipColor(int colorID)
    {
        GetComponent<SpriteRenderer>().sprite = sprites[colorID];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
