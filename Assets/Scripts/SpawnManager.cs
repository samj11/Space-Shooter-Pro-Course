using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] powerups;

    private bool _enemiesAlive = false;
    private int _waveCount = 1;

    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerupContainer;

    private bool _playerIsDead = false;

    public void StartSpawning()
    {
        StartCoroutine(SpawnWaves());
        StartCoroutine(SpawnPowerupRoutine());
    }


    IEnumerator SpawnWaves()
    {
        while (_playerIsDead == false)
        {
            if (_enemiesAlive == false)
            {
                for (int i = 0; i < _waveCount; i++)
                {
                    SpawnEnemy();
                }
                _waveCount++;
            }

            if (_enemyContainer.transform.childCount == 0)
                _enemiesAlive = false;

            yield return new WaitForSeconds(1.0f);
            Debug.Log("number of waves" + _waveCount);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 posSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
        GameObject newEnemy = Instantiate(_enemyPrefab, posSpawn, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
        _enemiesAlive = true;   
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
            return Random.Range(0, powerups.Length);
        return 5;
    }

    public void playerIsDead()
    {
        _playerIsDead = true;
    }
}