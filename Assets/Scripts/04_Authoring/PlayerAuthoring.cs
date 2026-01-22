using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public struct PlayerTag : IComponentData
{
}

public class PlayerAuthoring : MonoBehaviour
{

    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerTag());
        }
    }
    
}