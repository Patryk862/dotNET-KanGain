using KanGainNET.Data;
using KanGainNET.Models;
using System.IO.Ports;

public class RFIDReaderService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SerialPort _serialPort;

    public RFIDReaderService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        //_serialPort = new SerialPort("COM6", 115200);

        //_serialPort.DataReceived += SerialPort_DataReceived;

        //_serialPort.Open();
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        string uid = _serialPort.ReadLine().Trim();

        Console.WriteLine($"UID: {uid}");

        HandleCard(uid);
    }

    private void HandleCard(string uid)
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<SilowniaContext>();

        var karta = context.KartyRFID
            .FirstOrDefault(k => k.UID == uid && k.Aktywna);

        if (karta == null)
        {
            Console.WriteLine("Nieznana karta");
            return;
        }

        var ostatnia = context.Obecnosci.Where(o => o.KartaRFIDId == karta.Id).OrderByDescending(o => o.Data).FirstOrDefault();
        string typ = "WEJSCIE";

        if (ostatnia != null && ostatnia.Typ == "WEJSCIE")
        {
             typ = "WYJSCIE";
        }

        var obecnosc = new Obecnosc
        {
            Data = DateTime.Now,
            Typ = typ,
            KartaRFIDId = karta.Id
        };

        context.Obecnosci.Add(obecnosc);

        context.SaveChanges();

        Console.WriteLine("Obecnosc zapisana");
    }
}