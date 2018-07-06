using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class Translation : MonoBehaviour
    {
        public RectTransform current;
        public AnimationCurve curve;
        public List<RectTransform> list = new List<RectTransform>();

        private void Awake()
        {
            StartCoroutine(Translate());
        }
        public void View(RectTransform t)
        {
            list.Add(t);
        }
        IEnumerator Translate()
        {
            while (true)
            {


                while (list.Count > 0)
                {
                    Debug.Log(current);
                    RectTransform g = current;
                    RectTransform t = list[0];
                    SetPosition(Vector2.zero, ref g);
                    SetPosition(Vector2.right, ref t);
                    t.gameObject.SetActive(true);
                    g.gameObject.SetActive(true);
                    float f = 0;
                    for (float i = 0; i < 1; i += Time.deltaTime*3)
                    {
                        f = curve.Evaluate(i);
                        SetPosition(-Vector2.right * f, ref g);
                        SetPosition(Vector2.right - Vector2.right * f, ref t);
                        yield return new WaitForEndOfFrame();
                    }
                    SetPosition(Vector2.zero, ref t);
                    g.gameObject.SetActive(false);
                    current = t;
                    list.RemoveAt(0);
                }
                yield return new WaitForEndOfFrame();
            }
        }

        void SetPosition(Vector2 pos, ref RectTransform rect)
        {
            rect.anchorMin = pos;
            rect.anchorMax = pos + Vector2.one;
        }
    }
}
