using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager instance;
    private FurnitureSpawnPoint[] _furnitureSpawnPoint;
    [SerializeField] private float _timeBetweenFurnitureSpawn;
    [SerializeField] private int _maxFurnitureNumber;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _furnitureSpawnPoint = GetComponentsInChildren<FurnitureSpawnPoint>();
        StartCoroutine(SpawnFurniture());
    }

    private IEnumerator SpawnFurniture()
    {
        while (GameManager.instance.isPlayerAlive)
        {
            yield return new WaitForSeconds(_timeBetweenFurnitureSpawn);
            int n = FindObjectsOfType<InanimateObject>().Length;
            Debug.Log(n);
            if (n < _maxFurnitureNumber)
            {
                _furnitureSpawnPoint[Random.Range(0, _furnitureSpawnPoint.Length)].SpawnFurnitures();
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.ItemAlert);
            }
        }
    }


}