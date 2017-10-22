using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars : Singleton<GlobalVars> {

    protected GlobalVars() { } // guarantee this will be always a singleton only - can't use the constructor!

    // Player
    public float playerHeight = 1.5f;
    public float maxAngle = 45f;
    public float maxSteeringSpeed = 1f;
    public float speed = 3f;
    // Health points
    public int hp = 1;
    // Plancton
    public float planctonDensity = 0.5f;
    // Eating
    public float micThreshold = 0.5f;
    public bool voiceCheat = false;

    // Game Controller
    public float completeDifficultyCompletionTime = 60f * 10;
    public float easyEatingTime = 30f;
    public float hardEatingTime = 5f;
    public float easyFightingTime = 20f;
    public float hardFightingTime = 10f;
    public float timingRandomnessFactor = 0.2f;
}
