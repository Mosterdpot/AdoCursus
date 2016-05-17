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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Common;
using AdoGereedschap;

namespace AdoWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonBieren_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //var conString = ConfigurationManager.ConnectionStrings["Bieren"];
                //var factory = DbProviderFactories.GetFactory(conString.ProviderName);
                var manager = new BierenDbManager();

                using (var conBieren = manager.GetConnection())
                {
                    //conBieren.ConnectionString = conString.ConnectionString;
                    conBieren.Open();
                    labelStatus.Content = "Bieren geopend";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Content = ex.Message;
            }
        }

        private void buttonBonus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var manager = new RekeningenManager();
                labelStatus.Content = manager.SaldoBonus() + " rekeningen aangepast";
            }
            catch (Exception ex)
            {
                labelStatus.Content = ex.Message;
            }
        }


        private void buttonStorten_Click(object sender, RoutedEventArgs e)
        {
            Decimal teStorten;
            if (decimal.TryParse(textBoxTeStorten.Text, out teStorten))
            {
                try
                {
                    var manager = new RekeningenManager();
                    if (manager.Storten(teStorten, textBoxRekeningNr.Text))
                    { labelStatus.Content = "OK"; }
                    else
                    { labelStatus.Content = "Rekening niet gevonden"; }

                }
                catch (Exception ex)
                { labelStatus.Content = ex.Message; }
            }
            else
            { 
                labelStatus.Content = "Tik een getal bij het storten";
            }
        }

        //private void buttonBierenSA_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        using (var conBieren = new SqlConnection())
        //        {
        //            conBieren.ConnectionString = @"server=.\sqlexpress;database=bieren;user id=sa;password=Vdab123;";
        //            conBieren.Open();
        //            labelStatus.Content = "Bieren geopend";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        labelStatus.Content = ex.Message;
        //    }
        //}
    }
}
