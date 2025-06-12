using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NpcController : MonoBehaviour
{
    [SerializeField] private List<GameObject> npcPrefabs;

    private readonly Dictionary<ScaredNPC, Transform> _npcDictionary = new Dictionary<ScaredNPC, Transform>();
    private readonly List<NPCSpawnPoint> _spawnPoints = new ();
    private FoodSpawner _foodSpawner;
    
    public void RegisterSpawnPoint(NPCSpawnPoint point)
    {
        if (!_spawnPoints.Contains(point))
            _spawnPoints.Add(point);
    }

    public void UnregisterSpawnPoint(NPCSpawnPoint point)
    {
        _spawnPoints.Remove(point);
    }
    
    public void Initialize()
    {
        foreach (var npcSpawnPos in _spawnPoints)
        {
            var prefab = npcPrefabs[Random.Range(0, npcPrefabs.Count)];
            var npc = Instantiate(prefab, npcSpawnPos.transform.position, Quaternion.identity);
            var npcScript = npc.GetComponent<ScaredNPC>();
            _npcDictionary.Add(npcScript, npcSpawnPos.transform);
            npc.SetActive(false);
        }
    }

    public void SpawnNpc()
    {
        foreach (var (scaredNpc, posTransform) in _npcDictionary)
        {
            scaredNpc.transform.position = posTransform.position;
            scaredNpc.gameObject.SetActive(true);
            scaredNpc.Initialize(_foodSpawner);
        }
    }

    // _____________ Private _____________
    
    [Inject]
    private void Construct(FoodSpawner foodSpawner)
    {
        _foodSpawner = foodSpawner;
    }
    
    
}
