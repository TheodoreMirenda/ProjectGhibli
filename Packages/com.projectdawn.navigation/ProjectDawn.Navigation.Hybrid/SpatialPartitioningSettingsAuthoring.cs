using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectDawn.Navigation.Hybrid
{
    [AddComponentMenu("Agents Navigation/Settings/Spatial Partitioning Settings")]
    [DisallowMultipleComponent]
    [HelpURL("https://lukaschod.github.io/agents-navigation-docs/manual/authoring.html")]
    public class SpatialPartitioningSettingsAuthoring : SettingsBehaviour
    {
        [Tooltip("The size of single partition.")]
        [SerializeField]
        protected float3 CellSize = 3;

        [Tooltip("TODO")]
        [SerializeField]
        [HideInInspector]
        protected int QueryCapacity = 64;

        /// <summary>
        /// Returns default component of <see cref="AgentSpatialPartitioningSystem.Settings"/>.
        /// </summary>
        public AgentSpatialPartitioningSystem.Settings DefaultSettings => new AgentSpatialPartitioningSystem.Settings
        {
            CellSize = CellSize,
            QueryCapacity = QueryCapacity,
        };

        public override Entity GetOrCreateEntity()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var manager = world.EntityManager;
            return manager.CreateSingleton(DefaultSettings);
        }
    }

    internal class AgentSpatialBaker : Baker<SpatialPartitioningSettingsAuthoring>
    {
#if UNITY_ENTITIES_VERSION_65
        public override void Bake(SpatialPartitioningSettingsAuthoring authoring) => AddComponent(GetEntity(TransformUsageFlags.Dynamic), authoring.DefaultSettings);
#else
        public override void Bake(SpatialPartitioningSettingsAuthoring authoring) => AddComponent(authoring.DefaultSettings);
#endif
    }
}
