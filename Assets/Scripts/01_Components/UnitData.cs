using Unity.Entities;

public struct UnitBaseStats
{
    public float MaxHealth;
    public float MoveSpeed;
    public float AttackRange;
    public float Attackinterval;
    public float BulletSpeed;
    public float AttackDamage;
}

public struct UnitData : IComponentData
{
    public BlobAssetReference<UnitBaseStats> Stats;
}