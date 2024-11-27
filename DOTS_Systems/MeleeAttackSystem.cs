using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
partial struct MeleeAttackSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var job = new MeleeAttackJob
        {
            PhysicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>(),
            TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
            HealthLookup = SystemAPI.GetComponentLookup<Health>(),
            DeltaTime = SystemAPI.Time.DeltaTime,
            MeleeAttackDistanceSq = 2f
        };

        job.Schedule();
    }
}
[BurstCompile]
public partial struct MeleeAttackJob : IJobEntity
{
    [ReadOnly] public PhysicsWorldSingleton PhysicsWorld;
    [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
    [NativeDisableParallelForRestriction] public ComponentLookup<Health> HealthLookup;
    public float DeltaTime;
    public float MeleeAttackDistanceSq;

    private void Execute(
        in LocalTransform transform,
        ref MeleeAttack meleeAttack,
        in Target target,
        ref UnitMover unitMover)
    {
        if (target.targetEntity == Entity.Null)
        {
            meleeAttack.isAttack = false;
            return;
        }

        LocalTransform targetTransform = TransformLookup[target.targetEntity];
        float3 currentPos = transform.Position;
        float3 targetPos = targetTransform.Position;

        bool isCloseEnoughToAttack = math.distancesq(currentPos, targetPos) < MeleeAttackDistanceSq;
        bool isTouchingTarget = false;

        if (!isCloseEnoughToAttack)
        {
            meleeAttack.isAttack = false;
            unitMover.targetPosition = targetPos;

            float3 dirToTarget = targetPos - currentPos;
            dirToTarget = math.normalize(dirToTarget);
            float distanceExtraToTestRaycast = .4f;

            RaycastInput raycastInput = new RaycastInput
            {
                Start = currentPos,
                End = currentPos + dirToTarget * (meleeAttack.colliderSize + distanceExtraToTestRaycast),
                Filter = CollisionFilter.Default,
            };

            NativeList<RaycastHit> raycastHits = new NativeList<RaycastHit>(Allocator.Temp);
            raycastHits.Clear();

            if (PhysicsWorld.CollisionWorld.CastRay(raycastInput, ref raycastHits))
            {
                foreach (RaycastHit raycastHit in raycastHits)
                {
                    if (raycastHit.Entity == target.targetEntity)
                    {
                        isTouchingTarget = true;
                        break;
                    }
                }
            }
            raycastHits.Dispose();
        }

        if (!isCloseEnoughToAttack && !isTouchingTarget)
        {
            unitMover.targetPosition = targetPos;
            meleeAttack.isAttack = false;
        }
        else
        {
            unitMover.targetPosition = currentPos;
            meleeAttack.isAttack = true;
            meleeAttack.timer -= DeltaTime;

            if (meleeAttack.timer <= 0)
            {
                meleeAttack.timer = meleeAttack.timerMax;
                var targetHealth = HealthLookup[target.targetEntity];
                targetHealth.healthAmount -= meleeAttack.damageAmount;
                targetHealth.onHealthChanged = true;
                HealthLookup[target.targetEntity] = targetHealth;
            }
        }
    }
}