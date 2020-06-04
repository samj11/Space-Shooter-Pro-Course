using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSecFire : MonoBehaviour
{

    [SerializeField]
    private float _speed = 10f;
    private GameObject _closestEnemy;

    private void Start()
    {
        _closestEnemy = FindClosestEnemy();
        if(_closestEnemy == null)
        {
            Debug.LogError("No closest enemy found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        GoTowards();
        Boundaries();
    }

    private void Boundaries()
    {
        float posY = transform.position.y;
        float posX = transform.position.x;
        if (posY < -8 || posY > 8 || posX < -11 || posX > 11)
        {
            Destroy(gameObject);
        }
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = 1000;
        foreach (GameObject enemy in enemies)
        {
            Vector3 difference = enemy.transform.position - transform.position;
            float currentDistance = difference.sqrMagnitude;
            if (currentDistance < distance)
            {
                closest = enemy;
                distance = currentDistance;
            }
        }
        return closest;
    }

    private void GoTowards()
    {
        transform.position = Vector3.MoveTowards(transform.position, _closestEnemy.transform.position, _speed *Time.deltaTime);
    }

    //Each laser should follow a different enemy.
    //Right now, if the enemy has been destroyed, the laser that has been shot stays there.
}
