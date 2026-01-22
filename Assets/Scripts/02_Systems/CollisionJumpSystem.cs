// using UnityEngine;
// using Unity.Physics;
// using Unity.Physics.Systems; // 물리 시스템 그룹 사용
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
//
// // 물리는 FixedUpdate 타이밍에 돌기 때문에 , 시스템도 그 그룹에 넣어준다.
//
// [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
// [UpdateAfter(typeof(PhysicsSystemGroup))]
// [BurstCompile]
// public partial struct CollisionJumpSystem : ISystem
// {
//     public void OnCreate(ref SystemState state)
//     {
//         state.RequireForUpdate<SimulationSingleton>(); // 물리 시뮬레이션 데이터가 있어야 실행
//     }
//
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         // 1. 물리 세계(Simulation) 가져오기
//         var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
//         
//         // 2. 데이터 조회(Lookup) 도구 준비
//         // Job 안에서는 EntityManager를 쓰지 못하니 , 미리 데이터를 찾을 수 있는 "사전"을 챙겨준다.
//         
//         // 속도를 수정해야 하니까 isReadOnly = false
//         var velocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(isReadOnly: false);
//         
//         // 바닥인지 확인만 하면 되니까 isReadOnly = true
//         var floorLookup = SystemAPI.GetComponentLookup<FloorTag>(isReadOnly: true);
//         
//         // 3. Job 생성 및 예약
//         var job = new CollisionEventJob
//         {
//             VelocityLookup = velocityLookup,
//             FloorLookup = floorLookup,
//         };
//         state.Dependency = job.Schedule(simulation, state.Dependency);
//         
//
//     }
//
//     [BurstCompile]
//     struct CollisionEventJob : ICollisionEventsJob
//     {
//         public ComponentLookup<PhysicsVelocity> VelocityLookup;
//         [ReadOnly] public ComponentLookup<FloorTag> FloorLookup;
//
//         // 충돌이 발생할 때마다 이 함수가 실행
//         public void Execute(CollisionEvent collisionEvent)
//         {
//             // 부딪힌 두 물체 
//             Entity entityA = collisionEvent.EntityA;
//             Entity entityB = collisionEvent.EntityB;
//             
//             // 상황 1 : A가 바닥이고 B가 큐브일 때
//             // B가 속도(PhysicsVelocity)를 가지고 있고, A가 바닥 태그(FloorTag)를 가지고 있다면?
//             if (VelocityLookup.HasComponent(entityB) && FloorLookup.HasComponent(entityA))
//             {
//                 Bounce(entityB);
//             }
//             // 상황 2 : B가 바닥이고 A가 큐브일 때
//             else if (VelocityLookup.HasComponent(entityA) && FloorLookup.HasComponent(entityB))
//             {
//                 Bounce(entityA);
//             }
//         }
//
//         // 로직 함수
//         private void Bounce(Entity entity)
//         {
//             var velocity = VelocityLookup[entity];
//             velocity.Linear.y = 15.0f;
//             VelocityLookup[entity] = velocity;
//         }
//     }
// }