﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.Scripting;


namespace MSuhininTestovoe.B2B
{
    public class PlayerAtackSystem : EcsUguiCallbackSystem, IEcsInitSystem
    {
        private EcsFilter _filterRayOn;
        private EcsFilter _enemyFilter;
        private EcsPool<HealthViewComponent> _enemyHealthViewComponentPool;
        private EcsPool<EnemyHealthComponent> _enemyHealthComponentPool;
        private EcsPool<RayComponent> _rayComponent;
        private int _entity;
        private IEcsSystems _systems;

        
        [Preserve]
        [EcsUguiClickEvent(UIConstants.BTN_ATACK, WorldsNamesConstants.EVENTS)]
        void OnClickPlayerAttack(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filterRayOn)
            {
                ref RayComponent rayComponent = ref _rayComponent.Get(entity);
                var enemyEntity = rayComponent.Value.transform.gameObject.GetComponent<EnemyActor>().Entity;

                _entity = enemyEntity;
                Attack();
            }
        }


        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            _filterRayOn = world
                .Filter<IsPlayerComponent>()
                .Inc<RayComponent>()
                .Inc<IsPlayerCanAttackComponent>()
                .End();

            _enemyFilter = world
                .Filter<EnemyHealthComponent>()
                .Inc<HealthViewComponent>()
                .End();

            _enemyHealthViewComponentPool = world.GetPool<HealthViewComponent>();
            _enemyHealthComponentPool = world.GetPool<EnemyHealthComponent>();
            _rayComponent = world.GetPool<RayComponent>();
            _systems = systems;
        }


        private void Attack()
        {
            ref EnemyHealthComponent healthValue = ref _enemyHealthComponentPool.Get(_entity);
            ref HealthViewComponent healthView = ref _enemyHealthViewComponentPool.Get(_entity);
            healthValue.HealthValue -= 1;
            healthView.Value.size = new Vector2(healthValue.HealthValue, 1);
            AddHitSoundComponent(_systems, SoundsEnumType.FIRE);
        }

        private void AddHitSoundComponent(IEcsSystems systems, SoundsEnumType type)
        {
            var entity = SoundCatchSystem.sounEffectsSourceEntity;
            var sound = systems.GetWorld().GetPool<IsPlaySoundComponent>();
            if (sound.Has(entity)) return;

            ref var isHitSoundComponent = ref systems.GetWorld()
                .GetPool<IsPlaySoundComponent>()
                .Add(entity);
            isHitSoundComponent.SoundType = type;
        }
    }
}