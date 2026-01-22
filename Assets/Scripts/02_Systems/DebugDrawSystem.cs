// using Unity.Entities;
// using Unity.Transforms;
// using UnityEngine; // Debug.DrawLine을 쓰기 위해 필요
//
// // BurstCompile 안 씀 (Debug.DrawLine은 C# 함수라 Burst에서 못 씀)
// public partial class DebugDrawSystem : SystemBase
// {
//     protected override void OnUpdate()
//     {
//         // 1. "PlayerTag"를 가진 엔티티만 조회합니다. (딱 1명)
//         foreach (var (transform, target) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<Target>>().WithAll<PlayerTag>())
//         {
//             // 2. 타겟이 실제로 존재하는지 확인
//             if (target.ValueRO.TargetEntity == Entity.Null) continue;
//
//             // 3. 타겟의 위치를 알아내야 함
//             // (TargetEntity ID만 알지 위치는 모르니까, Lookup으로 찾아야 함)
//             if (SystemAPI.HasComponent<LocalTransform>(target.ValueRO.TargetEntity))
//             {
//                 var targetPos = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity).Position;
//                 
//                 // ★ 선 그리기! (주인공 -> 타겟)
//                 Debug.DrawLine(transform.ValueRO.Position, targetPos, Color.green);
//             }
//         }
//     }
// }