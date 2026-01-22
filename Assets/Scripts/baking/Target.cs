using Unity.Entities;

public struct Target : IComponentData
{
    public Entity TargetEntity;
    public float Distance;
}