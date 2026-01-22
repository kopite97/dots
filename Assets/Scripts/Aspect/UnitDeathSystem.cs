using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct UnitDeathSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        
        // Aspect
        // 쿼리에서 'UnitAspect'를 가져온다.
        // 컴포넌트 3~4개를 나열할 필요 없이 'unit'하나면 끝
        foreach (var (unit, entity) in SystemAPI.Query<UnitAspect>().WithEntityAccess())
        {
            if (unit.IsDead)
            {
                // 죽었으면 삭제
                ecb.DestroyEntity(entity);
                // (확인용 로그)
                UnityEngine.Debug.Log($"유닛 사망! ID: {entity.Index}");
            }
        }
    }
}