using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UniRx;
using UnityEngine;


namespace MSuhininTestovoe.B2B
{
    public class EnemyTargetSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter filterAI, filterPlayer;
        private EcsWorld _world;
        private EcsPool<AIPathComponent> _isReachedComponentPool;
        private EcsPool<HealthViewComponent> _playerHealthViewComponentPool;
        private EcsPool<AIDestanationComponent> _aiDestanationComponenPool;
        private EcsPool<TransformComponent> _playerTransformPool;


        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            filterPlayer = systems.GetWorld()
                .Filter<IsPlayerComponent>()
                .End();

            filterAI = systems.GetWorld()
                .Filter<AIPathComponent>()
                .Inc<AIDestanationComponent>()
                .End();


            _isReachedComponentPool = _world.GetPool<AIPathComponent>();
            _aiDestanationComponenPool = _world.GetPool<AIDestanationComponent>();
            _playerTransformPool = _world.GetPool<TransformComponent>();
        }


        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity in filterPlayer)
            {
                foreach (var entity in filterAI)
                {
                    ref AIPathComponent aiPathComponent = ref _isReachedComponentPool.Get(entity);
                    ref AIDestanationComponent target = ref _aiDestanationComponenPool.Get(entity);
                    ref TransformComponent player = ref _playerTransformPool.Get(playerEntity);
                    target.AIDestinationSetter.target = player.Value;
                    return;
                }
            }
        }
    }
}