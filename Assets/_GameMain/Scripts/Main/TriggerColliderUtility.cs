using UnityEngine;

/// <summary>
/// Утилита для автоматического добавления триггерного коллайдера, чуть больше основного
/// </summary>
public static class TriggerColliderUtility
{
    /// <summary>
    /// Добавляет на объект дополнительный триггер-коллайдер, чуть больше основного.
    /// Если такой уже есть — не создаёт дубликат.
    /// </summary>
    /// <param name="gameObject">Объект с основным коллайдером</param>
    /// <param name="mainCollider">Основной коллайдер (НЕ триггер)</param>
    /// <param name="scale">Во сколько раз триггер больше основного (например, 1.15 = +15%)</param>
    public static void CreateTriggerCollider(GameObject gameObject, Collider mainCollider, float scale = 1.15f)
    {
        // Не создавать дубликаты
        foreach (var c in gameObject.GetComponents<Collider>())
        {
            if (c != mainCollider && c.isTrigger)
                return;
        }

        if (mainCollider is SphereCollider sphere)
        {
            var trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = sphere.radius * scale;
            trigger.center = sphere.center;
        }
        else if (mainCollider is BoxCollider box)
        {
            var trigger = gameObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center = box.center;
            trigger.size = box.size * scale;
        }
        else if (mainCollider is CapsuleCollider capsule)
        {
            var trigger = gameObject.AddComponent<CapsuleCollider>();
            trigger.isTrigger = true;
            trigger.center = capsule.center;
            trigger.radius = capsule.radius * scale;
            trigger.height = capsule.height * scale;
            trigger.direction = capsule.direction;
        }
        else
        {
            // Для MeshCollider и других типов — сферический триггер на bounds
            var trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = mainCollider.bounds.extents.magnitude * scale;
            trigger.center = mainCollider.bounds.center - gameObject.transform.position;
        }
    }
}
