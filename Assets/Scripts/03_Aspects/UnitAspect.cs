using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// 1. readonly 필수!
public readonly partial struct UnitAspect : IAspect
{
    public readonly Entity Self;
    
    private readonly RefRW<LocalTransform> _transform;
    private readonly RefRW<UnitState> _state;
    private readonly RefRO<Target> _target;
    private readonly RefRW<AttackTimer> _timer;
    private readonly RefRW<Health> _health;
    
    // 2. 필드도 readonly 필수! (내용물 수정은 가능함)
    private readonly DynamicBuffer<ItemElement> _inventory;
    
    private readonly RefRO<UnitData> _unitData;

    // ... (프로퍼티들은 그대로 유지) ...
    public float3 Position => _transform.ValueRO.Position;
    public UnitStateType CurrentState => _state.ValueRO.CurrentState;
    public float AttackRange => _unitData.ValueRO.Stats.Value.AttackRange;
    public float MoveSpeed => _unitData.ValueRO.Stats.Value.MoveSpeed;
    public bool HasTarget => _target.ValueRO.TargetEntity != Entity.Null;
    public float TargetDistance => _target.ValueRO.Distance;
    public Entity TargetEntity => _target.ValueRO.TargetEntity;
    public bool IsDead => _health.ValueRO.Current <= 0;
    public bool IsAttackReady => _timer.ValueRO.CurrentTime <= 0;
    
    // 추가하셨어야 할 프로퍼티들 (시스템에서 쓰려면)
    public float MaxHealth => _unitData.ValueRO.Stats.Value.MaxHealth;
    public float CurrentHealth => _health.ValueRO.Current;
    public float AttackDamage => _unitData.ValueRO.Stats.Value.AttackDamage;

    // ... (기본 동작 함수들 그대로 유지) ...
    public void ChangeState(UnitStateType newState) => _state.ValueRW.CurrentState = newState;
    
    public void MoveToTarget(float deltaTime)
    {
        if (!HasTarget) return;
        float moveAmount = MoveSpeed * deltaTime;
        _transform.ValueRW.Position += _transform.ValueRO.Forward() * moveAmount;
    }

    public void LookAtTarget(float3 targetPos)
    {
        float3 dir = targetPos - Position;
        dir.y = 0;
        if (math.lengthsq(dir) > 0.001f)
            _transform.ValueRW.Rotation = quaternion.LookRotation(math.normalize(dir), math.up());
    }

    public void UpdateTimer(float deltaTime)
    {
        if (_timer.ValueRO.CurrentTime > 0) _timer.ValueRW.CurrentTime -= deltaTime;
    }

    public void ResetAttackTimer()
    {
        _timer.ValueRW.CurrentTime = _timer.ValueRW.MaxTime;
    }

    // 3. 인벤토리 로직 (readonly여도 이 코드는 작동합니다)
    public bool TryConsumePotion()
    {
        var buffer = _inventory;

        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].Type == ItemType.HealthPotion && buffer[i].Amount > 0)
            {
                var item = buffer[i];
                item.Amount--;

                buffer[i] = item;
                if (item.Amount <= 0)
                {
                    buffer.RemoveAt(i);
                }
                Heal(30.0f);
                
                UnityEngine.Debug.Log("물약을 마셨습니다!");
                return true;
            }
        }

        return false;
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        float newHealth = _health.ValueRO.Current + amount;
        float maxHealth = _unitData.ValueRO.Stats.Value.MaxHealth;

        _health.ValueRW.Current = math.min(newHealth, maxHealth);
    }
}