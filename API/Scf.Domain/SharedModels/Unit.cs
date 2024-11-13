using Scf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.SharedModels
{
    public class Unit : IKart
    {
        private static IEnumerable<Unit>? _list = null;

        protected Unit(string code, string name)
        {
            Code = code.Trim();
            Name = name.Trim();
        }

        public string Code { get; }
        public string Name { get; }

        public static Unit? FromCode(string code)
        {
            return GetAll().FirstOrDefault(x => x.Code == code);
        }

        public static IEnumerable<Unit> GetAll()
        {
            if(_list == null)
                _list = @"C62;ADET(UNIT)
B32;KG-METRE KARE
CCT;TON BAŞINA TAŞIMA KAPASİTESİ
PR;ÇİFT
D30;BRÜT KALORİ DEĞERİ
D40;BİN LİTRE
GFI;FISSILE İZOTOP GRAMI
GRM;GRAM
GT;GROSS TON
CEN;YÜZ ADET
KPO;KİLOGRAM POTASYUM OKSİT
MND;KURUTULMUŞ NET AĞIRLIKLI KİLOGRAMI
3I;KİLOGRAM-ADET
KFO;DİFOSFOR PENTAOKSİT KİLOGRAMI
KGM;KİLOGRAM
KHY;HİDROJEN PEROKSİT KİLOGRAMI
KMA;METİL AMİNLERİN KİLOGRAMI
KNI;AZOTUN KİLOGRAMI
KPH;KİLOGRAM POTASYUM HİDROKSİT
KSD;%90 KURU ÜRÜN KİLOGRAMI
KSH;SODYUM HİDROKSİT KİLOGRAMI
KUR;URANYUM KİLOGRAMI
D32;TERAWATT SAAT
GWH;GİGAWATT SAAT
MWH;MEGAWATT SAAT (1000 kW.h)
KWH;KİLOWATT SAAT
KWT;KİLOWATT
LPA;SAF ALKOL LİTRESİ
LTR;LİTRE
MTK;METRE KARE
DMK;DESİMETRE KARE
MTQ;METRE KÜP
MTR;METRE
NCL;HÜCRE ADEDİ
CTM;KARAT
SM3;STANDART METREKÜP
R9;BİN METRE KÜP
SET;SET
T3;BİN ADET".Split('\r', '\n')
.Where(x => !string.IsNullOrWhiteSpace(x))
.Select(x => x.Split(';'))
.Where(x => x.Length == 2)
.Select(x => new Unit(x[0], x[1]))
.ToArray();

            return _list;
        }

    }
}
