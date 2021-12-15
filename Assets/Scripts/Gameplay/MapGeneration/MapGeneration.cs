using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Tilemaps;
using Voronoi2;
using Random = UnityEngine.Random;

public class MapGeneration : MonoBehaviour
{
    [Header("Preferences")] 
    [SerializeField] private Transform mapCameraTransform;
    private Camera _mapCamera; 
    [Space]
    [SerializeField] private Tilemap ground;

    private int[,] voronoiMap; // Линии пересечений рисуются тут (заполнение уже на самой tilemap) /// по факту полые клетки 

    [Serializable]
    public class MapBiome
    {
        public Biomes biomeType;
        public List<EnemyTypes> enemies = new List<EnemyTypes>();
        public Tile[] tiles;

        [MinMax(0f, 1f)] public float min = 0;
        [MinMax(0f, 1f)] public float max = 1;
        
        // TODO: walk sound effect name here i think
    }
    [SerializeField] private List<MapBiome> biomes = new List<MapBiome>();
    [SerializeField] private float biomesPerlinScaler = 10;

    // TODO: delete
    [SerializeField] private Tile[] tiles;

    [Space]
    private Voronoi voroObject = new Voronoi(0.1f);
    private List<GraphEdge> ge;
    private List<Vector2> sites;

    [Header("Settings")]
    
    [Range(0.0f, 100.0f)] [SerializeField]
    // Perlin noise
    private float scaler = 15f;
    [SerializeField] private bool isEditMode = true;
    [Range(200, 500)]
    [SerializeField] private int siteCount = 200;
    [Range(0, 0.9f)]
    [SerializeField] private float minSizeBetweenPoints = 0.3f;
    [Range(256, 1024)]
    [SerializeField] private int mapSize = 512;
    
    [Space]
    [Range(0, 1.0f)]
    [SerializeField] private float fillRadius = 0.8f;
    [Range(0, 1f)]
    [SerializeField] private float cuteRadius = 0.8f;
    [Range(0, 1f)]
    [SerializeField] private float chanceToSaveBorder= 0.9f;
    
    [Space]
    [SerializeField] private int __devSeed = 0;

    private float FillRadius => (fillRadius * mapSize / 2);
    private Vector3Int MapCenter => new Vector3Int((int)((float)mapSize / 2), (int)((float)mapSize / 2), 0);


    // Objects Distribution
    [Space] [Header("Objects and structures")]
    [SerializeField] private GameObject spawnPlace;
    [Serializable]
    public class MapObject
    {
        [Range(1f, 10f)]
        public float distanceFromOther = 1.5f;
        public bool useLimit = false;
        [UnityEngine.Min(0)] public int limit = 1;
        [Range(0f, 1f)] public float createChance = 0.5f;
        public GameObject[] prefabs;
        [Space]
        [Range(0.0f, 1.0f)] public float minHeight = 0;
        [Range(0.0f, 1.0f)] public float maxHeight = 0;
    }
    [SerializeField] private LayerMask objectsLayerMask;
    [SerializeField] private List<MapObject> mapObjects = new List<MapObject>();

    private void Awake()
    {
        _mapCamera = mapCameraTransform.GetComponent<Camera>();
        GenerateNewMap();
        
        // DELETE NEXT LINES AFTER
        seedTxt.text = "Seed: " + __devSeed;
        mapSizeTxt.text = mapSize.ToString();
        sitesCountTxt.text = siteCount.ToString();
        showParams.SetActive(false);
    }

    /*TEMPORARY-------------------------------------------------------*/
    public GameObject showParams;
    public TextMeshProUGUI seedTxt;
    public TextMeshProUGUI mapSizeTxt;
    public TextMeshProUGUI sitesCountTxt;
    public PerlinNoiseVisualizer visualizer;
    public CameraFollow cameraFollow;
    public void IncSeed()
    {
        __devSeed++;
        seedTxt.text = "Seed: " + __devSeed;
    }

