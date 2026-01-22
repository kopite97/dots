using Unity.Collections;
using UnityEngine;
using Unity.Entities;

public class UnitStatusAuthoring : MonoBehaviour
{
    [Header("Shooting")] 
    public GameObject BulletPrefab;

    [Header("Base Stats")]
    public UnitBaseStatsSO BaseStats;
    
    class Baker : Baker<UnitStatusAuthoring>
    {
        public override void Bake(UnitStatusAuthoring authoring)
        {
            if (authoring.BaseStats == null) return;
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            //====================================================
            // Blob Asset 생성 과정 (공식)
            //====================================================

            // 1. 빌더 생성
            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            
            // 2. 루트 생성
            ref UnitBaseStats stats = ref blobBuilder.ConstructRoot<UnitBaseStats>();
            
            // 3 . 데이터 값 복사
            stats.MaxHealth = authoring.BaseStats.MaxHealth;
            stats.MoveSpeed = authoring.BaseStats.MoveSpeed;
            stats.AttackRange = authoring.BaseStats.AttackRange;
            stats.Attackinterval = authoring.BaseStats.AttackInterval;
            stats.BulletSpeed = authoring.BaseStats.BulletSpeed;
            
            // 4. 실제로 Blob Asset을 만들고, 그 참조(주소)를 가져옴
            var statsRef = blobBuilder.CreateBlobAssetReference<UnitBaseStats>(Allocator.Persistent);
            
            // 5. 엔티티에 컴포넌트로 추가
            AddComponent(entity,new UnitData{Stats = statsRef});
            
            // ======================================================
            
            AddComponent(entity,new Health
            {
                Current = authoring.BaseStats.MaxHealth,
                Max = authoring.BaseStats.MaxHealth
            });
            
            AddComponent(entity,new UnitState
            {
                CurrentState = UnitStateType.Idle,
                AttackRange = authoring.BaseStats.AttackRange,
                MoveSpeed = authoring.BaseStats.MoveSpeed
            });
            
            AddComponent(entity,new AttackTimer
            {
                CurrentTime = 0,
                MaxTime = authoring.BaseStats.AttackInterval
            });

            AddComponent(entity, new ShootingData
            {
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                BulletSpeed = authoring.BaseStats.BulletSpeed
            });

            DynamicBuffer<ItemElement> buffer = AddBuffer<ItemElement>(entity);
            buffer.Add(new ItemElement
            {
                Type = ItemType.HealthPotion,
                Amount = 3
            });
            
            AddComponent(entity, new UnitTag());

        }
    }
    
}