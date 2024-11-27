using Unity.Entities;
using UnityEngine;

public class AlienAuthoring : MonoBehaviour
{


    public class Baker : Baker<AlienAuthoring>
    {


        public override void Bake(AlienAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Alien());
        }
    }
}




public struct Alien : IComponentData
{
}