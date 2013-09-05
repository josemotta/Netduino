namespace NetduinoSerbDemo
{
    public class Program
    {

        public static void Main()
        {
            // uncomment to run simple test that moves robot in ramdom directions
            //new RandomTest(serb).Run();

            // runs the client that reads commands from serial modem (xbee)
            new SerbRemoteClient().Run();
        }
    }
}