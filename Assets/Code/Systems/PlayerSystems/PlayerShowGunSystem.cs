using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.Scripting;


namespace MSuhininTestovoe.B2B
{
    public class PlayerShowGunSystem :  IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _filterRayOn;
        private EcsFilter _filterRayOff;
        private EcsPool<IsPlayerCanAttackComponent> _isCanAttackComponentPool;
        private EcsPool<RayComponent> _rayComponent;
        private readonly EcsCustomInject<AttackInputView> _attackInput = default;
        
        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            
            _filterRayOn = world
                .Filter<IsPlayerComponent>()
                .Inc<RayComponent>()
                .Exc<IsPlayerCanAttackComponent>()
                .End();

            _filterRayOff = world
                .Filter<IsPlayerComponent>()
                .Inc<IsPlayerCanAttackComponent>()
                .Exc<RayComponent>()
                .End();
            
            _isCanAttackComponentPool = world.GetPool<IsPlayerCanAttackComponent>();
            _rayComponent = world.GetPool<RayComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filterRayOn)
            {
                ref RayComponent rayComponent = ref _rayComponent.Get(entity);
                if (rayComponent.Value.transform.gameObject.GetComponent<EnemyActor>() != null)
                {
                    _attackInput.Value.RayAction?.Invoke(true);
                    _isCanAttackComponentPool.Add(entity);
                }
                else
                {
                    _attackInput.Value.RayAction?.Invoke(false);
                }
            }

            foreach (var entity in _filterRayOff)
            {
                if (_attackInput.Value.AttackBtn.gameObject.activeSelf) _attackInput.Value.RayAction?.Invoke(false);
                _isCanAttackComponentPool.Del(entity);
            }
        }
    }
}