using Unity.Entities;

public struct ShootingData : IComponentData
{
    public Entity BulletPrefab;
    public float BulletSpeed;   
}