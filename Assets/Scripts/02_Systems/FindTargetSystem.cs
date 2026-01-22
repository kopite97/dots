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
        double currentTime = SystemAPI.Time.ElapsedTime;
        
        // 랜덤 생성을 위한 시드값 (프레임마다 다르게)
        uint seed = (uint)SystemAPI.Time.ElapsedTime * 1000 + 1;
        var random = Unity.Mathematics.Random.CreateFromIndex(seed);
        
        
        var job = new FindTargetJob
        {
            CollisionWorld = physicsWorld.CollisionWorld,
            CurrentTime = currentTime,
            RandomGen = random
        };
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
partial struct FindTargetJob : IJobEntity
{
    [ReadOnly] public CollisionWorld CollisionWorld;
    public double CurrentTime;
    public Unity.Mathematics.Random RandomGen;

    public void Execute(Entity entity, ref LocalTransform transform, RefRW<Target> target)
    {
        // 쿨타임 체크
        if (CurrentTime < target.ValueRO.NextSearchTime && target.ValueRO.TargetEntity != Entity.Null)
        {
            // 타겟과의 거리만 갱신 (가벼운 연산)
            if(target.ValueRO.Distance < float.MaxValue)
            {
                // 거리 갱신 로직을 추가할 수도 있지만,
                // 일단 검색 자체를 스킵해서 성능 아끼기
                return;
            }
        }
        
        float3 myPosition = transform.Position;
        uint serachMask = target.ValueRO.TargetLayerMask;

        var input = new PointDistanceInput
        {
            Position = myPosition,
            MaxDistance = 500.0f, // 넉넉하게
            // ★ 핵심 수정: 아무거나(Default) 찾는 게 아니라, 'Category 1'만 찾습니다!
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,       // 나는 모든 그룹에 속한 것으로 처리 (쿼리용)
                CollidesWith = serachMask, // ★ "Category 1" (두 번째 비트)만 감지해라!
                GroupIndex = 0
            }
        };

        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
        if (CollisionWorld.CalculateDistance(input,ref hits))
        {
            Entity closesEntity = Entity.Null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.Entity == entity) continue;

                if (hit.Distance < minDistance)
                {
                    minDistance = hit.Distance;
                    closesEntity = hit.Entity;
                }
            }

            target.ValueRW.TargetEntity = closesEntity;
            target.ValueRW.Distance = minDistance;
        }
        else
        {
            target.ValueRW.TargetEntity = Entity.Null;
        }

        hits.Dispose();
        float nextDelay = RandomGen.NextFloat(0.5f, 0.7f);
        target.ValueRW.NextSearchTime = CurrentTime+nextDelay;
    }
}