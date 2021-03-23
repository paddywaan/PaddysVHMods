using System;
using UnityEngine;

namespace SkillfulProgression
{
    public class SkillTracker : MonoBehaviour
    {
        public float LastHitTaken { get; set; }
        public float LastAttackStart { get; set; }

        public void Awake()
        {
            LastHitTaken = 0f;
            LastAttackStart = 0f;
        }
    }
}