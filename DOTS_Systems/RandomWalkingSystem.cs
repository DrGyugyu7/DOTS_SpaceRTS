using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var randomWalkingJob = new RandomWalkingJob
        {
            ReachedTargetPositionDistanceSq = UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ
        };

        randomWalkingJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct RandomWalkingJob : IJobEntity
{
    public float ReachedTargetPositionDistanceSq;

    public void Execute(ref RandomWalking randomWalking, ref UnitMover unitMover, in LocalTransform localTransform)
    {
        if (math.distancesq(localTransform.Position, randomWalking.targetPosition) < ReachedTargetPositionDistanceSq)
        {
            Random random = randomWalking.random;

            float3 randomDirection = math.normalize(new float3(random.NextFloat(-1f, +1f), 0, random.NextFloat(-1f, +1f)));

            randomWalking.targetPosition = randomWalking.originPosition + randomDirection * random.NextFloat(randomWalking.distanceMin, randomWalking.distanceMax);

            randomWalking.random = random;
        }
        else
        {
            unitMover.targetPosition = randomWalking.targetPosition;
        }
    }
}
