using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Eurovizio
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<Versenyzo> adatok = new List<Versenyzo>();
		public MainWindow()
		{
			InitializeComponent();
		}

		private void AllomanyOlvas(string allomanyNev)
		{
			try
			{
				foreach (var item in File.ReadAllLines(allomanyNev))
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
				double max = atlagok.Max();
				helyezes.Maximum = max;
				Versenyzo.Atlag = Math.Round(adatok.Average(x => x.Pontszam), 0);
				atlagKiir.Content = "Átlagos pontszám: " + Versenyzo.Atlag;
			}
			catch (Exception e)
			{
				MessageBox.Show("Hiba a beolvasáskor:"+e.Message);
			}
			
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			AllomanyOlvas("eurovizio.txt");
		}

		private void elsoHely_Click(object sender, RoutedEventArgs e)
		{
			var elsoHelyezesek = adatok.Where(x => x.Helyezes == 1).OrderBy(x=>x.Ev);
			lekerdezesGrid.ItemsSource = elsoHelyezesek;
			lekerdezesGrid.Columns[4].Visibility = Visibility.Hidden; //Így nem írja ki a helyezés oszlopot
		}

		private void orszagok_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int kivalasztottIndex = orszagok.SelectedIndex;
			string orszag = orszagok.Items[kivalasztottIndex].ToString(); //kiválasztott index szövege
			lekerdezesCimke.Content = orszag + " versenyzői:";
			var Orszaglekerdez = adatok.Where(x=>x.Orszag == orszag);
			lekerdezesGrid.ItemsSource = Orszaglekerdez;
			lekerdezesGrid.Columns[1].Visibility = Visibility.Hidden;
			//adott ország átlagos pontszáma:
			double orszagatlag = Math.Round(Orszaglekerdez.Average(x=>x.Pontszam),0);
			if (orszagatlag<Versenyzo.Atlag-10)
			{
				orszagAtlagKiir.Background = new SolidColorBrush(Color.FromRgb(236, 28, 15));
				helyezes.Foreground = new SolidColorBrush(Color.FromRgb(236, 28, 15));
			}
			else if (orszagatlag < Versenyzo.Atlag + 10)
			{
				orszagAtlagKiir.Background = new SolidColorBrush(Color.FromRgb(14, 209, 69));
				helyezes.Foreground = new SolidColorBrush(Color.FromRgb(14, 209, 69));
			}
			else
			{
				orszagAtlagKiir.Background = new SolidColorBrush(Color.FromRgb(255, 202, 24));
				helyezes.Foreground = new SolidColorBrush(Color.FromRgb(255, 202, 24));
			}
			orszagAtlagKiir.Content = orszag + " átlagos pontszáma: " + orszagatlag;
			helyezes.Value = orszagatlag;
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
			hibaLista.Text = "";

			try
			{
				short ev = Convert.ToInt16(_ev);
				if (ev < 1956 || ev > DateTime.Now.Year)
					throw new FormatException();

			}
			catch (FormatException)
			{
				hibaLista.Text += "Az év csak 1956 és aktuális év közötti érték lehet.\n";
				jo = false;
			}

			if (_eloado == "" || _orszag == "" || _cim == "")
			{
				hibaLista.Text += "Az előadó, ország és cím mezők nem lehetnek üresek.\n";
				jo = false;
			}

			try
			{
				byte byteValue;
				bool result = Byte.TryParse(_helyezes, out byteValue);
				if (!result)
					throw new Exception();
			}
			catch (Exception)
			{
				hibaLista.Text += "A helyezés csak 1 és 255 között lehet.\n";
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
				hibaLista.Text += "A pontszám csak 0 és 1000 között lehet.";
				jo = false;
			}

			if (jo == true)
			{
				adatFelv.IsEnabled = true;
				if (adatokFelvitelGrid.SelectedIndex != -1)
					modositas.IsEnabled = true;
			}
			else
			{
				adatFelv.IsEnabled = false;
				modositas.IsEnabled = false;
			}
		}

		private void adatFelv_Click(object sender, RoutedEventArgs e)
		{
			string ev = evBevitel.Text;
			string orszag = orszagBevitel.Text;
			string eloado = eloadoBevitel.Text;
			string cim = cimBevitel.Text;
			string helyezes = helyezesBevitel.Text;
			string pontszam = pontszamBevitel.Text;
			adatok.Add(new Versenyzo(ev,orszag,eloado,cim,helyezes,pontszam));
			adatokFelvitelGrid.Items.Refresh();
			adatokGrid.Items.Refresh();
		}

		private void Bevitel_TextChanged(object sender, TextChangedEventArgs e)
		{
			// Csak szám bevitelének engedélyezése:
			int parsed1;
			if (!int.TryParse(evBevitel.Text, out parsed1))
			{
				evBevitel.Text = "";
			}
			int parsed2;
			if (!int.TryParse(pontszamBevitel.Text, out parsed2))
			{
				pontszamBevitel.Text = "";
			}
			int parsed3;
			if (!int.TryParse(helyezesBevitel.Text, out parsed3))
			{
				helyezesBevitel.Text = "";
			}
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
			allomanyMentes.Filter = "txt állományok(*txt)|*.txt|Összes állomány(*.*)|*.*"; //txt-t lehet menteni

			if (allomanyMentes.ShowDialog() == true)
			{
				StreamWriter sw = new StreamWriter(allomanyMentes.FileName);
				foreach (var item in adatok)
				{
					string sor = string.Join(";",item.Ev,item.Orszag,item.Eloado,item.Cim,item.Helyezes,item.Pontszam);
					sw.WriteLine(sor);
				}
				sw.Close();
			}
		}

		private void betolt_Click(object sender, RoutedEventArgs e)
		{
			adatokGrid.ItemsSource = null;
			adatokFelvitelGrid.ItemsSource = null;
			adatok.Clear();
			orszagok.Items.Clear();
			atlagKiir.Content = "Átlagos pontszám: ";
			OpenFileDialog allomanyBetolt = new OpenFileDialog();
			allomanyBetolt.Filter = "txt állományok(*txt)|*.txt|Összes állomány(*.*)|*.*"; //txt-t lehet menteni
			if (allomanyBetolt.ShowDialog() == true)
			{
				AllomanyOlvas(allomanyBetolt.FileName);
			}
		}
	}
}
