using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParallax : MonoBehaviour
{
    private Transform target;
    public Transform[] farBackgorund, middleBackground,closeBackground,ground,nearby;
    private Vector2 lastPos;
    private int mapGroup;
    void Start()
    {
        mapGroup = 0;
        lastPos = transform.position;
        GetPlayerTransform();
        SetMapGroup(EntityManager.Instance.ReturnCurrentWaveIndex());
    }
    void Update()
    {
        Parallax();
        SetMapGroup(EntityManager.Instance.ReturnCurrentWaveIndex());
    }
    private void SetMapGroup(int mapGroup)
    {
        if(this.mapGroup == mapGroup)
            return;
        farBackgorund[this.mapGroup].gameObject.SetActive(false);
        middleBackground[this.mapGroup].gameObject.SetActive(false);
        closeBackground[this.mapGroup].gameObject.SetActive(false);
        ground[this.mapGroup].gameObject.SetActive(false);
        nearby[this.mapGroup].gameObject.SetActive(false);
        this.mapGroup = mapGroup;
        farBackgorund[mapGroup].gameObject.SetActive(true);
        middleBackground[mapGroup].gameObject.SetActive(true);
        closeBackground[mapGroup].gameObject.SetActive(true);
        ground[this.mapGroup].gameObject.SetActive(true);
        nearby[this.mapGroup].gameObject.SetActive(true);
    }
    void Parallax()
    {
        if (target == null) return;
        if (Mathf.Abs(target.position.x) < 18)
        {
            transform.position = new Vector3(target.position.x, transform.position.y, -10f);
            Vector2 amountToMove = new Vector2(transform.position.x - lastPos.x, transform.position.y - lastPos.y);
            if(farBackgorund[mapGroup] != null)
            farBackgorund[mapGroup].position += new Vector3(amountToMove.x*0.9f, amountToMove.y*0.9f, 0f);
            if (middleBackground[mapGroup] != null)
                middleBackground[mapGroup].position += new Vector3(amountToMove.x * 0.7f, amountToMove.y * 0.7f, 0f);
            if (closeBackground[mapGroup] != null)
                closeBackground[mapGroup].position += new Vector3(amountToMove.x * 0.5f, amountToMove.y * 0.5f, 0f);
            lastPos = transform.position;
        }
        else if(target.position.x<-18)
            ResetCamera();
    }
    public void GetPlayerTransform()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); 
    }
    public void ResetCamera()
    {
        transform.position = new Vector3(-18f, 0f, -10f);
        farBackgorund[mapGroup].position = new Vector3(0f, 0f, 10f);
        middleBackground[mapGroup].position = new Vector3(0f, 0f, 10f);
        closeBackground[mapGroup].position = new Vector3(0f, 0f, 10f);
        lastPos = transform.position;
    }
}
