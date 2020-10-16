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
    private int _enemyTypeID = 1;
    [SerializeField]
    private bool _shieldActive;

    //Fire
    private float _canFire = -2f;
    private float _fireRate = 2f;

    //Lasers
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

        //Choose random enemy and assign a shield
        _enemyTypeID = Random.Range(0, 2);
        _shieldActive = (Random.value > 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
        if (Time.time > _canFire)
        {
            EnemyWeapon();
        }

        BorderLimits();
    }

    //////////////////
    //Enemy behaviours
    //////////////////
    void EnemyMovement()
    {
        if(_enemyTypeID == 0)
        transform.Translate(new Vector3(Mathf.PingPong(Time.time, 1f),-1,0) * _speed * Time.deltaTime);
        else if(_enemyTypeID == 1)
        transform.Translate(new Vector3(Mathf.Sin(Time.time * 2f), -1, 0) * _speed * Time.deltaTime);
    }

    void EnemyWeapon()
    {
        _canFire = Time.time + _fireRate;
        if (_enemyTypeID == 0)
        {
            GameObject _laserShot = Instantiate(_laser, transform.position, Quaternion.identity);
            _laserShot.tag = "EnemyLaser";
            Laser laser = _laserShot.GetComponent<Laser>();
            laser.AssignEnemyFire();
        }
        else if(_enemyTypeID == 1)
        {
            _fireRate *= 1.2f;
            GameObject _laserShot = Instantiate(_laser, transform.position, Quaternion.identity);
            StartCoroutine(LaserScaleLerp(_laserShot, new Vector3(0, 0, 0), new Vector3(2, 7, 0), 5f));
            _laserShot.tag = "EnemyLaser";
            Laser laser = _laserShot.GetComponent<Laser>();
            laser.AssignEnemyFire();
        }    
    }

    IEnumerator LaserScaleLerp(GameObject obj, Vector3 minScale, Vector3 maxScale, float duration)
    {
        float i = 0f;
        while(i < 1)
        {
            obj.transform.localScale = Vector3.Lerp(minScale, maxScale, i);
            i += Time.deltaTime * duration;
                yield return null;
        }
    }
    

    //DESTROY ENEMY methods
    private void BorderLimits()
    {
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
            if (_shieldActive == false)
            {
                _animator.SetTrigger("OnEnemyDestroy");
                _speed = 0;
                _player.AddScore(10);
                _audioSource.Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.5f);
            }
            else
                _shieldActive = false;
        }

        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            if (_shieldActive == false)
            {
                _animator.SetTrigger("OnEnemyDestroy");
                _speed = 0;
                _audioSource.Play();
                Destroy(this.gameObject, 2.5f);
            }
            else
                _shieldActive = false;

        }
    }
}
