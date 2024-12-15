using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawn : NetworkBehaviour
{
    [Networked] private TickTimer delay { get; set; }
    [SerializeField] private Points _pointsPrefab;
    [SerializeField] private float timeBetweenPoints = 2f;
    [SerializeField] private Transform[] _pointsSpawn;
    [SerializeField] private float timeBetweenProjectiles = 5f;
    private Vector3 _forward = Vector3.forward;

    private void Start()
    {
        StartCoroutine(PointsSpawn());
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
}
