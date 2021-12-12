using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    [Header("移動速度"), Range(0, 100)]
    public float speed = 7;
    [Header("開槍延遲時間"), Range(0, 1)]
    public float delayTime = 0.5f;

    [SerializeField]
    private Bullet prefabBullet;

    [Networked]
    private TickTimer delay { get; set; }

    private Vector3 forward;
    private NetworkCharacterController ncc;

    private void Awake()
    {
        ncc = GetComponent<NetworkCharacterController>();
        forward = transform.forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            ncc.Move(speed * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0) forward = data.direction;

            if (delay.ExpiredOrNotRunning(Runner))
            {
                if (data.inputMouseLeft)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, delayTime);
                    Runner.Spawn(
                        prefabBullet,
                        transform.position + forward,
                        Quaternion.LookRotation(forward),
                        Object.InputAuthority,
                        (runner, o) => 
                        {
                            o.GetComponent<Bullet>().Init();
                        });
                }
            }
        }
    }
}
