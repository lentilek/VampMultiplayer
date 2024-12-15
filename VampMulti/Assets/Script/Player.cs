using Fusion.Sockets;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    [HideInInspector] public int points;
    public float speed = 10;
    public int projectileNumber = 1;
    [SerializeField] private Projectile _prefabProjectile;
    private Vector3 _forward = Vector3.forward;
    [Networked] private TickTimer delay { get; set; }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        points = 0;
        GetComponent<NetworkCharacterController>().maxSpeed = speed;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;
            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0) && projectileNumber > 0)
                {
                    projectileNumber--;
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabProjectile,
                    transform.position + _forward, Quaternion.LookRotation(_forward),
                    Object.InputAuthority, (runner, o) =>
                    {
                        o.GetComponent<Projectile>().Init();
                    });
                }
            }
        }
    }

    public void SpeedUp(float speedUp, float time)
    {
        StopAllCoroutines();
        GetComponent<NetworkCharacterController>().maxSpeed = speedUp;
        StartCoroutine(Speed(time));
    }

    IEnumerator Speed(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<NetworkCharacterController>().maxSpeed = speed;
    }
}
