using Scf.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.SharedModels
{
    public class Currency : ICard
    {
        private static IEnumerable<Currency>? _list = null;

        private Currency(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public static Currency FromCode(string code)
        {
            return GetAll().FirstOrDefault(x => x.Code == code?.ToUpperInvariant()) ?? throw new EntityNotFountException(nameof(Currency), code);
        }

        public static IEnumerable<Currency> GetAll()
        {
            if(_list == null)
            {
                _list = @"AED;784;2;Birleşik Arap Emirlikleri dirhemi; Birleşik Arap Emirlikleri
AFN;971;2;Afgani; Afganistan
ALL;008;2;Arnavut leki; Arnavutluk
AMD;051;2;Ermeni dramı; Ermenistan
ANG;532;2;Antiller Guldeni; Hollanda Antilleri
AOA;973;2;Angola kwanzası; Angola
ARS;032;2;Arjantin pesosu; Arjantin
AUD;036;2;Avustralya doları; Avustralya,  Christmas Adası,  Cocos Adaları, Avustralya Heard Adası ve McDonald Adaları,  Kiribati,  Nauru,  Norfolk Adası,  Tuvalu
AWG;533;2;Aruba florini; Aruba
AZN;944;2;Azerbaycan manatı; Azerbaycan
BAM;977;2;Bosna-Hersek değiştirilebilir markı; Bosna-Hersek
BBD;052;2;Barbados doları; Barbados
BDT;050;2;Bangladeş takası; Bangladeş
BGN;975;2;Bulgar levası; Bulgaristan
BHD;048;3;Bahreyn dinarı; Bahreyn
BIF;108;0;Burundi frangı; Burundi
BMD;060;2;Bermuda doları  Bermuda
BND;096;2;Brunei doları; Brunei
BOB;068;2;Bolivya bolivyanosu; Bolivya
BOV;984;2;Bolivya Mvdolu (Para kodu)
BRL;986;2;Brezilya reali; Brezilya
BSD;044;2;Bahama doları; Bahamalar
BTN;064;2;Ngultrum; Bhutan
BWP;072;2;Botsvana pulası; Botsvana
BYR;974;0;Belarus rublesi; Belarus
BZD;084;2;Belize doları; Belize
CAD;124;2;Kanada doları; Kanada
CDF;976;2;Kongo frangı; Kongo DC
CHE;947;2;WIR Euro (Tamamlayıcı para birimi);  İsviçre
CHF;756;2;İsviçre frangı
CHW;948;2;WIR Franc (Tamamlayıcı para birimi)
CLF;990;0;Unidades de formento (Para kodu); Şili
CLP;152;0;Şili Pesosu
CNY;156;2;Yuan Renminbisi; Çin
COP;170;2;Kolombiya pesosu; Kolombiya
COU;970;2;Unidad de Valor Real
CRC;188;2;Kosta Rika colonu; Kosta Rika
CUP;192;2;Küba pesosu; Küba
CVE;132;2;Yeşil Burun Adaları escudosu; Yeşil Burun Adaları
CYP;196;2;Kıbrıs lirası; Kıbrıs Cumhuriyeti
CZK;203;2;Çek korunası; Çek Cumhuriyeti
DJF;262;0;Cibuti frangı; Cibuti
DKK;208;2;Danimarka kronu; Danimarka,  Faroe Adaları,  Grönland
DOP;214;2;Dominik pesosu; Dominik Cumhuriyeti
DZD;012;2;Cezayir dinarı; Cezayir
EEK;233;2;Estonya kronu; Estonya
EGP;818;2;Mısır lirası/ Cuneyh; Mısır
ERN;232;2;Eritre nakfası; Eritre
ETB;230;2;Etiyopya birri; Etiyopya
EUR;978;2;Euro; Avrupa Birliği, bkz. Euro bölgesi
FJD;242;2;Fiji doları; Fiji
FKP;238;2;Falkland Adaları lirası; Falkland Adaları
GBP;826;2;İngiliz sterlini; Birleşik Krallık
GEL;981;2;Gürcistan larisi; Gürcistan
GHC;288;2;Gana cedisi; Gana
GIP;292;2;Cebelitarık lirası; Cebelitarık
GMD;270;2;Gambiya dalasisi; Gambiya
GNF;324;0;Gine frangı; Gine
GTQ;320;2;Guatemala kuetzalı; Guatemala
GYD;328;2;Guyana doları; Guyana
HKD;344;2;Hong Kong doları; Hong Kong
HNL;340;2;Honduras lempirası; Honduras
HRK;191;2;Hırvatistan kunası; Hırvatistan
HTG;332;2;Haiti gourde; Haiti
HUF;348;2;Macar forinti; Macaristan
IDR;360;2;Endonezya rupiahı; Endonezya
ILS;376;2;Yeni İsrail şekeli; İsrail
INR;356;2;Hindistan rupisi; Hindistan
IQD;368;3;Irak dinarı; Irak
IRR;364;2;İran riyali; İran
ISK;352;2;İzlanda kronası; İzlanda
JMD;388;2;Jamaika doları; Jamaika
JOD;400;3;Ürdün dinarı; Ürdün
JPY;392;0;Japon yeni; Japonya
KES;404;2;Kenya şilini; Kenya
KGS;417;2;Kırgızistan somu; Kırgızistan
KHR;116;2;Kamboçya rieli; Kamboçya
KMF;174;0;Komoro frangı; Komorlar
KPW;408;2;Kuzey Kore wonu; Kuzey Kore
KRW;410;0;Güney Kore wonu; Güney Kore
KWD;414;3;Kuveyt dinarı; Kuveyt
KYD;136;2;Cayman Adaları doları; Cayman Adaları
KZT;398;2;Kazak tengesi; Kazakistan
LAK;418;2;Laos kipi; Laos
LBP;422;2;Lübnan lirası; Lübnan
LKR;144;2;Sri Lanka rupisi; Sri Lanka
LRD;430;2;Liberya doları; Liberya
LSL;426;2;Lesotho lotisi; Lesotho
LTL;440;2;Litvanya litası; Litvanya
LVL;428;2;Letonya latsı; Letonya
LYD;434;3;Libya dinarı; Libya
MAD;504;2;Fas dirhemi; Fas,  Batı Sahra
MDL;498;2;Moldova leyi; Moldova
MGA;969;0;Madagaskar ariarisi; Madagaskar
MKD;807;2;Makedon dinarı; Kuzey Makedonya
MMK;104;2;Burma kyatı; Burma
MNT;496;2;Moğol tögrögü; Moğolistan
MOP;446;2;Makao patakası; Makao
MRO;478;2;Moritanya ugiyası; Moritanya
MTL;470;2;Malta lirası; Malta
MUR;480;2;Mauritius rupisi; Mauritius
MVR;462;2;Maldiv rufiyaası; Maldivler
MWK;454;2;Malavi kvaçası; Malavi
MXN;484;2;Meksika pesosu; Meksika
MXV;979;2;Mexican Unidad de Inversion (UDI) (Para kodu)
MYR;458;2;Malezya ringgiti; Malezya
MZN;943;2;Mozambik metikali; Mozambik
NAD;516;2;Namibya doları; Namibya
NGN;566;2;Nijerya nairası; Nijerya
NIO;558;2;Nikaragua kordobası; Nikaragua
NOK;578;2;Norveç kronu; Norveç
NPR;524;2;Nepal rupisi;   Nepal
NZD;554;2;Yeni Zelanda doları; Cook Adaları,  Yeni Zelanda,  Niue,  Pitcairn Adaları,  Tokelau
OMR;512;3;Umman riyali; Umman
PAB;590;2;Panama balboası; Panama
PEN;604;2;Peru nuevo solü; Peru
PGK;598;2;Papua Yeni Gine kinası; Papua Yeni Gine
PHP;608;2;Filipinler pesosu; Filipinler
PKR;586;2;Pakistan rupisi; Pakistan
PLN;985;2;Polonya zlotisi; Polonya
PYG;600;0;Paraguay guaranisi; Paraguay
QAR;634;2;Katar riyali; Katar
ROL;642;2;Eski Romanya Leyi; Romanya
RON;946;2;Rumen leyi
RSD;941;2;Sırp dinarı; Sırbistan
RUB;643;2;Rus rublesi; Rusya, Abhazya Abhazya, Kuzey Osetya Kuzey Osetya
RWF;646;0;Ruanda frangı; Ruanda
SAR;682;2;Suudi Arabistan riyali; Suudi Arabistan
SBD;090;2;Solomon Adaları doları; Solomon Adaları
SCR;690;2;Seyşeller rupisi; Seyşeller
SDD;736;2;Sudan sterlini; Sudan
SEK;752;2;İsveç kronu; İsveç
SGD;702;2;Singapur doları; Singapur
SHP;654;2;Saint Helena liraso; Saint Helena
SIT;705;2;Slovenya toları; Slovenya
SKK;703;2;Slovak korunası; Slovakya
SLL;694;2;Sierra Leone leonesi; Sierra Leone
SOS;706;2;Somali şilini; Somali
SRD;968;2;Surinam doları; Surinam
STD;678;2;São Tomé ve Príncipe dobrası; São Tomé ve Príncipe
SYP;760;2;Suriye lirası; Suriye
SZL;748;2;Esvatini lilangenisi; Esvatini
THB;764;2;Tayland bahtı; Tayland
TJS;972;2;Tacikistan somonisi; Tacikistan
TMM;795;2;Türkmenistan manatı; Türkmenistan
TND;788;3;Tunus dinarı; Tunus
TOP;776;2;Tonga a'angası; Tonga
TRY;949;2;Türk lirası; Türkiye ve  Kuzey Kıbrıs
TTD;780;2;Trinidad ve Tobago doları; Trinidad ve Tobago
TWD;901;2;Yeni Tayvan doları; Tayvan ve  Çin kontrolü altındaki diğer adalar
TZS;834;2;Tanzanya şilini; Tanzanya
UAH;980;2;Ukrayna grivnası; Ukrayna
UGX;800;2;Uganda şilini; Uganda
USD;840;2;Amerikan doları; ABD,  Amerikan Samoası,  Britanya Hint Okyanusu Toprakları,  Ekvador,  El Salvador,  Guam,  Haiti,  Marshall Adaları,  Mikronezya Federal Devletleri,  Kuzey Mariana Adaları,  Palau,  Panama,  Doğu Timor,  Turks ve Caicos Adaları,  ABD Virjin Adaları
USN;997;2;; ABD
USS;998;2;
UYU;858;2;Uruguay pesosu; Uruguay
UZS;860;2;Özbekistan somu; Özbekistan
VEB;862;2;Venezuela bolivarı; Venezuela
VND;704;2;Vietnam dongu; Vietnam
VUV;548;0;Vanuatu vatusu; Vanuatu
WST;882;2;Samoa talası; Samoa
XAF;950;0;CFA Franc BEAC; Kamerun,  Orta Afrika Cumhuriyeti,  Kongo Cumhuriyeti,  Çad,  Ekvador,  Gabon
XAG;961;.;Gümüş (1 Troy ons) (=31.1034768 gram);
XAU;959;.;Altın (1 Troy ons) (=31.1034768 gram);
XBA;955;.;Avrupa Para Birimi (EURCO) (Bond Piyasası Birimi);
XBB;956;.;Avrupa Para Birimi (E. M. U.-6) (Bond Piyasası Birimi);
XBC;957;.;Avrupa Hesap Birimi 9 (E. U. A.-9) (Bond Piyasası Birimi);
XBD;958;.;Avrupa Hesap Birimi 17 (E. U. A.-17) (Bond Piyasası Birimi);
XCD;951;2;Doğu Karayip doları; Anguilla,  Antigua ve Barbuda,  Dominika,  Grenada,  Montserrat,  Saint Kitts ve Nevis,  Saint Lucia,  Saint Vincent ve Grenadinler
XDR;960;.;Özel çekme hakları;Uluslararası Para Fonu
XFO;Nil;.;Altın Frank (özel ödeme para birimi);Uluslararası Ödemeler Bankası
XFU;Nil;.;UIC Frank (özel ödeme para birimi);Uluslararası Demiryolları Birliği
XOF;952;0;CFA Franc BCEAO; Benin,  Burkina Faso,  Fildişi Sahili,  Gine-Bissau,  Mali,  Nijer,  Senegal,  Togo
XPD;964;.;Palladium (1 3 gram Truva);
XPF;953;0;CFP franc;Fransız Polinezyası Fransız Polinezyası,  Yeni Kaledonya,  Wallis ve Futuna
XPT;962;.;Platin (1 3 gram Truva)(= 31.1034768 gram);
XTS;963;.;test edilen para birimleri için kullanılır;
XXX;999;.;Böyle Bir Para Birimi Yok;
YER;886;2;Yemen riyali; Yemen
ZAR;710;2;Güney Afrika randı; Güney Afrika
ZMK;894;2;Zambiya kwachası; Zambiya
ZWD;716;2;Zimbabve doları; Zimbabve"
.Split('\r', '\n')
.Where(x => !string.IsNullOrWhiteSpace(x))
.Select(x => x.Split(';'))
.Where(x => x.Length >= 3)
.Select(x => new Currency(x[0], x[3]))
.ToArray();
            }

            return _list;
        }
    }
}
