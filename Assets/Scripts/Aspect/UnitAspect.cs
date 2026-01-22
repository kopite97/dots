using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// readonly partial struct로 선언하고 IAspect를 상속받습니다.
public readonly partial struct UnitAspect : IAspect
{
    public readonly Entity Self;
    
    readonly RefRW<LocalTransform> _transform;
    readonly RefRW<UnitState> _state;
    readonly RefRO<Target> _target;         // ★ 타겟 정보 읽기
    readonly RefRW<AttackTimer> _timer;     // ★ 타이머 관리
    readonly RefRW<Health> _health;

    // 프로퍼티
    public UnitStateType CurrentState => _state.ValueRO.CurrentState;
    public bool HasTarget => _target.ValueRO.TargetEntity != Entity.Null;
    public float TargetDistance => _target.ValueRO.Distance;
    public float AttackRange => _state.ValueRO.AttackRange;
    public bool IsAttackReady => _timer.ValueRO.CurrentTime <= 0;
    public float3 Position => _transform.ValueRO.Position;
    public bool IsDead => _health.ValueRO.Current <= 0;
    public Entity TargetEntity => _target.ValueRO.TargetEntity;
    
    // 상태 변경
    public void ChangeState(UnitStateType state)
    {
        _state.ValueRW.CurrentState = state;
    }
    
    // 이동(목표 방향으로)
    public void MoveToTarget(float deltaTime)
    {
        if (!HasTarget) return;
        
        // 타겟 방향의 계산은 여기서 하지 않고 , FindTargetSystem이 구해준 Distance등을 활용하거나
        // 간단하게 LookAt처럼 방향만 잡고 전진
        // *주의: 물리 바디(PhysicsVelocity)를 쓴다면 Velocity를 수정해야 하지만, 
        // 일단 쉬운 이해를 위해 Transform을 직접 밉니다. (벽 통과 가능성 있음)
        float moveAmount = _state.ValueRO.MoveSpeed * deltaTime;
        _transform.ValueRW.Position += _transform.ValueRO.Forward() * moveAmount;
    }
    
    // 회전 (타겟 바라보기)
    public void LookAtTarget(float3 targetPos)
    {
        float3 dir = targetPos - Position;
        dir.y = 0; // 위아래로는 고개 안 숙임
        if (math.lengthsq(dir) > 0.001f)
        {
            _transform.ValueRW.Rotation = quaternion.LookRotation(math.normalize(dir), math.up());
            
        }
    }
    
    // 타이머 갱신
    public void UpdateTime(float deltaTime)
    {
        if (_timer.ValueRO.CurrentTime > 0)
        {
            _timer.ValueRW.CurrentTime -= deltaTime;
        }
    }
    
    
    
    // 공격 후 쿨타임 리셋 
    public void ResetAttackTimer()
    {
        _timer.ValueRW.CurrentTime = _timer.ValueRW.MaxTime;
    }
    
    
    public void TakeDamage(float damage)
    {
        // 이미 죽었으면 무시
        if (IsDead) return;

        // 체력 감소
        _health.ValueRW.Current -= damage;

        // 체력이 0 미만이면 0으로 고정
        if (_health.ValueRO.Current < 0)
        {
            _health.ValueRW.Current = 0;
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        _health.ValueRW.Current += amount;
        if (_health.ValueRO.Current > _health.ValueRO.Max)
        {
            _health.ValueRW.Current = _health.ValueRW.Max;
        }
    }
}