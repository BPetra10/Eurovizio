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
using System.IO;
using Microsoft.Win32;

namespace Eurovizio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        List<Versenyzo> adatok = new List<Versenyzo>(); //Ebbe kerül az összes adat

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AllomanyOlvas(string allomanynev)
        {
            try
            {
                foreach (var item in File.ReadAllLines(allomanynev))
                {
                    adatok.Add(new Versenyzo(item));
                }
                adatokGrid.ItemsSource = adatok;
                adatokGrid.Columns[0].Header = "Év";
                adatokGrid.Columns[1].Header = "Ország";
                adatokGrid.Columns[2].Header = "Előadó";
                adatokGrid.Columns[3].Header = "Cím";
                adatokGrid.Columns[4].Header = "Helyezés";
                adatokGrid.Columns[5].Header = "Pontszám";
                adatokFelvitelGrid.ItemsSource = adatok;
                List<double> atlagok = new List<double>();
                foreach (var item in adatok.OrderBy(x => x.Orszag).GroupBy(x => x.Orszag))
                {
                    orszagok.Items.Add(item.Key);
                    atlagok.Add(item.Average(x => x.Pontszam));
                }
                double maximum = atlagok.Max();
                helyezes.Maximum = maximum;
                Versenyzo.Atlag = Math.Round(adatok.Average(x => x.Pontszam), 0);
                atlagKiir.Content = "Átlagos pontszám: " + Versenyzo.Atlag;
            }
            catch(Exception hiba)
            {
                MessageBox.Show("Az állomány beolvasása során hiba történt. Hiba szövege: " + hiba.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AllomanyOlvas("eurovizio.txt");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var elsoHelyezesek = adatok.
                Where(x => x.Helyezes == 1).
                OrderByDescending(x => x.Ev);
            var kiir = elsoHelyezesek.Select(x => new
            {
                Év = x.Ev,
                Ország = x.Orszag,
                Előadó = x.Eloado,
                Cím = x.Cim,
                Pontszám = x.Pontszam
            });
            lekerdezesGrid.ItemsSource = kiir;
            lekerdezesCimke.Content = "Első helyezések:";
            //lekerdezesGrid.Columns[4].Visibility = Visibility.Hidden;
        }

        private void orszagok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int kivasztottIndex = orszagok.SelectedIndex; //A kiválasztott elem indexe
            string orszag = orszagok.Items[kivasztottIndex].ToString(); //A kiválasztott elem szövege
            lekerdezesCimke.Content = orszag + " versenyzői:";
            var orszagLekerdez = adatok.Where(x => x.Orszag == orszag);
            lekerdezesGrid.ItemsSource = orszagLekerdez;
            lekerdezesGrid.Columns[1].Visibility = Visibility.Hidden;
            double orszagAtlag = Math.Round(orszagLekerdez.Average(x => x.Pontszam),0);
            if (orszagAtlag < Versenyzo.Atlag - 10)
            {
                orszagAtlagKiir.Background = new SolidColorBrush(Color.FromRgb(236, 28, 36));
                helyezes.Foreground = new SolidColorBrush(Color.FromRgb(236, 28, 36));
            }
            else if (orszagAtlag > Versenyzo.Atlag + 10)
            {
                orszagAtlagKiir.Background = new SolidColorBrush(Color.FromRgb(14, 209, 69));
                helyezes.Foreground = new SolidColorBrush(Color.FromRgb(14, 209, 69));
            }
            else
            {
                orszagAtlagKiir.Background = new SolidColorBrush(Color.FromRgb(255, 202, 24));
                helyezes.Foreground = new SolidColorBrush(Color.FromRgb(255, 202, 24));
            }
            orszagAtlagKiir.Content = orszag + " átlagos pontszáma: " + orszagAtlag;
            helyezes.Value = orszagAtlag;
        }

        private void Ellenoriz()
        {
            string _ev = evBevitel.Text;
            string _orszag = orszagBevitel.Text;
            string _eloado = eloadoBevitel.Text;
            string _cim = cimBevitel.Text;
            string _helyezes = helyezesBevitel.Text;
            string _pontszam = pontszamBevitel.Text;
            bool jo = true;
            hibalista.Text = "";
            try
            {
                short ev = Convert.ToInt16(_ev);
                if (ev < 1956 || ev > DateTime.Now.Year)
                    throw new FormatException();

            }
            catch(FormatException)
            {
                hibalista.Text += "Az év csak 1956 és aktuális év közötti érték lehet.\n";
                jo = false;
            }
            if (_eloado == "" || _orszag == "" || _cim == "")
            {
                hibalista.Text += "Az előadó, ország és cím mezők nem lehetnek üresek.\n";
                jo = false;
            }
            try
            {
                short helyezes = Convert.ToInt16(_helyezes);
                if (helyezes < 1 || helyezes > 1000)
                    throw new FormatException();

            }
            catch (FormatException)
            {
                hibalista.Text += "A helyezés csak 1 és 1000 között lehet.\n";
                jo = false;
            }
            try
            {
                short pontszam = Convert.ToInt16(_pontszam);
                if (pontszam < 0 || pontszam > 1000)
                    throw new FormatException();

            }
            catch (FormatException)
            {
                hibalista.Text += "A pontszám csak 0 és 1000 között lehet.";
                jo = false;
            }
            if (jo == true)
            {
                felvitel.IsEnabled = true;
                if (adatokFelvitelGrid.SelectedIndex != -1)
                    modositas.IsEnabled = true;
            }
            else
            {
                felvitel.IsEnabled = false;
                modositas.IsEnabled = false;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string _ev = evBevitel.Text;
            string _orszag = orszagBevitel.Text;
            string _eloado = eloadoBevitel.Text;
            string _cim = cimBevitel.Text;
            string _helyezes = helyezesBevitel.Text;
            string _pontszam = pontszamBevitel.Text;
            adatok.Add(new Versenyzo(_ev, _orszag, _eloado, _cim, _helyezes, _pontszam));
            /*
            adatokFelvitelGrid.ItemsSource = null;
            adatokFelvitelGrid.ItemsSource = adatok;
            */
            adatokFelvitelGrid.Items.Refresh();
            adatokGrid.Items.Refresh();
        }

        private void evBevitel_TextChanged(object sender, TextChangedEventArgs e)
        {
            Ellenoriz();
        }

        private void adatokFelvitelGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (adatokFelvitelGrid.SelectedIndex != -1)
            {
                int index = adatokFelvitelGrid.SelectedIndex;
                evBevitel.Text = adatok[index].Ev.ToString();
                orszagBevitel.Text = adatok[index].Orszag.ToString();
                eloadoBevitel.Text = adatok[index].Eloado.ToString();
                cimBevitel.Text = adatok[index].Cim.ToString();
                helyezesBevitel.Text = adatok[index].Helyezes.ToString();
                pontszamBevitel.Text = adatok[index].Pontszam.ToString();
                modositas.IsEnabled = true;
                torles.IsEnabled = true;
            }
            else
            {
                modositas.IsEnabled = false;
                torles.IsEnabled = false;
            }
        }

        private void modositas_Click(object sender, RoutedEventArgs e)
        {
            int index = adatokFelvitelGrid.SelectedIndex;
            adatok[index].Ev = Convert.ToInt16(evBevitel.Text);
            adatok[index].Orszag = orszagBevitel.Text;
            adatok[index].Eloado = eloadoBevitel.Text;
            adatok[index].Cim = cimBevitel.Text;
            adatok[index].Helyezes = Convert.ToByte(helyezesBevitel.Text);
            adatok[index].Pontszam = Convert.ToInt16(pontszamBevitel.Text);
            adatokFelvitelGrid.Items.Refresh();
            adatokGrid.Items.Refresh();
        }

        private void torles_Click(object sender, RoutedEventArgs e)
        {
            int index = adatokFelvitelGrid.SelectedIndex;
            adatok.RemoveAt(index);
            adatokFelvitelGrid.Items.Refresh();
            adatokGrid.Items.Refresh();
        }

        private void mentes_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog allomanyMentes = new SaveFileDialog();
            allomanyMentes.Filter = "Szöveges állományok (*.txt)|*.txt|Minden állomány (*.*)|*.*";
            if (allomanyMentes.ShowDialog()==true)
            {
                StreamWriter sw = new StreamWriter(allomanyMentes.FileName);
                foreach (var item in adatok)
                {
                    string sor = string.Join(";", item.Ev, item.Orszag, item.Eloado, item.Cim, item.Helyezes, item.Pontszam);
                    sw.WriteLine(sor);
                }
                sw.Close();
            }

        }

        private void betoltes_Click(object sender, RoutedEventArgs e)
        {
            adatokGrid.ItemsSource = null;
            adatokFelvitelGrid.ItemsSource = null;
            adatok.Clear();
            orszagok.Items.Clear();
            atlagKiir.Content = "Átlagos pontszám: ";
            OpenFileDialog allomanyBetoltes = new OpenFileDialog();
            allomanyBetoltes.Filter = "Szöveges állományok (*.txt)|*.txt|Minden állomány (*.*)|*.*";
            if (allomanyBetoltes.ShowDialog() == true)
                AllomanyOlvas(allomanyBetoltes.FileName);
        }
    }
}
