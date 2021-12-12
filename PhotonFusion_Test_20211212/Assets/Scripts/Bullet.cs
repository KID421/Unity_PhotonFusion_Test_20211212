using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    [Header("���ʳt��"), Range(0, 50)]
    public float speed = 7;
    [Header("�ͩR�g��"), Range(0, 10)]
    public float lifeTime = 5;

    [Networked]
    private TickTimer life { get; set; }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner)) Runner.Despawn(Object);

        else transform.position += speed * transform.forward * Runner.DeltaTime;
    }
}
