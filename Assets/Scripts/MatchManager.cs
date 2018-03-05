using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour {

    [Header("Health")]
    public int _PlayerLives = 3;
    public int _TankStartingHealth = 100;

    [Header("Movement")]
    public float _TankAcceleration = 1f;
    public float _TankDecceleration = 2.5f;
    public float _TankBaseRotationSpeed = 4f;
    public float _CannonRotationSpeed = 5f;
    public float _MaxMovementSpeed = 10f;
    public float _MaxReverseMovementSpeed = -5f;

    [Header("Weapon")]
    public int _DamageSemiAutoShell = 15;
    public int _DamageFullAutoBullet = 5;
    public int _DamageLaserBeam = 1;
    public float _FiringDelaySemiAutoShell = 0.5f;
    public float _FiringDelayFullAutoBullet = 0.1f;
    public float _FiringDelayLaserCharge = 2f;
    public float _LaserChargeUpTime = 2f;
    public float _LaserFiringTime = 3f;

    [Header("Match")]
    public List<TankController> _AliveTanks;
    public List<TankController> _EliminatedTanks;
    private bool _GameOver = false;
    private bool _GamePaused = false;

    [HideInInspector]
    public int _GameTime = 0;
    [HideInInspector]
    public static MatchManager _pInstance;


    private void Awake() {

        // If the singleton has already been initialized
        if (_pInstance != null && _pInstance != this) {

            Destroy(this.gameObject);
            return;
        }

        // Set singleton
        _pInstance = this;
    }

    private void Start() {

        _EliminatedTanks = new List<TankController>();
    }

    private void Update () {

        // Update game timer
        _GameTime += (int)Time.deltaTime;

        // Check for eliminated tanks
        if (_AliveTanks.Count > 1) {

            foreach (var tank in _AliveTanks) {

                // valid memory address
                if (tank) {

                    // Tank has run out of lives
                    if (tank.LivesRemaining == 0) {

                        // Remove from alive array
                        _AliveTanks.Remove(tank);

                        // Send to eliminated array
                        _EliminatedTanks.Add(tank);
                    }
                }
            }
        }

        // Check for game over
        if (_AliveTanks.Count <= 1) {

            // Game over
            _GameOver = true;
        }
	}
    
    public bool GetGameOver() { return _GameOver; }

    public bool GetGamePaused() { return _GamePaused; }
}
