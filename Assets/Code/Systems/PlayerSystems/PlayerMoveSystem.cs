using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using LeopotamGroup.Globals;
using UnityEngine;


namespace MSuhininTestovoe.B2B
{
    public class PlayerMoveSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _playerFilter;
        private EcsPool<PlayerInputComponent> _playerInputComponentPool;
        private EcsPool<IsPlayerControlComponent> _isPlayerMoveComponentPool;
        private EcsPool<TransformComponent> _transformComponentPool;
        private ITimeService _timeService;
        private PlayerSharedData _sharedData;
        private Vector3 playerPosition;
        private readonly EcsCustomInject<AttackInputView> _attackInput = default;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            _sharedData = systems.GetShared<SharedData>().GetPlayerSharedData;
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
                    PlayerMoving(ref transformComponent,
                        ref playerInputComponent);
            }
        }


        private void PlayerMoving(ref TransformComponent transformComponent, ref PlayerInputComponent inputComponent)
        {
            if(Mathf.Abs(inputComponent.Vertical)  < Mathf.Abs(inputComponent.Horizontal)) return;
            Vector3 direction =
                transformComponent.Value.forward * inputComponent.Vertical;
            var transform = transformComponent.Value;
            transform.position = Vector3.Lerp(transform.position,
                transform.position + direction,
                _sharedData.GetPlayerCharacteristic.Speed * _timeService.DeltaTime);
            
            var player = _sharedData.GetPlayerCharacteristic.Transform;
            Ray r = new Ray(transform.transform.position,transform.forward);
            var hit = Physics.Raycast(r, out var hitInfo);
            Debug.DrawRay(transform.transform.position,transform.forward,Color.red);
            if (hit)
            {
                _attackInput.Value.TriggerEnter?.Invoke(true);
                Debug.Log(hitInfo.transform.gameObject);
            }
            else
            {
                _attackInput.Value.TriggerEnter?.Invoke(false);
            }
        }
    }
}