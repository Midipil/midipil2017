using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileHandler : MonoBehaviour {
    [SerializeField] public float Weight { get; private set; }

    private GameObject _tile;
    private TileHandler _prevTile;

    [SerializeField] private GameObject _planctonPrefab;
    private float _planctonDensity = 0.5f;

    public Vector3[] Balises
    {
        get;
        private set;
    }

    public void Init(TileHandler prevTile = null)
    {
        _tile = gameObject;
        _prevTile = prevTile;

        Balises = _tile.GetComponentsInChildren<Transform>().Where(t => t.gameObject.tag == "balise").Select(t => t.position).OrderBy(p => p.z).ToArray();

        // generate the plancton, interpolating the path from the previous tile
        var controlPoints = new List<Vector3>();
        if (_prevTile != null) controlPoints.Add(_prevTile.Balises.Last());
        controlPoints.AddRange(Balises);

        var path = MakeSmoothCurve(controlPoints, 3.0f);
        float totalZDistance = path.Last().z  - path.First().z;
        float step = _planctonPrefab.transform.localScale.z / _planctonDensity;

        int currentPointAfter = 1;
        for(float z = path.First().z; z <= path.Last().z && currentPointAfter < path.Length; z+= step)
        {
            if (path[currentPointAfter].z <= z) currentPointAfter++;
            var before = path[currentPointAfter - 1];
            var after = path[currentPointAfter];

            var interpolatedPos = Vector3.Lerp(before, after, (z - before.z) / (after.z - before.z));

            Instantiate(_planctonPrefab, transform, false);
            _planctonPrefab.transform.position = interpolatedPos;
        }
    }

    // arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
    public static Vector3[] MakeSmoothCurve(IList<Vector3> arrayToCurve, float smoothness)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (smoothness < 1.0f) smoothness = 1.0f;

        pointsLength = arrayToCurve.Count;

        curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3>(arrayToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints.ToArray());
    }
}
