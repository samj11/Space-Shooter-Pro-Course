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
    private int _powerupOnScreen = 0;
    private int _waveCount = 1;

    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerupContainer;

    private bool _playerIsDead = false;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemiesWaves());
        StartCoroutine(SpawnPowerupWaves());
    }

    IEnumerator SpawnEnemiesWaves()
    {
        while (_playerIsDead == false)
        {
            if (_enemiesAlive == false)
            {
                for (int i = 0; i < _waveCount; i++)
                    SpawnEnemy();

                _waveCount++;
            }

            if (_enemyContainer.transform.childCount == 0)
                _enemiesAlive = false;

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 posSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
        GameObject newEnemy = Instantiate(_enemyPrefab, posSpawn, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
        _enemiesAlive = true;
    }

    IEnumerator SpawnPowerupWaves()
    {
        while (_playerIsDead == false)
        {
            _powerupOnScreen = _powerupContainer.transform.childCount;

            if (_powerupOnScreen <= 1 && _enemiesAlive == true)
            {
                SpawnPowerup();
                yield return new WaitForSeconds(1);
            }

            float randomSec = Random.Range(3, 6);
            yield return new WaitForSeconds(randomSec);
        }
    }

    private void SpawnPowerup()
    {
        Vector3 posSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
        GameObject newPowerup = Instantiate(powerups[PowerupProbabibility()], posSpawn, Quaternion.identity);
        newPowerup.transform.parent = _powerupContainer.transform;
    }

    private int PowerupProbabibility()
    {
        float probability = Random.value;
        if (probability <= .75f)
            return Random.Range(0, 3);

        int rarePowerup = Random.Range(4, 7);
        return rarePowerup;
    }

    public void playerIsDead()
    {
        _playerIsDead = true;
    }
}