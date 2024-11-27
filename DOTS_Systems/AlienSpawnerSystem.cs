using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct AlienSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        new AlienSpawnerJob
        {
            entitiesReferences = entitiesReferences,
            deltaTime = SystemAPI.Time.DeltaTime,
            commandBuffer = commandBuffer.AsParallelWriter()
        }.ScheduleParallel();
    }
}

public partial struct AlienSpawnerJob : IJobEntity
{
    public EntitiesReferences entitiesReferences;
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter commandBuffer;

    public void Execute([EntityIndexInQuery] int entityIndex, RefRO<LocalTransform> localTransform, RefRW<AlienSpawner> alienSpawner)
    {
        alienSpawner.ValueRW.timer -= deltaTime;
        if (alienSpawner.ValueRW.timer > 0f)
            return;

        alienSpawner.ValueRW.timer = alienSpawner.ValueRO.timerMax;

        Entity alienEntity = commandBuffer.Instantiate(entityIndex, entitiesReferences.alienPrefabEntity);
        commandBuffer.SetComponent(entityIndex, alienEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
        commandBuffer.AddComponent(entityIndex, alienEntity, new RandomWalking
        {
            originPosition = localTransform.ValueRO.Position,
            targetPosition = localTransform.ValueRO.Position,
            distanceMin = alienSpawner.ValueRO.randomWalkingDistanceMin,
            distanceMax = alienSpawner.ValueRO.randomWalkingDistanceMax,
            random = new Unity.Mathematics.Random((uint)alienEntity.Index),
        });
    }

}
