using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eurovizio
{
    public class Versenyzo //1 versenyző tárolása
    {
        public short Ev { get; set; }
        public string Orszag { get; set; }
        public string Eloado { get; set; }
        public string Cim { get; set; }
        public byte Helyezes { get; set; }
        public short Pontszam { get; set; }
        public static double Atlag { get; set; }
        public Versenyzo(string sor) //Konstruktor
        {
            string[] resz = sor.Split(';'); //Szétbontás
            Ev = Convert.ToInt16(resz[0]);
            Orszag = resz[1];
            Eloado = resz[2];
            Cim = resz[3];
            Helyezes = Convert.ToByte(resz[4]);
            Pontszam = Convert.ToInt16(resz[5]);
        }

        public Versenyzo(string _ev,string _orszag, string _eloado, string _cim, string _helyezes, string _pontszam)
        {
            Ev = Convert.ToInt16(_ev);
            Orszag = _orszag;
            Eloado = _eloado;
            Cim = _cim;
            Helyezes = Convert.ToByte(_helyezes);
            Pontszam = Convert.ToInt16(_pontszam);
        }
    }
}
