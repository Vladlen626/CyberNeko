using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NpcController : MonoBehaviour
{
    [SerializeField] private Transform[] _npcSpawnPos;
    [SerializeField] private List<GameObject> _npcPrefabs;

    private Dictionary<ScaredNPC, Transform> _npcDictionary = new Dictionary<ScaredNPC, Transform>();
    private FoodSpawner _foodSpawner;
    
    public void Initialize()
    {
        foreach (var npcSpawnPos in _npcSpawnPos)
        {
            var prefab = _npcPrefabs[Random.Range(0, _npcPrefabs.Count)];
            var npc = Instantiate(prefab, npcSpawnPos.position, Quaternion.identity);
            var npcScript = npc.GetComponent<ScaredNPC>();
            _npcDictionary.Add(npcScript, npcSpawnPos);
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
