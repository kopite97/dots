using Unity.Entities;
using UnityEngine;

public struct BulletTag : IComponentData
{
}

public struct LifeTime : IComponentData
{
    public float Value;
}

public class BulletAuthoring : MonoBehaviour
{
    class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BulletTag());
            AddComponent(entity, new LifeTime { Value = 3.0f });
            
        }
    }
}