using Unity.Entities;

public struct Spawner : IComponentData
{
    public Entity Prefab; // 생성할 원본 엔티티
    public int Count;     // 생성할 개수 (예: 100,000)
}