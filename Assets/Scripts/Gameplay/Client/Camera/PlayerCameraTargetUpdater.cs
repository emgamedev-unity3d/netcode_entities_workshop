using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.MegacityMetro.Gameplay;
using Unity.MegacityMetro.UI;
using Unity.NetCode;
using Unity.Transforms;

namespace Unity.MegacityMetro.CameraManagement
{
    /// <summary>
    /// Updates player camera target position and rotation
    /// </summary>
    [BurstCompile]
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation)]
    public partial struct PlayerCameraTargetUpdater : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerCameraTarget>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            ComponentLookup<LocalToWorld> localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);

            foreach (var (laser, localTransform, ltw, camTarget) in SystemAPI.Query<RefRO<VehicleLaser>, RefRO<LocalTransform>, RefRO<LocalToWorld>, RefRO<PlayerCameraTarget>>().WithAll<GhostOwnerIsLocal>())
            {
                if (!HybridCameraManager.Instance.WasInitialized)
                {
                    HybridCameraManager.Instance.PlaceCamera(localTransform.ValueRO.Position, localTransform.ValueRO.Rotation);
                    HybridCameraManager.Instance.WasInitialized = true;
                }

                HybridCameraManager.Instance.SetPlayerTargetPosition(ltw.ValueRO.Position + (math.up() * camTarget.ValueRO.VerticalOffset));
                HybridCameraManager.Instance.UpdateAimCameraTargetPosition(laser.ValueRO.CalculateLaserEndPoint(ltw.ValueRO.Position, ltw.ValueRO.Rotation, ref localToWorldLookup, true));

                HybridCameraManager.Instance.CameraUpdate(deltaTime);
            }
        }
    }
}
