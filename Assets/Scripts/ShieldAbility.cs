using UnityEngine;

namespace Assets.Scripts
{
    // Prototype shield state and feedback. Damage handling can query IsActive later.
    public sealed class ShieldAbility : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField, Min(0.1f)] private float duration = 3f;

        private LineRenderer _outline;
        private Material _material;
        private float _expiresAt;

        public bool IsActive { get; private set; }

        private void Awake()
        {
            if (viewCamera == null)
                return;

            var outlineObject = new GameObject("Shield Outline");
            outlineObject.transform.SetParent(viewCamera.transform, false);
            _outline = outlineObject.AddComponent<LineRenderer>();
            _outline.useWorldSpace = false;
            _outline.loop = true;
            _outline.positionCount = 64;
            _outline.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _outline.receiveShadows = false;

            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                _material = new Material(shader);
                _material.color = new Color(0.15f, 0.85f, 1f);
                _outline.sharedMaterial = _material;
            }

            float depth = Mathf.Max(viewCamera.nearClipPlane + 0.1f, 0.5f);
            float halfHeight = viewCamera.orthographic
                ? viewCamera.orthographicSize
                : Mathf.Tan(viewCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * depth;
            float radius = halfHeight * Mathf.Min(1f, viewCamera.aspect) * 0.7f;
            _outline.widthMultiplier = radius * 0.025f;
            for (int i = 0; i < _outline.positionCount; i++)
            {
                float angle = i * (Mathf.PI * 2f / _outline.positionCount);
                _outline.SetPosition(i, new Vector3(
                    Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, depth));
            }
            _outline.enabled = false;
        }

        public void Activate()
        {
            IsActive = true;
            _expiresAt = Time.time + duration;
            if (_outline != null)
                _outline.enabled = true;
        }

        private void Update()
        {
            if (IsActive && Time.time >= _expiresAt)
                Deactivate();
        }

        private void OnGUI()
        {
            if (IsActive)
                GUI.Box(new Rect(Screen.width * 0.5f - 100f, 20f, 200f, 40f), "SHIELD ACTIVE");
        }

        private void OnDisable()
        {
            Deactivate();
        }

        private void Deactivate()
        {
            IsActive = false;
            if (_outline != null)
                _outline.enabled = false;
        }

        private void OnDestroy()
        {
            if (_outline != null)
                Destroy(_outline.gameObject);
            if (_material != null)
                Destroy(_material);
        }
    }
}
