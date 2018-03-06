using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Created by Daniel Marton

    public int ID;
    [HideInInspector]
    public int Lives;

    private void Start() {

        Lives = MatchManager._pInstance._PlayerLives;
    }
}
