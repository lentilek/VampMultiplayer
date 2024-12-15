using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawn : NetworkBehaviour
{
    [Networked] private TickTimer delay { get; set; }

    [SerializeField] private Points _pointsPrefab;
    [SerializeField] private float timeBetweenPoints;
    [SerializeField] private Transform[] _pointsSpawn;

    [SerializeField] private ProjectilePickUp _projectilePrefab;
    [SerializeField] private float timeBetweenProjectiles;
    [SerializeField] private Transform[] _projectileSpawn;

    [SerializeField] private SpeedUp _speedUpPrefab;
    [SerializeField] private float timeBetweenSpeedUp;
    [SerializeField] private Transform[] _speedUpSpawn;

    private Vector3 _forward = Vector3.forward;

    private void Start()
    {
        StartCoroutine(PointsSpawn());
        StartCoroutine(ProjectileSpawn());
        StartCoroutine(SpeedUpSpawn());
    }
    IEnumerator PointsSpawn()
    {
        yield return new WaitForSeconds(timeBetweenPoints);
        delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
        Runner.Spawn(_pointsPrefab, _pointsSpawn[Random.Range(0, _pointsSpawn.Length)].position + _forward, Quaternion.LookRotation(_forward),
        Object.InputAuthority, (runner, o) =>
        {
            o.GetComponent<Points>().Init();
        });
        StartCoroutine(PointsSpawn());
    }
    IEnumerator ProjectileSpawn()
    {
        yield return new WaitForSeconds(timeBetweenProjectiles);
        delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
        Runner.Spawn(_projectilePrefab, _projectileSpawn[Random.Range(0, _projectileSpawn.Length)].position + _forward, Quaternion.LookRotation(_forward),
        Object.InputAuthority, (runner, o) =>
        {
            o.GetComponent<ProjectilePickUp>().Init();
        });
        StartCoroutine(ProjectileSpawn());
    }
    IEnumerator SpeedUpSpawn()
    {
        yield return new WaitForSeconds(timeBetweenSpeedUp);
        delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
        Runner.Spawn(_speedUpPrefab, _speedUpSpawn[Random.Range(0, _speedUpSpawn.Length)].position + _forward, Quaternion.LookRotation(_forward),
        Object.InputAuthority, (runner, o) =>
        {
            o.GetComponent<SpeedUp>().Init();
        });
        StartCoroutine(SpeedUpSpawn());
    }
}
