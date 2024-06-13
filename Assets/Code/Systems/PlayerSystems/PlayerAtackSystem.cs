using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.Scripting;


namespace MSuhininTestovoe.B2B
{
    public class PlayerAtackSystem : EcsUguiCallbackSystem, IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _filterRayOn;
        private EcsFilter _filterRayOff;
        private EcsFilter _enemyFilter;
        private EcsPool<IsPlayerCanAttackComponent> _isCanAttackComponentPool;
        private EcsPool<HealthViewComponent> _enemyHealthViewComponentPool;
        private EcsPool<EnemyHealthComponent> _enemyHealthComponentPool;
        private EcsPool<RayComponent> _rayComponent;
        private int _entity;
        private readonly EcsCustomInject<AttackInputView> _attackInput = default;


        [Preserve]
        [EcsUguiClickEvent(UIConstants.BTN_ATACK, WorldsNamesConstants.EVENTS)]
        void OnClickPlayerAttack(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filterRayOn)
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
            _filterRayOn = world
                .Filter<IsPlayerComponent>()
                .Inc<RayComponent>()
                .Exc<IsPlayerCanAttackComponent>()
                .End();

            _filterRayOff = world
                .Filter<IsPlayerComponent>()
                .Inc<IsPlayerCanAttackComponent>()
                .Exc<RayComponent>()
                .End();


            _enemyFilter = world
                .Filter<EnemyHealthComponent>()
                .Inc<HealthViewComponent>()
                .End();
            _enemyHealthViewComponentPool = world.GetPool<HealthViewComponent>();
            _enemyHealthComponentPool = world.GetPool<EnemyHealthComponent>();
            _isCanAttackComponentPool = world.GetPool<IsPlayerCanAttackComponent>();
            _rayComponent = world.GetPool<RayComponent>();
        }


        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filterRayOn)
            {
                ref RayComponent rayComponent = ref _rayComponent.Get(entity);
                if (rayComponent.Value.transform.gameObject.GetComponent<EnemyActor>() != null)
                {
                    _attackInput.Value.RayAction?.Invoke(true);
                    _isCanAttackComponentPool.Add(entity);
                }
            }

            foreach (var entity in _filterRayOff)
            {
                if (_attackInput.Value.AttackBtn.gameObject.activeSelf) _attackInput.Value.RayAction?.Invoke(false);
                _isCanAttackComponentPool.Del(entity);
            }
        }

        private void Attack()
        {
            ref EnemyHealthComponent healthValue = ref _enemyHealthComponentPool.Get(_entity);
            healthValue.HealthValue -= 1;
        }
    }
}