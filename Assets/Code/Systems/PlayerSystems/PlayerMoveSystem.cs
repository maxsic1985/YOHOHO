using Leopotam.EcsLite;
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
        private   PlayerSharedData _sharedData;
        private Vector3 playerPosition;

        
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

      
        private void PlayerMoving(ref TransformComponent transformComponent, ref PlayerInputComponent inputComponent)//,ref DestinationComponent destinationComponent)
        {
            Vector3 direction = transformComponent.Value.forward * inputComponent.Vertical;// + transformComponent.Value.right * inputComponent.Horizontal;
         
         transformComponent.Value.position = Vector3.Lerp( transformComponent.Value.position,
             transformComponent.Value.position+direction,
             _sharedData.GetPlayerCharacteristic.Speed * _timeService.DeltaTime);
         
         Vector3 directionRotation = transformComponent.Value.right * inputComponent.Horizontal;
         var pers = transformComponent.Value;

         
         Quaternion toRotation = Quaternion.LookRotation(directionRotation, Vector3.up);
         pers.transform.rotation = Quaternion.RotateTowards(pers.transform.rotation, toRotation, 10 * _timeService.DeltaTime); 
         
         
         
        // float rotationY = pers.transform.localEulerAngles.y;
        // pers.transform.localEulerAngles = new Vector3(0, rotationY, 0);
         
         
         
         
        // pers.transform.rotation = Quaternion.LookRotation(pers.transform.forward, pers.transform.right+directionRotation);

         //transformComponent.Value.transform.rotation = Quaternion.Lerp(transformComponent.Value.transform.rotation,Quaternion.Euler(directionRotation),10);  
             //   transformComponent.Value.rotation = Quaternion.Lerp (transformComponent.Value.rotation , Quaternion.Euler(0,90,0),  _timeService.DeltaTime * 10);
        
         
        }
    }
}