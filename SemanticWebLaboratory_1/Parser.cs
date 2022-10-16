using AngleSharp.Dom;
using SemanticWebXmlLaboratory.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SemanticWebXmlLaboratory
{
    internal static class Parser
    {
        private static readonly StringBuilder stringBuilder;
        private static readonly Random random;
        static Parser()
        {
            stringBuilder = new StringBuilder();
            random = new Random();
        }

        public static Tobacco ParseIntoTobacco(IDocument doucment, string nameSelector, string statusSelector, string propertyClass)
        {
            Tobacco tobacco = new Tobacco();

            var properies = ValidateElements(doucment.GetElementsByClassName(propertyClass));

            tobacco.Name = RecivedStringValidOrEmpty(doucment.QuerySelector(nameSelector));

            tobacco.isAvailable = ParseStringFieldIntoBoolean(RecivedStringValidOrEmpty(doucment.QuerySelector(statusSelector)));

            tobacco.Price = random.Next(100, 470);

            tobacco.WeightGM = ParseStringFieldIntoNumeric(properies[0]);
            tobacco.Country = properies[1];
            tobacco.Strength = properies[2];
            tobacco.Manufacturer = properies[3];
            tobacco.Leaf = properies[4];

            return tobacco;
        }

        private static string RecivedStringValidOrEmpty(IElement element)
        {
            return element != null ? element.TextContent.Trim() : string.Empty;
        }
        private static List<string> ValidateElements(IHtmlCollection<IElement> elements)
        {
            List<string> validElements = new List<string>();
            for (int i = 0; i < elements.Length; i++)
            {
                validElements.Add(elements[i] != null ? elements[i].TextContent.Trim() : string.Empty);
            }
            return validElements;
        }
        private static double ParseStringFieldIntoNumeric(string field)
        {
            stringBuilder.Clear();

            for (int i = 0; i < field.Length; i++)
            {
                if (char.IsDigit(field[i]))
                {
                    stringBuilder.Append(field[i]);
                }
            }

            double.TryParse(stringBuilder.ToString(), out double parsedField);

            return parsedField;
        }
        private static bool ParseStringFieldIntoBoolean(string field)
        {
            if (field == null)
            {
                return false;
            }
            string state = "В наличии";

            return field == state ? true : false;
        }
    }
}
