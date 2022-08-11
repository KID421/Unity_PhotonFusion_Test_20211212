using UnityEngine;
using Fusion;

public class BulletPhysics : NetworkBehaviour
{
    [Header("子彈存活時間"), Range(0, 10)]
    public float lifeTime = 3.5f;

    [Networked]
    private TickTimer life { get; set; }

    public void Init(Vector3 forward)
    {
        life = TickTimer.CreateFromSeconds(Runner, lifeTime);
        GetComponent<Rigidbody>().velocity = forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
