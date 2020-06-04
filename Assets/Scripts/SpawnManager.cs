using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] powerups;

    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerupContainer;

    private bool _playerIsDead = false;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while(_playerIsDead == false)
        {
            Vector3 posSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while(_playerIsDead == false)
        {
            float randomSec = Random.Range(1, 2);
            Vector3 posSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newPowerup = Instantiate(powerups[GetRandomNumberProbabibility()], posSpawn, Quaternion.identity);
            newPowerup.transform.parent = _powerupContainer.transform;
            yield return new WaitForSeconds(randomSec);
        }
    }

    private int GetRandomNumberProbabibility()
    {
        float probability = Random.value;
        if (probability <= .9f)
            return Random.Range(0, 5);
        return 5;
    }

    public void playerIsDead()
    {
        _playerIsDead = true;
    }
}