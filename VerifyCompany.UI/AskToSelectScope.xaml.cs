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

namespace VerifyCompany.UI
{
    /// <summary>
    /// Logika interakcji dla klasy AskToSelectScope.xaml
    /// </summary>
    public partial class AskToSelectScope : Window
    {
        public AskToSelectScope(List<string> scopesToAnalyze)
        {
            InitializeComponent();

            this.scopeCB.ItemsSource = scopesToAnalyze;
        }

        public string SelectedScope { get; internal set; }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (scopeCB.SelectedItem != null)
            {
                SelectedScope = scopeCB.SelectedItem.ToString();

                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }
            this.Close();
        }
    }
}
