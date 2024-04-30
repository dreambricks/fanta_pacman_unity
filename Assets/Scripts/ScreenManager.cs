using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ScreenManager : MonoBehaviour, IPointerDownHandler
{
    public Texture ctaScreen;
    public Texture thankYouScreen;
    public GameObject workingHoursScreen;
    [SerializeField] private ArduinoCommunication arduinoCommunication;

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
    public float tareTime = 3;
    public float dropDelay = 5;
    public float checkWorkingDelay = 5;
    public float elapsedTime;
    public bool isActive = false;

    private bool isCTAActive;

    private RawImage imagePanel;

    void Start()
    {
        UnityEngine.Debug.Log("Start!");
        elapsedTime = playTime;
        imagePanel = GetComponent<RawImage>();
        ShowCTAScreen();
        StartCoroutine(CheckWorkingScreen());
    }

    private void MyOnMouseDown()
    {
        if (isCTAActive)
        {
            UnityEngine.Debug.Log("Pac-Man game!");
            DataLog dataLog = new();
            dataLog.status = StatusEnum.Jogou.ToString();
            LogUtil.SendLogCSV(dataLog);
            StartCoroutine(ShowScreens());
        }
    }
    public void OnPointerDown(PointerEventData p)
    {
        UnityEngine.Debug.Log("OnPointerDown");
        MyOnMouseDown();
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
        arduinoCommunication.SendMessageToArduino("2\n"); // message to start animation at the LED screen

        // Chama a função para trazer a janela do Pac-Man para frente
        BringPacManWindowToFront();

        // Espera por delayTime segundos
        StartChronometer();
        yield return new WaitForSeconds(playTime-tareTime);

        arduinoCommunication.SendMessageToArduino("4\n"); // message to set the tare

        yield return new WaitForSeconds(tareTime);

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
        arduinoCommunication.SendMessageToArduino("1\n"); // message to dispense product

        // Chama a função para trazer a janela Unity para frente
        BringUnityWindowToFront();

        // espera por delayTime segundos e verifica se recebeu uma
        // msg do arduino informando que o produto caiu.
        for (int t = 0; t < delayTime*2; t++)
        {
            yield return new WaitForSeconds(0.5F);
            var msgFromArduino = arduinoCommunication.GetLastestData();
            if (msgFromArduino == "dropped")
            {
                yield return new WaitForSeconds(dropDelay);
                break;
            }
            
        }
        
        BringWindowToBack();
        arduinoCommunication.SendMessageToArduino("3\n"); // message to stop the animation at the LED screen

        elapsedTime = playTime;
        ShowCTAScreen();
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


    public void BringWindowToBack()
    {
        IntPtr unityWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
        UnityEngine.Debug.Log(unityWindowHandle);

        SetWindowPos(unityWindowHandle, (IntPtr)HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
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


    IEnumerator CheckWorkingScreen()
    {
        TimeSpan start = TimeSpan.Parse("20:01"); 
        TimeSpan end = TimeSpan.Parse("09:59");   
        TimeSpan now = DateTime.Now.TimeOfDay;

        while (true)
        {
            if ((now > start) && (now < end))
            {
                workingHoursScreen.gameObject.SetActive(true);
            }
            else
            {
                workingHoursScreen.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(checkWorkingDelay);
            UnityEngine.Debug.Log("CheckWorking");
        }

    }
}
