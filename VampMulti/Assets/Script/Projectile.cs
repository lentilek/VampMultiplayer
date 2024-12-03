using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Projectile : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [SerializeField] private float speed = 20;
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
}
