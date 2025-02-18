﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    // Created by Daniel Marton

    // Health
    public enum EState { Okay, Damaged, Destroyed }
    public float _Health = 100;
    public MeshFilter _Mesh;
    public Mesh _DestroyedMesh;
    
    private EState _State = EState.Okay;
    private bool _IsAlive = true;
    private Mesh _OkayMesh;

    void Start () {

        // Get references
        _OkayMesh = _Mesh.mesh;
	}
	
	void Update () {

        // null checks
        if (_Mesh && _OkayMesh && _DestroyedMesh) {

            // Update gameObject mesh based on state
            switch (_State) {

                // Okay
                case EState.Okay: {

                    _Mesh.mesh = _OkayMesh;
                    break;
                }

                // Destroyed
                case EState.Destroyed: {
                    
                    _Mesh.mesh = _DestroyedMesh;
                    break;
                }

                default: break;
            }
        }

        // Set state to destroyed if there isnt any health left
        if (!CheckAlive()) { _State = EState.Destroyed; }
	}

    public bool CheckAlive() { return _IsAlive = _Health > 0; }

    public void Damage(float damage) { _Health -= damage; }

    public float GetHealth() { return _Health; }

}