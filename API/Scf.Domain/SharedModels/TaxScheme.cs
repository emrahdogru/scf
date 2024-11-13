using MongoDB.Bson.Serialization.Attributes;
using Scf.Domain.Interfaces;
using Scf.Domain.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.SharedModels
{
    public class TaxScheme : ICard
    {
        private static IEnumerable<TaxScheme>? _list = null;
        private readonly static string[] otvCodes = new string[] { "0071", "0073", "0074", "0075", "0076", "0077" };

        protected TaxScheme(string code, string name, string shortName, Func<TaxList, double> taxBaseAmount)
        {
            Code = code.Trim();
            Name = name.Trim();
            ShortName = shortName.Trim();
            TaxBaseAmount = taxBaseAmount;
        }

        protected TaxScheme(string code, string name, string shortName)
        {
            Code = code.Trim();
            Name = name.Trim();
            ShortName = shortName.Trim();
            TaxBaseAmount = list => list.BaseAmount;
        }

        public string Code { get; protected set; } = null!;

        public string Name { get; protected set; } = null!;

        public string ShortName { get; protected set; } = null!;

        public Func<TaxList, double> TaxBaseAmount { get; protected set; }

        public static TaxScheme FromCode(string code)
        {
            return GetAll().FirstOrDefault(x => x.Code == code) ?? throw new EntityNotFountException(nameof(TaxScheme), code);
        }

        public static IEnumerable<TaxScheme> GetAll()
        {
            if(_list == null)
            {
                _list = new TaxScheme[]
                {
                    new TaxScheme("0015", "GERÇEK USULDE KATMA DEĞER VERGİSİ", "KDV GERCEK", list => {
                        var baseAmount = list.BaseAmount;
                        var totalOtv = list.Where(x => otvCodes.Contains(x.TaxScheme.Code)).Sum(x => x.TaxAmount);

                        return baseAmount + totalOtv;
                    }),

                    new TaxScheme("0059", "KONAKLAMA VERGİSİ", "KONAKLAMA VERGİSİ", list => list.BaseAmount),

                    new TaxScheme("0071", "PETROL VE DOĞALGAZ ÜRÜNLERİNE İLİŞKİN ÖZEL TÜKETİM VERGİSİ", "ÖTV 1.LİSTE"),
                    new TaxScheme("0073", "KOLALI GAZOZ, ALKOLLÜ İÇEÇEKLER VE TÜTÜN MAMÜLLERİNE İLİŞKİN ÖZEL TÜKETİM VERGİSİ", "ÖTV 3.LİSTE"),
                    new TaxScheme("0074", "DAYANIKLI TÜKETİM VE DİĞER MALLARA İLİŞKİN ÖZEL TÜKETİM VERGİSİ", "ÖTV 4.LİSTE"),
                    new TaxScheme("0075", "ALKOLLÜ İÇEÇEKLERE İLİŞKİN ÖZEL TÜKETİM VERGİSİ", "ÖTV 3A LİSTE"),
                    new TaxScheme("0076", "TÜTÜN MAMÜLLERİNE İLİŞKİN ÖZEL TÜKETİM VERGİSİ", "ÖTV 3B LİSTE"),
                    new TaxScheme("0077", "KOLALI GAZOZLARA İLİŞKİN ÖZEL TÜKETİM VERGİSİ", "ÖTV 3C LİSTE"),
                    new TaxScheme("4080", "ÖZEL İLETİŞİM VERGİSİ", "Ö.İLETİŞİM V"),
                    new TaxScheme("4081", "5035 SAYILI KANUNA GÖRE ÖZEL İLETİŞİM VERGİSİ", "5035ÖZİLETV")
                };
            }

            return _list;
            //            if (_list == null)
            //            {
            //                _list = @"0003;GELİR VERGİSİ STOPAJI;GV STOPAJI
            //0011;KURUMLAR VERGİSİ STOPAJI;KV STOPAJI
            //0015;GERÇEK USULDE KATMA DEĞER VERGİSİ;KDV GERCEK
            //0021;BANKA MUAMELELERİ VERGİSİ;BMV
            //0022;SİGORTA MUAMELELERİ VERGİSİ;SMV
            //0059;KONAKLAMA VERGİSİ;KONAKLAMA VERGİSİ
            //0061;KAYNAK KULLANIMI DESTEKLEME FONU KESİNTİSİ;KKDF KESİNTİ
            //0071;PETROL VE DOĞALGAZ ÜRÜNLERİNE İLİŞKİN ÖZEL TÜKETİM VERGİSİ;ÖTV 1.LİSTE
            //0073;KOLALI GAZOZ, ALKOLLÜ İÇEÇEKLER VE TÜTÜN MAMÜLLERİNE İLİŞKİN ÖZEL TÜKETİM VERGİSİ;ÖTV 3.LİSTE
            //0074;DAYANIKLI TÜKETİM VE DİĞER MALLARA İLİŞKİN ÖZEL TÜKETİM VERGİSİ;ÖTV 4.LİSTE
            //0075;ALKOLLÜ İÇEÇEKLERE İLİŞKİN ÖZEL TÜKETİM VERGİSİ;ÖTV 3A LİSTE
            //0076;TÜTÜN MAMÜLLERİNE İLİŞKİN ÖZEL TÜKETİM VERGİSİ;ÖTV 3B LİSTE
            //0077;KOLALI GAZOZLARA İLİŞKİN ÖZEL TÜKETİM VERGİSİ;ÖTV 3C LİSTE
            //1047;DAMGA VERGİSİ;DAMGA V
            //1048;5035 SAYILI KANUNA GÖRE DAMGA VERGİSİ;5035SKDAMGAV
            //4071;ELEKTRİK VE HAVAGAZI TÜKETİM VERGİSİ;ELK.HAVAGAZ.TÜK.VER.
            //4080;ÖZEL İLETİŞİM VERGİSİ;Ö.İLETİŞİM V
            //4081;5035 SAYILI KANUNA GÖRE ÖZEL İLETİŞİM VERGİSİ;5035ÖZİLETV.
            //4171;PETROL VE DOĞALGAZ ÜRÜNLERİNE İLİŞKİN ÖTV TEVKİFATI;PTR-DGZ ÖTV TEVKİFAT
            //8001;BORSA TESCİL ÜCRETİ;BORSA TES.ÜC.
            //8002;ENERJİ FONU;ENERJİ FONU
            //8004;TRT PAYI;TRT PAYI
            //8005;ELEKTRİK TÜKETİM VERGİSİ;ELK.TÜK.VER.
            //8006;TELSİZ KULLANIM ÜCRETİ;TK KULLANIM
            //8007;TELSİZ RUHSAT ÜCRETİ;TK RUHSAT
            //8008;ÇEVRE TEMİZLİK VERGİSİ;ÇEV. TEM .VER.
            //9021;4961 BANKA SİGORTA MUAMELELERİ VERGİSİ;4961BANKASMV
            //9040;MERA FONU;MERA FONU
            //9077;MOTORLU TAŞIT ARAÇLARINA İLİŞKİN ÖZEL TÜKETİM VERGİSİ (TESCİLE TABİ OLANLAR);ÖTV 2.LİSTE
            //9944;BELEDİYELERE ÖDENEN HAL RÜSUMU;BEL.ÖD.HAL RÜSUM"
            //                .Split('\r', '\n')
            //                .Where(x => !string.IsNullOrWhiteSpace(x))
            //                .Select(x => x.Split(';'))
            //                .Where(x => x.Length == 3)
            //                .Select(x => new TaxScheme(x[0], x[1], x[2]))
            //                .ToArray();
            //}


        }
    }
}
