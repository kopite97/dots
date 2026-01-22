using Unity.Entities;

public enum UnitStateType
{
    Idle,
    Move,
    Attack
}

public struct UnitState : IComponentData
{
    public UnitStateType CurrentState;
    public float AttackRange;
    public float MoveSpeed;
}