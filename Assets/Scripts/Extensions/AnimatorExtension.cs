using UnityEngine;

namespace Assets.Scripts.Extensions {
    public static class AnimatorExtension {
        public static bool IsPlaying(this Animator animator, string stateName) {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }

        public static bool IsPlaying(this Animator animator) {
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // TODO: アニメーションの繰り返し再生に対応できてないかも
            return animatorStateInfo.length > animatorStateInfo.normalizedTime;
        }
    }
}