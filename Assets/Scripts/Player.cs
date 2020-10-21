using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Speed
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2f;

    //Thrusters
    [SerializeField]
    private float _thrusterMultiplier = 1.75f;
    private bool _isThrusterActive = false;
    private bool _isThrusterOverheat = false;
    [SerializeField]
    private float _sliderThrusterUp = 0.75f;
    [SerializeField]
    private float _sliderThrusterDown = 0.4f;

    //Fire
    [SerializeField]
    private GameObject _laserObj;
    [SerializeField]
    private GameObject _tripleLaser;
    [SerializeField]
    private GameObject _secFire;
    private float _fireRate = 0.3f;
    private float _canFire = -1f;

    //UI & Game design
    private int _lives = 3;
    private int _shieldStrength = 0;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private CameraShake _cameraShake;
    private int _score;
    private int _ammoCount = 15;

    //GameObj Shield & Thruster
    [SerializeField]
    private GameObject _shieldObj;
    [SerializeField]
    private GameObject _thrusterObj_Left, _thrusterObj_Right;

    //Powerup Activation
    private bool _isTripleShotActive = false;
    private bool _isSpeedActive = false;
    private bool _isShieldActive = false;
    private bool _isSecFireActive = false;
    private bool _isNegativeActive = false;

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
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        
        if (_spawnManager == null || _uiManager == null || _cameraShake == null)
        {
            Debug.LogError("Spawn Manager, UIManager or Camera not found");
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
        Thrusters();
        Boundaries();
        PowerupCollector();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    //MOVEMENTS
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        float timeSpeed = _speed * Time.deltaTime;

        transform.Translate(new Vector3(horizontalInput, verticalInput) * timeSpeed);
    }

    private void Thrusters()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && _isThrusterOverheat == false)
        {
            _speed *= _thrusterMultiplier;
            _isThrusterActive = true;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && _isThrusterOverheat == false)
        {
            _uiManager.UpdateThrusterSliderUp(_sliderThrusterUp);
            if (_uiManager._UISlider.value == _uiManager._UISlider.maxValue)
                _isThrusterOverheat = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && _isThrusterActive == true)
        {
            _speed /= _thrusterMultiplier;
            _isThrusterActive = false;
        }

        if(_isThrusterActive == true && _isThrusterOverheat == true)
        {
            _speed /= _thrusterMultiplier;
            _isThrusterActive = false;
        }            

        if (_isThrusterActive == false)
        {
            _uiManager.UpdateThrusterSliderDown(_sliderThrusterDown);
            if(_uiManager._UISlider.value == _uiManager._UISlider.minValue)
                _isThrusterOverheat = false;
        }
    }

    private void Boundaries()
    {
        float xPos = 11.3f;
        float yPos = 7.0f;
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

    //FIRE AND DAMAGE

    void FireLaser()
    {
        if(_ammoCount > 0)
        {
            _canFire = Time.time + _fireRate;
            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleLaser, transform.position, Quaternion.identity);
            }
            else if (_isSecFireActive == true)
            {
                Instantiate(_secFire, transform.position, Quaternion.identity);
            }                
            else
            {
                Instantiate(_laserObj, transform.position, Quaternion.identity);
                _ammoCount--;
                _uiManager.UpdateAmmo(_ammoCount);
            }
            _audioSource.Play();
        }
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
        DisplayPlayerHealth();
        StartCoroutine(_cameraShake.Shake(0.3f, 0.1f));

    }

    private void DisplayPlayerHealth()
    {
        _uiManager.UpdateLives(_lives);
        if (_lives == 3)
        {
            _thrusterObj_Left.SetActive(false);
            _thrusterObj_Right.SetActive(false);
        }
        if (_lives == 2)
        {
            _thrusterObj_Left.SetActive(true);
            _thrusterObj_Right.SetActive(false);
        }
        if (_lives == 1)
        {
            _thrusterObj_Right.SetActive(true);
        }

        if (_lives < 1)
        {
            _spawnManager.playerIsDead();
            Destroy(this.gameObject);
            Destroy(_thrusterObj_Left.gameObject);
            Destroy(_thrusterObj_Right.gameObject);
        }
    }

    //SCORE

    public void AddScore(int scorePoints)
    {
        _score += scorePoints;
        _uiManager.UpdateScore(_score);
    }

    //POWERUPS

    void PowerupCollector()
    {
        if(Input.GetKey(KeyCode.C))
        {
            GameObject[] powerups = GameObject.FindGameObjectsWithTag("Powerup");
            foreach(var powerup in powerups)
            {
                powerup.transform.position = Vector3.MoveTowards(powerup.transform.position, transform.position, 1f * _speed * Time.deltaTime);
            }
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
        yield return new WaitForSeconds(3f);
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

    public void EnablePowerupAmmo()
    {
        if(_ammoCount < 15)
        {
            _ammoCount = 15;
            _uiManager.UpdateAmmo(_ammoCount);
        }
    }

    public void EnablePowerupHealth()
    {
        if(_lives < 3)
        {
            _lives++;
            DisplayPlayerHealth();
        }
    }

    public void EnablePowerupSecFire()
    {
        _isSecFireActive = true;
        StartCoroutine(PowerupSecFireCountdown());
    }

    IEnumerator PowerupSecFireCountdown()
    {
        yield return new WaitForSeconds(5f);
        _isSecFireActive = false;
    }

    public void EnablePowerupNegative()
    {
        _isNegativeActive = true;
        _speed /= _speedMultiplier;
        StartCoroutine(PowerupNegativeCountdown());

    }

    IEnumerator PowerupNegativeCountdown()
    {
        yield return new WaitForSeconds(3f);
        _isNegativeActive = false;
        _speed *= _speedMultiplier;
    }
}