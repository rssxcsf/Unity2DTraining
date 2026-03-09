using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceThorn : MonoBehaviour
{
    [SerializeField] private Barrage Thorn;
    [SerializeField] private float liveTime;

    private Animator animator;
    private float timer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(SpawnBarrage());
    }
    private void Update()
    {
        Tick();
    }
    private void Tick()
    {
        timer += Time.deltaTime;
        if (timer > liveTime)
            EndSkill();
    }
    IEnumerator SpawnBarrage()
    {
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position+ new Vector3(0f,1f,0f), transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position+ new Vector3(0f, -1f, 0f), transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position+new Vector3(0f, 1f, 0f), transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position + new Vector3(0f, -1f, 0f), transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position + new Vector3(0f, 1f, 0f), transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position + new Vector3(0f, -1f, 0f), transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position + new Vector3(0f, 1f, 0f), transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Thorn, transform.position + new Vector3(0f, -1f, 0f), transform.rotation);
    }
    private void EndSkill()
    {
        animator.SetTrigger("Destroy");
        Destroy(gameObject,0.2f);
    }
}
