using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour
{
    [HideInInspector]
    public bool playerPlaced = false;
    [HideInInspector]
    public bool saveLoadMenuOpen = false;

    public Animator itemUIAnimation;
    public Animator optionUIAnimation;
    public Animator saveUIAnimation;
    public Animator loadUIAnimation;
    public MeshFilter mouseObject;
    public MouseScript user;
    public Mesh playerMarker;
    public Slider rotSlider;
    public GameObject rotUI;
    public InputField levelNameSave;
    public InputField levelNameLoad;
    public Text levelMessage;
    public Animator messageAnim;

    private bool itemPositionIn = true;
    private bool optionPositionIn = true;
    private bool saveLoadPositionIn = false;
    private LevelEditor level;

    void Start()
    {
        rotSlider.onValueChanged.AddListener(delegate { RotationValueChange(); }); 
        CreateEditor(); 
    }

    LevelEditor CreateEditor()
    {
        level = new LevelEditor();
        level.editorObjects = new List<EditorObject.Data>(); 
        return level;
    }

    void RotationValueChange()
    {
        user.rotObject.transform.localEulerAngles = new Vector3(0, rotSlider.value, 0); 
        user.rotObject.GetComponent<EditorObject>().data.rot = user.rotObject.transform.rotation; 
    }

    public void SlideItemMenu()
    {
        if (itemPositionIn == false)
        {
            itemUIAnimation.SetTrigger("ItemMenuIn"); 
            itemPositionIn = true; 
        }
        else
        {
            itemUIAnimation.SetTrigger("ItemMenuOut"); 
            itemPositionIn = false; 
        }
    }

    public void SlideOptionMenu()
    {
        if (optionPositionIn == false)
        {
            optionUIAnimation.SetTrigger("OptionMenuIn");
            optionPositionIn = true; 
        }
        else
        {
            optionUIAnimation.SetTrigger("OptionMenuOut"); 
            optionPositionIn = false;
        }
    }

    public void ChooseSave()
    {
        if (saveLoadPositionIn == false)
        {
            saveUIAnimation.SetTrigger("SaveLoadIn"); 
            saveLoadPositionIn = true; 
            saveLoadMenuOpen = true; 
        }
        else
        {
            saveUIAnimation.SetTrigger("SaveLoadOut");
            saveLoadPositionIn = false; 
            saveLoadMenuOpen = false; 
        }
    }

    public void ChooseLoad()
    {
        if (saveLoadPositionIn == false)
        {
            loadUIAnimation.SetTrigger("SaveLoadIn"); 
            saveLoadPositionIn = true; 
            saveLoadMenuOpen = true; 
        }
        else
        {
            loadUIAnimation.SetTrigger("SaveLoadOut"); 
            saveLoadPositionIn = false; 
            saveLoadMenuOpen = false; 
        }
    }

    public void ChooseCylinder()
    {
        user.itemOption = MouseScript.ItemList.Cylinder; 
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder); 
        mouseObject.mesh = cylinder.GetComponent<MeshFilter>().mesh;
        Destroy(cylinder);
    }
    
    public void ChooseCube()
    {
        user.itemOption = MouseScript.ItemList.Cube; 
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); 
        mouseObject.mesh = cube.GetComponent<MeshFilter>().mesh;
        Destroy(cube);
    }

    public void ChooseSphere()
    {
        user.itemOption = MouseScript.ItemList.Sphere; 
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); 
        mouseObject.mesh = sphere.GetComponent<MeshFilter>().mesh;
        Destroy(sphere);
    }

    public void ChoosePlayerStart()
    {
        user.itemOption = MouseScript.ItemList.Player; 
        mouseObject.mesh = playerMarker; 
    }

    public void ChooseCreate()
    {
        user.manipulateOption = MouseScript.LevelManipulation.Create; 
        user.mr.enabled = true; 
        rotUI.SetActive(false); 
    }

    public void ChooseRotate()
    {
        user.manipulateOption = MouseScript.LevelManipulation.Rotate;
        user.mr.enabled = false; 
        rotUI.SetActive(true); 
    }

    public void ChooseDestroy()
    {
        user.manipulateOption = MouseScript.LevelManipulation.Destroy; 
        user.mr.enabled = false; 
        rotUI.SetActive(false); 
    }

    public void SaveLevel()
    {
        EditorObject[] foundObjects = FindObjectsOfType<EditorObject>();
        foreach (EditorObject obj in foundObjects)
            level.editorObjects.Add(obj.data); 

        string json = JsonUtility.ToJson(level); 
        string folder = Application.dataPath + "/LevelData/"; 
        string levelFile = "";

        if (levelNameSave.text == "")
            levelFile = "new_level.json";
        else
            levelFile = levelNameSave.text + ".json";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, levelFile); 

        if (File.Exists(path))
            File.Delete(path);

        File.WriteAllText(path, json); 

        saveUIAnimation.SetTrigger("SaveLoadOut");
        saveLoadPositionIn = false;
        saveLoadMenuOpen = false;
        levelNameSave.text = ""; 
        levelNameSave.DeactivateInputField(); 

        levelMessage.text = levelFile + " saved to LevelData folder.";
        messageAnim.Play("MessageFade", 0, 0);
    }

    public void LoadLevel()
    {
        string folder = Application.dataPath + "/LevelData/";
        string levelFile = "";

        if (levelNameLoad.text == "")
            levelFile = "new_level.json";
        else
            levelFile = levelNameLoad.text + ".json";

        string path = Path.Combine(folder, levelFile); 

        if (File.Exists(path)) 
        {
            EditorObject[] foundObjects = FindObjectsOfType<EditorObject>();
            foreach (EditorObject obj in foundObjects)
                Destroy(obj.gameObject);

            playerPlaced = false; 

            string json = File.ReadAllText(path); 
            level = JsonUtility.FromJson<LevelEditor>(json); 
            CreateFromFile(); 
        }
        else 
        {
            loadUIAnimation.SetTrigger("SaveLoadOut"); 
            saveLoadPositionIn = false; 
            saveLoadMenuOpen = false; 
            levelMessage.text = levelFile + " could not be found!"; 
            messageAnim.Play("MessageFade", 0, 0);
            levelNameLoad.DeactivateInputField(); 
        }
    }

    void CreateFromFile()
    {
        GameObject newObj; 

        for (int i = 0; i < level.editorObjects.Count; i++)
        {
            if (level.editorObjects[i].objectType == EditorObject.ObjectType.Cylinder) 
            {
                newObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                newObj.transform.position = level.editorObjects[i].pos; 
                newObj.transform.rotation = level.editorObjects[i].rot; 
                newObj.layer = 9; 

                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Cylinder;
            }
            else if (level.editorObjects[i].objectType == EditorObject.ObjectType.Cube)
            {
                newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newObj.transform.position = level.editorObjects[i].pos; 
                newObj.transform.rotation = level.editorObjects[i].rot; 
                newObj.layer = 9; 

                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Cube;
            }
            else if (level.editorObjects[i].objectType == EditorObject.ObjectType.Sphere)
            {
                newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newObj.transform.position = level.editorObjects[i].pos; 
                newObj.transform.rotation = level.editorObjects[i].rot; 
                newObj.layer = 9; 

                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Sphere;
            }
            else if (level.editorObjects[i].objectType == EditorObject.ObjectType.Player)
            {
                newObj = Instantiate(user.Player, transform.position, Quaternion.identity);
                newObj.layer = 9; 
                newObj.AddComponent<CapsuleCollider>();
                newObj.GetComponent<CapsuleCollider>().center = new Vector3(0, 1, 0);
                newObj.GetComponent<CapsuleCollider>().height = 2;
                newObj.transform.position = level.editorObjects[i].pos;
                newObj.transform.rotation = level.editorObjects[i].rot; 
                playerPlaced = true;

                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Player;
            }
        }

        levelNameLoad.text = "";
        levelNameLoad.DeactivateInputField(); 

        loadUIAnimation.SetTrigger("SaveLoadOut"); 
        saveLoadPositionIn = false; 
        saveLoadMenuOpen = false; 

        levelMessage.text = "Level loading...done.";
        messageAnim.Play("MessageFade", 0, 0);
    }
}
