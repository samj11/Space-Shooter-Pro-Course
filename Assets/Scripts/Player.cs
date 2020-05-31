﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Speed
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2f;

    //Fire
    [SerializeField]
    private GameObject _laserObj;
    [SerializeField]
    private GameObject _tripleLaser;
    private float _fireRate = 0.3f;
    private float _canFire = -1f;

    //UI & Game design
    private int _lives = 3;
    private int _shieldStrength = 0;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private int _score;

    //GameObj Shield & Thruster
    [SerializeField]
    private GameObject _shieldObj;
    [SerializeField]
    private GameObject _thrusterObj_Left, _thrusterObj_Right;

    //Powerup Activation
    private bool _isTripleShotActive = false;
    private bool _isSpeedActive = false;
    private bool _isShieldActive = false;

    //Audio
    [SerializeField]
    private AudioClip _SoundLaser;
    [SerializeField]
    private AudioClip _SoundExplosion;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager not found");
        }
        if(_uiManager == null)
        {
            Debug.Log("UIManager not found");
        }

        _audioSource = GetComponent<AudioSource>();

        if(_audioSource == null)
        {
            Debug.LogError("AudioSource Player Null");
        }
        else
        {
            _audioSource.clip = _SoundLaser;
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float timeSpeed = _speed * Time.deltaTime;
        float xPos = 11.3f;
        float yPos = 7.0f;

        transform.Translate(new Vector3(horizontalInput, verticalInput) * timeSpeed);

        if (transform.position.x >= xPos)
        {
            transform.position = new Vector3(-xPos, transform.position.y, 0);
        }
        else if (transform.position.x <= -xPos)
        {
            transform.position = new Vector3(xPos, transform.position.y, 0);
        }

        if (transform.position.y >= yPos)
        {
            transform.position = new Vector3(transform.position.x, -yPos, 0);
        }
        else if (transform.position.y <= -yPos)
        {
            transform.position = new Vector3(transform.position.x, yPos, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleLaser, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserObj, transform.position, Quaternion.identity);
        }

        _audioSource.Play();

    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _shieldStrength--;
            _uiManager.UpdateShield(_shieldStrength);
            if(_shieldStrength <= 0)
            {
                _isShieldActive = false;
                _shieldObj.SetActive(false);
            }
            return;
        }

        _lives--;
        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            _thrusterObj_Left.SetActive(true);
        }
        if (_lives == 1)
        {
            _thrusterObj_Right.SetActive(true);
        }

        if(_lives < 1)
        {
            _spawnManager.playerIsDead();
            Destroy(this.gameObject);
            Destroy(_thrusterObj_Left.gameObject);
            Destroy(_thrusterObj_Right.gameObject);
        }
    }

    public void EnableTripleShot()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotCountdown());
    }

    IEnumerator TripleShotCountdown()
    {
        yield return new WaitForSeconds(2f);
        _isTripleShotActive = false;
    }

    public void EnablePowerupSpeed()
    {
        _isSpeedActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(CountdownPowerupSpeed());
    }

    IEnumerator CountdownPowerupSpeed()
    {
        yield return new WaitForSeconds(2f);
        _isSpeedActive = false;
        _speed /= _speedMultiplier;
    }

    public void EnablePowerupShield()
    {
        _shieldStrength = 3;
        _uiManager.UpdateShield(_shieldStrength);
        _isShieldActive = true;
        _shieldObj.SetActive(true);
    }

    public void AddScore(int scorePoints)
    {
        _score += scorePoints;
        _uiManager.UpdateScore(_score);
    }

}
