using Scf.Domain;
using Scf.Domain.Services;
using Scf.Domain.TenantModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Helpers
{
    public class RandomContentGenerator
    {
        //const string positionNames = "CEO,Genel Müdür,İcra Kurulu Başkanı,İnsan Kaynakları Direktörü,Finans Direktörü,Satış ve Pazarlama Direktörü,Operasyonlar Direktörü,Teknoloji Direktörü,Proje Yöneticisi,Bölüm Yöneticisi,Departman Yöneticisi,Proje Koordinatörü,Operasyonlar Yöneticisi,Satış Temsilcisi,Müşteri Hizmetleri Temsilcisi,İnsan Kaynakları Asistanı,Muhasebe Uzmanı,Finans Analisti,Teknoloji Uzmanı,Bilgi İşlem Sorumlusu,Veri Analisti,Satınalma Uzmanı,Proje Mühendisi,Yazılım Geliştirici,Donanım Mühendisi,Tasarım Uzmanı,Pazarlama Uzmanı,İçerik Yazarı,Sosyal Medya Uzmanı,İletişim Uzmanı,Araştırma Uzmanı,Lojistik Uzmanı,Satınalma Asistanı,Halkla İlişkiler Uzmanı,İş Analisti,Risk Yöneticisi,Kalite Kontrol Uzmanı,Üretim Sorumlusu,Bakım ve Onarım Uzmanı,İş Geliştirme Uzmanı";
        const string fNames = "Ahmet,Mehmet,Mustafa,Ali,Yusuf,İbrahim,Hasan,Hüseyin,Ömer,Osman,Adem,Murat,İsmail,Serkan,İsa,İlker,Emre,Cem,Selim,Tahir,Yusuf Ziya,Caner,Arda,Halil,Kemal,Yakup,Muhammed,Necati,Sinan,Faruk,Bekir,Rıdvan,Tuncay,Orhan,İlhan,İlyas,Engin,Ferhat,Sait,Erdal,Oktay,Adnan,Nihat,Serhat,Oğuz,Cengiz,Cemal,Celal,Şenol,Erhan,Mahmut,İsmet,Tarık,Alper,İhsan,Salih,Yasin,Hasan Hüseyin,Volkan,Şevket,Serdar,Mert,Serkan Ali,Muharrem,Zeki,Serhat Mustafa,Yusuf Emre,Bahadır,Durmuş,Levent,İhsan Gürdal,Bülent,Can,Yavuz,İrfan,Sezgin,Tolga,Ümit,Abdullah,Süleyman,Canan,Kürşat,Kemal Ahmet,Erkan,Haluk,İlker Can,Ramazan,Halil İbrahim,Hakan,Kadir,Suat,Yüksel,Özgür,Samet,Serkan İbrahim,Ümitkhan,Ayhan,Yaşar,Bayram,Zeynel,Onur,Kerem,Barış,Burak,Deniz,Ege,Enes,Erdem,Kenan,Nadir,Nuri,Sadık,Tayfun,Ufuk,Utku,Yahya,Yalçın,Yılmaz,Zafer,Atakan,Aykut,Batuhan,Berk,Doğan,Doruk,Emrah,Ersin,Fatih,Gökhan,Güney,Hayati,Kağan,Kıvanç,Mete,Murathan,Necmettin,Okan,Onat,Özkan,Özlem,Sabri,Savaş,Sefa,Şahin,Şamil,Şükrü,Tevfik,Turgay,Uğur,Vedat,Yıldırım,Yücel,Fatma,Ayşe,Hatice,Zeynep,Emine,Hacer,Serap,Aysel,Şerife,Sevgi,Asiye,Şükran,Fadime,Gülcan,Gülten,Merve,Aylin,Gülnur,Gülsüm,Güler,Necla,Arzu,Nuran,Asiye Nur,Ayten,Sevim,Sultan,Yıldız,Figen,Fidan,Cemile,Cennet,Nevin,Zehra,Ebru,Elif,Leyla,Gülsün,Hafize,Melek,Mehtap,Havva,Serpil,Sibel,Süheyla,Süreyya,Şebnem,Ummahan,Yasemin,Zekiye,Zekiye Betül,Asiye Fatma,Ayşegül,Belgin,Berfin,Betül,Birgül,Buket,Demet,Derya,Dilek,Ecem,Ecevit,Eda,Elvan,Esin,Eylem,Ferda,Feride,Feryal,Gülşen,Gülsen,Gülsünay,Güneş,Güzin,Hacer Ayşe,Handan,Hanım,Hayat,Hikmet,İclal,İkbal,İlknur,İpek,Kader,Kadriye,Kamile,Kanber,Kevser,Kibar,Kudret,Lale,Leyla Gül,Lütfiye,Melek Nur,Sümeyye,Esra,Dilara,İrem,Nil,Sena,Rümeysa,Özge,Tuğçe,Bahar,Ceren,Nazlı,Büşra,Aslı,Gamze,Asya,Nehir,Ezgi,Melisa,Pınar,Gülçin,Sezen,Bengü,Zerrin,Meltem,Pelin,Ceyda,Gizem,Rana,Yağmur,Gözde,Şeyma,Seda,Beste,Gülce,İlke,Ayça,İlayda,Buse,Cansu,Ece,Şule,Selma,Emel,Meryem,İnci,Nuray,Füsun,Mine,Şermin,Hande,Zeliha,Hazal,İrem Nur,Sare,Ayça Nur,Melis,Sude,Şenay,Yaren,Şima,Eylül,İklim,Nermin,Aydan,Gülden,Özden,Sevgül,Elvin,Aybüke,Eymen,Umut,Beren,Defne,Serhan,Sinem,Nisa,Berkan,İdil,Beril,Öykü,Melike,Sıla,Fırat,Ozan,Nihan,Yunus,Burcu,Baran,Asena,Irmak,Furkan,Kübra,Damla,Gökçe,Ela,Ayşenur,Beyza,Oğuzhan,Ulaş,Begüm,Ekin,Efe,Fulya,Bora,Alara,Aysun,Taylan,Kaan,Hira,Şevval,Kerim,İzel,Hilal,Meral,Mehmet Ali,Tuba,Cemre,Ayşen,Şerif,Nurdan,Erol,Ömer Faruk,Sıtkı,Bilge,Sevda,Melih,Filiz,Cemil,Hayal,Yiğit,Şahika,Aytuğ,Selçuk,Ahsen,Tamer,Ediz,Gülbahar,Tuğba,Selin,Fikret,Nihal,Sevinç,Metin,Neşe,Sezai,Egehan,Nur,Didem,Bahattin,Melodi,Akin,Elia,Asude,Ateş,Arif,Rüveyda,Altan,Burakhan,Bedia,Bengi,Reyhan,Dilhan,Sertaç,Oktar,Ayşin,Mürsel,Ferdi,Baki,Serdal,Aslıhan,Safiye,Tayyip,Saffet,Ceyhun,Rengin,İkram,Mehlika,Ercüment,Güngör,Eralp,Erkam,Seren,İlkin,Saniye,Işın,Aydoğan,Tuğrul,Gülistan,Fikriye,Ahmet Ali,Alihan,Cihangir,Tülin,Nuh,İbrahim Can,Bünyamin,Aykutcan,Akın,Numan,Oktayhan,Fazlı,Hasan Basri,Emin,Hayrettin,Güliz,Dilşad,Tansu,Sükrü,Abdulkadir,Zeynep Gizem,Selen,Semih,Kuzey,Duygu";
        const string sNames = "Yılmaz,Kaya,Demir,Şahin,Çelik,Öztürk,Doğan,Erdoğan,Aksoy,Aydın,Gündoğdu,Ay,Özdemir,Karadağ,Çetin,Avcı,Aydemir,Taş,Özkan,Kaplan,Sarı,Yalçın,Kılıç,Kara,Korkmaz,Özkaya,Aktaş,Uçar,Koca,Güneş,Akdeniz,Şimşek,Alkan,Tekin,Başaran,Akyol,Aygün,Coşkun,Aksu,Yıldız,Bayram,Yıldırım,Eren,Şen,Arslan,Yıldıran,Güzel,Tuncer,Türk,Ayhan,Şeker,Aytaç,Şahbaz,Dönmez,Akın,Aydoğan,Güçlü,Gökçe,Tekgül,Aras,Güler,Şanlı,Bayraktar,Özcan,Yıldızhan,Gürsoy,Taşkın,Yalın,Şenol,Çakır,Korkut,Akbulut,Atasoy,Altın,Işık,Akçay,Şekerler,Özmen,Çam,Yüksel,Şahinler,Bilgin,Özgür,Yavaş,Canpolat,Beşok,Dursun,Kılıçarslan,Gümüş,Tüfek,Şahinöz,Çiçekçi,Ekici,Akaslan,Akbulutlu,Akdolunay,Akkış,Alabas,Alkanoğlu,Anılken,Arıkanoğlu,Arıman,Armanoz,Ekinci,İnan,Türkmen,Can,Ateş,Gür,Karakaş,Köse,Ertaş,İpek,Ünal,Yavuz,Doğru,Ceylan,Uysal,Demirci,Karakaşlı,Kıran,Kurtuluş,Durmuş,Temel,Akbaş,Erden,Türkoğlu,İlhan,Uğur,Kalkan,Koçak,Cengiz,Öz,Köksal,Balkan,Akar,Ceyhan,Cemal,Şahingöz,Erşen,Özsoy,Kayaalp,Kılınç,Özdem,Dağ,Kaygusuz,Sönmez,Yurdakul,Köksüz,Kayabaşı,Uğurlu,Demirel,Karayel,Akdoğan,Uyar,Aslan,Kayar,Kılınçarslan,Orhan,Savaş,Gürkan,Gürbüz,Akça,Demirkan,Akgün,Topçu,Sözen,Gül,Özçelik,Kocaman,Başar,Turan,Özkanlı,Gültekin,Uzun,Akyüz,Özbek,Bozkurt,Karaman,Karabulut,Boz,Kurt,Şenöz,Karahan,Gündüz,Tan,Karakaya,Akkaya,Şentürk,Ercan,Yılmazer,Karagöz,Bayrak,İşcan,Göçer,Şensoy,Arıkan,Yücel,Şaşmaz,Çağlar,Bakır,Kayacan,Güngör,Özçetin,Arısoy,Türker,Durak,Güvenç,Kaymaz,Balcı,Karataş,Özen,Çetinkaya,Akman,Özgül,Yılmazsoy,Tanrıkulu,Şimşir,Özkanat,Ersoy,Özokur,Sırmacı,Yücebaş,Gürdöşmez,Tunaboylu,Şahinci,Karamancı,Sevük,Bozhan,Aydoğmuş,Güngörmüş,Cengizhan,İzci,Şahinbaş,Gökkurt,Şengöz,Ünsalver,Kılıçbay,Kaynarca,Bulatçı,Özkalp,Aydoğar,Bilgir,Kılıçkaya,Kırgız,Bozkurtlar,Yıldırımkaya,Kıvançer,Güneren,Alpaslaner,Gürlevik,Karaorman,Oğuzlu,Öztürkmen,Büyükalp,Koçman,Sayılkan,Ataşkan,Akpolat,Kılıçgün,Kılıçkıran,Asarkaya,İncebay,Şenkul,Taşçıoğlu,Güvenen,Türkyılmaz,Kılıçdaroğlu,Çelikkanat,Karaahmet,Korkmazgil,Karayaka,Üçler,Başdoğmuş,Ulusoylu,Pekgöz,Çeliksoy,Çiçeklioğlu,Doğangüzel,Baysalın,Tunaer,Karakoçak,Yorulmazlar,Kılıçkesmez,Sezginsoy,Özdaş,Yüceokur,Yıldızak,Kaymazlar,Gürsesli,Sarıkayaoglu,Kocasakal,Bayramoğulları,Kırkpınar,Dumaner,Ertegün,Yurdakulgil,Aydoğarlı,Ürküş,Kalkaner,Kılıçdaroğan,Kayıtken,Akyüzler,Türkaner,Bozdoğmuş,Şentürkmen,Yükselkan,Karatağ,Askerer,Tunaçar,Uysaler,Öztürkkan,Güngörmez,Akatay,Kıvançlı,Kıvançkan,Aladağ,Alagöz,Alataş,Alayunt,Alçıtepe,Alıcı,Altay,Arslantaş,Aykaç,Aytekin,Azaklı,Bağcı,Bahadır,Balaban,Başak,Bayar,Bektaş,Bilgiç,Boğa,Boğan,Bozdağ,Budak,Büyük,Çalışkan,Coşar,Dağlı,Deniz,Dinç,Duman,Duygulu,Eker,Elçi,Emre,Engin,Erkaya,Erkoç,Ertekin,Eryılmaz,Esen,Eser,Eskin,Evcil,Evren,Eyüboğlu,Fırat,Genç,Gerçek,Gezgin,Gök,Gökmen,Göktaş,Göktürk,Gönül,Hacıoğlu,Halıcı,Hancer,Hayta,Hazar,Işıklı,İlker,İnce,İyigün,Ağaoğlu,Akan,Ayyıldız,Aysel,Aytuğ,Bayık,Bingöl,Canbaz,Duran,Efe,Ege,Er,Erdoğmuş,Ergin,Erkan,Erol,Ertürk,Gülsüm,Güner,Hakan,İşler,Kabak,Kalaycı,Karaca,Karadeniz,Karavelioğlu,Kıvanç,Kocabıyık,Koç,Köseoğlu,Koyuncu,Kumcu,Kunter,Kutlu,Kuzucu,Levent,Memişoğlu,Meral,Mert,Metin,Nalbantoğlu,Öner,Örs,Özaydın,Bıyık,Çiftçi,Keskin,Tuncel,Karakuş,Keleş,Keser,Konuk,Şirin,Şişman,Tanrıverdi,Timur,Toprak,Yaşar,Yetkin,Arslantürk,Çiçek,Dilek,Elibol,Güldoğan,Gündoğan,Karamanoğlu,Karaoğlu,Kılıçoğlu,Kızıl,Şimşekçi,Günalp,Özkara,Aydınlık,Karagözlü,Şimşekli,Doğangün,Güneşli,Karahanlı,Özkoçak,Taşkentli,Aydınlar,Gülümser,Özmenekşe,Soysal,Aktaşoğlu,Düzgünoğlu,Güzelkan,Karamahmutoğlu,Özokutan,Taşkınlar,Aygönenç,Güzeltekin,Karakaşoğlu,Özpınar,Soysaldı,Aköz,Ecer,Karamanlı,Öztuna,Taşkınlı,Aymaz,Güzeltepe,Karakavak,Soysalp,Aktaşer,Ekinler,Öztürkoğlu,Taşlı,Aytaşçı,Gürdal,Karakılçık,Öztürksoy,Aktaşkan,Erdoğangüneş,Karaköprü,Özülkü,Taşlıova,Ayva,Gürgan,Karalıoğlu,Özyiğit,Şahinsoy,Akyıldız,Erdoğanlar,Karapınar,Özyürek,Tataroğlu,Gürlek,Karataşlı,Özyurtlu,Şahintepe,Akyağcı,Erdoğansoy,Karataşoğlu,Özzengin,Tatlı,Ayyıldızlı,Güzelhan,Karatekin,Özbekler,Şahinol,Alkanat,Erişen,Karatepe,Özçivit,Tavşanoğlu,Azrak,Güzelsu,Karatoprak,Özçınar,Şahinöztürk,Arslanhan,Eryiğit,Karatürk,Özdemiral,Temelkuran,Azra,Güzelyurt";

        List<position> positionDefs = new List<position>() {
            new("CEO", "Üst Yönetim"),
            new("Genel Müdür", "Üst Yönetim"),
            new("İcra Kurlu Başkanı", "Üst Yönetim"),
            new("İnsan Kaynakları Direktörü", "İnsan Kaynakları")
            {
                Subs = new List<position>(){
                    new("İnsan Kaynakları Asistanı", 3)
                }
            },
            new("Finans Direktörü", "Finans")
            {
                Subs = new List<position>()
                {
                    new("Muhasebe Uzmanı", 2)
                    {
                        Subs = new List<position>()
                        {
                            new("Muhasebe Sorumlusu", 2)
                        }
                    },
                    new("Finans Analisti", 4),
                }
            },
            new("Satış ve Pazarlama Direktörü", "Satış ve Pazarlama", 20)
            {
                Subs = new List<position>()
                {
                    new("Satış Bölge Yöneticisi", 8)
                    {
                        Subs = new List<position>()
                        {
                            new("Satış Temsilcisi", 20)
                        }
                    },
                    new("Sanal Pazarlar Yöneticisi", 1)
                    {
                        Subs = new List<position>()
                        {
                            new("Sanal Pazar Uzmanı", 4)
                            {
                                Subs = new List<position>()
                                {
                                    new("Sanal Pazar Operasyon Sorumlusu", 12)
                                }
                            }
                        }
                    },
                    new("Pazarlama Uzmanı", 3),
                    new("İçerik Yazarı", 1),
                    new("Sosyal Medya Uzmanı", 2),
                    new("İletişim Uzmanı", 2),
                    new("Araştırma Uzmanı", 1),

                },
            },
            new("Teknoloji Direktörü", "Teknoloji")
            {
                Subs = new List<position>(){
                    new("Bilgi İşlem Sorumlusu", 1)
                    {
                        Subs = new List<position>()
                        {
                            new("Bilgi İşlem Uzmanı", 6)
                        }
                    },
                    new("Veri Analisti", 2),
                    new("Veritabanı Uzmanı", 2)
                }
            },
            new("Satınalma Direktörü", "Satınalma")
            {
                Subs = new List<position>()
                {
                    new("Satınalma Uzmanı", 2)
                    {
                        Subs = new List<position>()
                        {
                            new("Satınalma Asistanı", 2)
                        }
                    }
                }
            },
            new("Ürün Sahibi", "Üretim")
            {
                Subs = new List<position>()
                {
                    new("Yazılım Proje Yöneticisi", 4)
                    {
                        Subs = new List<position>()
                        {
                            new("Yazılım Geliştirici", 6),
                            new("Yazılım Test Uzmanı", 2),
                            new("DevOps", 1),
                            new("Yazılım Analisti", 1)
                        }
                    }
                }
            }
        };

        private Dictionary<string, Group> groups = new Dictionary<string, Group>();
        private Dictionary<string, Position> positions = new Dictionary<string, Position>();

        private DomainContext context;
        private Tenant tenant = null!;

        private string[] firstnames;
        private string[] lastnames;

        public RandomContentGenerator(DomainContext domainContext) {
            this.context = domainContext;

            this.firstnames = fNames.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
            this.lastnames = sNames.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();


        }

        public async Task<Tenant> Initialize(string tenantCode)
        {
            await context.InitializeDatabase();

            this.tenant = context.Tenants.Create();
            tenant.Code = tenantCode;
            tenant.Title = tenantCode + " Hesabı";

            foreach (var p in positionDefs)
            {
                await CreatePosition(p);
            }

            await context.SaveChanges();

            return tenant;
        }

        private string GenerateEmailAddress(string fullName)
        {
            fullName = fullName
                .ToLower(CultureInfo.GetCultureInfo("en-US"))
                .Replace(" ", "")
                .Replace("ç", "c")
                .Replace("ğ", "g")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ş", "s")
                .Replace("ü", "u");

            return $"{fullName}.{Random.Shared.Next(999)}@{tenant.Code}.example.com";
        }

        protected async Task<Employee> CreateRandomEmployee()
        {
            string firstName = firstnames[Random.Shared.Next(firstnames.Length)];
            string lastName = lastnames[Random.Shared.Next(lastnames.Length)];
            string email = GenerateEmailAddress(firstName + lastName);

            var u = await context.Users.FindByEmailAsync(email);

            if(u == null)
            { 
                u = context.Users.Create();
                u.FirstName = firstName;
                u.LastName = lastName;
                u.Language = tenant.Settings.DefaultLanguage;
                u.Email = email;
                u.IsActive = true;
            }

            u.AddToTenant(tenant);

            var e = context.Employees.Create(tenant);
            e.FirstName = u.FirstName;
            e.LastName = u.LastName;
            e.Email = u.Email;
            e.ExternalId = Guid.NewGuid().ToString().Split('-')[4].ToUpper();
            e.User = u;

            return e;
        }

        private async Task CreatePosition(position p, Employee? manager = null)
        {
            if(!groups.TryGetValue(p.GroupName, out Group? group))
            {
                group = context.Groups.Create(tenant);
                group.Names = new MultilanguageText(p.GroupName, p.GroupName);
            }

            groups[p.GroupName] = group;


            if(!positions.TryGetValue(p.Name, out Position? position))
            {
                position = context.Positions.Create(tenant);
                position.Names = new MultilanguageText(p.Name, p.Name);
            }

            positions[p.Name] = position;
            
            for(int k = 0; k < p.PersonCount; k++)
            {
                var e = await CreateRandomEmployee();
                e.Groups = new Group[] { group };
                e.Position = position;
                e.Manager = manager;

                if (group.Manager == null)
                    group.Manager = e;

                foreach (var s in p.Subs)
                {
                    await CreatePosition(s, e);
                }
            }


        }

        protected class position
        {
            private IEnumerable<position>? _subs = null;
            private string _group = null!;

            public position(string name, string department) {
                this.Name = name;
                this.GroupName = department;
            }

            public position(string name, string department, int personCount)
            {
                this.Name = name;
                this.GroupName = department;
                this.PersonCount = personCount;
            }

            public position(string name, int personCount)
            {
                this.Name = name;
                this.PersonCount = personCount;
            }

            public string Name { get; set; }
            public string GroupName
            {
                get
                {
                    return _group;
                }
                set
                {
                    _group = value;
                    InitializeSubs();
                }
            }

            public int PersonCount { get; set; } = 1;

            public IEnumerable<position> Subs
            {
                get
                { return _subs ?? Array.Empty<position>(); }

                set
                {
                    _subs = value;
                    InitializeSubs();
                }
            }

            private void InitializeSubs()
            {
                if (_subs != null)
                {
                    foreach (var s in _subs)
                    {
                        s.GroupName = this.GroupName;
                    }
                }
            }

        }
    }
}
