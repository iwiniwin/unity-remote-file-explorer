using UnityEngine.UIElements;
using UnityEngine;

namespace URFS.Editor.UI 
{
    public class ObjectListArea : ScrollView 
    {
        private VerticalGrid m_Grid = new VerticalGrid();
        public ObjectListArea() : base(ScrollViewMode.Vertical)
        {
            this.style.height = Length.Percent(100);

            m_Grid.itemSize = new Vector2(80, 80);
            m_Grid.minHorizontalSpacing = 10;
            m_Grid.verticalSpacing = 10;
            
            
        }

        public void UpdateView()
        {
            m_Grid.fixedWidth = this.contentRect.width;

            int cols = m_Grid.CalcColumns();

            for(int i = 0; i < 10; i ++)
            {
                // Add(new ObjectItem());
                AddRow(cols);
            }
        }

        public void AddRow(int cols)
        {
            VisualElement v = new VisualElement();
            v.style.width = Length.Percent(100);
            v.style.flexDirection = FlexDirection.Row;
            v.style.justifyContent = Justify.SpaceAround;

            v.style.marginTop = m_Grid.verticalSpacing;
            for(int i = 0; i < cols; i ++)
            {
                var item = new ObjectItem(m_Grid.itemSize);
                item.UpdateView(new ObjectData(ObjectType.File, "aasssssssssssddddddd.cs"));
                v.Add(item);
            }
            Add(v);
        }
    }

    internal class VerticalGrid
    {
        int m_Columns = 1;
        int m_Rows;
        float m_Height;
        float m_HorizontalSpacing;

        public int columns {get {return m_Columns;}}
        public int rows {get {return m_Rows;}}
        public float height {get {return m_Height;}}
        public float horizontalSpacing {get {return m_HorizontalSpacing;}}

        public float fixedWidth {get; set;}
        public Vector2 itemSize {get; set;}
        public float verticalSpacing {get; set;}
        public float minHorizontalSpacing {get; set;}
        public float topMargin {get; set;}
        public float bottomMargin {get; set;}
        public float rightMargin {get; set;}
        public float leftMargin {get; set;}
        public float fixedHorizontalSpacing {get; set;}
        public bool useFixedHorizontalSpacing {get; set;}

        public int CalcColumns()
        {
            float horizontalSpacing = useFixedHorizontalSpacing ? fixedHorizontalSpacing : minHorizontalSpacing;
            int cols = (int)Mathf.Floor((fixedWidth - leftMargin - rightMargin) / (itemSize.x + horizontalSpacing));
            cols = Mathf.Max(cols, 1);
            return cols;
        }
    }
}