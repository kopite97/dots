using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections; // Allocator를 위해 필요

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // 1. EntityCommandBuffer(ECB) 준비
        // "SystemState"를 통해 기본 제공되는 ECB를 가져옵니다.
        // BeginSimulationEntityCommandBufferSystem은 시뮬레이션 시작 지점에 명령을 실행해주는 시스템입니다.
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // 2. 쿼리 순회
        foreach (var (spawner, entity) in SystemAPI.Query<RefRO<Spawner>>().WithEntityAccess())
        {
            // --- 엔티티 생성 (Instantiate) ---
            // EntityManager.Instantiate 대신 ecb.Instantiate를 씁니다. (예약)
            NativeArray<Entity> instances = new NativeArray<Entity>(spawner.ValueRO.Count, Allocator.Temp);
            ecb.Instantiate(spawner.ValueRO.Prefab, instances);

            // --- 위치 설정 ---
            // *주의: ECB로 생성한 엔티티(instances)는 아직 진짜가 아닙니다.
            // 하지만 위치 설정 같은 작업은 나중에 처리되도록 예약할 수 있습니다.
            
            // 랜덤 위치 로직 (약간 복잡하지만 성능을 위해 이렇게 씁니다)
            var random = Unity.Mathematics.Random.CreateFromIndex(1234);
            for (int i = 0; i < instances.Length; i++)
            {
                var position = random.NextFloat3(new float3(-50, 1, -50), new float3(50, 50, 50));
                
                // "이 엔티티가 태어나면, LocalTransform 값을 이걸로 설정해줘" 라고 예약
                ecb.SetComponent(instances[i], LocalTransform.FromPosition(position));
            }
            
            instances.Dispose(); // 사용한 임시 배열 정리

            // --- 3. 컴포넌트 제거 (RemoveComponent) ---
            // "나중에 이 엔티티에서 Spawner 컴포넌트를 떼어내줘" 라고 예약
            // 여기서 바로 state.EntityManager.RemoveComponent를 호출하면 에러가 납니다!
            ecb.RemoveComponent<Spawner>(entity);
        }
    }
}