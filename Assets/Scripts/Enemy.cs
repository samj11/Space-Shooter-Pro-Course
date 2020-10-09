using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;

    //Fire
    private float _canFire = -2f;
    private float _fireRate = 2f;

    [SerializeField]
    private GameObject _laser;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player obj not found");
        }
        _animator = gameObject.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is null");
        }

        _audioSource = GetComponent<AudioSource>();

        if(_audioSource == null)
        {
            Debug.LogError("AudioSource on Enemy NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
        {
            EnemyLaser();
        }


    }

    void CalculateMovement()
    {
        transform.Translate(new Vector3(Mathf.PingPong(Time.time, 1f),-1,0) * _speed * Time.deltaTime);

        if (transform.position.y < -5)
        {
            float randomTop = Random.Range(-9, 9);
            transform.position = new Vector3(randomTop, 8, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other.tag == "Weapon")
        {
            Destroy(other.gameObject);
            _animator.SetTrigger("OnEnemyDestroy");
            _speed = 0;
            _player.AddScore(10);
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);

        }

        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            _animator.SetTrigger("OnEnemyDestroy");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 2.5f);
        }
    }


    void EnemyLaser()
    {
        _canFire = Time.time + _fireRate;
        GameObject _laserShot = Instantiate(_laser, transform.position, Quaternion.identity);
        _laserShot.tag = "EnemyLaser";
        Laser laser = _laserShot.GetComponent<Laser>();
        laser.AssignEnemyFire();
    }
}
