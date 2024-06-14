using Leopotam.EcsLite;

namespace MSuhininTestovoe.B2B
{


    public sealed class SoundSystems
    {
        public SoundSystems(EcsSystems systems)
        {
            systems
                .Add(new SoundSystem())
                .Add(new SoundInitSystem())
                .Add(new SoundBuildSystem());

        }
    }
}