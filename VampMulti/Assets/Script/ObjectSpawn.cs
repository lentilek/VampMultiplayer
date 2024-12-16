using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawn : NetworkBehaviour
{
    public static ObjectSpawn Instance;
    [Networked] private TickTimer delay { get; set; }

    [SerializeField] private Points _pointsPrefab;
    [SerializeField] private float timeBetweenPoints;
    [SerializeField] public Transform[] _pointsSpawn;
    private bool spawnPoints;

    [SerializeField] private ProjectilePickUp _projectilePrefab;
    [SerializeField] private float timeBetweenProjectiles;
    [SerializeField] public Transform[] _projectileSpawn;
    private bool spawnProjectile;

    [SerializeField] private SpeedUp _speedUpPrefab;
    [SerializeField] private float timeBetweenSpeedUp;
    [SerializeField] public Transform[] _speedUpSpawn;
    private bool spawnSpeedUp;

    private Vector3 _forward = Vector3.forward;
    public int RandomNumber(int max)
    {
        int value = Random.Range(0, max);
        return value;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }
    private void Start()
    {
        spawnPoints = false;
        spawnProjectile = false;
        spawnSpeedUp = false;
        StartCoroutine(PointsSpawn());
        StartCoroutine(ProjectileSpawn());
        StartCoroutine(SpeedUpSpawn());
    }
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
        {
            if (spawnPoints)
            {
                //_forward = data.spawnPosition1;
                _forward = _pointsSpawn[RandomNumber(_pointsSpawn.Length)].position + Vector3.forward;
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(_pointsPrefab, _forward, Quaternion.identity,
                Object.InputAuthority, (runner, o) =>
                {
                    o.GetComponent<Points>().Init();
                });
                spawnPoints = false;
                StartCoroutine(PointsSpawn());
            }
            if (spawnProjectile)
            {
                //_forward = data.spawnPosition2;
                _forward = _projectileSpawn[RandomNumber(_projectileSpawn.Length)].position + Vector3.forward;

                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(_projectilePrefab, _forward, Quaternion.identity,
                Object.InputAuthority, (runner, o) =>
                {
                    o.GetComponent<ProjectilePickUp>().Init();
                });
                spawnProjectile = false;
                StartCoroutine(ProjectileSpawn());
            }
            if (spawnSpeedUp)
            {
                //_forward = data.spawnPosition3;
                _forward = _speedUpSpawn[RandomNumber(_speedUpSpawn.Length)].position + Vector3.forward;

                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(_speedUpPrefab, _forward, Quaternion.identity,
                Object.InputAuthority, (runner, o) =>
                {
                    o.GetComponent<SpeedUp>().Init();
                });
                spawnSpeedUp = false;
                StartCoroutine(SpeedUpSpawn());
            }
        }
    }
    IEnumerator PointsSpawn()
    {
        yield return new WaitForSeconds(timeBetweenPoints);
        spawnPoints = true;
    }
    IEnumerator ProjectileSpawn()
    {
        yield return new WaitForSeconds(timeBetweenProjectiles);
        spawnProjectile = true;
    }
    IEnumerator SpeedUpSpawn()
    {
        yield return new WaitForSeconds(timeBetweenSpeedUp);
        spawnSpeedUp = true;
    }
}
