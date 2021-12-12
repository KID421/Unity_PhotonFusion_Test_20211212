using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 連線基底
/// </summary>
public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    #region Fusion 事件
    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W)) data.direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) data.direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) data.direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) data.direction += Vector3.right;
        data.inputMouseLeft = Input.GetKey(KeyCode.Mouse0);

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Vector3 positionSpawn = new Vector3(player.RawEncoded % runner.Config.Simulation.DefaultPlayers * 3, 1, 0);
        NetworkObject networkPlayerObject = runner.Spawn(prefabPlayer, positionSpawn, Quaternion.identity, player);
        players.Add(player, networkPlayerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (players.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            players.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
    #endregion

    [SerializeField]
    private NetworkPrefabRef prefabPlayer;

    private Dictionary<PlayerRef, NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>();
    private NetworkRunner runner;

    /// <summary>
    /// 開始房間並包含模式
    /// </summary>
    /// <param name="">開啟房間、加入房間</param>
    public void StartGameWithMode(string mode)
    {
        if (mode == "開啟房間") StartGame(GameMode.Host);
        else if (mode == "加入房間") StartGame(GameMode.Client);
    }

    private async void StartGame(GameMode mode)
    {
        print("開始遊戲");

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "測試房間",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
}
