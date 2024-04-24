using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public Texture ctaScreen;
    public Texture thankYouScreen;
    //[SerializeField] private ArduinoCommunication arduinoCommunication;

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const int HWND_TOPMOST = -1;
    private const int HWND_NOTOPMOST = -2;

    public float playTime = 5;
    public float delayTime = 5;
    public float elapsedTime;
    public bool isActive = false;

    private bool isCTAActive;

    private RawImage imagePanel;

    void Start()
    {
        elapsedTime = playTime;
        imagePanel = GetComponent<RawImage>();
        ShowCTAScreen();
    }

    private void OnMouseDown()
    {
        if (isCTAActive)
        {
            UnityEngine.Debug.Log("Start!");
            DataLog dataLog = new();
            dataLog.status = StatusEnum.Jogou.ToString();
            LogUtil.SendLogCSV(dataLog);
            StartCoroutine(ShowScreens());
        }
    }

    private void Update()
    {
        Chronometer();
    }


    public void ShowCTAScreen()
    {
        isCTAActive = true;
        imagePanel.texture = ctaScreen;
    }

    public void ShowThankYouScreen()
    {
        isCTAActive = false;
        imagePanel.texture = thankYouScreen;
    }

    IEnumerator ShowScreens()
    {
        //arduinoCommunication.SendMessage("1");

        // Chama a função para trazer a janela do Pac-Man para frente
        BringPacManWindowToFront();

        // Espera por delayTime segundos
        StartChronometer();
        yield return new WaitForSeconds(playTime);

        ShowThankYouScreen();

        // Chama a função para trazer a janela Unity para frente
        StartCoroutine(BringUnityWindowToFrontCoroutine());
    }

    public void BringUnityWindowToFront()
    {
        // Pega a janela principal da aplicação Unity
        IntPtr unityWindowHandle = Process.GetCurrentProcess().MainWindowHandle;

        // Define a janela da Unity como o topo da pilha de janelas
        SetWindowPos(unityWindowHandle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
    }

    IEnumerator BringUnityWindowToFrontCoroutine()
    {
        //arduinoCommunication.SendMessage("0");

        // Chama a função para trazer a janela Unity para frente
        BringUnityWindowToFront();

        // Espera por delayTime segundos
        yield return new WaitForSeconds(delayTime);
        BringWindowToBack();

        SceneManager.LoadScene("SampleScene");
    }

    public void BringPacManWindowToFront()
    {
        isCTAActive = false;
        // Encontra a janela do Pac-Man pelo título da janela
        IntPtr pacManWindowHandle = FindWindow(null, "PAC-MAN");

        if (pacManWindowHandle != IntPtr.Zero)
        {
            SetForegroundWindow(pacManWindowHandle);
        }
    }

    void Chronometer()
    {
        if (isActive)
        {

            elapsedTime -= Time.deltaTime;

            if (elapsedTime <= 0f)
            {
                elapsedTime = 0f;
                isActive = false;
            }
        }
    }

    void StartChronometer()
    {
        elapsedTime = playTime;
        isActive = true;
    }




    public void BringWindowToBack()
    {
        IntPtr unityWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
        UnityEngine.Debug.Log(unityWindowHandle);

        SetWindowPos(unityWindowHandle, (IntPtr)HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
    }
}
