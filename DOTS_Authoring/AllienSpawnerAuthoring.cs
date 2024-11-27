using Unity.Entities;
using UnityEngine;

public class AlienSpawnerAuthoring : MonoBehaviour
{


    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;


    public class Baker : Baker<AlienSpawnerAuthoring>
    {

        public override void Bake(AlienSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AlienSpawner
            {
                timerMax = authoring.timerMax,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
            });
        }
    }


}


public struct AlienSpawner : IComponentData
{

    public float timer;
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;

}