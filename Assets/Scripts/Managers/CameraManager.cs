using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    private float shakeTimer;
    [SerializeField] CinemachineTargetGroup cameraGroup;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cameraPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cameraPerlin.m_AmplitudeGain = 0f;
            }
        }
    }

    public void PlayersToCameraGroup()
    {
        foreach (PlayerInput player in PlayerManager.instance.activePlayers)
        {
            cameraGroup.AddMember(player.gameObject.transform, 1f, 0f);
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cameraPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cameraPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    public void RemoveFromCameraGroup(GameObject player)
    {
        cameraGroup.RemoveMember(player.transform);
    }
}