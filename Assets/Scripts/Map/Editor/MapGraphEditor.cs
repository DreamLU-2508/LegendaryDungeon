using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DreamLU
{
    public class MapGraphEditor : EditorWindow
    {
        private GUIStyle roomNodeStyle;
        private GUIStyle roomNodeSelectedStyle;
        private RoomNodeListType roomNodeListType;

        // node stytle
        private const float nodeWidth = 160f;
        private const float nodeHeight = 75f;
        private const int nodePadding = 25;
        private const int nodeBoder = 12;

        // static
        private static RoomNodeGraph currentRoomNodeGraph;

        private RoomNode currentSelectedRoomNode = null;

        [MenuItem("Map Graph Editor", menuItem = "Window/Map Editor/Map Graph Editor")]

        private static void OpenWindow()
        {
            GetWindow<MapGraphEditor>("Map Graph Editor");
        }

        [OnOpenAsset(0)]
        public static bool OnDoubleClickAsset(int instanceID, int line)
        {
            RoomNodeGraph roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraph;

            if (roomNodeGraph != null)
            {
                OpenWindow();

                currentRoomNodeGraph = roomNodeGraph;
                currentRoomNodeGraph.roomNodeTypeList = GameResources.Instance.roomNodeListType;
                GUI.changed = true;

                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            // Difine node layout style
            roomNodeStyle = new GUIStyle();
            roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            roomNodeStyle.normal.textColor = Color.white;
            roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
            roomNodeStyle.border = new RectOffset(nodeBoder, nodeBoder, nodeBoder, nodeBoder);

            // Difine node selected layout style
            roomNodeSelectedStyle = new GUIStyle();
            roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
            roomNodeSelectedStyle.normal.textColor = Color.white;
            roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
            roomNodeSelectedStyle.border = new RectOffset(nodeBoder, nodeBoder, nodeBoder, nodeBoder);

            // Load room node list type 
            roomNodeListType = GameResources.Instance.roomNodeListType;
        }

        private void OnDestroy()
        {
            if(currentRoomNodeGraph != null)
            {
                if (currentRoomNodeGraph.roomNodes != null && currentRoomNodeGraph.roomNodes.Count > 0)
                {
                    foreach (var roomNode in currentRoomNodeGraph.roomNodes)
                    {
                        roomNode.isSelected = false;
                        roomNode.isLeftClickDragging = false;
                    }
                }
            }
            CleanLine();
        }

        private void CleanLine()
        {
            if (currentRoomNodeGraph != null)
            {
                currentRoomNodeGraph.roomNodeStartDrawLine = null;
                currentRoomNodeGraph.positionEndLine = Vector2.zero;
                GUI.changed = true;
            }
        }

        private void OnGUI()
        {
            if(currentRoomNodeGraph != null)
            {
                // Draw line conection
                DrawLine();

                // Process Events
                ProcessEvents(Event.current);

                // Draw Conections Between Rooms
                DrawRoomConections();

                // Draw Room Nodes
                DrawRoomNodes();
            }

            if(GUI.changed)
            {
                Repaint();
            }
        }

        #region Process Events

        private void ProcessEvents(Event currentEvent)
        {
            // Get Current Selected RoomNode
            currentSelectedRoomNode = GetRoomNodeByMouseOver(currentEvent);
            //if (currentSelectedRoomNode == null || currentSelectedRoomNode.isLeftClickDragging)
            //{
                
            //    Debug.LogError(currentSelectedRoomNode);
            //} 

            if(currentRoomNodeGraph.roomNodeStartDrawLine != null)
            {
                ProcessMapGrapgEvents(currentEvent);
                return;
            }

            if(currentSelectedRoomNode != null)
            {
                currentSelectedRoomNode.ProcessEvents(currentEvent);
            }
            else
            {
                ProcessMapGrapgEvents(currentEvent);
            }
        }

        private void ProcessMapGrapgEvents(Event currentEvent)
        {
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    ProcessMouseDownEvent(currentEvent);
                    break;

                case EventType.MouseUp:
                    ProcessMouseUpEvent(currentEvent);
                    break;

                case EventType.MouseDrag:
                    ProcessMouseDragEvent(currentEvent);
                    break;
                default:
                    break;
            }
        }

        private void ProcessMouseDownEvent(Event currentEvent)
        {
            if(currentEvent.button == 1)
            {
                ShowContextMenu(currentEvent.mousePosition);
            }
            else if (currentEvent.button == 0)
            {
                CleanLine();

                ClearAllSeclectedRoomNodes();
            }
        }

        private void ClearAllSeclectedRoomNodes()
        {
            foreach (var roomNode in currentRoomNodeGraph.roomNodes)
            {
                roomNode.isSelected = false;
            }

            GUI.changed = true;
        }

        private void ProcessMouseUpEvent(Event currentEvent)
        {
            if (currentEvent.button == 1)
            {
                if (currentRoomNodeGraph.roomNodeStartDrawLine != null)
                {
                    RoomNode roomNode = GetRoomNodeByMouseOver(currentEvent);

                    if (roomNode != null && roomNode != currentRoomNodeGraph.roomNodeStartDrawLine)
                    {
                        if (currentRoomNodeGraph.roomNodeStartDrawLine.AddChildRoomNodeToList(roomNode.id))
                        {
                            roomNode.AddParentRoomNodeToList(currentRoomNodeGraph.roomNodeStartDrawLine.id);
                        }
                    }

                    CleanLine();
                }
            }
        }

        private void ProcessMouseDragEvent(Event currentEvent)
        {
            if (currentEvent.button == 1)
            {
                if(currentRoomNodeGraph.roomNodeStartDrawLine != null)
                {
                    currentRoomNodeGraph.positionEndLine += currentEvent.delta;
                    GUI.changed = true;
                }
            }
        }

        private void DrawLine()
        {
            if (currentRoomNodeGraph.positionEndLine != Vector2.zero)
            {
                Handles.DrawBezier(currentRoomNodeGraph.roomNodeStartDrawLine.rect.center, currentRoomNodeGraph.positionEndLine,
                    currentRoomNodeGraph.roomNodeStartDrawLine.rect.center, currentRoomNodeGraph.positionEndLine, Color.white, null, 3f);
            }
        }

        private void ShowContextMenu(Vector2 mousePosition)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Delete Selected Room Node Links "), false, DeleteSelectedRoomNodeLinks);
            menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

            menu.ShowAsContext();
        }

        private void CreateRoomNode(object mousePositionObject)
        {
            if(currentRoomNodeGraph.roomNodes.Count == 0)
            {

                CreateNode(currentRoomNodeGraph.roomNodeTypeList.GetRoomNodeType(RoomType.Entrance, new RoomNodeType()
                {
                    disPlayInRoomNodeGraph = true,
                    type = RoomType.Entrance,
                    RoomName = "Entrance"
                }), new Vector2(200, 200));
            }

            RoomNodeType roomNodeType = new RoomNodeType()
            {
                disPlayInRoomNodeGraph = true,
                type = RoomType.None,
                RoomName = "None"
            };

            CreateNode(roomNodeType, (Vector2)mousePositionObject);
        }

        private void SelectAllRoomNodes()
        {
            if(currentRoomNodeGraph != null)
            {
                foreach(var roomNode in currentRoomNodeGraph.roomNodes)
                {
                    roomNode.isSelected = true;
                }
                GUI.changed = true;
            }
        }

        private void DeleteSelectedRoomNodeLinks()
        {
            foreach (var roomNode in currentRoomNodeGraph.roomNodes)
            {
                if(roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
                {
                    for(int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        RoomNode childRoomNode = currentRoomNodeGraph.roomNodes.Find(x => x.id == roomNode.childRoomNodeIDList[i]);
                        if(childRoomNode != null)
                        {
                            roomNode.RemoveChildRoomNodeToList(childRoomNode.id);

                            childRoomNode.RemoveParentRoomNodeToList(roomNode.id);
                        }
                    }
                }

                if (roomNode.isSelected && roomNode.parentRoomNodeIDList.Count > 0)
                {
                    for (int i = roomNode.parentRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        RoomNode parentRoomNode = currentRoomNodeGraph.roomNodes.Find(x => x.id == roomNode.parentRoomNodeIDList[i]);
                        if (parentRoomNode != null)
                        {
                            roomNode.RemoveParentRoomNodeToList(parentRoomNode.id);

                            parentRoomNode.RemoveChildRoomNodeToList(roomNode.id);
                        }
                    }
                }
            }

            ClearAllSeclectedRoomNodes();
        }

        private void DeleteSelectedRoomNodes()
        {
            Queue<RoomNode> list = new Queue<RoomNode>();
            foreach(var roomNode in currentRoomNodeGraph.roomNodes)
            {
                if (roomNode.isSelected)
                {
                    list.Enqueue(roomNode);
                }
            }

            DeleteSelectedRoomNodeLinks();

            while(list.Count > 0)
            {
                RoomNode roomNode = list.Dequeue();

                currentRoomNodeGraph.roomNodesDictionary.Remove(roomNode.id);
                currentRoomNodeGraph.roomNodes.Remove(roomNode);
                DestroyImmediate(roomNode, true);
                AssetDatabase.SaveAssets();
            }
        }

        private void CreateNode(RoomNodeType roomNodeType, Vector2 mousePosition)
        {
            RoomNode roomNode = ScriptableObject.CreateInstance<RoomNode>();
            currentRoomNodeGraph.roomNodes.Add(roomNode);
            roomNode.Init(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType, roomNodeListType);

            AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
            AssetDatabase.SaveAssets();

            // refresh data
            currentRoomNodeGraph.OnValidate();
        }

        private void DrawRoomConections()
        {
            if(currentRoomNodeGraph != null && currentRoomNodeGraph.roomNodes.Count > 0)
            {
                foreach(var roomNode in currentRoomNodeGraph.roomNodes)
                {
                    if(roomNode.childRoomNodeIDList.Count > 0)
                    {
                        foreach(var childID in roomNode.childRoomNodeIDList)
                        {
                            if (currentRoomNodeGraph.roomNodesDictionary.ContainsKey(childID))
                            {
                                DrawConectionsLine(roomNode, currentRoomNodeGraph.roomNodesDictionary[childID]);
                                GUI.changed = true;
                            }
                        }
                    }
                }
            }
        }

        private void DrawConectionsLine(RoomNode parentNode, RoomNode chilidNode)
        {
            Vector2 startPos = parentNode.rect.center;
            Vector2 endPos = chilidNode.rect.center;

            Vector2 midPos = (startPos + endPos) / 2f;
            Vector2 dir = endPos - startPos;

            // Arrow 
            Vector2 arrowPos1 = midPos - new Vector2(-dir.y, dir.x).normalized * 6f;
            Vector2 arrowPos2 = midPos + new Vector2(-dir.y, dir.x).normalized * 6f;
            Vector2 arrowHear = midPos + dir.normalized * 6f;

            // Draw Arrow
            Handles.DrawBezier(arrowHear, arrowPos1, arrowHear, arrowPos1, Color.white, null, 3f);
            Handles.DrawBezier(arrowHear, arrowPos2, arrowHear, arrowPos2, Color.white, null, 3f);

            // Draw line
            Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.white, null, 3f);

            GUI.changed = true;
        }

        #endregion

        #region Draw Room Nodes

        private void DrawRoomNodes()
        {
            if(currentRoomNodeGraph.roomNodes != null && currentRoomNodeGraph.roomNodes.Count > 0)
            {
                foreach (var roomNode in  currentRoomNodeGraph.roomNodes)
                {
                    if(roomNode.isSelected)
                    {
                        roomNode.Draw(roomNodeSelectedStyle);
                    }
                    else
                    {
                        roomNode.Draw(roomNodeStyle);
                    }
                        
                }
            }
        }

        #endregion

        #region Room Node Hanlder

        private RoomNode GetRoomNodeByMouseOver(Event currentEvent)
        {
            if(currentRoomNodeGraph.roomNodes != null && currentRoomNodeGraph.roomNodes.Count > 0)
            {
                foreach(var roomNode in currentRoomNodeGraph.roomNodes)
                {
                    if (roomNode.rect.Contains(currentEvent.mousePosition))
                    {
                        return roomNode;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
