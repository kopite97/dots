using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public struct PlayerTag : IComponentData
{
}

public class PlayerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public float BulletSpped = 20f;
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerTag());
            
            AddComponent(entity,new ShootingData
            {
                BulletPrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                BulletSpeed = authoring.BulletSpped
            });
        }
    }
    
}