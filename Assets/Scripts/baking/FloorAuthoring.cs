using Unity.Entities;
using UnityEngine;

public struct FloorTag : IComponentData
{
}

public class FloorAuthoring : MonoBehaviour
{
    class Baker : Baker<FloorAuthoring>
    {
        public override void Bake(FloorAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity,new FloorTag());
        }
    }
}