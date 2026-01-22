using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct UnitShootingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 1. ECB 생성 (병렬 처리를 위해 ParallelWriter 사용 가능하지만 일단 기본형으로)
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        // 2. 타겟의 위치를 알아야 하므로 Lookup준비
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);
        
        // 3. 쿼리 : UnitAspect + SHootingData를 지닌 모든 유닛
        foreach (var (unit, shootingData) in SystemAPI.Query<UnitAspect, RefRO<ShootingData>>())
        {
            // 조건 1 : 공격 상태여야 함
            if (unit.CurrentState != UnitStateType.Attack) continue;
            
            // 조건 2 : 공격 쿨타임이 끝났어야 함
            if (!unit.IsAttackReady) continue;
            
            // 조건 3 : 타겟이 유효해야 함
            if (!unit.HasTarget) continue;
            Entity targetEntity = unit.TargetEntity;

            if (transformLookup.HasComponent(targetEntity))
            {
                // A. 발사 위치 및 방향 계산
                float3 myPos = unit.Position;
                float3 targetPos = transformLookup[targetEntity].Position;
                
                // 총구 위치 (몸체에서 살짝 앞/위)
                float3 spawnPos = myPos + math.normalize(targetPos - myPos) * 1.0f;
                spawnPos.y = myPos.y; // 높이 보정

                float3 dir = math.normalize(targetPos - myPos);
                
                // B. 총알 생성 (데이터에 들어있는 '내 전용 총알' 을 발사)
                Entity bullet = ecb.Instantiate(shootingData.ValueRO.BulletPrefab);
                
                // C. 위치 설정
                ecb.SetComponent(bullet, LocalTransform.FromPosition(spawnPos));
                
                // D 속도 설정 (PhysicsVelocity)
                ecb.SetComponent(bullet,new PhysicsVelocity
                {
                    Linear = dir*shootingData.ValueRO.BulletSpeed,
                    Angular = float3.zero
                });
                
                // E. 쐇으니까 쿨타임 리셋
                unit.ResetAttackTimer();
            }
        }


    }
}