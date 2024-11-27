using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class UnitMoverAuthoring : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;


    public class Baker : Baker<UnitMoverAuthoring>
    {

        public override void Bake(UnitMoverAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMover
            {
                moveSpeed = authoring.moveSpeed,
                rotationSpeed = authoring.rotationSpeed,
            });
        }

    }

}

public struct UnitMover : IComponentData
{

    public bool isMove;
    public float moveSpeed;
    public float rotationSpeed;
    public float3 targetPosition;


}
