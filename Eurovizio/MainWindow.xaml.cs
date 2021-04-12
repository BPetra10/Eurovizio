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

	public class Versenyzo //1 versenyző tárolása
	{ 
		public short Ev { get; set; }
		public string Orszag { get; set; }
		public string Eloado { get; set; }
		public string Cim { get; set; }
		public byte Helyezes { get; set; }
		public short Pontszam { get; set; }
		public static double Atlag { get; set; }

		public Versenyzo(string sor)
		{
			string[] resz = sor.Split(';');
			Ev = Convert.ToInt16(resz[0]);
			Orszag = resz[1];
			Eloado = resz[2];
			Cim = resz[3];
			Helyezes = Convert.ToByte(resz[4]);
			Pontszam = Convert.ToInt16(resz[5]);
		}

	}
	public partial class MainWindow : Window
	{
		List<Versenyzo> adatok = new List<Versenyzo>();
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (var item in File.ReadAllLines("eurovizio.txt"))
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
			List<double> atlagok = new List<double>();
			foreach (var item in adatok.OrderBy(x=>x.Orszag).GroupBy(x=>x.Orszag))
			{
				orszagok.Items.Add(item.Key);
				atlagok.Add(item.Average(x=>x.Pontszam));
			}
			double max = atlagok.Max();
			helyezesBar.Maximum = max;
			Versenyzo.Atlag = Math.Round(adatok.Average(x=>x.Pontszam),0);
			atlagKiir.Content = "Átlagos pontszám: " + Versenyzo.Atlag;
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
				helyezesBar.Foreground = new SolidColorBrush(Color.FromRgb(236, 28, 15));
			}
			else if (orszagatlag < Versenyzo.Atlag + 10)
			{
				orszagAtlagKiir.Background = new SolidColorBrush(Color.FromRgb(14, 209, 69));
				helyezesBar.Foreground = new SolidColorBrush(Color.FromRgb(14, 209, 69));
			}
			else
			{
				orszagAtlagKiir.Background = new SolidColorBrush(Color.FromRgb(255, 202, 24));
				helyezesBar.Foreground = new SolidColorBrush(Color.FromRgb(255, 202, 24));
			}
			orszagAtlagKiir.Content = orszag + " átlagos pontszáma: " + orszagatlag;
			helyezesBar.Value = orszagatlag;
		}
	}
}
