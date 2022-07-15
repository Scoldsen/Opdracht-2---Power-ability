using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Linq;

public class TerrainGenerator : MonoBehaviour
{
    public Chunk[,,] chunks;
    public Material blockMaterial;

    [HideInInspector]
    public bool calcDiagonals = false;
    [HideInInspector]
    public bool calcSlopes = false;
    [HideInInspector]
    public bool calcSlopeCorners = false;
    [HideInInspector]
    public bool calcSlopeCornersConcave = false;

    public GameObject coinPrefab;

    private Bounds terrainBounds;
    private List<Level> levelPresets = new List<Level>();
    public Level currentLevel;

    public GameObject treePrefab;
    public GameObject[] treePrefabs;

    [SerializeField]
    private GameObject fallingRock;
    [SerializeField]
    private int nTopRocks = 50;

    private Transform levelGroupObject;
    public bool testRegenerate = false;
    [SerializeField]
    private Transform floorObject;

    private void FixedUpdate()
    {
        if (testRegenerate)
        {
            testRegenerate = false;
            GenerateLevelAsync();
        }
    }

    private void Awake()
    {
        LoadLevelPresets();
    }

    private void LoadLevelPresets()
    {
        var tmpLevelPresets = ResourceLoader.loadResources("Levels", typeof(Level));

        for (int i = 0; i < tmpLevelPresets.Length; i++)
        {
            var level = (Level)tmpLevelPresets[i];
            levelPresets.Add(level);
        }

        var shuffledList = levelPresets.OrderBy(x => UnityEngine.Random.value).ToList();
        levelPresets = shuffledList;
    }

    public void LoadNextLevel()
    {
        if (levelPresets.Count > 0)
        {
            // miss een queue van maken?
            currentLevel = levelPresets[0];
            levelPresets.RemoveAt(0);
            levelPresets.Add(currentLevel);
            terrainBounds = GetTerrainBounds();
        }
    }

    public Bounds GetTerrainBounds()
    {
        return new Bounds(currentLevel.GetCenterPosition(), currentLevel.nChunks * currentLevel.chunkDimensions);
    }

