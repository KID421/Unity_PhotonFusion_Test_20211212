using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [Header("移動速度"), Range(0, 100)]
    public float speed = 7;
    [Header("開槍延遲時間"), Range(0, 1)]
    public float delayTime = 0.5f;
    [Header("物理子彈發射速度"), Range(0, 1000)]
    public float speedBulletPhysics = 100;

    [SerializeField]
    private Bullet prefabBullet;
    [SerializeField]
    private BulletPhysics prefabBulletPhysics;

    [Networked]
    private TickTimer delay { get; set; }
    [Networked(OnChanged = nameof(OnBulletSpawned))]
    public NetworkBool spawned { get; set; }

    private Material _material;
    public Material material
    {
        get
        {
            if (_material == null) _material = GetComponentInChildren<MeshRenderer>().material;
            return _material;
        }
    }

    private Vector3 forward;
    private NetworkCharacterController ncc;
    private Text textMessage;

    public string namePlayer;
    public Text textNamePlayer;

    private void Awake()
    {
        ncc = GetComponent<NetworkCharacterController>();
        forward = transform.forward;
        textMessage = GameObject.Find("聊天室").GetComponent<Text>();
    }

    private void Start()
    {
        textNamePlayer.text = namePlayer;
    }

    private void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("YOYOYOYO!!!!");
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        if (info.Source == Runner.Simulation.LocalPlayer) message = $"You said : {message}\n";
        else message = $"Some other player said: {message}\n";

        textMessage.text += message;
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
                    spawned = !spawned;
                }
                else if (data.inputMouseRight)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, delayTime);
                    Runner.Spawn(
                        prefabBulletPhysics,
                        transform.position + forward,
                        Quaternion.LookRotation(forward),
                        Object.InputAuthority,
                        (runner, o) =>
                        {
                            o.GetComponent<BulletPhysics>().Init(forward * speedBulletPhysics);
                        });
                    spawned = !spawned;
                }
            }
        }
    }

    public override void Render()
    {
        material.color = Color.Lerp(material.color, Color.blue, Time.deltaTime);
    }

    public static void OnBulletSpawned(Changed<Player> changed)
    {
        changed.Behaviour.material.color = Color.white;
    }
}
