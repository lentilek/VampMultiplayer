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
    public static Spawner Instance;

    private NetworkRunner _runner;
    [SerializeField] private GameObject hub;
    [SerializeField] public GameObject hubBig;
    [SerializeField] private GameObject objectSpawner;
    [SerializeField] private GameObject startButton;
    [SerializeField] public GameObject[] playerUI;
    [SerializeField] public TextMeshProUGUI[] playerUIPoints;
    [SerializeField] public GameObject[] playerAvatars;
    [HideInInspector] public List<TextMeshProUGUI> playersInGamePointsUI = new List<TextMeshProUGUI>();
    [HideInInspector] public List<int> points = new List<int>();
    private Player playerHost;
    private bool isGameEnded;
    private void Awake()
    {
        isGameEnded = false;
        playersInGamePointsUI.Clear();
        hub.SetActive(true);
        startButton.SetActive(false);
        objectSpawner.SetActive(false);
        points.Clear();
    }
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        
        gameObject.AddComponent<RunnerSimulatePhysics3D>();
        RunnerSimulatePhysics3D cps = GetComponent<RunnerSimulatePhysics3D>();
        Debug.Log(cps.ToString());
        cps.ClientPhysicsSimulation = ClientPhysicsSimulation.Disabled;
        Debug.Log(cps.ToString());
        //_runner.SetIsSimulated(Object, false);

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
        //hubBig.SetActive(false);
        StartGame(GameMode.Client);
    }
    public void StartGame()
    {
        foreach (var player in _spawnedCharacters.Values)
        {
            player.GetComponent<Player>().StartGame(1f);
        }
        playerHost.RPC_HUB(false, points.ToArray());
        objectSpawner.SetActive(true);
        startButton.SetActive(false);
    }
    [SerializeField] private NetworkPrefabRef[] _playerPrefab;
    [HideInInspector] public Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3(0, 0, 0);
            if (player.RawEncoded % runner.Config.Simulation.PlayerCount == 2)
            {
                startButton.SetActive(true);
                spawnPosition = new Vector3(-8, 1f, 3);
                playerUI[0].SetActive(true);
                playerAvatars[0].SetActive(true);
                playersInGamePointsUI.Add(playerUIPoints[0]);
                NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab[0], spawnPosition, Quaternion.identity, player);
                _spawnedCharacters.Add(player, networkPlayerObject);
                playerHost = networkPlayerObject.GetComponent<Player>();
                points.Add(networkPlayerObject.GetComponent<Player>().points);
            }
            else if (player.RawEncoded % runner.Config.Simulation.PlayerCount == 3)
            {
                spawnPosition = new Vector3(8, 1f, 3);
                playerUI[1].SetActive(true);
                playerAvatars[1].SetActive(true);
                playersInGamePointsUI.Add(playerUIPoints[1]);
                NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab[1], spawnPosition, Quaternion.identity, player);
                _spawnedCharacters.Add(player, networkPlayerObject);
                points.Add(networkPlayerObject.GetComponent<Player>().points);
            }
            else if (player.RawEncoded % runner.Config.Simulation.PlayerCount == 4)
            {
                spawnPosition = new Vector3(-8, 1, -4);
                playerUI[2].SetActive(true);
                playerAvatars[2].SetActive(true);
                playersInGamePointsUI.Add(playerUIPoints[2]);
                NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab[2], spawnPosition, Quaternion.identity, player);
                _spawnedCharacters.Add(player, networkPlayerObject);
                points.Add(networkPlayerObject.GetComponent<Player>().points);
            }
            else if (player.RawEncoded % runner.Config.Simulation.PlayerCount == 5)
            {
                spawnPosition = new Vector3(8, 1, -4);
                playerUI[3].SetActive(true);
                playerAvatars[3].SetActive(true);
                playersInGamePointsUI.Add(playerUIPoints[3]);
                NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab[3], spawnPosition, Quaternion.identity, player);
                _spawnedCharacters.Add(player, networkPlayerObject);
                points.Add(networkPlayerObject.GetComponent<Player>().points);
            }
            // Create a unique position for the player
            // Keep track of the player avatars for easy access
            //Debug.Log(player.RawEncoded % runner.Config.Simulation.PlayerCount);
            PointsUpdate();
            playerHost.RPC_HUB(true, points.ToArray());
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
        if(playerHost != null) PointsUpdate();
        if (playerHost != null && GeneralUI.Instance.endGame && !isGameEnded)
        {
            EndGame();
            playerHost.RPC_FinishGame(points.ToArray());
        }
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
            points[i] = player.GetComponent<Player>().points;
            playerUI[i].SetActive(true);
            playersInGamePointsUI[i].text = $"Points: {points[i]}";
            i++;
        }
        playerHost.RPC_PointsUpdate(points.ToArray());
    }
    public void EndGame()
    {
        isGameEnded = true;
        int i = 0;
        //int pointsBest = -1;
        //List<Player> winners= new List<Player>();
        foreach (var player in _spawnedCharacters.Values)
        {
            playerUI[i].SetActive(false);
            playerUI[i + 4].SetActive(true);
            playerUIPoints[i + 4].text = $"Points: {player.GetComponent<Player>().points}";
            /*if(player.GetComponent<Player>().points == pointsBest)
            {
                winners.Add(player.GetComponent<Player>());
            }else if(player.GetComponent<Player>().points > pointsBest || i == 0)
            {
                winners.Clear();
                winners.Add(player.GetComponent<Player>());
                pointsBest = player.GetComponent<Player>().points;
            }*/
            i++;
        }
        /*foreach (Player player in winners)
        {
            player.Won();
        }*/
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
