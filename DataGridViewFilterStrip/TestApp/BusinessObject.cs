using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp {
    class Data {
        static Random rand = new Random(1);
        static List<KeyValuePair<string, int>> cities = new Dictionary<string, int> {
            { "Reno",89501 },   {" Reno",   89501}, {"Ashland",03217}, {"Livingston",07039},
            { "Santa Fe",87500 }, {"New York",  10001 },{"Oxford",  27565}, {"Walhalla",58282},
            { "Cleveland",   44101}, {"Tulsa	",74101},   {"Portland",    97201}, {"Pittsburgh",  15201},
            { "Newport", 02840}, {"Camden",  29020}, {"Aberdeen", 57401}
        }.ToList();

        static public KeyValuePair<string,int> GetCity() {
            int no = rand.Next(cities.Count);
            return cities[no];
        }
        static public int? DayOfBirth() {
            int no = rand.Next(40);
            if (no == 0 || no > 30)
                return null;
            return no;
        }
    }

    public class Address {
        public bool IsMainAddress { get; set; }
        public int ZipCode { get; set; }
        public string City { get; set; }
        public Address(bool isMain) {
            IsMainAddress = isMain;
            KeyValuePair<string, int> kvp = Data.GetCity();
            ZipCode = kvp.Value;
            City = kvp.Key;
        }
        static public List<Address> Create(int count) {
            List<Address> result = new List<Address> { new Address(true) };
            while (result.Count < count) {
                Address adr = new Address(false);
                // only unique ZipCodes
                if (result.Where(a => a.ZipCode == adr.ZipCode).FirstOrDefault() == null){
                    result.Add(adr);
                }
            }
            return result;
        }
    }
    public class Person {
        public string Name { get; set; }
        public int BirthYear { get; set; }
        public int? DayOfBirth { get; set; }
        public bool IsWoman { get; set; }
        public Address HomeAddress {
            get {
                return Addresses.Where(a => a.IsMainAddress).FirstOrDefault();
            }
        }
        public List<Address> Addresses { get; set; } = new List<Address>();
        //public SortableBindingList<Address> SAddresses {
        //    get {
                
        //        return new SortableBindingList<Address>(Addresses);
        //    }
        //}

        public static List<Person> CreateData() {
            List<Person> lst = new List<Person>();
            lst.Add(new Person { Name = "John", BirthYear = 1978, IsWoman = false,
                 Addresses = new List<Address> {
                     new Address(true), new Address(false), new Address(false)
                 }
            });
            lst.Add(new Person {
                Name = "Jane", BirthYear = 1979, DayOfBirth = Data.DayOfBirth(), IsWoman = true,
                Addresses = new List<Address> {
                     new Address(true), new Address(false), new Address(false)
                 }
            });
            lst.Add(new Person {
                Name = "Mary", BirthYear = 1985, DayOfBirth = Data.DayOfBirth(), IsWoman = true,
                Addresses = new List<Address> {
                     new Address(true), new Address(false), new Address(false)
                 }
            });
            lst.Add(new Person {
                Name = "Bart", BirthYear = 1995, DayOfBirth = Data.DayOfBirth(), IsWoman = false,
                Addresses = new List<Address> {
                     new Address(true), new Address(false), new Address(false)
                 }
            });
            lst.Add(new Person {
                Name = "Lisa", BirthYear = 1999, DayOfBirth = Data.DayOfBirth(), IsWoman = true,
                Addresses = new List<Address> {
                     new Address(true), new Address(false), new Address(false)
                 }
            });
            lst.Add(new Person {
                Name = "Homer", BirthYear = 1963, DayOfBirth = Data.DayOfBirth(), IsWoman = false,
                Addresses = new List<Address> {
                     new Address(true), new Address(false), new Address(false)
                 }
            });
            return lst;
        }
    }


}
