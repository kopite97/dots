using UnityEngine;
using Unity.Entities;

// 1. Authoring: Inspector 노출용 (MonoBehaviour)
public class RotateSpeedAuthoring : MonoBehaviour
{
    public float value = 3.0f; // 여기서 숫자를 입력받음

    // 2. Baker: 데이터를 ECS 세상으로 변환해주는 셰프
    // Baker<T>를 상속받습니다. T는 위 Authoring 클래스.
    class Baker : Baker<RotateSpeedAuthoring>
    {
        public override void Bake(RotateSpeedAuthoring authoring)
        {
            // (1) Entity 생성 (현재 GameObject를 Entity로 변환)
            // TransformUsageFlags.Dynamic: 이 녀석은 움직일 녀석이다! 라고 알려줌
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            // (2) Component 추가
            // 아까 만든 struct에 authoring의 값을 넣어 줌
            AddComponent(entity, new RotateSpeed
            {
                Value = authoring.value
            });
        }
    }
}