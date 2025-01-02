using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePickUp : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [SerializeField] public float lifeTime = 5f;
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
    }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<SphereCollider>().enabled = false;
        if (other.tag == "Player")
        {
            //AudioManager.Instance.PlaySound("pickUp");
            other.GetComponent<Player>().projectileNumber++;
        }
        isActive = false;
    }
}
