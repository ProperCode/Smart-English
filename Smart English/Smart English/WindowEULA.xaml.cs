using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Smart_English
{
    /// <summary>
    /// Interaction logic for WindowEULA.xaml
    /// </summary>
    public partial class WindowEULA : Window
    {
        public WindowEULA()
        {
            try
            {
                InitializeComponent();
                TB.IsReadOnly = true;

                TB.Text = "MIT License"
+ "\n\nSmart English"
+ "\n\nCopyright © 2026 Mikołaj Magowski"
+ "\n\nPermission is hereby granted, free of charge, to any person obtaining a copy"
+ "\nof this software and associated documentation files(the \"Software\"), to deal"
+ "\nin the Software without restriction, including without limitation the rights"
+ "\nto use, copy, modify, merge, publish, distribute, sublicense, and/ or sell"
+ "\ncopies of the Software, and to permit persons to whom the Software is"
+ "\nfurnished to do so, subject to the following conditions:"
+ "\n\nThe above copyright notice and this permission notice shall be included in all"
+ "\ncopies or substantial portions of the Software."
+ "\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR"
+ "\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,"
+ "\nFITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE"
+ "\nAUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER"
+ "\nLIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,"
+ "\nOUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE"
+ "\nSOFTWARE.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd WE001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
