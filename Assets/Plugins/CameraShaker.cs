//
// Based on http://tomori.hatenablog.com/entry/2016/07/30/224913
//
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

[DisallowMultipleComponent, RequireComponent(typeof(Camera))]
sealed public class CameraShaker : MonoBehaviour {
    #region variable
    // 振動のベクトル
    private List<Vector3> m_shakeVectors = new List<Vector3>();
    // 振動の強さ
    private List<Vector3> m_strengths = new List<Vector3>();

    // 振動のTweener
    private List<Tweener> m_shakeTweeners = new List<Tweener>();

    // 以前の座標
    private Vector3? m_previousPosition = null;
    #endregion

    #region event
    private void LateUpdate() {
        AddShakeVector();
    }
    #endregion

    #region method
    /// <summary>
    /// 振動のベクトルを加算する
    /// </summary>
    private void AddShakeVector() {
        if (m_previousPosition == null) {
            foreach (Vector3 shakeVector in m_shakeVectors)
                transform.position += shakeVector;
        } else {
            Vector3 sumShakeVector = Vector3.zero;
            foreach (Vector3 shakeVector in m_shakeVectors)
                sumShakeVector += shakeVector;

            transform.position = m_previousPosition.Value + sumShakeVector;
        }
    }

    /// <summary>
    /// カメラを揺らす
    /// </summary>
    /// <param name="duration">振動時間</param>
    /// <param name="strength">振動の強さ</param>
    /// <param name="vibrato">振動数</param>
    /// <param name="randomness">振動のランダム性 0.0f ≦ randomness ≦ 180.0f</param>
    /// <param name="isStopped">カメラが停止しているか？ 他にカメラを移動させるスクリプトがアタッチされている場合falseにしてください</param>
    public void Shake(float duration = 1.0f, Vector3? strength = null, int vibrato = 10, float randomness = 90.0f, bool isStopped = false) {
        m_previousPosition = m_shakeTweeners.Count == 0 ? null : m_previousPosition;
        m_previousPosition = isStopped ? m_previousPosition ?? (Vector3?)transform.position : null;

        // デフォルト引数のように扱うためnull許容型を使用
        strength = strength ?? new Vector3(0.4f, 0.4f, 0.0f);

        m_shakeVectors.Add(Vector3.zero);
        m_strengths.Add(strength.Value);

        Vector3 sumStrength = Vector3.zero;
        foreach (Vector3 str in m_strengths) {
            sumStrength += str;
        }

        m_shakeTweeners.Add(
            DOTween.Shake(() => Vector3.zero, vector => m_shakeVectors[m_shakeTweeners.Count - 1] = vector, duration, sumStrength, vibrato, randomness)
            .OnComplete(() => {
                for (int i = 0; i < m_shakeTweeners.Count; ++i) {
                    if (!(m_shakeTweeners[i].IsPlaying())) {
                        m_shakeVectors.RemoveAt(i);
                        m_strengths.RemoveAt(i);
                        m_shakeTweeners.RemoveAt(i);
                        break;
                    }
                }
            })
        );
    }
    #endregion
}