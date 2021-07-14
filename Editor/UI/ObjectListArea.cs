using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace RemoteFileExplorer.Editor.UI
{
    public class ObjectListArea : ScrollView
    {
        private VerticalGrid m_Grid = new VerticalGrid();

        private List<ObjectItem> m_Items = new List<ObjectItem>();

        private List<ObjectData> m_Data = new List<ObjectData>();

        private VisualElement m_Content;
        private Label m_EmptyLabel;

        private ObjectItem m_CurSelectItem;

        public Action<ObjectItem> clickItemCallback;
        public Action<ObjectItem> doubleClickItemCallback;
        public Action<ObjectItem> rightClickItemCallback;
        public Action clickEmptyAreaCallback;
        public Action rightClickEmptyAreaCallback;
        public Action<string[]> receiveDragPerformCallback;

        public ObjectListArea() : base(ScrollViewMode.Vertical)
        {
            this.style.width = Length.Percent(100);
            this.style.height = Length.Percent(100);

            m_Content = new VisualElement();
            m_EmptyLabel = new Label("This folder is empty");
            m_EmptyLabel.style.marginTop = 20;
            m_EmptyLabel.style.alignSelf = Align.Center;
            m_EmptyLabel.style.color = Color.gray;
            m_EmptyLabel.style.display = DisplayStyle.None;
            Add(m_Content);
            Add(m_EmptyLabel);

            m_Grid.itemSize = new Vector2(80, 80);
            m_Grid.minHorizontalSpacing = 10;
            m_Grid.verticalSpacing = 10;
            m_Grid.topMargin = 10;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);

            RegisterCallback<DragEnterEvent>(OnDragEnter);
            RegisterCallback<DragLeaveEvent>(OnDragLeave);
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        public void UpdateView(List<ObjectData> list)
        {
            if(list == null) return;
            m_CurSelectItem = null;
            m_Data = list;

            if (list.Count == 0)
            {
                m_EmptyLabel.style.display = DisplayStyle.Flex;
                m_Content.style.display = DisplayStyle.None;
                return;
            }
            m_EmptyLabel.style.display = DisplayStyle.None;
            m_Content.style.display = DisplayStyle.Flex;

            m_Content.Clear();
            m_Grid.fixedWidth = this.contentRect.width;
            m_Grid.InitNumRowsAndColumns(list.Count);
            m_Content.style.width = m_Grid.fixedWidth;
            m_Content.style.height = m_Grid.height;


            for (int i = 0; i < list.Count; i++)
            {
                Rect rect = m_Grid.CalcRect(i);
                var item = new ObjectItem(m_Grid.itemSize);
                item.style.position = Position.Absolute;
                item.style.marginLeft = rect.x;
                item.style.marginTop = rect.y;

                item.clickItemCallback += clickItemCallback;
                item.doubleClickItemCallback += doubleClickItemCallback;
                item.rightClickItemCallback += rightClickItemCallback;

                item.UpdateView(m_Data[i]);
                m_Items.Add(item);
                m_Content.Add(item);
            }
        }

        public void SetSelectItem(ObjectItem item)
        {
            if (m_CurSelectItem != null && m_CurSelectItem != item)
            {
                m_CurSelectItem.UpdateState(ObjectState.Normal);
            }
            m_CurSelectItem = item;
            if (m_CurSelectItem != null)
            {
                m_CurSelectItem.UpdateState(ObjectState.Selected);
            }
        }

        public ObjectItem GetSelectItem()
        {
            return m_CurSelectItem;
        }

        private void OnGeometryChanged(GeometryChangedEvent e)
        {
            UpdateView(m_Data);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if (e.button == 0 || e.button == 1)
            {
                if (clickEmptyAreaCallback != null)
                {
                    clickEmptyAreaCallback();
                }
            }
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (e.button == 1)
            {
                if (rightClickEmptyAreaCallback != null)
                {
                    rightClickEmptyAreaCallback();
                }
            }
        }

        private void OnDragEnter(DragEnterEvent e)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        private void OnDragLeave(DragLeaveEvent e)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.None;
        }

        private void OnDragUpdated(DragUpdatedEvent e)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        private void OnDragPerform(DragPerformEvent e)
        {
            DragAndDrop.AcceptDrag();
            if (receiveDragPerformCallback != null)
            {
                receiveDragPerformCallback(DragAndDrop.paths);
            }
        }
    }

    internal class VerticalGrid
    {
        int m_Columns = 1;
        int m_Rows;
        float m_Height;
        float m_HorizontalSpacing;

        public int columns { get { return m_Columns; } }
        public int rows { get { return m_Rows; } }
        public float height { get { return m_Height; } }
        public float horizontalSpacing { get { return m_HorizontalSpacing; } }

        public float fixedWidth { get; set; }
        public Vector2 itemSize { get; set; }
        public float verticalSpacing { get; set; }
        public float minHorizontalSpacing { get; set; }
        public float topMargin { get; set; }
        public float bottomMargin { get; set; }
        public float rightMargin { get; set; }
        public float leftMargin { get; set; }
        public float fixedHorizontalSpacing { get; set; }
        public bool useFixedHorizontalSpacing { get; set; }

        public void InitNumRowsAndColumns(int itemCount)
        {
            m_Columns = CalcColumns();
            m_Rows = CalcRows(itemCount);
            if (useFixedHorizontalSpacing)
            {
                m_HorizontalSpacing = fixedHorizontalSpacing;
            }
            else
            {
                m_HorizontalSpacing = Mathf.Max(0f, (fixedWidth - (m_Columns * itemSize.x + leftMargin + rightMargin)) / (m_Columns));
                if (m_Rows == 1)
                {
                    m_HorizontalSpacing = minHorizontalSpacing;
                }
            }
            m_Height = m_Rows * (itemSize.y + verticalSpacing) - verticalSpacing + topMargin + bottomMargin;
        }

        public int CalcColumns()
        {
            float horizontalSpacing = useFixedHorizontalSpacing ? fixedHorizontalSpacing : minHorizontalSpacing;
            int cols = (int)Mathf.Floor((fixedWidth - leftMargin - rightMargin) / (itemSize.x + horizontalSpacing));
            cols = Mathf.Max(cols, 1);
            return cols;
        }

        public int CalcRows(int itemCount)
        {
            return (int)Mathf.Ceil(itemCount / (float)CalcColumns());
        }

        public Rect CalcRect(int itemIdx)
        {
            float row = Mathf.Floor(itemIdx / columns);
            float column = itemIdx - row * columns;
            if (useFixedHorizontalSpacing)
            {
                return new Rect(leftMargin + column * (itemSize.x + fixedHorizontalSpacing),
                    row * (itemSize.y + verticalSpacing) + topMargin,
                    itemSize.x,
                    itemSize.y);
            }
            else
            {
                return new Rect(leftMargin + horizontalSpacing * 0.5f + column * (itemSize.x + horizontalSpacing),
                    row * (itemSize.y + verticalSpacing) + topMargin,
                    itemSize.x,
                    itemSize.y);
            }
        }
    }
}