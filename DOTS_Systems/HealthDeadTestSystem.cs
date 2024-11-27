using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthDeadTestSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get the EndSimulationEntityCommandBufferSystem's singleton instance to handle entity destruction
        EntityCommandBuffer ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        // Schedule the job
        var healthDeadTestJob = new HealthDeadTestJob
        {
            ECB = ecb.AsParallelWriter(),
        };

        healthDeadTestJob.ScheduleParallel();
    }


}
[BurstCompile]
public partial struct HealthDeadTestJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute([EntityIndexInQuery] int entityInQueryIndex, in Health health, Entity entity)
    {
        if (health.healthAmount <= 0)
        {
            // Mark the entity for destruction
            ECB.DestroyEntity(entityInQueryIndex, entity);
        }
    }
}