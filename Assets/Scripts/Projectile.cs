using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    // Created by Daniel Marton

    [Header("Movement")]
    public float _MovementSpeed = 5f;
    public bool _Reflects = false;
    public int _BounceCount = 5;
    public bool _ReflectsOffShields = true;

    [Header("Impact")]
    public ParticleSystem _ImpactEffect;
    public bool _ExplosiveProjectile = true;
    public float _ExplosionSphereRadius = 4f;
    public float _ExplosionDamage = 10f;

    [HideInInspector]
    public bool _Enabled = false;

    private TankController _Owner;
    private float _Damage = 20f;
    private float _DistanceTravelled = 0f;
    private float _MaxDistance = 10f;
    private bool _Moving = false;
    private Vector3 _Velocity;

    public void Start() {
        
        _Velocity = transform.forward;
    }

    void Update() {

        if (_Enabled) {

            // Constantly move forward
            transform.position += _Velocity * _MovementSpeed * Time.deltaTime;
      
            _Moving = true;

            // Destroy gameObject when it has reached max distance threshold
            if (_DistanceTravelled < _MaxDistance) _DistanceTravelled += Time.deltaTime;
            else { Destroy(gameObject); }
        }
    }

    // Colliding with colliders objects
    public void OnCollisionEnter(Collision collision) {

        // Check for collision with a shield
        if (collision.gameObject) {

            if (collision.gameObject.tag == "Shield" && _ReflectsOffShields) {

                // Reflect velocity based off contact point
                ContactPoint contact = collision.contacts[0];
                _Velocity = Vector3.Reflect(_Velocity, contact.normal);
            }

            else if (collision.gameObject.tag == "Shield") {
                
                // Play impact effect
                if (_ImpactEffect) { Instantiate(_ImpactEffect, transform).Play(); }

                if (_ExplosiveProjectile) {
                    
                    Bounds exp = new Bounds(transform.position, new Vector3(_ExplosionSphereRadius, _ExplosionSphereRadius));

                    if (collision.gameObject.tag == "Destroyable") {

                        if (exp.Intersects(collision.gameObject.GetComponent<Collider>().GetComponent<Bounds>())) {

                            // If not our owner that the projectile is colliding against
                            TankController tank = collision.gameObject.GetComponent<TankController>();

                            // Not colliding with owner & the tank has no active shield currently
                            if (tank != _Owner && !tank.GetShieldActive()) {
                                
                                // Get reference to the damagable component
                                Health obj = collision.gameObject.GetComponent<Health>();

                                obj.Damage(_Damage);
                            }
                        }
                    }
                }
                Destroy(gameObject);
            }
        }

        // Check for valid damagable object
        if (collision.gameObject.tag == "Destroyable" && _Reflects && _BounceCount > 0) {

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

        // Check for collision against walls
        if (collision.gameObject.tag == "Wall") {

            // Bounces
            if (_Reflects && _BounceCount > 0) {

                // Reflect velocity based off contact point
                ContactPoint contact = collision.contacts[0];                
                _Velocity = Vector3.Reflect(_Velocity, contact.normal);

                _BounceCount -= 1;
            }

            // Doesnt bounce
            else {

                // Play impact effect
                if (_ImpactEffect) { Instantiate(_ImpactEffect, collision.transform).Play(); }
                /*
                if (_ExplosiveProjectile) {

                    Bounds exp = new Bounds(transform.position, new Vector3(_ExplosionSphereRadius, _ExplosionSphereRadius));

                    if (collision.gameObject.tag == "Destroyable") {

                        if (exp.Intersects(collision.gameObject.GetComponent<Collider>().GetComponent<Bounds>())) {

                            // If not our owner that the projectile is colliding against
                            TankController tank3 = collision.gameObject.GetComponent<TankController>();

                            // Not colliding with owner & the tank has no active shield currently
                            if (tank3 != _Owner && !tank3.GetShieldActive()) {

                                // Get reference to the damagable component
                                Health obj3 = collision.gameObject.GetComponent<Health>();
                                obj3.Damage(_Damage);
                            }
                        }
                    }
                }*/
                Destroy(gameObject);
            }
        }
    }

    public void SetOwner(TankController owner) { _Owner = owner; }

    public void SetDamage(float damage) { _Damage = damage; }

    public bool GetMoving() { return _Moving; }
}