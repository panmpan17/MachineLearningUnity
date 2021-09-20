using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


public class MovingPlatformSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        bool hasGround = false;
        Entities.WithStructuralChanges().ForEach((Entity entity, MovingPlatform platform, ref Translation translation) =>
        {
            hasGround = true;
            translation.Value.x -= platform.speed * deltaTime;
            if (translation.Value.x < -4) {
                translation.Value.x = 12;
                EntityManager.DestroyEntity(entity);
            }
        }).Run();

        SpawnGroundData spawnGroundData = GetSingleton<SpawnGroundData>();
        bool spawnGround = !hasGround;

        if (hasGround)
        {
            float rightestX = -10;
            Entities.WithStructuralChanges().ForEach((Entity entity, in MovingPlatform _movingPlatform, in Translation _translation) =>
            {
                if (_translation.Value.x > rightestX)
                {
                    rightestX = _translation.Value.x;
                }
            }).Run();

            spawnGround = (spawnGroundData.startPosition.x - rightestX) > spawnGroundData.xGap;
        }

        if (spawnGround)
        {
            Entity newGroundEntity = EntityManager.Instantiate(spawnGroundData.prefab);
            float3 position = new float3
            {
                x = spawnGroundData.startPosition.x,
                y = UnityEngine.Random.Range(spawnGroundData.yMin, spawnGroundData.yMax),
            };
            Translation groundTranslation = new Translation { Value = position };
            EntityManager.SetComponentData(newGroundEntity, groundTranslation);
        }
    }
}
