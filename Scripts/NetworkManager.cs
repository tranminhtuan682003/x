using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkRunnerCallbackArgs;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager instance;
    public NetworkRunner networkRunner;
    public NetworkPrefabRef playerPrefab;

    private Vector3 currentDirection = Vector3.zero;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true;
        networkRunner.AddCallbacks(this);
        StartGame();
    }

    private async void StartGame()
    {
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "iam_tuan",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        await networkRunner.StartGame(startArgs);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            runner.Spawn(playerPrefab, new Vector3(25, 0, 2), Quaternion.identity, player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData
        {
            direction = currentDirection
        };

        // Xử lý điều hướng bằng cảm ứng
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 swipeDelta = touch.deltaPosition;
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    if (swipeDelta.x > 0) currentDirection = Vector3.right;
                    else if (swipeDelta.x < 0) currentDirection = Vector3.left;
                }
                else
                {
                    if (swipeDelta.y > 0) currentDirection = Vector3.forward;
                    else if (swipeDelta.y < 0) currentDirection = Vector3.back;
                }
                data.direction = currentDirection;
            }
        }

        // Xử lý điều hướng bằng phím WASD
        if (Input.GetKey(KeyCode.W))
        {
            currentDirection = Vector3.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentDirection = Vector3.back;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            currentDirection = Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentDirection = Vector3.right;
        }
        else
        {
            currentDirection = Vector3.zero;
        }

        data.direction = currentDirection;
        input.Set(data);
    }


    // Các callback khác...

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} left the game.");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.LogWarning($"Input missing from player {player}.");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Server shutdown. Reason: {shutdownReason}");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server.");
    }

    public void OnConnectRequest(NetworkRunner runner, ConnectRequest request, byte[] token)
    {
        Debug.Log("Connection request received.");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"Connection failed with {remoteAddress}. Reason: {reason}");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Session list updated.");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("Custom authentication response received.");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log($"Reliable data received from player {player}.");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load done.");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene load started.");
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"Object {obj} entered player {player}'s AOI.");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"Object {obj} exited player {player}'s AOI.");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("User simulation message received.");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host migration.");
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        Debug.Log($"Reliable data progress from player {player}: {progress * 100}%");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from server. Reason: {reason}");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        Debug.Log($"Reliable data received from player {player} with key {key}.");
    }
}