using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfTCPServer
{
    /// <summary>
    /// sendMessageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class sendMessageDialog : Window
    {
        public string text_str => textBox.Text;
        public bool msgbox = false;
        public sendMessageDialog()
        {
            InitializeComponent();
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            msgbox = (checkBox.IsChecked == true) ? true : false;
            Close();
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
