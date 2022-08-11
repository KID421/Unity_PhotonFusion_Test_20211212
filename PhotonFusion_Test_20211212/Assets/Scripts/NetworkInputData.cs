using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
    public bool inputMouseLeft;
    public bool inputMouseRight;
}
