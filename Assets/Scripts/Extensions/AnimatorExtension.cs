using UnityEngine;

namespace Assets.Scripts.Extensions {
    public static class AnimatorExtension {
        public static bool IsPlaying(this Animator animator, params string[] stateNames) {
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            foreach (string stateName in stateNames) {
                if (animatorStateInfo.IsName(stateName)) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsPlaying(this Animator animator) {
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // TODO: アニメーションの繰り返し再生に対応できてないかも
            return animatorStateInfo.length > animatorStateInfo.normalizedTime;
        }
    }
}