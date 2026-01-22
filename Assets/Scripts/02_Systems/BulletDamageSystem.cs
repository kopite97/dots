using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public partial struct BulletDamageSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>(); // 물리 시뮬레이션 필수
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 1. 물리 세계 가져오기
        var simSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        
        // 2. 전화번호부 챙기기 (체력부, 총알부)
        var healthLookup = SystemAPI.GetComponentLookup<Health>(isReadOnly: false);
        var bulletTagLookup = SystemAPI.GetComponentLookup<BulletTag>(isReadOnly: true);

        var bulletDamageLookup = SystemAPI.GetComponentLookup<BulletDamage>(isReadOnly: true);
        
        
        // 3. 충돌 작업(Job) 예약
        state.Dependency = new BulletCollisionJob
        {
            HealthLookup = healthLookup,
            BulletTagLookup = bulletTagLookup,
            BulletDamageLookup = bulletDamageLookup,
            ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
        }.Schedule(simSingleton, state.Dependency);

    }
}

[BurstCompile]
struct BulletCollisionJob : ICollisionEventsJob
{
    public ComponentLookup<Health> HealthLookup;
    [ReadOnly] public ComponentLookup<BulletTag> BulletTagLookup;
    [ReadOnly] public ComponentLookup<BulletDamage> BulletDamageLookup;
    
    public EntityCommandBuffer ECB; // 엔티티 삭제용


    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA =  collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        // 누가 총알인지 모르니 둘다 확인
        CheckBulletHit(entityA, entityB);
        CheckBulletHit(entityB, entityA);
    }

    private void CheckBulletHit(Entity bullet, Entity target)
    {
        // bullet이 진짜 총알인지 확인
        if (BulletTagLookup.HasComponent(bullet))
        {
            // target이 체력이 있는지 확인 (즉, 유닛인지)
            if (HealthLookup.HasComponent(target))
            {
                // 데미지 계산
                float damage = 0;
                // 총알에 데미지 정보가 들어있다면 그 값을 사용
                if (BulletDamageLookup.HasComponent(bullet))
                {
                    damage = BulletDamageLookup[bullet].Value;
                }
                else
                {
                    damage = 20;
                }
                
                // 체력 깎기
                var hp = HealthLookup[target];
                hp.Current -= damage;
                HealthLookup[target] = hp;
                
                // 총알은 이제 쓸모 없으니 삭제
                ECB.DestroyEntity(bullet);
            }
            
        }
        
    }
}