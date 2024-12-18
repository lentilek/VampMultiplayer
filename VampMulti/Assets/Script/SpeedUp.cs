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
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().SpeedUp(speedUp, buffTime);
        }
        isActive = false;
    }
}
