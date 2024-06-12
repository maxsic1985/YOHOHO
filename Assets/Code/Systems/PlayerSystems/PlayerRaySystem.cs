﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.Scripting;



namespace MSuhininTestovoe.B2B
{
    public class PlayerRaySystem : EcsUguiCallbackSystem, IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsFilter _enemyFilter;
        private EcsPool<HealthViewComponent> _enemyHealthViewComponentPool;
        private EcsPool<EnemyHealthComponent> _enemyHealthComponentPool;
        private int _entity;
        private readonly EcsCustomInject<AttackInputView> _attackInput = default;
        

        [Preserve]
        [EcsUguiClickEvent(UIConstants.BTN_ATACK, WorldsNamesConstants.EVENTS)] 
        void OnClickPlayerAttack(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter)
            {
                foreach (var enemyEntity in _enemyFilter)
                {
                    ref HealthViewComponent healthView = ref _enemyHealthViewComponentPool.Get(enemyEntity);
                    ref EnemyHealthComponent healthValue = ref _enemyHealthComponentPool.Get(enemyEntity);
                    var currentHealh = healthValue.HealthValue;
            
                    healthView.Value.size = new Vector2(currentHealh, 1);
                    _entity = enemyEntity;
                }
                Attack();
            }
        }
        
        
        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            _filter = world
                .Filter<IsPlayerCanAttackComponent>()
                .End();
            
            _enemyFilter = world
                .Filter<EnemyHealthComponent>()
                .Inc<HealthViewComponent>()
                .End();
            _enemyHealthViewComponentPool = world.GetPool<HealthViewComponent>();
            _enemyHealthComponentPool = world.GetPool<EnemyHealthComponent>();
        }
        
        
        private void Attack()
        {
            ref EnemyHealthComponent healthValue = ref _enemyHealthComponentPool.Get(_entity);
            healthValue.HealthValue -= 1;
        }
    }
}