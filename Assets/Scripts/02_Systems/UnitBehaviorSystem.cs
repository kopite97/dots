using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct UnitBehaviorSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // 타겟의 위치를 알기 위해 Lookup 필요 (Move/Attack 때 위치 필요)
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);
        
        // 유닛의 상태에 따라 행동 분기
        foreach (var unit in SystemAPI.Query<UnitAspect>())
        {
            // 0. 공통 : 쿨타임 돌리기
            unit.UpdateTime(deltaTime);
            
            // 1. 타겟 유효성 검사 (타겟이 죽었거나 사라졌으면 Idle)
            if (!unit.HasTarget && unit.CurrentState != UnitStateType.Idle)
            {
                unit.ChangeState(UnitStateType.Idle);
                continue;
            }
            
            // 2. 상태 머신 (FSM)
            switch (unit.CurrentState)
            {
                case UnitStateType.Idle:
                    // 적을 발견했으면 (HasTarget) 추적(Move) 시작
                    if (unit.HasTarget)
                    {
                        unit.ChangeState(UnitStateType.Move);
                    }

                    break;
                case UnitStateType.Move:
                    if (unit.TargetDistance <= unit.AttackRange)
                    {
                        unit.ChangeState(UnitStateType.Attack);
                    }
                    else
                    {
                        // 타겟을 바라보고 이동
                        if (transformLookup.HasComponent(unit.TargetEntity)) // 내부 필드 접근이 안되면 Aspect에 GetTargetPos 함수 추가 필요
                        {
                            // *임시: Aspect 내부 필드가 private이라 여기서 접근이 힘들 수 있습니다.
                            // Aspect에 'TargetEntity' 프로퍼티를 public으로 열어주거나,
                            // 아래처럼 Lookup을 Aspect에 넘겨서 처리하는 게 좋습니다.
                            // (편의상 지금은 Aspect에 'LookAt' 로직이 있으니 위치만 구해서 넘깁니다)
                            var targetPos = transformLookup[unit.TargetEntity].Position;
                            unit.LookAtTarget(targetPos);
                            unit.MoveToTarget(deltaTime);
                        }
                    }
                    break;
                case UnitStateType.Attack:
                    if (unit.TargetDistance > unit.AttackRange)
                    {
                        unit.ChangeState((UnitStateType.Move));
                    }
                    else
                    {
                        // 공격 중이어도 목표를 향하도록
                        if (transformLookup.HasComponent(unit.TargetEntity))
                        {
                            var targetPos = transformLookup[unit.TargetEntity].Position;
                            unit.LookAtTarget(targetPos);
                        }
                    }
                    break;
            }
            
        }
    }
}