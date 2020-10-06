using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    private float _speed = 2f;

    [SerializeField] // 0= triplesht 1= speed 2= shield 3=ammo 4= health 5= sec weapon
    private int powerupID;

    [SerializeField]
    private AudioClip _clipPowerup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if(transform.position.y < -6)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if(player != null)
            {
                AudioSource.PlayClipAtPoint(_clipPowerup, transform.position);

                switch (powerupID)
                {
                    case 0:
                        player.EnableTripleShot();
                        break;
                    case 1:
                        player.EnablePowerupSpeed();
                        break;
                    case 2:
                        player.EnablePowerupShield();
                        break;
                    case 3:
                        player.EnablePowerupAmmo();
                        break;
                    case 4:
                        player.EnablePowerupHealth();
                        break;
                    case 5:
                        player.EnablePowerupSecFire();
                        break;
                    case 6:
                        player.EnablePowerupNegative();
                        break;
                    default:
                        break;
                }
            }
            
            Destroy(this.gameObject);
        }
    }
}