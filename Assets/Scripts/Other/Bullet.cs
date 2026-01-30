using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        Normal,
        DelayedDestroy
    };

    public BulletType type;
    [HideInInspector] public float damageAmount;

    private bool isTimerStarted;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Beer"))
        {
            print("Trafiono w butelkê!");
            other.gameObject.GetComponent<Bottle>().Shatter();
        }
    }
    private void OnCollisionEnter(Collision objectWeHit)
    {
        NPCStats enemy = objectWeHit.gameObject.GetComponent<NPCStats>();
        if (enemy == null)
        {
            enemy = objectWeHit.gameObject.GetComponentInParent<NPCStats>();
        }

        if (enemy != null)
        {
            enemy.TakeDamage(damageAmount);
            Destroy(gameObject);
            return;
        }

        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            print("Trafiono w " + objectWeHit.gameObject.name + " !");   
            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            print("Trafiono w œcianê!");
            CreateBulletImpactEffect(objectWeHit);
            
            if(type == BulletType.Normal)
            {
                Destroy(gameObject);
            }
            else if(type == BulletType.DelayedDestroy)
            {
                if(!isTimerStarted)
                {
                    isTimerStarted = true;

                    Rigidbody rb = GetComponent<Rigidbody>();

                    if(rb != null)
                    {
                        rb.isKinematic = true;
                        rb.linearVelocity = Vector3.zero;
                    }

                    Destroy(gameObject, 5f);
                }
            }
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            ); 

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
