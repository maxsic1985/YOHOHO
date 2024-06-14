using LeoEcsPhysics;
using Leopotam.EcsLite;



namespace MSuhininTestovoe.B2B
{
    public  partial class TriggerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _filterEnterToTrigger;
        private EcsFilter _filterExitFromTrigger;
        

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filterEnterToTrigger = _world
                .Filter<OnTriggerEnter2DEvent>()
                .Exc<AIDestanationComponent>()
                .Exc<IsEnemyFollowingComponent>()
                .End();

            _filterExitFromTrigger = _world
                .Filter<OnTriggerExit2DEvent>()
                .End();

            _enemyIsFollowComponentPool = _world.GetPool<IsEnemyFollowingComponent>();
            _isPlayerCanAtackComponenPool = _world.GetPool<IsPlayerCanAttackComponent>();
            _isEnemyCanAtackComponenPool = _world.GetPool<AIPathComponent>();
            _enemyHealthViewComponentPool = _world.GetPool<HealthViewComponent>();
        }

        public  void Run(IEcsSystems ecsSystems)
        {
            var poolEnter = _world.GetPool<OnTriggerEnter2DEvent>();
            var poolExit = _world.GetPool<OnTriggerExit2DEvent>();

            EnemyEnterToTrigger(ecsSystems, poolEnter);
            EmnemyExitFromTRigger(poolExit);
        }
    }
}