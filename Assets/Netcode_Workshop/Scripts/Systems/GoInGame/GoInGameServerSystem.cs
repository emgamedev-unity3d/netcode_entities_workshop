using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.MegacityMetro.Gameplay;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

// When server receives go in game request, go in game and delete request
[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct GoInGameServerSystem : ISystem
{
    private ComponentLookup<NetworkId> networkIdFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerShipSpawner>();

        var builder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GoInGameRequest>()
            .WithAll<ReceiveRpcCommandRequest>();

        state.RequireForUpdate(state.GetEntityQuery(builder));

        networkIdFromEntity = state.GetComponentLookup<NetworkId>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // getting the prefab data from the CubeSpawner component
        Entity prefab = SystemAPI.GetSingleton<PlayerShipSpawner>().PlayerShipPrefab;
        state.EntityManager.GetName(prefab, out var prefabName);
        
        var worldName = new FixedString32Bytes(state.WorldUnmanaged.Name);

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        networkIdFromEntity.Update(ref state);

        foreach (var (reqSrc, reqEntity) in
            SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
                .WithAll<GoInGameRequest>()
                .WithEntityAccess())
        {
            commandBuffer.AddComponent<NetworkStreamInGame>(reqSrc.ValueRO.SourceConnection);
            NetworkId networkId = networkIdFromEntity[reqSrc.ValueRO.SourceConnection];

            Debug.Log(
                $"'{worldName}' setting connection '{networkId.Value}' to in game, spawning a Ghost '{prefabName}' for them!");

            var playerShip = InstantiatePlayerShip(ref commandBuffer, ref prefab, ref networkId);

            ApplySpawnPointPosition(ref state, ref prefab, ref commandBuffer, ref playerShip);

            // Add the player to the linked entity group so it is destroyed automatically on
            // disconnect
            commandBuffer.AppendToBuffer(
                reqSrc.ValueRO.SourceConnection,
                new LinkedEntityGroup { Value = playerShip });

            // Destroy the entity that started the requested of going into game
            commandBuffer.DestroyEntity(reqEntity);
        }
        commandBuffer.Playback(state.EntityManager);
    }

    private Entity InstantiatePlayerShip(
        ref EntityCommandBuffer entityCommandBuffer,
        ref Entity prefab,
        ref NetworkId networkId)
    {
        Entity player = entityCommandBuffer.Instantiate(prefab);
        entityCommandBuffer.SetComponent(
            player,
            new GhostOwner { NetworkId = networkId.Value });

        return player;
    }

    private void ApplySpawnPointPosition(
        ref SystemState state,
        ref Entity prefab,
        ref EntityCommandBuffer entityCommandBuffer,
        ref Entity playerShip)
    {
        var spawnPointBuffer = SystemAPI.GetSingletonBuffer<SpawnPointElement>();

        var originalTrans = state.EntityManager.GetComponentData<LocalTransform>(prefab);
        var newTrans = originalTrans;
        newTrans.Position = spawnPointBuffer.ElementAt(0).Value;
        newTrans.Position = spawnPointBuffer.ElementAt(0).Value;
        entityCommandBuffer.SetComponent(playerShip, newTrans);
    }
}