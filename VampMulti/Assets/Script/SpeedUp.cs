using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float speedUp;
    [SerializeField] private float buffTime;
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
            other.GetComponent<Player>().SpeedUp(speedUp, buffTime);
        }
        Runner.Despawn(Object);
    }
}
