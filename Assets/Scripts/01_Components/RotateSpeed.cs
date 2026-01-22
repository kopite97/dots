using Unity.Entities;

// IComponentData를 상속받아야 ECS용 데이터가 됩니다.
public struct RotateSpeed : IComponentData
{
    public float Value; // 회전 속도 값
}