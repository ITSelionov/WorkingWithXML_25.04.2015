using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace WorkingWithXML
{
    class Program
    {
        static int Menu(string[] arrayMenu)
        {
            int nSelect = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("-------M-e-n-u--------\n");

                for (int i = 0; i < arrayMenu.Length; i++)
                {
                    if (i == nSelect)
                    {
                        Console.Write("   ► ");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine(arrayMenu[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("     " + arrayMenu[i]);
                    }
                }

                Console.WriteLine("\n----E-n-d--M-e-n-u----");


                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.DownArrow:
                        {
                            if (nSelect >= arrayMenu.Length - 1) nSelect = 0;
                            else nSelect++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        {
                            if (nSelect == 0) nSelect = arrayMenu.Length - 1;
                            else nSelect--;
                        }
                        break;
                    case ConsoleKey.Enter:
                        return nSelect;
                }
            }
        }

        static void Show(XmlTextReader testXmlReadFile, List<decimal> listTest, List<decimal> listBasic, List<string> listCurrency)
        {
            listTest.Clear();
            listBasic.Clear();

            XmlTextReader xmlReadMainFile = new XmlTextReader(@"xmlWriteBasic.xml");

            while (xmlReadMainFile.Read())
                if (xmlReadMainFile.GetAttribute("currency") != null)
                    listBasic.Add(Convert.ToDecimal(xmlReadMainFile.GetAttribute("rate"), new CultureInfo("en-US")));

            while (testXmlReadFile.Read())
                if (testXmlReadFile.GetAttribute("currency") != null)
                    listTest.Add(Convert.ToDecimal(testXmlReadFile.GetAttribute("rate"), new CultureInfo("en-US")));
           
            Console.Clear();

            for (int i = 0; i < listBasic.Count; i++)
            {

                if (listBasic[i] > listTest[i])
                {
                    Console.Write(listCurrency[i] + " : ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(listBasic[i] + "▲");
                    Console.ResetColor();
                }
                else if (listBasic[i] < listTest[i])
                {
                    Console.Write(listCurrency[i] + " : ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(listBasic[i] + "▼");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(listCurrency[i] + " : " + listBasic[i]);
                }
            }

            xmlReadMainFile.Close();

            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            XmlTextReader xmlReadHttp = new XmlTextReader("http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");

            List<decimal> listTest = new List<decimal>();
            List<decimal> listBasic = new List<decimal>();
            List<string> listCurency = new List<string>();

            if (!(File.Exists(@"xmlWriteBasic.xml")))
            {
                XmlTextWriter xmlWriter = new XmlTextWriter(@"xmlWriteBasic.xml", Encoding.Unicode);

                xmlWriter.WriteStartElement("Cube");

                while (xmlReadHttp.Read())
                {
                    if (xmlReadHttp.GetAttribute("currency") != null)
                    {
                        listCurency.Add(xmlReadHttp.GetAttribute("currency"));
                        xmlWriter.WriteStartElement("Cube");
                        xmlWriter.WriteAttributeString("currency", xmlReadHttp.GetAttribute("currency"));
                        xmlWriter.WriteAttributeString("rate", xmlReadHttp.GetAttribute("rate"));
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
                xmlWriter.Close();
            }
            else
            {
                while (xmlReadHttp.Read())
                {
                    if (xmlReadHttp.GetAttribute("currency") != null)
                        listCurency.Add(xmlReadHttp.GetAttribute("currency"));
                }
            }

            xmlReadHttp.Close();

            string[] mainArrayMenu =
            {
                "Test Mode",
                "Basic Mode"
            };

            string[] testArrayMenu =
            {
                "TestFileFirst",
                "TestFileSecond",
                "TestFileThree"
            };

            while (true)
            {
                var nClick = Menu(mainArrayMenu);

                if (nClick == 0)
                {
                    nClick = Menu(testArrayMenu);

                    if (nClick == 0)
                    {
                        XmlTextReader testXmlReadFileFirst = new XmlTextReader(@"TestFileFirst.xml");
                        Show(testXmlReadFileFirst, listTest, listBasic, listCurency);
                        testXmlReadFileFirst.Close();
                    }

                    if (nClick == 1)
                    {
                        XmlTextReader testXmlReadFileSecond = new XmlTextReader(@"TestFileSecond.xml");
                        Show(testXmlReadFileSecond, listTest, listBasic, listCurency);
                        testXmlReadFileSecond.Close();
                    }

                    if (nClick == 2)
                    {
                        XmlTextReader testXmlReadFileThree = new XmlTextReader(@"TestFileThree.xml");
                        Show(testXmlReadFileThree, listTest, listBasic, listCurency);
                        testXmlReadFileThree.Close();
                    }

                    nClick = -1;
                }

                if (nClick == 1)
                {
                    XmlTextReader xmlHttpLoad = new XmlTextReader(@"http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");
                    Show(xmlHttpLoad, listTest, listBasic, listCurency);
                    xmlHttpLoad.Close();
                }
            }
        }
    }
}
