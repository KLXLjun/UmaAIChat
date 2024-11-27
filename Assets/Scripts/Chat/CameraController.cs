using NAudio.MediaFoundation;
using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public RectTransform canvas;
    public GameObject ChatBox;
    public GameObject UIObject;
    public GameObject ResultBox;

    private float cameraSpeed = 0.05f;

    private Vector3 offset;
    private float timer = 0;
    private bool isDragging = false;
    private bool displayUI = true;

    private static UmaViewerBuilder Builder => UmaViewerBuilder.Instance;
    private static UmaViewerUI UI => UmaViewerUI.Instance;
    private static CameraOrbit CameraOrbit => CameraOrbit.instance;

    private static GameObject charaObject;
    private static GameObject charaFace;

    void Start()
    {
        Builder.OnNormalUmaModelLoadComplete += UmaModelLoadComplete;
    }

    public void UmaModelLoadComplete(CharaEntry chara)
    {
        //Builder.CurrentUMAContainer.TrackTarget.transform.position.Set(Builder.CurrentUMAContainer.TrackTarget.transform.position.x, Builder.CurrentUMAContainer.TrackTarget.transform.position.y + 1.0f, Builder.CurrentUMAContainer.TrackTarget.transform.position.z);
    }

    public void ResetOrbitCameraValue()
    {
        CameraOrbit.OrbitCamFovSlider.value = 40.0f;
        CameraOrbit.OrbitCamZoomSlider.value = 3.0f;
        CameraOrbit.OrbitCamZoomSpeedSlider.value = 0.2f;
        CameraOrbit.OrbitCamTargetHeightSlider.value = 1.0f;
        CameraOrbit.OrbitCamHeightSlider.value = 1.0f;
        CameraOrbit.OrbitCamRotationSlider.value = 0.0f;
        CameraOrbit.OrbitCamSpeedSlider.value = 0.5f;
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.transform.root.gameObject.name.StartsWith("Chara_"))
            {
                if (charaObject == null)
                {
                    charaObject = hit.collider.transform.root.gameObject;
                    var Last = Utils.FindTransform(charaObject.transform, "Chara_");
                    var Face = Utils.FindTransform(Last.transform, "M_Face");
                    if (Face != null)
                    {
                        charaFace = Face.gameObject;
                        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(charaFace.transform.position);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform.root.gameObject.name.StartsWith("Chara_"))
                {
                    displayUI = !displayUI;

                    ChatBox.SetActive(displayUI);
                    UIObject.SetActive(displayUI);
                    ResultBox.SetActive(displayUI);

                    charaObject.transform.position = Vector3.zero;
                    Camera.main.transform.position = new Vector3(0, 1, 2.4f);
                    Camera.main.transform.rotation = Quaternion.Euler(0, 180, 0);
                    ResetOrbitCameraValue();

                    Camera.main.orthographic = !displayUI;
                    if (Camera.main.orthographic)
                    {
                        Camera.main.orthographicSize = 1.0f;
                    }
                }
            }
        }

        if (!Camera.main.orthographic) return;
        if (Builder.CurrentUMAContainer == null) return;

        float mouseCenter = Input.GetAxis("Mouse ScrollWheel");
        if (mouseCenter > 0.0f)
        {
            Camera.main.orthographicSize = Camera.main.orthographicSize - cameraSpeed;
        }
        if (mouseCenter < 0.0f)
        {
            Camera.main.orthographicSize = Camera.main.orthographicSize + cameraSpeed;
        }

        if (charaObject == null) return;
        if (Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;
            if (!isDragging)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = charaObject.transform.position.z;
                offset = charaObject.transform.position - mouseWorldPos;
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if(timer < 0.3f)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    ChatBox.SetActive(!ChatBox.activeSelf);
                    ResultBox.SetActive(ChatBox.activeSelf);
                }
            }

            isDragging = false;
            timer = 0;
            charaObject = null;
        }

        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = charaObject.transform.position.z;
            charaObject.transform.position = mouseWorldPos + offset;

            Bounds bounds = Utils.CalculateBounds(charaObject);

            // 将包围盒的最小和最大点转换为屏幕坐标
            Vector3 screenMin = Camera.main.WorldToScreenPoint(bounds.min);
            Vector3 screenMax = Camera.main.WorldToScreenPoint(bounds.max);

            // 计算宽度和高度
            float width = Mathf.Abs(screenMax.x - screenMin.x);
            float height = Mathf.Abs(screenMax.y - screenMin.y);

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(charaFace.transform.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas,
                screenPosition,
                Camera.current, // 用于渲染 Canvas 的相机
                out Vector2 localPoint
            );

            localPoint.y = localPoint.y + (height * 0.5f);
            ResultBox.transform.localPosition = localPoint;
        }
    }

    public void ChangeCameraMode()
    {
        Camera.main.orthographic = !Camera.main.orthographic;
        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize = 1.0f;
        }
        else
        {
            ResetOrbitCameraValue();
            //Camera.main.transform.rotation = Quaternion.identity;
        }
    }
}
