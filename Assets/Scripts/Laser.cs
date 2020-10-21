using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float speed = 8.0f;

    private bool _isEnemyFire = false;
    private bool _isEnemyFireUp = false;


    // Update is called once per frame
    void Update()
    {

        if (_isEnemyFire == false)
            MoveUp();
        else if (_isEnemyFireUp == false)
            MoveDown();
        else
            MoveUp();

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


    public void AssignEnemyFire(bool fireUp)
    {
        _isEnemyFire = true;
        _isEnemyFireUp = fireUp;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isEnemyFire == true && other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.Damage();
        }

        if(_isEnemyFire == true && other.tag == "Powerup")
        {
            Destroy(other.gameObject);
        }
    }
}
