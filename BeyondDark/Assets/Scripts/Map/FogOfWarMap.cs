using UnityEngine;
using System.Collections;

public class FogOfWarMap : MonoBehaviour
{
    public GameObject m_fogOfWarPlane;
    public Transform m_player;
    public LayerMask m_fogLayer;
    public float m_radius = 5f;
    private float m_radiusSqr { get { return m_radius * m_radius; } }

    private Mesh m_mesh;
    private Vector3[] m_vertices;
    private Color[] m_colors;

    public Color burnColor;

    // Use this for initialization
    void Start()
    {
        Initialize();
    }

    private void Awake()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(transform.position, (m_player.position + Vector3.forward * 4) - transform.position);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 1000, m_fogLayer, QueryTriggerInteraction.Collide))
        {
            for (int i = 0; i < m_vertices.Length; i++)
            {
                Vector3 v = m_vertices[i];
                float dist = Vector3.SqrMagnitude(v - hit.point);
                if (dist < m_radiusSqr)
                {
                    float alpha = Mathf.Min(m_colors[i].a, dist / m_radiusSqr);
                    float noiseValue = Mathf.PerlinNoise((v.x + Time.time) / m_radius, (v.z + Time.time) / m_radius);
                    m_colors[i] = Color.Lerp(Color.white, burnColor, noiseValue);
                    m_colors[i].a = alpha * 2;
                    StartCoroutine(pixelBurner(i, alpha));
                }
            }
            UpdateColor();
        }
    }

    IEnumerator pixelBurner(int hitPoint, float alpha)
    {
        yield return new WaitForSeconds(1);
        m_colors[hitPoint].a = Mathf.Lerp(m_colors[hitPoint].a, 0, 5);
    }

    void Initialize()
    {
        m_mesh = m_fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        m_vertices = m_mesh.vertices;
        m_colors = new Color[m_vertices.Length];
        for (int i = 0; i < m_vertices.Length; i++)
        {
            m_vertices[i] = m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
        }

        for (int i = 0; i < m_colors.Length; i++)
        {
            m_colors[i] = Color.white;
        }
        UpdateColor();
    }

    void UpdateColor()
    {
        m_mesh.colors = m_colors;
    }
}