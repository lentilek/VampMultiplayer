using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Projectile : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [SerializeField] private float speed = 20;
    [SerializeField] private int points = 3;
    private bool isActive;
    private void Awake()
    {
        isActive = true;
    }
    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner) || !isActive)
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += speed * transform.forward * Runner.DeltaTime;
        }
    }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
    }
    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<SphereCollider>().enabled = false;
            Debug.Log("-points");
            collision.gameObject.GetComponent<Player>().points -= points;
            isActive = false;
        }
        else if (!collision.gameObject.CompareTag("Pickable"))
        {
            GetComponent<SphereCollider>().enabled = false;
            isActive = false;
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //AudioManager.Instance.PlaySound("hit");
            GetComponent<SphereCollider>().enabled = false;
            other.GetComponent<Player>().points -= points;
            isActive = false;
        }
        else if (!other.CompareTag("Pickable") && !other.CompareTag("Point") && !other.CompareTag("Bullet"))
        {
            GetComponent<SphereCollider>().enabled = false;
            isActive = false;
        }
    }
}
