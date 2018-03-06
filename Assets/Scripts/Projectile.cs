using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float _MovementSpeed = 5f;
    public ParticleSystem _ImpactEffect;

    private TankController _Owner;
    private int _Damage = 20;
    private float _DistanceTravelled = 0f;
    private float _MaxDistance = 1000f;

    void Start() {


    }

    void Update() {

        // Constantly move forward
        transform.position += transform.forward * _MovementSpeed * Time.deltaTime;

        // Destroy gameObject when it has reached max distance threshold
        if (_DistanceTravelled < _MaxDistance) _DistanceTravelled += Time.deltaTime;
        else Destroy(gameObject);
    }

    // Colliding with kinematic objects
    public void OnCollisionEnter(Collision collision) {
        
        // Check for valid damagable object
        if (collision.gameObject.tag == "Destroyable") {

            // If not our owner that the projectile is colliding against
            TankController tank = collision.gameObject.GetComponent<TankController>();
            
            if (tank != _Owner) {

                // Get reference to the damagable component
                Health obj = collision.gameObject.GetComponent<Health>();

                obj.Damage(_Damage);

                // Play impact effect
                if (_ImpactEffect) {
                    //Instantiate(_ImpactEffect, transform.position)
                }
                Destroy(gameObject);
            }
        }
    }

    // Colliding against Non-kinematic objects (ie. walls)
    public void OnTriggerEnter(Collider other) {
        
        // Play impact effect
        if (_ImpactEffect) {
            //Instantiate(_ImpactEffect, transform.position)
        }
        Destroy(gameObject);
    }

    public void SetOwner(TankController owner) { _Owner = owner; }

    public void SetDamage(int damage) { _Damage = damage; }
}