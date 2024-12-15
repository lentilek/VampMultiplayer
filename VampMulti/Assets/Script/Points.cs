using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int points = 2;
    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
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
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().points += points;
        }
        Runner.Despawn(Object);
    }
}