    public void DecrSeed()
    {
        __devSeed--;
        seedTxt.text = "Seed: " + __devSeed;
    }

    public void SetMapSize(Single size)
    {
        mapSize = (int)size;
        mapSizeTxt.text = mapSize.ToString();
    }

    public void SetSitesCount(Single count)
    {
        siteCount = (int)count;
        sitesCountTxt.text = siteCount.ToString();
    }
    
    public void Regenerate()
    {
        GenerateNewMap();
        cameraFollow.ResetPosition();
    }
    /*TEMPORARY*/
    
    
    /// <summary>
    /// For development
    /// </summary>
    private void HandleDevButtons()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateNewMap();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            __devSeed--;
            GenerateNewMap();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            __devSeed++;
            GenerateNewMap();
            return;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.LogError("ERORR - P");
            Debug.Break();
        }
    }

    private void Update()
    {
        // DELETE LATER
        if (Input.GetKeyDown(KeyCode.P))
        {
            showParams.SetActive(!showParams.activeSelf);
        }

// #if DEBUG
        // HandleDevButtons();
// #endif
    }
    
    /// <summary>
    /// Generate map with all placed properties
    /// </summary>
    private void GenerateNewMap()
    {
        // visualizer.RegenerateTexture(mapSize, mapSize, scaler, 0, 0); // DEMO ONLY
        
        ClearMap(); // Clearing objects & tileset
        SpreadPoints(); // Voronoi
        
        FillVoronoiGhostMap();
        FillTilesInRadius();
        FillEmpty(); FillEmpty(); // TODO: fix (есть места с двойными дырами)
        CreateSpawnPoint();
        ObjectsDistribution();

        mapCameraTransform.transform.position = new Vector3(MapCenter.x, MapCenter.y, -100);
        _mapCamera.orthographicSize = mapSize / 2.5f;
    }

    private void CreateSpawnPoint()
    {
        // Спавн поинт
        var spawnPos = new Vector2(MapCenter.x, MapCenter.y) + Random.insideUnitCircle * 5;
        Instantiate(spawnPlace, spawnPos, Quaternion.identity, transform);
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = spawnPos;
        }
    }

    /// <summary>
    /// Clearing map objects and tiles
    /// </summary>
    private void ClearMap()
    {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        ground.ClearAllTiles();
    }

    /// <summary>
    /// Making points from random placed sites
    /// </summary>
    /// <returns>Voronoi graph</returns>
    private List<GraphEdge> MakeVoronoiGraph(List<Vector2> sites, int width, int height)
    {
        var xVal = new double[sites.Count];
        var yVal = new double[sites.Count];
        for (var i = 0; i < sites.Count; i++)
        {
            xVal[i] = sites[i].x;
            yVal[i] = sites[i].y;
        }
        return voroObject.generateVoronoi(xVal, yVal, 0, width, 0, height);

    }
    
    private void SpreadPoints()
    {
        sites = new List<Vector2>();
        // var seed = Random.Range(0, 999999999);;
// #if DEBUG
        var seed = __devSeed;
// #endif
        Random.InitState(seed);
        Debug.Log("SEED: " + seed);
        
        // Calculating small cube size
        var area = (mapSize * mapSize) / (float)siteCount;
        var areaLen = Mathf.Sqrt(area);
        for (float y = 0; y < mapSize - areaLen; y += areaLen)
        {
            var slideY = false;
            for (float x = 0; x < mapSize - areaLen; x += areaLen)
            {
                // Calculating circle inside small cube and putting random point                
                var squareCenterX = x + areaLen / 2;
                var squareCenterY = y + (slideY ? areaLen / 2 : areaLen);
                var radius = areaLen * minSizeBetweenPoints;
                slideY = !slideY;
                var point = Random.insideUnitCircle * radius;
                sites.Add(new Vector2((float)(point.x + squareCenterX), (float)(point.y + squareCenterY)));        
            }
        }
        
        ge = MakeVoronoiGraph(sites, mapSize, mapSize);
    }

    /// <summary>
    /// Makes ghost tilemap to restrict the cells of the map 
    /// </summary>
    private void FillVoronoiGhostMap()
    {
        voronoiMap = new int[mapSize + 64, mapSize + 64];
        for (var i = 0; i < ge.Count; i++)
        {
            var p1 = new Vector3Int((int)ge[i].x1, (int)ge[i].y1, 0);
            var p2 = new Vector3Int((int)ge[i].x2, (int)ge[i].y2, 0);
            VoronoiMapLine(p1, p2, ref voronoiMap);
        }
    }

    /// <summary>
    /// Lines between points for ghost voronoi map
    /// </summary>
    private void VoronoiMapLine(Vector3Int p1, Vector3Int p2, ref int[,] map)
    {
        while (p1.x != p2.x || p1.y != p2.y)
        {
            map[p1.x, p1.y] = 1;

            if (p1.x < p2.x)
                p1.x++;
            else if (p1.x > p2.x)
                p1.x--;

            if (p1.y < p2.y)
                p1.y++;
            else if (p1.y > p2.y)
                p1.y--;
        }
    }
    
    /// <summary>
    /// Filling restricted by circle radius map 
    /// </summary>
    private void FillTilesInRadius()
    {
        var perlinPointsByBiom = new List<Vector3Int>[biomes.Count];
        var offsetX = 0; //Random.Range(0, 10); // TODO: вернуть (only for demo)
        var offsetY = 0; //Random.Range(0, 10);
        for (var i = 0; i < biomes.Count; i++)
        {
            perlinPointsByBiom[i] = (CalcNoise(biomes[i].min, biomes[i].max, offsetX, offsetY, biomesPerlinScaler));
        }
        
        for (var i = 0; i < sites.Count; i++)
        {
            if (PointInCircleByRadius(sites[i], FillRadius))
            {
                if (!PointInCircleByRadius(sites[i], FillRadius * cuteRadius) && Random.Range(0.0f, 1f) <= chanceToSaveBorder)
                    continue;
                
                var point = new Vector3Int((int)sites[i].x, (int)sites[i].y, 0);


                var currentBiom = Biomes.Forest;
                for (var j = 0; j < perlinPointsByBiom.Length; j++)
                {
                    if (perlinPointsByBiom[j].Contains(point)) currentBiom = biomes[j].biomeType;
                }
                
                FillArea(point, currentBiom);
            }
        }
    }
    
    /// <summary>
    /// Point inside map center circle radius
    /// </summary>
    /// <returns>Bool - point in radius</returns>
    private bool PointInCircleByRadius(Vector2 point, float radius)
    {
        var dist = Vector2.Distance(new Vector2(MapCenter.x, MapCenter.y), point);
        return (dist < radius);
    }
    
    /// <summary>
    /// Filling cell by map centroid
    /// </summary>
    private void FillArea(Vector3Int centroid, Biomes biom)
    { 
        int limit = 256; 
        var filled = new List<Vector3Int>();
        filled.Add(centroid);
        while (true)
        {
            var filledOneAtLeast = false;
            var pointsToDelete = new List<Vector3Int>();
            MapBiome locBiom = biomes.Where(item => item.biomeType == biom).FirstOrDefault();
            if (locBiom == null) locBiom = biomes[0];
            
            for (var i = 0; i < filled.Count; i++)
            {
                var tile = locBiom.tiles[Random.Range(0, locBiom.tiles.Length)];
                var point = filled[i];
                var next = point + Vector3Int.right;
                if (voronoiMap[next.x, next.y] != 1 && !ground.HasTile(next))
                {
                    filledOneAtLeast = true;
                    ground.SetTile(next, tile);
                    filled.Add(next);
                }

                next = point + Vector3Int.left;
                if (voronoiMap[next.x, next.y] != 1 && !ground.HasTile(next))
                {
                    filledOneAtLeast = true;
                    ground.SetTile(next, tile);
                    filled.Add(next);
                }

                next = point + Vector3Int.up;
                if (voronoiMap[next.x, next.y] != 1 && !ground.HasTile(next))
                {
                    filledOneAtLeast = true;
                    ground.SetTile(next, tile);
                    filled.Add(next);
                }

                next = point + Vector3Int.down;
                if (voronoiMap[next.x, next.y] != 1 && !ground.HasTile(next))
                {
                    filledOneAtLeast = true;
                    ground.SetTile(next, tile);
                    filled.Add(next);
                }
                pointsToDelete.Add(point);
            }
            foreach (var p in pointsToDelete) filled.Remove(p);
            if (!filledOneAtLeast || limit-- < 0) break;
        }
    }

    /// <summary>
    /// Filling empty ghost map lines on tilemap 
    /// </summary>
    private void FillEmpty()
    {
        for (var x = 0; x < mapSize; x++)
        {
            for (var y = 0; y < mapSize; y++)
            {
                if (ground.HasTile(new Vector3Int(x, y, 0))) continue;
                
                if ((ground.HasTile(new Vector3Int(x + 1, y, 0)) || ground.HasTile(new Vector3Int(x + 2, y, 0)))
                && ground.HasTile(new Vector3Int(x - 1, y, 0)))
                {
                    ground.SetTile(new Vector3Int(x, y, 0), ground.GetTile(new Vector3Int(x - 1, y, 0)));
                }
                if ((ground.HasTile(new Vector3Int(x, y + 1, 0)) || ground.HasTile(new Vector3Int(x, y + 2, 0)))
                && ground.HasTile(new Vector3Int(x, y - 1, 0)))
                {
                    ground.SetTile(new Vector3Int(x, y, 0), ground.GetTile(new Vector3Int(x, y - 1, 0)));
                }
            }
        }
    }

    /// <summary>
    /// Check objects near
    /// </summary>
    /// <param name="pos">New object position</param>
    /// <param name="radius">Objects around that point</param>
    /// <returns>Bool - object has no object near</returns>
    private bool CheckObjectNear(Vector2 pos, float radius)
    {
        var results = new Collider2D[10];
        Physics2D.OverlapCircleNonAlloc(pos, radius, results, objectsLayerMask);
        return results.All(item => item == null);
    }
    
    /// <summary>
    /// Distribute objects on tilemap from settings list
    /// </summary>
    private void ObjectsDistribution()
    {
        foreach (var mapObject in mapObjects)
        {
            int placedCount = 0;
            var availablePoints = CalcNoise(mapObject.minHeight, mapObject.maxHeight);
            foreach (var pos in availablePoints)
            {
                var vec2 = new Vector2(pos.x, pos.y);
                if (!ground.HasTile(pos)
                    || Random.Range(0f, 1f) > mapObject.createChance
                    || !CheckObjectNear(vec2, mapObject.distanceFromOther)
                ) continue;
                if (mapObject.useLimit == true)
                {
                    placedCount++;
                    if (placedCount > mapObject.limit) break;
                }
                Instantiate(
                    mapObject.prefabs[Random.Range(0, mapObject.prefabs.Length)],
                    pos,
                    Quaternion.identity,
                    transform
                );
            }
        }
        
        // старый код размещения ресурсов TODO: delete
        // var bushes = CalcNoise(0.4f, 0.59f);
        // var trees = CalcNoise(0.5f, 0.9f);
        // var stones = CalcNoise(0.1f, 0.5f, 20000, 20000);
        // for (var i = 0; i < trees.Count; i++) 
        // {
        //     var pos = trees[i];
        //     if (Random.Range(0.0f, 1f) > 0.9f 
        //         && ground.HasTile(pos) 
        //         && PointInCircleByRadius(new Vector2(pos.x, pos.y), FillRadius)
        //         && CheckObjectNear(new Vector2(pos.x, pos.y), distanceBetweenObjects))
        //     {
        //         Instantiate(treesPrefabs[Random.Range(0, treesPrefabs.Length)], pos, Quaternion.identity, transform);
        //     }
        // }
        // for (var i = 0; i < bushes.Count; i++)
        // {
        //     var pos = bushes[i];
        //     if (Random.Range(0.0f, 1f) > 0.98f 
        //         && ground.HasTile(pos) 
        //         && PointInCircleByRadius(new Vector2(pos.x, pos.y), FillRadius)
        //         && CheckObjectNear(new Vector2(pos.x, pos.y), distanceBetweenObjects))
        //     {
        //         Instantiate(bushesPrefabs[Random.Range(0, bushesPrefabs.Length)], pos, Quaternion.identity, transform);
        //     }
        // }
        // for (var i = 0; i < bushes.Count; i++)
        // {
        //     var pos = bushes[i];
        //     if (Random.Range(0.0f, 1f) > 0.96f 
        //         && ground.HasTile(pos) 
        //         && PointInCircleByRadius(new Vector2(pos.x, pos.y), FillRadius)
        //         && CheckObjectNear(new Vector2(pos.x, pos.y), distanceBetweenObjects))
        //     {
        //         Instantiate(stonesPrefabs[Random.Range(0, stonesPrefabs.Length)], pos, Quaternion.identity, transform);
        //     }
        // }
    }
    
    /// <summary>
    /// Getting points between min and max from perlin noise
    /// </summary>
    /// <param name="min">Height from</param>
    /// <param name="max">Height to</param>
    /// <param name="offsetX">Perlin offset X</param>
    /// <param name="offsetY">Perlin offset Y</param>
    /// <param name="s">Perlin SCALE</param>
    /// <returns>Points from perlin noise map between min and  max values</returns>
    private List<Vector3Int> CalcNoise(float min, float max, float offsetX = 0, float offsetY = 0, float s = 0)
    {
        var poses = new List<Vector3Int>();
        var y = 0.0F;
        var scale = s > 0 ? s : scaler;
        while (y < mapSize)
        {
            var x = 0.0F;
            while (x < mapSize)
            {
                var xCoord = offsetX + x / mapSize * scale;
                var yCoord = offsetY + y / mapSize * scale;
                var height = Mathf.PerlinNoise(xCoord, yCoord);
                if (height >= min && height <= max)
                {
                    poses.Add(new Vector3Int((int)x, (int)y, 0));
                }
                x++;
            }
            y++;
        }
        return poses;
    }
    
    private void OnDrawGizmos()
    {
        if (!isEditMode) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(MapCenter.x, MapCenter.y, 0), FillRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(MapCenter.x, MapCenter.y, 0), FillRadius * cuteRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(MapCenter.x, MapCenter.y, 0), new Vector3(mapSize, mapSize));
        
        Gizmos.color = Color.grey;
        var area = (mapSize * mapSize) / (float)siteCount;
        var areaLen = Mathf.Sqrt(area);
        for (float y = 0; y < mapSize - areaLen; y += areaLen)
        {
            var slideY = false;
            for (float x = 0; x < mapSize - areaLen; x += areaLen)
            {
                // Calculating circle inside small cube and putting random point                
                var squareCenterX = x + areaLen / 2;
                var squareCenterY = y + (slideY ? areaLen / 2 : areaLen);
                var radius = areaLen * minSizeBetweenPoints;
                slideY = !slideY;

                Gizmos.DrawWireSphere(new Vector3(squareCenterX, squareCenterY, 0), radius);
            }
        }
        
        if (ge == null) return;
        foreach (var t in ge)
        {
            var p1 = new Vector3Int((int)t.x1, (int)t.y1, 0);
            var p2 = new Vector3Int((int)t.x2, (int)t.y2, 0);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(p1, 2f);
            Gizmos.DrawWireSphere(p2, 2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(p1, p2);
        }
        for (var i = 0; i < sites.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector2(sites[i].x - 1.5f, sites[i].y - 1.5f), 2f);
        }
    }
}
