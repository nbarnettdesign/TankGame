using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    // Created by Daniel Marton

    public float _MovementSpeed = 5f;
    public ParticleSystem _ImpactEffect;

    [HideInInspector]
    public bool _Enabled = false;

    private TankController _Owner;
    private int _Damage = 20;
    private float _DistanceTravelled = 0f;
    private float _MaxDistance = 10f;
    private bool _Moving = false;
    private Quaternion _Rotation;

    public void Start() {

        _Rotation = transform.rotation;
    }

    void Update() {

        if (_Enabled) {

            // Constantly move forward
            transform.rotation = _Rotation;
            transform.position += transform.forward * _MovementSpeed * Time.deltaTime;
            _Moving = true;

            // Destroy gameObject when it has reached max distance threshold
            if (_DistanceTravelled < _MaxDistance) _DistanceTravelled += Time.deltaTime;
            else { Destroy(gameObject); }
        }
    }

    // Colliding with kinematic objects
    public void OnCollisionEnter(Collision collision) {
        
        // Check for valid damagable object
        if (collision.gameObject.tag == "Destroyable") {

            // If not our owner that the projectile is colliding against
            TankController tank = collision.gameObject.GetComponent<TankController>();
            
            // Not colliding with owner & the tank has no active shield currently
            if (tank != _Owner && !tank.GetShieldActive()) {

                // Get reference to the damagable component
                Health obj = collision.gameObject.GetComponent<Health>();

                obj.Damage(_Damage);

                // Play impact effect
                if (_ImpactEffect) { Instantiate(_ImpactEffect, collision.transform).Play(); }
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

    public bool GetMoving() { return _Moving; }

    public void SetRotation(Quaternion rotation) { _Rotation = rotation; }
}