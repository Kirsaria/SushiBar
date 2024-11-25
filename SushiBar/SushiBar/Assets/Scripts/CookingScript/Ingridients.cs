using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingridients : MonoBehaviour
{
    public GameObject prefab;

    void Start()
    {
        
    }
    public void SpawnPrefab()
    {
        Vector2 position = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        Instantiate(prefab, position, Quaternion.identity);
    }
}
