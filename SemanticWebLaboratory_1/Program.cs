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

            var xmlPath = @".\ProductsDirectory\Products.xml";
            var filePath = @".\ProductsDirectory\Products.txt";
            var xmlSchemaPath = @".\ProductsDirectory\Products.xsd";

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

            foreach (var url in tobaccoUrls)
            {
                using (var document = await context.OpenAsync(url))
                {
                    var product = SemanticWebXmlLaboratory.Parser.ParseIntoTobacco(document, nameSelector, stateSelector, propertySelector);
                    Repository<Tobacco>.Add(product);
                }
            }


            var writingResult = WriteProductsDataIntoFile(filePath, Repository<Tobacco>.Items);
            Console.WriteLine("Result of writing data into text file: " + writingResult);

            var xmlDocument = GenerateProductXml(Repository<Tobacco>.Items);
            var generatingXmlDocumentResult = SaveXmlDocument(xmlPath, xmlDocument);
            Console.WriteLine("Result of generating XML Document: " + generatingXmlDocumentResult);;

            var xmlSchema = GenerateXmlSchema();
            var generatingXmlSchemaResult = SaveXmlSchema(xmlSchemaPath, xmlSchema);
            Console.WriteLine("Result of generating XML Shema: " + generatingXmlSchemaResult);


            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
            schemaSet.Add(xmlSchema);
            schemaSet.Compile();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.Schemas = schemaSet;

            XmlReader xmlReader = XmlReader.Create(xmlPath, xmlReaderSettings);
            while (xmlReader.Read());
            Console.WriteLine("Document is valid!");

        }

        private static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            Console.WriteLine(args.Message);
        }

        private static XmlSchema GenerateXmlSchema()
        {
            var xmlNamespace = "http://www.w3.org/2001/XMLSchema";
            var modelNamespace = "https://duman.com.ua/0";

            var tobaccosRootElement = new XmlSchemaElement();
            tobaccosRootElement.Name = "Tobaccos";

            var tobaccoNameSimpleType = new XmlSchemaSimpleType();
            tobaccoNameSimpleType.Name = "TobaccoName";
            var tobaccoNameSimpleTypeRestriction = new XmlSchemaSimpleTypeRestriction();
            tobaccoNameSimpleTypeRestriction.BaseTypeName = new XmlQualifiedName("string", xmlNamespace);
            var facet = new XmlSchemaMaxLengthFacet();
            facet.Value = "1000";
            tobaccoNameSimpleTypeRestriction.Facets.Add(facet);
            tobaccoNameSimpleType.Content = tobaccoNameSimpleTypeRestriction;


            var nameAttribute = new XmlSchemaAttribute();
            nameAttribute.SchemaTypeName = new XmlQualifiedName("TobaccoName", modelNamespace);
            nameAttribute.Use = XmlSchemaUse.Required;
            nameAttribute.Name = "Name";

            var priceElement = new XmlSchemaElement();
            priceElement.SchemaTypeName = new XmlQualifiedName("decimal", xmlNamespace);
            priceElement.Name = "Price";

            var weightGMElement = new XmlSchemaElement();
            weightGMElement.SchemaTypeName = new XmlQualifiedName("short", xmlNamespace);
            weightGMElement.Name = "WeightGM";

            var strengthElement = new XmlSchemaElement();
            strengthElement.SchemaTypeName = new XmlQualifiedName("string", xmlNamespace);
            strengthElement.Name = "Strength";

            var countryElement = new XmlSchemaElement();
            countryElement.SchemaTypeName = new XmlQualifiedName("string", xmlNamespace);
            countryElement.Name = "Country";

            var manufacturerElement = new XmlSchemaElement();
            manufacturerElement.SchemaTypeName = new XmlQualifiedName("string", xmlNamespace);
            manufacturerElement.Name = "Manufacturer";

            var leafElement = new XmlSchemaElement();
            leafElement.SchemaTypeName = new XmlQualifiedName("string", xmlNamespace);
            leafElement.Name = "Leaf";

            var isAvailableElement = new XmlSchemaElement();
            isAvailableElement.SchemaTypeName = new XmlQualifiedName("boolean", xmlNamespace);
            isAvailableElement.Name = "IsAvailable";

            var tobaccoComplexType = new XmlSchemaComplexType();
            var tobaccoElementSequence = new XmlSchemaSequence();

            tobaccoElementSequence.Items.Add(priceElement);
            tobaccoElementSequence.Items.Add(weightGMElement);
            tobaccoElementSequence.Items.Add(strengthElement);
            tobaccoElementSequence.Items.Add(countryElement);
            tobaccoElementSequence.Items.Add(manufacturerElement);
            tobaccoElementSequence.Items.Add(leafElement);
            tobaccoElementSequence.Items.Add(isAvailableElement);

            tobaccoComplexType.Particle = tobaccoElementSequence;
            tobaccoComplexType.Attributes.Add(nameAttribute);

            var tobaccoElement = new XmlSchemaElement();
            tobaccoElement.Name = "Tobacco";
            tobaccoElement.MinOccurs = 0;
            tobaccoElement.MaxOccurs = 200;
            tobaccoElement.SchemaType = tobaccoComplexType;

            var tobaccosRootElementComplexType = new XmlSchemaComplexType();
            var tobaccosRootElementSequence = new XmlSchemaSequence();

            tobaccosRootElementSequence.Items.Add(tobaccoElement);
            tobaccosRootElementComplexType.Particle = tobaccosRootElementSequence;

            tobaccosRootElement.SchemaType = tobaccosRootElementComplexType;

            var xmlSchema = new XmlSchema();
            xmlSchema.TargetNamespace = modelNamespace;
            xmlSchema.Items.Add(tobaccosRootElement);
            xmlSchema.Items.Add(tobaccoNameSimpleType);
            xmlSchema.AttributeFormDefault = XmlSchemaForm.Unqualified;
            xmlSchema.ElementFormDefault = XmlSchemaForm.Unqualified;

            return xmlSchema;
        }
       
        private static XDocument GenerateProductXml(List<Tobacco> tobaccos)
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
                    new XElement(nameof(tobacco.IsAvailable), tobacco.IsAvailable));
                xElement.Add(new XAttribute(nameof(tobacco.Name), tobacco.Name));

                root.Add(xElement);
            }

            return xmlDocument;
        }
        private static bool SaveXmlDocument(string path, XDocument xmlDocument)
        {
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                xmlDocument.Save(stream);
            }

            return File.Exists(path);
        }
        private static bool SaveXmlSchema(string path, XmlSchema xmlSchema)
        {
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                xmlSchema.Write(stream);
            }

            return File.Exists(path);
        }
        private static bool WriteProductsDataIntoFile(string path, List<Tobacco> tobaccos)
        {
            using (var stream = File.CreateText(path))
            {
                foreach (var tobacco in tobaccos)
                {
                    stream.WriteLine(tobacco.Name);
                    stream.WriteLine(tobacco.Price);
                    stream.WriteLine(tobacco.IsAvailable);
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
