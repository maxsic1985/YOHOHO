using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using LeopotamGroup.Globals;
using UniRx;


namespace MSuhininTestovoe.B2B
{
    public class EnemyRespawnSystem : IEcsInitSystem, IEcsDestroySystem, IDisposable
    {
        private EcsFilter filterTrigger;
        private IPoolService _poolService;
        private List<IDisposable> _disposables = new List<IDisposable>();
        private EcsPool<TransformComponent> _transformComponentPool;
        private EcsPool<EnemyStartPositionComponent> _enemyStartComponentPool;
        private EcsWorld _world;


        public void Init(IEcsSystems systems)
        {
            _poolService = Service<IPoolService>.Get();
            _world = systems.GetWorld();
            filterTrigger = systems.GetWorld()
                .Filter<EnemyStartPositionComponent>()
                .End();

            _transformComponentPool = _world.GetPool<TransformComponent>();
            _enemyStartComponentPool = _world.GetPool<EnemyStartPositionComponent>();

            Observable.Interval(TimeSpan.FromMilliseconds(LimitsConstants.COOLDOWN_ENEMY)).Where(_ => true)
                .Subscribe(x => { Respawn(); })
                .AddTo(_disposables);
        }


        private void Respawn()
        {
            foreach (var _ in filterTrigger)
            {
                if (_poolService == null)
                {
                    _poolService = Service<IPoolService>.Get();
                    Dispose();
                }

                var pooled = _poolService.Get(GameObjectsTypeId.Enemy);
                var entity = pooled.gameObject.GetComponent<EnemyActor>().Entity;

                ref EnemyStartPositionComponent enemyStartPositionComponent = ref _enemyStartComponentPool.Get(entity);
                ref TransformComponent transformComponent = ref _transformComponentPool.Get(entity);
                var index = enemyStartPositionComponent.Value.Count - 1;
                
                var position = enemyStartPositionComponent.Value[Extensions.GetRandomDigit(0, index)];
                transformComponent.Value.position = position;
                return;
            }
        }


        public void Destroy(IEcsSystems systems)
        {
            Dispose();
        }


        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}