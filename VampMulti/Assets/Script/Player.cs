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
    //[SerializeField] private GameObject boostIcon;
    private Vector3 _forward = Vector3.forward;
    [Networked] private TickTimer delay { get; set; }

    [HideInInspector] public bool isPlaying;

    private void Awake()
    {
        //boostIcon.SetActive(false);
        isPlaying = false;
        _cc = GetComponent<NetworkCharacterController>();
        points = 0;
        _cc.maxSpeed = speed;
    }
    private void Update()
    {
        if(Timer.Instance.currentTime <= 0 && isPlaying)
        {
            Endgame();
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data) && isPlaying)
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
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
        _cc.maxSpeed = speedUp;
        //boostIcon.SetActive(true);
        StartCoroutine(Speed(time));
    }

    IEnumerator Speed(float time)
    {
        yield return new WaitForSeconds(time);
        _cc.maxSpeed = speed;
        //boostIcon.SetActive(false);
    }
    public void StartGame(float time)
    {
        StartCoroutine(WaitForGame(time));
    }
    IEnumerator WaitForGame(float time)
    {
        yield return new WaitForSeconds(time);
        isPlaying = true;
        Timer.Instance.StartTiming();
    }
    public void Endgame()
    {
        isPlaying = false;
        Time.timeScale = 0f;
        if(ObjectSpawn.Instance!= null)
        {
            ObjectSpawn.Instance.gameObject.SetActive(false);
        }
    }
}
