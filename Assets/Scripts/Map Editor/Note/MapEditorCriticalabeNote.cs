using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MapEditorCriticalabeNote : MonoBehaviour
{
    public Sprite basicSprite;
    public Sprite criticalSprite;
    [SerializeField] bool startMode;

    bool isCritical;
    public bool IsCritical
    {
        get { return isCritical; }
        set
        {
            isCritical = value;
            if (isCritical)
            {
                image.sprite = criticalSprite;
            }
            else
            {
                image.sprite = basicSprite;
            }
        }
    }

    Image image;

    public void Start()
    {
        image = GetComponent<Image>();
        IsCritical = startMode;
    }
}
