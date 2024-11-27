using Unity.Entities;
using UnityEngine;

public class EntitiesReferencesAuthoring : MonoBehaviour
{


    public GameObject bulletPrefabGameObject;
    public GameObject alienPrefabGameObject;
    public GameObject shootLightPrefabGameObject;


    public class Baker : Baker<EntitiesReferencesAuthoring>
    {


        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic),
                alienPrefabEntity = GetEntity(authoring.alienPrefabGameObject, TransformUsageFlags.Dynamic),
                shootLightPrefabEntity = GetEntity(authoring.shootLightPrefabGameObject, TransformUsageFlags.Dynamic)
            });
        }

    }

}


public struct EntitiesReferences : IComponentData
{

    public Entity bulletPrefabEntity;
    public Entity alienPrefabEntity;
    public Entity shootLightPrefabEntity;

}