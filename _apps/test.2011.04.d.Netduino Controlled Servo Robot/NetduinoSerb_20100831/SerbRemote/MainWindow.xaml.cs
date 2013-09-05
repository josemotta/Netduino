using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerbRemote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerbServer _serb;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (sender, args) => ConnectSerb();
        }

        private void ConnectSerb()
        {
            try
            {
                _serb = new SerbServer(Properties.Settings.Default.SerialPortName,
                                       Properties.Settings.Default.SerialBaudRate);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to communicate with robot:\n" + ex.Message, "Connection error",
                                MessageBoxButton.OK);
            }
        }

        private void CommandButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var power = (byte) sliderPower.Value;
                _serb.SendCommand(SerbServer.Command.SetPower, power);

                var button = (Button)e.Source;
                var arg = (string) button.Tag;
                var duration = (byte) (sliderDuration.Value*1000/100);
                _serb.SendCommand(arg[0], duration);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to communicate with robot:\n" + ex.Message, "Connection error",
                                MessageBoxButton.OK);
            }
        }
    }
}
