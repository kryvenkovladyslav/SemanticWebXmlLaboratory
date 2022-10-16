using System;
using System.Text;

namespace SemanticWebXmlLaboratory.Core
{
    [Serializable]
    public sealed class Tobacco
    {
        
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double WeightGM { get; set; }
        public string Strength { get; set; }
        public string Country { get; set; }
        public string Manufacturer { get; set; }
        public string Leaf { get; set; }
        public bool IsAvailable { get; set; }

        public Tobacco() { }
        
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder
                .Append("Name: " + Name + "\n")
                .Append("Price: " + Price + "\n")
                .Append("IsAvailable: " + IsAvailable + "\n")
                .Append("Weight (gm): " + WeightGM + "\n")
                .Append("Strength: " + Strength + "\n")
                .Append("Leaf: " + Leaf + "\n")
                .Append("Country: " + Country + "\n")
                .Append("Manufacturer: " + Manufacturer);

            return stringBuilder.ToString();
        }
    }
}
