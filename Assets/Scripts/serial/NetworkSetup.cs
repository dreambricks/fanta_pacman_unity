
[System.Serializable]
public class NetworkSetup
{

    private string serialPort;
    private int baudrate;

    public NetworkSetup()
    {
    }

    public NetworkSetup(string serialPort, int baudrate)
    {
        this.serialPort = serialPort;
        this.baudrate = baudrate;
    }


    public string SerialPort { get => serialPort; set => serialPort = value; }
    public int Baudrate { get => baudrate; set => baudrate = value; }
}
