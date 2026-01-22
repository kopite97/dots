using UnityEngine;
using Unity.Entities;

public class UnitStatusAuthoring : MonoBehaviour
{
    [Header("Stats")]
    public float MaxHealth = 100f;
    public float AttackRange = 10.0f;
    public float MoveSpeed = 5.0f;
    public float AttackSpeed = 1.0f; // 공격 간격

    [Header("Shooting")] 
    public GameObject BulletPrefab;
    public float BulletSpeed = 10f;
    
    class Baker : Baker<UnitStatusAuthoring>
    {
        public override void Bake(UnitStatusAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new Health
            {
                Current = authoring.MaxHealth,
                Max = authoring.MaxHealth,
            });
            
            AddComponent(entity,new UnitState
            {
                CurrentState = UnitStateType.Idle,
                AttackRange = authoring.AttackRange, 
                MoveSpeed = authoring.MoveSpeed
            });
            
            AddComponent(entity,new AttackTimer
            {
                CurrentTime = 0,
                MaxTime = authoring.AttackSpeed
            });

            AddComponent(entity, new ShootingData
            {
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                BulletSpeed = authoring.BulletSpeed
            });
            
        }
    }
    
}