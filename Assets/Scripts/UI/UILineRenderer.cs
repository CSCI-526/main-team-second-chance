using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : MonoBehaviour
{
    public Vector2[] GetPoints() { return Points; }
    public void SetPoints(Vector2 startPoint, Vector2 endPoint)
    {
        Points[0] = startPoint;
        Points[1] = endPoint;
    }
    public Vector2[] Points = new Vector2[2];
    public Vector2 midPoint = new Vector2();
    [SerializeField]
    private float Thickness = 2.0f;
    private RectTransform LineRect;
    private Image Image;
    public bool GetIsTraversed() { return bIsTraversed; }
    public void SetIsTraversed(bool Value) { bIsTraversed = Value; }
    private bool bIsTraversed = false;
    private void Start()
    {
        LineRect = GetComponent<RectTransform>();
        Image = GetComponent<Image>();
    }
    public void AdjustDimensions()
    {
        if (LineRect == null)
        {
            LineRect = GetComponent<RectTransform>();
        }
        midPoint = (Points[0] + Points[1]) / 2.0f;
        LineRect.position = midPoint;

        Vector2 Direction = Points[0] - Points[1];
        LineRect.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg);
        //LineRect.localScale = new Vector3(Direction.magnitude, 1f, 1f);
        LineRect.sizeDelta = new Vector2(Vector2.Distance(Points[0], Points[1]), Thickness);
    }
    public void SetColor()
    {
        if(Image == null)
        {
            Image = GetComponent<Image>();
        }
        if (!bIsTraversed)
        {
            Image.color = NodeManager.Instance.lockedLevelOutlineColor;
        }
        else
        {
            Image.color = NodeManager.Instance.clearedLevelOutlineColor;
        }
    }
    public void SetColor(Color OverrideColor)
    {
        if (Image == null)
        {
            Image = GetComponent<Image>();
        }
        Image.color = OverrideColor;
    }
}
