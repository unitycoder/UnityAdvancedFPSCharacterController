using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager Instance;

    // Manages the tags that defines the object's material so that footsteps and raycast shoot effects/particles will vary
    [Header("Scene Object Tags")]
    [SerializeField] public string woodTag = "Material/Wood";
    [SerializeField] public string stoneTag = "Material/Stone";
    [SerializeField] public string metalTag = "Material/Metal";
    [SerializeField] public string grassTag = "Material/Grass";

    private void Awake()
    {
        Instance = this;
    }
}