using UnityEngine;
using Unity.Entities;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab; // 큐브 프리팹을 여기에 넣음
    public int count = 1000;  // 일단 1000개만

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new Spawner
            {
                // ★ 핵심: GameObject 프리팹을 -> Entity 프리팹으로 변환해서 ID를 저장함
                Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                Count = authoring.count
            });
        }
    }
}