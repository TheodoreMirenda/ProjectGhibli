using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectDawn.Navigation.Hybrid
{
    [AddComponentMenu("Agents Navigation/Settings/Nav Mesh Settings")]
    [DisallowMultipleComponent]
    [HelpURL("https://lukaschod.github.io/agents-navigation-docs/manual/authoring.html")]
    public class NavMeshSettingsAuthoring : SettingsBehaviour
    {
        [Tooltip("TODO")]
        [SerializeField]
        protected int m_MaxIterations = 1024;

        [Tooltip("TODO")]
        [SerializeField]
        protected int m_MaxPath = 1024;

        /// <summary>
        /// Returns default component of <see cref="AgentSpatialPartitioningSystem.Settings"/>.
        /// </summary>
        public NavMeshQuerySystem.Settings DefaultSettings => new()
        {
            MaxIterations = m_MaxIterations,
            MaxPathSize = m_MaxPath,
        };

        public override Entity GetOrCreateEntity()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var manager = world.EntityManager;
            return manager.CreateSingleton(DefaultSettings);
        }
    }

    internal class NavMeshSettingsBaker : Baker<NavMeshSettingsAuthoring>
    {
#if UNITY_ENTITIES_VERSION_65
        public override void Bake(NavMeshSettingsAuthoring authoring) => AddComponent(GetEntity(TransformUsageFlags.Dynamic), authoring.DefaultSettings);
#else
        public override void Bake(NavMeshSettingsAuthoring authoring) => AddComponent(authoring.DefaultSettings);
#endif
    }
}
