using Unity.Entities;

public enum ItemType
{
    None,
    HealthPotion,
    ManaPotion
}

[InternalBufferCapacity(8)] // 8개 까지는 힙 할당 없이 빠르게 처리 (최적화)
public struct ItemElement : IBufferElementData
{
    public ItemType Type;
    public int Amount;
}