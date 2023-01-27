using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeUsersLite
{
    internal abstract class FakeUserDatasetRUS: FakeUsersDataset
    {
        protected static string NamesFMale = "игорегорвадимойшалексемёниколаропукитемишантолегригоша";
        protected static string NamesFFemale = "евалисангелиннадеждарьядуняшамашаленаолялякатянкотятка";
        protected static string NamesFThing = "кофефруктбананяблокомпьютерминаложкабельприставканатакнигавтомобильникитаецтелефон";
        protected static string NamesFAnimal = "бобикмухтаряшалмазюзябабайкантобоняшашаспайкибуффалопа";
        protected static string NamesL = "петривансидоркошкаптеродактикривкосдураквасякунокжокабалделаскосозуб";
        protected static string[] NamesLEndsMale = { "ко", "нко", "ов", "енков" };
        protected static string[] NamesLEndsFeMale = { "ко", "нко", "ова", "енкова" };
        protected static string[] NamesMEndsMale = { "вич" };
        protected static string[] NamesMEndsFeMale = { "вна" };
        protected static string NamesM = "игорегорвадимойшалексемёниколартемишантолегригошапетрярикольгоалександрстанислав";
    }

    internal abstract class FakeUserDatasetENG: FakeUsersDataset
    { 
    protected static string NamesFMale = "igoregorvadimojshaleksemyonikolaropukitemishantolegrigosha";
    protected static string NamesFFemale = "evalisangelinnadezhdaryadunyashamashalenaolyalyakatyankotyatka";
    protected static string NamesL = "petrivansidorkoshkapterodaktikrivkosdurakvasyakunokzhokabaldelaskosozub";
    protected static string NamesFThing = "kofefruktbananjablokompjuterminalozhkabelpristavkanataknigavtomobilnikitaectelefon";
    protected static string NamesFAnimal = "petrivansidorkoshkapterodaktikrivkosdurakvasjakunokzhokabaldelaskosozub";
    protected static string[] NamesLEndsMale = { "ko", "nko", "ov", "enkov" };
    protected static string[] NamesLEndsFeMale = { "ko", "nko", "ova", "enkova" };
    protected static string[] NamesMEndsMale = { "vich" };
    protected static string[] NamesMEndsFeMale = { "vna" };
    protected static string NamesM = "igoregorvadimojshaleksemyonikolartemishantolegrigoshapetryarikolgoaleksandrstanislav";
    }

    internal abstract class FakeUsersDataset
    {
        protected static string SymbolsForInternet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_____00001111222233334455667777889999";
        protected static string[] DomensInternet = {"com","ru","net"};
        protected static string[] DomensMail = { "mail", "gmail", "yandex" };
    }

}
