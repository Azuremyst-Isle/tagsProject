using static System.Environment;

namespace RfidApi.Data;

public class AppDbInternalLogger
{
    public static void WriteLine(string message)
    {
        string path = Path.Combine(GetFolderPath(SpecialFolder.DesktopDirectory), "dblog.txt");
        StreamWriter textFile = File.AppendText(path);
        textFile.WriteLine($"{DateTime.Now}: {message}");
        textFile.Close();
    }
}
