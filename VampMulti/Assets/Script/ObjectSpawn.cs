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
    private List<Transform> spawnPointsToUse = new List<Transform>();
    private List<Transform> spawnPointsUsed = new List<Transform>();

    [SerializeField] private ProjectilePickUp _projectilePrefab;
    [SerializeField] private float timeBetweenProjectiles;
    [SerializeField] public Transform[] _projectileSpawn;
    private bool spawnProjectile;
    private List<Transform> spawnProjectilesToUse = new List<Transform>();
    private List<Transform> spawnProjectilesUsed = new List<Transform>();

    [SerializeField] private SpeedUp _speedUpPrefab;
    [SerializeField] private float timeBetweenSpeedUp;
    [SerializeField] public Transform[] _speedUpSpawn;
    private bool spawnSpeedUp;
    private List<Transform> spawnSpeedUpToUse = new List<Transform>();
    private List<Transform> spawnSpeedUPUsed = new List<Transform>();

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
        foreach(Transform t in _pointsSpawn)
        {
            spawnPointsToUse.Add(t);
        }
        foreach (Transform t in _projectileSpawn)
        {
            spawnProjectilesToUse.Add(t);
        }
        foreach (Transform t in _speedUpSpawn)
        {
            spawnSpeedUpToUse.Add(t);
        }
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
                int a = RandomNumber(spawnPointsToUse.Count);
                _forward = spawnPointsToUse[a].position + Vector3.forward;
                spawnPointsUsed.Add(spawnPointsToUse[a]);
                spawnPointsToUse.RemoveAt(a);
                StartCoroutine(PointsSpawnReturn());

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
                int b = RandomNumber(spawnProjectilesToUse.Count);
                _forward = spawnProjectilesToUse[b].position + Vector3.forward;
                spawnProjectilesUsed.Add(spawnProjectilesToUse[b]);
                spawnProjectilesToUse.RemoveAt(b);
                StartCoroutine(ProjectileSpawnReturn());

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
                int c = RandomNumber(spawnSpeedUpToUse.Count);
                _forward = spawnSpeedUpToUse[c].position + Vector3.forward;
                spawnSpeedUPUsed.Add(spawnSpeedUpToUse[c]);
                spawnSpeedUpToUse.RemoveAt(c);
                StartCoroutine(SpeedUpSpawnReturn());

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
    IEnumerator PointsSpawnReturn()
    {
        yield return new WaitForSeconds(_pointsPrefab.lifeTime);
        spawnPointsToUse.Add(spawnPointsUsed[0]);
        spawnPointsUsed.RemoveAt(0);
    }
    IEnumerator ProjectileSpawn()
    {
        yield return new WaitForSeconds(timeBetweenProjectiles);
        spawnProjectile = true;
    }
    IEnumerator ProjectileSpawnReturn()
    {
        yield return new WaitForSeconds(_projectilePrefab.lifeTime);
        spawnProjectilesToUse.Add(spawnProjectilesUsed[0]);
        spawnProjectilesUsed.RemoveAt(0);
    }
    IEnumerator SpeedUpSpawn()
    {
        yield return new WaitForSeconds(timeBetweenSpeedUp);
        spawnSpeedUp = true;
    }
    IEnumerator SpeedUpSpawnReturn()
    {
        yield return new WaitForSeconds(_speedUpPrefab.lifeTime);
        spawnSpeedUpToUse.Add(spawnSpeedUPUsed[0]);
        spawnSpeedUPUsed.RemoveAt(0);
    }
}
