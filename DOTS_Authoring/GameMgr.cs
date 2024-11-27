using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

class GameMgrAuthoring : MonoBehaviour
{
    private class GameMgrBaker : Baker<GameMgrAuthoring>
    {
        public override void Bake(GameMgrAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GameMgrComponent());
        }
    }
}
public struct GameMgrComponent : IComponentData
{
    public bool gameOver;
    public bool gameClear;
}
public partial struct GameMgrSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<int> unitCounts = new NativeArray<int>(2, Allocator.TempJob);

        var job = new GameMgrJob
        {
            unitCounts = unitCounts
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);

        state.Dependency.Complete();
        int soldierCount = unitCounts[(int)Faction.Friendly];
        int alienCount = unitCounts[(int)Faction.Alien];
        var gameMgrEntity = SystemAPI.GetSingletonEntity<GameMgrComponent>();
        var gameStatus = state.EntityManager.GetComponentData<GameMgrComponent>(gameMgrEntity);
        if (soldierCount == 0)
        {
            gameStatus.gameOver = true;
            gameStatus.gameClear = false;
        }
        else if (alienCount == 0)
        {
            gameStatus.gameClear = true;
            gameStatus.gameOver = false;
        }
        else
        {
            gameStatus.gameOver = false;
            gameStatus.gameClear = false;
        }
        state.EntityManager.SetComponentData(gameMgrEntity, gameStatus);
        unitCounts.Dispose();
    }
}
[BurstCompile]
public partial struct GameMgrJob : IJobEntity
{
    [NativeDisableParallelForRestriction] public NativeArray<int> unitCounts;
    public void Execute(ref Unit unit)
    {
        if (unit.faction == Faction.Friendly)
        {
            unitCounts[(int)Faction.Friendly] += 1;
        }
        else if (unit.faction == Faction.Alien)
        {
            unitCounts[(int)Faction.Alien] += 1;
        }
    }
}