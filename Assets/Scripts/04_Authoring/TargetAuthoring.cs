using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

public class TargetAuthoring : MonoBehaviour
{
    public PhysicsCategoryTags TargetCategory;
    class Baker : Baker<TargetAuthoring>
    {
        public override void Bake(TargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); // 큐브는 움직이니까 Dynamic
            
            // 이 스크립트가 붙은 GameObject가 엔티티로 변할 때,
            // 'Target'이라는 빈 데이터 주머니를 하나 더 차고 태어나게 만듭니다.
            AddComponent(entity, new Target
            {
                TargetEntity = Entity.Null, // 초기값: 없음
                Distance = float.MaxValue,   // 초기값: 아주 멀음
                TargetLayerMask = authoring.TargetCategory.Value,
                NextSearchTime = 0
            });
        }
    }
}