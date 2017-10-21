using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars : Singleton<GlobalVars> {

    protected GlobalVars() { } // guarantee this will be always a singleton only - can't use the constructor!

    // Player
    public float maxAngle = 45f;
    public float maxSteeringForce = 1f;
    public float speedForce = 5000f;
    // Health points
    public int hp = 1;
    // Plancton
    public float planctonDensity = 0.5f;

}
