using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
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


        [Preserve]
        [EcsUguiClickEvent(UIConstants.BTN_ATACK, WorldsNamesConstants.EVENTS)]
        void OnClickPlayerAttack(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filterRayOn)
            {
                ref RayComponent rayComponent = ref _rayComponent.Get(entity);
                var enemyEntity = rayComponent.Value.transform.gameObject.GetComponent<EnemyActor>().Entity;

                foreach (var ee in _enemyFilter)
                {
                    ref HealthViewComponent healthView = ref _enemyHealthViewComponentPool.Get(enemyEntity);
                    ref EnemyHealthComponent healthValue = ref _enemyHealthComponentPool.Get(enemyEntity);
                    var currentHealh = healthValue.HealthValue;

                    healthView.Value.size = new Vector2(currentHealh, 1);
                }
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
        }


        private void Attack()
        {
            ref EnemyHealthComponent healthValue = ref _enemyHealthComponentPool.Get(_entity);
            healthValue.HealthValue -= 1;
        }
    }
}