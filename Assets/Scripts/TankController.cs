using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class TankController : MonoBehaviour {

    // Created by Daniel Marton

    // Gamepad
    public PlayerIndex _PlayerIndex;
    private GamePadState _GamepadState;
    private GamePadState _PreviousGamepadState;
    private bool _ControllerIsRumbling = false;
    private float _TimerRumble = 0f;
    private float _RumbleTime = 1f;
    private float _MotorLeft = 0f;
    private float _MotorRight = 0f;

    // Components
    private GameObject _Cannon;
    private CharacterController _CharacterController;
    private Health _Health;
    public GameObject _ShellPrefab;
    public GameObject _BulletPrefab;
    public GameObject _MuzzleLaunchPoint;
    public Transform _SpawnPoint;

    // Movement controller
    private float _BaseLookX;
    private Vector3 _CannonLook = Vector3.zero;
    private float _CurrentSpeed = 0f;
    private float _BaseRotationSpeed = 20f;
    private float _CannonRotationSpeed = 10f;
    private float _Acceleration = 5f;
    private float _Decceleration = 0.5f;

    // Weapon controller
    public enum EFiremode { SemiAuto, FullAuto, Charge, Count }
    private EFiremode _Firemode = EFiremode.SemiAuto;
    private bool _CanFire;
    private bool _IsFiring = false;
    private bool _CanTryToFire = true;
    private float _FireDelay = 0f;
    private float _FiringDelaySemiAuto = 0.5f;
    private float _FiringDelayFullAuto = 0.1f;
    private float _FiringDelayCharge = 1f;
    private float _ChargeAmount = 0f;
    private float _ChargeTime = 2f;
    private bool _IsChargeFiring = false;
    private float _ChargeFiringAmount = 0f;
    private float _ChargeFiringTime = 2f;
    private int _MagazineSize = 1;
    private bool _BeenDestroyed = false;

    // Stats
    [HideInInspector]
    public int TimeAlive = 0;
    [HideInInspector]
    public int ShotsFired = 0;
    [HideInInspector]
    public int LivesRemaining = 1;

    private void Start () {

        // Get component references
        _Cannon = GameObject.FindGameObjectWithTag("TankCannon" + _PlayerIndex.ToString());
        _CharacterController = GetComponent<CharacterController>();
        _Health = GetComponent<Health>();

        // Set starting health
        _Health._Health = MatchManager._pInstance._TankStartingHealth;
        LivesRemaining = MatchManager._pInstance._PlayerLives;
    }

    private void FixedUpdate() {

        if (_ControllerIsRumbling) {

            // Start controller rumble
            GamePad.SetVibration(_PlayerIndex, _MotorLeft, _MotorRight);
        }

        else {

            // Stop controller rumble
            GamePad.SetVibration(_PlayerIndex, 0f, 0f);
            _TimerRumble = 0f;
        }
    }

    private void Update () {
        
        // Update defaults
        UpdateDefaults();

        // Update gamepad states
        _PreviousGamepadState = _GamepadState;
        _GamepadState = GamePad.GetState(_PlayerIndex);

        // If alive
        if (_Health.CheckAlive() && !MatchManager._pInstance.GetGamePaused() && !MatchManager._pInstance.GetGameOver()) {

            // Update cannon rotation
             _CannonLook = new Vector3((Mathf.Atan2(_GamepadState.ThumbSticks.Right.X, _GamepadState.ThumbSticks.Right.Y) * 180 / Mathf.PI), 0, 0);
            if (_Cannon) _Cannon.transform.Rotate(_CannonLook.normalized * _CannonRotationSpeed);
            else { _Cannon = GameObject.FindGameObjectWithTag("TankCannon" + _PlayerIndex.ToString()); }

            // Update base rotation
            _BaseLookX = _GamepadState.ThumbSticks.Left.X * _BaseRotationSpeed;
            if (_BaseLookX != 0) { transform.eulerAngles += new Vector3(0f, _BaseLookX, 0f); }

            // Update base movement
            if (_GamepadState.ThumbSticks.Left.Y > 0.01f) {

                // Forward
                _CurrentSpeed += _Acceleration;

                // Clamp max speed
                if (_CurrentSpeed > MatchManager._pInstance._MaxMovementSpeed) { _CurrentSpeed = MatchManager._pInstance._MaxMovementSpeed; }
            }
            else if (_GamepadState.ThumbSticks.Left.Y < -0.01f) {

                // Backward
                _CurrentSpeed -= _Decceleration;

                // Clamp max reverse speed
                if (_CurrentSpeed < MatchManager._pInstance._MaxReverseMovementSpeed) { _CurrentSpeed = MatchManager._pInstance._MaxReverseMovementSpeed; }
            }

            else {

                // Slowly decelerate to a stop
                if      (_CurrentSpeed >  0.01) { _CurrentSpeed -= _Decceleration / 50; }
                else if (_CurrentSpeed < -0.01) { _CurrentSpeed += _Decceleration / 50; }

                else { _CurrentSpeed = 0f; }
            }

            // Update controller movement
            _CharacterController.Move(transform.forward * _CurrentSpeed * Time.deltaTime);

            // Lock Y axis
            transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
            
            // Check for action input
            CheckFire();
            ChangeFireMode(); /// Debug

            // Update time alive
            TimeAlive = MatchManager._pInstance._GameTime;
        }
        else /* (!_Health.CheckAlive()) */ { KillTank(); }

        // Update firing delay
        if (_FireDelay > 0) { _FireDelay -= Time.deltaTime; }

        // Update controller rumble timer
        if (_ControllerIsRumbling && _Firemode != EFiremode.FullAuto) {

            _TimerRumble += Time.deltaTime;

            // Stop rumble once its reached timer threshold
            if (_TimerRumble >= _RumbleTime) { ControllerRumble(false, 0f, 0f); }
        }
    }

    private void UpdateDefaults() {

        // Update stats
        _Acceleration = MatchManager._pInstance._TankAcceleration;
        _Decceleration = MatchManager._pInstance._TankDecceleration;
        _BaseRotationSpeed = MatchManager._pInstance._TankBaseRotationSpeed;
        _CannonRotationSpeed = MatchManager._pInstance._CannonRotationSpeed;
        _FiringDelaySemiAuto = MatchManager._pInstance._FiringDelaySemiAutoShell;
        _FiringDelayFullAuto = MatchManager._pInstance._FiringDelayFullAutoBullet;
        _FiringDelayCharge = MatchManager._pInstance._FiringDelayLaserCharge;
        _ChargeTime = MatchManager._pInstance._LaserChargeUpTime;
        _ChargeFiringTime = MatchManager._pInstance._LaserFiringTime;
    }

    private void ControllerRumble(bool rumble, float leftMotor, float rightMotor) {

        _ControllerIsRumbling = rumble;
        _MotorLeft = leftMotor;
        _MotorRight = rightMotor;
    }

    private void CheckFire() {

        _CanFire = _FireDelay <= 0f && _CanTryToFire;

        // On trigger enter
        if (_GamepadState.Triggers.Right > 0f && _CanFire) {

            switch (_Firemode) {

                // Semi automatic firing mode
                case EFiremode.SemiAuto: {

                    if (_MagazineSize > 0) {

                        _IsFiring = true;
                        _FireDelay = _FiringDelaySemiAuto;
                        _CanTryToFire = false;
                        Fire();
                    }
                    break;
                }

                // Fully automatic firing mode
                case EFiremode.FullAuto: {

                    if (_MagazineSize > 0) {

                        _IsFiring = true;
                        _FireDelay = _FiringDelayFullAuto;
                        Fire();
                    }

                    // Revert back to single shot mode on empty clip
                    else {

                        _Firemode = EFiremode.SemiAuto;
                        _MagazineSize = 1;
                    }
                    break;
                }

                // Charge up firing mode
                case EFiremode.Charge: {

                    if (!_IsChargeFiring) {
                        
                        // Charge up
                        _ChargeAmount += Time.deltaTime;

                        // Controller rumble
                        ControllerRumble(true, 0.1f, 0.1f);
                        _RumbleTime = 0.1f;
                    }
                    break;
                }

                default: break;
            }
        }

        // On trigger release
        else if (_GamepadState.Triggers.Right < 0.1f) {

            _CanTryToFire = true;
            _IsFiring = false;

            // On charge up firing mode
            if (_Firemode == EFiremode.Charge && _ChargeAmount >= _ChargeTime && _CanFire) {
                
                Fire();
            }

            else if (_Firemode == EFiremode.FullAuto) {

                // Stop controller rumble
                GamePad.SetVibration(_PlayerIndex, 0f, 0f);
            }

            // Reset charge up
            _ChargeAmount = 0f;
        }

        LaserBeamFire();
    }

    private void Fire() {
        
        ShotsFired += 1;
        float rumbleLeft = 0f;
        float rumbleRight = 0f;
                        
        GameObject obj;        
        switch (_Firemode) {
            
            // Standard cannon shell
            case EFiremode.SemiAuto: {

                // Instantiate shell prefab facing forward based on cannon rotation
                if (_ShellPrefab && _MuzzleLaunchPoint && _Cannon) {

                    obj = Instantiate(_ShellPrefab, _MuzzleLaunchPoint.transform.position, _MuzzleLaunchPoint.transform.rotation);

                    // Set projectile properties
                    Projectile proj;
                    proj = obj.GetComponent<Projectile>();
                    proj.SetOwner(this);
                    proj.SetDamage(MatchManager._pInstance._DamageFullAutoBullet);
                    proj._Enabled = true;

                    _MagazineSize = 1;

                    // Set controller rumble properties
                    rumbleLeft = 0.7f; rumbleRight = 0.7f;
                    _RumbleTime = 0.5f;
                }
                break;
            }
            
            // Minigun
            case EFiremode.FullAuto: {

                // Instantiate bullet prefab facing forward based on cannon rotation
                if (_BulletPrefab && _MuzzleLaunchPoint && _Cannon) {

                    obj = Instantiate(_BulletPrefab, _MuzzleLaunchPoint.transform.position, _MuzzleLaunchPoint.transform.rotation);

                    // Set projectile properties
                    Projectile proj;
                    proj = obj.GetComponent<Projectile>();                
                    proj.SetOwner(this);
                    proj.SetDamage(MatchManager._pInstance._DamageFullAutoBullet);
                    proj._Enabled = true;

                    _MagazineSize -= 1;

                    // Set controller rumble properties
                    rumbleLeft = 0.35f; rumbleRight = 0.35f;
                    _RumbleTime = 0.25f;
                }
                break;
            }
            
            // Laser beam via raycast
            case EFiremode.Charge: {
                
                // Raycast is fired in 'LaserBeamFire()'
                _IsChargeFiring = true;
                break;
            }

            default: break;
        }

        // Start controller rumble
        ControllerRumble(true, rumbleLeft, rumbleRight);
    }

    private void LaserBeamFire() {

        if (_Firemode == EFiremode.Charge) {

            // Can fire & has completed charge up & hasnt finished the charge firing
            _IsChargeFiring = _CanFire && _ChargeAmount > _ChargeTime && _ChargeFiringAmount < _ChargeFiringTime;
            if (_IsChargeFiring) {

                // Fire raycast to simulate laser beam collisions
                float raycastDistance = 1000f;
                RaycastHit raycastHit;
                Physics.Raycast(_MuzzleLaunchPoint.transform.position, _Cannon.transform.forward, out raycastHit, raycastDistance);

                // Debug raycast
                if (raycastHit.transform) { Debug.DrawLine(_MuzzleLaunchPoint.transform.position, raycastHit.transform.position, Color.green); }
                else { Debug.DrawLine(_MuzzleLaunchPoint.transform.position, _Cannon.transform.forward * raycastDistance, Color.red); }

                // On successful hit with a damageable object
                if (raycastHit.collider) {

                    if (raycastHit.collider.gameObject.CompareTag("Destroyable")) {

                        // Apply damage to component
                        GameObject obj = raycastHit.collider.gameObject;
                        obj.GetComponent<Health>().Damage(MatchManager._pInstance._DamageLaserBeam);
                    }
                }

                // Add to charge fire timer
                _ChargeFiringAmount += Time.deltaTime;

                // Controller rumble
                ControllerRumble(true, 1f, 1f);
                _RumbleTime = 0.1f;
            }
            else if (_ChargeFiringAmount >= _ChargeFiringTime) {

                ControllerRumble(false, 0f, 0f);
                _ChargeFiringAmount = 0f;
                _ChargeAmount = 0f;
                _Firemode = EFiremode.SemiAuto;
                _MagazineSize = 1;
            }
        }
    }

    private void ChangeFireMode() {

        // On button press
        if (_PreviousGamepadState.Buttons.Y == ButtonState.Released && _GamepadState.Buttons.Y == ButtonState.Pressed) {

            // Change firemode
            _Firemode += 1;
            if (_Firemode == EFiremode.Count) { _Firemode = 0; _MagazineSize = 1; }

            if (_Firemode == EFiremode.FullAuto) { _MagazineSize = 20; }
        }
    }

    private void KillTank() {

        if (!_BeenDestroyed) {

            // Remove life from life count
            LivesRemaining -= 1;

            // Need to create the next tank prior to destroying the current one
            GameObject newTank = Instantiate(gameObject);

            // Destroy mesh of the old tank
            Destroy(_Cannon.gameObject);

            // Update stats
            newTank.GetComponent<TankController>().LivesRemaining = LivesRemaining;
            newTank.GetComponent<TankController>()._ShellPrefab = GameObject.FindGameObjectWithTag("Shell");
            newTank.GetComponent<TankController>()._BulletPrefab = GameObject.FindGameObjectWithTag("Bullet");

            // Move tank to spawn point
            newTank.transform.position = _SpawnPoint.position;
            newTank.transform.rotation = _SpawnPoint.rotation;

            _BeenDestroyed = true;
        }
    }

}
