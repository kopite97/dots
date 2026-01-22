using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
// using UnityEngine.InputSystem; // Input System 쓴다면 주석 해제

public partial class PlayerShootingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // 1. 입력 확인 로그
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Debug.Log("1. 스페이스 바 입력 감지됨!");
        }
        else
        {
            return; // 입력 없으면 리턴
        }

        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                            .CreateCommandBuffer(World.Unmanaged);

        // 2. 플레이어 쿼리 확인
        int playerCount = 0;
        foreach (var (transform, shootingData, target, entity) in 
                 SystemAPI.Query<RefRO<LocalTransform>, RefRO<ShootingData>, RefRO<Target>>()
                 .WithAll<PlayerTag>().WithEntityAccess())
        {
            playerCount++;
            // Debug.Log($"2. 플레이어 발견! (타겟 존재 여부: {target.ValueRO.TargetEntity != Entity.Null})");

            if (target.ValueRO.TargetEntity == Entity.Null) 
            {
                // Debug.Log("--> 타겟이 없어서 발사 취소");
                continue;
            }

            // 3. 총알 프리팹 확인
            if (shootingData.ValueRO.BulletPrefab == Entity.Null)
            {
                // Debug.LogError("--> 핵심 문제: 총알 프리팹이 연결되지 않았음! (Inspector 확인 필요)");
                continue;
            }
            //
            Debug.Log("3. 발사 시도!");
            
            // 발사 로직
            if (SystemAPI.HasComponent<LocalTransform>(target.ValueRO.TargetEntity))
            {
                float3 targetPos = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity).Position;
                float3 myPos = transform.ValueRO.Position;
                float3 diff = targetPos - myPos;
                Debug.Log($"거리 : {targetPos} - {myPos} =  {diff}");
                
                if (math.lengthsq(diff) < 0.001f) 
                {
                    // 너무 가까워서 방향을 알 수 없음 -> 발사 취소
                    continue; 
                }
                float3 dir = math.normalize(diff);

                Entity bullet = ecb.Instantiate(shootingData.ValueRO.BulletPrefab);
                ecb.SetComponent(bullet, LocalTransform.FromPosition(myPos + dir * 1.0f));
                ecb.SetComponent(bullet, new PhysicsVelocity
                {
                    Linear = dir * shootingData.ValueRO.BulletSpeed,
                    Angular = float3.zero
                });
                // Debug.Log("4. 발사 완료!");
            }
        }

        if (playerCount == 0)
        {
            // Debug.LogWarning("경고: PlayerTag를 가진 엔티티를 못 찾았습니다.");
        }
    }
}