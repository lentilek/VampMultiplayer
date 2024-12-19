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

    private bool canShoot;
    private int speedBoost;

    private void Awake()
    {
        //boostIcon.SetActive(false);
        isPlaying = false;
        _cc = GetComponent<NetworkCharacterController>();
        points = 0;
        _cc.maxSpeed = speed;
        canShoot = true;
        speedBoost = 0;
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
            
            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner) && Runner.IsServer) // check if server
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0) && projectileNumber > 0 && canShoot)
                {
                    canShoot = false;
                    StartCoroutine(ShootingNot());
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
        _cc.maxSpeed = speedUp;
        speedBoost++;
        //boostIcon.SetActive(true);
        StartCoroutine(Speed(time));
    }

    IEnumerator Speed(float time)
    {
        yield return new WaitForSeconds(time);
        speedBoost--;
        if(speedBoost == 0)
        {
            _cc.maxSpeed = speed;
        }
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
        //Timer.Instance.StartTiming();
    }
    IEnumerator ShootingNot()
    {
        yield return new WaitForSeconds(1);
        canShoot = true;
    }
    public void Endgame()
    {
        Debug.Log("miau");
        isPlaying = false;
        if(ObjectSpawn.Instance!= null)
        {
            ObjectSpawn.Instance.gameObject.SetActive(false);
        }
    }
    public void MainMenu()
    {
        GameObject go = FindObjectOfType<Spawner>().gameObject;
        Destroy(go);
        SceneManager.LoadScene(0);
    }
    /*public void Won()
    {
        GeneralUI.Instance.Win();
    }*/



    ///////////////////////
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_HUB(bool isHUBUp, int[] points, RpcInfo info = default)
    {
        Debug.Log("a");
        GeneralUI.Instance.hubBig.SetActive(isHUBUp);
        int i = 0;
        foreach (int p in points)
        {
            GeneralUI.Instance.playerUI[i].SetActive(true);
            GeneralUI.Instance.playerAvatars[i].SetActive(true);
            GeneralUI.Instance.playerUIPoints[i].text = $"Points: {p}";
            i++;
        }
        if(!isHUBUp)
        {
            Timer.Instance.StartTiming();
        }
    }
}
