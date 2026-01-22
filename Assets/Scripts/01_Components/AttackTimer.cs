using Unity.Entities;

public struct AttackTimer : IComponentData
{
    public float CurrentTime; // 0이 되면 발사
    public float MaxTime;
}