using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.Scripting;


namespace MSuhininTestovoe.B2B
{
    public class PlayerRaySystem : EcsUguiCallbackSystem, IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _playerFilter;
        private EcsPool<PlayerInputComponent> _playerInputComponentPool;
        private EcsPool<IsPlayerControlComponent> _isPlayerMoveComponentPool;
        private EcsPool<RayComponent> _rayComponent;
        private EcsPool<TransformComponent> _transformComponentPool;
        private PlayerSharedData _sharedData;
        private Vector3 playerPosition;
        private readonly EcsCustomInject<AttackInputView> _attackInput = default;


        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            _sharedData = systems.GetShared<SharedData>().GetPlayerSharedData;
            _playerFilter = world.Filter<IsPlayerComponent>()
                .Inc<TransformComponent>()
                .End();
            _playerInputComponentPool = world.GetPool<PlayerInputComponent>();
            _transformComponentPool = world.GetPool<TransformComponent>();
            _isPlayerMoveComponentPool = world.GetPool<IsPlayerControlComponent>();
            _rayComponent = world.GetPool<RayComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _playerFilter)
            {
                ref TransformComponent transformComponent = ref _transformComponentPool.Get(entity);
                ref PlayerInputComponent playerInputComponent = ref _playerInputComponentPool.Get(entity);

                if (_isPlayerMoveComponentPool.Has(entity))
                {
                    Ray ray = new Ray(transformComponent.Value.transform.position, transformComponent.Value.forward);
                    var hit = Physics.Raycast(ray, out var hitInfo);
                    
                    if (hit)
                    {
                        Extensions.AddPool<RayComponent>(systems,entity);
                        _rayComponent.Get(entity).Value = hitInfo;
                    }
                    else
                    {
                        _rayComponent.Del(entity);
                    }
                }
            }
        }
    }
}