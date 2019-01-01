using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Unity 引擎的 mesh 顶点顺序如下：
/// 
/// (0)-------(1)(4)-------(5)
///  |         |  |         |
///  |         |  |         |
/// (3)-------(2)(7)-------(6)
/// 
/// 上面这个四边形的三角形 index 如下：
/// 
/// 0 1 2, 2 3 0, 4 5 6, 6 7 4
/// 
/// 其中，1 和 4 共用一个顶点，2 和 7 共用一个顶点，一共 4 个三角形，6 个顶点
/// 
/// </summary>
[RequireComponent(typeof(Text))]
public class Underline : BaseMeshEffect
{
    private static readonly List<UIVertex> verts = new List<UIVertex>();
    private static readonly UIVertex[] data = new UIVertex[4];

    public Text text;

    public Color UnderlineColor = Color.black;
    public float LineOffset = 1;
    public float LineThickness = 2;

    protected Underline() { }

    protected override void Awake()
    {
        base.Awake();
        text = GetComponent<Text>();
        if (text.font == null)
        {
            Destroy(this);
            Debug.LogWarning("Text.font is null");
            return;
        }
        text.RegisterDirtyMaterialCallback(OnFontMaterialChanged);
        text.font.RequestCharactersInTexture("*", text.fontSize, text.fontStyle);
    }

    protected override void OnDestroy()
    {
        text.UnregisterDirtyMaterialCallback(OnFontMaterialChanged);
        base.OnDestroy();
    }

    private void OnFontMaterialChanged()
    {
        text.font.RequestCharactersInTexture("*", text.fontSize, text.fontStyle);
    }

    private Vector2 GetUnderlineCharUV()
    {
        var ch = '*';
        CharacterInfo info;
        if (text.font.GetCharacterInfo(ch, out info, text.fontSize, text.fontStyle))
        {
            return (info.uvBottomLeft + info.uvBottomRight + info.uvTopLeft + info.uvTopRight) * 0.25f;
        }
        Debug.LogWarning("GetCharacterInfo failed");
        return Vector2.zero;
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;
        if (vh.currentVertCount < 3)
            return;

        verts.Clear();
        vh.GetUIVertexStream(verts);

        var neededCpacity = verts.Count + 6;
        if (verts.Capacity < neededCpacity)
            verts.Capacity = neededCpacity;

        UIVertex vert = UIVertex.simpleVert;

        vh.PopulateUIVertex(ref vert, 3);
        var pos_a = vert.position;
        vh.PopulateUIVertex(ref vert, verts.Count * 2 / 3 - 2);
        var pos_b = vert.position;
        var y = (pos_a.y + pos_b.y) * 0.5f;

        var pos0 = new Vector3(pos_a.x, y - LineOffset);
        var pos1 = new Vector3(pos_b.x, y - LineOffset);
        var pos2 = new Vector3(pos_b.x, y - LineOffset - LineThickness);
        var pos3 = new Vector3(pos_a.x, y - LineOffset - LineThickness);

        vert.color = UnderlineColor;
        vert.uv0 = GetUnderlineCharUV();

        vert.position = pos0;
        data[0] = vert;
        vert.position = pos1;
        data[1] = vert;
        vert.position = pos2;
        data[2] = vert;
        vert.position = pos3;
        data[3] = vert;

        vh.AddUIVertexQuad(data);
    }
}