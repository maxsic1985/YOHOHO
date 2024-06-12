using Leopotam.EcsLite;
using LeopotamGroup.Globals;
using UnityEngine;


namespace MSuhininTestovoe.B2B
{
    public class PlayerRotateSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _playerFilter;
        private EcsPool<PlayerInputComponent> _playerInputComponentPool;
        private EcsPool<IsPlayerControlComponent> _isPlayerMoveComponentPool;
        private EcsPool<TransformComponent> _transformComponentPool;
        private ITimeService _timeService;
        private Vector3 playerPosition;


        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            _playerFilter = world.Filter<IsPlayerComponent>()
                .Inc<TransformComponent>()
                .Inc<IsPlayerControlComponent>()
                .End();
            
            _playerInputComponentPool = world.GetPool<PlayerInputComponent>();
            _transformComponentPool = world.GetPool<TransformComponent>();
            _isPlayerMoveComponentPool = world.GetPool<IsPlayerControlComponent>();
            _timeService = Service<ITimeService>.Get();
        }


        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _playerFilter)
            {
                ref TransformComponent transformComponent = ref _transformComponentPool.Get(entity);
                ref PlayerInputComponent playerInputComponent = ref _playerInputComponentPool.Get(entity);

                if (_isPlayerMoveComponentPool.Has(entity))
                    PlayerRotating(ref transformComponent,
                        ref playerInputComponent);
            }
        }


        private void PlayerRotating(ref TransformComponent transformComponent, ref PlayerInputComponent inputComponent)
        {
          //  var k =Mathf.Abs(inputComponent.Horizontal)  > 0.8f ? inputComponent.Horizontal * 1 : 0;
          if(Mathf.Abs(inputComponent.Horizontal)  < 0.8f) return;
            
            Debug.Log("hor"+inputComponent.Horizontal);
            Vector3 directionRotation = transformComponent.Value.right *inputComponent.Horizontal;
            var transform = transformComponent.Value.transform;

            Quaternion toRotation = Quaternion.LookRotation(directionRotation, Vector3.up);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, toRotation, 10 * _timeService.DeltaTime);
        }
    }
}