using Unity.Entities;

public struct Health : IComponentData
{
    public float Current; // 현재 체력
    public float Max;     // 최대 체력
}