using Unity.Entities;
using Unity.Transforms; // 위치/회전 관련 컴포넌트
using Unity.Burst;
using Unity.Mathematics;

// [BurstCompile]: 시스템도 당연히 Burst를 써야 빠릅니다.
[BurstCompile]
public partial struct RotateSystem : ISystem
{
    // Start()와 비슷 (초기화)
    public void OnCreate(ref SystemState state) { }

    // OnDestroy()와 비슷
    public void OnDestroy(ref SystemState state) { }

    // Update()와 비슷 (매 프레임 실행)
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // ★ ECS의 마법: 쿼리(Query)
        // "LocalTransform(위치/회전)과 RotateSpeed(속도)를 둘 다 가진 놈들 다 나와!"
        // RefRW: 읽고 쓰기 (Read Write)
        // RefRO: 읽기 전용 (Read Only)
        foreach (var (transform, speed) in 
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>())
        {
            // 수학 연산 (Y축 기준으로 회전)
            transform.ValueRW = transform.ValueRW.RotateY(speed.ValueRO.Value * deltaTime);
        }
    }
}