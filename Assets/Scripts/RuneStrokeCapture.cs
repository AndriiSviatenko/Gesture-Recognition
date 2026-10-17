using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class RuneStrokeCapture : MonoBehaviour
    {
        private List<Vector2> _history = new List<Vector2>();
        private float _dist = 4;
        private bool _isDrawing = false;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private RuneTemplate template;
        [SerializeField] private Camera cam;
        [SerializeField] private float drawDist = 1;
        [SerializeField] private int targetCount = 64;
        [SerializeField] private bool recordTemplate;
        [SerializeField] private ShieldAbility shieldAbility;
        [SerializeField, Min(0f)] private float maxMatchDistance = 0.15f;
        [SerializeField, Min(0f)] private float minStrokeLength = 40f;

        public float? LastDistance { get; private set; }

        public void AppendVisualPoint(Vector2 point)
        {
            Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(point.x, point.y, drawDist));
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, worldPoint);
        }

        private void Update()
        {
            if (Mouse.current == null) return;

            if (Mouse.current.leftButton.wasPressedThisFrame && !_isDrawing)
            {
                _history.Clear();
                LastDistance = null;
                lineRenderer.positionCount = 0;

                _history.Add(Mouse.current.position.ReadValue());
                AppendVisualPoint(_history[0]);

                _isDrawing = true;
            }
            else if (Mouse.current.leftButton.isPressed && _isDrawing)
            {
                var mousePos = Mouse.current.position.ReadValue();
                if (Vector2.Distance(mousePos, _history[_history.Count - 1]) > _dist)
                {
                    _history.Add(mousePos);
                    AppendVisualPoint(mousePos);
                }
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame && _isDrawing)
            {
                _isDrawing = false;
                var mousePos = Mouse.current.position.ReadValue();
                if (Vector2.Distance(mousePos, _history[_history.Count - 1]) > 0.001f)
                {
                    _history.Add(mousePos);
                    AppendVisualPoint(mousePos);
                }

                SubmitGesture(_history);
            }
        }

        public bool SubmitGesture(IReadOnlyList<Vector2> rawPoints)
        {
            LastDistance = null;
            if (rawPoints == null || rawPoints.Count < 2 || template == null)
                return false;

            if (GestureResampler.CalculatePathLength(rawPoints) < minStrokeLength)
                return false;

            if (!recordTemplate && (template.points == null || template.points.Count < 2))
                return false;

            int sampleCount = recordTemplate ? Mathf.Max(2, targetCount) : template.points.Count;
            var localHistory = GestureResampler.Resample(rawPoints, sampleCount);
            lineRenderer.positionCount = 0;
            for (int i = 0; i < localHistory.Count; i++)
                AppendVisualPoint(localHistory[i]);

            var normalizedHistory = GestureNormalizer.Center(localHistory);
            var scaledHistory = GestureNormalizer.NormalizeScale(normalizedHistory);
            if (scaledHistory.Count == 0)
                return false;

            if (recordTemplate)
            {
                template.points = new List<Vector2>(scaledHistory);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(template);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(template);
#endif
                recordTemplate = false;
                return false;
            }

            LastDistance = GestureMatcher.CalculateDistance(scaledHistory, template.points);
            bool accepted = LastDistance.Value <= maxMatchDistance && shieldAbility != null;
            lineRenderer.startColor = accepted ? Color.cyan : Color.red;
            lineRenderer.endColor = lineRenderer.startColor;
            if (accepted)
                shieldAbility.Activate();
            return accepted;
        }

        private void OnDisable()
        {
            _history.Clear();
            _isDrawing = false;
            LastDistance = null;
        }
    }
}
