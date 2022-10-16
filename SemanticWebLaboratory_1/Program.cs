using System;
using AngleSharp;
using AngleSharp.Dom;
using System.Xml.Schema;
using System.Threading.Tasks;
using System.Collections.Generic;
using SemanticWebXmlLaboratory.Core;
using System.Globalization;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Threading;
using System.Xml;

namespace SemanticWebLaboratory
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(configuration);

            var nameSelector = "h1";
            var stateSelector = "div.status-text-available";
            var propertySelector = "property-content";
            var xmlPath = "Products.xml";
            var filePath = "Products.txt";

            string[] tobaccoUrls =
            {
                "https://duman.com.ua/product/tabak-420-light-ays-grusha-250g",
                "https://duman.com.ua/product/tabak-buta-black-grape-buta-blek-chernyy-vinograd-black-line-100g",
                "https://duman.com.ua/product/tabak-samsvaril-kola-banka-100g",
                "https://duman.com.ua/product/tabak-420-waffle-420-vafli",
                "https://duman.com.ua/product/tabak-420-red-currant-420-krasnaya-smorodina",
                "https://duman.com.ua/product/tabak-dead-horse-eclair-ded-hors-ekler-200g",
                "https://duman.com.ua/product/tabak-jibiar-tsunami-dzhibiar-tsunami-led-myata-lenedtsy-50g",
                "https://duman.com.ua/product/tabak-jibiar-orange-cream-dzhibiar-apelsinovyy-krem-50g-potekshaya-utsenka",
                "https://duman.com.ua/product/bestabachnaya-smes-banshee-licorice-soda-banshi-lakrichnaya-sodovaya-dark",
                "https://duman.com.ua/product/tabak-smoke-mafia-cola-smouk-mafiya-kola-na-ves-20g",
            };




            XmlSchema schema = new XmlSchema();

            schema.Items.Add(new XmlSchemaElement { Name = "transaction", SchemaTypeName = new XmlQualifiedName("transactionType") });

            XmlSchemaComplexType complexType = new XmlSchemaComplexType { Name = "transactionType" };
            complexType.Attributes.Add(new XmlSchemaAttribute() { Name = "borrowDate", SchemaTypeName = new XmlQualifiedName("xs:date") });

            XmlSchemaSequence sequacne = new XmlSchemaSequence();
            sequacne.Items.Add(new XmlSchemaElement { Name = "Lender", SchemaTypeName = new XmlQualifiedName("address") });
            sequacne.Items.Add(new XmlSchemaElement { Name = "Borrower", SchemaTypeName = new XmlQualifiedName("address") });
            sequacne.Items.Add(new XmlSchemaElement { Name = "Books", SchemaTypeName = new XmlQualifiedName("books") });
            complexType.Particle = sequacne;
            schema.Items.Add(complexType);

            XmlSchemaElement newElement = new XmlSchemaElement { Name = "note", SchemaTypeName = new XmlQualifiedName("xs:string") };
            schema.Items.Add(newElement);

            XmlSchemaComplexType address = new XmlSchemaComplexType { Name = "address" };
            address.Attributes.Add(new XmlSchemaAttribute { Name = "phone", SchemaTypeName = new XmlQualifiedName("xs:string"), Use = XmlSchemaUse.Optional });
            XmlSchemaSequence addressSequence = new XmlSchemaSequence();
            addressSequence.Items.Add(new XmlSchemaElement { Name = "name", SchemaTypeName = new XmlQualifiedName("xs:string") });
            addressSequence.Items.Add(new XmlSchemaElement { Name = "street", SchemaTypeName = new XmlQualifiedName("xs:string") });
            addressSequence.Items.Add(new XmlSchemaElement { Name = "city", SchemaTypeName = new XmlQualifiedName("xs:string") });
            addressSequence.Items.Add(new XmlSchemaElement { Name = "state", SchemaTypeName = new XmlQualifiedName("xs:NMTOKEN") });
            address.Particle = sequacne;
            schema.Items.Add(address);







            /*foreach (var url in tobaccoUrls)
            {
                using (var document = await context.OpenAsync(url))
                {
                    var product = SemanticWebXmlLaboratory.Parser.ParseIntoTobacco(document, nameSelector, stateSelector, propertySelector);
                    Repository<Tobacco>.Add(product);
                }
            }

            foreach (var tobacco in Repository<Tobacco>.Items)
            {
                Console.WriteLine(tobacco);
            }

            var writingResult = WriteProductsDataIntoFile(filePath, Repository<Tobacco>.Items);
            var xmlWritingResult = SaveProductsDataAsXml(xmlPath, Repository<Tobacco>.Items);


            Console.WriteLine("\nРезультат записи в текстовый файл: " + writingResult);
            Console.WriteLine("Результат записис в xml: " + xmlWritingResult);*/


           


        }
        public static bool SaveProductsDataAsXml(string path, List<Tobacco> tobaccos)
        {
            var root = new XElement("Tobaccos");
            var xmlDocument = new XDocument(root);

            foreach (var tobacco in tobaccos)
            {
                var xElement = new XElement(nameof(Tobacco));
                xElement.Add(
                    new XElement(nameof(tobacco.Price), tobacco.Price),
                    new XElement(nameof(tobacco.WeightGM), tobacco.WeightGM),
                    new XElement(nameof(tobacco.Strength), tobacco.Strength),
                    new XElement(nameof(tobacco.Country), tobacco.Country),
                    new XElement(nameof(tobacco.Manufacturer), tobacco.Manufacturer),
                    new XElement(nameof(tobacco.Leaf), tobacco.Leaf),
                    new XElement(nameof(tobacco.isAvailable), tobacco.isAvailable));
                xElement.Add(new XAttribute(nameof(tobacco.Name), tobacco.Name));

                root.Add(xElement);
            }


            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                xmlDocument.Save(stream);
            }

            return File.Exists(path);
        }
        public static bool WriteProductsDataIntoFile(string path, List<Tobacco> tobaccos)
        {
            using (var stream = File.CreateText(path))
            {
                foreach (var tobacco in tobaccos)
                {
                    stream.WriteLine(tobacco.Name);
                    stream.WriteLine(tobacco.Price);
                    stream.WriteLine(tobacco.isAvailable);
                    stream.WriteLine(tobacco.Country);
                    stream.WriteLine(tobacco.Leaf);
                    stream.WriteLine(tobacco.Manufacturer);
                    stream.WriteLine(tobacco.Strength);
                    stream.WriteLine(tobacco.WeightGM);
                    stream.WriteLine("\n");
                }
            }

            return File.Exists(path);
        }
    } 
}
