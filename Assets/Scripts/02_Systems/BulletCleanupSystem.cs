using Unity.Entities;

public partial struct BulletCleanupSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // ECB 가져오기 (구조 변경을 위해)
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // 시간을 흐르게 하고, 수명이 0보다 작으면 파괴
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (lifeTime, entity) in SystemAPI.Query<RefRW<LifeTime>>().WithEntityAccess())
        {
            lifeTime.ValueRW.Value -= dt;

            if (lifeTime.ValueRW.Value <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }
    }
}