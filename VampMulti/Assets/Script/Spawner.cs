using Fusion.Sockets;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion.Addons.Physics;
using TMPro;
using Unity.VisualScripting;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    [SerializeField] private GameObject hub;
    [SerializeField] private GameObject objectSpawner;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject[] playerUI;
    [SerializeField] private TextMeshProUGUI[] playerUIPoints;
    private List<TextMeshProUGUI> playersInGamePointsUI = new List<TextMeshProUGUI>();
    private void Awake()
    {
        playersInGamePointsUI.Clear();
        hub.SetActive(true);
        startButton.SetActive(false);
        objectSpawner.SetActive(false);
    }
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        gameObject.AddComponent<RunnerSimulatePhysics3D>();


        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    public void StartHost()
    {
        hub.SetActive(false);
        StartGame(GameMode.Host);
    }
    public void StartClient()
    {
        hub.SetActive(false);
        StartGame(GameMode.Client);
    }
    public void StartGame()
    {
        foreach (var player in _spawnedCharacters.Values)
        {
            player.GetComponent<Player>().StartGame(1f);
        }
        objectSpawner.SetActive(true);
        startButton.SetActive(false);
    }
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3(0,0,0);
            if(player.RawEncoded % runner.Config.Simulation.PlayerCount == 2)
            {
                startButton.SetActive(true);            
                spawnPosition = new Vector3(-8, 1f, 3);
                playerUI[0].SetActive(true);
                playersInGamePointsUI.Add(playerUIPoints[0]);
            }else if(player.RawEncoded % runner.Config.Simulation.PlayerCount == 3)
            {
                spawnPosition = new Vector3(8, 1f, 3);
                playerUI[1].SetActive(true);
                playersInGamePointsUI.Add(playerUIPoints[1]);
            }
            else if(player.RawEncoded % runner.Config.Simulation.PlayerCount == 4)
            {
                spawnPosition = new Vector3(-8, 1, -4);
                playerUI[2].SetActive(true);
                playersInGamePointsUI.Add(playerUIPoints[2]);
            }
            else if(player.RawEncoded % runner.Config.Simulation.PlayerCount == 5)
            {
                spawnPosition = new Vector3(8, 1, -4);
                playerUI[3].SetActive(true);
                playersInGamePointsUI.Add(playerUIPoints[3]);
            }
            // Create a unique position for the player
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
            //Debug.Log(player.RawEncoded % runner.Config.Simulation.PlayerCount);
            PointsUpdate();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    private bool _mouseButton0;
    private void Update()
    {
        _mouseButton0 = _mouseButton0 || Input.GetMouseButton(0);
        PointsUpdate();
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, _mouseButton0);
        _mouseButton0 = false;
        /*if (objectSpawner.activeSelf)
        {
            Debug.Log("we");
            data.spawnPosition1 = ObjectSpawn.Instance._pointsSpawn[ObjectSpawn.Instance.RandomNumber(ObjectSpawn.Instance._pointsSpawn.Length)].position;
            data.spawnPosition2 = ObjectSpawn.Instance._projectileSpawn[ObjectSpawn.Instance.RandomNumber(ObjectSpawn.Instance._projectileSpawn.Length)].position;
            data.spawnPosition3 = ObjectSpawn.Instance._speedUpSpawn[ObjectSpawn.Instance.RandomNumber(ObjectSpawn.Instance._speedUpSpawn.Length)].position;
        }*/
         input.Set(data);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void PointsUpdate()
    {
        int i = 0;
        foreach (var player in _spawnedCharacters.Values)
        {
            playerUI[i].SetActive(true);
            playersInGamePointsUI[i].text = $"Points: {player.GetComponent<Player>().points}";
            i++;
        }
    }
}
