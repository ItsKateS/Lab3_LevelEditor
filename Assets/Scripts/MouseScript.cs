using UnityEngine;
using UnityEngine.EventSystems;

public class MouseScript : MonoBehaviour
{
    public enum LevelManipulation { Create, Rotate, Destroy }; 
    public enum ItemList { Cylinder, Cube, Sphere, Player }; 

    [HideInInspector] 
    public ItemList itemOption = ItemList.Cylinder; 
    [HideInInspector]
    public LevelManipulation manipulateOption = LevelManipulation.Create; 
    [HideInInspector]
    public MeshRenderer mr;
    [HideInInspector]
    public GameObject rotObject;

    public Material goodPlace;
    public Material badPlace;
    public GameObject Player;
    public ManagerScript ms;

    private Vector3 mousePos;
    private bool colliding;
    private Ray ray;
    private RaycastHit hit;

    void Start()
    {
        mr = GetComponent<MeshRenderer>(); 
    }

    void Update()
    {
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(
            Mathf.Clamp(mousePos.x, -20, 20),
            0.75f,
            Mathf.Clamp(mousePos.z, -20, 20)); 

        ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == 9) 
            {
                colliding = true; 
                mr.material = badPlace; 
            }
            else
            {
                colliding = false;
                mr.material = goodPlace;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject()) 
            {
                if (colliding == false && manipulateOption == LevelManipulation.Create) 
                    CreateObject();
                else if (colliding == true && manipulateOption == LevelManipulation.Rotate) 
                    SetRotateObject();
                else if (colliding == true && manipulateOption == LevelManipulation.Destroy) 
                {
                    if (hit.collider.gameObject.name.Contains("PlayerModel")) 
                        ms.playerPlaced = false;

                    Destroy(hit.collider.gameObject); 
                }

            }
        }
    }

    void CreateObject()
    {
        GameObject newObj;

        if (itemOption == ItemList.Cylinder) 
        {
            newObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            newObj.transform.position = transform.position;
            newObj.layer = 9;

            EditorObject eo = newObj.AddComponent<EditorObject>();
            eo.data.pos = newObj.transform.position;
            eo.data.rot = newObj.transform.rotation;
            eo.data.objectType = EditorObject.ObjectType.Cylinder;
        }
        else if (itemOption == ItemList.Cube) 
        {
            newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newObj.transform.position = transform.position;
            newObj.layer = 9; 

            EditorObject eo = newObj.AddComponent<EditorObject>();
            eo.data.pos = newObj.transform.position;
            eo.data.rot = newObj.transform.rotation;
            eo.data.objectType = EditorObject.ObjectType.Cube;
        }
        else if (itemOption == ItemList.Sphere) 
        {
            newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newObj.transform.position = transform.position;
            newObj.layer = 9; 

            EditorObject eo = newObj.AddComponent<EditorObject>();
            eo.data.pos = newObj.transform.position;
            eo.data.rot = newObj.transform.rotation;
            eo.data.objectType = EditorObject.ObjectType.Sphere;
        }
        else if (itemOption == ItemList.Player) 
        {
            if (ms.playerPlaced == false) 
            {
                newObj = Instantiate(Player, transform.position, Quaternion.identity);
                newObj.layer = 9; 
                newObj.AddComponent<CapsuleCollider>();
                newObj.GetComponent<CapsuleCollider>().center = new Vector3(0, 1, 0);
                newObj.GetComponent<CapsuleCollider>().height = 2;
                ms.playerPlaced = true;

                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Player;
            }
        }
    }

    void SetRotateObject()
    {
        rotObject = hit.collider.gameObject; 
        ms.rotSlider.value = rotObject.transform.rotation.y; 
    }
}
