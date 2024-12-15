using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Projectile : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [SerializeField] private float speed = 20;
    [SerializeField] private int points = 3;
    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().points -= points;
        }
        if (!collision.gameObject.CompareTag("Pickable"))
        {
            Runner.Despawn(Object);
        }
    }
}
