using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            ExecuteTransition(collision);
    }
    private void ExecuteTransition(Collider2D collision)
    {
        collision.transform.position = EntityManager.Instance.GetPlayerSpawnPosition();
        EntityManager.Instance.SetPortalTrigger();
        Destroy(gameObject);
    }
}
