using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleGame.Client.Game.Dungeon;

public sealed class DungeonRunController
{
    private readonly DungeonDefinition _definition;
    private readonly Dictionary<string, DungeonMonsterPrefab> _prefabs;
    private readonly Queue<DungeonSpawnRequest> _spawnQueue = new();
    private readonly HashSet<string> _aliveSpawnTokens = new(StringComparer.OrdinalIgnoreCase);

    private bool _portalTriggered;
    private bool _bossPhase;
    private int _waveIndex;
    private bool _waveSpawned;
    private bool _isCompleted;
    private int _spawnTokenCounter;

    public DungeonRunController(DungeonDefinition definition, IEnumerable<DungeonMonsterPrefab> prefabs)
    {
        _definition = definition;
        _prefabs = prefabs.ToDictionary(p => p.PrefabId, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsCompleted => _isCompleted;
    public bool PortalTriggered => _portalTriggered;

    public void Update(float playerX)
    {
        if (_isCompleted)
            return;

        if (!_portalTriggered)
        {
            if (playerX >= _definition.PortalX)
                _portalTriggered = true;
            else
                return;
        }

        var currentWaves = _bossPhase ? _definition.BossWaves : _definition.Waves;
        if (_waveIndex >= currentWaves.Count)
        {
            if (!_bossPhase && _definition.BossWaves.Count > 0)
            {
                _bossPhase = true;
                _waveIndex = 0;
                _waveSpawned = false;
                return;
            }

            _isCompleted = true;
            return;
        }

        if (!_waveSpawned)
        {
            EnqueueWaveSpawns(currentWaves[_waveIndex], _bossPhase);
            _waveSpawned = true;
            return;
        }

        if (_aliveSpawnTokens.Count == 0)
        {
            _waveIndex++;
            _waveSpawned = false;
        }
    }

    public bool TryDequeueSpawn(out DungeonSpawnRequest request)
    {
        if (_spawnQueue.Count == 0)
        {
            request = null!;
            return false;
        }

        request = _spawnQueue.Dequeue();
        return true;
    }

    public void MarkSpawnDefeated(string spawnToken)
    {
        _aliveSpawnTokens.Remove(spawnToken);
    }

    private void EnqueueWaveSpawns(DungeonWaveDefinition wave, bool isBoss)
    {
        foreach (var group in wave.Spawns)
        {
            if (!_prefabs.TryGetValue(group.PrefabId, out var prefab))
                continue;

            if (group.Count <= 0)
                continue;

            List<float> positions = group.SpawnXs.Count > 0
                ? group.SpawnXs
                : new List<float> { _definition.PortalX + 350f };

            for (int i = 0; i < group.Count; i++)
            {
                float x = positions[i % positions.Count];
                string token = $"spawn_{++_spawnTokenCounter}";
                _aliveSpawnTokens.Add(token);
                _spawnQueue.Enqueue(new DungeonSpawnRequest
                {
                    PrefabId = prefab.PrefabId,
                    CharacterId = prefab.CharacterId,
                    X = x,
                    Y = group.SpawnY,
                    IsBoss = isBoss,
                    WaveId = wave.WaveId,
                    SpawnToken = token
                });
            }
        }
    }
}
