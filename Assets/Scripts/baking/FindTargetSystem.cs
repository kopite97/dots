using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public partial struct FindTargetSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        
        var job = new FindTargetJob
        {
            CollisionWorld = physicsWorld.CollisionWorld,
        };
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
partial struct FindTargetJob : IJobEntity
{
    [ReadOnly] public CollisionWorld CollisionWorld;

    public void Execute(Entity entity, ref LocalTransform transform, RefRW<Target> target)
    {
        float3 myPosition = transform.Position;

        var input = new PointDistanceInput
        {
            Position = myPosition,
            MaxDistance = 500.0f, // 넉넉하게
            // ★ 핵심 수정: 아무거나(Default) 찾는 게 아니라, 'Category 1'만 찾습니다!
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,       // 나는 모든 그룹에 속한 것으로 처리 (쿼리용)
                CollidesWith = 1u << 1, // ★ "Category 1" (두 번째 비트)만 감지해라!
                GroupIndex = 0
            }
        };

        if (CollisionWorld.CalculateDistance(input, out DistanceHit hit))
        {
            // 이제 필터 덕분에 '나(Category 0)'는 아예 감지가 안 됩니다.
            // 즉, 여기서 잡힌 녀석은 무조건 '적'입니다.
            target.ValueRW.TargetEntity = hit.Entity;
            target.ValueRW.Distance = hit.Distance;
        }
    }
}