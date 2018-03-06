using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPoint : MonoBehaviour
{
    // Created By Liam Gates
    public float HalfX = 20f;
    public float HalfY = 15f;
    public float HalfZ = 15f;

    public Bounds FocusBounds;

    private void Update()
    {
        //Vector3 position = gameObject.transform.position;
        //Bounds bounds = new Bounds();
        //bounds.Encapsulate(new Vector3(position.x - HalfX, position.y - HalfY, position.z - HalfZ));
        //bounds.Encapsulate(new Vector3(position.x + HalfX, position.y + HalfY, position.z + HalfZ));
        //FocusBounds = bounds;
    }

}
