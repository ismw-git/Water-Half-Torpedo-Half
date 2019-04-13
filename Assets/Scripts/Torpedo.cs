﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public static List<Torpedo> TorpedoList = new List<Torpedo>();
    
    public GameObject detectObject;

    [HideInInspector] public static float minSpeed = 0.5f;
    [HideInInspector] public static float maxSpeed = 3f;

    [HideInInspector] public float torpedoMoveSpeed = 0.5f;


    private float torpedoY;

    private bool disappearFlag = false;

    private bool preCCW;


    // Start is called before the first frame update
    void Start()
    {
        torpedoY = transform.position.y;
        TorpedoList.Add(this);

        float lazerAngle = GameManager.gm.myLazer.getAngle();
        Vector2 lazerVector = new Vector2(Mathf.Cos(lazerAngle * Mathf.Deg2Rad), Mathf.Sin(lazerAngle * Mathf.Deg2Rad));
        
        Vector2 submarinePos = GameManager.gm.mySubmarine.getPos();
        Vector2 torpedoVector = new Vector2(transform.position.x - submarinePos.x, transform.position.y - submarinePos.y);

        preCCW = CCW(lazerVector, torpedoVector);
    }

    // Update is called once per frame
    void Update()
    {
        if(disappearFlag) return;
        torpedoY -= torpedoMoveSpeed;
        transform.position = (new Vector3(transform.position.x, torpedoY, transform.position.z));
        if(torpedoY < -480f) disappear();
    }

    void disappear()
    {
        TorpedoList.Remove(this);
        disappearFlag = true;
        Destroy(gameObject);
    }

    public void checkDetect()
    {
        if(disappearFlag) return;

        float lazerAngle = GameManager.gm.myLazer.getAngle();
        Vector2 lazerVector = new Vector2(Mathf.Cos(lazerAngle * Mathf.Deg2Rad), Mathf.Sin(lazerAngle * Mathf.Deg2Rad));
        
        Vector2 submarinePos = GameManager.gm.mySubmarine.getPos();
        Vector2 torpedoVector = new Vector2(transform.position.x - submarinePos.x, transform.position.y - submarinePos.y);

        bool nowCCW = CCW(lazerVector, torpedoVector);

        if((preCCW != nowCCW) && isSameDir(lazerVector, torpedoVector)) detected();

        preCCW = nowCCW;
    }

    bool isSameDir(Vector2 v1, Vector2 v2)
    {
        //    inner product > 0
        // => cos(theta) > 0
        // => abs(theta) < 90
        return (v1.x * v2.x + v1.y * v2.y) > 0;
    }

    bool CCW(Vector2 v1, Vector2 v2)
    {
        return (v1.x * v2.y - v1.y * v2.x) > 0;
    }

    void detected()
    {
        Detected dt = Instantiate(detectObject, transform.position, Quaternion.identity).GetComponent<Detected>();
        dt.detectedColor = new Color(1f, (maxSpeed - torpedoMoveSpeed) / (maxSpeed - minSpeed), 0f);
    }
}