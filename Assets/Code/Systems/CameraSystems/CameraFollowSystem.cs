using Leopotam.EcsLite;
using LeopotamGroup.Globals;
using UnityEngine;

namespace MSuhininTestovoe.B2B
{
    public class CameraFollowSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _cameraFilter;
        private EcsFilter _playerFilter;
        private EcsPool<IsCameraComponent> _isCameraComponentPool;
        private EcsPool<TransformComponent> _transformComponentPool;
        private ITimeService _timeService;


        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            _cameraFilter = world.Filter<IsCameraComponent>().Inc<TransformComponent>().End();
            _playerFilter = world.Filter<IsPlayerComponent>().End();
            _isCameraComponentPool = world.GetPool<IsCameraComponent>();
            _transformComponentPool = world.GetPool<TransformComponent>();
            _timeService = Service<ITimeService>.Get();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int cameraEntity in _cameraFilter)
            {
                ref IsCameraComponent isCameraComponent = ref _isCameraComponentPool.Get(cameraEntity);
                ref TransformComponent cameraTransformComponent = ref _transformComponentPool.Get(cameraEntity);
                ref TransformComponent playerTransformComponent =
                    ref _transformComponentPool.Get(_playerFilter.GetRawEntities()[0]);
                Vector3 currentPosition = cameraTransformComponent.Value.position;
                var fff = (playerTransformComponent.Value.position - cameraTransformComponent.Value.localPosition).normalized;
                Vector3 targetPoint = new Vector3(playerTransformComponent.Value.localPosition.x,
                    playerTransformComponent.Value.position.y+5,
                    playerTransformComponent.Value.localPosition.z)+fff;

                var pers = playerTransformComponent.Value;
                var cam = cameraTransformComponent.Value;
                cam.transform.rotation = Quaternion.LookRotation(pers.transform.forward, pers.transform.up);
                
                cameraTransformComponent.Value.position = Vector3.SmoothDamp(currentPosition, targetPoint,
                    ref isCameraComponent.CurrentVelocity, isCameraComponent.CameraSmoothness);


           
            }
        }
    }
}