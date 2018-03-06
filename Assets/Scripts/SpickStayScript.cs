using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpickStayScript : MonoBehaviour
{
    public float SpickDamage = 0.1f;
    public ParticleSystem _ImpactEffect;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Destroyable")
        {
            other.gameObject.GetComponent<Health>().Damage(SpickDamage);
            if(_ImpactEffect)
            {
                Instantiate(_ImpactEffect, other.transform).Play();
            }
        }
    }
}
