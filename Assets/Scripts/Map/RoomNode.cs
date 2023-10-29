using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DreamLU
{
    public class RoomNode : ScriptableObject
    {
        public string id;
        public List<string> parentRoomNodeIDList = new List<string>();
        public List<string> childRoomNodeIDList = new List<string>();
        public RoomNodeGraph roomNodeGraph;

        [Header("Type Room Node")]
        public RoomNodeType roomNodeType;
        public RoomNodeListType roomNodeTypeList;

        #region Editor Code

        [Header("Editor")]
        public Rect rect;

        [Header("Event Mouse Handle")]
        public bool isLeftClickDragging = false;
        public bool isSelected = false;

        public void Init(Rect rect, RoomNodeGraph roomNodeGraph, RoomNodeType roomNodeType, RoomNodeListType roomNodeTypeList)
        {
            this.id = Guid.NewGuid().ToString();
            this.name = "RoomNode";
            this.rect = rect;
            this.roomNodeGraph = roomNodeGraph;
            this.roomNodeType = roomNodeType;
            this.roomNodeTypeList = roomNodeTypeList;
        }

        public void Draw(GUIStyle nodeStyle)
        {
            GUILayout.BeginArea(rect, nodeStyle);
            EditorGUI.BeginChangeCheck();

            if (parentRoomNodeIDList.Count > 0 || roomNodeType.type == RoomType.Entrance)
            {
                EditorGUILayout.LabelField(roomNodeType.RoomName);
            }
            else
            {
                int selected = this.roomNodeTypeList.list.FindIndex(x => x.type == roomNodeType.type);
                int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypeDisplay());

                this.roomNodeType = this.roomNodeTypeList.list[selection];
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }

            GUILayout.EndArea();
        }

        private string[] GetRoomNodeTypeDisplay()
        {
            string[] roomArray = new string[roomNodeTypeList.list.Count];

            for(int i = 0; i < roomNodeTypeList.list.Count; i++)
            {
                if (roomNodeTypeList.list[i].disPlayInRoomNodeGraph)
                {
                    roomArray[i] = roomNodeTypeList.list[i].RoomName;
                }
            }

            return roomArray;
        }

        // Process Events
        #region Process Events
        public void ProcessEvents(Event currentEvent)
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
            if(currentEvent.button == 0)
            {
                ProcessLeftClickDownEvent(false);
            }
            else if(currentEvent.button == 1)
            {
                ProcessLeftClickDownEvent(true);
                roomNodeGraph.roomNodeStartDrawLine = this;
                roomNodeGraph.positionEndLine = currentEvent.mousePosition;
            }
        }

        private void ProcessMouseDragEvent(Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                isLeftClickDragging = true;
                if(!isSelected) isSelected = true;

                var oldPos = this.rect.position;
                this.rect.position += currentEvent.delta*1.1f;
                if(oldPos != this.rect.position)
                {
                    GUI.changed = true;
                    EditorUtility.SetDirty(this);
                }
            }
        }

        private void ProcessMouseUpEvent(Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                isLeftClickDragging = false;
            }
        }

        private void ProcessLeftClickDownEvent(bool isStartDragging)
        {
            Selection.activeObject = this;

            if(!isSelected && isStartDragging)
            {
                isSelected = true;
            }
            else
            {
                isSelected = !isSelected;
            }
        }
        #endregion

        #endregion

        public bool AddParentRoomNodeToList(string parentID)
        {
            if (parentRoomNodeIDList.Contains(parentID))
            {
                return false;
            }

            parentRoomNodeIDList.Add(parentID);
            return true;
        }

        public bool AddChildRoomNodeToList(string childID)
        {
            if (childRoomNodeIDList.Contains(childID))
            {
                return false;
            }

            childRoomNodeIDList.Add(childID);
            return true;
        }

        public bool RemoveParentRoomNodeToList(string parentID)
        {
            if (parentRoomNodeIDList.Contains(parentID))
            {
                parentRoomNodeIDList.Remove(parentID);
                return true;
            }

            return false;
        }

        public bool RemoveChildRoomNodeToList(string childID)
        {
            if (childRoomNodeIDList.Contains(childID))
            {
                childRoomNodeIDList.Remove(childID);
                return true;
            }

            return false;
        }

        public IEnumerable<RoomNode> GetChildRoomNode()
        {
            foreach(var childID  in childRoomNodeIDList)
            {
                yield return GetChildRoomNodeByID(childID);
            }
        }

        private RoomNode GetChildRoomNodeByID(string childId)
        {
            if(roomNodeGraph.roomNodesDictionary.TryGetValue(childId, out var childRoomNode))
            {
                return childRoomNode;
            }

            return null;
        }
    }

}