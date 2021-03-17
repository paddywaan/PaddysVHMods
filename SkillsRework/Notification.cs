using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SkillsRework
{
    internal class Notification : MonoBehaviour
    {
        private const float NotificationTime = 5f;
        private float timeStart;
        private RectTransform rect;
        //private string text = "";
        public string text;
        public float Duration;
        private float timeLeft;
        public Vector3 nextPos;
        RectTransform borderRect;
        bool flag;


        public void Start()
        {
            timeStart = Time.time;
            rect = this.gameObject.GetComponent<RectTransform>();
            borderRect = this.transform.Find("borderOverlay").GetComponent<RectTransform>();
            timeLeft = (timeLeft == 0f) ? NotificationTime : timeLeft;

            Main.log.LogDebug($"{rect}{borderRect}");
            if (!rect || !borderRect) Main.log.LogDebug($"Failed to load assets for Notification {text}: {rect}{borderRect}");
            flag = false;
            this.gameObject.transform.localPosition = new Vector3(-280, -30, 0); //Starting pos
            timeStart = Time.time;
            //this.gameObject.SetActive(true);
        }

        public void Update()
        {
            if (rect && borderRect)
            {
                
                var goPos = this.gameObject.transform.localPosition;
                //Main.log.LogDebug($"{goPos}");
                if (goPos.y < nextPos.y) this.gameObject.transform.localPosition = new Vector3(goPos.x, goPos.y + 1, goPos.z);
                else flag = true;
                if (timeLeft > 0 && flag) timeLeft -= Time.deltaTime;
                if (timeLeft < 0)
                {
                    var cg = this.gameObject.GetComponent<CanvasGroup>();
                    //this.gameObject.GetComponent<Renderer>().
                    //if (!isOffScreen())
                    //{
                    this.gameObject.transform.localPosition = new Vector3(goPos.x, goPos.y + 2, goPos.z);
                        
                    if (cg) cg.alpha -= 0.01f;
                    //}
                    if (cg.alpha <= 0)
                    {
                        NotificationHandler.Instance.Remove(rect.gameObject);
                        Destroy(this.gameObject);
                    }
                }
            }
        }

        public void MoveUp()
        {
            //if(!rect || !borderRect) nextPos = new Vector3(rect.localPosition.x, rect.localPosition.y + 30, rect.localPosition.z);
            //if (rect.localPosition.y % borderRect.rect.height == 0)
            //{
            nextPos = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y + 30, this.gameObject.transform.localPosition.z);
            Main.log.LogDebug($"MovedUp from {this.gameObject.transform.localPosition} to {nextPos}");
            //    return true;
            //}
            //return false;

        }

        public bool isOffScreen()
        {
            if (rect.localPosition.y <= -borderRect.rect.height || rect.localPosition.y >= 210) return true;
            return false;
        }

        public bool isMoving()
        {
            if (this.timeLeft < 0) return false;
            //Main.log.LogDebug($"comparing {this.gameObject.transform.localPosition} to {nextPos}");
            if (this.gameObject.transform.localPosition.Equals(nextPos) || nextPos.Equals(Vector3.zero))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sourced from: https://forum.unity.com/threads/simple-ui-animation-fade-in-fade-out-c.439825/
        /// </summary>
        /// <param name="img"></param>
        /// <param name="fadeAway"></param>
        /// <returns></returns>
        IEnumerator FadeImage(Image img, bool fadeAway)
        {
            // fade from opaque to transparent
            if (fadeAway)
            {
                // loop over 1 second backwards
                for (float i = 1; i >= 0; i -= Time.deltaTime)
                {
                    // set color with i as alpha
                    img.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }
            // fade from transparent to opaque
            else
            {
                // loop over 1 second
                for (float i = 0; i <= 1; i += Time.deltaTime)
                {
                    // set color with i as alpha
                    img.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }
        }

        /// <summary>
        /// sourced from: https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/
        /// </summary>
        /// <param name="t"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public IEnumerator FadeTextToFullAlpha(float t, Text i)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
            while (i.color.a < 1.0f)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
                yield return null;
            }
        }

        /// <summary>
        /// sourced from: https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/
        /// </summary>
        /// <param name="t"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public IEnumerator FadeTextToZeroAlpha(float t, Text i)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
            while (i.color.a > 0.0f)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
                yield return null;
            }
        }
    }
}