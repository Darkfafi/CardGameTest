using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

public class BaseGenericNodeEditor : EditorWindow, IOriginScene
{
    public const string EDITOR_EXTENSION = ".gne";

    public Vector2 SceneOrigin
    {
        get
        {
            return origin + viewport;
        }
    }

    
    [MenuItem("Assets/Create Generic Node Editor File")]
    public static void CreateWindow()
    {
        string path = AssetDatabase.GenerateUniqueAssetPath(Selection.activeObject.name + "/newNodeFile" + EDITOR_EXTENSION);
        AssetDatabase.CreateAsset(new TextAsset(), path);
        CreateWindow(path).Save();
    }

    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        Debug.Log(Selection.activeObject.GetType());
        DefaultAsset da = Selection.activeObject as DefaultAsset;
        
        if (da != null)
        {
            string path = AssetDatabase.GetAssetPath(da);
            if(Path.GetExtension(path) == EDITOR_EXTENSION)
            {
                CreateWindow(path).Load();
                return true;
            }
        }

        return false;
    }

    private static BaseGenericNodeEditor CreateWindow(string path)
    {
        BaseGenericNodeEditor bgne = GetWindow<BaseGenericNodeEditor>("Generic Node Editor DEBUG WINDOW", true);
        bgne.filePath = path;
        bgne.ShowPopup();
        return bgne;
    }

    public string filePath = "";

    private ConnectionsController connectionsController = new ConnectionsController();
    private List<INodeEditorDrawable> drawables = new List<INodeEditorDrawable>();
    private Dictionary<BaseNodeModel, BaseNodeView> modelsToViewsMap = new Dictionary<BaseNodeModel, BaseNodeView>();

    private GenericMenuBar menuBar;

    private Vector2 origin = new Vector2();
    private Vector2 viewport = new Vector2();
    private float scale = 1;

    private NodeSocketView currentViewSelected = null;

    public Vector2 ToViewportPosition(Vector2 positionToConvert)
    {
        return SceneOrigin + (positionToConvert - SceneOrigin) * (1 / scale) - viewport;
    }

    protected void OnEnable()
    {
        origin = position.size * 0.5f;
        wantsMouseMove = true;
    }

    protected void OnGUI()
    {
        // Drawing
        DrawBackground();

        EditorGUI.DrawRect(new Rect(SceneOrigin, new Vector2(10 * scale, 10 * scale)), Color.green);

        DrawDrawables();

        // Objects Interactions
        DrawablesInteractions(Event.current);

        // Scene Interactions
        SceneInteractions(Event.current);
        ScreenNavigationControlls(Event.current);

        // UI
        if (menuBar == null)
        {
            menuBar = new GenericMenuBar(this);
            menuBar.AddButton("Save", OnSaveClicked);
            menuBar.AddButton("Load", OnLoadClicked);
        }
        menuBar.DrawMenuBarGUI(20);

        // Draw Connection Preview
        if (currentViewSelected != null)
        {
            Vector2 posA = currentViewSelected.ViewportRect.GetViewportPositionCenter();
            Vector2 posB = Event.current.mousePosition;
            ConnectionView.DrawConnectionLine(posB, posA, Color.white, 6);
            GUI.changed = true;
        }

        if (GUI.changed) { Repaint(); }
    }

    public void Load()
    {
        CleanEditor();
        XmlObjectReferences references = new XmlObjectReferences();
        GenericNodeEditorSavedData data = new GenericNodeEditorSavedData();
        references.Loading_LoadContainer(data, filePath);

        connectionsController = data.ConnectionController;

        for (int i = 0; i < data.NodeViews.Length; i++)
        {
            data.NodeViews[i].SetScene(this);
            AddNode(data.NodeViews[i]);
        }

        for (int i = 0; i < connectionsController.AllConnections.Length; i++)
        {
            ConnectionModel cm = connectionsController.AllConnections[i];
            CreateConnectionView(cm, modelsToViewsMap[cm.InputSocket.ParentNode].GetSocketView(cm.InputSocket), modelsToViewsMap[cm.OutputSocket.ParentNode].GetSocketView(cm.OutputSocket));
        }
    }

    public void Save()
    {
        XmlObjectReferences references = new XmlObjectReferences();
        GenericNodeEditorSavedData data = new GenericNodeEditorSavedData();

        // Setting Data to save
        data.ConnectionModels = connectionsController.AllConnections;
        data.NodeViews = drawables.Where((d => typeof(BaseNodeView).IsAssignableFrom(d.GetType()))).Cast<BaseNodeView>().ToArray();

        List<BaseNodeModel> models = new List<BaseNodeModel>();
        List<BaseNodeSocketModel> socketModels = new List<BaseNodeSocketModel>();

        for (int i = 0; i < data.NodeViews.Length; i++)
        {
            models.Add(data.NodeViews[i].NodeModel);

            socketModels.AddRange(data.NodeViews[i].NodeModel.InputSockets);
            socketModels.AddRange(data.NodeViews[i].NodeModel.OutputSockets);
        }

        data.NodeModels = models.ToArray();
        data.NodeSockets = socketModels.ToArray();
        data.ConnectionController = connectionsController;
        // End setting data to save

        references.Saving_SaveContainer(data, filePath);
        AssetDatabase.Refresh();
    }

    // Registering
    public void RegisterDrawable(INodeEditorDrawable instance)
    {
        drawables.Add(instance);
        SetScale(scale);
        SetViewportPosition(viewport);
    }

    public void UnregisterDrawable(INodeEditorDrawable instance)
    {
        drawables.Remove(instance);
        instance.OnRemovedFromDraw();
    }

    // Drawing
    private void DrawBackground()
    {
        EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(0.2f, 0.2f, 0.2f));

        DrawGrid(20 * scale, 0.2f, Color.green);
        DrawGrid(100 * scale, 0.8f, Color.green);
    }

    private void DrawDrawables()
    {
        for(int i = 0; i < drawables.Count; i++)
        {
            drawables[i].Draw();
        }
    }

    // Handling Events
    private void DrawablesInteractions(Event e)
    {
        for (int i = 0; i < drawables.Count; i++)
        {
            drawables[i].HandleEvents(e);
        }
    }

    private void SceneInteractions(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                if(e.button == 0)
                {
                    EditorGUI.FocusTextInControl("");
                }
                if(e.button == 1)
                {
                    OpenSceneOptionsMenu(e);
                    e.Use();
                }
                break;
                
        }
    }

    private void ScreenNavigationControlls(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDrag:
                if (e.button == 2)
                {
                    SetViewportPosition(viewport + e.delta);
                }
                break;
            case EventType.ScrollWheel:
                float inputScale = scale + -e.delta.y * (0.05f * scale);
                float oldScale = scale;
                SetScale(inputScale);
                SetViewportPosition(viewport * (scale / oldScale));
                break;

        }
    }

    private void SetScale(float scale)
    {
        this.scale = Mathf.Clamp(scale, 0.25f, 1.25f);

        for (int i = 0; i < drawables.Count; i++)
        {
            drawables[i].ViewportRect.SetScale(scale);
        }

        GUI.changed = true;
    }

    private void SetViewportPosition(Vector2 viewportPos)
    {
        viewport = viewportPos;

        for (int i = 0; i < drawables.Count; i++)
        {
            drawables[i].ViewportRect.SetOffset(viewport);
        }

        GUI.changed = true;
    }

    private void OpenSceneOptionsMenu(Event e)
    {
        GenericMenu gm = new GenericMenu();
        Type[] nodeTypes = typeof(BaseNodeView).Assembly.GetTypes().Where(nt => typeof(BaseNodeView).IsAssignableFrom(nt) && !nt.IsAbstract && nt.IsDefined(typeof(NodeViewForModelAttribute), true)).ToArray();

        for (int i = 0; i < nodeTypes.Length; i++)
        {
            int index = i;
            string path = GetPathForNodeType(nodeTypes[i]);
            NodeViewForModelAttribute nvma = nodeTypes[index].GetCustomAttributes(typeof(NodeViewForModelAttribute), true)[0] as NodeViewForModelAttribute;
            if (path != NodePathAttribute.DO_NOT_MENTION)
                gm.AddItem(new GUIContent("Add Node/" + path + nodeTypes[i].Name), false, () => { CreateAndAddNode(nodeTypes[index], nvma.NodeModelType, e.mousePosition, true); });
        }
        gm.ShowAsContext();
    }

    private void CreateAndAddNode(Type viewType, Type modelType, Vector2 position, bool canBeRemoved)
    {
        BaseNodeModel model = (BaseNodeModel)Activator.CreateInstance(modelType, new object[] { connectionsController }); 
        BaseNodeView nv = (BaseNodeView)Activator.CreateInstance(viewType, new object[] { model, ToViewportPosition(position), this , canBeRemoved });
        AddNode(nv);
    }

    private void AddNode(BaseNodeView nodeView)
    {
        nodeView.RegisterToInteractionEvents(RemoveNode, ResetConnectionsForNode);
        nodeView.NodeSocketClickedEvent += OnNodeSocketClickedEvent;
        modelsToViewsMap.Add(nodeView.NodeModel, nodeView);
        RegisterDrawable(nodeView);
    }

    private void RemoveNode(BaseNodeView nodeView)
    {
        connectionsController.RemoveAllConnectionsForNode(nodeView.NodeModel);
        nodeView.UnregisterToInteractionEvents(RemoveNode, ResetConnectionsForNode);
        modelsToViewsMap.Remove(nodeView.NodeModel);
        UnregisterDrawable(nodeView);
    }

    private void OnNodeSocketClickedEvent(NodeSocketView nodeSocketViewClicked)
    {
        if(currentViewSelected == null && nodeSocketViewClicked.SocketModel.SocketModelType == SocketModelType.Output)
        {
            currentViewSelected = nodeSocketViewClicked;
        }
        else if (currentViewSelected != null)
        {
            ConnectionInfo connectionInfo;
            ConnectionModel cm = connectionsController.Connect(currentViewSelected.SocketModel, nodeSocketViewClicked.SocketModel, out connectionInfo);
            if (cm != null)
            {
                CreateConnectionView(cm, nodeSocketViewClicked, currentViewSelected);
            }
            currentViewSelected = null;
        }
    }

    private void CreateConnectionView(ConnectionModel connectionModel, NodeSocketView input, NodeSocketView output)
    {
        ConnectionView cv = new ConnectionView(connectionModel, this, input, output);
        RegisterDrawable(cv);
        cv.ConnectionDestroyRequestEvent += OnConnectionDestroyRequestEvent;
    }

    private void OnConnectionDestroyRequestEvent(ConnectionView connectionViewToDestroy)
    {
        connectionViewToDestroy.ConnectionDestroyRequestEvent -= OnConnectionDestroyRequestEvent;
        UnregisterDrawable(connectionViewToDestroy);
    }

    private void ResetConnectionsForNode(BaseNodeView nodeview)
    {
        connectionsController.RemoveAllConnectionsForNode(nodeview.NodeModel);
    }

    private string GetPathForNodeType(Type nodeType)
    {
        object[] attributes = nodeType.GetCustomAttributes(typeof(NodePathAttribute), true);
        if (attributes.Length == 0) { return ""; }
        NodePathAttribute attr = attributes[0] as NodePathAttribute;
        if (attr.Path == NodePathAttribute.DO_NOT_MENTION) { return NodePathAttribute.DO_NOT_MENTION; }
        return attr.Path + "/";
    }

    private void DrawGrid(float gridSpacing, float alpha, Color gridColor)
    {
        int devW = Mathf.CeilToInt(position.width / gridSpacing);
        int devH = Mathf.CeilToInt(position.height / gridSpacing);

        Vector2 gridVisualOffset = new Vector2(viewport.x % gridSpacing, viewport.y % gridSpacing);

        Vector2 s;
        Vector2 e;

        Handles.BeginGUI();
        Color c = gridColor;
        c.a = alpha;
        gridColor = c;

        Handles.color = gridColor;
        
        for(int i = 0; i < devW; i++)
        {
            s = new Vector3(gridSpacing * i + gridVisualOffset.x, -gridSpacing);
            e = s;
            e.y += position.height + gridSpacing;

            Handles.DrawLine(s, e);
        }

        for(int i = 0; i < devH; i++)
        {
            s = new Vector3(-gridSpacing, gridSpacing * i + gridVisualOffset.y);
            e = s;
            e.x += position.width + gridSpacing;

            Handles.DrawLine(s, e);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    // Menubar Interactions

    private void OnSaveClicked(string nameButton)
    {
        Save();
    }

    private void OnLoadClicked(string nameButton)
    {
        Load();
    }

    private void CleanEditor()
    {
        BaseNodeView[] allCurrentNodeViews = drawables.Where((d => typeof(BaseNodeView).IsAssignableFrom(d.GetType()))).Cast<BaseNodeView>().ToArray();

        for(int i = 0; i < allCurrentNodeViews.Length; i++)
        {
            RemoveNode(allCurrentNodeViews[i]);
        }
    }
}
