using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour {

    // Created by Daniel Marton

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
    public Transform[] _SpawnTransforms;
    public GameObject _NewTank;


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
        _NewTank.SetActive(false);
    }

    private void Update () {

        // Update game timer
        _GameTime += (int)Time.deltaTime;

        // Check for game over
        if (_AliveTanks.Count <= 1) {

            // Game over
            _GameOver = true;
        }
	}
    
    public bool GetGameOver() { return _GameOver; }

    public bool GetGamePaused() { return _GamePaused; }

    public void Restart() {

        // Destroy any excess tanks
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Destroyable");

        foreach (var item in objs) {

            Destroy(item.gameObject);
        }

        // Make new tanks to play with
        for (int i = 0; i < 4; ++i) {

            _NewTank.SetActive(true);
            GameObject obj = Instantiate(_NewTank);
            TankController tank = obj.GetComponent<TankController>();

            // Set controller index
            tank._PlayerIndex = (XInputDotNetPure.PlayerIndex)i;

            // Set cannon tag (for controller rotation)
            tank.transform.GetChild(1).tag = string.Concat("TankCannon" + tank._PlayerIndex.ToString());

            // Set respawn point
            tank._SpawnPoint = _SpawnTransforms[i];
            _NewTank.SetActive(false);
        }

    }
}
