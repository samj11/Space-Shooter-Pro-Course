using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    private Player _player;
    private bool _playerIsNear;
    private bool _isPlayerLaserNear;

    private Animator _animator;
    private AudioSource _audioSource;

    [SerializeField]
    private int _enemyTypeID = 1;
    private bool _enemyIsBoss = false;

    [SerializeField]
    private bool _shieldActive;

    //Boss
    private bool bossInPos = false;
    private float _bossPosition = 5f;
    [SerializeField]
    private float _bossLeftPos = -3f;
    [SerializeField]
    private float _bossRightPos = 3f;
    private bool reachedLeftPos;

    //Fire
    private float _canFire = -2f;
    private float _fireRate = 2f;
    private float _canRapidFire;
    private float _fireRateBack = 0.05f;

    //Lasers
    [SerializeField]
    private GameObject _laser;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.LogError("Player obj not found");

        _animator = gameObject.GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Animator is null");

        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null)
            Debug.LogError("AudioSource on Enemy NULL");
    }

    // Update is called once per frame
    void Update()
    {
        if(_enemyIsBoss == false)
        {
            BorderLimits();
            EnemyMovement();
            if (Time.time > _canFire)
                EnemyWeapon();

            if (Time.time > _canRapidFire)
            {
                WeaponBehind();
                ShootPowerup();
            }
        }
        else
        {
            BossMovement();
            if(bossInPos && Time.time > _canFire)
                BossWeapon();
        }
    }

    public void EnemyType(bool isBoss)
    {
        _enemyIsBoss = isBoss;
        if (_enemyIsBoss == false)
        {
            _enemyTypeID = Random.Range(0, 2);
            _shieldActive = (Random.value > 0.5f);
        }
        else
            _shieldActive = true;
    }

    //////////////////
    //Enemy behaviours
    //////////////////
    void EnemyMovement()
    {
        float velocity = _speed * Time.deltaTime;

     //Come to player
        float distancePlayer = Vector3.Distance(_player.transform.position, transform.position);
        if (distancePlayer < 4)
        {
            _playerIsNear = true;
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, velocity *1.25f);
        }
        else
            _playerIsNear = false;
    //Normal movement
        if(_playerIsNear == false)
        {
            if (_enemyTypeID == 0)
            {
                if (_isPlayerLaserNear)
                    LaserDodging();
                else
                    transform.Translate(new Vector3(Mathf.PingPong(Time.time, 1f), -1, 0) * velocity);
            }
                
            else if (_enemyTypeID == 1)
            {
                if (_isPlayerLaserNear)
                    LaserDodging();
                else
                    transform.Translate(new Vector3(Mathf.Sin(Time.time * 2f), -1, 0) * velocity);
            }
        }
    }

    void BossMovement()
    {
        float step = _speed * Time.deltaTime;

        if (transform.position.y > _bossPosition)
            transform.Translate(new Vector3(0, -1, 0) * step);
        else
            bossInPos = true;

        if(bossInPos == true)
        {
            if (transform.position.x == _bossLeftPos)
                reachedLeftPos = true;
            else if (transform.position.x == _bossRightPos)
                reachedLeftPos = false;

            if (reachedLeftPos == false)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(_bossLeftPos, _bossPosition, 0), step);

            else if(reachedLeftPos == true)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(_bossRightPos, _bossPosition, 0), step);
        }
    }

    void BossWeapon()
    {

        _canFire = Time.time + _fireRate;

        int laserAngle = -75;

        for(int i = 0; i < 12; i++)
        {
            GameObject _laserShot = Instantiate(_laser, transform.position, Quaternion.Euler(0, 0, laserAngle));
            _laserShot.tag = "EnemyLaser";
            Laser laser = _laserShot.GetComponent<Laser>();
            laser.AssignEnemyFire(false);

            laserAngle = laserAngle + 15;
        }
    }

    public void LaserNear(bool isPlayerLaserNear)
    {
        _isPlayerLaserNear = isPlayerLaserNear;
    }

    void LaserDodging()
    {
        transform.Translate(new Vector3(-1, -1, 0) * _speed * Time.deltaTime);
    }    

    void EnemyWeapon()
    {
        _canFire = Time.time + _fireRate;
        if (_enemyTypeID == 0)
        {
            GameObject _laserShot = Instantiate(_laser, transform.position, Quaternion.identity);
            _laserShot.tag = "EnemyLaser";
            Laser laser = _laserShot.GetComponent<Laser>();
            laser.AssignEnemyFire(false);
        }
        else if(_enemyTypeID == 1)
        {
            _fireRate *= 1.2f;
            GameObject _laserShot = Instantiate(_laser, transform.position, Quaternion.identity);
            StartCoroutine(LaserScaleLerp(_laserShot, new Vector3(0, 0, 0), new Vector3(2, 7, 0), 5f));
            _laserShot.tag = "EnemyLaser";
            Laser laser = _laserShot.GetComponent<Laser>();
            laser.AssignEnemyFire(false);
        }    
    }

    IEnumerator LaserScaleLerp(GameObject obj, Vector3 minScale, Vector3 maxScale, float duration)
    {
        float i = 0f;
        while (i < 1)
        {
            obj.transform.localScale = Vector3.Lerp(minScale, maxScale, i);
            i += Time.deltaTime * duration;
            yield return null;
        }
    }

    void WeaponBehind()
    {
        _canRapidFire = Time.time + _fireRateBack;

        int layerMaskPlayer = 1 << 10;
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.up, Mathf.Infinity, layerMaskPlayer);

        if(hitInfo.collider != null)
        {
            GameObject _laserShot = Instantiate(_laser, transform.position, Quaternion.identity);
            _laserShot.tag = "EnemyLaser";
            Laser laser = _laserShot.GetComponent<Laser>();
            laser.AssignEnemyFire(true);
        }
    }


    void ShootPowerup()
    {
        _canRapidFire = Time.time + _fireRateBack;

        int layerMaskPowerup = 1 << 9;
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, layerMaskPowerup);

        if(hitInfo.collider != null)
        {
            GameObject _laserShot = Instantiate(_laser, transform.position, Quaternion.identity);
            _laserShot.tag = "EnemyLaser";
            Laser laser = _laserShot.GetComponent<Laser>();
            laser.AssignEnemyFire(false);
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