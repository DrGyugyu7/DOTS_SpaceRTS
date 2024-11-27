using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
public partial struct AnimationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (gameObjectPrefab, entity) in
                 SystemAPI.Query<GameObjectPrefab>().WithNone<AnimatorReference>().WithEntityAccess())
        {
            var visualGameObject = Object.Instantiate(gameObjectPrefab.prefab);
            var animatorReference = new AnimatorReference
            {
                animator = visualGameObject.GetComponent<Animator>()
            };
            ecb.AddComponent(entity, animatorReference);
        }

        foreach (var (transform, animatorReference) in
                 SystemAPI.Query<LocalTransform, AnimatorReference>())
        {

            animatorReference.animator.transform.position = transform.Position;
            animatorReference.animator.transform.rotation = transform.Rotation;
        }

        foreach (var (animatorReference, shootAttack, unitMover) in
                SystemAPI.Query<AnimatorReference, RefRO<ShootAttack>, RefRO<UnitMover>>())
        {

            if (shootAttack.ValueRO.onShoot.isTriggered)
            {
                animatorReference.animator.SetBool(animatorReference.hashAttack, true);
            }
            else
            {
                animatorReference.animator.SetBool(animatorReference.hashAttack, false);
            }
            if (unitMover.ValueRO.isMove)
            {
                animatorReference.animator.SetBool(animatorReference.hashMove, true);
            }
            else
            {
                animatorReference.animator.SetBool(animatorReference.hashMove, false);
            }
        }
        foreach (var (animatorReference, meleeAttack, unitMover) in
                SystemAPI.Query<AnimatorReference, RefRO<MeleeAttack>, RefRO<UnitMover>>())
        {
            if (meleeAttack.ValueRO.isAttack)
            {
                animatorReference.animator.SetBool(animatorReference.hashAttack, true);
            }
            else
            {
                animatorReference.animator.SetBool(animatorReference.hashAttack, false);
            }
            if (unitMover.ValueRO.isMove)
            {
                animatorReference.animator.SetBool(animatorReference.hashMove, true);
            }
            else
            {
                animatorReference.animator.SetBool(animatorReference.hashMove, false);
            }
        }

        foreach (var (animatorReference, entity) in
                 SystemAPI.Query<AnimatorReference>().WithNone<GameObjectPrefab, LocalTransform>()
                     .WithEntityAccess())
        {
            Object.Destroy(animatorReference.animator.gameObject);
            ecb.RemoveComponent<AnimatorReference>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}