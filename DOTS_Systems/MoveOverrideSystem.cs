using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverrideSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var moveOverrideJob = new MoveOverrideJob
        {
            ReachedTargetPositionDistanceSq = UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ
        };

        moveOverrideJob.ScheduleParallel();
    }
}

public partial struct MoveOverrideJob : IJobEntity
{
    public float ReachedTargetPositionDistanceSq;

    public void Execute(ref UnitMover unitMover, ref MoveOverride moveOverride, ref LocalTransform localTransform, EnabledRefRW<MoveOverride> moveOverrideEnabled)
    {
        if (math.distancesq(localTransform.Position, moveOverride.targetPosition) > ReachedTargetPositionDistanceSq)
        {
            unitMover.targetPosition = moveOverride.targetPosition;
        }
        else
        {
            moveOverrideEnabled.ValueRW = false;
        }
    }
}
