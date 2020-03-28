using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float speed = 8.0f;

    private bool _isEnemyFire = false;


    // Update is called once per frame
    void Update()
    {

        if (_isEnemyFire == false)
        {
            MoveUp();
        } else
        {
            MoveDown();
        }

    }

    private void MoveUp()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (transform.position.y > 8)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < -8)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }


    public void AssignEnemyFire()
    {
        _isEnemyFire = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isEnemyFire == true && other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.Damage();
        }

            
    }

}
