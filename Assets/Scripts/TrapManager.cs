using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    // Created By Liam Gates
    public int TrapDamage = 50;
    public ParticleSystem _ImpactEffect;

    // Update is called once per frame
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Destroyable")
        {
            other.gameObject.GetComponent<Health>().Damage(TrapDamage);
            if(_ImpactEffect)
            {
                Instantiate(_ImpactEffect, other.transform).Play();
            }
        }
    }
}
