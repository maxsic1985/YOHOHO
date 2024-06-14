using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UniRx;
using UnityEngine;


namespace MSuhininTestovoe.B2B
{
    public class EnemyAtackSystem : IEcsInitSystem, IEcsRunSystem, IDisposable
    {
        private List<IDisposable> _disposables = new List<IDisposable>();
        private EcsFilter filterTrigger, filterPlayer;
        private EcsWorld _world;
        private bool _reachedToPlayer ;
        private EcsPool<AIPathComponent> _isReachedComponentPool;
        private EcsPool<HealthViewComponent> _playerHealthViewComponentPool;
        private EcsPool<AIDestanationComponent> _aiDestanationComponenPool;
        private EcsPool<TransformComponent> _playerTransformPool;

        private PlayerSharedData _sharedData;


        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _sharedData = systems.GetShared<SharedData>().GetPlayerSharedData;


            filterPlayer = systems.GetWorld()
                .Filter<IsPlayerComponent>()
                .Inc<HealthViewComponent>()
                .End();


            filterTrigger = systems.GetWorld()
                .Filter<AIPathComponent>()
                .Inc<AIDestanationComponent>()
                .End();

            _playerHealthViewComponentPool = _world.GetPool<HealthViewComponent>();
            _isReachedComponentPool = _world.GetPool<AIPathComponent>();
            _aiDestanationComponenPool = _world.GetPool<AIDestanationComponent>();
            _playerTransformPool = _world.GetPool<TransformComponent>();

            Observable.Interval(TimeSpan.FromMilliseconds(3000)).Where(_ => _reachedToPlayer)
                .Subscribe(x => { Attack(); })
                .AddTo(_disposables);
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity in filterPlayer)
            {
                foreach (var entity in filterTrigger)
                {
                    ref AIPathComponent aiPathComponent = ref _isReachedComponentPool.Get(entity);

                    _reachedToPlayer = aiPathComponent.AIPath.reachedEndOfPath;
                    ref AIDestanationComponent target = ref _aiDestanationComponenPool.Get(entity);
                    ref TransformComponent player = ref _playerTransformPool.Get(playerEntity);
                    target.AIDestinationSetter.target = player.Value;
                    ref HealthViewComponent healthView = ref _playerHealthViewComponentPool.Get(playerEntity);
                    var currentHealh = _sharedData.GetPlayerCharacteristic.GetLives.GetCurrrentLives;
                    healthView.Value.size = new Vector2(currentHealh, 1);

                    if (_sharedData.GetPlayerCharacteristic.GetLives.GetCurrrentLives == 0)
                    {
                        _reachedToPlayer = false;
                        _isReachedComponentPool.Del(entity);
                    }

                    return;
                }

                _reachedToPlayer = false;
            }
        }


        private void Attack()
        {
            _sharedData.GetPlayerCharacteristic.GetLives.UpdateLives(-1);
        }
        
        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}