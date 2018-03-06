using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

	// Created by Daniel Marton

    public enum EPickupType { Shield, SpeedBoost, Minigun, LaserBeam, Count_DONT_USE_EVEN_IF_THERES_A_FIRE }
    public EPickupType _PickupType;

    private void OnTriggerEnter(Collider other) {
        
        if (other.gameObject.tag == "Destroyable") {

            TankController tank = other.gameObject.GetComponent<TankController>();
            if (tank) {

                // Apply pickup modifier based on type
                switch (_PickupType) {

                    case EPickupType.Shield: {

                        // Shield
                        tank.SetHasShield(true);
                        break;
                    }

                    case EPickupType.SpeedBoost: {

                        // Speed boost
                        tank.SetSpeedModifier(MatchManager._pInstance._SpeedBoostModifier);
                        break;
                    }

                    case EPickupType.Minigun: {

                        // Minigun
                        tank.SetFiremode(TankController.EFiremode.FullAuto);
                        break;
                    }

                    case EPickupType.LaserBeam: {

                        // Laser beam
                        tank.SetFiremode(TankController.EFiremode.Charge);
                        break;
                    }

                    default: break;
                }
            }
        }
    }
}
