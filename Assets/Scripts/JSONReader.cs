using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset modelJSON;

    [System.Serializable]
    public class Model
    {
        public string name;
        public float[] verts;
        public int[] tetIds;
        public int[] tetEdgeIds;
        public int[] tetSurfaceTriIds;
    }

    public Model model = new Model();
    // Start is called before the first frame update
    void Start()
    {
        model = JsonUtility.FromJson<Model>(modelJSON.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