    public Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public async Task SetupChunksAsync()
    {
        var chunkGroupObj = ExtraFunctions.CreateGroupObject("Chunk");
        chunkGroupObj.SetParent(levelGroupObject);
        NoiseS3D.seed = 1;// UnityEngine.Random.Range(0, int.MaxValue);

        Block[,,] terrainData = new Block[currentLevel.chunkDimensions.x, currentLevel.chunkDimensions.y, currentLevel.chunkDimensions.z];

        chunks = new Chunk[currentLevel.nChunks.x, currentLevel.nChunks.y, currentLevel.nChunks.z];

        int nChunksTotal = currentLevel.nChunks.x * currentLevel.nChunks.y * currentLevel.nChunks.z;
        int currentChunkIndex = 0;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            await Task.Delay(1);
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    if (GameManager.Instance.IsCancelled()) return;

                    Vector3Int pos = new Vector3Int(x, y, z);
                    var ChunkGameObject = new GameObject();
                    ChunkGameObject.transform.SetParent(chunkGroupObj);
                    ChunkGameObject.transform.name = $"chunk {pos.x}, {pos.y}, {pos.z}";
                    ChunkGameObject.layer = 14;

                    var Chunk = ChunkGameObject.AddComponent<Chunk>();

                    Chunk.chunkPosition = pos;
                    Chunk.dimensions = currentLevel.chunkDimensions;
                    GetTerrain(ref Chunk);
                    Chunk.Init(blockMaterial);
                    chunks[pos.x, pos.y, pos.z] = Chunk;

                    UIManager.Instance.UpdateLoadBar("Setting up chunks", currentChunkIndex, nChunksTotal);
                    currentChunkIndex++;
                }
            }
        }

        await Task.Yield();
    }

    public async Task SetupNeighboursAsync()
    {
        int nChunksTotal = currentLevel.nChunks.x * currentLevel.nChunks.y * currentLevel.nChunks.z;
        int currentChunkIndex = 0;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            await Task.Delay(1);
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    if (GameManager.Instance.IsCancelled()) return;
                    Chunk chunk = GetChunkByChunkPosition(new Vector3Int(x, y, z));

                    foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                    {
                        chunk.neighBours.SetNeighbour(GetAdjacentChunk(chunk.chunkPosition, direction), direction);
                    }

                    chunk.SetupBlockNeighbours();
                    UIManager.Instance.UpdateLoadBar("Setting up neighbours", currentChunkIndex, nChunksTotal);
                    currentChunkIndex++;
                }
            }
        }
        await Task.Yield();
    }

    public async Task ResetChunkBlockShapesAsync()
    {
        int nChunksTotal = currentLevel.nChunks.x * currentLevel.nChunks.y * currentLevel.nChunks.z;
        int currentChunkIndex = 0;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            await Task.Delay(1);
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    if (GameManager.Instance.IsCancelled()) return;
                    Chunk chunk = GetChunkByChunkPosition(new Vector3Int(x, y, z));
                    chunk.ResetBlockShapes();
                    UIManager.Instance.UpdateLoadBar("Resetting block shapes", currentChunkIndex, nChunksTotal);
                    currentChunkIndex++;
                }
            }
        }
        await Task.Yield();
    }

    public async Task CalcDiagonalsAsync()
    {
        int nChunksTotal = currentLevel.nChunks.x * currentLevel.nChunks.y * currentLevel.nChunks.z;
        int currentChunkIndex = 0;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            await Task.Delay(1);
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    if (GameManager.Instance.IsCancelled()) return;
                    Chunk chunk = GetChunkByChunkPosition(new Vector3Int(x, y, z));
                    chunk.UpdateBlockShapesDiagonals();
                    UIManager.Instance.UpdateLoadBar("Calculating diagonal shapes", currentChunkIndex, nChunksTotal);
                    currentChunkIndex++;
                }
            }
        }
        await Task.Yield();
    }

    public async Task CalcSlopesAsync()
    {
        int nChunksTotal = currentLevel.nChunks.x * currentLevel.nChunks.y * currentLevel.nChunks.z;
        int currentChunkIndex = 0;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            await Task.Delay(1);
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    Chunk chunk = GetChunkByChunkPosition(new Vector3Int(x, y, z));
                    chunk.UpdateBlockShapesSlopes();
                    UIManager.Instance.UpdateLoadBar("Calculating slope shapes", currentChunkIndex, nChunksTotal);
                    currentChunkIndex++;
                }
            }
        }
        await Task.Yield();
    }

    public async Task CalcSlopeCornersAsync()
    {
        int nChunksTotal = currentLevel.nChunks.x * currentLevel.nChunks.y * currentLevel.nChunks.z;
        int currentChunkIndex = 0;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            await Task.Delay(1);
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    Chunk chunk = GetChunkByChunkPosition(new Vector3Int(x, y, z));
                    chunk.UpdateBlockShapesSlopeCorners();
                    UIManager.Instance.UpdateLoadBar("Calculating slope corner shapes", currentChunkIndex, nChunksTotal);
                    currentChunkIndex++;
                }
            }
        }
        await Task.Yield();
    }

    public async Task CalcSlopeCornersConcaveAsync()
    {
        int nChunksTotal = currentLevel.nChunks.x * currentLevel.nChunks.y * currentLevel.nChunks.z;
        int currentChunkIndex = 0;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            await Task.Delay(1);
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    Chunk chunk = GetChunkByChunkPosition(new Vector3Int(x, y, z));
                    chunk.UpdateBlockShapesSlopeCornersConcave();
                    UIManager.Instance.UpdateLoadBar("Calculating slope corner shapes 2", currentChunkIndex, nChunksTotal);
                    currentChunkIndex++;
                }
            }
        }
        await Task.Yield();
    }

    public async Task UpdateChunkModelsAsync(bool forced)
    {
        int nChunksTotal = currentLevel.nChunks.x * currentLevel.nChunks.y * currentLevel.nChunks.z;
        int currentChunkIndex = 0;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            await Task.Delay(1);
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    Chunk chunk = GetChunkByChunkPosition(new Vector3Int(x, y, z));
                    chunk.UpdateTerrainModel();
                    UIManager.Instance.UpdateLoadBar("Updating terrain models", currentChunkIndex, nChunksTotal);
                    currentChunkIndex++;
                }
            }
        }
        await Task.Yield();
    }


    public bool PostionIsOutOfBounds(Vector3 pos)
    {
        if (pos.x < -5 || pos.x > currentLevel.nChunks.x * currentLevel.chunkDimensions.x + 5) return true;
        if (pos.y < -20) return true;
        if (pos.z < -5 || pos.z > currentLevel.nChunks.z * currentLevel.chunkDimensions.z + 5) return true;

        return false;
    }
    
    private void CleanUp()
    {
        if (levelGroupObject != default)
        {
            Destroy(levelGroupObject.gameObject);
            chunks = null;
        }
    }

    private void ResizeFloor()
    {
        var pos = currentLevel.GetCenterPosition();
        var size = currentLevel.GetLevelSize();
        size.y = 1;
        size.x += 5;
        size.z += 5;
        pos.y = 0;
        floorObject.transform.position = pos;
        floorObject.transform.localScale = size;
    }

    public async Task GenerateLevelAsync()
    {
        CleanUp();

        levelGroupObject = ExtraFunctions.CreateGroupObject("Level");

        LoadNextLevel();
        ResizeFloor();
        await SetupChunksAsync();
        await SetupNeighboursAsync();
        await UpdateBlockShapesAsync();
        await UpdateChunkModelsAsync(true);
        //AddCoins(currentLevel.nCoins);
        AddTrees(currentLevel.nTrees);
        var obj = FindObjectOfType<WinTrigger>();
        obj.ReshapeWinCollider();
    }

    /// <summary>
    /// Vervanging voor telkens 3 genestelde for loops in de code
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="funcName"></param>
    /// <param name="action"></param>
    public void TerrainLoop(out Vector3Int pos, string funcName, Action action)
    {
        pos = Vector3Int.zero;
        float startTime = Time.realtimeSinceStartup;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    pos = new Vector3Int(x, y, z);
                    action();
                }
            }
        }

        if (funcName != "") Debug.Log($"{funcName} duurde {Time.realtimeSinceStartup - startTime} seconden");
    }

    public void TerrainLoop(out Chunk chunk, string funcName, Action action)
    {
        chunk = default;
        float startTime = Time.realtimeSinceStartup;

        for (int x = 0; x < currentLevel.nChunks.x; x++)
        {
            for (int y = 0; y < currentLevel.nChunks.y; y++)
            {
                for (int z = 0; z < currentLevel.nChunks.z; z++)
                {
                    chunk = GetChunkByChunkPosition(new Vector3Int(x, y, z));
                    action();
                }
            }
        }
        if (funcName != "") Debug.Log($"{funcName} duurde {Time.realtimeSinceStartup - startTime} seconden");
    }

    public void TerrainLoop(out Vector3Int pos, Action action)
    {
        pos = Vector3Int.zero;
        TerrainLoop(out pos, "", action);
    }

    /// <summary>
    /// Reset de vorm van alle blokken in elke chunk zodat deze opnieuw bepaald kan worden
    /// </summary>
    private void ResetChunkBlockShapes()
    {
        Chunk chunk = default;
        TerrainLoop(out chunk, "ResetChunkBlockShapes", delegate { chunk.ResetBlockShapes(); });
    }

    private void ResetChunkBlockShapes(List<Chunk> chunkList)
    {
        foreach (var chunk in chunkList)
        {
            chunk.ResetBlockShapes();
        }
    }

    public async Task UpdateBlockShapesAsync()
    {
        await ResetChunkBlockShapesAsync();
        if (calcDiagonals) await CalcDiagonalsAsync();
        if (calcSlopes) await CalcSlopesAsync();
        if (calcSlopeCorners) await CalcSlopeCornersAsync();
        if (calcSlopeCornersConcave) await CalcSlopeCornersConcaveAsync();

        await Task.Yield();
    }

    /// <summary>
    /// Werkt de vorm van alle blokken bij in elke chunk
    /// </summary>
    private void UpdateChunkBlockShapes()
    {
        //Vector3Int pos = Vector3Int.zero;
        Chunk chunk = default;

        if (calcDiagonals)
        {
            TerrainLoop(out chunk, "CalcDiagonals", delegate { chunk.UpdateBlockShapesDiagonals(); });
        }

        if (calcSlopes)
        {
            TerrainLoop(out chunk, "CalcSlopes", delegate { chunk.UpdateBlockShapesSlopes(); });
        }

        if (calcSlopeCorners)
        {
            TerrainLoop(out chunk, "CalcSlopeCorners", delegate { chunk.UpdateBlockShapesSlopeCorners(); });
        }

        if (calcSlopeCornersConcave)
        {
            TerrainLoop(out chunk, "CalcSlopeCornersConcave", delegate { chunk.UpdateBlockShapesSlopeCornersConcave(); });
        }

        chunk = null;
        TerrainLoop(out chunk, "UpdateTerrainModifiedBool", delegate { chunk.UpdateTerrainModifiedBool(); });
    }

    private void UpdateChunkBlockShapes(List<Chunk> chunkList)
    {
        //Chunk chunk = default;

        if (calcDiagonals)
        {
            foreach (var chunk in chunkList)
            {
                chunk.UpdateBlockShapesDiagonals();
            }
        }

        if (calcSlopes)
        {
            foreach (var chunk in chunkList)
            {
                chunk.UpdateBlockShapesSlopes();
            }
        }

        if (calcSlopeCorners)
        {
            foreach (var chunk in chunkList)
            {
                chunk.UpdateBlockShapesSlopeCorners();
            }
        }

        if (calcSlopeCornersConcave)
        {
            foreach (var chunk in chunkList)
            {
                chunk.UpdateBlockShapesSlopeCornersConcave();
            }
        }

        foreach (var chunk in chunkList)
        {
            chunk.UpdateTerrainModifiedBool();
        }
    }

    /// <summary>
    /// Werkt het terreinmodel bij van iedere aangepaste chunk
    /// </summary>
    private void UpdateChunkModels(bool forced)
    {
        Vector3Int pos = Vector3Int.zero;
        TerrainLoop(out pos, "UpdateTerrainModel", delegate
        {
            Chunk chunk = GetChunkByChunkPosition(pos);
            if (chunk.shouldUpdateTerrainModel || forced)
            {
                chunk.shouldUpdateTerrainModel = false;
                chunk.UpdateTerrainModel();
            }
        });
    }

    private void UpdateChunkModels(List<Chunk> chunkList)
    {
        foreach (var chunk in chunkList)
        {
            if (chunk.shouldUpdateTerrainModel)
            {
                chunk.shouldUpdateTerrainModel = false;
                chunk.UpdateTerrainModel();
            }
        }
    }

    /// <summary>
    /// Maakt chunks aan en stelt ze in
    /// </summary>
    private void SetupChunks()
    {
        Block[,,] terrainData = new Block[currentLevel.chunkDimensions.x, currentLevel.chunkDimensions.y, currentLevel.chunkDimensions.z];

        chunks = new Chunk[currentLevel.nChunks.x, currentLevel.nChunks.y, currentLevel.nChunks.z];

        Vector3Int pos = Vector3Int.zero;
        TerrainLoop(out pos, delegate
        {
            var ChunkGameObject = new GameObject();
            ChunkGameObject.transform.SetParent(transform);
            ChunkGameObject.transform.name = $"chunk {pos.x}, {pos.y}, {pos.z}";

            var Chunk = ChunkGameObject.AddComponent<Chunk>();
            Chunk.chunkPosition = pos;
            Chunk.dimensions = currentLevel.chunkDimensions;
            GetTerrain(ref Chunk);
            Chunk.Init(blockMaterial);
            chunks[pos.x, pos.y, pos.z] = Chunk;
        });
    }

    /// <summary>
    /// Stelt terreindata in en referentie van chunk in ieder blok
    /// </summary>
    /// <param name="chunk"></param>
    private void GetTerrain(ref Chunk chunk)
    {
        Block[,,] terrainData = new Block[currentLevel.chunkDimensions.x, currentLevel.chunkDimensions.y, currentLevel.chunkDimensions.z];

        Vector3Int offset = chunk.chunkPosition;
        Vector3 chunkZeroPos = new Vector3(offset.x * currentLevel.chunkDimensions.x, offset.y * currentLevel.chunkDimensions.y, offset.z * currentLevel.chunkDimensions.z);
        float totalWidth = currentLevel.nChunks.x * currentLevel.chunkDimensions.x / 2;

        float curveMinXValue = currentLevel.mountainShape.keys[0].time;
        float curveMaxXValue = currentLevel.mountainShape.keys[currentLevel.mountainShape.keys.Length - 1].time;
        float maxYvalue = currentLevel.chunkDimensions.y * currentLevel.nChunks.y;

        for (int x = 0; x < currentLevel.chunkDimensions.x; x++)
        {
            for (int y = 0; y < currentLevel.chunkDimensions.y; y++)
            {
                float realYValue = currentLevel.chunkDimensions.y * offset.y + y;

                for (int z = 0; z < currentLevel.chunkDimensions.z; z++)
                {
                    var mapTerrainMiddlePos = new Vector3(0.5f * currentLevel.nChunks.x * currentLevel.chunkDimensions.x, chunkZeroPos.y + y, 0.5f * currentLevel.nChunks.z * currentLevel.chunkDimensions.z);
                    var currentPos = new Vector3(chunkZeroPos.x + x, chunkZeroPos.y + y, chunkZeroPos.z + z);

                    float radT = Mathf.InverseLerp(0, maxYvalue, Mathf.FloorToInt(currentPos.y));
                    float animT = Mathf.Lerp(curveMinXValue, curveMaxXValue, radT);

                    float bottomWidth = currentLevel.mountainShape.Evaluate(curveMinXValue);
                    float topWidth = currentLevel.mountainShape.Evaluate(curveMaxXValue);

                    float minWidth = topWidth < bottomWidth ? topWidth : bottomWidth;
                    float maxWidth = minWidth == topWidth ? bottomWidth : topWidth;

                    float topScaleFactor = maxWidth / minWidth;
                    float scaleFactor = Mathf.Lerp(1, topScaleFactor, radT);

                    var sscale = currentLevel.noiseScale;
                    // kijken of ik dit werkend kan krijgen zodat je niet meer zo vaak te grote gaten bovenaan hebt
                    // en daardoor niet verder kan klimmen
                    //if (dynamicScale) sscale = scaleFactor * scale;

                    float xVal = (x + offset.x * currentLevel.chunkDimensions.x) * sscale;
                    float yVal = (y + offset.y * currentLevel.chunkDimensions.y) * sscale;
                    float zVal = (z + offset.z * currentLevel.chunkDimensions.z) * sscale;
                    var noiseVal = NoiseS3D.NoiseCombinedOctaves(xVal, yVal, zVal);
                    uint result = 0;

                    int radius = (int)(currentLevel.mountainShape.Evaluate(animT) * totalWidth);

                    int dist = (int)Mathf.Abs(Vector3.Distance(mapTerrainMiddlePos, currentPos));
                    if (dist <= radius)
                    {
                        result = (uint)Mathf.Max(2, (uint)Mathf.FloorToInt((float)noiseVal * 10));
                        if (currentLevel.solidBounds)
                        {
                            //zijkant
                            if (Mathf.Abs(dist - radius) <= currentLevel.shellThickness) result = 1;
                            //bovenkant
                            if (currentPos.y == maxYvalue - 1 || currentPos.y == 0) result = 1;
                        }
                    }

                    BlockShape shape = result >= 1 ? BlockShape.FULL : BlockShape.EMPTY;

                    Vector3Int blockPosition = new Vector3Int(x, y, z);
                    Block block = new Block(blockPosition, shape, result);
                    block.chunk = chunk;
                    terrainData[x, y, z] = block;
                }
            }
        }

        chunk.terrainData = terrainData;
    }

    /// <summary>
    /// Controleert of er een chunk bestaat op de opgegeven positie
    /// </summary>
    /// <param name="chunkPosition"></param>
    /// <returns></returns>
    private bool IsValidChunkPosition(Vector3Int chunkPosition)
    {
        return (chunkPosition.x >= 0 && chunkPosition.x < currentLevel.nChunks.x &&
                chunkPosition.y >= 0 && chunkPosition.y < currentLevel.nChunks.y &&
                chunkPosition.z >= 0 && chunkPosition.z < currentLevel.nChunks.z);
    }

    public Chunk GetChunkByChunkPosition(Vector3Int chunkPosition)
    {
        if (IsValidChunkPosition(chunkPosition)) return chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z];
        return null;
    }

    public Chunk GetAdjacentChunk(Vector3Int chunkPosition, Direction direction)
    {
        var targetChunkPos = chunkPosition + GameData.directions[direction];
        return GetChunkByChunkPosition(targetChunkPos);
    }

    private void SetupNeighbours()
    {
        Vector3Int pos = Vector3Int.zero;

        TerrainLoop(out pos, "SetupBlockNeighbours", delegate
        {
            Chunk chunk = GetChunkByChunkPosition(pos);
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                chunk.neighBours.SetNeighbour(GetAdjacentChunk(chunk.chunkPosition, direction), direction);
            }

            chunk.SetupBlockNeighbours();
        });
    }

    public void ModifyTerrainAtPosition(Vector3Int chunkPosition, Vector3Int blockPosition, bool dig)
    {
        var chunk = GetChunkByChunkPosition(chunkPosition);
        if (chunk != null)
        {
            if (chunk.IsValidBlockPosition(blockPosition))
            {
                var block = chunk.GetBlockAt(blockPosition);
                if (block != null)
                {
                    block.blockShape = dig ? BlockShape.EMPTY : BlockShape.FULL;
                    RegenerateChunk(chunk);
                }
            }
        }
    }

    private void RegenerateChunk(Chunk chunk)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();

        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            Chunk tmpChunk = chunk.neighBours.GetNeighbour(direction);
            if (tmpChunk != null) chunksToUpdate.Add(tmpChunk);
        }

        ResetChunkBlockShapes(chunksToUpdate);
        UpdateChunkBlockShapes(chunksToUpdate);
        UpdateChunkModels(chunksToUpdate);
    }

    public void AddCoins(int nCoins)
    {   
        float yPosMax = currentLevel.GetTop();
        var coinGroupObj = ExtraFunctions.CreateGroupObject("Coin");
        coinGroupObj.SetParent(levelGroupObject);

        for (int i = 0; i < nCoins; i++)
        {
            Vector3 rayOriginPoint = GetRandomPointInBounds(terrainBounds);
            rayOriginPoint.y = yPosMax;

            RaycastHit hit;
            if (Physics.Raycast(rayOriginPoint, Vector3.down, out hit, terrainBounds.size.y))
            {
                var coin = Instantiate(coinPrefab);
                coin.transform.SetParent(coinGroupObj.transform);
                coin.transform.localScale *= 2;
                coin.transform.position = hit.point + Vector3.up * UnityEngine.Random.Range(0.1f, 1);
            }
        }
    }

    public void AddTrees(int nTrees)
    {   
        float yPosMax = currentLevel.GetTop();
        var treeGroupObj = ExtraFunctions.CreateGroupObject("Tree");
        treeGroupObj.SetParent(levelGroupObject);

        for (int i = 0; i < nTrees; i++)
        {
            Vector3 rayOriginPoint = GetRandomPointInBounds(terrainBounds);
            rayOriginPoint.y = yPosMax;

            RaycastHit hit;
            if (Physics.Raycast(rayOriginPoint, Vector3.down, out hit, terrainBounds.size.y) && hit.point.y > 1)
            {
                var prefab = ExtraFunctions.GetRandomObject(treePrefabs);
                float scaleModifier = UnityEngine.Random.Range(0.5f, 2);
                float xRotationModifier = UnityEngine.Random.Range(-20, 20);
                float yRotationModifier = UnityEngine.Random.Range(-180, 180);

                var tree = Instantiate(prefab);
                var treeTransform = tree.transform;
                treeTransform.Rotate(tree.transform.up, yRotationModifier);
                treeTransform.Rotate(tree.transform.right, xRotationModifier);

                treeTransform.SetParent(treeGroupObj.transform);
                treeTransform.localScale *= scaleModifier;
                treeTransform.position = hit.point;
            }
        }
    }

    public void GenerateTopFallingBlocks()
    {
        Vector3Int chunkDims = currentLevel.chunkDimensions;
        Vector3Int nChunks = currentLevel.nChunks;
        var RockGroupObj = ExtraFunctions.CreateGroupObject("FallingRock");
        RockGroupObj.SetParent(levelGroupObject);

        for (int i = 0; i < nTopRocks; i++)
        {
            float xVal = UnityEngine.Random.Range(0, chunkDims.x * nChunks.x);
            float zVal = UnityEngine.Random.Range(0, chunkDims.z * nChunks.z);
            var offSet = new Vector3(xVal, nChunks.y * chunkDims.y + 10, zVal);
            var rock = Instantiate(fallingRock);
            var r = rock.GetComponent<FallingRock>();
            r.PointToCenter = true;
            r.centerPos = currentLevel.GetCenterPosition();
            rock.transform.position = offSet;
            rock.transform.SetParent(RockGroupObj);
        }
    }
}
