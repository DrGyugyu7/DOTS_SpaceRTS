using Unity.Entities;
using UnityEngine;

public class AnimatorAuthoring : MonoBehaviour
{
    public GameObject GameObjectPrefab;

    private class GameObjectPrefabBaker : Baker<AnimatorAuthoring>
    {
        public override void Bake(AnimatorAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new GameObjectPrefab { prefab = authoring.GameObjectPrefab });
        }
    }
}
public class GameObjectPrefab : IComponentData
{
    public GameObject prefab;
}

public class AnimatorReference : ICleanupComponentData
{
    public Animator animator;
    public readonly int hashAttack = Animator.StringToHash("isAttack");
    public readonly int hashMove = Animator.StringToHash("isMove");
    public readonly int hashHit = Animator.StringToHash("Hit");
}